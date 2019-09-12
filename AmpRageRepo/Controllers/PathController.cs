using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AmpRageRepo.Models;
using System.Net.Http;
using Newtonsoft.Json;
using AmpRageRepo.Models;

namespace AmpRageRepo.Controllers
{
    public class PathController : Controller
    {
        public static string apiKey = "AIzaSyBxb_cf_iJ3-24dffKVrlAfwGEZjTCSkGI";

        public IActionResult CreatePath()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePath(Path path)
        {
            if (path.Range == 0)
            {
                var car = await LicensePlateSearcher.FindPlate(path.LicensePlate);
                path.Range = car.Range;
                path.EffectiveRange = (path.Range * 1000 * 0.8);    //km -> m -> x0.8
            }


            var direction = await GetDirection(path);

            if (await EvaluateDirection(direction, path) == false)
            {
                //recursion?!
                await CreatePath(path);
            }

            return RedirectToAction(nameof(DisplayPath), path);
        }
        public IActionResult DisplayPath(Path path)
        {
            return View(path);
        }
        private async Task<DirectionRootObject> GetDirection(Path path)
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
                if (i == 0)
                    request += $"&waypoints={path.WayPoints[i]}";
                if (i > 0)
                    request += $"|{path.WayPoints[i]}";
            }

            request += $"&key={apiKey}";

            return request;
        }
        private async Task<bool> EvaluateDirection(DirectionRootObject direction, Path path)
        {
            try
            {
                double totalDistance = 0; //total distance for direction
                double lastOkDistance = 0; //last distance that was less than range
                double currentStepValue = 0; //distance for one step

                List<Step> stepList = new List<Step>();

                foreach (var route in direction.routes)
                {
                    foreach (var leg in route.legs)
                    {
                        foreach (var step in leg.steps)
                        {
                            stepList.Add(step);
                            currentStepValue = step.distance.value;
                            totalDistance += currentStepValue;
                            path.EffectiveRange -= currentStepValue;

                            if (path.EffectiveRange < (path.Range * 1000 * 0.1))
                            {
                                path.EffectiveRange = await ChooseChargingStation(stepList, path, lastOkDistance);

                                //By now a waypoint has been added to the direction
                                //redo the direction with the new waypoint
                                return false;
                            }
                            else
                            {
                                lastOkDistance += currentStepValue;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }
        private async Task<double> ChooseChargingStation(List<Step> steps, Path path, double distance)
        {
            //TODO get a list of coordEntity
            var coordEntity = await GetChargingStationLocation(steps, path, distance);

            //TODO get a list of chargintStation for each coordEntity
            //TODO choose chargintStation based on other things than shortest route (timeToCharge etc)
            var chargingStation = await GetChargingStationInfo(coordEntity);

            //TODO add timeToCharge to traveltime
            var timeToCharge = ChargingCalculator(35.8, chargingStation);

            path.WayPoints.Add(coordEntity.Address);

            //Temp fins one charging station and adds a million range
            //TODO logic for charging car
            return (path.Range * 1000 * 0.8);
        }
        private int ChargingCalculator(double carCapacity, ChargingStationRootObject chargingStation)
        {
            double carWatt = carCapacity * 60 * 60; //Wh -> W
            double stationWatt = chargingStation.Connections[0].PowerKW;

            return (int)Math.Round(carWatt / stationWatt);
        }
        private async Task<ChargingStationRootObject> GetChargingStationInfo(CoordinateEntity coordEntity)
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
        private async Task<CoordinateEntity> GetChargingStationLocation(List<Step> steps, Path path, double currentDistance)
        {
            //documentation: https://developers.google.com/places/web-service/search
            //Magic numbers to not do too many api requests
            int maxSearch = 10;
            int currentSearch = 0;

            try
            {
                for (int i = 0; i < steps.Count; i++)
                {
                    var step = steps[steps.Count - (i + 1)];

                    var coordEntity = Decode(step.polyline.points).ToList();

                    if (currentSearch > maxSearch)
                        throw new Exception("Too many requests");

                    var maxCoord = GetMaxCoord(coordEntity, currentDistance, path.EffectiveRange);

                    currentSearch++;
                    var search = await SearchForChargingStations(maxCoord);

                    if (search.results != null && search.results.Count > 0)
                    {
                        //Choose the first result
                        //TODO return a list
                        var result = search.results[0];
                        return new CoordinateEntity {
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
        private async Task<TextSearchRootObject> SearchForChargingStations(string coord)
        {
            TextSearchRootObject rootObject = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("Https://maps.googleapis.com/maps/api/place/textsearch/");

                var responseTask = client.GetAsync(
                    $"json?query=charging+station&location={coord}&radius=5000&key={apiKey}"
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

        private string GetMaxCoord(List<CoordinateEntity> coords, double currentDistance, double maxDistance)
        {
            //Returns the coordinate where range runs out

            double lastLat = coords[0].Latitude;
            double lastLng = coords[0].Longitude;

            double currentLat;
            double currentLng;

            int loop = 0;

            for (int i = 1; i < coords.Count; i++)
            {
                loop++;

                if (loop != 10)
                    continue;

                loop = 0;

                currentLat = coords[i].Latitude;
                currentLng = coords[i].Longitude;

                currentDistance += DistanceBetweenCoords(lastLat, lastLng, currentLat, currentLng, 'M');

                if (currentDistance >= maxDistance)
                    return currentLat + "," + currentLng;

                lastLat = currentLat;
                lastLng = currentLng;
            }

            return null;
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