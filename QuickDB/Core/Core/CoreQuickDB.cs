using QuickDB.Core.Models;
using QuickDB.Core.QuickDBException;
using QuickDB.Dependencies.Contracts;
using QuickDB.Dependencies.Encryptions;
using QuickDB.Dependencies.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickDB.Core.Core
{
    public abstract class CoreQuickDB<TClientModelObject> where TClientModelObject : new()
    {
        public int MaximumUpdateHistoryDepth { set; get; }

        #region Ctor & Dependencies

        public bool ReadOnly { set; get; }

        protected AbstractEncryptionHandler EncryptionHandler { set; get; }

        protected AbstractSerializationHandler SerializationHandler { set; get; }

        protected AbstractFileAccessHandler FileAccessHandler { set; get; }

        protected bool EnableEncryption { set; get; }

        public void VerifyQuickDBDependencySetUp(QuickDBDependencySetUpObject quickDBDependencySetUpObject)
        {
            if (quickDBDependencySetUpObject == null) throw new ArgumentNullException();
            if (quickDBDependencySetUpObject.EncryptionHandler == null) throw new ArgumentNullException();
            if (quickDBDependencySetUpObject.AbstractSerializationHandler == null) throw new ArgumentNullException();
            if (quickDBDependencySetUpObject.FileAccessHandler == null) throw new ArgumentNullException();
        }

        protected CoreQuickDB(QuickDBDependencySetUpObject quickDBDependencySetUpObject, bool readOnly = false)
        {
            MaximumUpdateHistoryDepth = 100;
            VerifyQuickDBDependencySetUp(quickDBDependencySetUpObject);
            ReadOnly = readOnly;
            EnableEncryption = quickDBDependencySetUpObject.EnableEncryption;
            EncryptionHandler = quickDBDependencySetUpObject.EncryptionHandler;
            SerializationHandler = quickDBDependencySetUpObject.AbstractSerializationHandler;
            FileAccessHandler = quickDBDependencySetUpObject.FileAccessHandler;
        }

        #endregion Ctor & Dependencies

        #region private methods

        #endregion private methods

        #region Main Functions

        protected ADocumentObject<TClientModelObject> _Deserialize(string stringContentInConfigFile)
        {
            return SerializationHandler.DeSerialize<ADocumentObject<TClientModelObject>>(stringContentInConfigFile);
        }

        protected ADocumentObject<TClientModelObject> _DecryptAndDeserialize(string stringContentInConfigFile)
        {
            return _Deserialize(EncryptionHandler.DeCrypt(stringContentInConfigFile));
        }

        protected ADocumentObject<TClientModelObject> _TryDeserializeStrategies(string stringContentInConfigFile)
        {
            var tmpStringContent = stringContentInConfigFile;
            try
            {
                if (EnableEncryption)
                    return _DecryptAndDeserialize(tmpStringContent);

                return _Deserialize(tmpStringContent);
            }
            catch (Exception)
            {
                try
                {
                    return _Deserialize(tmpStringContent);
                }
                catch (Exception)
                {
                    return _DecryptAndDeserialize(tmpStringContent);
                }
            }
        }

        protected void _SaveRaw(string json)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();
            if (EnableEncryption)
                json = EncryptionHandler.EnCrypt(json);

            FileAccessHandler.Save(json);
        }

        protected void _Delete(ADocumentObject<TClientModelObject> updatedDocument)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();
            try
            {
                _TrySave(() =>
                {
                    updatedDocument.State = DocumentState.Deleted;
                    return updatedDocument;
                });
            }
            catch (Exception)
            {
                throw new QuickDBErrorDeletingDocumentException();
            }
        }

        protected void _DeleteDocumentPermanently(ADocumentObject<TClientModelObject> updatedDocument)
        {
           if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();
            var permanentDeleteException = new QuickDBPermanentDocumentDeleteException();
            try
            {
                _Delete(updatedDocument);
                FileAccessHandler.Delete();

                if (FileAccessHandler.Exists())
                    throw permanentDeleteException;
            }
            catch (Exception e)
            {
                if (e is QuickDBErrorDeletingDocumentException)
                    throw;

                throw permanentDeleteException;
            }
        }

        #endregion Main Functions

        #region Critical Points

        private bool _handleCheckIfEtagHasChangedSinceLastLoad(ADocumentObject<TClientModelObject> updatedDocument, ADocumentObject<TClientModelObject> currentDocument, bool throwExceptionIfThereAreChangesSinceLastLoad)
        {
            var etagHasChangedSinceLastLoad = (currentDocument != null && currentDocument.ETag != updatedDocument.ETag) || (currentDocument == null);

            if (throwExceptionIfThereAreChangesSinceLastLoad && etagHasChangedSinceLastLoad)
            {
                throw new QuickDBUnSafeDocumentWriteException("Document has been modified since last load with . Please reload the document before trying to save");
            }

            return etagHasChangedSinceLastLoad;
        }

        protected string _CreateNewDocument(bool makeDocumentReadOnly = false)
        {
            if (FileAccessHandler.Exists())
            {
                throw new QuickDBTryingToCreateAnAlreadyExistingDocumentException();
            }
            FileAccessHandler.CreateRequiredDirectoryIfItDoesntAlreadyExist();
            FileAccessHandler.CreateRequiredFileIfItDoesntAlreadyExist();

            var docId = Guid.NewGuid();
            var doc = _UpdateDocumentPropertiesAndSaveToDisc(new ADocumentObject<TClientModelObject>(docId), 0, DateTime.UtcNow, makeDocumentReadOnly);
            return doc;
        }

        protected string _LoadRaw(bool decrypt = false, bool createIfItDoesNotExist = false)
        {
            if (createIfItDoesNotExist)
            {
                _CreateNewDocument();
            }

            if (!FileAccessHandler.Exists())
            {
                throw new QuickDBTryingToReadNonExistentDocumentException();
            }

            var result = FileAccessHandler.Read();

            if (string.IsNullOrEmpty(result))
            {
                throw new NotImplementedException();
            }

            if (decrypt)
            {
                result = EncryptionHandler.DeCrypt(result);
            }

            return result;
        }

        #endregion Critical Points

        #region Document Updates methods

        protected void _TrySave(Func<ADocumentObject<TClientModelObject>> updateDocumentActionToBeRetried, bool makeReadOnly = false, int numberOfRetries = 0, int intervalBetweenRetries = 100,Guid? transactionOwned=null)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException(" 'ReadOnly' is True and prevents document from being updated");
            if (numberOfRetries < 0) throw new ArgumentException("numberOfRetries cannot be negative");

            var retryCount = 0;
            var saved = false;
            while ((retryCount <= numberOfRetries))
            {
                try
                {
                    var data = updateDocumentActionToBeRetried.Invoke();

                    _Save(data, makeReadOnly, true, transactionOwned);
                    saved = true;
                    break;
                }
                catch (Exception e)
                {
                    if (!(e is QuickDBUnSafeDocumentWriteException))
                        if (!(e is QuickDBTryingToSaveADocumentThatIsAlreadyInATransactionException))
                        throw e;

                    System.Threading.Thread.Sleep(intervalBetweenRetries);
                    retryCount++;
                }
            }
            if (!saved)
                throw new QuickDBMaximumNumberOfRetrrDocumentWriteExceededException("Number of retries is " + numberOfRetries);
        }

        private void _UpdateDocumentHistory(ADocumentObject<TClientModelObject> updatedDocument, ADocumentObject<TClientModelObject> currentDocument, DateTime stamp)
        {
            if (updatedDocument.ReadOnlyDocument)
                throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            if (ReadOnly)
                throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            if (currentDocument != null)
            {
                updatedDocument.UpdateHistory = currentDocument.UpdateHistory ?? new List<ADocumentObject<TClientModelObject>>();

                if (currentDocument.UpdateHistory != null && currentDocument.UpdateHistory.Count > MaximumUpdateHistoryDepth)
                {
                    currentDocument.UpdateHistory = currentDocument.UpdateHistory.OrderBy(x => x.Timestamp)
                        .Skip(currentDocument.UpdateHistory.Count + 1 - MaximumUpdateHistoryDepth)
                        .Take(MaximumUpdateHistoryDepth)
                        .ToList();
                }

                updatedDocument.UpdateHistory.Add(new ADocumentObject<TClientModelObject>()
                {
                    Document = currentDocument.Document,
                    DocumentName = currentDocument.DocumentName,
                    ETag = currentDocument.ETag,
                    DateCreated = currentDocument.DateCreated,
                    Timestamp = stamp,
                    State = currentDocument.State,
                    Version = currentDocument.Version
                });
            }
        }

        private void _UpdateDocumentProperties(ADocumentObject<TClientModelObject> updatedDocument, long version, DateTime stamp, bool makeReadOnly = false)
        {
            if (updatedDocument.ReadOnlyDocument)
            {
                throw new QuickDBTryingToWriteToAReadOnlyDocumentException();
            }

            if (ReadOnly)
            {
                throw new QuickDBTryingToWriteToAReadOnlyDocumentException();
            }
            if (updatedDocument.ID == null)
            {
                throw new NotImplementedException();
            }

            updatedDocument.ReadOnlyDocument = makeReadOnly;
            updatedDocument.ETag = Guid.NewGuid();
            updatedDocument.Timestamp = stamp;
            updatedDocument.DateCreated = stamp;
            updatedDocument.Version = version;
            updatedDocument.State = updatedDocument.State == DocumentState.Unknown ? DocumentState.Valid : updatedDocument.State;
            updatedDocument.DocumentName = typeof(TClientModelObject).Name;
        }

        #endregion Document Updates methods

        #region Main Load and save

        protected ADocumentObject<TClientModelObject> _Load(string raw = null, bool createIfItDoesNotExist = false,Guid? transactionOwned=null)
        {
            try
            {
                if (raw == null)
                {
                    raw = _LoadRaw(decrypt: false, createIfItDoesNotExist: createIfItDoesNotExist);
                }
                var result = _TryDeserializeStrategies(raw);
    var foundEmptyExistingFile = (result.Document == null);

                result.Document = foundEmptyExistingFile ? new TClientModelObject() : result.Document;
                if (result.ID == null)
                {
                    throw new QuickDBInvalidDocumentException();
                }

                if (result.State == DocumentState.Deleted)
                {
                    throw  new QuickDBTryingToLoadDeletedDocumentException();
                }

            

                if(  (transactionOwned != null)&&raw!=null)
                {
                    throw new Exception("cannot do a transaction when default raw data is supplied");
                }

                //if doc has been set with transaction
                if (result.TransactionId != null)
                {
                    if (result.TransactionId != transactionOwned)
                    {
                        throw new QuickDBTryingToEnterIntoTransactionWithADocumentAlreadyInAnExistingTransactionException();
                    }
                }
                else // if no existing transaction on doc
                {
                    // and if a transaction is requested
                    if (transactionOwned == null) return result;
                    // try to enter into a transaction
                    try
                    {
                        var result1 = result;
                        result1.TransactionId = transactionOwned;
                        _TrySave(() => result1,false,100,100, transactionOwned);
                    }
                    catch (Exception)
                    {
                        throw new QuickDBUnableToLoadParticipantOfATransactionException();
                    }
                    try
                    {
                         result = _Load();
                    }
                    catch (Exception)
                    {
                        // this file should be locked down for recovery
                        throw new QuickDBFatalAndDataIntegritySeriousException();
                    }
                   
                }



                return result;
            }
            catch (Exception e)
            {
                if (e is QuickDBTryingToLoadDeletedDocumentException)
                    throw;

                if (e is QuickDBTryingToReadNonExistentDocumentException)
                    throw;

                if (e is QuickDBTryingToCreateAnAlreadyExistingDocumentException)
                    throw;

                return null;
            }
        }

        protected string _Save(ADocumentObject<TClientModelObject> updatedDocument, bool makeReadOnly = false, bool throwExceptionIfThereAreChangesSinceLastLoad = false,Guid? transactionOwned=null)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();
            var stamp = DateTime.UtcNow;

            var currentDocument = _Load();

            if (currentDocument.TransactionId != null)
            {
                if (transactionOwned!=currentDocument.TransactionId)
                {
                    throw new QuickDBTryingToSaveADocumentThatIsAlreadyInATransactionException();
                }
            }
           

            _handleCheckIfEtagHasChangedSinceLastLoad(updatedDocument, currentDocument, throwExceptionIfThereAreChangesSinceLastLoad);

            _UpdateDocumentHistory(updatedDocument, currentDocument, stamp);

            var version = currentDocument.Version + 1;

            return _UpdateDocumentPropertiesAndSaveToDisc(updatedDocument, version, stamp, makeReadOnly);
        }

        private string _UpdateDocumentPropertiesAndSaveToDisc(ADocumentObject<TClientModelObject> updatedDocument, long version, DateTime stamp, bool makeReadOnly = false)
        {
            if (updatedDocument.ReadOnlyDocument)
                throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            _UpdateDocumentProperties(updatedDocument, version, stamp, makeReadOnly);

            updatedDocument.Document = updatedDocument.Document.Equals(null) ? new TClientModelObject() : updatedDocument.Document;
            var json = SerializationHandler.Serialize(updatedDocument);

            if (EnableEncryption)
                json = EncryptionHandler.EnCrypt(json);

            FileAccessHandler.Save(json);

            return json;
        }

        #endregion Main Load and save
    }
}