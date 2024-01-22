USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_role_management]    Script Date: 01/12/2023 14:52:49 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO




ALTER PROC [dbo].[sproc_role_management]
    @Flag VARCHAR(10),
    @RoleType VARCHAR(20) = NULL,
    @RoleName VARCHAR(50) = NULL,
    @RoleDescription VARCHAR(500) = NULL,
    @ActionUser VARCHAR(200) = NULL
AS
BEGIN
    IF ISNULL(@Flag, '') = 'grl' --get role list
    BEGIN
        SELECT a.StaticDataValue AS RoleId,
               a.StaticDataLabel AS RoleName,
               a.StaticDataDescription AS RoleDescription,
               FORMAT(a.ActionDate, 'MMM dd, yyyy hh:mm:ss') AS ActionDate,
               a.ActionUser
        FROM tbl_static_data a WITH (NOLOCK)
        WHERE a.StaticDataType = 1
              AND ISNULL(a.Status, '') = 'A'
              AND a.StaticDataValue NOT IN ( 1, 3, 5 )
        ORDER BY a.StaticDataLabel ASC;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'grtl' --get role type list
    BEGIN
        SELECT a.Id AS RoleId,
               a.RoleType,
               a.RoleName,
               a.RoleDescription,
               a.ActionUser,
               CONVERT(VARCHAR(10), a.ActionDate, 120) AS ActionDate
        FROM tbl_roles a WITH (NOLOCK)
            INNER JOIN tbl_static_data b WITH (NOLOCK)
                ON b.StaticDataValue = a.RoleType
                   AND b.StaticDataType = 1
        WHERE a.RoleType = @RoleType
              AND ISNULL(b.Status, '') = 'A'
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY a.RoleName ASC;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'irt' --insert role type
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM tbl_roles a WITH (NOLOCK)
            WHERE a.RoleName = @RoleName
        )
        BEGIN
            SELECT 1 Code,
                   'Role already exists.' Message;
            RETURN;
        END;

        INSERT INTO tbl_roles
        (
            RoleType,
            RoleName,
            RoleDescription,
            Status,
            ActionDate,
            ActionUser
        )
        VALUES
        (@RoleType, @RoleName, @RoleDescription, 'A', GETDATE(), @ActionUser);

        SELECT 0 Code,
               'Role created successfully.' Message;
        RETURN;
    END;
    ELSE
    BEGIN
        SELECT 1 Code,
               'Invalid details.' Message;
        RETURN;
    END;
END;
GO


