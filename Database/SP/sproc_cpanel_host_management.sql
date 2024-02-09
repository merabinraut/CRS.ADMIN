USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_cpanel_host_management]    Script Date: 2/7/2024 5:42:56 PM ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO





ALTER PROC [dbo].[sproc_cpanel_host_management]
    @Flag VARCHAR(10),
    @CustomerAgentId BIGINT = NULL,
    @HostId BIGINT = NULL
AS
BEGIN
    IF @Flag = 'ghd' --get host details
    BEGIN
        SELECT a.HostId,
               b.AgentId AS ClubId,
               b.ClubName1 AS ClubNameEnglish,
               b.ClubName2 AS ClubNameJapanese,
               ISNULL(
               (
                   SELECT TOP 1
                          a2.LocationName
                   FROM dbo.tbl_location a2 WITH (NOLOCK)
                   WHERE a2.LocationId = b.LocationId
                         AND ISNULL(b.Status, '') = 'A'
               ),
               ''
                     ) AS ClubLocationName,
               b.Logo AS ClubLogo,
               a.HostNameJapanese,
               a.HostName AS HostNameEnglish,
               CONCAT(
                         YEAR(CONVERT(DATE, a.DOB)),
                         N'年',
                         MONTH(CONVERT(DATE, a.DOB)),
                         N'月',
                         DAY(CONVERT(DATE, a.DOB)),
                         N'日 (',
                         DATEDIFF(YEAR, CONVERT(DATE, a.DOB), GETDATE()),
                         N'才)'
                     ) AS HostDOB,
               DATEDIFF(YEAR, CONVERT(DATE, a.DOB), GETDATE()) AS HostAge,
               a.Address AS HostBirthPlace,
               a.Rank AS HostRank,
               a.HostIntroduction,
               CONCAT(a.Height, 'cm') AS HostHeight,
               ISNULL(c.WebsiteLink, '#') AS HostWebsiteLink,
               ISNULL(c.TiktokLink, '#') AS HostTiktokLink,
               ISNULL(c.TwitterLink, '#') AS HostTwitterLink,
               ISNULL(c.InstagramLink, '#') AS HostInstagramLink,
               ISNULL(c.FacebookLink, '#') AS HostFacebookLink,
               ISNULL(c.Line, '#') AS HostLine,
               ISNULL(d.StaticDataLabel, '') AS HostConstellationGroup,
               ISNULL(d.StaticDataLabel, '') AS HostPreviousOccupation,
               ISNULL(f.StaticDataLabel, '') AS HostBloodType,
               ISNULL(g.StaticDataLabel, '') AS HostLiquorStrength,
               CASE
                   WHEN ISNULL(@CustomerAgentId, '') <> ''
                        AND EXISTS
    (
        SELECT 'X'
        FROM dbo.tbl_bookmark b2 WITH (NOLOCK)
        WHERE b2.CustomerId = @CustomerAgentId
              AND b2.AgentType = 'host'
              AND b2.HostId = a.HostId
              AND b2.ClubId = b.AgentId
              AND ISNULL(b2.Status, '') = 'A'
    )          THEN
                       'Y'
                   ELSE
                       'N'
               END AS IsBookmarked,
               ISNULL(
               (
                   SELECT COUNT(c2.BookmarkId)
                   FROM dbo.tbl_bookmark c2
                   WHERE c2.AgentType = 'Host'
                         AND c2.ClubId = b.AgentId
                         AND c2.HostId = a.HostId
                         AND ISNULL(c2.Status, '') = 'A'
               ),
               0
                     ) AS HostLoveCount,
               '100%' CustomerHostCompatibility
        FROM dbo.tbl_host_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') = 'A'
            LEFT JOIN dbo.tbl_website_details c WITH (NOLOCK)
                ON c.AgentId = b.AgentId
                   AND c.HostId = a.HostId
                   AND c.RoleId = 5
            LEFT JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 13
                   AND d.StaticDataValue = a.ConstellationGroup
                   AND ISNULL(d.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 12
                   AND e.StaticDataValue = a.PreviousOccupation
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data f WITH (NOLOCK)
                ON f.StaticDataType = 18
                   AND f.StaticDataValue = a.BloodType
                   AND ISNULL(f.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data g WITH (NOLOCK)
                ON g.StaticDataType = 19
                   AND g.StaticDataValue = a.LiquorStrength
                   AND ISNULL(g.Status, '') = 'A'
        WHERE a.HostId = @HostId;
    END;

    ELSE IF @Flag = 'ghs' --get host skills
    BEGIN
        CREATE TABLE #temp_ghs
        (
            LabelType VARCHAR(MAX),
            LabelEnglish VARCHAR(MAX),
            LabelJapanese NVARCHAR(MAX),
            LabelValue NVARCHAR(MAX)
        );

        INSERT INTO #temp_ghs
        SELECT 'Skills',
               c.StaticDataLabel AS LabelEnglish,
               c.AdditionalValue1 AS LabelJapanese,
               ISNULL(a.IdentityDescription, 0) AS LabelValue
        FROM dbo.tbl_host_identity_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                ON b.HostId = a.HostId
            LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = a.IdentityType
                   AND c.StaticDataValue = a.IdentityDDLType
                   AND ISNULL(c.Status, '') = 'A'
        WHERE b.HostId = @HostId
              AND a.IdentityType = '31'
        ORDER BY CAST(c.StaticDataValue AS INT) ASC;

        IF NOT EXISTS (SELECT 'X' FROM #temp_ghs a WITH (NOLOCK))
        BEGIN
            INSERT INTO #temp_ghs
            SELECT 'Skills',
                   a.StaticDataLabel AS LabelEnglish,
                   a.AdditionalValue1 AS LabelJapanese,
                   '0'
            FROM dbo.tbl_static_data a WITH (NOLOCK)
            WHERE a.StaticDataType = 31
                  AND ISNULL(a.Status, '') = 'A'
            ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        END;
        SELECT *
        FROM #temp_ghs;
        DROP TABLE #temp_ghs;

        RETURN;
    END;

    ELSE IF @Flag = 'ghpnl' --get host personality and Lifestyle
    BEGIN
        CREATE TABLE #temp_ghp
        (
            LabelId VARCHAR(10),
            LabelType VARCHAR(10),
            LabelEnglish VARCHAR(MAX),
            LabelJapanese NVARCHAR(MAX),
            LabelValue NVARCHAR(MAX)
        );

        INSERT INTO #temp_ghp
        (
            LabelId,
            LabelType,
            LabelEnglish,
            LabelJapanese
        )
        SELECT a.StaticDataValue,
               a.StaticDataType,
               a.StaticDataLabel AS LabelEnglish,
               a.AdditionalValue1 AS LabelJapanese
        FROM dbo.tbl_static_data a WITH (NOLOCK)
        WHERE a.StaticDataType IN ( 32, 33 )
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY CAST(a.StaticDataType AS INT),
                 CAST(a.StaticDataValue AS INT) ASC;


        UPDATE a
        SET a.LabelValue = b.IdentityDescription
        FROM #temp_ghp a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_identity_details b WITH (NOLOCK)
                ON b.IdentityValue = a.LabelId
                   AND b.IdentityType = a.LabelType
            INNER JOIN dbo.tbl_host_details c WITH (NOLOCK)
                ON c.HostId = b.HostId
                   AND ISNULL(c.Status, '') = 'A'
        WHERE c.HostId = @HostId;

        SELECT CASE
                   WHEN LabelType = 32 THEN
                       'Personality'
                   WHEN LabelType = 33 THEN
                       'Lifestyle'
                   ELSE
                       LabelType
               END AS LabelType,
               LabelEnglish,
               LabelJapanese,
               LabelValue
        FROM #temp_ghp WITH (NOLOCK);

        DROP TABLE #temp_ghp;
        RETURN;
    END;

    ELSE IF ISNULL(@Flag, '') = 'ghgil' --get host gallery image list
    BEGIN
        WITH CTE
        AS (SELECT TOP 3
                   c.ImagePath
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                INNER JOIN dbo.tbl_gallery c WITH (NOLOCK)
                    ON c.AgentId = b.HostId
                       AND c.RoleId = 5
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(c.Status, '') = 'A'
            WHERE b.HostId = @HostId
            ORDER BY NEWID())
        SELECT *
        FROM CTE a WITH (NOLOCK)
        ORDER BY NEWID();

        RETURN;
    END;
END;
GO


