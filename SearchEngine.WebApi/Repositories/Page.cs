using System.Collections.Generic;

namespace SearchEngine.WebApi.Repositories
{
    public class Page
    {
        public string Url { get; set; }

        public IEnumerable<int> Words { get; set; }

        public IEnumerable<string> Links { get; set; }

        public double Rank { get; set; }

        public double Frequency { get; set; }

        public double Location { get; set; }
    }
}
