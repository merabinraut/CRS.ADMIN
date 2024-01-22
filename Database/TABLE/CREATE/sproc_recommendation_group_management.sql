USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_recommendation_group_management]    Script Date: 26/11/2023 20:47:44 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

ALTER PROC [dbo].[sproc_recommendation_group_management]
    @Flag VARCHAR(10) = N'grgl',
    @GroupName NVARCHAR(200) = NULL,
    @Description NVARCHAR(512) = NULL,
    @DisplayOrderId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @GroupId VARCHAR(10) = NULL,
    @SearchField VARCHAR(200) = NULL,
    @LocationId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @SQL VARCHAR(MAX) = '',
        @SQLParameter VARCHAR(MAX) = '';
BEGIN
    IF @Flag = 'cg' --create group
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_recommendation_group a WITH (NOLOCK)
            WHERE a.GroupName = @GroupName
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
            CreatedIP
        )
        VALUES
        (@GroupName, @Description, @DisplayOrderId, 'A', @ActionUser, GETDATE(), @ActionIP);

        SET @Sno = SCOPE_IDENTITY();
        UPDATE tbl_recommendation_group
        SET GroupId = @Sno
        WHERE Sno = @Sno;

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
        IF @SearchField IS NOT NULL
        BEGIN
            SET @SQLParameter = ' AND a.GroupName LIKE ''%' + @SearchField + '%''';
        END;

        SET @SQL
            = 'SELECT a.GroupId
			   ,a.GroupName
			   ,a.Description
			   ,a.DisplayOrderId
			   ,a.Status
			   ,a.CreatedBy
			   ,a.CreatedDate
			   ,a.CreatedIP
			   ,(SELECT COUNT(GroupId) FROM tbl_recommendation_detail b WITH (NOLOCK) WHERE b.LocationId = c.LocationId AND b.GroupId = a.GroupId AND b.Status = ''A'') AS TotalClubs
			   ,c.LocationId
		FROM tbl_recommendation_group a WITH (NOLOCK)
		INNER JOIN dbo.tbl_location c WITH (NOLOCK) ON c.LocationId = ''' + @LocationId
              + '''
			AND ISNULL(c.Status, '''') = ''A''
		WHERE a.Status IN (''A'')' + @SQLParameter;

        PRINT (@SQL);
        EXEC (@SQL);

        RETURN;
    END;

    ELSE IF @Flag = 'gll' --get location list
    BEGIN
        SELECT a.LocationId,
               a.LocationName,
               a.LocationImage
        FROM dbo.tbl_location a WITH (NOLOCK)
        WHERE ISNULL(a.Status, '') = 'A'
        ORDER BY a.LocationName ASC;
        RETURN;
    END;
END;
GO


