using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QuickDB.Core.Models
{
    public class ADocumentObject<T> where T :new ()
    {
        public Guid? ID { set; get; }

        public ADocumentObject(Guid? docId = null, bool makeDocumentReadOnly = false)
        {
            UpdateHistory = new List<ADocumentObject<T>>();
            DocumentName = "";
            Document = new T();
            if (docId != null)
            {
                ID = docId;
            }
           
        }

        public Guid? TransactionId { set; get; }

        public List<ADocumentObject<T>> UpdateHistory { set; get; }

        public Guid? ETag { set; get; }

        public DateTime Timestamp { set; get; }

        /// <summary>
        /// this property is only set in the core.
        /// 
        /// </summary>
        public bool ReadOnlyDocument { set; get; }

        public T Document { set; get; }

        public string DocumentName { set; get; }
        public long Version { set; get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentState State { set; get; }
        public DateTime DateCreated { get; set; }
    }
}