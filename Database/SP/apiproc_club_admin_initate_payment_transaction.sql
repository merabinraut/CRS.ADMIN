USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[apiproc_club_admin_initate_payment_transaction]    Script Date: 6/7/2024 4:48:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROC [dbo].[apiproc_club_admin_initate_payment_transaction]
@AgentId VARCHAR(10) NULL,
@UserId VARCHAR(10) NULL,
@ProcessId VARCHAR(100) NULL,
@PaymentMethod VARCHAR(10) NULL,
@Amount INT = NULL,
@ActionUser NVARCHAR(200) = NULL,
@ActionIP VARCHAR(50) = NULL,
@ActionPlatform NVARCHAR(20) = NULL
AS
DECLARE @Sno VARCHAR(10),
	    @ClubName NVARCHAR(512);
BEGIN
	IF NOT EXISTS
	(
		SELECT 1
		FROM dbo.tbl_club_details a WITH (NOLOCK)
		INNER JOIN dbo.tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
			AND b.RoleType = 4
			AND ISNULL(a.Status, '') = 'A'
			AND ISNULL(b.Status, '') = 'A'
		WHERE a.AgentId = @AgentId
			AND b.UserId = @UserId
	)
	BEGIN
		 SELECT 1 Code,
			    'Invalid_Club' Message;
		RETURN;
	END

	INSERT INTO dbo.tbl_club_admin_payment_transaction_detail
	(
		AgentId
	   ,UserId
	   ,ProcessId
	   ,PaymentMethod
	   ,Amount
	   ,Status
	   ,CreatedBy
	   ,CreatedDate
	   ,CreatedUTCDate
	   ,CreatedIP
	   ,CreatedPlatform
	)
	VALUES
	(
		@AgentId
	   ,@UserId
	   ,@ProcessId
	   ,@PaymentMethod
	   ,@Amount
	   ,'P'
	   ,@ActionUser
	   ,GETDATE()
	   ,GETUTCDATE()
	   ,@ActionIP
	   ,@ActionPlatform
	)

	SET @Sno = SCOPE_IDENTITY();

	UPDATE dbo.tbl_club_admin_payment_transaction_detail
	SET TxnId = @Sno
	WHERE Sno = @Sno;

	IF ISNULL(@PaymentMethod, '') = '3'
	BEGIN
		SELECT @ClubName = ISNULL(a.ClubName2, a.ClubName1)
		FROM dbo.tbl_club_details a WITH (NOLOCK)
		INNER JOIN dbo.tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
			AND b.RoleType = 4
			AND ISNULL(a.Status, '') = 'A'
			AND ISNULL(b.Status, '') = 'A'
		WHERE a.AgentId = @AgentId
			AND b.UserId = @UserId;

		INSERT INTO dbo.tbl_admin_notification
		(
			NotificationTo
		   ,NotificationType
		   ,NotificationSubject
		   ,NotificationBody
		   ,NotificationStatus
		   ,NotificationReadStatus
		   ,CreatedBy
		   ,CreatedDate
		)
		VALUES
		(
			0
		   ,1
		   ,'Club Point Request'
		   ,'Club ' + @ClubName + ' has request a point for amount ' + CAST(ISNULL(@Amount, 0) AS VARCHAR(200))
		   ,'A'
		   ,'A'
		   ,0
		   ,GETDATE()
		)
	END

	SELECT 0 Code,
		   'Club_Admin_Payment_Transaction_Initated_Successfully' Message,
		   @Sno AS Id;
	RETURN;
END
GO


