using CRS.ADMIN.BUSINESS.ClubManagement;
using CRS.ADMIN.REPOSITORY.ApiResponseMessage;
using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ApiResponseMessage;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.ApiResponseMessage
{
    public class ApiResponseMessageBusiness : IApiResponseMessageBusiness
    {
        IApiResponseMessageRepository _REPO;
        public ApiResponseMessageBusiness(ApiResponseMessageRepository REPO)
        {
            _REPO = REPO;
        }

        public ApiResponseMessageCommon ApiResponseMessageDetail(string MessageId = "")
        {
            return _REPO.ApiResponseMessageDetail(MessageId);
        }

        public List<ApiResponseMessageCommon> ApiResponseMessageList(PaginationFilterCommon Request)
        {
            return _REPO.ApiResponseMessageList(Request);
        }

        public CommonDbResponse StoreApiResponseMessage(ApiResponseMessageCommon Request)
        {
            return _REPO.StoreApiResponseMessage(Request); 
        }

        public CommonDbResponse UpdateApiResponseMessage(ApiResponseMessageCommon Request)
        {
            return _REPO.UpdateApiResponseMessage(Request);
        }
    }
}
