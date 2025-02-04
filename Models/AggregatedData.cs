namespace API_Aggregation.Models
{
    public class AggregatedData
    {
        public WeatherForecast Weather { get; set; }
        public NewsResponse News { get; set; }
        public TwitterResponse SocialMediaPosts { get; set; }
    }
}
