using FleetManagerApi.Models;

namespace FleetManagerApi.DTOs
{
    /// <summary>
    /// Data Transfer Object representing an individual driver's evaluation score 
    /// and spatial metrics relative to a specific load assignment.
    /// </summary>
    public class DriverScore
    {
        /// <summary>
        /// The domain entity profile of the evaluated driver.
        /// </summary>
        public required Driver Driver { get; set; }

        /// <summary>
        /// The computed operational score derived from the matching engine's weighting matrix.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// The physical empty distance in miles from the driver's last known GPS ping to the load origin yard.
        /// </summary>
        public double DeadheadMiles { get; set; }
    }
}
