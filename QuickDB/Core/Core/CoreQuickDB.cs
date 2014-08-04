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
    public abstract class CoreQuickDB<TConfigurationObject> : IQuickDBSession<ADocumentObject<TConfigurationObject>> where TConfigurationObject : new()
    {
        #region

        protected AbstractEncryptionHandler EncryptionHandler { set; get; }

        protected AbstractSerializationHandler SerializationHandler { set; get; }

        protected AbstractFileAccessHandler FileAccessHandler { set; get; }

        protected bool EnableEncryption { set; get; }

        private void UpdateDocumentHistory(ADocumentObject<TConfigurationObject> updatedDocument, ADocumentObject<TConfigurationObject> currentDocument, int maxHistoryDepth, DateTime stamp)
        {
            if (currentDocument != null)
            {
                updatedDocument.UpdateHistory = currentDocument.UpdateHistory ?? new List<ADocumentObject<TConfigurationObject>>();

                if (currentDocument.UpdateHistory != null && currentDocument.UpdateHistory.Count > maxHistoryDepth)
                {
                    currentDocument.UpdateHistory = currentDocument.UpdateHistory.OrderBy(x => x.Timestamp)
                        .Skip(currentDocument.UpdateHistory.Count + 1 - maxHistoryDepth)
                        .Take(maxHistoryDepth)
                        .ToList();
                }

                updatedDocument.UpdateHistory.Add(new ADocumentObject<TConfigurationObject>()
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

        private bool EtagHasChangedSinceLastLoad(ADocumentObject<TConfigurationObject> updatedDocument, ADocumentObject<TConfigurationObject> currentDocument)
        {
            return (currentDocument != null && currentDocument.ETag != updatedDocument.ETag) || (currentDocument == null);
        }

        private void UpdateDocumentProperties(
           ADocumentObject<TConfigurationObject> updatedDocument, long version, DateTime stamp)
        {
            updatedDocument.ETag = Guid.NewGuid();
            updatedDocument.Timestamp = stamp;
            updatedDocument.DateCreated = stamp;
            updatedDocument.Version = version;
            updatedDocument.State = updatedDocument.State == DocumentState.Unknown ? DocumentState.Valid : updatedDocument.State;
            updatedDocument.DocumentName = typeof(TConfigurationObject).Name;
        }

        protected ADocumentObject<TConfigurationObject> Deserialize(string stringContentInConfigFile)
        {
            return SerializationHandler.DeSerialize<ADocumentObject<TConfigurationObject>>(stringContentInConfigFile);
        }

        protected ADocumentObject<TConfigurationObject> DecryptAndDeserialize(string stringContentInConfigFile)
        {
            return Deserialize(EncryptionHandler.DeCrypt(stringContentInConfigFile));
        }

        protected ADocumentObject<TConfigurationObject> TryDeserializeStrategies(string stringContentInConfigFile)
        {
            var tmpStringContent = stringContentInConfigFile;
            try
            {
                if (EnableEncryption)
                    return DecryptAndDeserialize(tmpStringContent);

                return Deserialize(tmpStringContent);
            }
            catch (Exception)
            {
                try
                {
                    return Deserialize(tmpStringContent);
                }
                catch (Exception)
                {
                    return DecryptAndDeserialize(tmpStringContent);
                }
            }
        }

        protected string _LoadRaw(bool decrypt = false, bool createIfItDoesNotExist = false)
        {
            if (createIfItDoesNotExist)
            {
                if (FileAccessHandler.Exists())
                {
                    throw new QuickDBTryingToCreateAnAlreadyExistingDocumentException();
                }
                FileAccessHandler.CreateRequiredDirectoryIfItDoesntAlreadyExist();
                FileAccessHandler.CreateRequiredFileIfItDoesntAlreadyExist();
            }

            if (!FileAccessHandler.Exists())
                throw new QuickDBTryingToReadNonExistentDocumentException();

            var result = FileAccessHandler.Read();

            if (string.IsNullOrEmpty(result))
            {
                result = UpdateDocumentPropertiesAndSaveToDisc(new ADocumentObject<TConfigurationObject>(), 0, DateTime.UtcNow);
            }

            if (decrypt)
                result = EncryptionHandler.DeCrypt(result);
            return result;
        }

        protected ADocumentObject<TConfigurationObject> _Load(string raw = null, bool createIfItDoesNotExist = false)
        {
            try
            {
                var result = TryDeserializeStrategies(raw ?? _LoadRaw(false, createIfItDoesNotExist));
                result.Document = result.Document == null ? new TConfigurationObject() : result.Document;

                return result;
            }
            catch (Exception e)
            {
                if (e is QuickDBTryingToReadNonExistentDocumentException)
                    throw;

                if (e is QuickDBTryingToCreateAnAlreadyExistingDocumentException)
                    throw;

                return null;
            }
        }

        protected string _Save(ADocumentObject<TConfigurationObject> updatedDocument, bool throwExceptionIfThereAreChangesSinceLastLoad = false, int maxHistoryDepth = 100)
        {
            FileAccessHandler.CreateRequiredDirectoryIfItDoesntAlreadyExist();
            FileAccessHandler.CreateRequiredFileIfItDoesntAlreadyExist();

            var raw = _LoadRaw();
            var currentDocument = _Load(raw);
            var etagHasChangedSinceLastLoad = EtagHasChangedSinceLastLoad(updatedDocument, currentDocument);

            if (throwExceptionIfThereAreChangesSinceLastLoad && etagHasChangedSinceLastLoad)
            {
                throw new QuickDBUnSafeDocumentWriteException("Document has been modified since last load with . Please reload the document before trying to save");
            }
            var stamp = DateTime.UtcNow;

            UpdateDocumentHistory(updatedDocument, currentDocument, maxHistoryDepth, stamp);

            long version;
            try
            {
                version = currentDocument.Version + 1;
            }
            catch (Exception)
            {
                version = 0;
            }
            return UpdateDocumentPropertiesAndSaveToDisc(updatedDocument, version, stamp);
        }

        private string UpdateDocumentPropertiesAndSaveToDisc(
           ADocumentObject<TConfigurationObject> updatedDocument, long version, DateTime stamp)
        {
            UpdateDocumentProperties(updatedDocument, version, stamp);

            updatedDocument.Document = updatedDocument.Document.Equals(null) ? new TConfigurationObject() : updatedDocument.Document;
            var json = SerializationHandler.Serialize(updatedDocument);

            if (EnableEncryption)
                json = EncryptionHandler.EnCrypt(json);

            FileAccessHandler.Save(json);

            return json;
        }

        protected void _SaveRaw(string json)
        {
            if (EnableEncryption)
                json = EncryptionHandler.EnCrypt(json);

            FileAccessHandler.Save(json);
        }

        protected void _TrySave(Func<ADocumentObject<TConfigurationObject>> updateDocumentActionToBeRetried, int numberOfRetries = 0, int intervalBetweenRetries = 100)
        {
            if (numberOfRetries < 0) throw new ArgumentException("numberOfRetries cannot be negative");

            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException(" 'ReadOnly' is True and prevents document from being updated");

            var retryCount = 0;
            bool saved = false;
            while ((retryCount <= numberOfRetries))
            {
                try
                {
                    var data = updateDocumentActionToBeRetried.Invoke();
                    _Save(data, true);
                    saved = true;
                    break;
                }
                catch (Exception e)
                {
                    if (!(e is QuickDBUnSafeDocumentWriteException)) throw e;
                    System.Threading.Thread.Sleep(intervalBetweenRetries);
                    retryCount++;
                }
            }
            if (!saved)
                throw new QuickDBMaximumNumberOfRetrrDocumentWriteExceededException("Number of retries is " + numberOfRetries);
        }

        protected void _Delete(ADocumentObject<TConfigurationObject> updatedDocument)
        {
            try
            {
                _TrySave(() =>
                {
                    updatedDocument.State = DocumentState.Deleted;
                    return updatedDocument;
                }, 10);
            }
            catch (Exception)
            {
                throw new QuickDBErrorDeletingDocumentException();
            }
        }

        protected void _DeleteDocumentPermanently(ADocumentObject<TConfigurationObject> updatedDocument)
        {
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

        #endregion
        #region Ctor

        public void VerifyQuickDBDependencySetUp(QuickDBDependencySetUpObject QuickDBDependencySetUpObject)
        {
            if (QuickDBDependencySetUpObject == null) throw new ArgumentNullException("QuickDBDependencySetUpObject");
            if (QuickDBDependencySetUpObject.EncryptionHandler == null) throw new ArgumentNullException("encryptionHandler");
            if (QuickDBDependencySetUpObject.AbstractSerializationHandler == null) throw new ArgumentNullException("abstractSerializationHandler");
            if (QuickDBDependencySetUpObject.FileAccessHandler == null) throw new ArgumentNullException("fileAccessHandler");
        }

        protected CoreQuickDB(QuickDBDependencySetUpObject QuickDBDependencySetUpObject, bool readOnly = false)
        {
            ReadOnly = readOnly;
            VerifyQuickDBDependencySetUp(QuickDBDependencySetUpObject);
            EnableEncryption = QuickDBDependencySetUpObject.EnableEncryption;
            EncryptionHandler = QuickDBDependencySetUpObject.EncryptionHandler;
            SerializationHandler = QuickDBDependencySetUpObject.AbstractSerializationHandler;
            FileAccessHandler = QuickDBDependencySetUpObject.FileAccessHandler;

            //FileAccessHandler.CreateRequiredDirectoryIfItDoesntAlreadyExist();
            //FileAccessHandler.CreateRequiredFileIfItDoesntAlreadyExist();
        }

        #endregion

        public string LoadRaw()
        {
            return SerializationHandler.Serialize(_Load());
        }

        public string LoadRawDocument()
        {
            return SerializationHandler.Serialize(_Load().Document);
        }

        public string LoadRawDocumentSettings()
        {
            var obj = _Load();
            obj.Document = default(TConfigurationObject);
            obj.UpdateHistory = null;
            return SerializationHandler.Serialize(obj);
        }

        public string LoadRawDocumentHistory()
        {
            return SerializationHandler.Serialize(_Load().UpdateHistory);
        }

        public ADocumentObject<TConfigurationObject> Load(bool createIfItDoesNotExist = false)
        {
            return _Load(null, createIfItDoesNotExist);
        }

        public void DeleteDocumentPermanently(ADocumentObject<TConfigurationObject> data)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            _DeleteDocumentPermanently(data);
        }

        public void Delete(ADocumentObject<TConfigurationObject> data)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            _Delete(data);
        }

        public void SaveRaw(string data)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            _SaveRaw(data);
        }

        public void Save(ADocumentObject<TConfigurationObject> data)
        {
            if (ReadOnly) throw new QuickDBTryingToWriteToAReadOnlyDocumentException();

            _Save(data);
        }

        public void TrySave(Func<ADocumentObject<TConfigurationObject>> updateDocumentActionToBeRetried, int numberOfRetries, int intervalBetweenRetries)
        {
            _TrySave(updateDocumentActionToBeRetried, numberOfRetries, intervalBetweenRetries);
        }

        public bool ReadOnly { set; get; }

        public bool ThrowExceptionIfThereAreChangesSinceLastLoad { set; get; }

        public int MaximumUpdateHistoryDepth { set; get; }
    }
}