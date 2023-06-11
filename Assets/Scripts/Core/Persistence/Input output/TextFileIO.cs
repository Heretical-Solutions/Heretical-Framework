using System;
using System.IO;

namespace HereticalSolutions.Persistence.IO
{
    public static class TextFileIO
    {
        public static bool Write(
            FileSystemSettings settings,
            string contents)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);

            File.WriteAllText(savePath, contents);

            return true;
        }
        
        public static bool Write(
            FileSystemSettings settings,
            byte[] contents)
        {
            string savePath = settings.FullPath;

            EnsureDirectoryExists(savePath);

            File.WriteAllBytes(savePath, contents);

            return true;
        }

        public static bool Read(
            FileSystemSettings settings,
            out string contents)
        {
            string savePath = settings.FullPath;

            contents = string.Empty;
			
            if (!FileExists(savePath))
                return false;
			
            contents = File.ReadAllText(savePath);

            return true;
        }
        
        public static bool Read(
            FileSystemSettings settings,
            out byte[] contents)
        {
            string savePath = settings.FullPath;

            contents = null;
			
            if (!FileExists(savePath))
                return false;
			
            contents = File.ReadAllBytes(savePath);

            return true;
        }

        public static void Erase(FileSystemSettings settings)
        {
            string savePath = settings.FullPath;

            if (File.Exists(savePath))
                File.Delete(savePath);
        }

        /// <summary>
        /// Checks whether the file at the specified path exists
        /// - Also makes sure the folder directory specified in the path actually exists anyway
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Does the file exist</returns>
        private static bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[TextFileIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[TextFileIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            return File.Exists(path);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("[TextFileIO] INVALID PATH");
			
            string directoryPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(directoryPath))
                throw new Exception("[TextFileIO] INVALID DIRECTORY PATH");
			
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}