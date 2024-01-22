using CRS.ADMIN.BUSINESS.AffiliateManagement;
using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.BUSINESS.CommissionManagement;
using CRS.ADMIN.BUSINESS.CommonManagement;
using CRS.ADMIN.BUSINESS.CustomerManagement;
using CRS.ADMIN.BUSINESS.EmailLog;
using CRS.ADMIN.BUSINESS.ErrorLog;
using CRS.ADMIN.BUSINESS.Home;
using CRS.ADMIN.BUSINESS.HostManagement;
using CRS.ADMIN.BUSINESS.LocationManagement;
using CRS.ADMIN.BUSINESS.LogManagement.APILogManagement;
using CRS.ADMIN.BUSINESS.LogManagement.EmailLogManagement;
using CRS.ADMIN.BUSINESS.LogManagement.ErrorLogManagement;
using CRS.ADMIN.BUSINESS.NotificationManagement;
using CRS.ADMIN.BUSINESS.PaymentManagement;
using CRS.ADMIN.BUSINESS.PlanManagement;
using CRS.ADMIN.BUSINESS.ProfileManagement;
using CRS.ADMIN.BUSINESS.PromotionManagement;
using CRS.ADMIN.BUSINESS.RecommendationManagement;
using CRS.ADMIN.BUSINESS.RecommendationManagement_V2;
using CRS.ADMIN.BUSINESS.ReservationLedger;
using CRS.ADMIN.BUSINESS.ReviewAndRatingsManagement;
using CRS.ADMIN.BUSINESS.RoleManagement;
using CRS.ADMIN.BUSINESS.ScheduleManagement;
using CRS.ADMIN.BUSINESS.SMSLog;
using CRS.ADMIN.BUSINESS.StaffManagement;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;

namespace CRS.ADMIN.APPLICATION
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IClubManagementBusiness, ClubManagementBusiness>();
            container.RegisterType<ICommissionManagementBusiness, CommissionManagementBusiness>();
            container.RegisterType<ICustomerManagementBusiness, CustomerManagementBusiness>();
            container.RegisterType<IHomeBusiness, HomeBusiness>();
            container.RegisterType<IPromotionManagementBusiness, PromotionManagementBusiness>();
            container.RegisterType<IRoleManagementBusiness, RoleManagementBusiness>();
            container.RegisterType<IAPILogManagementBusiness, APILogManagementBusiness>();
            container.RegisterType<IEmailLogManagementBusiness, EmailLogManagementBusiness>();
            container.RegisterType<IErrorLogManagementBusiness, ErrorLogManagementBusiness>();
            container.RegisterType<IProfileManagementBusiness, ProfileManagementBusiness>();
            container.RegisterType<IPlanManagementBusiness, PlanManagementBusiness>();
            container.RegisterType<IHostManagementBusiness, HostManagementBusiness>();
            container.RegisterType<ICommonManagementBusiness, CommonManagementBusiness>();
            container.RegisterType<INotificationManagementBusiness, NotificationManagementBusiness>();
            container.RegisterType<ILocationManagementBusiness, LocationManagementBusiness>();
            container.RegisterType<IStaffManagementBusiness, StaffManagementBusiness>();
            container.RegisterType<IReservationLedgerBusiness, ReservationLedgerBusiness>();
            container.RegisterType<IPaymentManagementBusiness, PaymentManagementBusiness>();
            container.RegisterType<IRecommendationManagementBusiness, RecommendationManagementBusiness>();
            container.RegisterType<IRecommendationManagementV2Business, RecommendationManagementV2Business>();
            container.RegisterType<IScheduleManagementBusiness, ScheduleManagementBusiness>();
            container.RegisterType<IAffiliateManagementBusiness, AffiliateManagementBusiness>();
            container.RegisterType<IReviewAndRatingsBusiness, ReviewAndRatingsBusiness>();
            container.RegisterType<ISMSLogBusiness, SMSLogBusiness>();
            container.RegisterType<IErrorLogBusiness, ErrorLogBusiness>();
            container.RegisterType<IEmailLogBusiness, EmailLogBusiness>();
            return container;
        }
    }
}