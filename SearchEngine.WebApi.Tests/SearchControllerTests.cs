using Moq;
using SearchEngine.WebApi.Controllers;
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
    public class SearchControllerTests
    {
        [Fact]
        public void SearchForNintendo()
        {
            //
            // Arrange
            //

            var codeBaseFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var contentRootPath = Path.Combine(codeBaseFolder.Substring(0,
                    codeBaseFolder.LastIndexOf("\\SearchEngine.WebApi.Test")),
                    "SearchEngine.WebApi");

            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            hostingEnvironmentMock.SetupGet(m => m.ContentRootPath).Returns(contentRootPath);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(m => m["RelativePathToWikipediaZipFile"]).Returns(@"Data\wikipedia.zip");
            configurationMock.SetupGet(m => m["PageRankingIterations"]).Returns("20");

            var wikipediaRepository = new WikipediaRepository(
                configurationMock.Object, 
                hostingEnvironmentMock.Object, 
                new CacheHelper(), 
                new IZipArchiveHelperFactory());

            var sut = new SearchController(wikipediaRepository);

            //
            // Act
            //

            var result = sut.Get("Nintendo", 5, true).Value.ToArray();

            //
            // Assert
            //

            Assert.NotNull(result);
            Assert.Equal(5, result.Length);

            Assert.Equal("/wiki/Nintendo", result[0].Url);
            Assert.Equal("/wiki/Nintendo_Switch", result[1].Url);
            Assert.Equal("/wiki/Nintendo_Entertainment_System", result[2].Url);
            Assert.Equal("/wiki/List_of_Game_of_the_Year_awards", result[3].Url);
            Assert.Equal("/wiki/Super_Nintendo_Entertainment_System", result[4].Url);

            Assert.Equal(2.02, Math.Round(result[0].Score, 2));
            Assert.Equal(1.50, Math.Round(result[1].Score, 2));
            Assert.Equal(1.45, Math.Round(result[2].Score, 2));
            Assert.Equal(0.91, Math.Round(result[3].Score, 2));
            Assert.Equal(0.77, Math.Round(result[4].Score, 2));
        }

        [Fact]
        public void SearchForCAndSharp()
        {
            //
            // Arrange
            //

            var codeBaseFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var contentRootPath = Path.Combine(codeBaseFolder.Substring(0,
                    codeBaseFolder.LastIndexOf("\\SearchEngine.WebApi.Test")),
                    "SearchEngine.WebApi");

            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            hostingEnvironmentMock.SetupGet(m => m.ContentRootPath).Returns(contentRootPath);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(m => m["RelativePathToWikipediaZipFile"]).Returns(@"Data\wikipedia.zip");
            configurationMock.SetupGet(m => m["PageRankingIterations"]).Returns("20");

            var wikipediaRepository = new WikipediaRepository(
                configurationMock.Object,
                hostingEnvironmentMock.Object,
                new CacheHelper(),
                new IZipArchiveHelperFactory());

            var sut = new SearchController(wikipediaRepository);

            //
            // Act
            //

            var result = sut.Get("C Sharp", 5, true).Value.ToArray();

            //
            // Assert
            //

            Assert.NotNull(result);
            Assert.Equal(5, result.Length);

            Assert.Equal("/wiki/C_(programming_language)", result[0].Url);
            Assert.Equal("/wiki/C%2B%2B", result[1].Url);
            Assert.Equal("/wiki/Charles_Babbage", result[2].Url);
            Assert.Equal("/wiki/Domain-specific_language", result[3].Url);
            Assert.Equal("/wiki/Tablet_computer", result[4].Url);

            Assert.Equal(1.26, Math.Round(result[0].Score, 2));
            Assert.Equal(1.10, Math.Round(result[1].Score, 2));
            Assert.Equal(1.01, Math.Round(result[2].Score, 2));
            Assert.Equal(1.01, Math.Round(result[3].Score, 2));
            Assert.Equal(0.90, Math.Round(result[4].Score, 2));
        }
    }
}
