using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PageRankConsole
{
    class Article
    {
        public string Name { get; set; }
        public string[] Links { get; set; }
        public decimal PageRank { get; set; }
        public decimal NormalizedPageRank { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var wikipediaLinksPath =
                //@"C:\andersvikstrom\AHV\Cources\LNU\WebIntelligence\Assignment3\PageRankConsole\Data\wikipedia\wikipedia\Links";
                @"C:\Users\ander\source\repos\AHV\Courses\LNU\Web Intelligence\Assignment3\PageRankConsole\Data\wikipedia\wikipedia\Links";

            var articles = new List<Article>();

            foreach (var subDir in new string[] { "Games", "Programming" })
            {
                foreach (var file in Directory.GetFiles(Path.Combine(wikipediaLinksPath, subDir)))
                {
                    var article = new Article { Name = Path.GetFileName(file), PageRank = 1 };

                    if (articles.Any(a => a.Name == article.Name))
                        continue;

                    var text = File.ReadAllText(file);

                    article.Links = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                    articles.Add(article);
                }
            }

            Console.WriteLine("Total number of articles: " + articles.Count());
            Console.WriteLine("Total number of links: "  + articles.SelectMany(a => a.Links).Count());

            for (var iteration = 0; iteration < 20; iteration++)
            {
                foreach (var article in articles)
                {
                    var pageRank = 0m;

                    foreach (var otherArticle in articles)
                    {
                        if (otherArticle.Links.Any(l => l.Equals($"/wiki/{article.Name}")))
                        {
                            pageRank += otherArticle.PageRank / otherArticle.Links.Length;
                        }
                    }

                    article.PageRank = 0.85m * pageRank + 0.15m;
                }
            }

            var maxPageRank = articles.Max(a => a.PageRank);
            Console.WriteLine($"Max page rank: {maxPageRank}");

            foreach (var article in articles)
            {
                article.NormalizedPageRank = article.PageRank / maxPageRank;
            }

            var nintendoArticles = new []
            {
                "Nintendo",
                "Nintendo_Switch",
                "Nintendo_Entertainment_System",
                "List_of_Game_of_the_Year_awards",
                "Super_Nintendo_Entertainment_System"
            };

            foreach (var article in nintendoArticles.Select(na => articles.First(a => a.Name == na)))
            {
                Console.WriteLine(article.Name);
                Console.WriteLine($"\tRank = {Math.Round(article.PageRank, 2)}");
                Console.WriteLine($"\tNormalized Rank = {Math.Round(article.NormalizedPageRank, 2)}");
                Console.WriteLine($"\tNormalized Rank * 0.5 = {Math.Round(article.NormalizedPageRank * 0.5m, 2)}");
            }
        }
    }
}
