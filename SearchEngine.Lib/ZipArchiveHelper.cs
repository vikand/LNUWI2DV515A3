using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System;
using System.IO;

namespace SearchEngine.Lib
{
    public class ZipArchiveHelper : IZipArchiveHelper
    {
        private Lazy<ZipArchive> zipArchive;

        public ZipArchiveHelper(string externalPath)
        {
            zipArchive = new Lazy<ZipArchive>(() => ZipFile.OpenRead(externalPath));
        }

        public IEnumerable<ZipArchiveEntry> GetEntries(
            string fullNameRegexPattern, 
            RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            return this.zipArchive.Value.Entries
                .Where(e => Regex.IsMatch(e.FullName, fullNameRegexPattern, regexOptions))
                .OrderBy(e => e.FullName)
                .ToList();
        }

        public ZipArchiveEntry GetEntry(
            string fullNameRegexPattern,
            RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            return GetEntries(fullNameRegexPattern, regexOptions).FirstOrDefault();
        }

        public string ReadAllTextFromEntry(ZipArchiveEntry zipArchiveEntry)
        {
            var path = FileHelper.GetTempFileName("ZipArchiveHelper");

            try
            {
                zipArchiveEntry.ExtractToFile(path);
                return File.ReadAllText(path);
            }
            finally 
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public void Dispose()
        {
            if (zipArchive != null)
            {
                if (zipArchive.IsValueCreated)
                {
                    zipArchive.Value.Dispose();
                }

                zipArchive = null;
            }
        }
    }
}
