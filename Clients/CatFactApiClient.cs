using API_Aggregation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace API_Aggregation.Clients
{
    public class CatFactApiClient
    {

        private readonly HttpClient _httpClient;

        public CatFactApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<CatFact>> GetCatFactsAsync(int count)
        {
            var url = $"?count={count}";
            var fullUrl = $"{_httpClient.BaseAddress}{url}";

            // Add required headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");


            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(content)["data"];
            var catFacts = new List<CatFact>();

            foreach (var fact in data)
            {
                catFacts.Add(new CatFact { Fact = fact.ToString() });
            }

            return catFacts;
        }
    }
}
