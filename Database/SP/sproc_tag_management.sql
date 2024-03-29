USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_tag_management]    Script Date: 3/4/2024 11:10:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sproc_tag_management]
    -- Add the parameters for the stored procedure here
    @Flag VARCHAR(10) = NULL,
    @ClubId VARCHAR(10) = NULL,
    @Tag1Location VARCHAR(512) = NULL,
    @Tag1Status CHAR(1) = NULL,
    @Tag2RankName VARCHAR(100) = NULL,
    @Tag2RankDescription VARCHAR(512) = NULL,
    @Tag2Status CHAR(1) = NULL,
    @Tag3CategoryName VARCHAR(100) = NULL,
    @Tag3CategoryDescription VARCHAR(512) = NULL,
    @Tag3Status CHAR(1) = NULL,
    @Tag4ExcellencyName VARCHAR(100) = NULL,
    @Tag4ExcellencyDescription VARCHAR(512) = NULL,
    @Tag4Status CHAR(1) = NULL,
    @Tag5StoreName VARCHAR(100) = NULL,
    @Tag5StoreDescription VARCHAR(512) = NULL,
    @Tag5Status CHAR(1) = NULL,
    @ActionUser VARCHAR(100) = NULL,
    @ActionIp VARCHAR(100) = NULL,
    @TagId VARCHAR(10) = NULL
AS
BEGIN
    DECLARE @Sno BIGINT;
    IF ISNULL(@Flag, '') = 'it' --insert tag
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_tag_detail td WITH (NOLOCK)
            WHERE td.ClubId = @ClubId
        )
        BEGIN
            SELECT 1 Code,
                   'Tags are already exists' Message;
            RETURN;
        END;

		If exists(select 'x' from tbl_tag_detail  where Tag2RankDescription=@Tag2RankName)
        begin
         SELECT 1 Code,
                   'Rank is occupied.' Message;
            RETURN;
        end

        INSERT INTO dbo.tbl_tag_detail
        (
            ClubId,
            Tag1Location,
            Tag1Status,
            Tag2RankName,
            Tag2RankDescription,
            Tag2Status,
            Tag3CategoryName,
            Tag3CategoryDescription,
            Tag3Status,
            --Tag4ExcellencyName,
            --Tag4ExcellencyDescription,
            Tag4Status,
            Tag5StoreName,
            Tag5SoteDescription,
            Tag5Status,
            ActionUser,
            ActionIP,
            ActionDate
        )
        VALUES
        (   @ClubId,           -- ClubId - bigint
            @Tag1Location,     -- Tag1Location - varchar(512)
            @Tag1Status,       -- Tag1Status - char(1)
            @Tag2RankName,     -- Tag2RankName - varchar(100)
            @Tag2RankName,     -- Tag2RankDescription - varchar(512)
            @Tag2Status,       -- Tag2Status - char(1)
            @Tag3CategoryName, -- Tag3CategoryName - varchar(100)
            @Tag3CategoryName, -- Tag3CategoryDescription - varchar(512)
            @Tag3Status,       -- Tag3Status - char(1)
                               --@Tag4ExcellencyName,        -- Tag4ExcellencyName - varchar(100)
                               --@Tag4ExcellencyDescription, -- Tag4ExcellencyDescription - varchar(512)
            @Tag4Status,       -- Tag4Status - char(1)
            @Tag5StoreName,    -- Tag5StoreName - varchar(100)
            @Tag5StoreName,    -- Tag5SoteDescription - varchar(512)
            @Tag5Status,       -- Tag5Status - char(1)
            @ActionUser,       -- ActionUser - varchar(200)
            @ActionIp,         -- ActionIP - varchar(50)
            GETDATE()          -- ActionDate - datetime
            );
        SET @Sno = SCOPE_IDENTITY();
        UPDATE dbo.tbl_tag_detail
        SET TagId = @Sno
        WHERE Sno = @Sno;
        SELECT 0 Code,
               'Tag Added Successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'ut' --update tag
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_tag_detail td WITH (NOLOCK)
            WHERE td.ClubId = @ClubId
                  AND td.TagId = @TagId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalids Tag Details' Message;
            RETURN;
        END;

		If exists(select 'x' from tbl_tag_detail  where Tag2RankDescription=@Tag2RankName and ClubId != @ClubId
                  and TagId! = @TagId)
        begin
         SELECT 1 Code,
                   'Rank is occupied.' Message;
            RETURN;
        end

        UPDATE dbo.tbl_tag_detail
        SET Tag1Location = ISNULL(@Tag1Location, Tag1Location),
            Tag1Status = ISNULL(@Tag1Status, Tag1Status),
            Tag2RankName = ISNULL(@Tag2RankName, Tag2RankName),
            Tag2RankDescription = ISNULL(@Tag2RankName, Tag2RankDescription),
            Tag2Status = ISNULL(@Tag2Status, Tag2Status),
            Tag3CategoryName = ISNULL(@Tag3CategoryName, Tag3CategoryName),
            Tag3CategoryDescription = ISNULL(@Tag3CategoryName, Tag3CategoryDescription),
            Tag3Status = ISNULL(@Tag3Status, Tag3Status),
            --Tag4ExcellencyName = ISNULL(@Tag4ExcellencyName, Tag4ExcellencyName),
            --Tag4ExcellencyDescription = ISNULL(@Tag4ExcellencyDescription, Tag4ExcellencyDescription),
            Tag4Status = ISNULL(@Tag4Status, Tag4Status),
            Tag5StoreName = ISNULL(@Tag5StoreName, Tag5StoreName),
            Tag5SoteDescription = ISNULL(Tag5StoreName, Tag5SoteDescription),
            Tag5Status = ISNULL(@Tag5Status, Tag5Status),
            ActionUser = @ActionUser,
            ActionIP = @ActionIp,
            ActionDate = GETDATE()
        WHERE ClubId = @ClubId
              AND TagId = @TagId;
        SELECT 0 Code,
               'Tag details updated successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gtd' --get tag detail clubid wise
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_tag_detail a WITH (NOLOCK)
            WHERE a.ClubId = @ClubId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid tag details' Message;
            RETURN;
        END;
        SELECT 0 Code,
               'Success' Message,
               td.TagId AS TagId,
               td.Tag1Location AS Tag1Location,
               td.Tag1Status AS Tag1Status,
               td.Tag2RankName AS Tag2RankName,
               td.Tag2RankDescription AS Tag2RankDescription,
               td.Tag2Status AS Tag2Status,
               td.Tag3CategoryName AS Tag3CategoryName,
               td.Tag3CategoryDescription AS Tag3CategoryDescription,
               td.Tag3Status AS Tag3Status,
               td.Tag4ExcellencyName AS Tag4ExcellencyName,
               td.Tag4ExcellencyDescription AS Tag4ExcellencyDescription,
               td.Tag4Status AS Tag4Status,
               td.Tag5StoreName AS Tag5StoreName,
               td.Tag5SoteDescription AS Tag5StoreDescription,
               td.Tag5Status AS Tag5Status
        FROM dbo.tbl_tag_detail td
        WHERE td.ClubId = @ClubId;
        RETURN;
    END;
END;
