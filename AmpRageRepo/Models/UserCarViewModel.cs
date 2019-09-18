using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class UserCarViewModel
    {
        public int UserId { get; set; }
        public string CarBrand { get; set; }
        public string CarMake { get; set; }

        public IEnumerable<SelectListItem> AllCarBrands { get; set; }
        public IEnumerable<SelectListItem> AllCarModels { get; set; }
    }
}
