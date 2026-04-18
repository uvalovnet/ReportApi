using Microsoft.AspNetCore.Mvc;
using ReportApi.Models.DTOs;
using ReportApi.Services;

namespace ReportApi.Controllers
{
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService) => _reportService = reportService;

        [HttpPost("user_statistics")]
        public async Task<IActionResult> CreateUserStatistics([FromBody] CreateReportRequest request)
        {
            var queryId = await _reportService.CreateReportAsync(request);
            return Ok(new { query = queryId.ToString() });
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetReportInfo([FromQuery] Guid query)
        {
            try
            {
                var status = await _reportService.GetReportStatusAsync(query);
                return Ok(status);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Запрос не найден" });
            }
        }
    }
}
