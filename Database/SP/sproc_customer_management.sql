USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_customer_management]    Script Date: 11/20/2023 10:19:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_customer_management] @Flag VARCHAR(10)
	,@AgentId VARCHAR(10) = NULL
	,@MobileNumber VARCHAR(15) = NULL
	,@NickName VARCHAR(200) = NULL
	,@FirstName VARCHAR(75) = NULL
	,@LastName VARCHAR(75) = NULL
	,@DOB VARCHAR(15) = NULL
	,@EmailAddress VARCHAR(256) = NULL
	,@Gender VARCHAR(10) = NULL
	,@PreferredLocation VARCHAR(10) = NULL
	,@PostalCode VARCHAR(75) = NULL
	,@Prefecture VARCHAR(10) = NULL
	,@City VARCHAR(75) = NULL
	,@Street VARCHAR(75) = NULL
	,@ResidenceNumber VARCHAR(75) = NULL
	,@Status CHAR(1) = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
AS
DECLARE @Sno VARCHAR(10)
	,@Sno2 VARCHAR(10)
	,@TransactionName VARCHAR(200)
	,@ErrorDesc VARCHAR(MAX)
	,@RandomPassword VARCHAR(20);

BEGIN TRY
	IF ISNULL(@Flag, '') = 'icd' --insert customer details
	BEGIN
		IF EXISTS (
				SELECT 'X'
				FROM tbl_customer a WITH (NOLOCK)
				INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') = 'A'
				WHERE a.MobileNumber = @MobileNumber
				)
		BEGIN
			SELECT 1 Code
				,'Duplicate mobile number' Message;

			RETURN;
		END;

		SET @TransactionName = 'Function_icd';

		BEGIN TRANSACTION @TransactionName;

		INSERT INTO tbl_customer (
			NickName
			,FirstName
			,LastName
			,MobileNumber
			,DOB
			,EmailAddress
			,Gender
			,PreferredLocation
			,PostalCode
			,Prefecture
			,City
			,Street
			,ResidenceNumber
			,ActionUser
			,ActionIP
			,ActionPlatform
			,ActionDate
			)
		VALUES (
			@NickName
			,@FirstName
			,@LastName
			,@MobileNumber
			,@DOB
			,@EmailAddress
			,@Gender
			,@PreferredLocation
			,@PostalCode
			,@Prefecture
			,@City
			,@Street
			,@ResidenceNumber
			,@ActionUser
			,@ActionIP
			,@ActionPlatform
			,GETDATE()
			);

		SELECT @Sno = SCOPE_IDENTITY()
			,@RandomPassword = dbo.func_generate_random_no(8);

		UPDATE tbl_customer
		SET AgentId = @Sno
		WHERE Sno = @Sno;

		INSERT INTO tbl_users (
			RoleId
			,AgentId
			,LoginId
			,Password
			,STATUS
			,IsPrimary
			,ActionUser
			,ActionIP
			,ActionPlatform
			,ActionDate
			)
		VALUES (
			3
			,@Sno
			,@MobileNumber
			,PWDENCRYPT(@RandomPassword)
			,'A'
			,'Y'
			,@ActionUser
			,@ActionIP
			,@ActionPlatform
			,GETDATE()
			);

		SELECT @Sno2 = SCOPE_IDENTITY();

		UPDATE tbl_users
		SET UserId = @Sno2
		WHERE Sno = @Sno2;

		SELECT 0 Code
			,'User registred successfully' Message;

		COMMIT TRANSACTION @TransactionName;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'ucd' --update customer details
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_customer a WITH (NOLOCK)
				INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') = 'A'
				WHERE b.AgentId = @AgentId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid customer details' Message;

			RETURN;
		END;

		UPDATE tbl_customer
		SET NickName = ISNULL(@NickName, NickName)
			,FirstName = ISNULL(@FirstName, FirstName)
			,LastName = ISNULL(@LastName, LastName)
			,MobileNumber = ISNULL(@MobileNumber, MobileNumber)
			,DOB = ISNULL(@DOB, DOB)
			,EmailAddress = ISNULL(@EmailAddress, EmailAddress)
			,Gender = ISNULL(@Gender, Gender)
			,PreferredLocation = ISNULL(@PreferredLocation, PreferredLocation)
			,PostalCode = ISNULL(@PostalCode, PostalCode)
			,Prefecture = ISNULL(@Prefecture, Prefecture)
			,City = ISNULL(@City, City)
			,Street = ISNULL(@Street, Street)
			,ResidenceNumber = ISNULL(@ResidenceNumber, ResidenceNumber)
			,ActionUser = @ActionUser
			,ActionIP = @ActionIP
			,ActionPlatform = @ActionPlatform
			,ActionDate = GETDATE()
		WHERE AgentId = @AgentId;

		SELECT 0 Code
			,'Customer details updated successfully' Message;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'gcd' --get customer details
	BEGIN
		SELECT a.NickName
			,a.FirstName
			,a.LastName
			,a.MobileNumber
			,a.DOB
			,a.EmailAddress
			,a.Gender
			,a.PreferredLocation
			,a.PostalCode
			,a.Prefecture
			,a.City
			,a.Street
			,a.ResidenceNumber
		FROM tbl_customer a WITH (NOLOCK)
		INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
			AND ISNULL(b.STATUS, '') NOT IN (
				'D'
				,'S'
				)
		WHERE a.AgentId = @AgentId;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'ucs' --update customer status
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_customer a WITH (NOLOCK)
				INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') NOT IN (
						'D'
						,'S'
						)
				WHERE a.AgentId = @AgentId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid customer details' Message;

			RETURN;
		END;

		IF ISNULL(@Status, '') NOT IN (
				'A'
				,'I'
				)
		BEGIN
			SELECT 1 Code
				,'Invalid status' Message;

			RETURN;
		END;

		IF ISNULL(@Status, '') IN ('A')
		BEGIN
			IF NOT EXISTS (
					SELECT 'X'
					FROM tbl_customer a WITH (NOLOCK)
					INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
						AND ISNULL(b.STATUS, '') IN ('I')
					WHERE a.AgentId = @AgentId
					)
			BEGIN
				SELECT 1 Code
					,'Invalid customer details' Message;

				RETURN;
			END;

			UPDATE tbl_users
			SET STATUS = 'A'
				,ActionUser = @ActionUser
				,ActionIP = @ActionIP
				,ActionDate = GETDATE()
				,ActionPlatform = @ActionPlatform
			WHERE AgentId = @AgentId;
		END;
		ELSE
		BEGIN
			IF NOT EXISTS (
					SELECT 'X'
					FROM tbl_customer a WITH (NOLOCK)
					INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
						AND ISNULL(b.STATUS, '') IN ('A')
					WHERE a.AgentId = @AgentId
					)
			BEGIN
				SELECT 1 Code
					,'Invalid customer details' Message;

				RETURN;
			END;

			UPDATE tbl_users
			SET STATUS = 'I'
				,ActionUser = @ActionUser
				,ActionIP = @ActionIP
				,ActionDate = GETDATE()
				,ActionPlatform = @ActionPlatform
			WHERE AgentId = @AgentId;
		END;

		SELECT 0 COde
			,'Customer status updated successfully' Message;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'rcp' --reset customer password
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_customer a WITH (NOLOCK)
				INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
					AND ISNULL(b.STATUS, '') IN ('A')
				WHERE a.AgentId = @AgentId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid customer details' Message;

			RETURN;
		END;

		SELECT @RandomPassword = dbo.func_generate_random_no(8);

		UPDATE tbl_users
		SET Password = PWDENCRYPT(@RandomPassword)
			,ActionUser = @ActionUser
			,ActionIP = @ActionIP
			,ActionDate = GETDATE()
			,ActionPlatform = @ActionPlatform
		WHERE AgentId = @AgentId;

		SELECT 0 Code
			,'Customer password reset successfully' Message;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'gcl' --get customer list
	BEGIN
		SELECT a.NickName
			,ISNULL(a.FirstName, '') + ' ' + ISNULL(a.LastName, '') FullName
			,b.AgentId
			,a.MobileNumber
			,a.EmailAddress
			,dbo.CalculateAge(CAST(a.DOB AS DATE)) Age
			,b.STATUS
			,a.ProfileImage
			,'' Referer
			,'Customer' Type
			,ISNULL(a.Street + ', ', '') + ISNULL(a.City, '') Location
			,FORMAT(a.ActionDate, 'MMM dd, yyyy HH:mm:ss') AS CreatedDate
			,FORMAT(a.ActionDate, 'MMM dd, yyyy HH:mm:ss') AS UpdatedDate
		FROM tbl_customer a WITH (NOLOCK)
		INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
			AND b.STATUS NOT IN ('D');

		RETURN;
	END;
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION @TransactionName;

	SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

	INSERT INTO tbl_error_log (
		ErrorDesc
		,ErrorScript
		,QueryString
		,ErrorCategory
		,ErrorSource
		,ActionDate
		)
	VALUES (
		@ErrorDesc
		,'sproc_customer_management(Flag: ' + ISNULL(@Flag, '') + ')'
		,'SQL'
		,'SQL'
		,'sproc_customer_management'
		,GETDATE()
		);

	SELECT 1 Code
		,'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;

	RETURN;
END CATCH;
