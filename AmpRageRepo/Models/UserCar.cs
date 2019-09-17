using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class UserCar
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}
