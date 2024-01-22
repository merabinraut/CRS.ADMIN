using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.CommonManagement
{
    public class CommonManagementRepository : ICommonManagementRepository
    {
        private RepositoryDao _DAO;
        public CommonManagementRepository()
        {
            _DAO = new RepositoryDao();
        }

        public Dictionary<string, string> GetDropDown(string Flag, string Extra1 = "", string Extra2 = "")
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            string SQL = "EXEC sproc_dropdown_management ";
            SQL += "@Flag=" + _DAO.FilterString(Flag);
            SQL += ",@SearchField1=" + _DAO.FilterString(Extra1);
            SQL += ",@SearchField2=" + _DAO.FilterString(Extra2);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                    response.Add(item["Value"].ToString(), item["Text"].ToString());
            }
            return response;
        }
    }
}
