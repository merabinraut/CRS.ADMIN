USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_get_menus]    Script Date: 11/10/2023 23:22:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_get_menus] @Flag VARCHAR(10),
@Username VARCHAR(200) = NULL
AS
DECLARE @RoleType VARCHAR(50);
BEGIN
	IF ISNULL(@Flag, '') = 'gam' --get admin menus
	BEGIN
		SELECT @RoleType = b.RoleDescription
		FROM tbl_admin a WITH (NOLOCK)
		INNER JOIN tbl_roles b WITH (NOLOCK) ON b.Id = a.RoleId
		WHERE a.UserName = @Username
			AND ISNULL(a.[Status], '') = 'A'
			print(@RoleType)
		IF ISNULL(@RoleType, '') IN ('Superadmin')
		BEGIN
			SELECT a.MenuName,
				   a.MenuUrl,
				   a.MenuGroup,
				   a.ParentGroup,
				   a.CssClass,
				   a.GroupOrderPosition,
				   a.MenuOrderPosition
			FROM tbl_menus a WITH (NOLOCK)
			WHERE ISNULL(a.[Status], '') = 'A'
				AND a.MenuAccessCategory = 'Admin'
			ORDER BY a.GroupOrderPosition,
				a.MenuOrderPosition,
				a.ParentGroup
			RETURN;
		END
		ELSE
		BEGIN
			SELECT a.MenuName,
				   a.MenuUrl,
				   a.MenuGroup,
				   a.ParentGroup,
				   a.CssClass,
				   a.GroupOrderPosition,
				   a.MenuOrderPosition
			FROM tbl_menus a WITH (NOLOCK)
			WHERE ISNULL(a.[Status], '') = 'A'
				AND a.MenuAccessCategory = 'Admin'
				AND a.MenuId IN 
				(
					SELECT b2.MenuId 
					FROM tbl_admin a2 WITH (NOLOCK)
					INNER JOIN tbl_menus_role b2 WITH (NOLOCK) ON b2.RoleId = a2.RoleId
					WHERE a2.UserName = @UserName
				)
			ORDER BY a.GroupOrderPosition,
				a.MenuOrderPosition,
				a.ParentGroup
			RETURN;
		END
	END
END
