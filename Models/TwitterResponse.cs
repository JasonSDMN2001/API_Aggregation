namespace API_Aggregation.Models
{
    public class TwitterResponse
    {
        public List<Tweet> Data { get; set; }
    }

    public class Tweet
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string CreatedAt { get; set; }
    }
}
