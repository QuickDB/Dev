using QuickDB.Core.Models;
using QuickDB.Core.QuickDBException;
using System;

namespace QuickDB.Core.Core
{
    public abstract class CoreFacade<TClientModelObject> : CoreQuickDB<TClientModelObject> where TClientModelObject : new()
    {
        protected CoreFacade(QuickDBDependencySetUpObject quickDBDependencySetUpObject, bool readOnly = false)
            : base(quickDBDependencySetUpObject, readOnly)
        {



        }

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
            obj.Document = default(TClientModelObject);
            obj.UpdateHistory = null;
            return SerializationHandler.Serialize(obj);
        }

        public string LoadRawDocumentHistory()
        {
            return SerializationHandler.Serialize(_Load().UpdateHistory);
        }

        public ADocumentObject<TClientModelObject> Load(bool createIfItDoesNotExist = false,Guid? guidForEnteringIntoTransaction=null)
        {
            return _Load(null, createIfItDoesNotExist);
        }

        public void DeleteDocumentPermanently(ADocumentObject<TClientModelObject> data)
        {
          
            _DeleteDocumentPermanently(data);
        }

        public void Delete(ADocumentObject<TClientModelObject> data)
        {
          
            _Delete(data);
        }

        public void SaveRaw(string data)
        {
        
            _SaveRaw(data);
        }

        public void Save(ADocumentObject<TClientModelObject> data,bool makeReadOnly=false)
        {
        
            _Save(data, makeReadOnly);
        }

        public void TrySave(Func<ADocumentObject<TClientModelObject>> updateDocumentActionToBeRetried, int numberOfRetries=100, int intervalBetweenRetries=100, bool makeDocumentReadOnly=true)
        {
            _TrySave(updateDocumentActionToBeRetried, makeDocumentReadOnly, numberOfRetries, intervalBetweenRetries);
        }
    }
}