USE [CRS.UAT_V2]
GO
/****** Object:  StoredProcedure [dbo].[sproc_recommendation_group_management]    Script Date: 7/9/2024 11:07:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_recommendation_group_management]
    @Flag VARCHAR(10),
    @GroupName NVARCHAR(200) = NULL,
    @Description NVARCHAR(512) = NULL,
    @DisplayOrderId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @GroupId VARCHAR(10) = NULL,
    @SearchField NVARCHAR(200) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @ActionPlatfrom VARCHAR(20) = NULL
AS
DECLARE @Sno BIGINT,
        @SQL NVARCHAR(MAX) = N'',
        @SQLParameter NVARCHAR(MAX) = N'',
        @OrderQuery NVARCHAR(MAX) = N'',
        @PositionId INT;
BEGIN
    IF @Flag = 'cg' --create group
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_group a WITH (NOLOCK)
            WHERE a.GroupName = @GroupName and LocationId=@LocationId
                  AND Status = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Group name already exists' Message;
            RETURN;
        END;

        INSERT INTO dbo.tbl_recommendation_group
        (
            GroupName,
            Description,
            DisplayOrderId, -- Pagination id like 1 for page 1, 2 for page 2 etc
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
			LocationId
        )
        VALUES
        (@GroupName, @Description, @DisplayOrderId, 'A', @ActionUser, GETDATE(), @ActionIP,	@LocationId);

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_recommendation_group
        SET GroupId = @Sno
        WHERE Sno = @Sno;

        --SELECT @PositionId = MAX(a.PositionId) + 1
        --FROM dbo.tbl_recommendation_group_pagination a WITH (NOLOCK)
        --WHERE ISNULL(a.Status, '') = 'A' and  @LocationId; 
		  select @PositionId = MAX(a.PositionId) + 1
				FROM dbo.tbl_recommendation_group_pagination a WITH (NOLOCK)
			    INNER JOIN dbo.tbl_recommendation_group b WITH (NOLOCK) ON b.GroupId = a.GroupId
			WHERE 
				ISNULL(a.Status, '') = 'A' AND b.LocationId = @LocationId

        INSERT INTO dbo.tbl_recommendation_group_pagination
        (
            GroupId,
            PositionId,
            Status,
            ActionUser,
            ActionDate,
            ActionIP,
            ActionPlatfrom
        )
        VALUES
        (   @Sno,           -- GroupId - bigint
            ISNULL(@PositionId, 1),    -- PositionId - int
            'A',            -- Status - char(1)
            @ActionUser,    -- ActionUser - varchar(200)
            GETDATE(),      -- ActionDate - datetime
            @ActionIP,      -- ActionIP - varchar(50)
            @ActionPlatfrom -- ActionPlatfrom - varchar(20)
            );
        SELECT 0 Code,
               'Recommendation group added successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'ug' --update group
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_group a WITH (NOLOCK)
            WHERE GroupId = @GroupId
                  AND Status IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_recommendation_group
        SET Description = ISNULL(@Description, Description),
            DisplayOrderId = ISNULL(@DisplayOrderId, DisplayOrderId),
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP
        WHERE GroupId = @GroupId
              AND Status IN ( 'A' );

        SELECT 0 Code,
               'Recommendation group updated successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'grgl' --get recommendation group list
    BEGIN
        SET @OrderQuery = N' ORDER BY a.GroupName ASC';
        IF @SearchField IS NOT NULL
        BEGIN
            SET @SQLParameter = N' AND a.GroupName LIKE N''%' + @SearchField + N'%''';
        END;

        SET @SQL
            = N'SELECT a.GroupId
			   ,a.GroupName
			   ,a.Description
			   ,a.DisplayOrderId
			   ,a.Status
			   ,a.CreatedBy
			   ,a.CreatedDate
			   ,a.CreatedIP
			    ,(SELECT COUNT(GroupId) FROM tbl_display_mainpage b WITH (NOLOCK) 
				INNER JOIN dbo.tbl_recommendation_detail d WITH (nolock) ON d.RecommendationId = b.RecommendationId
				INNER JOIN dbo.tbl_club_details c WITH (NOLOCK)
                ON c.AgentId = d.ClubId
                   AND ISNULL(c.Status, '''') = ''A''
				WHERE d.LocationId = c.LocationId AND b.GroupId = a.GroupId AND b.Status = ''A'') AS TotalClubs
			   ,c.LocationId
		FROM tbl_recommendation_group a WITH (NOLOCK)
		INNER JOIN dbo.tbl_location c WITH (NOLOCK) ON c.LocationId = ''' + @LocationId
              + N'''
			AND ISNULL(c.Status, '''') = ''A''
		WHERE  a.LocationId=''' + @LocationId + ''' AND a.Status IN (''A'')' + @SQLParameter + @OrderQuery;

        PRINT (@SQL);
        EXEC (@SQL);

        RETURN;
    END;
END;
