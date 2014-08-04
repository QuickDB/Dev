using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QuickDB.Core.Models
{
    public class ADocumentObject<T> where T :new ()
    {
        public ADocumentObject()
        {
            UpdateHistory = new List<ADocumentObject<T>>();
            DocumentName = "";
            Document = new T();
        }

        public List<ADocumentObject<T>> UpdateHistory { set; get; }

        public Guid? ETag { set; get; }

        public DateTime Timestamp { set; get; }

        public T Document { set; get; }

        public string DocumentName { set; get; }
        public long Version { set; get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentState State { set; get; }
        public DateTime DateCreated { get; set; }
    }
}