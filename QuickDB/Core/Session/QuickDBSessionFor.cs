using QuickDB.Core.Models;
using QuickDB.Core.QuickDBException;
using QuickDB.Core.SessionBackBones;
using QuickDB.DependencyMapProvider;
using System;

namespace QuickDB.Core.Session
{
    public class QuickDBSessionFor<TClientModelObject> : IDisposable where TClientModelObject : new()
    {
        private static QuickDBSessionFor<TClientModelObject> CurrentSession { set; get; }

        public QuickDBSessionFor()
        {
            DependencyInjection.SetUpDefault();
            CurrentSession = this;
        }

        private QuickDBFor<TClientModelObject> Context { set; get; }

        private TClientModelObject Data { set; get; }

        private string RawData { set; get; }

        private string RawModelData { set; get; }

        public SensitiveOPerations Administration = new SensitiveOPerations();

        public class SensitiveOPerations
        {
            public string LoadRawModel(string documentId = null)
            {
                return CurrentSession.LoadRawModel(documentId);
            }

            public void DeleteDocumentPermanently(string documentId = null)
            {
                CurrentSession.DeleteDocumentPermanently(documentId);
            }

            public string LoadRaw(string documentId = null)
            {
                return CurrentSession.LoadRaw(documentId);
            }

            public void SaveRawChanges()
            {
                CurrentSession.SaveRawChanges();
            }
        }

        private void DeleteDocumentPermanently(string documentId = null)
        {
            Context = new QuickDBFor<TClientModelObject>(null, documentId, EnableEncryption, ReadOnly);
            Context.DeleteDocumentPermanently();
        }

        public TClientModelObject LoadNew(string documentId = null)
        {
            Context = new QuickDBFor<TClientModelObject>(null, documentId, EnableEncryption, ReadOnly, true);
            Data = Context.Data;
            return Data;
        }

        public TClientModelObject LoadAndCreateIfItDoesntExist(string documentId = null)
        {
            try
            {
                return Load(documentId);
            }
            catch (Exception e)
            {
                if (e is QuickDBTryingToReadNonExistentDocumentException)
                    return LoadNew(documentId);
                throw;
            }
        }

        public TClientModelObject Load(string documentId = null)
        {
            Context = new QuickDBFor<TClientModelObject>(null, documentId, EnableEncryption, ReadOnly);
            Data = Context.Data;
            return Data;
        }

        private string LoadRawModel(string documentId = null)
        {
            Context = new QuickDBFor<TClientModelObject>(null, documentId, EnableEncryption, ReadOnly);
            RawModelData = Context.RawModelData;
            return RawModelData;
        }

        private string LoadRaw(string documentId = null)
        {
            Context = new QuickDBFor<TClientModelObject>(null, documentId, EnableEncryption, ReadOnly);
            RawData = Context.RawData;
            return RawData;
        }

        private void SaveRawChanges()
        {
            Context.RawData = RawData;
        }

        public void SaveChanges(bool saveAsReadOnly = false)
        {
            Context.ReadOnly = saveAsReadOnly;
            Context.Data = Data;
        }

        public void TrySaveChanges(Action updateDocumentActionToBeRetried, bool saveAsReadOnly = false)
        {
            //todo to move into qdbfor
            Context.TrySaveChanges(() =>
            {
                updateDocumentActionToBeRetried.Invoke();
                Context.ReadOnly = saveAsReadOnly;
                if (Context.DocumentVersion == null)
                    Context.DocumentVersion = new ADocumentObject<TClientModelObject>();

                Context.DocumentVersion.Document = Data;

                Context.ConfigHandle.Save(Context.DocumentVersion, ReadOnly);
                return Context.DocumentVersion;
            });
        }

        public bool EnableEncryption { set; get; }

        public bool ReadOnly { set; get; }

        public void Dispose()
        {
            //todo: dispose resources being used
        }
    }
}