using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public string CarBrand { get; set; }
        public string CarMake { get; set; }
        public Car Car { get; set; }

        public int RangeKm { get; set; } //km
        public double EffectiveRangeM { get; set; } //MaxRangeM - MinRangeM
        public double TotalRangeM { get; set; } //Total distance of path
        [Display(Name ="Current Range in %")]
        [Required(ErrorMessage = "Current Range is required.")]
        public double CurrentRangeM { get; set; } //TotalMaxRangeM - traveled so far
        public double MinRangeM { get; set; } //MaxRangeM * 0.2
        public double MaxRangeM { get; set; } //RangeKm -> km -> m
        public IEnumerable<SelectListItem> AllCarBrands { get; set; }
        public IEnumerable<SelectListItem> AllCarModels { get; set; }


        public string temp;

        //public SecretController SecretController { get; private set; }
        public DirectionRootObject Direction { get; set; }
        public List<CoordinateEntity> WayPoints { get; set; }
        public List<ChargingStationRootObject> ChargingStations { get; set; }
        public List<string> WayPointStrings { get; set; }
    }
}
