using System;

namespace FleetManagerApi.Common.Utilities
{
    /// <summary>
    /// Core geospatial utility for processing fleet logistics coordinates.
    /// </summary>
    public static class DistanceHelper
    {
        private const double EarthRadiusMiles = 3956;

        /// <summary>
        /// Calculates the straight-line distance between two GPS coordinates in miles using the Haversine formula.
        /// </summary>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusMiles * c;
        }

        private static double ToRadians(double angle) => (Math.PI / 180) * angle;
    }
}
