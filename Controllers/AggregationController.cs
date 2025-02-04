using API_Aggregation.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace API_Aggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly AggregationService _aggregationService;

        public AggregationController(AggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet("aggregate")]
        public async Task<IActionResult> GetAggregatedData([FromQuery] string city, [FromQuery] string query,[FromQuery] int count,[FromQuery] string sortBy, [FromQuery] string filterBy)
        {
            try
            {
                var data = await _aggregationService.GetAggregatedDataAsync(city, query,count, sortBy, filterBy);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
