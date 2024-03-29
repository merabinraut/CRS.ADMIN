USE [db_a9fc4e_crs]
GO
`
/****** Object:  StoredProcedure [dbo].[sproc_profile_management]    Script Date: 10/7/2023 4:06:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sproc_profile_management] (
	@Flag CHAR(5) = NULL
	,@CurrentPassword VARCHAR(16) = NULL
	,@NewPassword VARCHAR(16) = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIp VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	,@Session VARCHAR(200) = NULL
	,@UserId VARCHAR(200) = NULL
	,@FullName VARCHAR(200) = NULL
	,@ProfileImage VARCHAR(200) = NULL
	)
AS
BEGIN
	SET NOCOUNT ON;

	--SHOW ADMIN PROFILE
	IF @Flag = 'sap'
	BEGIN
		SELECT EmailAddress
			,fullName
			,MobileNumber
			,userName
			,'Admin' roleName
			,'' RoleType
			,ProfileImage
			,ActionUser UpdatedBy
			,ActionDate UpdatedLocalDate
		FROM tbl_admin ta WITH (NOLOCK)
		WHERE ta.Id = @UserId

		RETURN;
	END

	--UPDATE ADMIN DETAILS
	IF @Flag = 'u'
	BEGIN
		UPDATE tbl_admin
		SET FullName = @FullName
			,ActionUser = @ActionUser
			,@ActionIp = @ActionIp
		WHERE Id = @UserId

		SELECT '0' code
			,'Admin detail updated successfully' message

		RETURN;
	END

	--UPLOAD ADMIN USER IMAGE
	IF @Flag = 'uimg'
	BEGIN
		UPDATE tbl_admin
		SET ProfileImage = @ProfileImage
		WHERE Id = @UserId

		SELECT '0' code
			,'Profile picture updated successfully' message

		RETURN;
	END

	--CHANGE ADMIN USER PASSWORD
	IF @Flag = 'cp'
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_admin ta WITH (NOLOCK)
				WHERE Username = @ActionUser
					AND PWDCOMPARE(@CurrentPassword, ta.Password) = 1
				)
		BEGIN
			SELECT '1' code
				,'Invalid user credentials' message

			RETURN;
		END
		ELSE
		BEGIN
			UPDATE tbl_admin
			SET Password = PWDENCRYPT(@NewPassword)
				,LastPasswordChangedDate = GETDATE()
				,Session = @Session
			WHERE Username = @ActionUser

			SELECT '0' code
				,'Password changed successfully' message

			RETURN;
		END
	END
END
