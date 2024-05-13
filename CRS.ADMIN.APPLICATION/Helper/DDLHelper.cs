
using CRS.ADMIN.BUSINESS.CommonManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CRS.ADMIN.APPLICATION.Library;

namespace CRS.ADMIN.APPLICATION.Helper
{
    public class DDLHelper
    {
        public static object LoadDropdownList(string forMethod, string search1 = "", string search2 = "")
        {
            var response = new Dictionary<string, string>();
            var context = HttpContext.Current;
            if (context == null)
                return response;//throw new InvalidOperationException("HttpContext not available.");

            var culture = context.Request.Cookies["culture"]?.Value ?? "ja";
            var _CommonBuss = new CommonManagementBusiness();
            var dbResponse = new List<StaticDataCommon>();
            switch (forMethod.ToUpper())
            {
                case "PAYMENTMETHODLIST":
                    dbResponse = _CommonBuss.GetDropDownLanguage("1", search1, search2, "");
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response;
                case "PREF":
                    dbResponse = _CommonBuss.GetDropDownLanguage("3", search1, search2, "");
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response;
                case "HOLIDAY":
                    dbResponse = _CommonBuss.GetDropDownLanguage("4", search1, search2, "");
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response;

                case "USERTYPELIST":
                    dbResponse = _CommonBuss.GetDropDownLanguage("14", search1, search2, "");
                    var filteredList = dbResponse
                    .Where(item => item.StaticValue == "3" || item.StaticValue == "4" || item.StaticValue == "6")
                    .ToList();
                    filteredList.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response; 
                case "TRANSACTIONTYPE":
                    dbResponse = _CommonBuss.GetDropDownLanguage("15", search1, search2, "");
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response;
                case "LOCATIONLIST":
                    dbResponse = _CommonBuss.GetDropDownLanguage("17", search1, search2, "");
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response;
                case "CLUBTOADMINPAYMENTMETHODLIST":
                    dbResponse = _CommonBuss.GetDropDownLanguage("18", search1, search2, "");
                    dbResponse.ForEach(item => { response.Add(item.StaticValue.EncryptParameter(), GetLocalizedLabel(item, culture)); });
                    return response;

                default:
                    return response;
            }
        }

        private static string GetLocalizedLabel(StaticDataCommon item, string culture)
        {
            switch (culture.ToLower())
            {
                case "ja":
                    return item.StaticLabelJapanese;
                case "en":
                    return item.StaticLabelEnglish;

                default:
                    return item.StaticLabelJapanese;
            }
        }
    }
}