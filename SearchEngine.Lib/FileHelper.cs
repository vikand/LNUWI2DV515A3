using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SearchEngine.Lib
{
    /// <summary>
    /// Helper class for File related issues.
    /// </summary>
    public static class FileHelper
    {
        public static string GetTempFolder(string subFolder = null)
        {
            var tempFolder = string.IsNullOrWhiteSpace(subFolder)
                ? Path.GetTempPath()
                : Path.Combine(Path.GetTempPath(), subFolder);

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            return tempFolder;
        }

        /// <summary>
        /// Used instead of <see cref="Path.GetTempFileName"/> as it easily creates duplicates./>
        /// </summary>
        public static string GetTempFileName(
            string subFolder = null, 
            string extension = ".tmp",
            bool createFolderIfItDoesNotExist = true)
        {
            var tempFolder = GetTempFolder(subFolder);
            var fileName = string.IsNullOrWhiteSpace(extension)
                ? Guid.NewGuid().ToString()
                : Guid.NewGuid().ToString() + extension;

            return Path.Combine(tempFolder, fileName);
        }
    }
}
