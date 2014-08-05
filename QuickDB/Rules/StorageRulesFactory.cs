using System;

namespace QuickDB.Rules
{
    public class StorageRulesFactory
    {
        public static AccessHandlerRules CreateAccessHandlerRules(string fileName,string documentId)
        {
            documentId = string.IsNullOrEmpty(documentId) ? "" : documentId + "_";
             string mainDirectory =System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +@"\QuickDB";
            const string documentExtention = "qdb";
            var documentName = documentId+fileName + "_Document";
            var tableDirectoryName = fileName;

            return new AccessHandlerRules(documentName, documentExtention, mainDirectory, tableDirectoryName);
        }
        public class AccessHandlerRules
        {
            public AccessHandlerRules(string fileName, string fileExtension, string baseDirectoryName, string sectionDirectoryName)
            {
                if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
                if (string.IsNullOrEmpty(fileExtension)) throw new ArgumentNullException("fileExtension");
                if (string.IsNullOrEmpty(baseDirectoryName)) throw new ArgumentNullException("baseDirectoryName");
                if (string.IsNullOrEmpty(sectionDirectoryName)) throw new ArgumentNullException("sectionDirectoryName");

                FileName = fileName;
                FileExtention = fileExtension;
                BaseDirectoryName = baseDirectoryName;
                SectionDirectoryName = sectionDirectoryName;
            }

            public string FileName { set; get; }

            public string FileExtention { set; get; }

            public string BaseDirectoryName { set; get; }

            public string SectionDirectoryName { set; get; }
        }

    }
}