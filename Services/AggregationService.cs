using API_Aggregation.Clients;
using API_Aggregation.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace API_Aggregation.Services
{
    public class AggregationService
    {
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly ICatFactApiClient _catFactApiClient;
        private readonly IArtApiClient _artApiClient;
        private readonly IMemoryCache _cache;
        private readonly RequestStatisticsService _requestStatisticsService;

        public AggregationService(
            IWeatherApiClient weatherApiClient,
            ICatFactApiClient catFactApiClient,
            IArtApiClient artApiClient,
            IMemoryCache cache,
            RequestStatisticsService requestStatisticsService)
        {
            _weatherApiClient = weatherApiClient;
            _catFactApiClient = catFactApiClient;
            _artApiClient = artApiClient;
            _cache = cache;
            _requestStatisticsService = requestStatisticsService;
        }

        public async Task<AggregatedData> GetAggregatedDataAsync(string city, string query, int count, string sortBy = null, string filterBy = null)
        {
            var weatherTask = GetWeatherAsync(city);
            var catFactsTask = GetCatFactsAsync(count);
            var artworkTask = GetArtworksAsync(query);
            //parallel computing
            await Task.WhenAll(weatherTask, catFactsTask, artworkTask);

            var weather = await weatherTask;
            var catFacts = await catFactsTask;
            var artwork = await artworkTask;

            // Apply filtering
            if (!string.IsNullOrEmpty(filterBy))
            {
                catFacts = catFacts.Where(cf => cf.Fact.Contains(filterBy)).ToList();
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "length asc")
                {
                    catFacts = catFacts.OrderBy(cf => cf.Fact.Length).ToList();
                }
                else if (sortBy == "length desc")
                {
                    catFacts = catFacts.OrderByDescending(cf => cf.Fact.Length).ToList();
                }
            }

            return new AggregatedData
                {
                Weather = weather,
                CatFact = catFacts,
                Artwork = artwork
            };
        }
        private async Task<WeatherForecast> GetWeatherAsync(string city)
        {
            if (!_cache.TryGetValue($"Weather_{city}", out WeatherForecast weather))
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                weather = await _weatherApiClient.GetWeatherAsync(city);
                stopwatch.Stop();
                _requestStatisticsService.RecordRequest("WeatherApi", stopwatch.ElapsedMilliseconds);

                _cache.Set($"Weather_{city}", weather, TimeSpan.FromMinutes(10));
            }
            return weather;
        }

        private async Task<List<CatFact>> GetCatFactsAsync(int count)
        {
            if (!_cache.TryGetValue($"CatFacts_{count}", out List<CatFact> catFacts))
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                catFacts = await _catFactApiClient.GetCatFactsAsync(count);
                stopwatch.Stop();
                _requestStatisticsService.RecordRequest("CatFactApi", stopwatch.ElapsedMilliseconds);

                _cache.Set($"CatFacts_{count}", catFacts, TimeSpan.FromMinutes(10));
            }
            return catFacts;
        }

        private async Task<Artwork> GetArtworksAsync(string query)
        {
            if (!_cache.TryGetValue($"Artwork_{query}", out Artwork artwork))
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                artwork = await _artApiClient.GetArtworksAsync(query);
                stopwatch.Stop();
                _requestStatisticsService.RecordRequest("ArtApi", stopwatch.ElapsedMilliseconds);

                _cache.Set($"Artwork_{query}", artwork, TimeSpan.FromMinutes(10));
            }
            return artwork;
        }
    
}
}
