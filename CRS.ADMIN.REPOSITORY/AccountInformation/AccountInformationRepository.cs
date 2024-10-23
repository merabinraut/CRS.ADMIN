using CRS.ADMIN.SHARED.AccountInformation;
using CRS.ADMIN.SHARED.AdminPointManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.AccountInformation
{
    public class AccountInformationRepository : IAccountInformationRepository
    {
        private readonly RepositoryDao _dao;
        public AccountInformationRepository(RepositoryDao dao)
        {
            _dao = dao;
        }

        public List<AccountInformationCommonResponse> GetAccountInformationAsync(AccountInformationFilterCommon request, PaginationFilterCommon paginationFilterCommon)
        {
            var response = new List<AccountInformationCommonResponse>();
            string SQL = "EXEC sproc_get_affiliate_account_information ";
            SQL += !string.IsNullOrEmpty(paginationFilterCommon.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(paginationFilterCommon.SearchFilter) : string.Empty;
            SQL += !string.IsNullOrEmpty(request.fromDate) ? "@fromDate=" + _dao.FilterString(request.fromDate) : "@fromDate=NULL";
            SQL += !string.IsNullOrEmpty(request.toDate) ? " ,@toDate=" + _dao.FilterString(request.toDate) : " ,@toDate =NULL";
            SQL += " ,@Skip=" + paginationFilterCommon.Skip;
            SQL += ",@Take=" + paginationFilterCommon.Take;
       
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new AccountInformationCommonResponse()
                    {
                        bankType = Convert.ToString(_dao.ParseColumnValue(item, "bankType")),
                        bankName = Convert.ToString(_dao.ParseColumnValue(item, "bankName")),
                        branchName = Convert.ToString(_dao.ParseColumnValue(item, "branchName")),
                        accountType = Convert.ToString(_dao.ParseColumnValue(item, "accountType")),
                        accountNumber = Convert.ToString(_dao.ParseColumnValue(item, "accountNumber")),
                        accountHolderName = Convert.ToString(_dao.ParseColumnValue(item, "accountHolderName")),
                        affiliateId = Convert.ToString(_dao.ParseColumnValue(item, "affiliateId")),
                        accountSymbol = Convert.ToString(_dao.ParseColumnValue(item, "accountSymbol")),
                        roleType = Convert.ToString(_dao.ParseColumnValue(item, "roleType")),
                        status = Convert.ToString(_dao.ParseColumnValue(item, "status")),
                        createdDate = !string.IsNullOrEmpty(_dao.ParseColumnValue(item, "createdDate").ToString()) ? DateTime.Parse(_dao.ParseColumnValue(item, "createdDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _dao.ParseColumnValue(item, "createdDate").ToString(),
                        createdBy = Convert.ToString(_dao.ParseColumnValue(item, "createdBy")),
                        AffiliateName = Convert.ToString(_dao.ParseColumnValue(item, "AffiliateName")),
                        AffiliateNameEng = Convert.ToString(_dao.ParseColumnValue(item, "AffiliateNameEng")),
                        MobileNumber = Convert.ToString(_dao.ParseColumnValue(item, "MobileNumber"))

                    });
                }
            }
            return response;
        }
    }
}
