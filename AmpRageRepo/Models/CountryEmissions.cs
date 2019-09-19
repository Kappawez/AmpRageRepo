using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class CountryEmission
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public double KgCo2Kwh { get; set; }
    }
}
