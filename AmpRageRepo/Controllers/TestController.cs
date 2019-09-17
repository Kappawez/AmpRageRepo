using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmpRageRepo.Models;
using System.Net.Http;

namespace AmpRageRepo.Controllers
{
    public class TestController : Controller
    {
        public TestController(SecretController secret)
        {
            secretController = secret;
            value1 = secretController.GetSecret("GoogleApiKey190916").Result;
            Sync();
        }
        private async void Sync()
        {
            value2 = await secretController.GetSecret("GoogleApiKey190916");
        }

        private SecretController secretController;

        private string value1 = "FAIL_1";
        private string value2 = "FAIL_2";

        public IActionResult Test()
        {
            var test = new TestVault { test1 = value1, test2 = value2 };

            return View(test);
        }
    }
}
