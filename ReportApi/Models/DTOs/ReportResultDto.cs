using System.Text.Json.Serialization;

namespace ReportApi.Models.DTOs
{
    public class ReportResultDto
    {
        [JsonPropertyName("user_id")] 
        public Guid UserId { get; set; }

        [JsonPropertyName("count_sign_in")] 
        public int CountSignIn { get; set; }
    }
}
