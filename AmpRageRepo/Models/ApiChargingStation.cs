using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmpRageRepo.Models
{
    public class DataProviderStatusType
    {
        public bool? IsProviderEnabled { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class DataProvider
    {
        public string WebsiteURL { get; set; }
        public object Comments { get; set; }
        public DataProviderStatusType DataProviderStatusType { get; set; }
        public bool? IsRestrictedEdit { get; set; }
        public bool? IsOpenDataLicensed { get; set; }
        public bool? IsApprovedImport { get; set; }
        public string License { get; set; }
        public object DateLastImported { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class OperatorInfo
    {
        public string WebsiteURL { get; set; }
        public object Comments { get; set; }
        public object PhonePrimaryContact { get; set; }
        public object PhoneSecondaryContact { get; set; }
        public object IsPrivateIndividual { get; set; }
        public object AddressInfo { get; set; }
        public object BookingURL { get; set; }
        public object ContactEmail { get; set; }
        public object FaultReportEmail { get; set; }
        public object IsRestrictedEdit { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class UsageType
    {
        public bool? IsPayAtLocation { get; set; }
        public bool? IsMembershipRequired { get; set; }
        public bool? IsAccessKeyRequired { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class Country
    {
        public string ISOCode { get; set; }
        public string ContinentCode { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class AddressInfo
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public string AddressLine1 { get; set; }
        public object AddressLine2 { get; set; }
        public string Town { get; set; }
        public object StateOrProvince { get; set; }
        public string Postcode { get; set; }
        public int? CountryID { get; set; }
        public Country Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ContactTelephone1 { get; set; }
        public object ContactTelephone2 { get; set; }
        public object ContactEmail { get; set; }
        public object AccessComments { get; set; }
        public object RelatedURL { get; set; }
        public double Distance { get; set; }
        public int DistanceUnit { get; set; }
    }

    public class StatusType
    {
        public bool? IsOperational { get; set; }
        public bool? IsUserSelectable { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class SubmissionStatus
    {
        public bool? IsLive { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class ConnectionType
    {
        public object FormalName { get; set; }
        public bool? IsDiscontinued { get; set; }
        public bool? IsObsolete { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class StatusType2
    {
        public bool? IsOperational { get; set; }
        public bool? IsUserSelectable { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
    }

    public class Level
    {
        public string Comments { get; set; }
        public bool? IsFastChargeCapable { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class CurrentType
    {
        public string Description { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
    }

    public class Connection
    {
        public int? ID { get; set; }
        public int? ConnectionTypeID { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public object Reference { get; set; }
        public int? StatusTypeID { get; set; }
        public StatusType2 StatusType { get; set; }
        public int LevelID { get; set; }
        public Level Level { get; set; }
        public int Amps { get; set; }
        public int Voltage { get; set; }
        public double PowerKW { get; set; }
        public int? CurrentTypeID { get; set; }
        public CurrentType CurrentType { get; set; }
        public int? Quantity { get; set; }
        public object Comments { get; set; }
    }

    public class ChargingStationRootObject
    {
        public int? ID { get; set; }
        public string UUID { get; set; }
        public object ParentChargePointID { get; set; }
        public int? DataProviderID { get; set; }
        public DataProvider DataProvider { get; set; }
        public object DataProvidersReference { get; set; }
        public int? OperatorID { get; set; }
        public OperatorInfo OperatorInfo { get; set; }
        public string OperatorsReference { get; set; }
        public int? UsageTypeID { get; set; }
        public UsageType UsageType { get; set; }
        public object UsageCost { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public int NumberOfPoints { get; set; }
        public string GeneralComments { get; set; }
        public object DatePlanned { get; set; }
        public object DateLastConfirmed { get; set; }
        public int? StatusTypeID { get; set; }
        public StatusType StatusType { get; set; }
        public DateTime DateLastStatusUpdate { get; set; }
        public int DataQualityLevel { get; set; }
        public DateTime DateCreated { get; set; }
        public int? SubmissionStatusTypeID { get; set; }
        public SubmissionStatus SubmissionStatus { get; set; }
        public object UserComments { get; set; }
        public object PercentageSimilarity { get; set; }
        public List<Connection> Connections { get; set; }
        public object MediaItems { get; set; }
        public object MetadataValues { get; set; }
        public bool? IsRecentlyVerified { get; set; }
        public object DateLastVerified { get; set; }
    }
}
