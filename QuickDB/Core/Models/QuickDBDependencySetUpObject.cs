using QuickDB.Dependencies.Contracts;
using QuickDB.Dependencies.Encryptions;
using QuickDB.Dependencies.FileIO;
using QuickDB.Dependencies.Serializations;

namespace QuickDB.Core.Models
{
    public class QuickDBDependencySetUpObject
    {
        public AbstractFileAccessHandler FileAccessHandler { set; get; }

        public AbstractEncryptionHandler EncryptionHandler { set; get; }

        public AbstractSerializationHandler AbstractSerializationHandler { set; get; }

        public bool EnableEncryption { set; get; }
    }
}