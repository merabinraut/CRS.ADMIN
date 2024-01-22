USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_login_management]    Script Date: 18/12/2023 11:25:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER PROC [dbo].[sproc_admin_login_management] @flag VARCHAR(10)
	,@Username VARCHAR(200) = NULL
	,@Password VARCHAR(200) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@Session VARCHAR(200) = NULL
AS
DECLARE @MaxFailedLoginAttempt VARCHAR(1) = 5;

BEGIN
	IF ISNULL(@flag, '') = 'Login'
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_admin a WITH (NOLOCK)
				WHERE a.Username = @Username
					AND ISNULL(a.STATUS, '') = 'A'
				)
		BEGIN
	
			SELECT 1 Code
				,'User is inactive' Message

			RETURN;
		END

		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_admin a WITH (NOLOCK)
				WHERE a.Username = @Username
					AND PWDCOMPARE(@Password, Password) = 1
				)
		BEGIN

			IF EXISTS (
					SELECT FailedLoginAttempt
					FROM tbl_admin a WITH (NOLOCK)
					WHERE a.Username = @Username
						AND ISNULL(a.FailedLoginAttempt, 0) = @MaxFailedLoginAttempt
					)
			BEGIN
	
				UPDATE tbl_admin
				SET STATUS = 'B'
					,FailedLoginAttempt = 0
					,Session = NULL
					,ActionIP = @ActionIP
				WHERE Username = @Username

				SELECT 1 Code
					,'Invalid credentials. User is blocked' Message

				RETURN;
			END
		END

		IF EXISTS (
				SELECT 'X'
				FROM tbl_admin a WITH (NOLOCK)
				WHERE a.Username = @Username
					AND PWDCOMPARE(@Password, Password) = 1
					AND ISNULL(a.STATUS, '') = 'A'
				)
		BEGIN
				SELECT @Session = NEWID();

			UPDATE tbl_admin
			SET Session = @Session
				,FailedLoginAttempt = 0
				,LastLoginDate = GETDATE()
				,ActionIP = @ActionIP
			WHERE Username = @Username
				AND PWDCOMPARE(@Password, Password) = 1
				AND ISNULL(STATUS, '') = 'A'

			SELECT 0 Code
				,'Success' Message
				,a.RoleId
				,a.Id AS UserId
				,b.RoleName
				,a.Username
				,isnull(a.FullName, a.Username) AS FullName
				,a.ProfileImage
				,@Session AS Session
				,a.IsPasswordForceful
				,a.LastPasswordChangedDate
			FROM tbl_admin a WITH (NOLOCK)
			INNER JOIN tbl_roles b WITH (NOLOCK) ON b.Id = a.RoleId
				--AND b.RoleType  1
			WHERE a.Username = @Username
				AND PWDCOMPARE(@Password, Password) = 1
				AND ISNULL(a.STATUS, '') = 'A'

			RETURN;
		END
		ELSE IF (
				(
					SELECT FailedLoginAttempt
					FROM tbl_admin a WITH (NOLOCK)
					WHERE a.Username = @Username
					) = (@MaxFailedLoginAttempt - 1)
				)
		BEGIN
		
			UPDATE tbl_admin
			SET FailedLoginAttempt = ISNULL(FailedLoginAttempt, 0) + 1
				,ActionIP = @ActionIP
			WHERE Username = @Username

			SELECT 1 Code
				,'Invalid credentials! <br/> Last attempt remaning' Message

			RETURN;
		END
		ELSE
		BEGIN
	
			UPDATE tbl_admin
			SET FailedLoginAttempt = ISNULL(FailedLoginAttempt, 0) + 1
				,ActionIP = @ActionIP
			WHERE Username = @Username

			SELECT 1 Code
				,'Invalid credentials!' Message

			RETURN;
		END
	END
END
GO


