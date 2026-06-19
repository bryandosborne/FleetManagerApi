namespace FleetManagerApi.Services.HOS
{
    public class HoursOfServiceCalculator
    {
        // Core FMCSA Rule Constants
        private static readonly TimeSpan MaxDrivingLimit = TimeSpan.FromHours(11);
        private static readonly TimeSpan MaxOnDutyWindow = TimeSpan.FromHours(14);
        private static readonly TimeSpan MaxDrivingWithoutBreak = TimeSpan.FromHours(8);
        private static readonly TimeSpan MaxWeeklyOnDuty = TimeSpan.FromHours(70);

        private static readonly TimeSpan RequiredShiftReset = TimeSpan.FromHours(10);
        private static readonly TimeSpan RequiredWeeklyReset = TimeSpan.FromHours(34);
        private static readonly TimeSpan RequiredBreakDuration = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Calculates remaining available hours at a specific evaluation time based on historical logs.
        /// </summary>
        public HosRemainingTime CalculateRemainingHours(List<LogEvent> historicalLogs, DateTime evaluationTime)
        {
            if (historicalLogs == null || !historicalLogs.Any())
            {
                return GetFullLimits();
            }

            // 1. Sort logs sequentially
            var sortedLogs = historicalLogs.OrderBy(l => l.Timestamp).ToList();

            // 2. Identify the active 34-Hour Weekly Reset
            DateTime weeklyResetStart = FindLastResetTime(sortedLogs, RequiredWeeklyReset, evaluationTime);

            // 3. Identify the active 10-Hour Shift Reset (Must occur at or after the last weekly reset)
            DateTime shiftResetStart = FindLastResetTime(sortedLogs, RequiredShiftReset, evaluationTime);
            if (shiftResetStart < weeklyResetStart)
            {
                shiftResetStart = weeklyResetStart;
            }

            // 4. Calculate accumulated metrics within relevant windows
            TimeSpan drivingTimeSinceShiftReset = TimeSpan.Zero;
            TimeSpan weeklyOnDutyAccumulated = TimeSpan.Zero;

            DateTime continuousDrivingStart = DateTime.MinValue;
            DateTime lastBreakEnd = shiftResetStart;

            // Define the 8-day rolling window start for the 70-hour calculation
            DateTime rollingWeeklyStart = evaluationTime.AddDays(-8);
            DateTime weeklyCalculationStart = rollingWeeklyStart > weeklyResetStart ? rollingWeeklyStart : weeklyResetStart;

            for (int i = 0; i < sortedLogs.Count; i++)
            {
                var current = sortedLogs[i];
                DateTime currentSegmentStart = current.Timestamp;
                DateTime currentSegmentEnd = (i + 1 < sortedLogs.Count) ? sortedLogs[i + 1].Timestamp : evaluationTime;

                // Restrict processing to timeline up to evaluation time
                if (currentSegmentStart >= evaluationTime) break;
                if (currentSegmentEnd > evaluationTime) currentSegmentEnd = evaluationTime;

                TimeSpan duration = currentSegmentEnd - currentSegmentStart;

                // Track Weekly On-Duty Hours (OnDuty or Driving) within the active weekly window
                if (currentSegmentEnd > weeklyCalculationStart)
                {
                    DateTime activeStart = currentSegmentStart > weeklyCalculationStart ? currentSegmentStart : weeklyCalculationStart;
                    TimeSpan activeDuration = currentSegmentEnd - activeStart;

                    if (current.Status == DutyStatus.OnDutyNotDriving || current.Status == DutyStatus.Driving)
                    {
                        weeklyOnDutyAccumulated += activeDuration;
                    }
                }

                // Track Daily Metrics within the active Shift window
                if (currentSegmentEnd > shiftResetStart)
                {
                    DateTime activeStart = currentSegmentStart > shiftResetStart ? currentSegmentStart : shiftResetStart;
                    TimeSpan activeDuration = currentSegmentEnd - activeStart;

                    if (current.Status == DutyStatus.Driving)
                    {
                        drivingTimeSinceShiftReset += activeDuration;
                    }
                }

                // Track 30-minute Rest Breaks to evaluate the 8-hour driving limit
                // Valid breaks can be OffDuty, SleeperBerth, or OnDutyNotDriving (per latest rule update)
                if (current.Status != DutyStatus.Driving && duration >= RequiredBreakDuration)
                {
                    lastBreakEnd = currentSegmentEnd;
                }
            }

            // 5. Compute driving time accumulated since the last qualifying 30-minute rest break
            TimeSpan drivingSinceLastBreak = TimeSpan.Zero;
            DateTime breakTrackingStart = lastBreakEnd > shiftResetStart ? lastBreakEnd : shiftResetStart;

            for (int i = 0; i < sortedLogs.Count; i++)
            {
                var current = sortedLogs[i];
                DateTime start = current.Timestamp;
                DateTime end = (i + 1 < sortedLogs.Count) ? sortedLogs[i + 1].Timestamp : evaluationTime;

                if (end <= breakTrackingStart) continue;
                if (start >= evaluationTime) break;

                DateTime activeStart = start > breakTrackingStart ? start : breakTrackingStart;
                DateTime activeEnd = end < evaluationTime ? end : evaluationTime;

                if (current.Status == DutyStatus.Driving)
                {
                    drivingSinceLastBreak += (activeEnd - activeStart);
                }
            }

            // 6. Final rule checking calculations
            TimeSpan remainingDriving = MaxDrivingLimit - drivingTimeSinceShiftReset;

            TimeSpan timeSinceShiftStart = evaluationTime - shiftResetStart;
            TimeSpan remainingOnDutyWindow = MaxOnDutyWindow - timeSinceShiftStart;

            TimeSpan remainingBeforeBreak = MaxDrivingWithoutBreak - drivingSinceLastBreak;
            TimeSpan remainingWeekly = MaxWeeklyOnDuty - weeklyOnDutyAccumulated;

            // Bind values to zero if limits are already exceeded
            return new HosRemainingTime
            {
                RemainingDrivingTime = remainingDriving < TimeSpan.Zero ? TimeSpan.Zero : remainingDriving,
                RemainingOnDutyWindow = remainingOnDutyWindow < TimeSpan.Zero ? TimeSpan.Zero : remainingOnDutyWindow,
                RemainingTimeBeforeBreak = remainingBeforeBreak < TimeSpan.Zero ? TimeSpan.Zero : remainingBeforeBreak,
                RemainingWeeklyHours = remainingWeekly < TimeSpan.Zero ? TimeSpan.Zero : remainingWeekly
            };
        }

        /// <summary>
        /// Scans backward to locate the end time of the most recent continuous rest block matching a targeted threshold.
        /// </summary>
        private DateTime FindLastResetTime(List<LogEvent> sortedLogs, TimeSpan requiredResetDuration, DateTime evaluationTime)
        {
            DateTime lastResetEndTime = sortedLogs.First().Timestamp;

            for (int i = 0; i < sortedLogs.Count; i++)
            {
                var current = sortedLogs[i];

                // Resets are achieved when a driver is completely off-duty or inside the sleeper berth
                if (current.Status == DutyStatus.OffDuty || current.Status == DutyStatus.SleeperBerth)
                {
                    DateTime start = current.Timestamp;
                    DateTime end = (i + 1 < sortedLogs.Count) ? sortedLogs[i + 1].Timestamp : evaluationTime;

                    if (end - start >= requiredResetDuration)
                    {
                        lastResetEndTime = end;
                    }
                }
            }
            return lastResetEndTime;
        }

        private HosRemainingTime GetFullLimits()
        {
            return new HosRemainingTime
            {
                RemainingDrivingTime = MaxDrivingLimit,
                RemainingOnDutyWindow = MaxOnDutyWindow,
                RemainingTimeBeforeBreak = MaxDrivingWithoutBreak,
                RemainingWeeklyHours = MaxWeeklyOnDuty
            };
        }
    }
}
