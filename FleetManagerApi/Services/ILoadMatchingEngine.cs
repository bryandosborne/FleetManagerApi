
using System.Collections.Generic;
using FleetManagerApi.Models;
using FleetManagerApi.DTOs;

namespace FleetManagerApi.Services
{
    public interface ILoadMatchingEngine
    {
        DriverMatchResult FindBestDriverForLoad(Load load, List<Driver> availableDrivers);
    }
}
