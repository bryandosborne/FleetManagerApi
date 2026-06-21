using FleetManagerApi.Common.Utilities;
using FleetManagerApi.DTOs;
using FleetManagerApi.Models;
using FleetManagerApi.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FleetManagerApi.Services
{
    public class LoadMatchingEngine : ILoadMatchingEngine
    {
        private readonly MatchingOptions _options;

        // DI Container automatically resolves and supplies the configuration instance here
        public LoadMatchingEngine(IOptions<MatchingOptions> options)
        {
            _options = options.Value;
        }

        public DriverMatchResult FindBestDriverForLoad(Load load, List<Driver> availableDrivers)
        {
            var scoringSheet = new List<DriverScore>();

            foreach (var driver in availableDrivers)
            {
                // 1. SPATIAL: Calculate deadhead miles to the load pickup
                double deadheadMiles = DistanceHelper.CalculateDistance(
                    (double)driver.CurrentLatitude, (double)driver.CurrentLongitude,
                    (double)load.PickupLatitude, (double)load.PickupLongitude);

                // 2. TEMPORAL: How long will it take to physically drive to the pickup? Use option config
                double travelHoursToPickup = deadheadMiles / (double)_options.AverageTruckSpeedMph;
                TimeSpan timeToArrive = TimeSpan.FromHours(travelHoursToPickup);

                // 3. COMPLIANCE: Does the driver legally have enough HOS time to get there and finish the load?
                if (driver.RemainingHours < timeToArrive)
                {
                    continue; // Automatically filter out drivers who will violate HOS rules
                }

                // 4. SCORING MATRIX: Apply dynamically configurable weights
                decimal distanceScore = 100 - (decimal)deadheadMiles;
                decimal hosScore = (decimal)driver.RemainingHours.TotalHours * 5;
                decimal totalScore = (distanceScore * _options.DistanceWeight) + (hosScore * _options.HosWeight);

                scoringSheet.Add(new DriverScore { Driver = driver, Score = totalScore, DeadheadMiles = deadheadMiles });
            }

            // Return the driver with the absolute highest optimized score
            var bestMatch = scoringSheet.OrderByDescending(s => s.Score).FirstOrDefault();
            return new DriverMatchResult { BestDriver = bestMatch?.Driver, Score = bestMatch?.Score ?? 0 };
        }
    }
}
