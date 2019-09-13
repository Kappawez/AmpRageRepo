using AmpRageRepo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AmpRageRepo.Models
{
    public class Path
    {
        public Path(SecretController secret)
        {
            Secret = secret;
            WayPoints = new List<string>();
        }

        [Required(ErrorMessage = "Orgin is required.")]
        public string Origin { get; set; }
        [Required(ErrorMessage = "Destination is required.")]
        public string Destination { get; set; }
        [Required(ErrorMessage = "Range is required.")]
        public double Range { get; set; } //km
        public double EffectiveRange { get; set; } //km -> m -> x0.8

        public List<string> WayPoints { get; set; }
        public SecretController Secret { get; private set; }


    }
}
