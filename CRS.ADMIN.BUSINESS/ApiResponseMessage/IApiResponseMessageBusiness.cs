using CRS.ADMIN.SHARED.ApiResponseMessage;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.ApiResponseMessage
{
    public interface IApiResponseMessageBusiness
    {
        CommonDbResponse StoreApiResponseMessage(ApiResponseMessageCommon Request);
        CommonDbResponse UpdateApiResponseMessage(ApiResponseMessageCommon Request);

        List<ApiResponseMessageCommon> ApiResponseMessageList(PaginationFilterCommon Request);

        ApiResponseMessageCommon ApiResponseMessageDetail(string MessageId = "");
    }
}
