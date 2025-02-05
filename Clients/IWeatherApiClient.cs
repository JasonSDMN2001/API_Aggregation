using API_Aggregation.Models;

namespace API_Aggregation.Clients
{
    public interface IWeatherApiClient
    {
        Task<WeatherForecast> GetWeatherAsync(string city);
    }
}
