//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AmpRageRepo.Models
//{
//    public class Attributes
//    {
//        public int Id { get; set; }
//        public string regno { get; set; }
//        public string vin { get; set; }
//    }

//    public class Link
//    {
//        public int Id { get; set; }
//        public string rel { get; set; }
//        public string uri { get; set; }
//    }

//    public class Data2
//    {
//        public int Id { get; set; }
//        public string make { get; set; }
//        public string model { get; set; }
//        public bool? status { get; set; }
//        public string color { get; set; }
//        public string type { get; set; }
//        public string type_class { get; set; }
//        public int? vehicle_year { get; set; }
//        public int? model_year { get; set; }
//        public bool reused_regno { get; set; }
//    }

//    public class Basic
//    {
//        public int Id { get; set; }
//        public Data2 data { get; set; }
//    }

//    public class Data3
//    {
//        public int Id { get; set; }
//        public decimal? power_hp_1 { get; set; }
//        public decimal? power_hp_2 { get; set; }
//        public decimal? power_hp_3 { get; set; }
//        public decimal? power_kw_1 { get; set; }
//        public decimal? power_kw_2 { get; set; }
//        public decimal? power_kw_3 { get; set; }
//        public decimal? cylinder_volume { get; set; }
//        public decimal? top_speed { get; set; }
//        public decimal? fuel_1 { get; set; }
//        public decimal? fuel_2 { get; set; }
//        public decimal? fuel_3 { get; set; }
//        public decimal? fuel_combination { get; set; }
//        public decimal? consumption_1 { get; set; }
//        public decimal? consumption_2 { get; set; }
//        public decimal? consumption_3 { get; set; }
//        public decimal? co2_1 { get; set; }
//        public decimal? co2_2 { get; set; }
//        public decimal? co2_3 { get; set; }
//        public int? transmission { get; set; }
//        public bool four_wheel_drive { get; set; }
//        public decimal? sound_level_1 { get; set; }
//        public decimal? sound_level_2 { get; set; }
//        public decimal? sound_level_3 { get; set; }
//        public int number_of_passengers { get; set; }
//        public bool passenger_airbag { get; set; }
//        public decimal? hitch { get; set; }
//        public decimal? hitch_2 { get; set; }
//        public decimal? chassi_code_1 { get; set; }
//        public decimal? chassi_code_2 { get; set; }
//        public string chassi { get; set; }
//        public string color { get; set; }
//        public decimal? length { get; set; }
//        public decimal? width { get; set; }
//        public decimal? height { get; set; }
//        public decimal? kerb_weight { get; set; }
//        public decimal? total_weight { get; set; }
//        public decimal? load_weight { get; set; }
//        public decimal? trailer_weight { get; set; }
//        public decimal? unbraked_trailer_weight { get; set; }
//        public decimal? trailer_weight_b { get; set; }
//        public decimal? trailer_weight_be { get; set; }
//        public decimal? carriage_weight { get; set; }
//        public string tire_front { get; set; }
//        public string tire_back { get; set; }
//        public string rim_front { get; set; }
//        public string rim_back { get; set; }
//        public decimal? axel_width { get; set; }
//        public string category { get; set; }
//        public string eeg { get; set; }
//        public decimal? nox_1 { get; set; }
//        public decimal? nox_2 { get; set; }
//        public decimal? nox_3 { get; set; }
//        public decimal? thc_nox_1 { get; set; }
//        public decimal? thc_nox_2 { get; set; }
//        public decimal? thc_nox_3 { get; set; }
//        public int? eco_class { get; set; }
//        public int? emission_class { get; set; }
//        public int? euro_ncap { get; set; }
//    }

//    public class Technical
//    {
//        public int Id { get; set; }
//        public Data3 data { get; set; }
//    }

//    public class Data
//    {
//        public int Id { get; set; }
//        public string type { get; set; }
//        public Attributes attributes { get; set; }
//        public List<Link> links { get; set; }
//        public Basic basic { get; set; }
//        public Technical technical { get; set; }
//    }

//    public class RootObject
//    {
//        public int Id { get; set; }
//        public Data data { get; set; }
//    }
//}
