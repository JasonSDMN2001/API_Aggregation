using API_Aggregation.Clients;
using API_Aggregation.Models;

namespace API_Aggregation.Services
{
    public class AggregationService
    {
        private readonly WeatherApiClient _weatherApiClient;
        private readonly NewsApiClient _newsApiClient;
        private readonly SocialMediaApiClient _socialMediaApiClient;

        public AggregationService(
            WeatherApiClient weatherApiClient,
            NewsApiClient newsApiClient,
            SocialMediaApiClient socialMediaApiClient
            )
        {
            _weatherApiClient = weatherApiClient;
            _newsApiClient = newsApiClient;
            _socialMediaApiClient = socialMediaApiClient;
        }

        public async Task<AggregatedData> GetAggregatedDataAsync(string city, string country,string username)
        {
            var weatherTask = _weatherApiClient.GetWeatherAsync(city);
            var newsTask = _newsApiClient.GetTopHeadlinesAsync(country);
            var socialMediaTask = _socialMediaApiClient.GetUserTimelineAsync(username);

            await Task.WhenAll(weatherTask, newsTask, socialMediaTask);

            return new AggregatedData
            {
                Weather = await weatherTask,
                News = await newsTask,
                SocialMediaPosts = await socialMediaTask
            };
        }
    }
}
