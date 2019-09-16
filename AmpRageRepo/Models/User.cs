using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<UserCar> UserCars { get; set; }
    }
}
