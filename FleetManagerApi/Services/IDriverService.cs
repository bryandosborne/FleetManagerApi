using FleetManagerApi.Models;
using FleetManagerApi.Services.HOS;

namespace FleetManagerApi.Services
{
    public interface IDriverService
    {
        Task<List<Driver>> GetAllDriversAsync();
        Task<Driver?> GetDriverByIdAsync(int id);
        Task<Driver?> UpdateDriverAsync(int id, Driver driver);        
        Task<HosRemainingTime> CalculateRemainingHours(List<LogEvent> historicalLogs, DateTime evaluationTime);
    }
}
