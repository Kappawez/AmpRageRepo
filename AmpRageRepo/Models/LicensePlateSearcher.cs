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

namespace AmpRageRepo.Models
{
    public class LicensePlateSearcher
    {
        [HttpGet]
        public static async Task<Car> FindPlate(Car inputCar)
        {
            //var inputRegNumber = "XJE60L";
            var inputRegNumber = inputCar.LicensePlate;
            var car = CheckForPlateInFile(inputRegNumber);
            if (car.Brand != null)
            {
                return car;
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
            car = FindMatchingCar(rootObject);


            return car;
        }

        private static Car FindMatchingCar(RootObject rootObject)
        {
            bool foundMatch = false;
            var counter = 0;
            var listOfProps = new List<string>();
            string filepath = @"~/data/elcars3.txt";
            foreach (var item in File.ReadAllLines(filepath))
            {
                if (item.ToString().Contains(rootObject.data.basic.data.make))
                {
                    foundMatch = true;
                    listOfProps.Add(item.ToString());
                }
                if (foundMatch && counter != 10) //NUMBER OF LINES IT TAKES + 1)
                {
                    listOfProps.Add(item.ToString());
                    counter++;
                }
            };
            var car = new Car();
            if (listOfProps.Count > 0)
            {
                car = CreateCar(listOfProps);
            }

            return car;
        }

        private static Car CheckForPlateInFile(string inputRegNumber)
        {
            bool foundMatch = false;
            var counter = 0;
            var listOfProps = new List<string>();
            string filepath = @"~/data/elcars4.txt";
            foreach (var item in File.ReadAllLines(filepath))
            {
                if (item.ToString().Contains(inputRegNumber))
                {
                    foundMatch = true;
                    listOfProps.Add(item.ToString());

                }
                if (foundMatch && counter != 10) //NUMBER OF LINES IT TAKES + 1)
                {
                    listOfProps.Add(item.ToString());
                    counter++;
                }
            };
            var car = new Car();
            if (listOfProps.Count > 0)
            {
                car = CreateCar(listOfProps);
            }

            return car;
        }

        private static Car CreateCar(List<string> listOfProps)
        {
            var car = new Car()
            {
                Brand = listOfProps[1],
                Make = listOfProps[2],
                Capacity = decimal.Parse(listOfProps[3]),
                ZeroToHundred = decimal.Parse(listOfProps[4]),
                TopSpeed = int.Parse(listOfProps[5]),
                Range = int.Parse(listOfProps[6]),
                Efficiency = decimal.Parse(listOfProps[7]),
                Fastcharge = int.Parse(listOfProps[8])
            };
            return car;
        }
    }
}
