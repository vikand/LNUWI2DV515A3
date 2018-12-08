namespace SearchEngine.WebApi.Repositories
{
    public interface IWikipediaRepository
    {
        PageDatabase GetPagesDB(bool applyPageRanking);
    }
}