namespace ReportApi.Models.DTOs
{
    public class CreateReportRequest
    {
        public Guid UserId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
