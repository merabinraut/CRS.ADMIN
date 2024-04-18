
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Models.PointSetup
{
    public class PointSetupModel
    {
        public string SearchFilter { get; set; }
        public List<CategoryModel> CategoryList = new List<CategoryModel>();
        public CategoryModel ManageCategory { get; set; }
        public List<UserTypeModel>  UserTypeList = new List<UserTypeModel>();
    }
    public class UserTypeModel
    {
        public string SNO { get; set; }
        public string RoleTypeId { get; set; }       
        public string RoleTypeName { get; set; }
    }
    public class CategoryModel
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