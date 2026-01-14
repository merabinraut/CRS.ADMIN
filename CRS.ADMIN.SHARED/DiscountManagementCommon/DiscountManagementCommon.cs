using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.DiscountManagementCommon
{

    public class DiscountManagementRequestCommon:Common
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string discountName { get; set; }
        public string description { get; set; }
    }

    public class DiscountManagementCommon
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; } 
        public string Status {  get; set; }
        public string CreatedDate {  get; set; }
        public string UpdatedDate {  get; set; }
        public int TotalRecords {  get; set; }
        public int SNO {  get; set; }
    }


    public class DiscountCategoryDetailsResponseCommon
    {
        public string SubCategoryId { get; set; }
        public string CategoryId { get; set; }
        public string categoryName { get; set; }
        public long FromAmount { get; set; }
        public long ToAmount { get; set; }
        public string DiscountType { get; set; }
        public long Value { get; set; }
        public long MinValue { get; set; }
        public long MaxValue { get; set; }
        public string Status { get; set; }
        public int TotalRecords { get; set; }
        public int SNO { get; set; }
        public string createdDate { get; set; }
        public string updatedDate { get; set; }
    }


    public class DiscountSubCategoryManagementRequestCommon:Common
    {
        public string categoryId { get; set; }
        public string subCategoryId { get; set; }
        public long? FromAmount { get; set; }
        public long? ToAmount { get; set; }
        public string DiscountType { get; set; }
        public long? Value { get; set; }
        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }
    }


    public class DiscountSubCategoryRequestCommon
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string subCategoryId { get; set; }
    }


    public class AssignDiscountCommon:Common
    {
        public string AgentId { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CurrentCategory { get; set; }
    }
}
