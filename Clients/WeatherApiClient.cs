using API_Aggregation.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace API_Aggregation.Clients
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        public WeatherApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherForecast> GetWeatherAsync(string city)
        {
            var url = $"{city}?format=j1";
            var fullUrl = $"{_httpClient.BaseAddress}{url}";
            var response = await _httpClient.GetAsync($"{city}?format=j1");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var weatherData = JObject.Parse(content);
            var currentCondition = weatherData["current_condition"].First();

            var weatherForecast = new WeatherForecast
            {
                temp_C = (double)currentCondition["temp_C"],
                humidity = (int)currentCondition["humidity"],
                windspeedKmph = (double)currentCondition["windspeedKmph"],
                uvIndex = (int)currentCondition["uvIndex"],
            };

            return weatherForecast;
        }
    }
}
