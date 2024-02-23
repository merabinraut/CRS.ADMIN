USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_cp_dashboard_v2]    Script Date: 2/23/2024 12:39:16 PM ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO



ALTER PROC [dbo].[sproc_cp_dashboard_v2]
    @flag VARCHAR(10),
    @ResultType VARCHAR(10) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @CustomerId VARCHAR(10) = NULL
AS
DECLARE @DefaultLocationId VARCHAR(10) = 9;
BEGIN
    IF @flag = '1' --get new club list
    BEGIN
        IF ISNULL(@ResultType, '') = '1'
        BEGIN
            ;WITH CTE
             AS (SELECT TOP (6)
                        a.AgentId AS ClubId,
                        a.ClubName1 AS ClubName,
                        a.ClubName2 AS ClubNameJapanese,
                        a.Logo AS ClubLogo,
                        a.LocationId AS ClubLocationId,
                        CASE
                            WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )               THEN
                                'Y'
                            ELSE
                                'N'
                        END AS IsBookmarked
                 FROM dbo.tbl_club_details a WITH (NOLOCK)
                     INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                         ON b.AgentId = a.AgentId
                            AND b.RoleType = 4
                            AND ISNULL(a.Status, '') = 'A'
                            AND ISNULL(b.Status, '') = 'A'
                 WHERE a.LocationId = @LocationId
                       AND FORMAT(a.ActionDate, 'yyyy-MM-dd') >= FORMAT(DATEADD(MONTH, -1, GETDATE()), 'yyyy-MM-dd'))
            SELECT *
            FROM
            (
                SELECT ClubId,
                       ClubName,
                       ClubNameJapanese,
                       ClubLogo,
                       ClubLocationId,
                       IsBookmarked
                FROM CTE
                UNION ALL
                SELECT TOP (6)
                       a.AgentId AS ClubId,
                       a.ClubName1 AS ClubName,
                       a.ClubName2 AS ClubNameJapanese,
                       a.Logo AS ClubLogo,
                       a.LocationId AS ClubLocationId,
                       CASE
                           WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )              THEN
                               'Y'
                           ELSE
                               'N'
                       END AS IsBookmarked
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND b.RoleType = 4
                           AND ISNULL(a.Status, '') = 'A'
                           AND ISNULL(b.Status, '') = 'A'
                WHERE NOT EXISTS
                (
                    SELECT 1 FROM CTE WHERE CTE.ClubId = a.AgentId
                )
                      AND a.LocationId = @DefaultLocationId
                      AND FORMAT(a.ActionDate, 'yyyy-MM-dd') >= FORMAT(DATEADD(MONTH, -1, GETDATE()), 'yyyy-MM-dd')
            ) AS Result
            ORDER BY NEWID();

            RETURN;
        END;
        ELSE
        BEGIN
            ;WITH CTE
             AS (SELECT a.AgentId AS ClubId,
                        a.ClubName1 AS ClubName,
                        a.ClubName2 AS ClubNameJapanese,
                        a.Logo AS ClubLogo,
                        a.LocationId AS ClubLocationId,
                        CASE
                            WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )               THEN
                                'Y'
                            ELSE
                                'N'
                        END AS IsBookmarked
                 FROM dbo.tbl_club_details a WITH (NOLOCK)
                     INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                         ON b.AgentId = a.AgentId
                            AND b.RoleType = 4
                            AND ISNULL(a.Status, '') = 'A'
                            AND ISNULL(b.Status, '') = 'A'
                 WHERE a.LocationId = @LocationId
                       AND FORMAT(a.ActionDate, 'yyyy-MM-dd') >= FORMAT(DATEADD(MONTH, -1, GETDATE()), 'yyyy-MM-dd'))
            SELECT *
            FROM
            (
                SELECT ClubId,
                       ClubName,
                       ClubNameJapanese,
                       ClubLogo,
                       ClubLocationId,
                       IsBookmarked
                FROM CTE
                UNION ALL
                SELECT a.AgentId AS ClubId,
                       a.ClubName1 AS ClubName,
                       a.ClubName2 AS ClubNameJapanese,
                       a.Logo AS ClubLogo,
                       a.LocationId AS ClubLocationId,
                       CASE
                           WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )              THEN
                               'Y'
                           ELSE
                               'N'
                       END AS IsBookmarked
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND b.RoleType = 4
                           AND ISNULL(a.Status, '') = 'A'
                           AND ISNULL(b.Status, '') = 'A'
                WHERE NOT EXISTS
                (
                    SELECT 1 FROM CTE WHERE CTE.ClubId = a.AgentId
                )
                      AND a.LocationId = @DefaultLocationId
                      AND FORMAT(a.ActionDate, 'yyyy-MM-dd') >= FORMAT(DATEADD(MONTH, -1, GETDATE()), 'yyyy-MM-dd')
            ) AS Result
            ORDER BY NEWID();
            RETURN;
        END;
    END;

    ELSE IF @flag = '2' --get new host list
    BEGIN
        IF ISNULL(@ResultType, '') = '1'
        BEGIN
            ;WITH CTE
             AS (SELECT TOP (6)
                        b.AgentId AS ClubId,
                        b.ClubName1 AS ClubNameEnglish,
                        b.ClubName2 AS ClubNameJapanese,
                        b.Logo AS ClubLogo,
                        a.HostId,
                        a.HostName AS HostNameEnglish,
                        a.HostNameJapanese,
                        a.ImagePath AS HostLogo,
                        b.LocationId AS ClubLocationId,
                        CASE
                            WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )               THEN
                                'Y'
                            ELSE
                                'N'
                        END AS IsBookmarked
                 FROM dbo.tbl_host_details a WITH (NOLOCK)
                     INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                         ON a.AgentId = b.AgentId
                     INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                         ON b.AgentId = c.AgentId
                 WHERE b.LocationId = @LocationId
                       AND c.RoleType = 4
                       AND b.Status = 'A'
                       AND c.Status = 'A'
                       AND a.ActionDate >= DATEADD(MONTH, -1, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0)))
            SELECT *
            FROM
            (
                SELECT a.ClubId,
                       a.ClubNameEnglish,
                       a.ClubNameJapanese,
                       a.ClubLogo,
                       a.HostId,
                       a.HostNameEnglish,
                       a.HostNameJapanese,
                       a.HostLogo,
                       a.ClubLocationId,
                       a.IsBookmarked
                FROM CTE a WITH (NOLOCK)
                UNION ALL
                SELECT TOP (6)
                       b.AgentId AS ClubId,
                       b.ClubName1 AS ClubNameEnglish,
                       b.ClubName2 AS ClubNameJapanese,
                       b.Logo AS ClubLogo,
                       a.HostId,
                       a.HostName AS HostNameEnglish,
                       a.HostNameJapanese,
                       a.ImagePath AS HostLogo,
                       b.LocationId AS ClubLocationId,
                       CASE
                           WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )              THEN
                               'Y'
                           ELSE
                               'N'
                       END AS IsBookmarked
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                        ON a.AgentId = b.AgentId
                    INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                        ON b.AgentId = c.AgentId
                WHERE b.LocationId = @DefaultLocationId
                      AND c.RoleType = 4
                      AND b.Status = 'A'
                      AND c.Status = 'A'
                      AND a.ActionDate >= DATEADD(MONTH, -1, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0))
            ) AS Result
            ORDER BY NEWID();
            RETURN;
        END;
        ELSE
        BEGIN
            ;WITH CTE
             AS (SELECT b.AgentId AS ClubId,
                        b.ClubName1 AS ClubNameEnglish,
                        b.ClubName2 AS ClubNameJapanese,
                        b.Logo AS ClubLogo,
                        a.HostId,
                        a.HostName AS HostNameEnglish,
                        a.HostNameJapanese,
                        a.ImagePath AS HostLogo,
                        b.LocationId AS ClubLocationId,
                        CASE
                            WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )               THEN
                                'Y'
                            ELSE
                                'N'
                        END AS IsBookmarked
                 FROM dbo.tbl_host_details a WITH (NOLOCK)
                     INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                         ON a.AgentId = b.AgentId
                     INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                         ON b.AgentId = c.AgentId
                 WHERE b.LocationId = @LocationId
                       AND c.RoleType = 4
                       AND b.Status = 'A'
                       AND c.Status = 'A'
                       AND a.ActionDate >= DATEADD(MONTH, -1, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0)))
            SELECT *
            FROM
            (
                SELECT a.ClubId,
                       a.ClubNameEnglish,
                       a.ClubNameJapanese,
                       a.ClubLogo,
                       a.HostId,
                       a.HostNameEnglish,
                       a.HostNameJapanese,
                       a.HostLogo,
                       a.ClubLocationId,
                       IsBookmarked
                FROM CTE a WITH (NOLOCK)
                UNION ALL
                SELECT b.AgentId AS ClubId,
                       b.ClubName1 AS ClubNameEnglish,
                       b.ClubName2 AS ClubNameJapanese,
                       b.Logo AS ClubLogo,
                       a.HostId,
                       a.HostName AS HostNameEnglish,
                       a.HostNameJapanese,
                       a.ImagePath AS HostLogo,
                       b.LocationId AS ClubLocationId,
                       CASE
                           WHEN EXISTS
        (
            SELECT 1
            FROM dbo.tbl_bookmark tb WITH (NOLOCK)
            WHERE tb.CustomerId = @CustomerId
                  AND tb.AgentType = 'club'
                  AND tb.ClubId = a.AgentId
                  AND tb.Status = 'A'
        )              THEN
                               'Y'
                           ELSE
                               'N'
                       END AS IsBookmarked
                FROM dbo.tbl_host_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                        ON a.AgentId = b.AgentId
                    INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                        ON b.AgentId = c.AgentId
                WHERE b.LocationId = @DefaultLocationId
                      AND c.RoleType = 4
                      AND b.Status = 'A'
                      AND c.Status = 'A'
                      AND a.ActionDate >= DATEADD(MONTH, -1, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0))
            ) AS Result
            ORDER BY NEWID();
            RETURN;
        END;

    END;
END;
GO

