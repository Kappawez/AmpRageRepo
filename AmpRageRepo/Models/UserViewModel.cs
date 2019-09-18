using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefonnummer")]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail adress")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        public int MyProperty { get; set; }

        public string CarBrand { get; set; }
        public string CarMake { get; set; }

        public IEnumerable<SelectListItem> AllCarBrands { get; set; }
        public IEnumerable<SelectListItem> AllCarModels { get; set; }
    }
}
