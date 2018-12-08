namespace SearchEngine.Lib
{
    public class IZipArchiveHelperFactory : IIZipArchiveHelperFactory
    {
        public ZipArchiveHelper Create(string externalPath)
        {
            return new ZipArchiveHelper(externalPath);
        }
    }
}
