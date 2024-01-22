USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_host_management]    Script Date: 10/10/2023 04:10:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[sproc_host_management] @Flag VARCHAR(10)
,@AgentId VARCHAR(20) = NULL
,@HostId VARCHAR(20) = NULL
,@HostName VARCHAR(256) = NULL
,@Position VARCHAR(200) = NULL
,@Rank VARCHAR(10) = NULL
,@DOB VARCHAR(15) = NULL
,@ConstellationGroup VARCHAR(10) = NULL
,@Height VARCHAR(5) = NULL
,@BloodType VARCHAR(10) = NULL
,@PreviousOccupation VARCHAR(10) = NULL
,@LiquorStrength VARCHAR(10) = NULL
,@ActionUser VARCHAR(200) = NULL
,@ActionIP VARCHAR(50) = NULL
,@ActionPlatform VARCHAR(20) = NULL
,@Status CHAR(1) = NULL
,@WebsiteLink VARCHAR(MAX) = NULL
,@TiktokLink VARCHAR(MAX) = NULL
,@TwitterLink VARCHAR(MAX) = NULL
,@InstagramLink VARCHAR(MAX) = NULL
AS
DECLARE @Sno VARCHAR(10),
		@Sno2 VARCHAR(10),
		@TransactionName VARCHAR(200),
		@ErrorDesc VARCHAR(MAX),
		@RandomPassword VARCHAR(20);
BEGIN TRY
	IF ISNULL(@Flag, '') = 'rh' --register host
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_club_details a WITH (NOLOCK)
			WHERE a.AgentId = @AgentId
				AND ISNULL(a.Status, '') = 'A'
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid club details' Message;
			RETURN;
		END

		SET @TransactionName = 'Flag_rh';
		BEGIN TRANSACTION @TransactionName;

		INSERT INTO tbl_host_details
		(
			AgentId
		   ,HostName
		   ,Position
		   ,[Rank]
		   ,DOB
		   ,ConstellationGroup
		   ,Height
		   ,BloodType
		   ,PreviousOccupation
		   ,LiquorStrength
		   ,Status
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
		)
		VALUES
		(
			@AgentId
		   ,@HostName
		   ,@Position
		   ,@Rank
		   ,@DOB
		   ,@ConstellationGroup
		   ,@Height
		   ,@BloodType
		   ,@PreviousOccupation
		   ,@LiquorStrength
		   ,'A'
		   ,@ActionUser
		   ,@ActionIP
		   ,@ActionPlatform
		   ,GETDATE()
		)

		SELECT @Sno = SCOPE_IDENTITY();

		UPDATE tbl_host_details
		SET HostId = @Sno
		WHERE Sno = @Sno

		INSERT INTO tbl_website_details
		(
			AgentId
		   ,HostId
		   ,WebsiteLink
		   ,TiktokLink
		   ,TwitterLink
		   ,InstagramLink
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
		)
		VALUES
		(
			@AgentId
		   ,@Sno
		   ,@WebsiteLink
		   ,@TiktokLink
		   ,@TwitterLink
		   ,@InstagramLink
		   ,@ActionUser
		   ,@ActionIP
		   ,@ActionPlatform
		   ,GETDATE()
		)

		SELECT 0 Code,
			   'Host registred successfully' Message;
		
		COMMIT TRANSACTION @TransactionName;
		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'uh' --update host
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_host_details a WITH (NOLOCK)
			WHERE a.HostId = @HostId
				AND a.AgentId = @AgentId
				AND ISNULL(a.Status, '') = 'A'
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid host details' Message;
			RETURN;
		END

		SET @TransactionName = 'Flag_uh';

		BEGIN TRANSACTION @TransactionName;

		UPDATE tbl_host_details
		SET HostName = ISNULL(@HostName, HostName)
		   ,Position = ISNULL(@Position, Position)
		   ,[Rank] = ISNULL(@Rank, [Rank])
		   ,DOB = ISNULL(@DOB, DOB)
		   ,ConstellationGroup = ISNULL(@ConstellationGroup, ConstellationGroup)
		   ,Height = ISNULL(@Height, Height)
		   ,BloodType = ISNULL(@BloodType, BloodType)
		   ,PreviousOccupation = ISNULL(@PreviousOccupation, PreviousOccupation)
		   ,LiquorStrength = ISNULL(@LiquorStrength, LiquorStrength)
		   ,ActionUser = @ActionUser
		   ,ActionIP = @ActionIP
		   ,ActionPlatform = @ActionPlatform
		   ,ActionDate = GETDATE()
		WHERE AgentId = @AgentId
			AND HostId = @HostId
			AND ISNULL(Status, '') = 'Á'

		UPDATE tbl_website_details
		SET WebsiteLink = ISNULL(@WebsiteLink,WebsiteLink)
		   ,TiktokLink = ISNULL(@TiktokLink, TiktokLink)
		   ,TwitterLink = ISNULL(@TwitterLink, TwitterLink)
		   ,InstagramLink = ISNULL(@InstagramLink, InstagramLink)
		   ,ActionUser = @ActionUser
		   ,ActionIP = @ActionIP
		   ,ActionPlatform = @ActionPlatform
		   ,ActionDate = GETDATE()
		WHERE AgentId = @AgentId
			AND HostId = @HostId

		SELECT 0 Code,
			   'Host updated successfully' Message;

		COMMIT TRANSACTION @TransactionName;
		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'ghl' --get host list
	BEGIN

		SELECT AgentId
			  ,HostId
			  ,HostName
			  ,Position
			  ,[Rank] 
			  ,DOB AS Age			  
			  ,[Status]
			  ,ActionUser
			  ,ActionIP
			  ,ActionPlatform
			  ,ActionDate CreatedDate
			  ,ActionDate UpdatedDate
		FROM tbl_host_details a WITH (NOLOCK)
		WHERE a.AgentId = @AgentId
			AND ISNULL(a.[Status], '') NOT IN ('D', 'S')
		
		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'ghd' --get host details
	BEGIN
		SELECT a.AgentId
			  ,a.HostId
			  ,a.HostName
			  ,a.Position
			  ,a.Rank
			  ,a.DOB
			  ,a.ConstellationGroup
			  ,a.Height
			  ,a.BloodType
			  ,a.PreviousOccupation
			  ,a.LiquorStrength
			  ,a.Status
			  ,b.WebsiteLink
			  ,b.InstagramLink
			  ,b.TiktokLink
			  ,b.TwitterLink
		FROM tbl_host_details a WITH (NOLOCK)
		INNER JOIN tbl_website_details b WITH (NOLOCK) ON b.AgentId = a.AgentId AND b.HostId = @HostId
		WHERE a.AgentId = @AgentId
			AND a.HostId = @HostId
			AND ISNULL(a.[Status], '') NOT IN ('D', 'S')
	END
	ELSE IF ISNULL(@Flag, '') = 'uhs' --update host status
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_host_details a WITH (NOLOCK)
			WHERE a.AgentId = @AgentId
				AND a.HostId = @HostId
				AND ISNULL(a.[Status], '') NOT IN ('D', 'S')
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid host details' Message;
			RETURN;
		END

		IF ISNULL(@Status, '') = 'A'
		BEGIN
			IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_host_details a WITH (NOLOCK)
				WHERE a.AgentId = @AgentId
					AND a.HostId = @HostId
					AND ISNULL(a.[Status], '') = 'I'
			)
			BEGIN
				SELECT 1 Code,
					   'Invalid host status' Message;
				RETURN;
			END

			UPDATE tbl_host_details 
			SET Status = 'A',
				ActionUser = @ActionUser,
				ActionIP = @ActionIP,
				ActionPlatform = @ActionPlatform,
				ActionDate = GETDATE()
			WHERE AgentId = @AgentId
					AND HostId = @HostId
					AND ISNULL([Status], '') = 'I'
		END
		ELSE IF ISNULL(@Status, '') = 'I'
		BEGIN
			IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_host_details a WITH (NOLOCK)
				WHERE a.AgentId = @AgentId
					AND a.HostId = @HostId
					AND ISNULL(a.[Status], '') = 'A'
			)
			BEGIN
				SELECT 1 Code,
					   'Invalid host status' Message;
				RETURN;
			END

			UPDATE tbl_host_details 
			SET Status = 'I',
				ActionUser = @ActionUser,
				ActionIP = @ActionIP,
				ActionPlatform = @ActionPlatform,
				ActionDate = GETDATE()
			WHERE AgentId = @AgentId
					AND HostId = @HostId
					AND ISNULL([Status], '') = 'A'
		END
		ELSE
		BEGIN
			SELECT 1 Code,
				   'Invalid status' Message;
			RETURN;
		END

		SELECT 0 Code,
			   'Host status updated successfully' Message;
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
		,'sproc_host_management(Flag: ' + ISNULL(@Flag, '') + ')'
		,'SQL'
		,'SQL'
		,'sproc_host_management'
		,GETDATE()
	)

	SELECT 1 Code,
		   'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message
	RETURN;
END CATCH
GO


