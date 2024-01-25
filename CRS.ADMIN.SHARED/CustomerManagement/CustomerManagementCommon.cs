using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.CustomerManagement
{
    public class CustomerSearchFilterCommon : PaginationFilterCommon
    {
        public string SearchFilter { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Status { get; set; }
        public string MobileNumber { get; set; }
    }
    public class ManageCustomerCommon : Common
    {
        public string AgentId { get; set; }
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string DOB { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public string PreferredLocation { get; set; }
        public string PostalCode { get; set; }
        public string Prefecture { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ResidenceNumber { get; set; }
    }
    public class CustomerListCommon
    {
        public string AgentId { get; set; }
        public string ProfileImage { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Age { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Referer { get; set; }
        public string Location { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

    }
}
