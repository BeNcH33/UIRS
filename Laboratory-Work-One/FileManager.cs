using System.IO;

namespace Laboratory_Work_One
{
    public static class FileManager
    {
        public static string[] GetFilePathsFromDirectory(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        }
    }
}
