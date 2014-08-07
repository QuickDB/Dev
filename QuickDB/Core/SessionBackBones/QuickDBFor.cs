using System;
using QuickDB.Core.Core;
using QuickDB.Core.Models;
using QuickDB.Dependencies.Contracts;
using QuickDB.DependencyInjector;
using QuickDB.Instances;

namespace QuickDB.Core.SessionBackBones
{
    //todo : create more overloads

    public class QuickDBFor<TDocumentObject> where TDocumentObject : new()
    {
        public bool IsNew { set; get; }
        public ADocumentObject<TDocumentObject> DocumentVersion { set; get; }
        protected string RawDocumentVersion { set; get; }
        protected string RawModelDocumentVersion { set; get; }



        public string RawModelData
        {
            get
            {
                RawDocumentVersion = ConfigHandle.LoadRawDocument();
                return RawDocumentVersion;
            }
        }


        public void TrySaveChanges(Func<ADocumentObject<TDocumentObject>> updateDocumentActionToBeRetried) 
        {
            ConfigHandle.TrySave(updateDocumentActionToBeRetried);
        }


        public bool ReadOnly { set; get; }

        public TDocumentObject Data
        {
            set
            {
               
                if (DocumentVersion == null)
                    DocumentVersion = new ADocumentObject<TDocumentObject>();

                DocumentVersion.Document = value;

                ConfigHandle.Save(DocumentVersion,ReadOnly);
            }
            get
            {
                DocumentVersion = ConfigHandle.Load(IsNew);
                return DocumentVersion.Document;
            }
        }



        public string RawData
        {
            set
            {

               
                RawDocumentVersion = value;

                ConfigHandle.SaveRaw(value);
            }
            get
            {
                RawDocumentVersion = ConfigHandle.LoadRaw();
                return RawDocumentVersion;
            }
        }



        public CoreFacade<TDocumentObject> ConfigHandle { set; get; }

        protected void QuickDBFactoryConstructor(CoreFacade<TDocumentObject> configHandle, string documentId, bool readOnly = false,
            bool enableEncryption = true, bool createIfItDoesNotExist = false)
        {

            IsNew = createIfItDoesNotExist;

            ReadOnly = readOnly;
            var fName = typeof(TDocumentObject).Name;

            if (configHandle == null)
            {
                configHandle = new QuickDB<TDocumentObject>(new QuickDBDependencySetUpObject()
                {
                    FileAccessHandler = QInjector.Inject<AbstractFileAccessHandler>(fName, documentId),
                    EncryptionHandler = QInjector.Inject<AbstractEncryptionHandler>(QInjector.Inject<ISecurityStrings>()),
                    AbstractSerializationHandler =QInjector.Inject<AbstractSerializationHandler>(),
                    EnableEncryption = enableEncryption
                }, ReadOnly);
            }

            ConfigHandle = configHandle;
        }


        public void DeleteDocumentPermanently()
        {
            ConfigHandle.DeleteDocumentPermanently(ConfigHandle.Load());
        }

        public QuickDBFor(CoreFacade<TDocumentObject> configHandle, string documentId = null, bool enableEncryption = true, bool readOnly = false, bool createIfItDoesNotExist = false)
        {
            QuickDBFactoryConstructor(configHandle, documentId, readOnly, enableEncryption, createIfItDoesNotExist);
        }

        public QuickDBFor(string documentId = null, bool enableEncryption = true, bool readOnly = false, bool createIfItDoesNotExist = false)
        {
            QuickDBFactoryConstructor(null, documentId, readOnly, enableEncryption, createIfItDoesNotExist);
        }
    }
}