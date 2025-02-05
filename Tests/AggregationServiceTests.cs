using API_Aggregation.Clients;
using API_Aggregation.Models;
using API_Aggregation.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace API_Aggregation.Tests
{
    public class AggregationServiceTests
    {
        private readonly Mock<IWeatherApiClient> _mockWeatherApiClient;
        private readonly Mock<ICatFactApiClient> _mockCatFactApiClient;
        private readonly Mock<IArtApiClient> _mockArtApiClient;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<RequestStatisticsService> _mockRequestStatisticsService;
        private readonly AggregationService _aggregationService;

        public AggregationServiceTests()
        {
            _mockWeatherApiClient = new Mock<IWeatherApiClient>();
            _mockCatFactApiClient = new Mock<ICatFactApiClient>();
            _mockArtApiClient = new Mock<IArtApiClient>();
            _mockCache = new Mock<IMemoryCache>();
            _mockRequestStatisticsService = new Mock<RequestStatisticsService>();
            _aggregationService = new AggregationService(
                _mockWeatherApiClient.Object,
                _mockCatFactApiClient.Object,
                _mockArtApiClient.Object,
                _mockCache.Object,
                _mockRequestStatisticsService.Object);
        }

        [Fact]
        public async Task GetAggregatedDataAsync_ReturnsAggregatedData()
        {
            // Arrange
            var city = "TestCity";
            var query = "TestQuery";
            var count = 5;
            var weatherForecast = new WeatherForecast { temp_C = 20, humidity = 50, windspeedKmph = 10, uvIndex = 5 };
            var catFacts = new List<CatFact> { new CatFact { Fact = "TestFact1" }, new CatFact { Fact = "TestFact2" } };
            var artwork = new Artwork { title = "TestArtwork" };

            _mockWeatherApiClient.Setup(x => x.GetWeatherAsync(city)).ReturnsAsync(weatherForecast);
            _mockCatFactApiClient.Setup(x => x.GetCatFactsAsync(count)).ReturnsAsync(catFacts);
            _mockArtApiClient.Setup(x => x.GetArtworksAsync(query)).ReturnsAsync(artwork);

            object weatherCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out weatherCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            object catFactsCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out catFactsCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            object artworkCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out artworkCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync(city, query, count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(weatherForecast, result.Weather);
            Assert.Equal(catFacts, result.CatFact);
            Assert.Equal(artwork, result.Artwork);
        }

        [Theory]
        [InlineData("TestFact1", 1)]
        [InlineData("TestFact2", 1)]
        [InlineData("NonExistentFact", 0)]
        public async Task GetAggregatedDataAsync_FiltersCatFacts(string filterBy, int expectedCount)
        {
            // Arrange
            var city = "TestCity";
            var query = "TestQuery";
            var count = 5;
            var weatherForecast = new WeatherForecast { temp_C = 20, humidity = 50, windspeedKmph = 10, uvIndex = 5 };
            var catFacts = new List<CatFact> { new CatFact { Fact = "TestFact1" }, new CatFact { Fact = "TestFact2" } };
            var artwork = new Artwork { title = "TestArtwork" };

            _mockWeatherApiClient.Setup(x => x.GetWeatherAsync(city)).ReturnsAsync(weatherForecast);
            _mockCatFactApiClient.Setup(x => x.GetCatFactsAsync(count)).ReturnsAsync(catFacts);
            _mockArtApiClient.Setup(x => x.GetArtworksAsync(query)).ReturnsAsync(artwork);

            object weatherCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out weatherCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            object catFactsCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out catFactsCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            object artworkCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out artworkCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync(city, query, count, filterBy: filterBy);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.CatFact.Count);
            if (expectedCount > 0)
            {
                Assert.Equal(filterBy, result.CatFact[0].Fact);
            }
        }

        [Theory]
        [InlineData("length asc", "TestFact1")]
        [InlineData("length desc", "LongLengthTestFact2")]
        public async Task GetAggregatedDataAsync_SortsCatFactsByLength(string sortBy, string expectedFirstFact)
        {
            // Arrange
            var city = "TestCity";
            var query = "TestQuery";
            var count = 5;
            var weatherForecast = new WeatherForecast { temp_C = 20, humidity = 50, windspeedKmph = 10, uvIndex = 5 };
            var catFacts = new List<CatFact> { new CatFact { Fact = "LongLengthTestFact2" }, new CatFact { Fact = "TestFact1" } };
            var artwork = new Artwork { title = "TestArtwork" };

            _mockWeatherApiClient.Setup(x => x.GetWeatherAsync(city)).ReturnsAsync(weatherForecast);
            _mockCatFactApiClient.Setup(x => x.GetCatFactsAsync(count)).ReturnsAsync(catFacts);
            _mockArtApiClient.Setup(x => x.GetArtworksAsync(query)).ReturnsAsync(artwork);

            object weatherCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out weatherCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            object catFactsCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out catFactsCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            object artworkCacheEntry = null;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out artworkCacheEntry)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync(city, query, count, sortBy: sortBy);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CatFact.Count);
            Assert.Equal(expectedFirstFact, result.CatFact[0].Fact);
        }
    }
}