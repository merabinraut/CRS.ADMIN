USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_manage_club_to_admin_payment_transaction]    Script Date: 6/7/2024 5:31:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_admin_manage_club_to_admin_payment_transaction] @AgentId VARCHAR(10) NULL
	,@UserId VARCHAR(10) NULL
	,@TxnId VARCHAR(20) = NULL
	,@Status CHAR(1) = NULL
	,@ActionUser NVARCHAR(200) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@ActionPlatform NVARCHAR(20) = NULL
	,@ImageURL VARCHAR(512) = NULL
	,@AdminRemark NVARCHAR(300) = NULL
AS
DECLARE @ErrorDesc NVARCHAR(MAX)
	,@TransactionName VARCHAR(100);
DECLARE @Amount INT
	,@Remarks NVARCHAR(300)
	,@GeneratedTxnId NVARCHAR(100) = NULL
	,@Prefix VARCHAR(10) = NULL
	,@Lastnumber BIGINT = 0
	,@previousLastnumber BIGINT
	,@remainingLastnumber BIGINT;
DECLARE @GeneratedTxnId2 NVARCHAR(100) = NULL
	,@Prefix2 VARCHAR(10) = NULL
	,@Lastnumber2 BIGINT = 0
	,@previousLastnumber2 BIGINT
	,@remainingLastnumber2 BIGINT;
DECLARE @GeneratedTxnId3 NVARCHAR(100) = NULL
	,@Prefix3 VARCHAR(10) = NULL
	,@Lastnumber3 BIGINT = 0
	,@previousLastnumber3 BIGINT
	,@remainingLastnumber3 BIGINT;
DECLARE @Sno VARCHAR(10)
	,@Sno2 VARCHAR(10)
	,@Sno3 VARCHAR(10)
	,@Sno4 VARCHAR(10)
	,@Sno5 VARCHAR(10)
	,@Point INT = 0;
DECLARE @AdminPointRetriveAmount DECIMAL(18, 2)
	,@AdminPointRetriveRemark NVARCHAR(512)
	,@AdminPointRetriveSystemRemark NVARCHAR(512);

BEGIN TRY
	IF NOT EXISTS (
			SELECT 1
			FROM dbo.tbl_club_details a WITH (NOLOCK)
			INNER JOIN dbo.tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
				AND b.RoleType = 4
				AND ISNULL(a.STATUS, '') = 'A'
				AND ISNULL(b.STATUS, '') = 'A'
			WHERE a.AgentId = @AgentId
				AND b.UserId = @UserId
			)
	BEGIN
		SELECT 1 Code
			,'Invalid club' Message;

		RETURN;
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.tbl_club_admin_payment_transaction_detail a WITH (NOLOCK)
			WHERE a.TxnId = @TxnId
				AND a.AgentId = @AgentId
				AND a.UserId = @UserId
			)
	BEGIN
		SELECT 1 Code
			,'Invalid transaction detail' Message;

		RETURN;
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.tbl_club_admin_payment_transaction_detail a WITH (NOLOCK)
			WHERE a.TxnId = @TxnId
				AND a.AgentId = @AgentId
				AND a.UserId = @UserId
				AND a.STATUS = 'S'
			)
	BEGIN
		SELECT 0 Code
			,'Transaction already success' Message;

		RETURN;
	END

	IF @Status IN (
			'S'
			,'R'
			)
	BEGIN
		SET @TransactionName = 'sproc_admin_manage_club_to_admin_payment_transaction';

		BEGIN TRANSACTION @TransactionName

		UPDATE dbo.tbl_club_admin_payment_transaction_detail
		SET STATUS = @Status
			,ImageURL = @ImageURL
			,Remarks = @AdminRemark
			,UpdatedBy = @ActionUser
			,UpdatedDate = GETDATE()
			,UpdatedUTCDate = GETUTCDATE()
			,UpdatedIP = @ActionIP
			,UpdatedPlatform = @ActionPlatform
		WHERE TxnId = @TxnId
			AND AgentId = @AgentId
			AND UserId = @UserId
			AND STATUS <> 'S';

		IF @Status = 'S'
		BEGIN
			SELECT @Point = a.Amount
				,@Remarks = N'Admin payment of TxnId#' + ISNULL(a.TxnId, '-') + ' and PaymentTxnId#' + ISNULL(a.PaymentTxnId, '-')
			FROM dbo.tbl_club_admin_payment_transaction_detail a WITH (NOLOCK)
			WHERE a.TxnId = @TxnId
				AND a.AgentId = @AgentId
				AND a.UserId = @UserId
				AND a.STATUS = 'S';

			PRINT (1)

			------------ Add txn balance as a point to the system initiate------------
			SELECT @Lastnumber = LastNumber
				,@prefix = Prefix
			FROM dbo.tbl_billing_type WITH (NOLOCK)
			WHERE BillType = 'AdminTransferPoint'

			SET @previousLastnumber = ISNULL((
						SELECT TOP 1 SUBSTRING(TransactionId, LEN(@prefix) + 1, LEN(TransactionId) - LEN(@prefix)) AS num_value
						FROM tbl_AdminTransferPoint WITH (NOLOCK)
						ORDER BY id DESC
						), @Lastnumber)

			IF EXISTS (
					SELECT 'x'
					FROM tbl_AdminTransferPoint WITH (NOLOCK)
					WHERE TransactionId = (
							SELECT CONCAT (
									@prefix
									,CAST(@Lastnumber AS VARCHAR(MAX))
									)
							)
					)
			BEGIN
				PRINT (3)

				SET @remainingLastnumber = cast(@previousLastnumber AS BIGINT) - cast(@Lastnumber AS BIGINT)

				UPDATE dbo.tbl_billing_type
				SET LastNumber = CAST(@Lastnumber + @remainingLastnumber AS INT)
				WHERE BillType = 'AdminTransferPoint'

				SET @Lastnumber = @Lastnumber + @remainingLastnumber
				SET @GeneratedTxnId = (
						SELECT CONCAT (
								@prefix
								,CAST(@Lastnumber AS VARCHAR(MAX))
								)
						)
			END
			ELSE
			BEGIN
				SET @GeneratedTxnId = (
						SELECT CONCAT (
								@prefix
								,@Lastnumber
								)
						)

				UPDATE dbo.tbl_billing_type
				SET LastNumber = cast(@Lastnumber AS INT) + 1
				WHERE BillType = 'AdminTransferPoint'
			END

			INSERT INTO dbo.tbl_AdminTransferPoint (
				TransactionId
				,AgentId
				,Amount
				,Remark
				,TrancationType
				,IMAGE
				,STATUS
				,ActionDate
				,ActionUser
				)
			VALUES (
				@GeneratedTxnId
				,0
				,@Point
				,@Remarks
				,8
				,''
				,'A'
				,GETDATE()
				,0
				)

			SET @Sno = SCOPE_IDENTITY();

			-- CR system --  
			INSERT INTO dbo.tbl_PointLogs (
				TranscationId
				,UserId
				,RoleType
				,TransactionType
				,Point
				,TransactionDate
				,TransactionMode
				,ActionUser
				,Remark
				,SystemRemark
				)
			VALUES (
				@Sno
				,0
				,1
				,8
				,@Point
				,GETDATE()
				,'CR'
				,0
				,@Remarks
				,'Club to Admin transfer'
				)

			UPDATE dbo.tbl_System_Balance
			SET Amount = ISNULL(Amount, 0) + ISNULL(@Point, 0)
				,LastUpdate = GETDATE()
			WHERE Id = 1

			------------ Add txn balance as a point to the system complete------------
			------------ Point transfer to the club initate ------------
			IF EXISTS (
					SELECT 1
					FROM dbo.tbl_System_Balance a WITH (NOLOCK)
					WHERE Id = 1
						AND (
							a.Amount IS NULL
							OR a.Amount < ISNULL(@Point, 0)
							)
					)
			BEGIN
				ROLLBACK TRANSACTION @TransactionName;

				SELECT 1 AS Code
					,'Insufficient admin point' AS Message;

				RETURN;
			END

			SELECT @Lastnumber2 = LastNumber
				,@prefix2 = Prefix
			FROM dbo.tbl_billing_type WITH (NOLOCK)
			WHERE BillType = 'AdminTransferPoint'

			SET @previousLastnumber2 = ISNULL((
						SELECT TOP 1 SUBSTRING(TransactionId, LEN(@prefix2) + 1, LEN(TransactionId) - LEN(@prefix2)) AS num_value
						FROM tbl_AdminTransferPoint WITH (NOLOCK)
						ORDER BY id DESC
						), @Lastnumber2)

			IF EXISTS (
					SELECT 'x'
					FROM tbl_AdminTransferPoint WITH (NOLOCK)
					WHERE TransactionId = (
							SELECT CONCAT (
									@prefix2
									,@Lastnumber2
									)
							)
					)
			BEGIN
				SET @remainingLastnumber2 = cast(@previousLastnumber2 AS BIGINT) - cast(@Lastnumber2 AS BIGINT)

				UPDATE dbo.tbl_billing_type
				SET LastNumber = @Lastnumber2 + @remainingLastnumber2
				WHERE BillType = 'AdminTransferPoint'

				SET @Lastnumber2 = @Lastnumber2 + @remainingLastnumber2;
				SET @GeneratedTxnId2 = (
						SELECT CONCAT (
								@prefix2
								,@Lastnumber
								)
						)
			END
			ELSE
			BEGIN
				SET @GeneratedTxnId2 = (
						SELECT CONCAT (
								@prefix2
								,@Lastnumber2
								)
						)

				UPDATE dbo.tbl_billing_type
				SET LastNumber = cast(@Lastnumber2 AS INT) + 1
				WHERE BillType = 'AdminTransferPoint'
			END

			INSERT INTO dbo.tbl_AdminTransferPoint (
				TransactionId
				,AgentId
				,Amount
				,Remark
				,TrancationType
				,IMAGE
				,STATUS
				,ActionDate
				,ActionUser
				)
			VALUES (
				@GeneratedTxnId2
				,@AgentId
				,@Point
				,@Remarks
				,7
				,''
				,'A'
				,GETDATE()
				,0
				)

			SET @Sno2 = SCOPE_IDENTITY();

			-- DR system --
			INSERT INTO dbo.tbl_PointLogs (
				TranscationId
				,UserId
				,RoleType
				,TransactionType
				,Point
				,TransactionDate
				,TransactionMode
				,ActionUser
				,Remark
				,SystemRemark
				)
			VALUES (
				@Sno2
				,0
				,1
				,7
				,@Point
				,GETDATE()
				,'DR'
				,0
				,@Remarks
				,'Admin to Club transfer'
				)

			-- CR user --
			INSERT INTO dbo.tbl_PointLogs (
				TranscationId
				,UserId
				,RoleType
				,TransactionType
				,Point
				,TransactionDate
				,TransactionMode
				,ActionUser
				,Remark
				,SystemRemark
				)
			VALUES (
				@Sno
				,@AgentId
				,4
				,7
				,@Point
				,GETDATE()
				,'CR'
				,0
				,@Remarks
				,'Admin to club transfer'
				)

			IF EXISTS (
					SELECT 1
					FROM dbo.tbl_agent_balance a WITH (NOLOCK)
					WHERE a.AgentId = @AgentId
						AND a.RoleType = 4
					)
			BEGIN
				UPDATE dbo.tbl_agent_balance
				SET Amount = ISNULL(Amount, 0) + ISNULL(@Point, 0)
					,ActionUser = 0
					,ActionDate = GETDATE()
				WHERE AgentId = @AgentId
					AND RoleType = 4;
			END
			ELSE
			BEGIN
				INSERT INTO dbo.tbl_agent_balance (
					AgentId
					,Amount
					,creditLimit
					,CreditPointUse
					,STATUS
					,ActionUser
					,ActionDate
					,RoleType
					)
				VALUES (
					@AgentId
					,@Point
					,0
					,0
					,'A'
					,0
					,GETDATE()
					,4
					)
			END

			UPDATE dbo.tbl_System_Balance
			SET Amount = ISNULL(Amount, 0) - ISNULL(@Point, 0)
				,LastUpdate = GETDATE()
			WHERE Id = 1

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
				,7 -- Hoslog Points Has Been Added/Deducte
				,'Points Addition'
				,CAST(ISNULL(@Point, 0) AS VARCHAR(100)) + ' points has been added via system'
				,'A'
				,'P'
				,0
				,GETDATE()
				)

			SET @Sno4 = SCOPE_IDENTITY();

			UPDATE tbl_club_notification
			SET notificationId = @Sno4
			WHERE Sno = @Sno4;

			------------ Point transfer to the club complete ------------	
			------------ Point retrive in case of club used credit amount -----------------
			IF EXISTS (
					SELECT 1
					FROM tbl_agent_balance a WITH (NOLOCK)
					WHERE a.AgentId = @AgentId
						AND a.RoleType = 4
						AND ISNULL(a.STATUS, '') = 'A'
						AND ISNULL(CreditPointUse, 0) > 0
					)
			BEGIN
				IF EXISTS (
						SELECT 1
						FROM tbl_agent_balance a WITH (NOLOCK)
						WHERE a.AgentId = @AgentId
							AND a.RoleType = 4
							AND ISNULL(a.STATUS, '') = 'A'
							AND ISNULL(a.CreditPointUse, 0) > 0
							AND (
								ISNULL(a.CreditPointUse, 0) = ISNULL(a.Amount, 0)
								OR ISNULL(a.CreditPointUse, 0) > ISNULL(a.Amount, 0)
								)
						)
				BEGIN
					SELECT @AdminPointRetriveAmount = ISNULL(a.Amount, 0)
					FROM tbl_agent_balance a WITH (NOLOCK)
					WHERE a.AgentId = @AgentId
						AND a.RoleType = 4
						AND ISNULL(a.STATUS, '') = 'A'
						AND ISNULL(CreditPointUse, 0) > 0;
				END
				ELSE
				BEGIN
					SELECT @AdminPointRetriveAmount = ISNULL(a.CreditPointUse, 0)
					FROM tbl_agent_balance a WITH (NOLOCK)
					WHERE a.AgentId = @AgentId
						AND a.RoleType = 4
						AND ISNULL(a.STATUS, '') = 'A'
						AND ISNULL(CreditPointUse, 0) > 0;
				END

				SELECT @AdminPointRetriveRemark = 'Point retrive via system from club of ' + CAST(ISNULL(@AdminPointRetriveAmount, '-') AS VARCHAR(MAX))
					,@AdminPointRetriveSystemRemark = 'Point retrive via system from club of amount ' + CAST(ISNULL(@AdminPointRetriveAmount, 0) AS VARCHAR(MAX)) + ' for transaction id ' + CAST(ISNULL(@Sno, '-') AS VARCHAR(MAX));

				SELECT @Lastnumber3 = LastNumber
					,@prefix3 = Prefix
				FROM dbo.tbl_billing_type WITH (NOLOCK)
				WHERE BillType = 'AdminTransferPoint'

				SET @previousLastnumber3 = ISNULL((
							SELECT TOP 1 SUBSTRING(TransactionId, LEN(@prefix3) + 1, LEN(TransactionId) - LEN(@prefix3)) AS num_value
							FROM tbl_AdminTransferPoint WITH (NOLOCK)
							ORDER BY id DESC
							), @Lastnumber3)

				IF EXISTS (
						SELECT 'x'
						FROM tbl_AdminTransferPoint WITH (NOLOCK)
						WHERE TransactionId = (
								SELECT CONCAT (
										@prefix3
										,CAST(@Lastnumber3 AS VARCHAR(MAX))
										)
								)
						)
				BEGIN
					SET @remainingLastnumber3 = cast(@previousLastnumber3 AS BIGINT) - cast(@Lastnumber3 AS BIGINT)

					UPDATE dbo.tbl_billing_type
					SET LastNumber = CAST(@Lastnumber3 + @remainingLastnumber3 AS INT)
					WHERE BillType = 'AdminTransferPoint'

					SET @Lastnumber3 = @Lastnumber3 + @remainingLastnumber3
					SET @GeneratedTxnId3 = (
							SELECT CONCAT (
									@prefix3
									,CAST(@Lastnumber3 AS VARCHAR(MAX))
									)
							)
				END
				ELSE
				BEGIN
					SET @GeneratedTxnId3 = (
							SELECT CONCAT (
									@prefix3
									,@Lastnumber3
									)
							)

					UPDATE dbo.tbl_billing_type
					SET LastNumber = cast(@Lastnumber3 AS INT) + 1
					WHERE BillType = 'AdminTransferPoint'
				END

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
					,7 -- Hoslog Points Has Been Added/Deducte
					,'Points Addition'
					,CAST(ISNULL(@AdminPointRetriveAmount, 0) AS VARCHAR(100)) + ' points has been deducted via system for credit settlement'
					,'A'
					,'P'
					,0
					,GETDATE()
					)

				SET @Sno5 = SCOPE_IDENTITY();

				UPDATE tbl_club_notification
				SET notificationId = @Sno5
				WHERE Sno = @Sno5;

				INSERT INTO [dbo].[tbl_AdminTransferPoint] (
					TransactionId
					,AgentId
					,Amount
					,Remark
					,TrancationType
					,STATUS
					,
					--Image,
					ActionDate
					,ActionUser
					)
				VALUES (
					@GeneratedTxnId3
					,@AgentId
					,@AdminPointRetriveAmount
					,@AdminPointRetriveRemark
					,'3'
					,'A'
					,
					--@Image,
					GETDATE()
					,0
					)

				SET @Sno3 = SCOPE_IDENTITY();

				--------DR (AGENT)-------------
				INSERT INTO [dbo].[tbl_PointLogs] (
					TranscationId
					,UserId
					,RoleType
					,TransactionType
					,Point
					,TransactionDate
					,TransactionMode
					,ActionUser
					,Remark
					,SystemRemark
					)
				VALUES (
					@Sno3
					,@AgentId
					,4
					,'3'
					,@AdminPointRetriveAmount
					,GETDATE()
					,'DR'
					,0
					,@AdminPointRetriveRemark
					,@AdminPointRetriveSystemRemark
					)

				--------CR (ADMIN)-------------
				INSERT INTO [dbo].[tbl_PointLogs] (
					TranscationId
					,UserId
					,RoleType
					,TransactionType
					,Point
					,TransactionDate
					,TransactionMode
					,ActionUser
					,Remark
					,SystemRemark
					)
				VALUES (
					@Sno3
					,0
					,'2'
					,'3'
					,@AdminPointRetriveAmount
					,GETDATE()
					,'CR'
					,0
					,@AdminPointRetriveRemark
					,@AdminPointRetriveSystemRemark
					)

				UPDATE tbl_agent_balance
				SET Amount = Amount - @AdminPointRetriveAmount
					,CreditPointUse = CreditPointUse - @AdminPointRetriveAmount
					,ActionUser = 0
					,ActionDate = GETDATE()
				WHERE AgentId = @AgentId
					AND RoleType = 4;

				UPDATE tbl_System_Balance
				SET Amount = ISNULL(Amount, 0) + ISNULL(@AdminPointRetriveAmount, 0)
				WHERE ID = 1;
			END

			------------ Point retrive in case of club used credit amount -----------------
			COMMIT TRANSACTION @TransactionName;

			SELECT 0 Code
				,'Point transfer request approved successfully' Message;

			RETURN;
		END

		COMMIT TRANSACTION @TransactionName;

		SELECT 0 Code
			,'Point transfer request rejected successfully' Message;

		RETURN;
	END
	ELSE
	BEGIN
		SELECT 1 Code
			,'Invalid request' Message;

		RETURN;
	END
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION @TransactionName;

	SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

	INSERT INTO dbo.tbl_error_log (
		ErrorDesc
		,ErrorScript
		,QueryString
		,ErrorCategory
		,ErrorSource
		,ActionDate
		)
	VALUES (
		@ErrorDesc
		,'sproc_admin_manage_club_to_admin_payment_transaction'
		,'SQL'
		,'SQL'
		,'sproc_admin_manage_club_to_admin_payment_transaction'
		,GETDATE()
		);

	SELECT 1 Code
		,'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;

	RETURN;
END CATCH;
GO


