using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SearchEngine.Entities;

namespace SearchEngine.WebApp.Pages
{
    public class SearchModel : PageModel
    {
        private IHttpClientWrapper client;

        public SearchModel(IHttpClientWrapper client)
        {
            this.client = client;
        }

        [BindProperty]
        public string SearchQuery { get; set; }

        [BindProperty]
        public bool ApplyPageRanking { get; set; }

        public IEnumerable<ScoredPage> ScoredPages { get; set; }

        public TimeSpan Duration { get; set; }

        public void OnGet()
        {
            SearchQuery = "";
            ApplyPageRanking = true;
            ScoredPages = null;
        }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                return;
            }

            var query = string.Join('+', SearchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            var requestUri = $"api/search?query={query}&take=5&applypageranking={ApplyPageRanking}";
            var startTime = DateTime.Now;

            var result = client.Get<IEnumerable<ScoredPage>>(requestUri);

            Duration = DateTime.Now - startTime;

            if (result.Item2 == System.Net.HttpStatusCode.OK)
            {
                ScoredPages = result.Item1;
            }
        }
    }
}
