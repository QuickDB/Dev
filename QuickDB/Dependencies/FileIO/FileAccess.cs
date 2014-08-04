using System.IO;
using QuickDB.Dependencies.Contracts;

namespace QuickDB.Dependencies.FileIO
{
    public class FileAccessHandler : AbstractFileAccessHandler
    {
        public FileAccessHandler(string fileName, string documentId) : base(fileName, documentId)
        {
        }

        public override string Read()
        {
            using (var reader = new StreamReader(GetFileName()))
            {
                return reader.ReadToEnd();
            }
        }

        public override void Save(string content)
        {
            using (var writer = new StreamWriter(GetFileName(), false))
            {
                writer.Write(content);
            }
        }

        public override bool Exists()
        {
            return File.Exists(GetFileName());
        }

        public override void Delete()
        {
            if (Exists())
            {
                File.Delete(GetFileName());
            }
        }

        public override bool DirectoryExists()
        {
            return Directory.Exists(GetDirectoryName());
        }


        public override void CreateRequiredFileIfItDoesntAlreadyExist()
        {
            if (Exists()) return;

            File.Create(GetFileName()).Close();
        }
        public override void CreateRequiredDirectoryIfItDoesntAlreadyExist()
        {
            if (DirectoryExists()) return;

            Directory.CreateDirectory(GetDirectoryName());
        }
    }
}