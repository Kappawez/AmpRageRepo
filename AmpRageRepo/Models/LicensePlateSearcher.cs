﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace AmpRageRepo.Models
{
    public class LicensePlateSearcher
    {
        static readonly AmpContext _contex = new AmpContext();

        public static Car CheckForCarInDatabase(string carBrand, string carMake)
        {
            var newCarMake = carMake.Split('-')[0].Trim().Replace(' ', ';');
            var car = _contex.Cars.Where(x => x.Brand == carBrand && x.Make == carMake).FirstOrDefault();
          
            return car;
        }

        internal static IEnumerable<string> GetAllBrands()
        {
            var listOfBrands = new List<string>();
            foreach (var car in _contex.Cars.ToList())
            {
                if (!listOfBrands.Contains(car.Brand))
                {
                    listOfBrands.Add(car.Brand);
                }
            }
            return listOfBrands.OrderBy(x => x);
        }

        internal static IEnumerable<string> GetAllModels()
        {
            var listOfModels = new List<string>();
            foreach (var car in _contex.Cars.ToList())
            {
                if (!listOfModels.Contains(car.Make))
                {
                    //listOfModels.Add(car.Make.ToString().Replace(';', ' ') + $" - ({car.Range}km)");
                    listOfModels.Add(car.Make);
                }
            }
            return listOfModels.OrderBy(x => x);
        }

        internal static IEnumerable<object> GetAllCarsByUser()
        {
            throw new NotImplementedException();
        }

        internal static IEnumerable<Car> GetAllCars()
        {
           return _contex.Cars.ToList();

        }
    }
}
