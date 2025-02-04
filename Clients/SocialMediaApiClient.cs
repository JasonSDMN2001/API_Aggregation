using API_Aggregation.Models;
using Newtonsoft.Json;

namespace API_Aggregation.Clients
{
    public class SocialMediaApiClient
    {
        private readonly HttpClient _httpClient;

        public SocialMediaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TwitterResponse> GetUserTimelineAsync(string username)
        {
            var response = await _httpClient.GetAsync($"/2/tweets?username={username}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TwitterResponse>(content);
        }

    }
}
