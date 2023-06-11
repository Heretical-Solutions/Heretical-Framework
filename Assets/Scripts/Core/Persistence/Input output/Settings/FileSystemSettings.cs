using System;

using System.IO;

namespace HereticalSolutions.Persistence.IO
{
    /// <summary>
    /// File system settings specifying where to locate save files and providing shorthand methods to users
    /// </summary>
    [Serializable]
    public class FileSystemSettings
    {
        /// <summary>
        /// Save file's relative path to data folder location
        /// </summary>
        public string RelativePath;

        /// <summary>
        /// Application's data folder
        /// </summary>
        public string ApplicationDataFolder;

        /// <summary>
        /// Full file path
        /// </summary>
        public string FullPath
        {
            get
            {
                return Path.Combine(ApplicationDataFolder, RelativePath).Replace("\\", "/");
            }
        }
    }
}