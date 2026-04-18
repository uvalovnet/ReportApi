using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportApi.Controllers;
using ReportApi.Models.DTOs;
using ReportApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Tests.Controllers
{
    public class ReportControllerUnitTests
    {
        private readonly Mock<IReportService> _mockService;
        private readonly ReportController _controller;

        public ReportControllerUnitTests()
        {
            _mockService = new Mock<IReportService>();
            _controller = new ReportController(_mockService.Object);
        }

        [Fact]
        public async Task CreateUserStatistics_CallsServiceAndReturnsOk()
        {
            var expectedId = Guid.NewGuid();
            _mockService.Setup(s => s.CreateReportAsync(It.IsAny<CreateReportRequest>()))
                        .ReturnsAsync(expectedId);

            var request = new CreateReportRequest
            {
                UserId = Guid.NewGuid(),
                PeriodStart = DateTime.UtcNow,
                PeriodEnd = DateTime.UtcNow.AddDays(1)
            };

            var result = await _controller.CreateUserStatistics(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _mockService.Verify(s => s.CreateReportAsync(It.IsAny<CreateReportRequest>()), Times.Once);
        }

        [Fact]
        public async Task GetReportInfo_CallsServiceAndReturnsOk()
        {
            var queryId = Guid.NewGuid();
            var expectedStatus = new ReportStatusDto
            {
                Query = queryId,
                Percent = 75.0,
                Result = null
            };
            _mockService.Setup(s => s.GetReportStatusAsync(queryId)).ReturnsAsync(expectedStatus);

            var result = await _controller.GetReportInfo(queryId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var status = Assert.IsType<ReportStatusDto>(okResult.Value);

            Assert.Equal(queryId, status.Query);
            Assert.Equal(75.0, status.Percent);
            _mockService.Verify(s => s.GetReportStatusAsync(queryId), Times.Once);
        }

        [Fact]
        public async Task GetReportInfo_ServiceThrowsNotFound_ReturnsNotFound()
        {
            var queryId = Guid.NewGuid();
            _mockService.Setup(s => s.GetReportStatusAsync(queryId))
                        .Throws<KeyNotFoundException>();

            var result = await _controller.GetReportInfo(queryId);

            Assert.IsType<NotFoundObjectResult>(result);
            _mockService.Verify(s => s.GetReportStatusAsync(queryId), Times.Once);
        }
    }
}
