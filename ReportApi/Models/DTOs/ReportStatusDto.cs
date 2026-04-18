using System.Text.Json.Serialization;

namespace ReportApi.Models.DTOs
{
    public class ReportStatusDto
    {
        [JsonPropertyName("query")] 
        public Guid Query { get; set; }
        [JsonPropertyName("percent")] 
        public double Percent { get; set; }
        [JsonPropertyName("result")] 
        public ReportResultDto? Result { get; set; }
    }
}
