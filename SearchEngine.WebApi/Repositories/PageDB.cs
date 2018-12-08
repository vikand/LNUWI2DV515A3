using System.Collections.Generic;

namespace SearchEngine.WebApi.Repositories
{
    public class PageDatabase
    {
        public Dictionary<string, int> WordToId { get; set; }

        public IEnumerable<Page> Pages { get; set; }

        public bool IsPageRankingApplied { get; set; }
    }
}
