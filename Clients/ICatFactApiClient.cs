using API_Aggregation.Models;

namespace API_Aggregation.Clients
{
    public interface ICatFactApiClient
    {
        Task<List<CatFact>> GetCatFactsAsync(int count);
    }
}
