using System.Collections.Concurrent;

namespace API_Aggregation.Services
{
    public class RequestStatisticsService
    {
        private readonly ConcurrentDictionary<string, List<long>> _requestTimes = new();

        public void RecordRequest(string apiName, long responseTime)
        {
            if (!_requestTimes.ContainsKey(apiName))
            {
                _requestTimes[apiName] = new List<long>();
            }
            _requestTimes[apiName].Add(responseTime);
        }

        public Dictionary<string, RequestStatistics> GetStatistics()
        {
            var statistics = new Dictionary<string, RequestStatistics>();

            foreach (var api in _requestTimes.Keys)
            {
                var times = _requestTimes[api];
                var totalRequests = times.Count;
                var averageResponseTime = times.Average();
                var fastRequests = times.Count(t => t < 100);
                var averageRequests = times.Count(t => t >= 100 && t <= 200);
                var slowRequests = times.Count(t => t > 200);

                statistics[api] = new RequestStatistics
                {
                    TotalRequests = totalRequests,
                    AverageResponseTime = averageResponseTime,
                    FastRequests = fastRequests,
                    AverageRequests = averageRequests,
                    SlowRequests = slowRequests
                };
            }

            return statistics;
        }
    }

    public class RequestStatistics
    {
        public int TotalRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public int FastRequests { get; set; }
        public int AverageRequests { get; set; }
        public int SlowRequests { get; set; }
    }
}