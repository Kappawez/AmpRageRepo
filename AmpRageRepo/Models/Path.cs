using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AmpRageRepo.Controllers;

namespace AmpRageRepo.Models
{
    public class Path
    {
        public Path()
        {
            WayPoints = new List<CoordinateEntity>();
            ChargingStations = new List<ChargingStationRootObject>();
            WayPointStrings = new List<string>();
        }

        [Required(ErrorMessage = "Orgin is required.")]
        public string Origin { get; set; }
        [Required(ErrorMessage = "Destination is required.")]
        public string Destination { get; set; }
        [Display(Name = "License Plate")]
        [Required(ErrorMessage = "License Plate is required.")]
        [StringLength(6, ErrorMessage = "License Plate must be 6 chars")]
        public string LicensePlate { get; set; }

        public int RangeKm { get; set; } //km
        public double EffectiveRangeM { get; set; } //MaxRangeM - MinRangeM
        public double TotalRangeM { get; set; } //Total distance of path
        public double CurrentRangeM { get; set; } //TotalMaxRangeM - traveled so far
        public double MinRangeM { get; set; } //MaxRangeM * 0.2
        public double MaxRangeM { get; set; } //RangeKm -> km -> m

        public string temp;

        //public SecretController SecretController { get; private set; }
        public DirectionRootObject Direction { get; set; }
        public List<CoordinateEntity> WayPoints { get; set; }
        public List<ChargingStationRootObject> ChargingStations { get; set; }
        public List<string> WayPointStrings { get; set; }
    }
}
