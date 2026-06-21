using FleetManagerApi.Data;
using FleetManagerApi.Models;
using FleetManagerApi.Services.HOS;
using Microsoft.EntityFrameworkCore;

namespace FleetManagerApi.Services
{
    public class DriverService : IDriverService
    {
        private readonly FleetManagerApiContext _db;

        public DriverService(FleetManagerApiContext db)
        {
            _db = db;
        }

        public async Task<HosRemainingTime> CalculateRemainingHours(List<LogEvent> historicalLogs, DateTime evaluationTime)
        {
            var calculator = new HoursOfServiceCalculator();

            if (historicalLogs == null || !historicalLogs.Any())
            {
                return calculator.GetFullLimits();
            }

            return calculator.CalculateRemainingHours(historicalLogs, evaluationTime);
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            var drivers = await _db.Drivers
                .Include(d => d.LogEvents) 
                .ToListAsync();

            var evaluationTime = DateTime.UtcNow;

            foreach (var driver in drivers)
            {
                await EnrichDriverOperationalData(driver, evaluationTime);
            }

            return drivers;
        }

        public async Task<Driver?> GetDriverByIdAsync(int id)
        {
            var driver = await _db.Drivers
                .Include(d => d.LogEvents)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver != null)
            {
                await EnrichDriverOperationalData(driver, DateTime.UtcNow);
            }

            return driver;
        }

        public async Task<Driver?> UpdateDriverAsync(int id, Driver driver)
        {
            var existingDriver = await _db.Drivers.FindAsync(id);

            if (existingDriver == null)
            {
                return null;
            }

            existingDriver.FirstName = driver.FirstName;
            existingDriver.LastName = driver.LastName;
            existingDriver.DriverLicenseNumber = driver.DriverLicenseNumber;
            existingDriver.AssignedTruckId = driver.AssignedTruckId; // Keep truck assignment mutable

            await _db.SaveChangesAsync();
            return existingDriver;
        }
                
        // Internal helper to calculate dynamic HOS limits and extract spatial vectors for the matching engine.        
        private async Task EnrichDriverOperationalData(Driver driver, DateTime evaluationTime)
        {
            // 1. Calculate and bind live HOS remaining time
            var hosLimits = await CalculateRemainingHours(driver.LogEvents, evaluationTime);
            driver.RemainingHours = hosLimits.RemainingDrivingTime; // Maps your calculated Timespan

            // 2. Extract current location and status from their latest sequential log event
            var latestLog = driver.LogEvents
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefault();

            if (latestLog != null)
            {
                driver.CurrentLatitude = latestLog.Latitude;
                driver.CurrentLongitude = latestLog.Longitude;
                driver.CurrentStatus = (DutyStatus)latestLog.Status; 
            }
            else
            {
                driver.CurrentStatus = DutyStatus.OffDuty;
            }
        }
    }
}
