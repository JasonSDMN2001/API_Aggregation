using API_Aggregation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace API_Aggregation.Clients
{
    public class ArtApiClient
    {
        private readonly HttpClient _httpClient;

        public ArtApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Artwork> GetArtworksAsync(string query)
        {
            var url = $"api/v1/artworks/search?q={query}";
            var fullUrl = $"{_httpClient.BaseAddress}{url}";
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();


            var data = JObject.Parse(content);
            var a = data["data"].First();

            var artwork = new Artwork
            {
                api_model = (string)a["api_model"],
                id = (int)a["id"],
                is_boosted = (bool)a["is_boosted"],
                api_link = (string)a["api_link"],
                title = (string)a["title"]
            };

            return artwork;
        }

    }
}
