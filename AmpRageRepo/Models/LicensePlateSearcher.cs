using System;
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
        static AmpContext _contex = new AmpContext();

        [HttpGet]
        public static async Task<Car> FindPlate(Car inputCar)
        {
            //var inputRegNumber = "XJE60L";
            var inputRegNumber = inputCar.LicensePlate;
            SearchedCar searchedCar = CheckForPlateInFile(inputRegNumber);
            if (searchedCar != null)
            {
                return new Car()
                {
                    Id = searchedCar.Id,
                    Brand = searchedCar.Brand,
                    Make = searchedCar.Make,
                    LicensePlate = searchedCar.LicensePlate
                }; ;
            }
            RootObject rootObject = null;
            var apiAddress = $@"https://api.biluppgifter.se/api/v1/vehicle/regno/{inputRegNumber}?api_token=HVoQrD3Lz8iFKKgUKV2VVALvlCbfPkiC2tTl1uaHM9iG2aAtjV9nWguASFc1";

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(apiAddress))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings();
                    //settings.CheckAdditionalContent = true;
                    try
                    {
                        rootObject = JsonConvert.DeserializeObject<RootObject>(json, settings);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }

            }
            var car = FindMatchingCar(rootObject);

            _contex.Add(new SearchedCar()
            {
                Brand = car.Brand,
                Make = car.Make,
                LicensePlate = car.LicensePlate
            });
            _contex.SaveChanges();
            return car;
        }

        private static Car FindMatchingCar(RootObject rootObject)
        {

            var tempMake = rootObject.data.basic.data.make.Split(';');

            var car = _contex.Cars.Where(x => x.Brand.Contains(rootObject.data.basic.data.make)).FirstOrDefault(); // Searching only on brand, not model

            car.LicensePlate = rootObject.data.attributes.regno;

            return car;
        }

        private static SearchedCar CheckForPlateInFile(string inputRegNumber)
        {
            return _contex.SearchedCars.Where(x => x.LicensePlate == inputRegNumber).FirstOrDefault(); ;
        }
    }
}
