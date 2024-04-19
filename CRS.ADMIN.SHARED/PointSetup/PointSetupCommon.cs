using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.PointSetup
{
    public class PointSetupCommon
    {
        public string SearchFilter { get; set; }
        public List<CategoryCommon> CategoryListl = new List<CategoryCommon>();
        public List<UserTypeCommon> UserTypeList = new List<UserTypeCommon>();
        public List<CategorySlabCommon> CategorySlabList = new List<CategorySlabCommon>();
    }
    public class UserTypeCommon : PaginationResponseCommon
    {
        public string RoleTypeId { get; set; }
        public string RoleTypeName { get; set; }
    }

    public class CategoryCommon : PaginationResponseCommon
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryDescription { get; set; }
        public string RoleTypeId { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }
    public class CategorySlabCommon : PaginationResponseCommon
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string FromAmount { get; set; }
        public string ToAmount { get; set; }
        public string PointType { get; set; }
        public string PointValue { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public string RoleTypeId { get; set; }
        public string CategorySlabId { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string PointType2 { get; set; }
        public string PointValue2 { get; set; }
        public string MinValue2 { get; set; }
        public string MaxValue2 { get; set; }
        public string PointType3 { get; set; }
        public string PointValue3 { get; set; }
        public string MinValue3 { get; set; }
        public string MaxValue3 { get; set; }
    }
}
