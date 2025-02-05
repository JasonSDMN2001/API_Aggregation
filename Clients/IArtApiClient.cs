using API_Aggregation.Models;

namespace API_Aggregation.Clients
{
    public interface IArtApiClient
    {
        Task<Artwork> GetArtworksAsync(string query);
    }
}
