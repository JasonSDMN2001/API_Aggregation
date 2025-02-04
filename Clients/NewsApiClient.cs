using API_Aggregation.Models;
using Newtonsoft.Json;

namespace API_Aggregation.Clients
{
    public class NewsApiClient
    {

        private readonly HttpClient _httpClient;

        public NewsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NewsResponse> GetTopHeadlinesAsync(string country)
        {
            var response = await _httpClient.GetAsync($"/v2/top-headlines?country={country}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NewsResponse>(content);
        }
    }
}
