using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ProfileManagement;

namespace CRS.ADMIN.REPOSITORY.ProfileManagement
{
    public class ProfileManagementRepository : IProfileManagementRepository
    {
        private readonly RepositoryDao _dao;

        public ProfileManagementRepository(RepositoryDao dao) => this._dao = dao;
        public CommonDbResponse ChangePassword(PasswordCommon passwordCommon)
        {
            string sql = "exec sproc_profile_management";
            sql += " @Flag='cp'";
            sql += " ,@Session=" + _dao.FilterString(passwordCommon.Session);
            sql += " ,@CurrentPassword=" + _dao.FilterString(passwordCommon.OldPassword);
            sql += " ,@NewPassword=" + _dao.FilterString(passwordCommon.ConfirmPassword);
            sql += " ,@ActionPlatform=" + _dao.FilterString(passwordCommon.BrowserInfo);
            sql += " ,@ActionIP=" + _dao.FilterString(passwordCommon.IPAddress);
            sql += " ,@ActionUser=" + _dao.FilterString(passwordCommon.ActionUser);
            return _dao.ParseCommonDbResponse(sql);
        }

        public CommonDbResponse ChangeProfileImage(UserProfileCommon common)
        {
            var sql = "sproc_profile_management @flag='uimg'";
            sql += " ,@ProfileImage=" + _dao.FilterString(common.AdminLogoPath);
            sql += " ,@UserId=" + _dao.FilterString(common.UserId);
            return _dao.ParseCommonDbResponse(sql);
        }

        public UserProfileCommon ShowUserProfile(UserProfileCommon userProfileCommon)
        {
            var profile = new UserProfileCommon();
            string sql = "Exec sproc_profile_management @flag='sap', @userId= "
                + _dao.FilterString(userProfileCommon.UserId);
            var dr = _dao.ExecuteDataRow(sql);
            if (dr != null)
            {
                profile.UserId = userProfileCommon.UserId;
                profile.Email =  dr["EmailAddress"].ToString();
                profile.FullName = dr["fullName"].ToString();
                profile.MobileNumber = dr["MobileNumber"].ToString();
                profile.UserName = dr["userName"].ToString();
                profile.RoleName = dr["roleName"].ToString();
                profile.RoleType = dr["RoleType"].ToString();
                profile.ProfileImage = dr["ProfileImage"].ToString();
                profile.UpdatedBy = dr["UpdatedBy"].ToString();
                profile.UpdatedDate = dr["UpdatedLocalDate"].ToString();
            }
            return profile;
        }

        public CommonDbResponse UpdateAdminUserDetails(UserProfileCommon userProfileCommon)
        {
            string sql = "Exec sproc_profile_management";
            sql += " @flag= 'u'";
            sql += ", @FullName=" + _dao.FilterString(userProfileCommon.FullName);
            sql += ", @UserId=" + _dao.FilterString(userProfileCommon.UserId);
            sql += ", @ActionUser=" + _dao.FilterString(userProfileCommon.ActionUser);
            return _dao.ParseCommonDbResponse(sql);
        }
    }
}