using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AmpRageRepo.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace AmpRageRepo.Controllers
{
    public class PathController : Controller
    {
        public PathController(SecretController secret)
        {
            apiKey = secret.GetSecret("GoogleApiKey190916").Result;
            //Sync(secret);
        }
        //private async void Sync(SecretController secret)
        //{
        //    apiKey = await secret.GetSecret("GoogleApiKey190916");
        //}

        public static string apiKey = "FAIIIILLLLLLLLLLL";

        public IActionResult CreatePath()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePath(Path path)
        {
            if (path.RangeKm == 0)
            {
                //var car = await LicensePlateSearcher.FindPlate(path.LicensePlate);
                path.RangeKm = 350;
                path.MaxRangeM = (path.RangeKm * 1000);    //km -> m
                path.MinRangeM = (path.RangeKm * 1000 * 0.2); //20% of MaxRangeM
                path.EffectiveRangeM = path.MaxRangeM - path.MinRangeM; //diff
                path.CurrentRangeM = 50 * 1000;
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
                        throw new Exception();

                    x++;
                    direction = await Google_GetDirection(path);
                }
            }
            catch
            {
                throw new Exception("LOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOP");
            }

            path.Direction = direction;

            return RedirectToAction(nameof(DisplayPath), path);
        }
        public IActionResult DisplayPath(Path path)
        {
            return View(path);
        }
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
        private string GetRequestString(Path path)
        {
            //documentation: https://developers.google.com/maps/documentation/directions/intro

            string request = "https://maps.googleapis.com/maps/api/directions/json?";

            request += $"origin={path.Origin}";
            request += $"&destination={path.Destination}";

            for (int i = 0; i < path.WayPoints.Count; i++)
            {
                string coord = path.WayPoints[i].Latitude + "," + path.WayPoints[i].Longitude;

                if (i == 0)
                    request += $"&waypoints={coord}";
                if (i > 0)
                    request += $"|{coord}";
            }

            request += $"&key={apiKey}";

            return request;
        }
        private async Task<bool> EvaluateDirection(DirectionRootObject direction, Path path)
        {
            try
            {
                List<Step> stepList = new List<Step>();

                var currentRoute = direction.routes[direction.routes.Count - 1];
                var currentLeg = currentRoute.legs[currentRoute.legs.Count - 1];

                foreach (var step in currentLeg.steps)
                {
                    stepList.Add(step);

                    if ((path.CurrentRangeM - step.distance.value) <= 0)
                    {
                        return await FindChargingStation(stepList, path);
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

            return true;
        }
        private async Task<bool> FindChargingStation(List<Step> steps, Path path)
        {
            try
            {
                //TODO get a list of coordEntity
                var coordEntity = await GetChargingStationLocation(steps, path);

                //TODO get a list of chargintStation for each coordEntity
                //TODO choose chargintStation based on other things than shortest route (timeToCharge etc)
                var chargingStation = await OpenChargeMap_GetChargingStationInfo(coordEntity);

                path.WayPoints.Add(coordEntity);
                path.WayPointStrings.Add(coordEntity.Latitude + "," + coordEntity.Longitude);
                path.ChargingStations.Add(chargingStation);

                //TODO add timeToCharge to traveltime
                //var timeToCharge = ChargingCalculator(35.8, chargingStation);

                //TODO logic for charging car
                //path.TotalRangeM += path.MaxRangeM;
                path.CurrentRangeM = path.EffectiveRangeM;

                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private int ChargingCalculator(double carCapacity, ChargingStationRootObject chargingStation)
        {
            double carWatt = carCapacity * 60 * 60; //Wh -> W
            double stationWatt = chargingStation.Connections[0].PowerKW;

            return (int)Math.Round(carWatt / stationWatt);
        }
        private async Task<ChargingStationRootObject> OpenChargeMap_GetChargingStationInfo(CoordinateEntity coordEntity)
        {
            //documentation https://openchargemap.org/site/develop/api

            List<ChargingStationRootObject> rootObjects = null;

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://api.openchargemap.io/v3/poi/?output=json");

                var responseTask = client.GetAsync(
                    $"https://api.openchargemap.io/v3/poi/?output=json&maxresults=1&compact=true&verbose=false&" +
                    $"latitude={coordEntity.Latitude}&longitude={coordEntity.Longitude}"
                    );

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
        private async Task<CoordinateEntity> GetChargingStationLocation(List<Step> steps, Path path)
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

                    //return new CoordinateEntity
                    //{
                    //    Address = result.formatted_address,
                    //    Latitude = double.Parse(motherf[0]),
                    //    Longitude = double.Parse(motherf[1])
                    //};

                    if (search.results != null && search.results.Count > 0)
                    {
                        //Choose the first result
                        //TODO return a list
                        var result = search.results[0];
                        return new CoordinateEntity
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
        private async Task<TextSearchRootObject> Google_SearchForChargingStations(string coord)
        {
            TextSearchRootObject rootObject = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("Https://maps.googleapis.com/maps/api/place/textsearch/");

                var responseTask = client.GetAsync(
                    $"json?query=charging+station&location={coord}&radius=10000&key={apiKey}"
                    );

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

        private string GetMaxCoord(List<CoordinateEntity> coords, Path path)
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

        #region Decode
        /// <summary>
        /// https://gist.github.com/shinyzhu/4617989
        /// </summary>
        private static IEnumerable<CoordinateEntity> Decode(string encodedPoints)
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

                yield return new CoordinateEntity
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