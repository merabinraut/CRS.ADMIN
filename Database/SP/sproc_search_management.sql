USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_search_management]    Script Date: 06/12/2023 16:28:20 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


ALTER PROC [dbo].[sproc_search_management]
    @Flag VARCHAR(10),
    @SearchText VARCHAR(100) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @AgentId VARCHAR(8) = NULL,
    @ClubCategory VARCHAR(8) = NULL,
    @Shift VARCHAR(8) = NULL,
    @Rank VARCHAR(8) = NULL,
    @Height VARCHAR(8) = NULL,
    @BloodType VARCHAR(8) = NULL,
    @Zodiac VARCHAR(8) = NULL,
    @LiquorStrength VARCHAR(8) = NULL,
    @PrevOccupation VARCHAR(8) = NULL,
    @Handsomeness VARCHAR(100) = NULL
AS
BEGIN
    DECLARE @sqlQuery NVARCHAR(MAX);

    IF ISNULL(@Flag, '') = 'gncl' --get new club list
    BEGIN
        SELECT TOP 20
               a.AgentId AS ClubId,
               a.ClubName1 AS ClubName,
               a.Logo AS ClubLogo
        FROM dbo.tbl_club_details a WITH (NOLOCK);

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'csm' --club search management
    BEGIN
        --SELECT a.AgentId AS ClubId
        --	,a.LocationId AS LocationId
        --	,a.ClubName1 AS ClubName
        --	,a.GroupName
        --	,(ISNULL(a.FirstName, '') + ISNULL(a.MiddleName, '') + ' ' + ISNULL(a.LastName, '')) AS FullName
        --	,a.Logo AS ClubLogo
        --	,a.CoverPhoto AS ClubCoverPhoto
        --	,a.Description AS ClubDescription
        --	,c.LocationName AS Tag1
        --	,CASE 
        --		WHEN ISNULL(b.Tag2Status, '') = 'A'
        --			THEN b.Tag2RankName
        --		ELSE ''
        --		END AS Tag2
        --	,CASE 
        --		WHEN ISNULL(b.Tag3Status, '') = 'A'
        --			THEN b.Tag3CategoryName
        --		ELSE ''
        --		END AS Tag3
        --	,CASE 
        --		WHEN ISNULL(b.Tag4Status, '') = 'A'
        --			THEN b.Tag4ExcellencyName
        --		ELSE ''
        --		END AS Tag4
        --	,CASE 
        --		WHEN ISNULL(b.Tag5Status, '') = 'A'
        --			THEN b.Tag5StoreName
        --		ELSE ''
        --		END AS Tag5
        --	,a.ClubOpeningTime
        --	,a.ClubClosingTime
        --FROM dbo.tbl_club_details a WITH (NOLOCK)
        --LEFT JOIN dbo.tbl_tag_detail b WITH (NOLOCK) ON b.ClubId = a.AgentId
        --LEFT JOIN dbo.tbl_location c WITH (NOLOCK) ON c.LocationId = b.Tag1Location
        --	AND b.Tag1Status = 'A'
        --	AND ISNULL(c.[Status], '') = 'A'
        --WHERE a.ClubName1 LIKE '%' + ISNULL(@SearchText, a.ClubName1) + '%'
        --	AND a.LocationId = ISNULL(@LocationId, a.LocationId)
        --	AND a.AgentId = ISNULL(@AgentId, a.AgentId)
        --	AND b.Tag3CategoryName = ISNULL(@ClubCategory, b.Tag3CategoryName)
        --	AND ISNULL(a.[Status], '') = 'A'
        --	AND (
        --		(
        --			@Shift = 'day'
        --			AND (
        --				CAST(a.ClubOpeningTime AS TIME) >= '10:00'
        --				OR CAST(a.ClubOpeningTime AS TIME) <= '05:59'
        --				)
        --			AND (
        --				CAST(a.ClubClosingTime AS TIME) >= '10:00'
        --				OR CAST(a.ClubClosingTime AS TIME) <= '05:59'
        --				)
        --			)
        --		OR (
        --			@Shift = 'night'
        --			AND (
        --				CAST(a.ClubOpeningTime AS TIME) >= '06:00'
        --				OR CAST(a.ClubOpeningTime AS TIME) <= '23:50'
        --				)
        --			AND (
        --				CAST(a.ClubClosingTime AS TIME) >= '06:00'
        --				OR CAST(a.ClubClosingTime AS TIME) <= '23:50'
        --				)
        --			)
        --		);
        --RETURN;
        SET @sqlQuery
            = N'
			SELECT a.AgentId AS ClubId
				,a.LocationId AS LocationId
				,a.ClubName1 AS ClubName
				,a.GroupName
				,(ISNULL(a.FirstName, '''') + ISNULL(a.MiddleName, '''') + '' '' + ISNULL(a.LastName, '''')) AS FullName
				,a.Logo AS ClubLogo
				,a.CoverPhoto AS ClubCoverPhoto
				,a.Description AS ClubDescription
				,c.LocationName AS Tag1
				,CASE 
					WHEN ISNULL(b.Tag2Status, '''') = ''A''
						THEN b.Tag2RankName
					ELSE '''' 
					END AS Tag2
				,CASE 
					WHEN ISNULL(b.Tag3Status, '''') = ''A''
						THEN b.Tag3CategoryName
					ELSE '''' 
					END AS Tag3
				,CASE 
					WHEN ISNULL(b.Tag4Status, '''') = ''A''
						THEN b.Tag4ExcellencyName
					ELSE '''' 
					END AS Tag4
				,CASE 
					WHEN ISNULL(b.Tag5Status, '''') = ''A''
						THEN b.Tag5StoreName
					ELSE '''' 
					END AS Tag5
				,a.ClubOpeningTime
				,a.ClubClosingTime
			FROM dbo.tbl_club_details a WITH (NOLOCK)
			LEFT JOIN dbo.tbl_tag_detail b WITH (NOLOCK) ON b.ClubId = a.AgentId
			LEFT JOIN dbo.tbl_location c WITH (NOLOCK) ON c.LocationId = b.Tag1Location
				AND b.Tag1Status = ''A''
				AND ISNULL(c.[Status], '''') = ''A''
			WHERE 1=1';

        IF @SearchText IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND a.ClubName1 LIKE ''%'+ @SearchText + '%''';
        END;

        IF @LocationId IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND a.LocationId = ' + CAST(@LocationId AS NVARCHAR);
        END;

        IF @AgentId IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND a.AgentId = ' + CAST(@AgentId AS NVARCHAR);
        END;

        IF @ClubCategory IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.Tag3CategoryName = ''' + @ClubCategory + N'''';
        END;

        IF @Shift IS NOT NULL
        BEGIN
            IF @Shift = 'day'
            BEGIN
                SET @sqlQuery
                    = @sqlQuery
                      + N' AND (CAST(a.ClubOpeningTime AS TIME) >= ''10:00'' OR CAST(a.ClubOpeningTime AS TIME) <= ''05:59'')';
                SET @sqlQuery
                    = @sqlQuery
                      + N' AND (CAST(a.ClubClosingTime AS TIME) >= ''10:00'' OR CAST(a.ClubClosingTime AS TIME) <= ''05:59'')';
            END;
            ELSE IF @Shift = 'night'
            BEGIN
                SET @sqlQuery
                    = @sqlQuery
                      + N' AND (CAST(a.ClubOpeningTime AS TIME) >= ''06:00'' OR CAST(a.ClubOpeningTime AS TIME) <= ''23:50'')';
                SET @sqlQuery
                    = @sqlQuery
                      + N' AND (CAST(a.ClubClosingTime AS TIME) >= ''06:00'' OR CAST(a.ClubClosingTime AS TIME) <= ''23:50'')';
            END;
        END;

        SET @sqlQuery = @sqlQuery + N' AND ISNULL(a.[Status], '''') = ''A'';';
        PRINT @sqlQuery;
        EXEC sp_executesql @sqlQuery;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'hsm' --host search management
    BEGIN
        SET @sqlQuery
            = N'
						SELECT a.LocationId
							,b.AgentId ClubId
							,b.HostId
							,b.HostName
							,b.ImagePath HostImage
							,d.StaticDataLabel AS Occupation
							,b.Rank
						FROM dbo.tbl_club_details a WITH (NOLOCK)
						INNER JOIN dbo.tbl_host_details b WITH (NOLOCK) ON b.AgentId = a.AgentId
							AND ISNULL(a.[Status], '''') = ''A''
						INNER JOIN tbl_static_data d WITH (NOLOCK) ON d.StaticDataType = 12
							AND d.StaticDataValue = b.PreviousOccupation
							AND isnull(d.STATUS, '''') = ''A''
						WHERE 1 = 1';

        IF @LocationId IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND a.LocationId = ' + CAST(@LocationId AS NVARCHAR);
        END;

        IF @SearchText IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.HostName LIKE ''%'+ @SearchText + '%''';
        END;

        IF @AgentId IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.HostId = ' + CAST(@AgentId AS NVARCHAR);
        END;

        IF @Rank IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.Rank = ''' + CAST(@Rank AS NVARCHAR) + N'''';
        END;

        IF @Height IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.Height = ''' + CAST(@Height AS NVARCHAR) + N'''';
        END;

        IF @BloodType IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.BloodType = ''' + CAST(@BloodType AS NVARCHAR) + N'''';
        END;

        IF @Zodiac IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.ConstellationGroup = ''' + CAST(@Zodiac AS NVARCHAR) + N'''';
        END;

        IF @LiquorStrength IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.LiquorStrength = ''' + CAST(@LiquorStrength AS NVARCHAR) + N'''';
        END;

        IF @PrevOccupation IS NOT NULL
        BEGIN
            SET @sqlQuery = @sqlQuery + N' AND b.PreviousOccupation = ''' + CAST(@PrevOccupation AS NVARCHAR) + N'''';
        END;

        SET @sqlQuery = @sqlQuery + N' AND ISNULL(b.[Status], '''') = ''A'';';

        PRINT @sqlQuery;

        EXEC sp_executesql @sqlQuery;

        RETURN;
    END;
END;
GO


