using CRS.ADMIN.SHARED.Home;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.Home
{
    public interface IHomeRepository
    {
        List<ReceivedAmountModelCommon> GetReceivedAmount();
        List<ChartInfoCommon> GetChartInfoList();
        DashboardInfoModelCommon GetDashboardAnalytic();
        List<HostListModelCommon> GetHostList();
        List<TopBookedHostRankingModelCommon> GetTopBookedHostList();
        CommonDbResponse Login(LoginRequestCommon Request);
        string GetAdminBalance();
    }
}
