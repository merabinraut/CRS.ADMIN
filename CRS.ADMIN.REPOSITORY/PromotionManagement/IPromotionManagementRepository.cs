﻿using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PromotionManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.PromotionManagement
{
    public interface IPromotionManagementRepository
    {
        List<PromotionManagementCommon> GetPromotionalImageLists(PaginationFilterCommon Request);
        PromotionManagementCommon GetPromotionalImageById(string Id);
        CommonDbResponse AddPromotionalImage(PromotionManagementCommon promotion);
        CommonDbResponse EditPromotionalImage(PromotionManagementCommon promotion);
        CommonDbResponse DeletePromotionalImage(PromotionManagementCommon promotion);
        List<AdvertisementManagementCommon> GetAdvertisementImageLists(PaginationFilterCommon Request);
        AdvertisementDetailCommon GetAdvertisementImageById(string Id);
        CommonDbResponse UpdateAdvertisementImage(AdvertisementDetailCommon promotion);
        CommonDbResponse BlockUnblockAdvertisementImage(AdvertisementDetailCommon promotion);

    }
}