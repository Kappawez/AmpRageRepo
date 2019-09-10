using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AmpRageRepo.Models
{
    public class LicensePlateSearcher
    {
        [HttpGet]
        public static async Task<RootObject> FindPlateOnline()
        {
            RootObject rootObject = null;
            var inputRegNumber = "HUU906";
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
                return rootObject;


                //var banan = theReturn.Split(',');
                ////o["report"]["Id"].ToString();
                //var forecast = new ForecastViewModel()
                //{
                //    Time = DateTime.Parse(json["timeSeries"][1]["validTime"].ToString()),
                //    Value = decimal.Parse(json["timeSeries"][1]["validTime"]["name"]["values"].ToString()),
                //    ParamName = json["timeSeries"][1]["validTime"]["name"]["values"].ToString()
                //};
                //if (!response.IsSuccessStatusCode)
                //    throw new Exception(response.ReasonPhrase);

            }


        }




    }
}
