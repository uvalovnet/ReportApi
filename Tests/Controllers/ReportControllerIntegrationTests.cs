using Infrastructure.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ReportApi.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class ReportApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase($"IntegrationTestDb"));
            });
        }
    }

    public class ReportControllerIntegrationTests : IClassFixture<ReportApiFactory>
    {
        private readonly HttpClient _client;

        public ReportControllerIntegrationTests(ReportApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostUserStatistics_ValidRequest_Returns200AndQueryId()
        {
            var request = new CreateReportRequest
            {
                UserId = Guid.NewGuid(),
                PeriodStart = DateTime.UtcNow,
                PeriodEnd = DateTime.UtcNow.AddDays(1)
            };

            var response = await _client.PostAsJsonAsync("/report/user_statistics", request);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<JsonDocument>();

            Assert.NotNull(json);
            var queryStr = json.RootElement.GetProperty("query").GetString();
            Assert.True(Guid.TryParse(queryStr, out var queryId));
            Assert.NotEqual(Guid.Empty, queryId);
        }

        [Fact]
        public async Task GetReportInfo_ExistingQuery_ReturnsStatus()
        {
            var request = new CreateReportRequest
            {
                UserId = Guid.NewGuid(),
                PeriodStart = DateTime.UtcNow,
                PeriodEnd = DateTime.UtcNow
            };
            var postResponse = await _client.PostAsJsonAsync("/report/user_statistics", request);
            var postJson = await postResponse.Content.ReadFromJsonAsync<JsonDocument>();
            var queryId = Guid.Parse(postJson.RootElement.GetProperty("query").GetString()!);

            var response = await _client.GetAsync($"/report/info?query={queryId}");

            response.EnsureSuccessStatusCode();
            var status = await response.Content.ReadFromJsonAsync<ReportStatusDto>();

            Assert.NotNull(status);
            Assert.Equal(queryId, status.Query);
            Assert.InRange(status.Percent, 0.0, 100.0);
            Assert.Null(status.Result);
        }

        [Fact]
        public async Task GetReportInfo_NonExistingQuery_Returns404()
        {
            var response = await _client.GetAsync("/report/info?query=00000000-0000-0000-0000-000000000000");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
