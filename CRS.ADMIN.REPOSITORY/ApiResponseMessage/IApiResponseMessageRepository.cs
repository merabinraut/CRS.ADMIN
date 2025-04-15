
﻿using CRS.ADMIN.SHARED.PaginationManagement;

using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.ApiResponseMessage;

namespace CRS.ADMIN.REPOSITORY.ApiResponseMessage
{
    public interface IApiResponseMessageRepository
    {
        CommonDbResponse StoreApiResponseMessage(ApiResponseMessageCommon Request);
        CommonDbResponse UpdateApiResponseMessage(ApiResponseMessageCommon Request);

        List<ApiResponseMessageCommon> ApiResponseMessageList(ApiResponseMessageFilterCommon Request);

        ApiResponseMessageCommon ApiResponseMessageDetail(string MessageId="");

    }
}
