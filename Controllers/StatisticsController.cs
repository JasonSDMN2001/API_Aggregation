using API_Aggregation.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Aggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly RequestStatisticsService _requestStatisticsService;

        public StatisticsController(RequestStatisticsService requestStatisticsService)
        {
            _requestStatisticsService = requestStatisticsService;
        }

        [HttpGet]
        public IActionResult GetStatistics()
        {
            var statistics = _requestStatisticsService.GetStatistics();
            return Ok(statistics);
        }
    }
}