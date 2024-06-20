USE [CRS.UAT_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_cp_reservation_transaction_management]    Script Date: 6/7/2024 10:35:19 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_cp_reservation_transaction_management] @Flag VARCHAR(10)
	,@ClubId VARCHAR(10) = NULL
	,@CustomerId VARCHAR(10) = NULL
	,@VisitDate VARCHAR(10) = NULL
	,@VisitTime VARCHAR(5) = NULL
	,@NoOfPeople INT = 0
	,@HostIdList VARCHAR(MAX) = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	,@PlanId VARCHAR(10) = NULL
	,@InvoiceId VARCHAR(100) = NULL
	,@ReservationId VARCHAR(10) = NULL
	,@PaymentType VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT
	,@Sno2 BIGINT
	,@PlanDetailId BIGINT
	,@TransactionName VARCHAR(200)
	,@ErrorDesc VARCHAR(MAX);
DECLARE @SQLString VARCHAR(MAX)
	,@NickName NVARCHAR(200)
	,@EmailAddress VARCHAR(200)
	,@SmsEmailResponseCode INT = 1
	,@MobileNumber VARCHAR(15)
	,@UserId VARCHAR(10);
DECLARE @ReservationType VARCHAR(10)
	,@ReservationTxnId VARCHAR(10)
	,@CustomerCostAmount DECIMAL(18, 2)
	,@OTPCode VARCHAR(10)
	,@AgentId VARCHAR(10) = NULL;
DECLARE @PlanAmount DECIMAL(18, 2)
	,@TotalPlanAmount DECIMAL(18, 2)
	,@TotalClubPlanAmount DECIMAL(18, 2)
	,@CommissionId VARCHAR(10)
	,@AdminPlanCommissionAmount DECIMAL(18, 2)
	,@TotalAdminPlanCommissionAmount DECIMAL(18, 2)
	,@AdminCommissionAmount DECIMAL(18, 2)
	,@TotalAdminCommissionAmount DECIMAL(18, 2)
	,@TotalAdminPayableAmount DECIMAL(18, 2);

BEGIN
	BEGIN TRY
		IF ISNULL(@Flag, '') = 'rc' --Reservation Confirmation
		BEGIN
			IF NOT EXISTS (
					SELECT 'X'
					FROM dbo.tbl_customer a WITH (NOLOCK)
					INNER JOIN dbo.tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
						AND ISNULL(b.[Status], '') = 'A'
					WHERE a.AgentId = @CustomerId
					)
			BEGIN
				SELECT 1 Code
					,'Invalid customer details' Message;

				RETURN;
			END;

			IF NOT EXISTS (
					SELECT 'X'
					FROM dbo.tbl_club_details a WITH (NOLOCK)
					INNER JOIN dbo.tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
						AND ISNULL(a.[Status], '') = 'A'
					WHERE a.AgentId = @ClubId
					)
			BEGIN
				SELECT 1 Code
					,'Invalid club details' Message;

				RETURN;
			END;

			IF NOT EXISTS (
					SELECT 'X'
					FROM dbo.tbl_plans a WITH (NOLOCK)
					WHERE a.PlanId = @PlanId
						AND ISNULL(a.[PlanStatus], '') = 'A'
					)
			BEGIN
				SELECT 1 Code
					,'Invalid plan details' Message;

				RETURN;
			END;

			IF (
					SELECT COALESCE(COUNT(a.ReservationId), 0)
					FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
					WHERE a.ClubId = @ClubId
						AND a.VisitDate = @VisitDate
						AND DATEPART(HOUR, a.VisitTime) = DATEPART(HOUR, @VisitTime)
						AND ISNULL(a.TransactionStatus, '') IN (
							'P'
							,'A'
							,'S'
							)
					) > 5
			BEGIN
				SELECT 1 Code
					,'Reservations are full for the given date and time. Please select another date or time.' Message;

				RETURN;
			END;

			SELECT @TransactionName = 'Flag_ird'
				,@InvoiceId = dbo.[func_generate_invoice_no]('CRS')
				,@VisitDate = FORMAT(CONVERT(DATE, @VisitDate, 111), 'yyyy-MM-dd');

			BEGIN TRANSACTION @TransactionName;

			INSERT INTO dbo.tbl_reservation_detail (
				InvoiceId
				,ClubId
				,CustomerId
				,VisitDate
				,VisitTime
				,NoOfPeople
				,PaymentType
				,TransactionStatus
				,LocationVerificationStatus
				,OTPVerificationStatus
				,ActionUser
				,ActionIP
				,ActionPlatform
				)
			VALUES (
				@InvoiceId
				,@ClubId
				,@CustomerId
				,@VisitDate
				,@VisitTime
				,@NoOfPeople
				,@PaymentType
				,'A'
				,'P'
				,'P'
				,@ActionUser
				,@ActionIP
				,@ActionPlatform
				);

			SET @Sno = SCOPE_IDENTITY();

			UPDATE dbo.tbl_reservation_detail
			SET ReservationId = @Sno
			WHERE Sno = @Sno;

			INSERT INTO dbo.tbl_reservation_plan_detail (
				ReservationId
				,PlanName
				,PlanType
				,PlanTime
				,Price
				,Liquor
				,Nomination
				,Remarks
				,PlanId
				)
			SELECT @Sno
				,a.PlanName
				,a.PlanType
				,a.PlanTime
				,a.Price
				,a.Liquor
				,a.Nomination
				,a.Remarks
				,a.PlanId
			FROM dbo.tbl_plans a WITH (NOLOCK)
			WHERE a.PlanId = @PlanId;

			SET @PlanDetailId = SCOPE_IDENTITY();

			UPDATE dbo.tbl_reservation_plan_detail
			SET PlanDetailId = @PlanDetailId
			WHERE Sno = @PlanDetailId;

			IF @HostIdList = '0'
			BEGIN
				INSERT INTO dbo.tbl_reservation_host_detail (
					ReservationId
					,HostId
					,CreatedUser
					,CreatedIP
					,CreatedPlatform
					)
				VALUES (
					@Sno
					,0
					,@ActionUser
					,@ActionIP
					,@ActionPlatform
					);

				SET @Sno2 = SCOPE_IDENTITY();

				UPDATE dbo.tbl_reservation_host_detail
				SET HostDetailId = @Sno2
				WHERE Sno = @Sno2;
			END;
			ELSE
			BEGIN
				SET @SQLString = 'INSERT INTO dbo.tbl_reservation_host_detail
        (
            ReservationId,
            HostId,
            CreatedUser,
            CreatedIP,
            CreatedPlatform
        )';
				SET @SQLString += ' SELECT ' + CAST(@Sno AS VARCHAR) + ', a.HostId,''' + CAST(ISNULL(@ActionUser, '') AS VARCHAR(200)) + ''',''' + CAST(ISNULL(@ActionIP, '') AS VARCHAR(50)) + ''',''' + CAST(ISNULL(@ActionPlatform, '') AS VARCHAR(20)) + ''' FROM dbo.tbl_host_details a WITH (NOLOCK) WHERE a.AgentId = ' + CAST(@ClubId AS VARCHAR(10)) + ' AND a.HostId IN (' + CAST(@HostIdList AS VARCHAR(MAX)) + '); UPDATE dbo.tbl_reservation_host_detail SET HostDetailId = SCOPE_IDENTITY()  WHERE Sno = SCOPE_IDENTITY();';

				PRINT (@SQLString);

				EXEC (@SQLString);

				SET @Sno2 = SCOPE_IDENTITY();

				UPDATE dbo.tbl_reservation_host_detail
				SET HostDetailId = @Sno2
				WHERE Sno = @Sno2;
			END;

			UPDATE dbo.tbl_reservation_detail
			SET PlanDetailId = @PlanDetailId
				,HostDetailId = @Sno2
			WHERE ReservationId = @Sno;

			SELECT @CommissionId = b.CommissionId
				,@PlanAmount = ISNULL(c.Price, 0)
				,@ClubId = a.ClubId
				,@NoOfPeople = a.NoOfPeople
			FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
			INNER JOIN dbo.tbl_club_details b WITH (NOLOCK) ON b.AgentId = a.ClubId
			INNER JOIN dbo.tbl_reservation_plan_detail c WITH (NOLOCK) ON c.ReservationId = a.ReservationId
			WHERE a.ReservationId = @Sno
				AND a.CustomerId = @CustomerId
				AND ISNULL(a.[TransactionStatus], '') = 'A';

			IF ISNULL(@CommissionId, '') = ''
			BEGIN
				SELECT @CommissionId = a.CategoryId
				FROM dbo.tbl_commission_category a WITH (NOLOCK)
				WHERE ISNULL(a.IsDefault, 0) = 1
					AND ISNULL(a.STATUS, '') = 'A'
			END

			IF ISNULL(@CommissionId, '') <> ''
			BEGIN
				SELECT @AdminCommissionAmount = MAX(CASE 
							WHEN b.AdminCommissionTypeId = 1
								THEN ISNULL(b.CommissionValue, 0)
							END)
					,@AdminPlanCommissionAmount = MAX(CASE 
							WHEN b.AdminCommissionTypeId = 2
								THEN ISNULL(b.CommissionValue, 0)
							END)
				FROM dbo.tbl_commission_category a WITH (NOLOCK)
				INNER JOIN dbo.tbl_commission_category_detail b WITH (NOLOCK) ON b.CategoryId = a.CategoryId
				WHERE a.CategoryId = @CommissionId
					AND @PlanAmount BETWEEN ISNULL(b.FromAmount, 0)
						AND ISNULL(b.ToAmount, 0)
					AND ISNULL(a.STATUS, '') = 'A'
					AND ISNULL(b.STATUS, '') = 'A';
			END
			ELSE
			BEGIN
				SELECT @AdminCommissionAmount = 0
					,@AdminPlanCommissionAmount = 0;
			END

			SELECT @TotalPlanAmount = ISNULL(@PlanAmount, 0) * @NoOfPeople
				,@TotalAdminCommissionAmount = ISNULL(@AdminCommissionAmount, 0) * @NoOfPeople
				,@TotalAdminPlanCommissionAmount = ISNULL(@AdminPlanCommissionAmount, 0) * @NoOfPeople;

			SELECT @TotalClubPlanAmount = ISNULL(@TotalPlanAmount, 0) - ISNULL(@TotalAdminPlanCommissionAmount, 0);

			IF NOT EXISTS (
					SELECT 'X'
					FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
					WHERE a.CustomerId = @CustomerId
						AND a.ReservationId <> @Sno
					)
			BEGIN
				SELECT @ReservationType = 1
					,@TotalAdminPlanCommissionAmount = 0
					,@TotalAdminPayableAmount = ISNULL(@AdminCommissionAmount, 0) * @NoOfPeople
					,@CustomerCostAmount = 0
					,@TotalClubPlanAmount = 0;
			END;
					--ELSE IF EXISTS
					--(
					--    SELECT 'X'
					--    FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
					--    WHERE a.CustomerId = @CustomerId
					--          AND ISNULL(a.TransactionStatus, '') IN ( 'S', 'A' )
					--          --AND ISNULL(a.OTPVerificationStatus, '') IN ( 'A' )
					--          AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
					--          BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(GETDATE(), 'yyyy-MM-dd')
					--          AND a.ClubId = @ClubId
					--          AND a.ReservationId <> @Sno
					--)
					--BEGIN
					--    SELECT @ReservationType = 2,
					--           @TotalAdminPayableAmount = 0,
					--           @CustomerCostAmount = ISNULL(@TotalPlanAmount, 0);
					--END;
			ELSE
			BEGIN
				SELECT @ReservationType = 3
					,@TotalAdminPayableAmount = ISNULL(@TotalAdminPlanCommissionAmount, 0) + ISNULL(@TotalAdminCommissionAmount, 0)
					,@CustomerCostAmount = ISNULL(@TotalPlanAmount, 0);
			END;

			INSERT INTO dbo.tbl_reservation_transaction_detail (
				ReservationId
				,ReservationType
				,PlanAmount
				,TotalPlanAmount
				,TotalClubPlanAmount
				,CommissionId
				,AdminPlanCommissionAmount
				,TotalAdminPlanCommissionAmount
				,AdminCommissionAmount
				,TotalAdminCommissionAmount
				,TotalAdminPayableAmount
				,Remarks
				,AdminPaymentStatus
				,ActionUser
				,ActionDate
				,ActionIP
				,ActionPlatform
				)
			VALUES (
				@Sno
				,@ReservationType
				,@PlanAmount
				,@TotalPlanAmount
				,@TotalClubPlanAmount
				,@CommissionId
				,@AdminPlanCommissionAmount
				,@TotalAdminPlanCommissionAmount
				,@AdminCommissionAmount
				,@TotalAdminCommissionAmount
				,@TotalAdminPayableAmount
				,NULL
				,'I'
				,@ActionUser
				,GETDATE()
				,@ActionIP
				,@ActionPlatform
				)

			SET @ReservationTxnId = SCOPE_IDENTITY();

			UPDATE dbo.tbl_reservation_transaction_detail
			SET ReservationTxnId = @ReservationTxnId
			WHERE Sno = @ReservationTxnId;

			SELECT @MobileNumber = b.MobileNumber
				,@NickName = (b.NickName)
				,@UserId = c.UserId
				,@EmailAddress = b.EmailAddress
				,@OTPCode = dbo.func_generate_otp_code(6)
				,@AgentId = b.AgentId
			FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
			INNER JOIN dbo.tbl_customer b WITH (NOLOCK) ON b.AgentId = a.CustomerId
			INNER JOIN dbo.tbl_users c WITH (NOLOCK) ON c.AgentId = b.AgentId
				AND c.RoleType = 3
				AND ISNULL(c.[Status], '') = 'A'
			WHERE a.ReservationId = @Sno
				AND a.CustomerId = @CustomerId
				AND ISNULL(a.TransactionStatus, '') = 'A';

			COMMIT TRANSACTION @TransactionName;

			CREATE TABLE #temp_ird (Code INT);

			INSERT INTO #temp_ird (Code)
			EXEC dbo.sproc_email_sms_management @Flag = '6'
				,@MobileNumber = @MobileNumber
				,@EmailSendTo = @EmailAddress
				,@VerificationCode = @OTPCode
				,@Username = @NickName
				,@AgentId = @AgentId
				,@UserId = @UserId
				,@ActionUser = @ActionUser
				,@ActionIP = @ActionIP
				,@ActionPlatform = @ActionPlatform
				,@ExtraDatailId1 = @Sno
				,@ClubId = @ClubId
				,@ResponseCode = @SmsEmailResponseCode OUTPUT;

			DROP TABLE #temp_ird;

			SELECT 0 Code
				,'Reservation successfull' Message;

			RETURN;
		END;
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION @TransactionName;

		SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR(200));

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
			,'sproc_cp_reservation_transaction_management(Flag: ' + ISNULL(@Flag, '') + ')'
			,'SQL'
			,'SQL'
			,'sproc_cp_reservation_transaction_management'
			,GETDATE()
			);

		SELECT 1 Code
			,'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR(10)) Message;

		RETURN;
	END CATCH;
END;
GO


