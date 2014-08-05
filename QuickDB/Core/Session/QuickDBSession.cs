using QuickDB.Core.QuickDBException;
using QuickDB.Core.SessionBackBones;
using QuickDB.DependencyMapProvider;
using System;

namespace QuickDB.Core.Session
{
    public class QuickDBSessionFor<TConfigurationObject> : IDisposable where TConfigurationObject : new()
    {
        private static QuickDBSessionFor<TConfigurationObject> CurrentSession { set; get; }

        public QuickDBSessionFor()
        {
            DependencyInjection.SetUpDefault();
            CurrentSession = this;
        }

        private QuickDBFor<TConfigurationObject> Context { set; get; }

        private TConfigurationObject Data { set; get; }

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
            Context = new QuickDBFor<TConfigurationObject>(null, documentId, EnableEncryption, ReadOnly);
            Context.DeleteDocumentPermanently();
        }

        public TConfigurationObject LoadNew(string documentId = null)
        {
            Context = new QuickDBFor<TConfigurationObject>(null, documentId, EnableEncryption, ReadOnly, true);
            Data = Context.Data;
            return Data;
        }

        public TConfigurationObject LoadAndCreateIfItDoesntExist(string documentId = null)
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

        public TConfigurationObject Load(string documentId = null)
        {
            Context = new QuickDBFor<TConfigurationObject>(null, documentId, EnableEncryption, ReadOnly);
            Data = Context.Data;
            return Data;
        }

        private string LoadRawModel(string documentId = null)
        {
            Context = new QuickDBFor<TConfigurationObject>(null, documentId, EnableEncryption, ReadOnly);
            RawModelData = Context.RawModelData;
            return RawModelData;
        }

        private string LoadRaw(string documentId = null)
        {
            Context = new QuickDBFor<TConfigurationObject>(null, documentId, EnableEncryption, ReadOnly);
            RawData = Context.RawData;
            return RawData;
        }

        private void SaveRawChanges()
        {
            Context.RawData = RawData;
        }

        public void SaveChanges()
        {
            Context.Data = Data;
        }

        public bool EnableEncryption { set; get; }

        public bool ReadOnly { set; get; }

        public void Dispose()
        {
            //todo: dispose resources being used
        }
    }
}