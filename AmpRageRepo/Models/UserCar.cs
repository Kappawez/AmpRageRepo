using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class UserCar
    {
        public int Id { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }

        public virtual Car Car { get; set; }
        public int CarId { get; set; }
    }
}
