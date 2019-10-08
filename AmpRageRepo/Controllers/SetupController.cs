using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AmpRageRepo.Models;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace AmpRageRepo.Controllers
{
    public class SetupController : Controller
    {
        private readonly AmpContext _context;

        public SetupController(AmpContext ampContext)
        {
            _context = ampContext;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Index()
        {
            return View();
        }
        private IActionResult Continue()
        {
            var x = "hello";

            return RedirectToAction("CreatePath", "Path");
        }
        [HttpPost]
        private async Task<IActionResult> Recreate()
        {
            //Called from button
            try
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
                ViewData["Message"] = "Database recreated";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<IActionResult> Seed()
        {
            //Called from button
            try
            {
                var cars = await SeedCars();
                var emissions = await SeedEmissions();

                if (cars == false || emissions == false)
                    throw new Exception("Failed to seed!");

                ViewData["Message"] = "Seeding done";
                return View("Index");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<bool> SeedCars()
        {
            try
            {
                using (var reader = new StreamReader("DATA\\cars.csv"))
                using (var csv = new CsvReader(reader))
                {
                    var records = csv.GetRecords<Car>();

                    await _context.AddRangeAsync(records);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<bool> SeedEmissions()
        {
            try
            {
                using (var reader = new StreamReader("DATA\\emissions.csv"))
                using (var csv = new CsvReader(reader))
                {
                    var records = csv.GetRecords<CountryEmission>();

                    await _context.AddRangeAsync(records);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
