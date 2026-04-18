using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IReportRepository
    {
        Task<Guid> SaveReportAsync(UserStatisticsReport report);
        Task<UserStatisticsReport> GetReportAsync(Guid id);
    }
}
