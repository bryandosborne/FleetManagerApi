using System;
using System.Collections.Generic;
using FleetManagerApi.Models;
using FleetManagerApi.Services.HOS;

namespace FleetManagerApi.Data
{
    public static class FleetDataSeeder
    {
        public static List<Driver> GetSimulatedDriversWithLogs()
        {
            DateTime evaluationTime = new DateTime(2026, 7, 20, 12, 0, 0);

            var drivers = new List<Driver>
            {
                // ====================================================================
                // DRIVER 1: Bill McCoy
                // ====================================================================
                new Driver
                {
                    // REMOVED Id = 1
                    FirstName = "Bill", LastName = "McCoy", DriverLicenseNumber = "546-85-8566",
                    LogEvents = new List<LogEvent>
                    {
                        // REMOVED Id = 101 and Id = 102
                        new LogEvent { Status = DutyStatus.Driving, Timestamp = evaluationTime.AddDays(-3), Latitude = 36.9448m, Longitude = -83.1177m },
                        new LogEvent { Status = DutyStatus.OffDuty, Timestamp = evaluationTime.AddDays(-2).AddHours(-10), Latitude = 36.9448m, Longitude = -83.1177m }
                    }
                },

                // ====================================================================
                // DRIVER 2: John Travis
                // ====================================================================
                new Driver
                {
                    // REMOVED Id = 2
                    FirstName = "John", LastName = "Travis", DriverLicenseNumber = "560-64-6844",
                    LogEvents = new List<LogEvent>
                    {
                        // REMOVED manual Ids
                        new LogEvent { Status = DutyStatus.OnDutyNotDriving, Timestamp = evaluationTime.AddHours(-12), Latitude = 32.7766m, Longitude = -96.7971m },
                        new LogEvent { Status = DutyStatus.Driving, Timestamp = evaluationTime.AddHours(-11), Latitude = 32.7766m, Longitude = -96.7971m },
                        new LogEvent { Status = DutyStatus.OffDuty, Timestamp = evaluationTime.AddHours(-6), Latitude = 34.0000m, Longitude = -100.0000m },
                        new LogEvent { Status = DutyStatus.Driving, Timestamp = evaluationTime.AddHours(-5.5), Latitude = 34.0000m, Longitude = -100.0000m },
                        new LogEvent { Status = DutyStatus.Driving, Timestamp = evaluationTime, Latitude = 32.7153m, Longitude = -117.1610m }
                    }
                },

                // ====================================================================
                // DRIVER 3: Calvin Baker
                // ====================================================================
                new Driver
                {
                    // REMOVED Id = 3
                    FirstName = "Calvin", LastName = "Baker", DriverLicenseNumber = "475-45-9632",
                    LogEvents = GenerateHeavyWeeklyLogs(evaluationTime) // Removed driver ID parameter
                },

                // ====================================================================
                // DRIVER 4: Troy Calton
                // ====================================================================
                new Driver
                {
                    // REMOVED Id = 4
                    FirstName = "Troy", LastName = "Calton", DriverLicenseNumber = "445-78-0012",
                    LogEvents = new List<LogEvent>
                    {
                        // REMOVED manual Ids
                        new LogEvent { Status = DutyStatus.OffDuty, Timestamp = evaluationTime.AddHours(-24), Latitude = 38.9250m, Longitude = 79.8433m },
                        new LogEvent { Status = DutyStatus.OnDutyNotDriving, Timestamp = evaluationTime.AddHours(-5), Latitude = 38.9250m, Longitude = 79.8433m },
                        new LogEvent { Status = DutyStatus.Driving, Timestamp = evaluationTime.AddHours(-4.5), Latitude = 38.9250m, Longitude = 79.8433m }
                    }
                }
            };

            return drivers;
        }

        private static List<LogEvent> GenerateHeavyWeeklyLogs(DateTime evaluationTime)
        {
            var logs = new List<LogEvent>();

            for (int i = 6; i >= 1; i--)
            {
                DateTime dayStart = evaluationTime.AddDays(-i).Date.AddHours(6);
                // REMOVED Id and DriverId parameter assignments
                logs.Add(new LogEvent { Status = DutyStatus.OnDutyNotDriving, Timestamp = dayStart, Latitude = 38.6764m, Longitude = 83.7250m });
                logs.Add(new LogEvent { Status = DutyStatus.Driving, Timestamp = dayStart.AddMinutes(30), Latitude = 38.6764m, Longitude = 83.7250m });
                logs.Add(new LogEvent { Status = DutyStatus.OffDuty, Timestamp = dayStart.AddHours(11.5), Latitude = 38.6764m, Longitude = 83.7250m });
            }

            logs.Add(new LogEvent { Status = DutyStatus.OnDutyNotDriving, Timestamp = evaluationTime.AddHours(-1), Latitude = 38.6764m, Longitude = 83.7250m });
            return logs;
        }
    }
}
