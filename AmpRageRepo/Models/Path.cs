﻿using System;
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
            Waypoints = new List<Waypoint>();
            //ChargingStations = new List<ChargingStationRootObject>();
            WayPointStrings = new List<string>();
            ChargeTimes = new List<int?>();
            Emissions = new List<double?>();
        }

        [Required(ErrorMessage = "Start är obligatorisk.")]
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CarBrand { get; set; }
        public string CarMake { get; set; }
        public Car Car { get; set; }

        public bool IgnoreRange { get; set; }
        public bool Arrived { get; set; }

        public int RangeKm { get; set; } //km
        public double EffectiveRangeM { get; set; } //MaxRangeM - MinRangeM
        public double TotalRangeM { get; set; } //Total distance of path
        [Range(0, 100, ErrorMessage = "Räckvidden måste anges i sifferform mellan 0-100")]
        [Required(ErrorMessage = "Räckvidd i procent är obligatorisk")]
        public double CurrentRangeM { get; set; } //TotalMaxRangeM - traveled so far
        public double MinRangeM { get; set; } //MaxRangeM * 0.2
        public double MaxRangeM { get; set; } //RangeKm -> km -> m
        public UserViewModel UserViewModel { get; set; }
        public User User { get; set; }
        public IEnumerable<SelectListItem> AllCarBrands { get; set; }
        public IEnumerable<SelectListItem> AllCarModels { get; set; }
        public IEnumerable<SelectListItem> AllCars { get; set; }
        public IEnumerable<SelectListItem> AllCarsByUser { get; set; }
        public IEnumerable<Car> Cars { get; set; }

        public DirectionRootObject Direction { get; set; }
        public List<Waypoint> Waypoints { get; set; }
        //public List<ChargingStationRootObject> ChargingStations { get; set; }

        //stupid razor pages cannot loop  through WayPoints.CoordString??
        public List<string> WayPointStrings { get; set; }
        public List<int?> ChargeTimes { get; set; }
        public List<double?> Emissions { get; set; }
    }
}
