USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_get_function]    Script Date: 11/10/2023 23:26:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_get_function] @Flag VARCHAR(10),
@RoleId VARCHAR(10) = NULL
AS
DECLARE @RoleType VARCHAR(50);
BEGIN	
	IF @Flag = 'gaf' --get admin functions
	BEGIN
		SELECT @RoleType = a.RoleDescription
		FROM tbl_roles a WITH (NOLOCK)
		WHERE a.Id = @RoleId
			AND ISNULL(a.[Status], '') = 'A';

		IF @RoleType = 'Superadmin'
		BEGIN
			SELECT DISTINCT a.MenuName,
							a.menuId,
							a.ParentGroup,
							b.FunctionURL
			FROM tbl_menus a WITH (NOLOCK)
			INNER JOIN tbl_application_functions b WITH (NOLOCK) ON b.MenuId = a.MenuId AND ISNULL(b.[Status], '') = 'A'
			WHERE a.MenuAccessCategory = 'Admin'
				AND ISNULL(a.[Status], '') = 'A'
			RETURN;
		END
		ELSE
		BEGIN
			SELECT DISTINCT a.MenuName,
							a.menuId,
							a.ParentGroup,
							b.FunctionURL
			FROM tbl_menus a WITH (NOLOCK)
			INNER JOIN tbl_application_functions b WITH (NOLOCK) ON b.MenuId = a.MenuId AND ISNULL(b.[Status], '') = 'A'
			INNER JOIN tbl_application_functions_role c WITH (NOLOCK) ON c.FunctionId= b.FunctionId AND c.RoleId = @RoleId
			WHERE a.MenuAccessCategory = 'Admin'
				AND ISNULL(a.[Status], '') = 'A'
			RETURN;
		END
	END
END
