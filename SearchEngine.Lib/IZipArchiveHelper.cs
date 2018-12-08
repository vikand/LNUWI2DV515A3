using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace SearchEngine.Lib
{
    public interface IZipArchiveHelper : IDisposable
    {
        IEnumerable<ZipArchiveEntry> GetEntries(string fullNameRegexPattern, RegexOptions regexOptions = RegexOptions.IgnoreCase);
        string ReadAllTextFromEntry(ZipArchiveEntry zipArchiveEntry);
    }
}