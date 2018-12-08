using Moq;
using SearchEngine.WebApi.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SearchEngine.Lib;
using Xunit;

namespace SearchEngine.WebApi.Tests
{
    public class WikipediaRepositoryTests
    {

        [Fact]
        public void GetPagesDBFromMiniZip()
        {
            //
            // Arrange
            //

            var codeBaseFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var contentRootPath = Path.Combine(codeBaseFolder.Substring(0,
                    codeBaseFolder.LastIndexOf("\\SearchEngine.WebApi.Tests")),
                    "SearchEngine.WebApi.Tests");

            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            hostingEnvironmentMock.SetupGet(m => m.ContentRootPath).Returns(contentRootPath);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(m => m["RelativePathToWikipediaZipFile"]).Returns(@"Data\wikipedia-mini.zip");
            configurationMock.SetupGet(m => m["PageRankingIterations"]).Returns("20");

            var sut = new WikipediaRepository(
                configurationMock.Object,
                hostingEnvironmentMock.Object,
                new CacheHelper(),
                new IZipArchiveHelperFactory());

            //
            // Act
            //

            var result = sut.GetPagesDB(true);

            //
            // Assert
            //

            Assert.NotNull(result);

            var pages = result.Pages.OrderByDescending(p => p.Rank).ThenBy(p => p.Url).ToArray();

            Assert.Equal(4, pages.Length);

            Assert.Equal("/wiki/A", pages[0].Url);
            Assert.Equal("/wiki/B", pages[1].Url);
            Assert.Equal("/wiki/C", pages[2].Url);
            Assert.Equal("/wiki/D", pages[3].Url);

            Assert.Equal(0.334875, Math.Round(pages[0].Rank, 6));
            Assert.Equal(0.15, Math.Round(pages[1].Rank, 2));
            Assert.Equal(0.15, Math.Round(pages[2].Rank, 2));
            Assert.Equal(0.15, Math.Round(pages[3].Rank, 2));
        }
    }
}
