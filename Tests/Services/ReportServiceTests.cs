using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ReportApi.Models.DTOs;
using ReportApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class ReportServiceTests
    {
        private static AppDbContext CreateInMemoryContext() =>
            new(new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);

        private static IReportRepository CreateReportRepository(AppDbContext context) =>
            new ReportRepository(context);

        private static IConfiguration CreateOptions(int timeoutMs) =>
            new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>() { { "ProcessingConfig:TimeoutMs", timeoutMs.ToString() } } )
            .Build();

        [Fact]
        public async Task CreateReportAsync_ReturnsNewGuid_AndSavesToDb()
        {
            await using var context = CreateInMemoryContext();
            var service = new ReportService(CreateReportRepository(context), CreateOptions(2000));

            var id = await service.CreateReportAsync(new CreateReportRequest
            {
                UserId = Guid.NewGuid(),
                PeriodStart = DateTime.UtcNow,
                PeriodEnd = DateTime.UtcNow.AddDays(1)
            });

            Assert.NotEqual(Guid.Empty, id);
            Assert.Equal(1, await context.Reports.CountAsync());
        }

        [Fact]
        public async Task GetReportStatusAsync_CalculatesProgress_Correctly()
        {
            await using var context = CreateInMemoryContext();
            var service = new ReportService(new ReportRepository(context), CreateOptions(2000));

            var queryId = await service.CreateReportAsync(new CreateReportRequest
            {
                UserId = Guid.NewGuid(),
                PeriodStart = DateTime.UtcNow,
                PeriodEnd = DateTime.UtcNow
            });

            await Task.Delay(1000);
            var status = await service.GetReportStatusAsync(queryId);
            Assert.InRange(status.Percent, 40.0, 60.0);
            Assert.Null(status.Result);

            await Task.Delay(1000);
            status = await service.GetReportStatusAsync(queryId);
            Assert.Equal(100.0, status.Percent);
        }
    }
}
