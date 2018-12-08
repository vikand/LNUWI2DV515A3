using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SearchEngine.Entities;
using SearchEngine.Lib;
using SearchEngine.WebApi.Repositories;

namespace SearchEngine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IWikipediaRepository wikipediaRepository;

        public SearchController(IWikipediaRepository wikipediaRepository)
        {
            this.wikipediaRepository = wikipediaRepository;
        }

        // GET api/search?query=hello+world
        [HttpGet()]
        public ActionResult<IEnumerable<ScoredPage>> Get(string query, int take, bool applyPageRanking)
        {
            var rankedPages = new List<ScoredPage>();

            if (string.IsNullOrWhiteSpace(query)) { return rankedPages; }

            var pageDB = wikipediaRepository.GetPagesDB(applyPageRanking);

            var queryWords = query
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => pageDB.WordToId.ContainsKey(w))
                .Select(w => new { Word = w, Id = pageDB.WordToId[w] })
                .ToArray();   
            
            if (queryWords.Length == 0) { return rankedPages; }

            foreach (var page in pageDB.Pages)
            {
                page.Frequency = 0;
                page.Location = 0;

                foreach (var queryWord in queryWords)
                {
                    var frequency = 0;
                    var location = 100000;
                    var index = 0;

                    foreach (var id in page.Words)
                    {
                        if (queryWord.Id == id)
                        {
                            if (frequency == 0)
                            {
                                location = index + 1;
                            }

                            frequency++;
                        }

                        index++;
                    }

                    page.Frequency += frequency;
                    page.Location += location;
                }
            }

            var minFrequency = pageDB.Pages.Min(p => p.Frequency);
            var maxFrequency = pageDB.Pages.Max(p => p.Frequency);
            var minLocation = pageDB.Pages.Min(p => p.Location);
            var maxLocation = pageDB.Pages.Max(p => p.Location);
            var minRank = pageDB.Pages.Min(p => p.Rank);
            var maxRank = pageDB.Pages.Max(p => p.Rank);

            return pageDB.Pages
                .Select(p => new ScoredPage
                {
                    Url = p.Url,
                    Score = MathHelper.Normalize(p.Frequency, minFrequency, maxFrequency, false) +
                            0.8 * MathHelper.Normalize(p.Location, minLocation, maxLocation, true) +
                            (applyPageRanking
                                ? 0.5 * MathHelper.Normalize(p.Rank, minRank, maxRank, false)
                                : 0)
                })
                .OrderByDescending(p => p.Score)
                .ThenBy(p => p.Url)
                .Take(take)
                .ToList();
        }
    }
}
