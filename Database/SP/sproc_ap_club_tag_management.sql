USE CRS;
GO

ALTER PROC [dbo].[sproc_ap_club_tag_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @TagType VARCHAR(10) = NULL,
    @TagId VARCHAR(10) = NULL,
    @TagDescription NVARCHAR(MAX) = NULL,
    @TagStatus CHAR(1) = NULL,
    @ActionUser NVARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL
AS
BEGIN
    IF @Flag = 'gtl' --get tag list
    BEGIN
        CREATE TABLE #temp_gtl
        (
            StaticType VARCHAR(10),
            StaticLabel NVARCHAR(MAX),
            StaticValue VARCHAR(10),
            StaticDescription NVARCHAR(MAX),
            StaticStatus CHAR(1)
        );

        INSERT INTO #temp_gtl
        SELECT a.StaticDataType,
               b.StaticDataLabel,
               b.StaticDataValue,
               '',
               'B'
        FROM dbo.tbl_static_data_type a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data b WITH (NOLOCK)
                ON b.StaticDataType = a.StaticDataType
                   AND ISNULL(a.Status, '') = 'A'
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.StaticDataType = 36;


        UPDATE a
        SET a.StaticStatus = ISNULL(b.TagStatus, 'B'),
            a.StaticDescription = ISNULL(b.TagDescription, '')
        FROM #temp_gtl a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_tag b WITH (NOLOCK)
                ON b.TagType = a.StaticType
        WHERE b.ClubId = @ClubId
              AND b.TagId = a.StaticValue;


        SELECT a.StaticType,
               a.StaticLabel,
               a.StaticValue,
               a.StaticDescription,
               a.StaticStatus
        FROM #temp_gtl a WITH (NOLOCK)
        ORDER BY a.StaticValue ASC;

        DROP TABLE #temp_gtl;

        RETURN;
    END;

    ELSE IF @Flag = 'mt' --manage tag
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club details' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_tag a WITH (NOLOCK)
            WHERE a.ClubId = @ClubId
                  AND a.TagType = @TagType
                  AND a.TagId = @TagId
        )
        BEGIN
            INSERT INTO dbo.tbl_club_tag
            (
                ClubId,
                TagType,
                TagId,
                TagDescription,
                TagStatus,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            VALUES
            (@ClubId, @TagType, @TagId, @TagDescription, ISNULL(@TagStatus, 'B'), @ActionUser, GETDATE(), @ActionIP,
             @ActionPlatform);

            SELECT 0 Code,
                   'Success' Message;
            RETURN;
        END;
        ELSE
        BEGIN
            UPDATE dbo.tbl_club_tag
            SET TagStatus = ISNULL(@TagStatus, TagStatus),
                TagDescription = ISNULL(@TagDescription, TagDescription),
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE(),
                UpdatedIP = @ActionIP,
                UpdatedPlatform = @ActionPlatform
            WHERE ClubId = @ClubId
                  AND TagType = @TagType
                  AND TagId = @TagId;

            SELECT 0 Code,
                   'Success' Message;
            RETURN;
        END;
    END;
END;
GO

