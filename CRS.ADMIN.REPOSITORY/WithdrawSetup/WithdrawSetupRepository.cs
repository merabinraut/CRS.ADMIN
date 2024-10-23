using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.WithdrawSetup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.WithdrawSetup
{
    public class WithdrawSetupRepository: IWithdrawSetupRepository
    {
        private readonly RepositoryDao _dao;
        public WithdrawSetupRepository(RepositoryDao dao)
        {
            _dao = dao;
        }
        public CommonDbResponse ManageWithdrawSetup(string Xml,string Type)
        {
            string SQL =string.IsNullOrEmpty(Type)? "EXEC sproc_withdraw_setup " : "EXEC sproc_withdraw_setup_update ";
            SQL += " @ConfigXml=" + _dao.FilterString(Xml);
            return _dao.ParseCommonDbResponse(SQL);
        }

        public List<WithdrawSetupListCommon> GetWithdrawSetupList()
        {
            List<WithdrawSetupListCommon> response = new List<WithdrawSetupListCommon>();
            string SQL = "sproc_withdraw_setup_select ";           
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new WithdrawSetupListCommon()
                    {
                        fromDate = _dao.ParseColumnValue(item, "fromDate").ToString(),
                        toDate = _dao.ParseColumnValue(item, "toDate").ToString(),
                        minAmount = _dao.ParseColumnValue(item, "minAmount").ToString(),
                        maxAmount = _dao.ParseColumnValue(item, "maxAmount").ToString(),
                        withdrawDate = _dao.ParseColumnValue(item, "withdrawDate").ToString(),                                      
                    });
                }
            }
            return response;
        }

        public WithdrawSetupListCommon GetWithdrawSetupDetail()
        {
            WithdrawSetupListCommon response = new WithdrawSetupListCommon();
            string SQL = "sproc_withdraw_setup_select ";
            var dbResponse = _dao.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new WithdrawSetupListCommon()
                {
                    fromDate = _dao.ParseColumnValue(dbResponse, "fromDate").ToString(),
                    toDate = _dao.ParseColumnValue(dbResponse, "toDate").ToString(),
                    minAmount = _dao.ParseColumnValue(dbResponse, "minAmount").ToString(),
                    maxAmount = _dao.ParseColumnValue(dbResponse, "maxAmount").ToString(),
                    withdrawDate = _dao.ParseColumnValue(dbResponse, "withdrawDate").ToString()

                };
            }
            return response;
        }
    }
}
