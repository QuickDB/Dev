namespace QuickDB.Dependencies.Contracts
{
    public abstract class AbstractSerializationHandler
    {
        public abstract string Serialize<T>(T data);

        public abstract T DeSerialize<T>(string data);
    }
}