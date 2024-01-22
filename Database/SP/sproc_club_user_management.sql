ALTER PROC sproc_club_user_management @Flag VARCHAR(10)
,@AgentId VARCHAR(20) = NULL
,@UserId VARCHAR(20) = NULL
,@LoginId VARCHAR(200) = NULL
,@ActionUser VARCHAR(200) = NULL
,@ActionIP VARCHAR(50) = NULL
,@ActionPlatform VARCHAR(20) = NULL
,@Status CHAR(1) = NULL
AS
DECLARE @Sno VARCHAR(10),
		@TransactionName VARCHAR(200),
		@ErrorDesc VARCHAR(MAX),
		@RandomPassword VARCHAR(20);
BEGIN TRY
	IF ISNULL(@Flag, '') = 'gcul' --get club user list
	BEGIN
		SELECT  a.AgentId
			   ,b.UserId
			   ,b.LoginId
			   ,b.Status
			   ,IsPrimary
		FROM tbl_club_details a WITH (NOLOCK)
		INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND ISNULL(b.Status, '') NOT IN ('D', '')
		WHERE b.AgentId = @AgentId
			AND ISNULL(a.Status, '') NOT IN ('D', '')
	END
	ELSE IF ISNULL(@Flag, '') = 'gcud' --get club user details
	BEGIN
		SELECT b.AgentId
			  ,b.UserId
			  ,b.LoginId
			  ,b.Password
			  ,b.Status
			  ,b.IsPrimary
		FROM tbl_club_details a WITH (NOLOCK)
		INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND ISNULL(b.Status, '') NOT IN ('D', '')
		WHERE a.AgentId = @AgentId
			AND b.UserId = @UserId
			AND ISNULL(a.Status, '') NOT IN ('D', '');

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'rcu' --register club user
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_club_details a WITH (NOLOCK)
			WHERE a.AgentId = @AgentId
				AND ISNULL(a.Status, '') IN ('A')
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid details' Message;
			RETURN;
		END

		IF EXISTS 
		(
			SELECT 'X'
			FROM tbl_club_details a WITH (NOLOCK)
			INNER JOIn tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND ISNULL(b.Status, '') NOT IN ('D')
			WHERE b.AgentId = @AgentId
				AND b.LoginId = @LoginId
		)
		BEGIN
			SELECT 1 Code,
				   'Dublicate username' Message;
			RETURN;
		END		

		INSERT INTO tbl_users
		(
			 AgentId
			,LoginId
			,Status
			,IsPrimary
			,ActionUser
			,ActionIP
			,ActionPlatform
			,ActionDate
		)

		VALUES
		(
			 @AgentId
			,@LoginId
			,'A'
			,'N'
			,@ActionUser
			,@ActionIP
			,@ActionPlatform
			,GETDATE()
		)

		SELECT @Sno = SCOPE_IDENTITY()
		      ,@randomPassword = dbo.func_generate_random_no(8);

		UPDATE tbl_users
		SET Password = PWDENCRYPT(@RandomPassword),
			UserId = @Sno
		WHERE Sno = @Sno
			AND AgentId = @AgentId;

		SELECT 0 Code,
			   'Club user registred successfully' Message;

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'rcup' --reset club user password
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_club_details a WITH (NOLOCK)
			INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND ISNULL(a.Status, '') IN ('A')
			WHERE a.AgentId = @AgentId
				AND b.UserId = @UserId				
				AND ISNULL(b.Status, '') IN ('A')
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid user detail' Message;
			RETURN;
		END

		SET @RandomPassword = dbo.func_generate_random_no(8);

		UPDATE tbl_users
		SET Password = PWDENCRYPT(@RandomPassword),
		    ActionUser = @ActionUser,
			ActionIP = @ActionIP,
			ActionPlatform = @ActionPlatform,
			ActionDate = GETDATE()
		WHERE AgentId = @AgentId
			AND UserId = @UserId;

		SELECT 0 Code,
			   'Club user password reset successfully' Message;

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'ucus' --update club user status
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_club_details a WITH (NOLOCK)
			INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND ISNULL(a.Status, '') IN ('A')
			WHERE a.AgentId = @AgentId
				AND b.UserId = @UserId				
				AND ISNULL(b.Status, '') IN ('A')
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid user detail' Message;
			RETURN;
		END

		IF ISNULL(@Status, '') NOT IN ('A', 'I')
		BEGIN
			SELECT 1 Code,
				   'Invalid status' Message;
			RETURN;
		END

		UPDATE tbl_users
		SET Status = @Status,
			ActionUser = @ActionUser,
			ActionIP = @ActionIP,
			ActionPlatform = @ActionPlatform,
			ActionDate = GETDATE()
		WHERE AgentId = @AgentId
			AND UserId = @UserId;

		SELECT 0 Code,
			   'Club user status updated successfully' Message;
		RETURN;
	END
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION @TransactionName;

	SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

	INSERT INTO tbl_error_log
	(
		 ErrorDesc
		,ErrorScript
		,QueryString
		,ErrorCategory
		,ErrorSource
		,ActionDate
	)
	VALUES
	(
		 @ErrorDesc
		,'sproc_club_user_management(Flag: ' + ISNULL(@Flag, '') + ')'
		,'SQL'
		,'SQL'
		,'sproc_club_user_management'
		,GETDATE()
	)

	SELECT 1 Code,
		   'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message
	RETURN;
END CATCH
GO