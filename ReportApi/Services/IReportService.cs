using ReportApi.Models.DTOs;

namespace ReportApi.Services
{
    public interface IReportService
    {
        Task<Guid> CreateReportAsync(CreateReportRequest request);
        Task<ReportStatusDto> GetReportStatusAsync(Guid queryId);
    }
}
