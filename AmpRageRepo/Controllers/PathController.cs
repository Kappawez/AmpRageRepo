using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AmpRageRepo.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace AmpRageRepo.Controllers
{
    public class PathController : Controller
    {
        private readonly AmpContext _context;

        public PathController(AmpContext ampContext)
        {
            _context = ampContext;
        }

        public static string apiKey = "AIzaSyBhIgKBChJZ9HwlAS5FdKkMFKuneDc8RjY";

        public async Task<IActionResult> Snabb()
        {
            //Method with hardcoded vaues for testing
            var path = new Path
            {
                Car = new Car { Capacity = 75 },
                RangeKm = 350,
                Origin = "60.675216,17.140548",
                Destination = "62.389899,17.306923",
                CurrentRangeM = 10
            };

            path.MaxRangeM = (path.RangeKm * 1000);    //km -> m
            path.MinRangeM = (path.RangeKm * 1000 * 0.2); //20% of MaxRangeM
            path.EffectiveRangeM = path.MaxRangeM - path.MinRangeM; //diff
            path.CurrentRangeM = path.MaxRangeM * (path.CurrentRangeM / 100);  //50 * 1000;

            if(path.Destination == null || path.Destination == string.Empty)
            {
                try
                {
                    path.IgnoreRange = true;
                    await GetChargingStationLocationAsDestination(path);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }


            var direction = await Google_GetDirection(path);

            try
            {
                int x = 0;

                while (await EvaluateDirection(direction, path) == false)
                {
                    //A path could have more than 20 waypoints
                    //Temp solution to prevent infinite loops
                    if (x >= 9)
                        throw new Exception("LOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOP");

                    x++;
                    direction = await Google_GetDirection(path);
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }

            return RedirectToAction(nameof(DisplayPath), path);
        }

        public IActionResult CreatePath(UserViewModel user)
        {
            if (_context.Cars.Count() == 0)
            {
                return RedirectToAction("Setup", "Setup");
            }

            var path = new Path()
            {
                User = new User() { Name = "" },
                //AllCarBrands = LicensePlateSearcher.GetAllBrands().Select(x => new SelectListItem
                //{
                //    Text = x,
                //    Value = x.ToString()
                //}),
                //AllCarModels = LicensePlateSearcher.GetAllModels().Select(x => new SelectListItem
                //{
                //    Text = x,
                //    Value = x.ToString()
                //}),
                //AllCars = LicensePlateSearcher.GetAllCars().Select(x => new SelectListItem
                //{
                //    Text = x.Brand,
                //    Value = x.Make.ToString().Replace(';', ' ') + $" - ({x.Range}km)"
                //}),
                //Cars = _context.Cars.ToList()
            };
            if (user.Name == null)
            {
                path.User.Name = "Gästläge";
            } 
            else
            {
                path.User = _context.Users.Where(x => x.Name == user.Name && x.Phone == user.Phone && x.Password == user.Password).Include(x => x.UserCars).ThenInclude(x => x.Car).FirstOrDefault();
            }

            return View(path);
        }
        [HttpPost]
        //public async Task<IActionResult> CreatePath(Path path)
        public async Task<IActionResult> CreatePath(Path path)
        {
            //path.Car = LicensePlateSearcher.CheckForCarInDatabase(path.CarBrand, path.CarMake);

            ////If car couldnt be found set it to a default one
            //if (path.Car == null)
            //    path.Car = LicensePlateSearcher.CheckForCarInDatabase("BMW", "iX3");


            path.RangeKm = path.Car.Range;

            path.MaxRangeM = (path.RangeKm * 1000);    //km -> m
            path.MinRangeM = (path.RangeKm * 1000 * 0.2); //20% of MaxRangeM
            path.EffectiveRangeM = path.MaxRangeM - path.MinRangeM; //diff
            path.CurrentRangeM = path.MaxRangeM * (path.CurrentRangeM / 100);  //50 * 1000;

            //If no entered destination set it to the closest charging station
            if (path.Destination == null || path.Destination == string.Empty)
            {
                try
                {
                    path.IgnoreRange = true;
                    await GetChargingStationLocationAsDestination(path);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            var direction = await Google_GetDirection(path);

            try
            {
                int loop = 0;

                while (await EvaluateDirection(direction, path) == true)
                {
                    //A path could have more than 20 waypoints
                    //Temp solution to prevent infinite loops
                    if (loop >= 9)
                        throw new Exception("LOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOP");

                    loop++;
                    direction = await Google_GetDirection(path);
                }

                if(path.Arrived == false)
                {
                    throw new Exception("Couldn't make a path");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return RedirectToAction(nameof(DisplayPath), path);
        }
        public IActionResult DisplayPath(Path path)
        {
            return View(path);
        }
       
        private string GetRequestString(Path path)
        {
            //documentation: https://developers.google.com/maps/documentation/directions/intro

            string request = "https://maps.googleapis.com/maps/api/directions/json?";

            request += $"origin={path.Origin}";
            request += $"&destination={path.Destination}";

            if(path.IgnoreRange == false)
            {
                for (int i = 0; i < path.Waypoints.Count; i++)
                {
                    string coord = path.Waypoints[i].CoordString;

                    if (i == 0)
                        request += $"&waypoints={coord}";
                    if (i > 0)
                        request += $"|{coord}";
                }
            }


            request += $"&key={apiKey}";

            return request;
        }
        private async Task<bool> EvaluateDirection(DirectionRootObject direction, Path path)
        {
            try
            {
                if (path.IgnoreRange == true)
                {
                    path.Arrived = true;
                    return false;
                }
                if(path.Waypoints.Count > 0 && DistanceCheck(direction, path) == false)
                {
                    path.Arrived = false;
                    return false;
                }

                var stepList = new List<Step>();

                var currentRoute = direction.routes[direction.routes.Count - 1];
                var currentLeg = currentRoute.legs[currentRoute.legs.Count - 1];

                foreach (var step in currentLeg.steps)
                {
                    stepList.Add(step);

                    if ((path.CurrentRangeM - step.distance.value) <= 0)
                    {
                        return await FindChargingStationOnStep(stepList, path);
                    }
                    else
                    {
                        path.CurrentRangeM -= step.distance.value;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            path.Arrived = true;
            return false;
        }
        private bool DistanceCheck(DirectionRootObject direction, Path path)
        {
            //If google apis can't find a charging station it defaults back to your geolocation
            //If the ditance between the start and the last waypoint is greater than the distance between
            //the start and the end stop looping.
            var currentRoute = direction.routes[direction.routes.Count - 1];
            var firstLeg = currentRoute.legs[0];
            var lastLeg = currentRoute.legs[currentRoute.legs.Count - 1];
            var currentWaypoint = path.Waypoints[path.Waypoints.Count - 1];

            var startLat = firstLeg.start_location.lat;
            var startLng = firstLeg.start_location.lng;

            var wayPointLat = currentWaypoint.Latitude;
            var wayPointLng = currentWaypoint.Longitude;

            var endLat = lastLeg.end_location.lat;
            var endLng = lastLeg.end_location.lng;

            var startToEnd = DistanceBetweenCoords(startLat, startLng, endLat, endLng, 'M');
            var startToWaypoint = DistanceBetweenCoords(startLat, startLng, wayPointLat, wayPointLng, 'M');

            //Multiplier just to be sure
            if (startToWaypoint > (startToEnd * 5))
                return false;

            return true;
        }
        private async Task<bool> FindChargingStationOnStep(List<Step> steps, Path path)
        {
            try
            {
                //TODO get a list of coordEntity
                var waypoint = await GetChargingStationLocationAsStep(steps, path);

                return await SetupWaypoint(path, waypoint);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<bool> SetupWaypoint(Path path, Waypoint waypoint)
        {
            try
            {
                //TODO get a list of chargintStation for each coordEntity
                //TODO choose chargintStation based on other things than shortest route (timeToCharge etc)
                var chargingStation = await OpenChargeMap_GetChargingStationInfo(waypoint);

                if (chargingStation == null)
                    return false;

                waypoint.ChargingStation = chargingStation;

                waypoint.TimeToCharge = ChargingCalculator(chargingStation, path); //S
                waypoint.Emissions = await GetEmissionsOfCharging(chargingStation, path); //g/co2
                waypoint.CoordString = waypoint.Latitude + "," + waypoint.Longitude;

                //stupid razor pages cannot loop  through WayPoints.CoordString??
                path.WayPointStrings.Add(waypoint.CoordString);
                //path.ChargeTimes.Add(waypoint.TimeToCharge != null ? (int)waypoint.TimeToCharge : 0);
                //path.Emissions.Add(waypoint.Emissions != null ? (double)waypoint.Emissions : 0);
                path.ChargeTimes.Add(waypoint.TimeToCharge);
                path.Emissions.Add(waypoint.Emissions);

                path.Waypoints.Add(waypoint);

                //TODO more logic for charging car
                path.CurrentRangeM = path.EffectiveRangeM;

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<Waypoint> GetChargingStationLocationAsDestination(Path path)
        {
            var chargingLocations = await Google_SearchForChargingStations(path.Origin);
            var chargingLocation = chargingLocations.results[0];

            var waypoint = new Waypoint
            {
                Latitude = chargingLocation.geometry.location.lat,
                Longitude = chargingLocation.geometry.location.lng
            };

            await SetupWaypoint(path, waypoint);

            waypoint.Address = waypoint.ChargingStation.AddressInfo.Title;

            path.Destination = waypoint.ChargingStation.AddressInfo.Latitude + "," + waypoint.ChargingStation.AddressInfo.Longitude;

            return waypoint;
        }
        private async Task<Waypoint> GetChargingStationLocationAsStep(List<Step> steps, Path path)
        {
            //documentation: https://developers.google.com/places/web-service/search
            //Magic numbers to not do too many api requests
            int maxSearch = 9;
            int currentSearch = 0;

            try
            {
                for (int i = 0; i < steps.Count; i++)
                {
                    var step = steps[steps.Count - (i + 1)];

                    var coordEntity = Decode(step.polyline.points).ToList();

                    if (currentSearch >= maxSearch)
                        throw new Exception("Too many requests");

                    var maxCoord = GetMaxCoord(coordEntity, path);
                    //string[] motherf = maxCoord.Split(",");

                    currentSearch++;
                    var search = await Google_SearchForChargingStations(maxCoord);

                    if (search.results != null && search.results.Count > 0)
                    {
                        //Choose the first result
                        //TODO return a list
                        var result = search.results[0];
                        return new Waypoint
                        {
                            Address = result.formatted_address,
                            Latitude = Math.Round(result.geometry.location.lat, 6),
                            Longitude = Math.Round(result.geometry.location.lng, 6)
                        };
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return null;
        }

        private string GetMaxCoord(List<Waypoint> coords, Path path)
        {
            //Returns the coordinate where range runs out

            double lastLat = coords[0].Latitude;
            double lastLng = coords[0].Longitude;

            double currentLat;
            double currentLng;

            var value = path.CurrentRangeM;

            int loop = 0;

            for (int i = 1; i < coords.Count; i++)
            {
                loop++;

                if (loop != 10)
                    continue;

                loop = 0;

                currentLat = coords[i].Latitude;
                currentLng = coords[i].Longitude;

                var amount = DistanceBetweenCoords(lastLat, lastLng, currentLat, currentLng, 'M');

                value -= amount;
                //path.TotalRangeM += amount;

                if (value <= 0)
                    return lastLat + "," + lastLng;

                lastLat = currentLat;
                lastLng = currentLng;
            }

            return lastLat + "," + lastLng;
        }

        #region Calc
        private async Task<double?> GetEmissionsOfCharging(ChargingStationRootObject charging, Path path)
        {
            //https://www.rensmart.com/Calculators/KWH-to-CO2
            var country = charging.AddressInfo.Country.Title; //Sweden
            var countryEmissions = await _context.CountryEmissions.FirstOrDefaultAsync(y => y.Country.ToUpper() == country.ToUpper()); //g/co2 per kWh

            if (countryEmissions == null)
                return null;

            var currentBattery = GetCurrentBattery(path); //kWh
            var maxBattery = (double)path.Car.Capacity; //kWh
            var batteryDiff = maxBattery - currentBattery; //kWh
            var co2 = countryEmissions.KgCo2Kwh; //g/co2
            var emissions = batteryDiff * co2; //g co2 to charge
            var round = Math.Round(emissions, 6);

            return round;
        }
        private int? ChargingCalculator(ChargingStationRootObject chargingStation, Path path)
        {
            var stationCapacity = (double?)chargingStation.Connections.Max(x => x.PowerKW); //kW

            if (stationCapacity == null)
                return null;

            var maxBattery = (double)path.Car.Capacity * 60 * 60; //kWh -> kW
            var currentBattery = GetCurrentBattery(path, "KW");
            var batteryDiff = maxBattery - currentBattery;
            //Choose the heighest capacity connection
            var timeToCharge = (int)Math.Round(batteryDiff / (double)stationCapacity);

            return timeToCharge;
        }
        private double GetCurrentBattery(Path path, string Unit = "KWH")
        {
            //current battery(kWh) / max battery(kWh) = current range(m) / max range(m) ->
            //current battery(kWh) = (current range(m) / max range(m)) * max battery(kWh) 
            var maxBattery = (double)path.Car.Capacity;
            var quotient = path.CurrentRangeM / path.EffectiveRangeM;
            var currentBattery = Math.Clamp((maxBattery * quotient), 0, maxBattery);

            if (Unit == "W")
                currentBattery = currentBattery * (1000 * 60 * 60);
            if (Unit == "KW")
                currentBattery = currentBattery * (60 * 60);

            return currentBattery;
        }
        #endregion

        #region ApiRequests
        private async Task<DirectionRootObject> Google_GetDirection(Path path)
        {
            DirectionRootObject rootObject = null;

            string request = GetRequestString(path);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(request);

                responseTask.Wait();
                //To store result of web api response.   
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings();

                    try
                    {
                        rootObject = JsonConvert.DeserializeObject<DirectionRootObject>(json, settings);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }

            return rootObject;
        }
        private async Task<ChargingStationRootObject> OpenChargeMap_GetChargingStationInfo(Waypoint coordEntity)
        {
            //documentation https://openchargemap.org/site/develop/api
            List<ChargingStationRootObject> rootObjects = null;

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://api.openchargemap.io/v3/poi/?output=json");

                var responseTask = client.GetAsync($"https://api.openchargemap.io/v3/poi/?output=json&maxresults=1&" +
                    $"latitude={coordEntity.Latitude}&longitude={coordEntity.Longitude}");

                responseTask.Wait();
                //To store result of web api response.   
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings();

                    try
                    {
                        rootObjects = JsonConvert.DeserializeObject<List<ChargingStationRootObject>>(json, settings);
                    }
                    catch (Exception e)
                    {

                        throw new Exception(e.Message);
                    }
                }
            }
            return rootObjects[0];
        }

        private async Task<TextSearchRootObject> Google_SearchForChargingStations(string location)
        {
            TextSearchRootObject rootObject = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("Https://maps.googleapis.com/maps/api/place/textsearch/");
                string request = $"json?query=charging+station+{location}&key={apiKey}";
                //$"json?query=charging+station&location={location}&radius=10000&key={apiKey}"

                var locationArray = location.Split(",");

                if(locationArray.Length > 1)
                {
                    var lLeft = locationArray[0].Trim();

                    if(double.TryParse(lLeft, out double x) == true)
                    {
                        request = $"json?query=charging+station&location={location}&radius=10000&key={apiKey}";
                    }
                }

                var responseTask = client.GetAsync(request);

                responseTask.Wait();
                //To store result of web api response.   
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings();

                    try
                    {
                        rootObject = JsonConvert.DeserializeObject<TextSearchRootObject>(json, settings);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
            return rootObject;
        }
        #endregion

        #region Decode
        /// <summary>
        /// https://gist.github.com/shinyzhu/4617989
        /// </summary>
        private static IEnumerable<Waypoint> Decode(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                throw new ArgumentNullException("encodedPoints");

            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                yield return new Waypoint
                {
                    Latitude = Convert.ToDouble(currentLat) / 1E5,
                    Longitude = Convert.ToDouble(currentLng) / 1E5
                };
            }
        }
        #endregion

        #region Distance
        /// <summary>
        /// https://www.geodatasource.com/developers/c-sharp
        /// </summary>
        private double DistanceBetweenCoords(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(DegToRad(lat1)) * Math.Sin(DegToRad(lat2)) + Math.Cos(DegToRad(lat1)) * Math.Cos(DegToRad(lat2)) * Math.Cos(DegToRad(theta));
                dist = Math.Acos(dist);
                dist = RadToDeg(dist);
                dist = dist * 60 * 1.1515;
                if(unit == 'M')
                {
                    dist = dist * 1609.344;
                }
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }
        private double DegToRad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private double RadToDeg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
        #endregion
    }
}