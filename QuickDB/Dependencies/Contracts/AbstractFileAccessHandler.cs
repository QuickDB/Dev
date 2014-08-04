using System;
using QuickDB.Rules;

namespace QuickDB.Dependencies.Contracts
{
    public abstract class AbstractFileAccessHandler
    {
        protected AbstractFileAccessHandler(string fileName, string documentId)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            AccessHandlerRules = StorageRulesFactory.CreateAccessHandlerRules(fileName, documentId);
        }

        protected StorageRulesFactory.AccessHandlerRules AccessHandlerRules { set; get; }

        public string GetFileName()
        {
            return GetDirectoryName() + "\\" + AccessHandlerRules.FileName + "." + AccessHandlerRules.FileExtention;
        }

        public string GetDirectoryName()
        {
            return AccessHandlerRules.BaseDirectoryName + "\\" + AccessHandlerRules.SectionDirectoryName;
        }

        public abstract string Read();

        public abstract bool Exists();

        public abstract void Delete();

        public abstract bool DirectoryExists();

        public abstract void CreateRequiredDirectoryIfItDoesntAlreadyExist();

        public abstract void Save(string content);
        public abstract void CreateRequiredFileIfItDoesntAlreadyExist();
    }
}