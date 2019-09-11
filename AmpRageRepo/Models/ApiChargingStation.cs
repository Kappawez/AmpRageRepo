using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class AddressInfo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string AddressLine1 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public int CountryID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public int DistanceUnit { get; set; }
        public string StateOrProvince { get; set; }
        public string AccessComments { get; set; }
    }

    public class Connection
    {
        public int ID { get; set; }
        public int ConnectionTypeID { get; set; }
        public int StatusTypeID { get; set; }
        public int LevelID { get; set; }
        public double PowerKW { get; set; }
        public int CurrentTypeID { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; }
        public int? Amps { get; set; }
        public int? Voltage { get; set; }
        public string Comments { get; set; }
    }

    public class ChargingStationRootObject
    {
        public int ID { get; set; }
        public string UUID { get; set; }
        public int DataProviderID { get; set; }
        public int OperatorID { get; set; }
        public string OperatorsReference { get; set; }
        public int UsageTypeID { get; set; }
        public string UsageCost { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public int NumberOfPoints { get; set; }
        public string GeneralComments { get; set; }
        public int StatusTypeID { get; set; }
        public DateTime DateLastStatusUpdate { get; set; }
        public int DataQualityLevel { get; set; }
        public DateTime DateCreated { get; set; }
        public int SubmissionStatusTypeID { get; set; }
        public List<Connection> Connections { get; set; }
        public bool IsRecentlyVerified { get; set; }
        public string DataProvidersReference { get; set; }
    }
}
