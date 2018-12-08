namespace SearchEngine.Lib
{
    public interface IIZipArchiveHelperFactory
    {
        ZipArchiveHelper Create(string externalPath);
    }
}