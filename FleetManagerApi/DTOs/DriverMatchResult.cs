using FleetManagerApi.Models;

namespace FleetManagerApi.DTOs
{
    public class DriverMatchResult
    {
        public Driver? BestDriver { get; set; } // 👈 Make sure this line is exactly here
        public decimal Score { get; set; }
    }
}
