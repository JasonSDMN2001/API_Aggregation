using API_Aggregation.Clients;
using API_Aggregation.Models;
using System.Globalization;

namespace API_Aggregation.Services
{
    public class AggregationService
    {
        private readonly WeatherApiClient _weatherApiClient;
        private readonly CatFactApiClient _catFactApiClient;
        private readonly ArtApiClient _artApiClient;

        public AggregationService(
            WeatherApiClient weatherApiClient,
            CatFactApiClient catFactApiClient,
            ArtApiClient artApiClient
            )
        {
            _weatherApiClient = weatherApiClient;
            _catFactApiClient = catFactApiClient;
            _artApiClient = artApiClient;
        }

        public async Task<AggregatedData> GetAggregatedDataAsync(string city, string query, int count, string sortBy = null, string filterBy = null)
        {
            var weatherTask = _weatherApiClient.GetWeatherAsync(city);
            var catTask = _catFactApiClient.GetCatFactsAsync(count);
            var artTask = _artApiClient.GetArtworksAsync(query);

            await Task.WhenAll(weatherTask, catTask, artTask);

            var aggregatedData = new AggregatedData
            {
                Weather = await weatherTask,
                CatFact = await catTask,
                Artwork = await artTask 
            };

            // Apply filtering
            if (!string.IsNullOrEmpty(filterBy))
            {
                aggregatedData.CatFact = aggregatedData.CatFact.Where(cf => cf.Fact.Contains(filterBy, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "asc":
                        aggregatedData.CatFact = aggregatedData.CatFact.OrderBy(a => a.Fact.Length).ToList();
                        break;
                    case "desc":
                        aggregatedData.CatFact = aggregatedData.CatFact.OrderByDescending(a => a.Fact.Length).ToList();
                        break;
                }
            }

            return aggregatedData;
        }
    }
}
