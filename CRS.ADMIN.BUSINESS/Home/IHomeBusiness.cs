using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.Home;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.Home
{
    public interface IHomeBusiness
    {
        List<ReceivedAmountModelCommon> GetReceivedAmount();
        List<ChartInfoCommon> GetChartInfoList();
        DashboardInfoModelCommon GetDashboardAnalytic();
        List<HostListModelCommon> GetHostList();
        List<TopBookedHostRankingModelCommon> GetTopBookedHostList();
        CommonDbResponse Login(LoginRequestCommon Request);
    }
}
