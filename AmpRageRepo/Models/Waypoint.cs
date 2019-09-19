using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class Waypoint
    {
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeToCharge { get; set; }
        public string CoordString { get; set; }
        public double Emissions { get; set; }

        public ChargingStationRootObject ChargingStation { get; set; }
    }
}
