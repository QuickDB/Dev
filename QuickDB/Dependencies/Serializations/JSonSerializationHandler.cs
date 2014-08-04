using Newtonsoft.Json;
using QuickDB.Dependencies.Contracts;

namespace QuickDB.Dependencies.Serializations
{
  public   class JSonSerializationHandler:AbstractSerializationHandler
    {
        public override string Serialize<T>(T data)
        {
         return   JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public override T DeSerialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
