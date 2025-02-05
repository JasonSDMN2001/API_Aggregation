using API_Aggregation.Clients;
using API_Aggregation.Models;
using System.Globalization;

namespace API_Aggregation.Services
{
    public class AggregationService
    {
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly ICatFactApiClient _catFactApiClient;
        private readonly IArtApiClient _artApiClient;

        public AggregationService(
            IWeatherApiClient weatherApiClient,
            ICatFactApiClient catFactApiClient,
            IArtApiClient artApiClient)
        {
            _weatherApiClient = weatherApiClient;
            _catFactApiClient = catFactApiClient;
            _artApiClient = artApiClient;
        }

        public async Task<AggregatedData> GetAggregatedDataAsync(string city, string query, int count, string sortBy = null, string filterBy = null)
        {
            var weather = await _weatherApiClient.GetWeatherAsync(city);
            var catFacts = await _catFactApiClient.GetCatFactsAsync(count);
            var artwork = await _artApiClient.GetArtworksAsync(query);

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
    }
}
