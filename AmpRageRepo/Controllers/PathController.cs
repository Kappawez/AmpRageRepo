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
        public IActionResult CreatePath()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePath(Path path)
        {
            var directions = await GetDirection(path);

            if (await EvaluateDirection(directions, path) == false)
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
        [HttpGet]
        public async Task<DirectionRootObject> GetDirection(Path path)
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
        public string GetRequestString(Path path)
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

            request += "&key=AIzaSyCIzHA5mZwa3FIinpH-qpuSg4NNFUnrOj8";

            return request;
        }
        public async Task<bool> EvaluateDirection(DirectionRootObject direction, Path path)
        {
            try
            {
                int distance = 0;
                List<Step> stepList = new List<Step>();

                foreach (var route in direction.routes)
                {
                    foreach (var leg in route.legs)
                    {
                        foreach (var step in leg.steps)
                        {
                            stepList.Add(step);
                            distance += step.distance.value;

                            if (distance > (path.Range * 1000))
                            {
                                string address = await GetChargingStation(stepList, path);

                                //Temp fins one charging station and adds a million range
                                path.WayPoints.Add(address);
                                path.Range += 1000000;

                                return false;
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
        public async Task<string> GetChargingStation(List<Step> steps, Path path)
        {
            //documentation: https://developers.google.com/places/web-service/search
            //Magic numbers to not do too many api requests
            int maxSearch = 10;
            int currentSearch = 0;

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[steps.Count - (i + 1)];

                var coordEntity = Decode(step.polyline.points).Reverse().ToList();

                foreach (var item in coordEntity)
                {
                    var dist = DistanceBetweenCoords(step.start_location.lat, step.start_location.lng, item.Latitude, item.Longitude, 'K');

                    if (dist > path.Range || currentSearch > maxSearch)
                        continue;

                    string coord = item.Latitude + "," + item.Longitude;

                    currentSearch++;
                    var textSearch = await TextSearch(coord);

                    if (textSearch.results != null && textSearch.results.Count > 0)
                    {
                        return textSearch.results[0].formatted_address;
                    }
                }
            }

            return null;
        }
        public async Task<TextSearchRootObject> TextSearch(string coord)
        {
            TextSearchRootObject rootObject = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("Https://maps.googleapis.com/maps/api/place/textsearch/");

                var responseTask = client.GetAsync(
                    $"json?query=charging+station&location={coord}&radius=5000&key=AIzaSyCIzHA5mZwa3FIinpH-qpuSg4NNFUnrOj8"
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

        #region Decode
        /// <summary>
        /// https://gist.github.com/shinyzhu/4617989
        /// </summary>
        public struct CoordinateEntity
        {
            public double Latitude; public double Longitude;
            public CoordinateEntity(double x, double y) { this.Latitude = x; this.Longitude = y; }
        }
        public static IEnumerable<CoordinateEntity> Decode(string encodedPoints)
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