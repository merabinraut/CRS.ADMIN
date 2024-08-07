USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_function_management]    Script Date: 05/10/2023 04:27:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROC [dbo].[sproc_function_management] @Flag VARCHAR(10)
,@RoleId BIGINT = NULL
,@ActionUser VARCHAR(200) = NULL
,@ActionIP VARCHAR(50) = NULL
,@Functions VARCHAR(MAX) = NULL
AS
DECLARE @StringSQL VARCHAR(MAX);
BEGIN
	IF ISNULL(@Flag, '') = 'gf' --get function
	BEGIN
		SELECT a.FunctionName 
			  ,a.FunctionId
			  ,b.MenuName
			  ,b.MenuGroup
			  ,CASE
				WHEN d.Sno IS NULL
					THEN 'N'
				ELSE 'Y'
			   END Status
		FROM tbl_application_functions a WITH (NOLOCK)
		INNER JOIN tbl_menus b WITH (NOLOCK) ON b.MenuId = a.MenuId
		INNER JOIN tbl_menus_role c WITH (NOLOCK) ON c.MenuId = b.MenuId
		LEFT JOIN tbl_application_functions_role d WITH (NOLOCK) ON d.FunctionId = a.FunctionId
			AND d.RoleId = c.RoleId
		WHERE c.RoleId = @RoleId
			AND ISNULL(a.Status, '') = 'A'
		ORDER BY MenuGroup
		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'af' --assign function
	BEGIN
		DELETE FROM tbl_application_functions_role 
		WHERE RoleId = @RoleId
		
		SELECT @StringSQL = 'INSERT INTO tbl_application_functions_role' +
		                    '(RoleId,FunctionId,ActionUser,ActionIP,ActionDate) '+
							'SELECT '+ CAST(@RoleId AS VARCHAR) +',FunctionId,''' + ISNULL(@ActionUser, '') + ''',''' + ISNULL(@ActionIP, '') + ''',GETDATE() FROM tbl_application_functions WITH (NOLOCK) WHERE FunctionId IN (' + @Functions + ')';
		PRINT(@StringSQL)
		EXEC(@StringSQL)

		SELECT 0 Code
			  ,'Function assigned successfully' Message
		RETURN;
	END
END
