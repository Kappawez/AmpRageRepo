using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class Key
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime DateTime { get; set; }
        public string Value { get; set; }
    }
}
