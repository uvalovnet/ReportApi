using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Models;
using ReportApi.Models;
using ReportApi.Models.DTOs;
using System.Text.Json;

namespace ReportApi.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IConfiguration _configuration;

        private readonly int _minimalProcessTimeMilliseconds;

        public ReportService(IReportRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
            _minimalProcessTimeMilliseconds = _configuration.GetValue<int>("ProcessingConfig:TimeoutMs", 60000);
        }

        public async Task<Guid> CreateReportAsync(CreateReportRequest request)
        {
            var report = new UserStatisticsReport
            {
                Id = Guid.Empty,
                UserId = request.UserId,
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.SaveReportAsync(report);
        }

        public async Task<ReportStatusDto> GetReportStatusAsync(Guid queryId)
        {
            var report = await _repository.GetReportAsync(queryId);

            var elapsedMs = (DateTime.UtcNow - report.CreatedAt).TotalMilliseconds;
            var percent = Math.Min(100.0, (elapsedMs / _minimalProcessTimeMilliseconds) * 100.0);

            ReportResultDto? result = null;

            if (report.ResultJson != null && percent != 100)
                result = JsonSerializer.Deserialize<ReportResultDto>(report.ResultJson);

            return new ReportStatusDto
            {
                Query = report.Id,
                Percent = (int)percent,
                Result = result
            };
        }
    }
}
