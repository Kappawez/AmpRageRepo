using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class Attributes
    {
        public string regno { get; set; }
        public string vin { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string uri { get; set; }
    }

    public class Data2
    {
        public string make { get; set; }
        public string model { get; set; }
        public object status { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string type_class { get; set; }
        public int vehicle_year { get; set; }
        public int model_year { get; set; }
        public bool reused_regno { get; set; }
    }

    public class Basic
    {
        public Data2 data { get; set; }
    }

    public class Data3
    {
        public int power_hp_1 { get; set; }
        public object power_hp_2 { get; set; }
        public object power_hp_3 { get; set; }
        public int power_kw_1 { get; set; }
        public object power_kw_2 { get; set; }
        public object power_kw_3 { get; set; }
        public int cylinder_volume { get; set; }
        public int top_speed { get; set; }
        public int fuel_1 { get; set; }
        public object fuel_2 { get; set; }
        public object fuel_3 { get; set; }
        public object fuel_combination { get; set; }
        public double consumption_1 { get; set; }
        public object consumption_2 { get; set; }
        public object consumption_3 { get; set; }
        public int co2_1 { get; set; }
        public object co2_2 { get; set; }
        public object co2_3 { get; set; }
        public int transmission { get; set; }
        public bool four_wheel_drive { get; set; }
        public int sound_level_1 { get; set; }
        public object sound_level_2 { get; set; }
        public object sound_level_3 { get; set; }
        public int number_of_passengers { get; set; }
        public bool passenger_airbag { get; set; }
        public object hitch { get; set; }
        public object hitch_2 { get; set; }
        public int chassi_code_1 { get; set; }
        public object chassi_code_2 { get; set; }
        public string chassi { get; set; }
        public string color { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int kerb_weight { get; set; }
        public int total_weight { get; set; }
        public int load_weight { get; set; }
        public int trailer_weight { get; set; }
        public int unbraked_trailer_weight { get; set; }
        public int trailer_weight_b { get; set; }
        public int trailer_weight_be { get; set; }
        public object carriage_weight { get; set; }
        public string tire_front { get; set; }
        public string tire_back { get; set; }
        public string rim_front { get; set; }
        public string rim_back { get; set; }
        public int axel_width { get; set; }
        public string category { get; set; }
        public string eeg { get; set; }
        public double nox_1 { get; set; }
        public object nox_2 { get; set; }
        public object nox_3 { get; set; }
        public double thc_nox_1 { get; set; }
        public object thc_nox_2 { get; set; }
        public object thc_nox_3 { get; set; }
        public object eco_class { get; set; }
        public object emission_class { get; set; }
        public int euro_ncap { get; set; }
    }

    public class Technical
    {
        public Data3 data { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public Attributes attributes { get; set; }
        public List<Link> links { get; set; }
        public Basic basic { get; set; }
        public Technical technical { get; set; }
    }

    public class RootObject
    {
        public Data data { get; set; }
    }
}
