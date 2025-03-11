﻿using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CRS.ADMIN.REPOSITORY.AffiliateManagement
{
    public interface IAffiliateManagementRepository
    {
        List<AffiliateManagementCommon> GetAffiliateList(PaginationFilterCommon Request);
        CommonDbResponse ApproveRejectAffiliateRequest(ApproveRejectAffiliateCommon Request);
        CommonDbResponse ManageAffiliateStatus(ManageAffiliateStatusCommon Request);
        List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string affiliateId, string filterDate, PaginationFilterCommon Request);
        AffiliatePageAnalyticCommon GetAffiliateAnalytic();
        CommonDbResponse ResetPassword(ManageAffiliateStatusCommon Request, SqlConnection connection = null, SqlTransaction transaction = null);
        ManageAffiliateCommon GetAffiliateDetails(string AffiliateId, String culture = "");
        CommonDbResponse ManageAffiliate(ManageAffiliateCommon Request);
    }
}
