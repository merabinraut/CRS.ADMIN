USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_club_login_management_Api]    Script Date: 6/7/2024 3:05:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_club_login_management_Api] @LoginId VARCHAR(200) = NULL
	,@Password VARCHAR(16) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(10) = NULL
	,@AgentId BIGINT = NULL
AS
DECLARE @Sno BIGINT;

BEGIN
	SET NOCOUNT ON;

	DECLARE @MaxFailedLoginAttempt INT = 5
		,@Session VARCHAR(500);

	-- Check if the user exists and is active
	IF EXISTS (
			SELECT 'X'
			FROM dbo.tbl_users u WITH (NOLOCK)
			INNER JOIN dbo.tbl_club_details cd WITH (NOLOCK) ON u.AgentId = cd.AgentId
			INNER JOIN dbo.tbl_roles r WITH (NOLOCK) ON u.RoleId = r.Id
			WHERE u.LoginId = @LoginId
				AND ISNULL(u.STATUS, '') = 'A'
				AND ISNULL(cd.STATUS, '') = 'A'
				AND r.RoleName = 'Club'
			)
	BEGIN
		-- Verify login credentials
		SELECT @AgentId = u.AgentId
		FROM dbo.tbl_users u WITH (NOLOCK)
		INNER JOIN dbo.tbl_club_details cd WITH (NOLOCK) ON u.AgentId = cd.AgentId
		INNER JOIN dbo.tbl_roles r WITH (NOLOCK) ON u.RoleId = r.Id
		WHERE u.LoginId = @LoginId
			AND cd.AgentId = u.AgentId
			AND ISNULL(u.STATUS, '') = 'A'
			AND ISNULL(cd.STATUS, '') = 'A'
			AND r.RoleName = 'Club'
			AND PWDCOMPARE(@Password, u.Password) = 1;

		-- Check if login credentials are valid
		IF @AgentId IS NOT NULL
		BEGIN
			-- Generate a new session ID
			SELECT @Session = NEWID();

			-- Update user session and reset failed login attempts
			UPDATE dbo.tbl_users
			SET Session = @Session
				,FailedLoginAttempt = 0
				,ActionUser = @LoginId
				,ActionIP = @ActionIP
				,ActionPlatform = @ActionPlatform
				,ActionDate = GETDATE()
			WHERE LoginId = @LoginId
				AND AgentId = @AgentId
				AND ISNULL([Status], '') = 'A';

			-- Return success message along with user information
			SELECT 0 AS Code
				,'Success' AS Message
				,cd.AgentId
				,u.UserId
				,cd.ClubName1 AS ClubName
				,cd.ClubName2 AS ClubNameJp
				,cd.Email AS EmailAddress
				,cd.Logo AS profileImage
				,dbo.fn_IsClubProfileComplete(cd.AgentId) AS IsComplete
				,Isnull(IsPasswordForceful, 'N') AS IsPasswordForceful
				,u.LoginId AS Username
				,0 AS RoleId
			FROM dbo.tbl_club_details cd WITH (NOLOCK)
			INNER JOIN dbo.tbl_users u WITH (NOLOCK) ON u.AgentId = cd.AgentId
			INNER JOIN dbo.tbl_roles r WITH (NOLOCK) ON u.RoleId = r.Id
			WHERE u.LoginId = @LoginId
				AND cd.AgentId = @AgentId;

			IF NOT EXISTS (
					SELECT 1
					FROM dbo.tbl_agent_balance a WITH (NOLOCK)
					WHERE a.AgentId = @AgentId
						AND ISNULL(a.STATUS, '') = 'A'
						AND ISNULL(a.Amount, 0) > 100000
					)
			BEGIN
				INSERT INTO dbo.tbl_club_notification (
					ToAgentId
					,NotificationType
					,NotificationSubject
					,NotificationBody
					,NotificationStatus
					,NotificationReadStatus
					,CreatedBy
					,CreatedDate
					)
				VALUES (
					@AgentId
					,6 --Hoslog Balance Is Less Than 100000
					,'Balance'
					,'Your current balance is less then 100000!'
					,'A'
					,'P'
					,0
					,GETDATE()
					)

				SET @Sno = SCOPE_IDENTITY();

				UPDATE dbo.tbl_club_notification
				SET notificationId = @Sno
				WHERE Sno = @Sno;
			END
		END
		ELSE
		BEGIN
			-- Increment failed login attempts and lock the account if necessary
			UPDATE dbo.tbl_users
			SET FailedLoginAttempt = ISNULL(FailedLoginAttempt, 0) + 1
				,ActionUser = @LoginId
				,ActionIP = @ActionIP
				,ActionPlatform = @ActionPlatform
				,ActionDate = GETDATE()
			WHERE LoginId = @LoginId
				AND AgentId = @AgentId;

			-- Check if the account is locked
			IF (
					SELECT FailedLoginAttempt
					FROM dbo.tbl_users
					WHERE LoginId = @LoginId
						AND AgentId = @AgentId
					) = (@MaxFailedLoginAttempt - 1)
			BEGIN
				-- Lock the account
				UPDATE dbo.tbl_users
				SET STATUS = 'B'
					,Session = NULL
				WHERE LoginId = @LoginId
					AND AgentId = @AgentId;

				-- Return error message for account lockout
				SELECT 1 AS Code
					,'Account_is_locked_due_to_multiple_failed_login_attempts.' AS Message;
			END
			ELSE
			BEGIN
				-- Return error message for invalid credentials
				SELECT 1 AS Code
					,'Invalid_username_or_password' AS Message;
			END
		END
	END
	ELSE
	BEGIN
		-- Return error message for invalid username or password
		SELECT 1 AS Code
			,'Invalid_username_or_password' AS Message;
	END

	SET NOCOUNT OFF;
END
GO


