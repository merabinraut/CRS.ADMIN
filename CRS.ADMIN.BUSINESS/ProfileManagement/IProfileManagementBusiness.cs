using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ProfileManagement;

namespace CRS.ADMIN.BUSINESS.ProfileManagement
{
    public interface IProfileManagementBusiness
    {
        UserProfileCommon ShowUserProfile(UserProfileCommon userProfileCommon);
        CommonDbResponse ChangePassword(PasswordCommon passwordCommon);
        CommonDbResponse UpdateAdminUserDetails(UserProfileCommon userProfileCommon);
        CommonDbResponse ChangeProfileImage(UserProfileCommon common);
    }
}