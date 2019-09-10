using System;
using System.Collections.Generic;
using System.Text;

namespace AmpRageRepo
{
    class Car
    {
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Make { get; set; }
        public decimal Capacity { get; set; }
        public decimal ZeroToHundred { get; set; }
        public int TopSpeed { get; set; }
        public int Range { get; set; }
        public decimal Efficiency { get; set; }
        public int Fastcharge { get; set; }

        public List<string> GetAllProps()
        {
            var listOfProps = new List<string>();

            if (this.LicensePlate.Length > 0)
            {
                listOfProps.Add(this.LicensePlate);
            }
            listOfProps.Add(this.Brand);
            listOfProps.Add(this.Make);
            listOfProps.Add(this.Capacity.ToString());
            listOfProps.Add(this.ZeroToHundred.ToString());
            listOfProps.Add(this.TopSpeed.ToString());
            listOfProps.Add(this.Range.ToString());
            listOfProps.Add(this.Efficiency.ToString());
            listOfProps.Add(this.Fastcharge.ToString());

            return listOfProps;
        }
    }
}
