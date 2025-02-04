namespace API_Aggregation.Models
{
    public class AggregatedData
    {
        public WeatherForecast Weather { get; set; }
        public List<CatFact> CatFact { get; set; }
        public Artwork Artwork { get; set; }
    }
}
