using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PromotionManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PromotionManagement
{
    public interface IPromotionManagementBusiness
    {
        List<PromotionManagementCommon> GetPromotionalImageLists(PaginationFilterCommon Request);
        PromotionManagementCommon GetPromotionalImageById(string Id);
        CommonDbResponse AddPromotionalImage(PromotionManagementCommon promotion);
        CommonDbResponse EditPromotionalImage(PromotionManagementCommon promotion);
        CommonDbResponse DeletePromotionalImage(PromotionManagementCommon promotion);
    }
}