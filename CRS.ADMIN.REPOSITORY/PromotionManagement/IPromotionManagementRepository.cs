using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PromotionManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.PromotionManagement
{
    public interface IPromotionManagementRepository
    {
        List<PromotionManagementCommon> GetPromotionalImageLists();
        PromotionManagementCommon GetPromotionalImageById(string Id);
        CommonDbResponse AddPromotionalImage(PromotionManagementCommon promotion);
        CommonDbResponse EditPromotionalImage(PromotionManagementCommon promotion);
        CommonDbResponse DeletePromotionalImage(PromotionManagementCommon promotion);
    }
}