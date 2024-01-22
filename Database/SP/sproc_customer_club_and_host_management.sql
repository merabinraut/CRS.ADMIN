USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_customer_club_and_host_management]    Script Date: 10/12/2023 10:06:17 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


-- =============================================
-- Author:		<Paras Maharjan>
-- Create date: <2023-10-21>
-- Description:	<customer club and host management>
-- =============================================
ALTER PROCEDURE [dbo].[sproc_customer_club_and_host_management]
(
    @Flag CHAR(10),
    @LocationId VARCHAR(10) = NULL,
    @ClubId VARCHAR(10) = NULL,
    @PlanId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIp VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @SelectType CHAR(1) = NULL,
    @HostId VARCHAR(10) = NULL,
    @customerAgentId VARCHAR(10) = NULL
)
AS
DECLARE @CurrentDate VARCHAR(50) = CONVERT(VARCHAR, GETDATE(), 23);
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    --Get club list via location id
    IF @Flag = 'cl'
    BEGIN
        SELECT DISTINCT
               a.AgentId AS ClubId,
               a.LocationId AS LocationId,
               a.ClubName1 AS ClubName,
               a.ClubName2 AS ClubNameJapanese,
               a.GroupName,
               (ISNULL(a.FirstName, '') + ISNULL(a.MiddleName, '') + ' ' + ISNULL(a.LastName, '')) AS FullName,
               a.Logo AS ClubLogo,
               a.CoverPhoto AS ClubCoverPhoto,
               a.Description AS ClubDescription,
               c.LocationName AS Tag1,
               CASE
                   WHEN ISNULL(b.Tag2Status, '') = 'A' THEN
                       t2.StaticDataLabel
                   ELSE
                       ''
               END AS Tag2,
               CASE
                   WHEN ISNULL(b.Tag3Status, '') = 'A' THEN
                       t3.StaticDataLabel
                   ELSE
                       ''
               END AS Tag3,
               CASE
                   WHEN ISNULL(b.Tag4Status, '') = 'A' THEN
                       'Excellent'
                   ELSE
                       '' --'Not Excellent'
               END AS Tag4,
               CASE
                   WHEN ISNULL(b.Tag5Status, '') = 'A' THEN
                       t5.StaticDataLabel
                   ELSE
                       ''
               END AS Tag5,
               a.ClubOpeningTime,
               a.ClubClosingTime,
               CASE
                   WHEN d.ClubSchedule IS NOT NULL
                        AND d.ClubSchedule = 1 THEN
                       'Reservable'
                   ELSE
                       'Unreservable'
               END AS ClubTodaySchedule,
               CASE
                   WHEN EXISTS
    (
        SELECT 1
        FROM dbo.tbl_bookmark tb WITH (NOLOCK)
        WHERE tb.CustomerId = @customerAgentId
              AND tb.AgentType = 'club'
              AND tb.ClubId = a.AgentId
              AND tb.Status = 'A'
    )          THEN
                       'Y'
                   ELSE
                       'N'
               END AS IsBookmarked
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            LEFT JOIN dbo.tbl_tag_detail b WITH (NOLOCK)
                ON b.ClubId = a.AgentId
            LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = b.Tag1Location
                   AND b.Tag1Status = 'A'
                   AND ISNULL(c.[Status], '') = 'A'
            LEFT JOIN dbo.tbl_static_data t5 WITH (NOLOCK)
                ON t5.StaticDataType = 21
                   AND t5.StaticDataValue = b.Tag5StoreName
                   AND t5.Status = 'A'
            LEFT JOIN dbo.tbl_static_data t3 WITH (NOLOCK)
                ON t3.StaticDataType = 17
                   AND t3.StaticDataValue = b.Tag3CategoryName
                   AND t3.Status = 'A'
            LEFT JOIN dbo.tbl_static_data t2 WITH (NOLOCK)
                ON t2.StaticDataType = 14
                   AND t2.StaticDataValue = b.Tag2RankName
                   AND t2.Status = 'A'
            LEFT JOIN dbo.tbl_club_schedule d WITH (NOLOCK)
                ON d.ClubId = a.AgentId
                   AND ISNULL(d.Status, '') = 'A'
                   AND d.DateValue = @CurrentDate
        WHERE a.LocationId = @LocationId
              AND ISNULL(a.[Status], '') = 'A';

        RETURN;
    END;
    --Get host list via location id
    ELSE IF @Flag = 'hl'
    BEGIN
        SELECT a.LocationId,
               b.AgentId ClubId,
               b.HostId,
               b.HostName,
               d.StaticDataLabel AS Occupation,
               b.Rank,
               a.ClubName1 AS ClubName,
               a.Logo AS ClubLogo,
               (
                   SELECT TOP 1
                          ImagePath
                   FROM tbl_gallery c WITH (NOLOCK)
                   WHERE c.AgentId = b.HostId
                         AND c.RoleId = 7
                         AND ISNULL(c.Status, '') = 'A'
                   ORDER BY c.Sno DESC
               ) AS HostImage,
               CASE
                   WHEN EXISTS
         (
             SELECT 1
             FROM dbo.tbl_bookmark tb WITH (NOLOCK)
             WHERE tb.CustomerId = @customerAgentId
                   AND tb.AgentType = 'host'
                   AND tb.Status = 'A'
         )     THEN
                       'Y'
                   ELSE
                       'N'
               END AS IsBookmarked
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND ISNULL(a.[Status], '') = 'A'
            LEFT JOIN tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 12
                   AND d.StaticDataValue = b.PreviousOccupation
                   AND ISNULL(d.Status, '') = 'A'
        WHERE a.LocationId = ISNULL(@LocationId, a.LocationId)
              AND a.AgentId = ISNULL(@ClubId, a.AgentId)
              AND ISNULL(b.[Status], '') = 'A';

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'cd'
    BEGIN
        SELECT a.AgentId AS ClubId,
               a.LocationId AS LocationId,
               a.ClubName1 AS ClubNameEng,
               a.ClubName2 AS ClubNameJp,
               a.GroupName,
               (ISNULL(a.FirstName, '') + ISNULL(a.MiddleName, '') + ' ' + ISNULL(a.LastName, '')) AS FullName,
               a.Logo AS ClubLogo,
               a.CoverPhoto AS ClubCoverPhoto,
               a.Description AS ClubDescription,
               c.LocationName AS Tag1,
               CASE
                   WHEN ISNULL(b.Tag2Status, '') = 'A' THEN
                       t2.StaticDataLabel
                   ELSE
                       ''
               END AS Tag2,
               CASE
                   WHEN ISNULL(b.Tag3Status, '') = 'A' THEN
                       t3.StaticDataLabel
                   ELSE
                       ''
               END AS Tag3,
               CASE
                   WHEN ISNULL(b.Tag4Status, '') = 'A' THEN
                       'Excellent'
                   ELSE
                       ''
               END AS Tag4,
               CASE
                   WHEN ISNULL(b.Tag5Status, '') = 'A' THEN
                       t5.StaticDataLabel
                   ELSE
                       ''
               END AS Tag5,
               a.ClubOpeningTime,
               a.ClubClosingTime,
               a.MobileNumber AS ClubMobileNumber,
               CASE
                   WHEN COALESCE(a.InputStreet, '') = ''
                        AND COALESCE(a.InputCity, '') = '' THEN
                       ''
                   ELSE
                       CONCAT(   ISNULL(a.InputStreet, ''),
                                 CASE
                                     WHEN COALESCE(a.InputStreet, '') <> ''
                                          AND COALESCE(a.InputCity, '') <> '' THEN
                                         ', '
                                     ELSE
                                         ''
                                 END,
                                 ISNULL(a.InputCity, '')
                             )
               END AS ClubLocation,
               ISNULL(d.WebsiteLink, '') AS WebsiteLink,
               ISNULL(d.TiktokLink, '') AS TiktokLink,
               ISNULL(d.TwitterLink, '') AS TwitterLink,
               ISNULL(d.InstagramLink, '') AS InstagramLink,
               ISNULL(em.EventTitle, '') AS EventTitle,
               ISNULL(em.Description, '') AS EventDescription,
               ISNULL(a.LocationURL, '#') AS LocationURL,
               CASE
                   WHEN EXISTS
         (
             SELECT 1
             FROM dbo.tbl_bookmark tb WITH (NOLOCK)
             WHERE tb.CustomerId = @customerAgentId
                   AND tb.AgentType = 'club'
                   AND tb.ClubId = a.AgentId
                   AND tb.Status = 'A'
         )     THEN
                       'Y'
                   ELSE
                       'N'
               END AS IsBookmarked
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            LEFT JOIN dbo.tbl_tag_detail b WITH (NOLOCK)
                ON b.ClubId = a.AgentId
                   AND ISNULL(a.[Status], '') = 'A'
            LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = b.Tag1Location
                   AND b.Tag1Status = 'A'
                   AND ISNULL(c.[Status], '') = 'A'
            LEFT JOIN dbo.tbl_static_data t5 WITH (NOLOCK)
                ON t5.StaticDataType = 21
                   AND t5.StaticDataValue = b.Tag5StoreName
                   AND t5.Status = 'A'
            LEFT JOIN dbo.tbl_static_data t3 WITH (NOLOCK)
                ON t3.StaticDataType = 17
                   AND t3.StaticDataValue = b.Tag3CategoryName
                   AND t3.Status = 'A'
            LEFT JOIN dbo.tbl_static_data t2 WITH (NOLOCK)
                ON t2.StaticDataType = 14
                   AND t2.StaticDataValue = b.Tag2RankName
                   AND t2.Status = 'A'
            LEFT JOIN tbl_website_details d WITH (NOLOCK)
                ON d.AgentId = a.AgentId
                   AND d.RoleId = 5
            LEFT JOIN dbo.tbl_event_management em
                ON em.AgentId = a.AgentId
                   AND FORMAT(em.ActionDate, 'yyyy-MM-dd') = FORMAT(GETDATE(), 'yyyy-MM-dd')
        WHERE a.Sno = @ClubId;

        RETURN;
    END;
    ELSE IF @Flag = 'gpl' --get plan list
    BEGIN
        SELECT a.PlanId,
               a.PlanName,
               d.StaticDataLabel AS PlanType,
               b.StaticDataLabel AS PlanTime,
               a.Price,
               c.StaticDataLabel AS Liquor,
               a.Nomination,
               ISNULL(a.Remarks, '') AS Remarks
        FROM dbo.tbl_plans a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data b WITH (NOLOCK)
                ON b.StaticDataValue = a.PlanTime
                   AND b.StaticDataType = 8
                   AND ISNULL(b.[Status], '') = 'A'
            INNER JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataValue = a.Liquor
                   AND c.StaticDataType = 9
                   AND ISNULL(c.[Status], '') = 'A'
            INNER JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataValue = a.PlanType
                   AND d.StaticDataType = 7
                   AND ISNULL(d.[Status], '') = 'A'
        WHERE a.PlanId = ISNULL(@PlanId, a.PlanId)
              AND ISNULL(a.PlanStatus, '') = 'A';

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gcgil' --get club gallery image list
    BEGIN
        IF ISNULL(@SelectType, '') = 'A'
        BEGIN
            SELECT b.ImagePath
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_gallery b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 5
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') = 'A'
                       AND a.AgentId = @ClubId
            ORDER BY NEWID();

            RETURN;
        END;
        ELSE
        BEGIN
            SELECT TOP 3
                   b.ImagePath
            FROM tbl_club_details a WITH (NOLOCK)
                INNER JOIN tbl_gallery b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 5
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') = 'A'
                       AND a.AgentId = @ClubId
            ORDER BY NEWID();

            RETURN;
        END;
    END;
    ELSE IF ISNULL(@Flag, '') = 'vhd' --view host details
    BEGIN
        SELECT a.HostId,
               a.HostName,
               b.AgentId ClubId,
               b.ClubName1 AS ClubName,
               b.Logo AS ClubLogo,
               ISNULL(c.StaticDataLabel, '') AS ConstellationGroup,
               Height,
               ISNULL(e.StaticDataLabel, '') AS BloodType,
               ISNULL(d.StaticDataLabel, '') AS PreviousOccupation,
               ISNULL(f.StaticDataLabel, '') AS LiquorStrength,
               DOB,
               CASE
                   WHEN EXISTS
         (
             SELECT 1
             FROM dbo.tbl_bookmark tb WITH (NOLOCK)
             WHERE tb.CustomerId = @customerAgentId
                   AND tb.AgentType = 'host'
                   AND tb.HostId = a.HostId
                   AND tb.ClubId = b.AgentId
                   AND tb.Status = 'A'
         )     THEN
                       'Y'
                   ELSE
                       'N'
               END AS IsBookmarked
        FROM tbl_host_details a WITH (NOLOCK)
            INNER JOIN tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND ISNULL(b.Status, '') = 'A'
            LEFT JOIN tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = 13
                   AND c.StaticDataValue = a.ConstellationGroup
                   AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 12
                   AND d.StaticDataValue = a.PreviousOccupation
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 18
                   AND e.StaticDataValue = a.BloodType
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN tbl_static_data f WITH (NOLOCK)
                ON f.StaticDataType = 19
                   AND f.StaticDataValue = a.LiquorStrength
                   AND ISNULL(f.Status, '') = 'A'
        WHERE a.HostId = @HostId
              AND ISNULL(a.Status, '') = 'A';

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ghgil' --get host gallery image list
    BEGIN
        SELECT b.ImagePath
        FROM tbl_host_details a WITH (NOLOCK)
            INNER JOIN tbl_gallery b WITH (NOLOCK)
                ON b.AgentId = a.HostId
                   AND b.RoleId = 7
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') = 'A'
                   AND a.HostId = @HostId
        ORDER BY NEWID();

        RETURN;
    END;
END;
GO


