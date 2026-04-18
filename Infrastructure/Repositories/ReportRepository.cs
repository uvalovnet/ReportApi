using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context) => _context = context;

        public async Task<Guid> SaveReportAsync(UserStatisticsReport report)
        {

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report.Id;
        }

        public async Task<UserStatisticsReport> GetReportAsync(Guid queryId)
        {
            var x = _context.Reports.ToList();
            return await _context.Reports.FirstOrDefaultAsync(x => x.Id == queryId)
                         ?? throw new KeyNotFoundException("Запрос не найден");
        }
    }
}
