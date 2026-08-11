using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;

namespace CRS.ADMIN.REPOSITORY.HostManagement
{
    public class HostManagementRepository : IHostManagementRepository
    {
        RepositoryDao _DAO;
        public HostManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        //public List<HostListCommon> GetHostList(string AgentId, PaginationFilterCommon Request)
        //{
        //    var response = new List<HostListCommon>();
        //    string SQL = "EXEC sproc_host_management @Flag='ghl'";
        //    SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
        //    SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
        //    SQL += ",@Skip=" + Request.Skip;
        //    SQL += ",@Take=" + Request.Take;
        //    var dbResponse = _DAO.ExecuteDataTable(SQL);
        //    if (dbResponse != null)
        //    {
        //        foreach (DataRow item in dbResponse.Rows)
        //        {
        //            response.Add(new HostListCommon()
        //            {
        //                AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
        //                HostId = _DAO.ParseColumnValue(item, "HostId").ToString(),
        //                HostName = _DAO.ParseColumnValue(item, "HostName").ToString(),
        //                Position = _DAO.ParseColumnValue(item, "Position").ToString(),
        //                Rank = _DAO.ParseColumnValue(item, "Rank").ToString(),
        //                Age = _DAO.ParseColumnValue(item, "Age").ToString(),
        //                Status = _DAO.ParseColumnValue(item, "Status").ToString(),
        //                CreatedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "CreatedDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "CreatedDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
        //                UpdatedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "UpdatedDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "UpdatedDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
        //                ClubName = _DAO.ParseColumnValue(item, "ClubName").ToString(),
        //                Ratings = _DAO.ParseColumnValue(item, "Ratings").ToString(),
        //                TotalVisitors = _DAO.ParseColumnValue(item, "TotalVisitors").ToString(),
        //                HostImage = _DAO.ParseColumnValue(item, "HostImage").ToString(),
        //                TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
        //                SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString()),
        //                Height = _DAO.ParseColumnValue(item, "Height").ToString(),
        //                Address = _DAO.ParseColumnValue(item, "Address").ToString()
        //            });
        //        }
        //    }
        //    return response;
        //}
        public List<HostListCommon> GetHostList(string AgentId, PaginationFilterCommon Request)
        {
            var response = new List<HostListCommon>();
            string SQL = "EXEC sproc_host_management @Flag='ghl_rk'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var i = Request.Skip + 1;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new HostListCommon()
                    {
                        AgentId = _DAO.ParseColumnValue(item, "AgentId").ToString(),
                        HostId = _DAO.ParseColumnValue(item, "HostId").ToString(),
                        HostName = _DAO.ParseColumnValue(item, "HostName").ToString(),
                        Position = _DAO.ParseColumnValue(item, "Position").ToString(),
                        Rank = _DAO.ParseColumnValue(item, "Rank").ToString(),
                        Age = _DAO.ParseColumnValue(item, "Age").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        CreatedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "CreatedDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "CreatedDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "UpdatedDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "UpdatedDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        //ClubName = _DAO.ParseColumnValue(item, "ClubName").ToString(),
                        //Ratings = _DAO.ParseColumnValue(item, "Ratings").ToString(),
                        TotalVisitors = _DAO.ParseColumnValue(item, "TotalVisitors").ToString(),
                        HostImage = _DAO.ParseColumnValue(item, "Thumbnail").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "RowsTotal").ToString()),
                        SNO = i,
                        Height = _DAO.ParseColumnValue(item, "Height").ToString(),
                        Address = _DAO.ParseColumnValue(item, "Address").ToString()

                    });
                    i++;
                }
            }
            return response;
        }
        public ManageHostCommon GetHostDetail(string AgentId, string HostId)
        {
            var Response = new ManageHostCommon();
            string SQL = "EXEC sproc_host_management @Flag='ghd'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);

            string SQL1 = "EXEC sproc_host_management @Flag='gsl_ases'";
            SQL1 += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL1 += ",@HostId=" + _DAO.FilterString(HostId);
            var dbResponse1 = _DAO.ExecuteDataRow(SQL1);

            if (dbResponse != null)
            {
                Response = new ManageHostCommon()
                {
                    AgentId = _DAO.ParseColumnValue(dbResponse, "AgentId")?.ToString() ?? string.Empty,
                    HostId = _DAO.ParseColumnValue(dbResponse, "HostId")?.ToString() ?? string.Empty,
                    HostName = _DAO.ParseColumnValue(dbResponse, "HostName")?.ToString() ?? string.Empty,
                    Position = _DAO.ParseColumnValue(dbResponse, "Position")?.ToString() ?? string.Empty,
                    DOB = _DAO.ParseColumnValue(dbResponse, "DOB")?.ToString() ?? string.Empty,
                    ConstellationGroup = _DAO.ParseColumnValue(dbResponse, "ConstellationGroup")?.ToString() ?? string.Empty,
                    Height = _DAO.ParseColumnValue(dbResponse, "Height")?.ToString() ?? string.Empty,
                    BloodType = _DAO.ParseColumnValue(dbResponse, "BloodType")?.ToString() ?? string.Empty,
                    PreviousOccupation = _DAO.ParseColumnValue(dbResponse, "PreviousOccupation")?.ToString() ?? string.Empty,
                    LiquorStrength = _DAO.ParseColumnValue(dbResponse, "LiquorStrength")?.ToString() ?? string.Empty,
                    InstagramLink = _DAO.ParseColumnValue(dbResponse, "InstagramLink")?.ToString() ?? string.Empty,
                    TiktokLink = _DAO.ParseColumnValue(dbResponse, "TiktokLink")?.ToString() ?? string.Empty,
                    TwitterLink = _DAO.ParseColumnValue(dbResponse, "TwitterLink")?.ToString() ?? string.Empty,
                    Rank = _DAO.ParseColumnValue(dbResponse, "Rank")?.ToString() ?? string.Empty,
                    Line = _DAO.ParseColumnValue(dbResponse, "Line")?.ToString() ?? string.Empty,
                    ImagePath = _DAO.ParseColumnValue(dbResponse, "ImagePath")?.ToString() ?? string.Empty,
                    IconImagePath = _DAO.ParseColumnValue(dbResponse, "IconImagePath")?.ToString() ?? string.Empty,
                    Address = _DAO.ParseColumnValue(dbResponse, "Address")?.ToString() ?? string.Empty,
                    HostNameJapanese = _DAO.ParseColumnValue(dbResponse, "HostNameJapanese")?.ToString() ?? string.Empty,
                    HostIntroduction = _DAO.ParseColumnValue(dbResponse, "HostIntroduction")?.ToString() ?? string.Empty,
                    MBTI = _DAO.ParseColumnValue(dbResponse, "MBTI")?.ToString() ?? string.Empty,
                    Title = _DAO.ParseColumnValue(dbResponse, "Title")?.ToString() ?? string.Empty,

                    appreanceScoreValue = _DAO.ParseColumnValue(dbResponse1, "AppearanceAndOverallImpression")?.ToString() ?? string.Empty,
                    conversationScoreValue = _DAO.ParseColumnValue(dbResponse1, "ConversationStyle")?.ToString() ?? string.Empty,
                    beverageToleranceScoreValue = _DAO.ParseColumnValue(dbResponse1, "AlcoholTolerance")?.ToString() ?? string.Empty,
                    atmosphereScoreValue = _DAO.ParseColumnValue(dbResponse1, "Atmosphere")?.ToString() ?? string.Empty,
                };

                string SQL2 = "EXEC sproc_host_identity_detail_management @Flag = 'ghid'";
                SQL2 += ",@ClubId=" + _DAO.FilterString(AgentId);
                SQL2 += ",@HostId=" + _DAO.FilterString(HostId);
                var dbResponse2 = _DAO.ExecuteDataTable(SQL2);

                if (dbResponse2 != null && dbResponse2.Rows.Count > 0) Response.HostIdentityDataModel = _DAO.DataTableToListObject<HostIdentityDataCommon>(dbResponse2).ToList();
            }


            return Response;
        }

        public CommonDbResponse ManageHost(ManageHostCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_host_management ";
            SQL += !string.IsNullOrEmpty(Request.HostId) ? "@Flag='uh'" : "@Flag='rh'";
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += !string.IsNullOrEmpty(Request.HostId) ? ",@HostId=" + _DAO.FilterString(Request.HostId) : "";
            //SQL += ",@HostName=" + _DAO.FilterString(Request.HostName);
            SQL += ",@HostName=N" + _DAO.FilterString(Request.HostName);
            SQL += ",@HostNameJapanese=N" + _DAO.FilterString(Request.HostNameJapanese);
            SQL += string.IsNullOrEmpty(Request.Position) ? ",@Position=" + _DAO.FilterString(Request.Position) : ",@Position=N" + _DAO.FilterString(Request.Position);
            //SQL += string.IsNullOrEmpty(Request.OtherPositionRemark) ? ",@OtherPositionRemark=" + _DAO.FilterString(Request.OtherPositionRemark) : ",@OtherPositionRemark=N" + _DAO.FilterString(Request.OtherPositionRemark);
            SQL += !string.IsNullOrEmpty(Request.Rank?.ToString()) ? ",@Rank=" + Request.Rank : "";
            SQL += ",@DOB=" + "'" + Request.DOB + "'";
            SQL += ",@ConstellationGroup=" + _DAO.FilterString(Request.ConstellationGroup);
            SQL += ",@Height=" + _DAO.FilterString(Request.Height);
            SQL += ",@BloodType=" + _DAO.FilterString(Request.BloodType);
            SQL += string.IsNullOrEmpty(Request.PreviousOccupation) ? ",@PreviousOccupation=" + _DAO.FilterString(Request.PreviousOccupation) : ",@PreviousOccupation=N" + _DAO.FilterString(Request.PreviousOccupation);
            //SQL += ",@PreviousOccupation=N" + _DAO.FilterString(Request.PreviousOccupation);
            SQL += ",@LiquorStrength=" + _DAO.FilterString(Request.LiquorStrength);
            //SQL += ",@WebsiteLink=" + _DAO.FilterString(Request.WebsiteLink);


            SQL += ",@TiktokLink=" + _DAO.FilterString(Request.TiktokLink);
            SQL += ",@TwitterLink=" + _DAO.FilterString(Request.TwitterLink);
            SQL += ",@InstagramLink=" + _DAO.FilterString(Request.InstagramLink);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ImagePath=" + _DAO.FilterString(Request.ImagePath);
            SQL += ",@Line=" + _DAO.FilterString(Request.Line);
            SQL += ",@IconImagePath=" + _DAO.FilterString(Request.IconImagePath);
            SQL += string.IsNullOrEmpty(Request.Address) ? ",@Address=" + _DAO.FilterString(Request.Address) : ",@Address=N" + _DAO.FilterString(Request.Address);
            SQL += string.IsNullOrEmpty(Request.HostIntroduction) ? ",@HostIntroduction=" + _DAO.FilterString(Request.HostIntroduction) : ",@HostIntroduction=N" + _DAO.FilterString(Request.HostIntroduction);
            Response = _DAO.ParseCommonDbResponse(SQL);
            foreach (var item in Request.HostIdentityDataModel)
            {
                var SQL2 = "EXEC sproc_host_identity_detail_management @Flag = 'mhid'";
                SQL2 += ",@ClubId=" + _DAO.FilterString(Request.AgentId);
                SQL2 += !string.IsNullOrEmpty(Request.HostId) ? ",@HostId=" + _DAO.FilterString(Request.HostId) : ",@HostId=" + _DAO.FilterString(Response.Extra1);
                SQL2 += ",@IdentityType=" + _DAO.FilterString(item.IdentityType);
                SQL2 += ",@IdentityValue=" + _DAO.FilterString(item.IdentityValue);
                SQL2 += !string.IsNullOrEmpty(item.IdentityDDLType) ? ",@IdentityDDLType=" + _DAO.FilterString(item.IdentityDDLType) : null;
                SQL2 += string.IsNullOrEmpty(item.IdentityDescription) ? ",@IdentityDescription=" + _DAO.FilterString(item.IdentityDescription) : ",@IdentityDescription=N" + _DAO.FilterString(item.IdentityDescription);
                SQL2 += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
                SQL2 += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
                _DAO.ParseCommonDbResponse(SQL2);
            }
            return Response;
        }

        public CommonDbResponse ManageHostStatus(string AgentId, string HostId, string Status, Common Request)
        {
            string SQL = "EXEC sproc_host_management @Flag='uhs'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            SQL += ",@Status=" + _DAO.FilterString(Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #region Manage gallery
        public List<HostGalleryManagementCommon> GetGalleryImage(string AgentId, string HostId, string GalleryId = "", string SearchFilter = "")
        {
            string SQL = "EXEC dbo.sproc_host_gallery_management @Flag='ghgl'";
            SQL += ", @AgentId =" + _DAO.FilterString(AgentId);
            SQL += ", @HostId =" + _DAO.FilterString(HostId);
            SQL += !string.IsNullOrEmpty(GalleryId) ? ", @GalleryId =" + _DAO.FilterString(GalleryId) : "";
            SQL += !string.IsNullOrEmpty(SearchFilter) ? ", @SearchFilter =N" + _DAO.FilterString(SearchFilter) : "";
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _DAO.DataTableToListObject<HostGalleryManagementCommon>(dbResponse).ToList();
            return new List<HostGalleryManagementCommon>();
        }

        public CommonDbResponse ManageGalleryImage(HostManageGalleryImageCommon Request)
        {
            string SQL = "EXEC dbo.sproc_host_gallery_management ";
            SQL += !string.IsNullOrEmpty(Request.GalleryId) ? "@Flag='mhgi'" : "@Flag='ihgi'";
            SQL += !string.IsNullOrEmpty(Request.GalleryId) ? ", @GalleryId =" + _DAO.FilterString(Request.GalleryId) : "";
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ", @HostId =" + _DAO.FilterString(Request.HostId);
            SQL += ",@ImageTitle=N" + _DAO.FilterString(Request.ImageTitle);
            SQL += ",@ImagePath=" + _DAO.FilterString(Request.ImagePath);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageGalleryImageStatus(string AgentId, string HostId, string GalleryId, Common Request)
        {
            string SQL = "EXEC dbo.sproc_host_gallery_management @Flag='mhgis'";
            SQL += ",@AgentId=" + _DAO.FilterString(AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(HostId);
            SQL += ",@GalleryId=" + _DAO.FilterString(GalleryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Host Identity Detail Management 
        public List<HostIdentityDataCommon> GetHostIdentityDetail(string AgentId = "", string HostId = "")
        {
            var Response = new List<HostIdentityDataCommon>();
            string SQL = "EXEC sproc_host_identity_detail_management @Flag = 'ghid'";
            SQL += !string.IsNullOrEmpty(AgentId) ? ",@ClubId=" + _DAO.FilterString(AgentId) : null;
            SQL += !string.IsNullOrEmpty(HostId) ? ",@HostId=" + _DAO.FilterString(HostId) : null;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) Response = _DAO.DataTableToListObject<HostIdentityDataCommon>(dbResponse).ToList();
            return Response;
        }
        public List<StaticDataCommon> GetSkillsDLL()
        {
            string SQL = "EXEC sproc_host_identity_detail_management @Flag = 'gsddl'";
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _DAO.DataTableToListObject<StaticDataCommon>(dbResponse).ToList();
            return new List<StaticDataCommon>();
        }

        public List<InquiryListCommon> GetInquiryListAsync(string SearchFilter, int StartIndex, int PageSize)
        {
            string SQL = "EXEC sproc_get_customer_enquiry ";
            SQL += !string.IsNullOrEmpty(SearchFilter) ? "@searchFilter=" + _DAO.FilterString(SearchFilter) + "," : "";
            SQL += "@Skip=" + StartIndex;
            SQL += ",@Take=" + PageSize;

            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _DAO.DataTableToListObject<InquiryListCommon>(dbResponse).ToList();
            return new List<InquiryListCommon>();
        }
        #endregion

        public CommonDbResponse UploadHostImage(string ClubName, string LocationId, string HostName, string ImagePath)
        {
            string SQL = "EXEC sproc_bulk_image_upload ";
            SQL += "@club_name=" + _DAO.FilterString(ClubName);
            SQL += ",@locationId=" + _DAO.FilterString(LocationId);
            SQL += ",@host_name=" + _DAO.FilterString(HostName);
            SQL += ",@imagePath=" + _DAO.FilterString(ImagePath);

            return _DAO.ParseCommonDbResponse(SQL);
        }

        public InquiryListCommon GetInquiryDetailsAsync(string inquiryId)
        {
            string SQL = "EXEC sproc_admin_get_customer_enquiry_details";
            SQL += " @inqueryId=" + _DAO.FilterString(inquiryId);
            var dataTable = _DAO.ExecuteDataTable(SQL);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return new InquiryListCommon()
                {
                    id = dataTable.Rows[0]["id"].ToString(),
                    InquiryId = dataTable.Rows[0]["InquiryId"].ToString(),
                    FullName = dataTable.Rows[0]["FullName"].ToString(),
                    PhoneNumber = dataTable.Rows[0]["PhoneNumber"].ToString(),
                    InquiryType = dataTable.Rows[0]["InquiryType"].ToString(),
                    CompanyName = dataTable.Rows[0]["CompanyName"].ToString(),
                    EmailAddress = dataTable.Rows[0]["EmailAddress"].ToString(),
                    Subject = dataTable.Rows[0]["Subject"].ToString(),
                    Message = dataTable.Rows[0]["Message"].ToString(),
                    PostedDate = dataTable.Rows[0]["PostedDate"].ToString(),
                    Attachments = dataTable.Rows[0]["Attachments"].ToString(),
                    AttachmentName = dataTable.Rows[0]["AttachmentName"].ToString()
                };
            }
            return new InquiryListCommon();
        }

        public CommonDbResponse ManageNewHostDetails(ManageHostCommon request)
        {
            string SQL = "EXEC sproc_host_new_management ";
            SQL += !string.IsNullOrEmpty(request.HostId) ? "@Flag='U'" : "@Flag='I'";
            SQL += ",@AgentId=" + _DAO.FilterString(request.AgentId);
            SQL += ",@HostId=" + _DAO.FilterString(request.HostId);
            SQL += ",@HostName=N" + _DAO.FilterString(request.HostName);

            SQL += ",@Position=" + (string.IsNullOrEmpty(request.Position) ? "null" : "N" + _DAO.FilterString(request.Position) + "");
            SQL += ",@Rank=" + (string.IsNullOrEmpty(request.Rank) ? "0" : "N" + _DAO.FilterString(request.Rank) + "");



            //SQL += ",@DOB=" + _DAO.FilterString(request.DOB);
            SQL += ",@DOB=" + _DAO.FilterString(request.DOB);
            SQL += ",@year=" + _DAO.FilterString(request.year);
            SQL += ",@month=" + _DAO.FilterString(request.month);
            SQL += ",@date=" + _DAO.FilterString(request.date);

            SQL += ",@ConstellationGroup=N" + _DAO.FilterString(request.ConstellationGroup);
            SQL += ",@Height=" + (string.IsNullOrEmpty(request.Height) ? "null" : "N" + _DAO.FilterString(request.Height) + "");

            SQL += ",@BloodType=" + _DAO.FilterString(request.BloodType);

            SQL += ",@TiktokLink=" + _DAO.FilterString(request.TiktokLink);
            SQL += ",@TwitterLink=" + _DAO.FilterString(request.TwitterLink);
            SQL += ",@InstagramLink=" + _DAO.FilterString(request.InstagramLink);
            SQL += ",@Line=" + _DAO.FilterString(request.Line);
            SQL += ",@ImagePath=" + _DAO.FilterString(request.ImagePath);
            SQL += ",@Address=" + (string.IsNullOrEmpty(request.Address) ? "null" : "N" + _DAO.FilterString(request.Address) + "");

            SQL += ",@HostNameJapanese=" + (string.IsNullOrEmpty(request.HostNameJapanese) ? "null" : "N" + _DAO.FilterString(request.HostNameJapanese) + "");


            SQL += ",@appreanceScoreValue=" + _DAO.FilterString(request.appreanceScoreValue);
            SQL += ",@conversationScoreValue=" + _DAO.FilterString(request.conversationScoreValue);
            SQL += ",@beverageToleranceScoreValue=" + _DAO.FilterString(request.beverageToleranceScoreValue);
            SQL += ",@atmosphereScoreValue=" + _DAO.FilterString(request.atmosphereScoreValue);

            SQL += ",@MBTI=" + (!string.IsNullOrEmpty(request.MBTI)? "N" + _DAO.FilterString(request.MBTI) + "": "NULL");
            SQL += ",@Title=" + (!string.IsNullOrEmpty(request.Title) ? "N" + _DAO.FilterString(request.Title) + "" : "NULL");

            SQL += ",@ActionUser=N" + _DAO.FilterString(request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(request.ActionIP);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(request.ActionPlatform);

            return _DAO.ParseCommonDbResponse(SQL);
        }
    }
}
