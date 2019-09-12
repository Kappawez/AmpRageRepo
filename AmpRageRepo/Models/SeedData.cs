using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AmpRageRepo.Models
{
    public class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AmpContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<AmpContext>>()))
            {
                // Look for any movies.
                if (context.Cars.Any())
                {
                    return;   // DB has been seeded
                }

                var listOfCars = GetTheCars();

                context.Cars.AddRange(listOfCars);
                context.Database.CanConnect();
                context.Database.GetDbConnection();

                context.SaveChanges();
            }
        }

        private static List<Car> GetTheCars()
        {
            string filepath = @"C:\tmp\elcars3.txt";
            var listOfProps = new List<string>();
            var listOfCars = new List<Car>();
            var counter = 0;
            var car = new Car();

            foreach (var item in File.ReadAllLines(filepath))
            {
                if (counter != 8) //NUMBER OF LINES IT TAKES + 1)
                {
                    listOfProps.Add(item.ToString());
                    counter++;
                } else
                {
                    car = CreateCar(listOfProps);
                    listOfCars.Add(car);
                    counter = 1;
                    listOfProps.RemoveRange(0, listOfProps.Count);
                    listOfProps.Add(item.ToString());
                }
            };

            return listOfCars;
        }
        private static Car CreateCar(List<string> listOfProps)
        {
            var car = new Car()
            {
                Brand = listOfProps[0],
                Make = listOfProps[1],
                Capacity = decimal.Parse(listOfProps[2]),
                ZeroToHundred = decimal.Parse(listOfProps[3]),
                TopSpeed = int.Parse(listOfProps[4]),
                Range = int.Parse(listOfProps[5]),
                Efficiency = decimal.Parse(listOfProps[6]),
                Fastcharge = int.Parse(listOfProps[7])
            };
            return car;
        }
    }
}
