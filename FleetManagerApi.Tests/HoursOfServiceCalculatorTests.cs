using System;
using System.Collections.Generic;
using System.Linq;
using FleetManagerApi.Models;
using FleetManagerApi.Services.HOS;
using Xunit;

namespace FleetManagerApi.Tests
{
    public class HoursOfServiceCalculatorTests
    {
        private readonly HoursOfServiceCalculator _calculator;

        public HoursOfServiceCalculatorTests()
        {
            // ARRANGE (Global): Initialize our domain engine before each test runs
            _calculator = new HoursOfServiceCalculator();
        }

        [Fact]
        public void FindLastResetTime_ShouldDetectInProgress34HourWeeklyReset_WhenDriverIsCurrentlyResting()
        {
            // ====================================================================
            // 1. ARRANGE
            // ====================================================================
            // Goal: Simulate a driver who did work, then parked 36 hours ago 
            // and is STILL resting up to the exact evaluation time.

            var evaluationTime = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

            var historicalLogs = new List<LogEvent>
            {
                // Driver goes On-Duty 48 hours ago
                new LogEvent
                {
                    Timestamp = evaluationTime.AddDays(-2),
                    Status = DutyStatus.OnDutyNotDriving
                },
                
                // Driver starts Driving 44 hours ago
                new LogEvent
                {
                    Timestamp = evaluationTime.AddHours(-44),
                    Status = DutyStatus.Driving
                },
                
                // CRITICAL STEP: Driver parks and enters Sleeper Berth exactly 36 hours ago.
                // Since this is the LAST log entry, they remain in this state up until evaluationTime.
                new LogEvent
                {
                    Timestamp = evaluationTime.AddHours(-36),
                    Status = DutyStatus.SleeperBerth
                }
            };

            // Test against the FMCSA 34-hour weekly rest requirement constant
            TimeSpan requiredWeeklyReset = TimeSpan.FromHours(34);

            // ====================================================================
            // 2. ACT
            // ====================================================================
            // Execute the backward lookback logic inside our domain calculator engine
            DateTime actualResetTime = _calculator.FindLastResetTime(historicalLogs, requiredWeeklyReset, evaluationTime);

            // ====================================================================
            // 3. ASSERT
            // ====================================================================
            // The driver started resting 36 hours ago. They hit the 34-hour threshold
            // exactly 34 hours AFTER they went into the sleeper berth.
            DateTime expectedResetTime = evaluationTime.AddHours(-36).AddHours(34);

            Assert.Equal(expectedResetTime, actualResetTime);
        }

        [Fact]
        public void FindLastResetTime_ShouldReturnOldestLog_WhenNoResetThresholdIsMet()
        {
            // ====================================================================
            // 1. ARRANGE
            // ====================================================================
            // Goal: Verify fallback behavior. If a driver has been driving or on-duty 
            // non-stop, they have earned zero resets.

            var evaluationTime = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
            DateTime oldestLogTimestamp = evaluationTime.AddDays(-5);

            var historicalLogs = new List<LogEvent>
            {
                new LogEvent { Timestamp = oldestLogTimestamp, Status = DutyStatus.OnDutyNotDriving },
                new LogEvent { Timestamp = evaluationTime.AddDays(-2), Status = DutyStatus.Driving }
            };

            TimeSpan requiredReset = TimeSpan.FromHours(10); // Daily 10-hour shift limit

            // ====================================================================
            // 2. ACT
            // ====================================================================
            DateTime actualResetTime = _calculator.FindLastResetTime(historicalLogs, requiredReset, evaluationTime);

            // ====================================================================
            // 3. ASSERT
            // ====================================================================
            // Because the driver never took a continuous 10-hour off-duty break, 
            // the engine must fall back to the very first tracking point in history.
            Assert.Equal(oldestLogTimestamp, actualResetTime);
        }
    }
}
