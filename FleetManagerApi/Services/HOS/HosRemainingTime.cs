namespace FleetManagerApi.Services.HOS
{
    public class HosRemainingTime
    {
        public TimeSpan RemainingDrivingTime { get; set; }
        public TimeSpan RemainingOnDutyWindow { get; set; }
        public TimeSpan RemainingTimeBeforeBreak { get; set; }
        public TimeSpan RemainingWeeklyHours { get; set; }
        public bool IsViolatingAnyRule =>
            RemainingDrivingTime <= TimeSpan.Zero ||
            RemainingOnDutyWindow <= TimeSpan.Zero ||
            RemainingTimeBeforeBreak <= TimeSpan.Zero ||
            RemainingWeeklyHours <= TimeSpan.Zero;
    }
}
