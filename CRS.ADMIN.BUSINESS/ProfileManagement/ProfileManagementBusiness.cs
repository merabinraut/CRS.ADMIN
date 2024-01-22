using CRS.ADMIN.REPOSITORY.ProfileManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ProfileManagement;

namespace CRS.ADMIN.BUSINESS.ProfileManagement
{
    public class ProfileManagementBusiness : IProfileManagementBusiness
    {
        private readonly IProfileManagementRepository _repo;

        public ProfileManagementBusiness(ProfileManagementRepository profileManagementRepository) => this._repo = profileManagementRepository;

        public CommonDbResponse ChangePassword(PasswordCommon passwordCommon)
        {
            return _repo.ChangePassword(passwordCommon);
        }

        public CommonDbResponse ChangeProfileImage(UserProfileCommon common)
        {
            return _repo.ChangeProfileImage(common);
        }

        public UserProfileCommon ShowUserProfile(UserProfileCommon userProfileCommon)
        {
            return _repo.ShowUserProfile(userProfileCommon);
        }

        public CommonDbResponse UpdateAdminUserDetails(UserProfileCommon userProfileCommon)
        {
            return _repo.UpdateAdminUserDetails(userProfileCommon);
        }
    }
}