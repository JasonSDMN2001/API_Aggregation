using API_Aggregation.Clients;
using API_Aggregation.Models;
using API_Aggregation.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace API_Aggregation.Tests
{
    public class AggregationServiceTests
    {
        private readonly Mock<WeatherApiClient> _mockWeatherApiClient;
        private readonly Mock<CatFactApiClient> _mockCatFactApiClient;
        private readonly Mock<ArtApiClient> _mockArtApiClient;
        private readonly AggregationService _aggregationService;

        public AggregationServiceTests()
        {
            _mockWeatherApiClient = new Mock<WeatherApiClient>(null);
            _mockCatFactApiClient = new Mock<CatFactApiClient>(null);
            _mockArtApiClient = new Mock<ArtApiClient>(null);
            _aggregationService = new AggregationService(
                _mockWeatherApiClient.Object,
                _mockCatFactApiClient.Object,
                _mockArtApiClient.Object);
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

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync(city, query, count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(weatherForecast, result.Weather);
            Assert.Equal(catFacts, result.CatFact);
            Assert.Equal(artwork, result.Artwork);
        }

        [Fact]
        public async Task GetAggregatedDataAsync_FiltersCatFacts()
        {
            // Arrange
            var city = "TestCity";
            var query = "TestQuery";
            var count = 5;
            var filterBy = "TestFact1";
            var weatherForecast = new WeatherForecast { temp_C = 20, humidity = 50, windspeedKmph = 10, uvIndex = 5 };
            var catFacts = new List<CatFact> { new CatFact { Fact = "TestFact1" }, new CatFact { Fact = "TestFact2" } };
            var artwork = new Artwork { title = "TestArtwork" };

            _mockWeatherApiClient.Setup(x => x.GetWeatherAsync(city)).ReturnsAsync(weatherForecast);
            _mockCatFactApiClient.Setup(x => x.GetCatFactsAsync(count)).ReturnsAsync(catFacts);
            _mockArtApiClient.Setup(x => x.GetArtworksAsync(query)).ReturnsAsync(artwork);

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync(city, query, count, filterBy: filterBy);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.CatFact);
            Assert.Equal("TestFact1", result.CatFact[0].Fact);
        }

        [Fact]
        public async Task GetAggregatedDataAsync_SortsCatFactsByLength()
        {
            // Arrange
            var city = "TestCity";
            var query = "TestQuery";
            var count = 5;
            var sortBy = "asc";
            var weatherForecast = new WeatherForecast { temp_C = 20, humidity = 50, windspeedKmph = 10, uvIndex = 5 };
            var catFacts = new List<CatFact> { new CatFact { Fact = "TestFact2" }, new CatFact { Fact = "TestFact1" } };
            var artwork = new Artwork { title = "TestArtwork" };

            _mockWeatherApiClient.Setup(x => x.GetWeatherAsync(city)).ReturnsAsync(weatherForecast);
            _mockCatFactApiClient.Setup(x => x.GetCatFactsAsync(count)).ReturnsAsync(catFacts);
            _mockArtApiClient.Setup(x => x.GetArtworksAsync(query)).ReturnsAsync(artwork);

            // Act
            var result = await _aggregationService.GetAggregatedDataAsync(city, query, count, sortBy: sortBy);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CatFact.Count);
            Assert.Equal("TestFact1", result.CatFact[0].Fact);
            Assert.Equal("TestFact2", result.CatFact[1].Fact);
        }
    }
}