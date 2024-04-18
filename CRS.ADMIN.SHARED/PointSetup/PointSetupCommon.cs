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
}
