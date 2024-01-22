USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_menu_management]    Script Date: 01/12/2023 20:15:03 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO




ALTER PROC [dbo].[sproc_menu_management]
    @Flag VARCHAR(10),
    @RoleId BIGINT = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @Roles VARCHAR(MAX) = NULL
AS
DECLARE @StringSQL VARCHAR(MAX);
BEGIN
    IF ISNULL(@Flag, '') = 'gm' --get menu
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM tbl_roles a WITH (NOLOCK)
            WHERE a.Id = @RoleId
                  AND a.RoleType = '1'
        )
        BEGIN
            SELECT a.MenuName,
                   a.MenuId,
                   a.ParentGroup,
                   CASE
                       WHEN b.Sno IS NULL THEN
                           'N'
                       ELSE
                           'Y'
                   END Status,
                   a.MenuUrl
            FROM tbl_menus a WITH (NOLOCK)
                LEFT JOIN tbl_menus_role b WITH (NOLOCK)
                    ON b.MenuId = a.MenuId
                       AND b.RoleId = @RoleId
            WHERE ISNULL(a.Status, '') = 'A'
                  AND a.MenuAccessCategory = 'Admin'
			ORDER BY a.MenuName ASC;
            RETURN;
        END;
        ELSE IF EXISTS
        (
            SELECT 'X'
            FROM tbl_roles a WITH (NOLOCK)
            WHERE a.Id = @RoleId
                  AND a.RoleType = '4'
        )
        BEGIN
            SELECT a.MenuName,
                   a.MenuId,
                   a.ParentGroup,
                   CASE
                       WHEN b.Sno IS NULL THEN
                           'N'
                       ELSE
                           'Y'
                   END Status,
                   a.MenuUrl
            FROM tbl_menus a WITH (NOLOCK)
                LEFT JOIN tbl_menus_role b WITH (NOLOCK)
                    ON b.MenuId = a.MenuId
                       AND b.RoleId = @RoleId
            WHERE ISNULL(a.Status, '') = 'A'
                  AND a.MenuAccessCategory = 'Club'
			ORDER BY a.MenuName ASC;
            RETURN;
        END;
    END;
    ELSE IF ISNULL(@Flag, '') = 'am' -- assign menu
    BEGIN
        DELETE FROM tbl_menus_role
        WHERE RoleId = @RoleId;

        SELECT @StringSQL
            = 'INSERT INTO tbl_menus_role' + '(RoleId,MenuId,ActionUser,ActionIP,ActionDate) ' + 'SELECT '
              + CAST(@RoleId AS VARCHAR) + ',MenuId,''' + ISNULL(@ActionUser, '') + ''',''' + @ActionIP
              + ''',GETDATE() FROM tbl_menus WHERE MenuId IN (' + @Roles + ')';
        PRINT (@StringSQL);
        EXEC (@StringSQL);

        SELECT 0 Code,
               'Roles assigned successfully' Message;
        RETURN;
    END;
END;
GO


