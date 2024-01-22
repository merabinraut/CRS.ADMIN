using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ProfileManagement;

namespace CRS.ADMIN.REPOSITORY.ProfileManagement
{
    public interface IProfileManagementRepository
    {
        UserProfileCommon ShowUserProfile(UserProfileCommon userProfileCommon);
        CommonDbResponse ChangePassword(PasswordCommon passwordCommon);
        CommonDbResponse UpdateAdminUserDetails(UserProfileCommon userProfileCommon);
        CommonDbResponse ChangeProfileImage(UserProfileCommon common);

    }
}