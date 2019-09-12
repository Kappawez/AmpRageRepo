using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class Path
    {
        public Path()
        {
            WayPoints = new List<string>();
        }

        public string Origin { get; set; }
        public string Destination { get; set; }
        public int Range { get; set; } //km
        public double EffectiveRange { get; set; } //km -> m -> x0.8

        public List<string> WayPoints { get; set; }
    }
}
