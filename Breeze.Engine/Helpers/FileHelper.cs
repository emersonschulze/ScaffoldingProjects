using System.IO;
using System.Text;
using Breeze.Engine.SourceType;

namespace Breeze.Engine.Helpers
{
    public class FileHelper : IFileHelper
    {
        public string GenerateFile(ISource sourceFile)
        {
            var fileInfo = sourceFile.Info();
            var content = sourceFile.Content();

            if (fileInfo.Exists && !sourceFile.OverrideFile)
                return string.Empty;

            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            var file = File.Open(Path.Combine(fileInfo.Directory.FullName, fileInfo.Name), FileMode.Create);
            var writer = new StreamWriter(file, Encoding.UTF8);

            try
            {
                writer.Write(content);
            }
            finally
            {
                writer.Close();
                file.Close();
            }
            return Path.Combine(fileInfo.Directory.FullName, fileInfo.Name);
        }
    }

    public interface IFileHelper
    {
        string GenerateFile(ISource sourceFile);
    }
}
