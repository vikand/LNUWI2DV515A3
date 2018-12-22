using Microsoft.Extensions.Configuration;
using SearchEngine.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Runtime.Caching;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace SearchEngine.WebApi.Repositories
{
    public class WikipediaRepository : IWikipediaRepository
    {
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICacheHelper cacheHelper;
        private readonly IZipArchiveHelperFactory zipArchiveHelperFactory;

        public WikipediaRepository(
            IConfiguration configuration, 
            IHostingEnvironment hostingEnvironment,
            ICacheHelper cacheHelper,
            IZipArchiveHelperFactory zipArchiveHelperFactory)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.cacheHelper = cacheHelper;
            this.zipArchiveHelperFactory = zipArchiveHelperFactory;
        }

        public PageDatabase GetPagesDB(bool applyPageRanking)
        {
            var zipArchivePath = GetZipArchivePath();

            if (this.cacheHelper.Contains(zipArchivePath))
            {
                var cachedPageDB = this.cacheHelper.Get<PageDatabase>(zipArchivePath);
                if (cachedPageDB.IsPageRankingApplied || !applyPageRanking)
                {
                    return cachedPageDB;
                }
            }

            var wordToId = new Dictionary<string, int>();
            var pageDB = new PageDatabase
            {
                WordToId = wordToId,
                Pages = GetPages(zipArchivePath, wordToId),
                IsPageRankingApplied = applyPageRanking,
            };

            if (applyPageRanking)
            {
                ApplyPageRanking(pageDB.Pages);
            }

            var cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { zipArchivePath }));
            this.cacheHelper.Set(zipArchivePath, pageDB, cacheItemPolicy);

            return pageDB;
        }

        private string GetZipArchivePath()
        {
            var rootPath = this.hostingEnvironment.ContentRootPath;
            var relativePathToBlogDataFile = this.configuration["RelativePathToWikipediaZipFile"];

            return Path.Combine(rootPath, relativePathToBlogDataFile);
        }

        private IEnumerable<Page> GetPages(string zipArchivePath, Dictionary<string, int> wordToId)
        {
            var pages = new List<Page>();

            using (var zipArchiveHelper = zipArchiveHelperFactory.Create(zipArchivePath))
            {
                var articles = zipArchiveHelper
                    .GetEntries("^wikipedia/(Links|Words)/[^/]+/[^/]+$")
                    .Select(e => e.FullName.Substring(e.FullName.LastIndexOf('/') + 1))
                    .Distinct()
                    .OrderBy(a => a)
                    .ToArray();

                foreach (var article in articles)
                {
                    var escapedArticle = Regex.Escape(article);
                    var wordsEntry = zipArchiveHelper
                        .GetEntry($"^wikipedia/Words/[^/]+/{escapedArticle}$");
                    var linksEntry = zipArchiveHelper
                        .GetEntry($"^wikipedia/Links/[^/]+/{escapedArticle}$");

                    var page = new Page
                    {
                        Url = $"/wiki/{article}",
                        Rank = 1d,
                        Words = GetWords(zipArchiveHelper, wordsEntry, wordToId),
                        Links = GetLinks(zipArchiveHelper, linksEntry)
                    };

                    pages.Add(page);
                }
            }

            return pages;
        }

        private IEnumerable<int> GetWords(
            ZipArchiveHelper zipArchiveHelper, 
            ZipArchiveEntry zipArchiveEntry, 
            IDictionary<string, int> wordToId)
        {
            List<int> ids = new List<int>();

            if (zipArchiveEntry != null)
            {
                var text = zipArchiveHelper.ReadAllTextFromEntry(zipArchiveEntry);
                var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    if (!wordToId.Keys.Contains(word))
                    {
                        wordToId.Add(word, wordToId.Count);
                    }

                    ids.Add(wordToId[word]);
                }
            }

            return ids;
        }

        private IEnumerable<string> GetLinks(
            ZipArchiveHelper zipArchiveHelper, 
            ZipArchiveEntry zipArchiveEntry)
        {
            if (zipArchiveEntry == null)
            {
                return new string[0];
            }

            var text = zipArchiveHelper.ReadAllTextFromEntry(zipArchiveEntry);

            if (text.Contains(Environment.NewLine))
            {
                Console.WriteLine("");
            }

            return text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        private void ApplyPageRanking(IEnumerable<Page> pages)
        {
            var pageRankingIterations = int.Parse(this.configuration["PageRankingIterations"]);

            for (var iteration = 0; iteration < pageRankingIterations; iteration++)
            {
                foreach (var page in pages)
                {
                    page.Rank = 0.15;

                    foreach (var otherPage in pages)
                    {
                        if (otherPage.Url != page.Url &&
                            otherPage.Links.Any(l => l.Equals(page.Url, StringComparison.OrdinalIgnoreCase)))
                        {
                            page.Rank += 0.85 * (otherPage.Rank / otherPage.Links.Count());
                        }
                    }
                }
            }
        }
    }
}
