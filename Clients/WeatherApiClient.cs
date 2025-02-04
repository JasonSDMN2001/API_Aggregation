using API_Aggregation.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_Aggregation.Clients
{
    public class WeatherApiClient
    {
        private readonly HttpClient _httpClient;
        public WeatherApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherForecast> GetWeatherAsync(string city)
        {
            var response = await _httpClient.GetAsync($"weather?q={city}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WeatherForecast>(content);
        }
    }
}
