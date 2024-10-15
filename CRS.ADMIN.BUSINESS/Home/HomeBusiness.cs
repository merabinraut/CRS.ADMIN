using CRS.ADMIN.REPOSITORY.Home;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.Home;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.Home
{
    public class HomeBusiness : IHomeBusiness
    {
        IHomeRepository _REPO;
        public HomeBusiness(HomeRepository REPO)
        {
            _REPO = REPO;
        }

        #region "Dashboard Information"
        public List<ReceivedAmountModelCommon> GetReceivedAmount()
        {
            return _REPO.GetReceivedAmount();
        }

        public List<ChartInfoCommon> GetChartInfoList()
        {
            return _REPO.GetChartInfoList();
        }
        public DashboardInfoModelCommon GetDashboardAnalytic()
        {
            return _REPO.GetDashboardAnalytic();
        }

        public List<HostListModelCommon> GetHostList()
        {
            return _REPO.GetHostList();
        }

        public List<TopBookedHostRankingModelCommon> GetTopBookedHostList()
        {
            return _REPO.GetTopBookedHostList();
        }
        #endregion
        public CommonDbResponse Login(LoginRequestCommon Request)
        {
            return _REPO.Login(Request);
        }
        public string GetAdminBalance()
        {
            return _REPO.GetAdminBalance();
        }
    }
}
