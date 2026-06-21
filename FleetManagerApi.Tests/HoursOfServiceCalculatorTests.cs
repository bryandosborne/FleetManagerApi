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

        [Theory]
        [InlineData(10)] // Test case A: Checks the 10-hour daily shift reset threshold
        [InlineData(34)] // Test case B: Checks the 34-hour weekly restart threshold
        public void FindLastResetTime_ShouldReturnOldestLog_WhenNoResetThresholdIsMet(int requiredResetHours)
        {
            // ====================================================================
            // 1. ARRANGE
            // ====================================================================
            // The fixed time anchor keeps our evaluation completely deterministic.
            var evaluationTime = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
            DateTime oldestLogTimestamp = evaluationTime.AddDays(-5);

            // Simulate a driver who did non-stop active work (No OffDuty or Sleeper logs)
            var historicalLogs = new List<LogEvent>
    {
        new LogEvent { Timestamp = oldestLogTimestamp, Status = DutyStatus.OnDutyNotDriving },
        new LogEvent { Timestamp = evaluationTime.AddDays(-2), Status = DutyStatus.Driving }
    };

            // Map our parameterized input integer straight into a strongly-typed TimeSpan
            TimeSpan requiredResetThreshold = TimeSpan.FromHours(requiredResetHours);

            // ====================================================================
            // 2. ACT
            // ====================================================================
            DateTime actualResetTime = _calculator.FindLastResetTime(historicalLogs, requiredResetThreshold, evaluationTime);

            // ====================================================================
            // 3. ASSERT
            // ====================================================================
            // Because the driver never accumulated a single minute of continuous rest, 
            // the engine must fall back to the very first tracking point in history, 
            // regardless of whether we evaluated a 10-hour window or a 34-hour window.
            Assert.Equal(oldestLogTimestamp, actualResetTime);
        }

    }
}
