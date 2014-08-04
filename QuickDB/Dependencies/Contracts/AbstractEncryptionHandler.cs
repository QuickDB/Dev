


using QuickDB.Core.Models;

namespace QuickDB.Dependencies.Contracts
{
    public abstract class AbstractEncryptionHandler
    {
        protected abstract ISecurityStrings SecurityStrings { set; get; }
        public abstract string EnCrypt(string s);
        public abstract string DeCrypt(string s);
    }
}