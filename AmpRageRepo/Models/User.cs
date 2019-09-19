using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class User
    {
        public User()
        {
            UserCars = new List<UserCar>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public virtual List<UserCar> UserCars { get; set; }
    }

}
