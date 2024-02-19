USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_cp_reservation_transaction_management]    Script Date: 2/16/2024 5:53:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROC [dbo].[sproc_cp_reservation_transaction_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @CustomerId VARCHAR(10) = NULL,
    @VisitDate VARCHAR(10) = NULL,
    @VisitTime VARCHAR(5) = NULL,
    @NoOfPeople INT = 0,
    @HostIdList VARCHAR(MAX) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @PlanId VARCHAR(10) = NULL,
    @InvoiceId VARCHAR(100) = NULL,
    @ReservationId VARCHAR(10) = NULL,
    @PaymentType VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @PlanDetailId BIGINT,
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @CommissionAmount DECIMAL(18, 2),
        @SQLString VARCHAR(MAX),
        @NickName NVARCHAR(200),
        @EmailAddress VARCHAR(200),
        @SmsEmailResponseCode INT = 1,
        @MobileNumber VARCHAR(15),
        @UserId VARCHAR(10);
DECLARE @CommissionId BIGINT,
        @Price DECIMAL(18, 2),
        @AdminCommissionAmount DECIMAL(18, 2),
        @AdminTotalCommission DECIMAL(18, 2),
        @TotalAmount DECIMAL(18, 2),
        @AdminPaymentAmount DECIMAL(18, 2),
        @ReservationType VARCHAR(10),
        @ReservationTxnId VARCHAR(10),
        @CustomerCostAmount DECIMAL(18, 2);
BEGIN
    BEGIN TRY
        IF ISNULL(@Flag, '') = 'rc' --Reservation Confirmation
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_customer a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(b.[Status], '') = 'A'
                WHERE a.AgentId = @CustomerId
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid customer details' Message;
                RETURN;
            END;

            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_club_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND ISNULL(a.[Status], '') = 'A'
                WHERE a.AgentId = @ClubId
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid club details' Message;
                RETURN;
            END;

            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_plans a WITH (NOLOCK)
                WHERE a.PlanId = @PlanId
                      AND ISNULL(a.[PlanStatus], '') = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid plan details' Message;
                RETURN;
            END;

            SELECT @TransactionName = 'Flag_ird',
                   @InvoiceId = dbo.[func_generate_invoice_no]('CRS');

            BEGIN TRANSACTION @TransactionName;

            INSERT INTO dbo.tbl_reservation_detail
            (
                InvoiceId,
                ClubId,
                CustomerId,
                VisitDate,
                VisitTime,
                NoOfPeople,
                PaymentType,
                TransactionStatus,
                LocationVerificationStatus,
                OTPVerificationStatus,
                ActionUser,
                ActionIP,
                ActionPlatform
            )
            VALUES
            (@InvoiceId, @ClubId, @CustomerId, @VisitDate, @VisitTime, @NoOfPeople, @PaymentType, 'P', 'P', 'P',
             @ActionUser, @ActionIP, @ActionPlatform);

            SET @Sno = SCOPE_IDENTITY();

            UPDATE dbo.tbl_reservation_detail
            SET ReservationId = @Sno
            WHERE Sno = @Sno;

            INSERT INTO dbo.tbl_reservation_plan_detail
            (
                ReservationId,
                PlanName,
                PlanType,
                PlanTime,
                Price,
                Liquor,
                Nomination,
                Remarks,
                PlanId
            )
            SELECT @Sno,
                   a.PlanName,
                   a.PlanType,
                   a.PlanTime,
                   a.Price,
                   a.Liquor,
                   a.Nomination,
                   a.Remarks,
                   a.PlanId
            FROM dbo.tbl_plans a WITH (NOLOCK)
            WHERE a.PlanId = @PlanId;

            SET @PlanDetailId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_reservation_plan_detail
            SET PlanDetailId = @PlanDetailId
            WHERE Sno = @PlanDetailId;

            SET @SQLString
                = 'INSERT INTO dbo.tbl_reservation_host_detail
		(
		    ReservationId,
			HostDetailId,
		    HostId,
		    CreatedUser,
		    CreatedIP,
		    CreatedPlatform
		)'  ;
            SET @SQLString += ' SELECT ' + CAST(@Sno AS VARCHAR) + ',' + CAST(@Sno AS VARCHAR) + ', a.HostId,'''
                              + CAST(ISNULL(@ActionUser, '') AS VARCHAR(200)) + ''','''
                              + CAST(ISNULL(@ActionIP, '') AS VARCHAR(50)) + ''','''
                              + CAST(ISNULL(@ActionPlatform, '') AS VARCHAR(20))
                              + ''' FROM dbo.tbl_host_details a WITH (NOLOCK) WHERE a.AgentId = '
                              + CAST(@ClubId AS VARCHAR(10)) + ' AND a.HostId IN (' + CAST(@HostIdList AS VARCHAR(MAX))
                              + '); UPDATE dbo.tbl_reservation_host_detail SET HostDetailId = SCOPE_IDENTITY()  WHERE Sno = SCOPE_IDENTITY();';

            PRINT (@SQLString);
            EXEC (@SQLString);

            UPDATE dbo.tbl_reservation_detail
            SET PlanDetailId = @PlanDetailId,
                HostDetailId = @Sno2
            WHERE ReservationId = @Sno;

            SELECT @CommissionId = b.CommissionId,
                   @Price = ISNULL(c.Price, 0),
                   @ClubId = a.ClubId,
                   @NoOfPeople = a.NoOfPeople
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = a.ClubId
                INNER JOIN dbo.tbl_reservation_plan_detail c WITH (NOLOCK)
                    ON c.ReservationId = a.ReservationId
            WHERE a.ReservationId = @Sno
                  AND a.CustomerId = @CustomerId
                  AND ISNULL(a.[TransactionStatus], '') = 'P';


            SELECT TOP 1
                   @AdminCommissionAmount = ISNULL(b.CommissionValue, 0)
            FROM dbo.tbl_commission_category a WITH (NOLOCK)
                INNER JOIN dbo.tbl_commission_category_detail b WITH (NOLOCK)
                    ON b.CategoryId = a.CategoryId
            WHERE a.CategoryId = @CommissionId
                  AND @Price
                  BETWEEN ISNULL(b.FromAmount, 0) AND ISNULL(b.ToAmount, 0)
                  AND ISNULL(a.Status, '') = 'A'
                  AND ISNULL(b.Status, '') = 'A';


            SELECT @Price = @Price,
                   @TotalAmount = @Price * @NoOfPeople,
                   @AdminTotalCommission = @AdminCommissionAmount * @NoOfPeople;

            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.CustomerId = @CustomerId
                      AND a.ReservationId <> @Sno
            )
            BEGIN
                SELECT @ReservationType = 1,
                       @AdminPaymentAmount = @AdminCommissionAmount * @NoOfPeople,
                       @CustomerCostAmount = 0;

            END;
            ELSE IF EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.CustomerId = @CustomerId
                      AND ISNULL(a.TransactionStatus, '') IN ( 'S' )
                      AND ISNULL(a.OTPVerificationStatus, '') IN ( 'A' )
                      AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                      BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(GETDATE(), 'yyyy-MM-dd')
                      AND a.ClubId = @ClubId
                      AND a.ReservationId <> @Sno
            )
            BEGIN
                SELECT @ReservationType = 2,
                       @AdminPaymentAmount = 0,
                       @CustomerCostAmount = @TotalAmount;

            END;
            ELSE
            BEGIN
                SELECT @ReservationType = 3,
                       @AdminPaymentAmount = @TotalAmount + @AdminTotalCommission,
                       @CustomerCostAmount = @TotalAmount;
            END;

            INSERT INTO dbo.tbl_reservation_transaction_detail
            (
                ReservationId,
                PlanAmount,
                TotalAmount,
                CommissionId,
                CommissionAmount,
                TotalCommissionAmount,
                AdminPaymentAmount,
                Status,
                ActionUser,
                ActionDate,
                ActionIP,
                ActionPlatform,
                ReservationType
            )
            VALUES
            (@Sno, @Price, @TotalAmount, @CommissionId, @AdminCommissionAmount, @AdminTotalCommission,
             @AdminPaymentAmount, 'I', @ActionUser, GETDATE(), @ActionIP, @ActionPlatform, @ReservationType);

            SET @ReservationTxnId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_reservation_transaction_detail
            SET ReservationTxnId = @ReservationTxnId
            WHERE Sno = @ReservationTxnId;

            SELECT @MobileNumber = b.MobileNumber,
                   @NickName = (b.NickName),
                   @UserId = c.UserId,
                   @EmailAddress = b.EmailAddress
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_customer b WITH (NOLOCK)
                    ON b.AgentId = a.CustomerId
                INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
                       AND ISNULL(c.[Status], '') = 'A'
            WHERE a.ReservationId = @Sno
                  AND a.CustomerId = @CustomerId
                  AND ISNULL(a.TransactionStatus, '') = 'P';

            COMMIT TRANSACTION @TransactionName;

            CREATE TABLE #temp_ird
            (
                Code INT
            );

            INSERT INTO #temp_ird
            (
                Code
            )
            EXEC dbo.sproc_email_sms_management @Flag = '4',
                                                @EmailSendTo = @EmailAddress,
                                                @Username = @NickName,
                                                @AgentId = @CustomerId,
                                                @UserId = @CustomerId,
                                                @ActionUser = @ActionUser,
                                                @ActionIP = @ActionIP,
                                                @ActionPlatform = @ActionPlatform,
                                                @ResponseCode = @SmsEmailResponseCode OUTPUT;
            DROP TABLE #temp_ird;

            SELECT 0 Code,
                   'Reservation successfull' Message;
        END;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION @TransactionName;

        SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR(200));

        INSERT INTO dbo.tbl_error_log
        (
            ErrorDesc,
            ErrorScript,
            QueryString,
            ErrorCategory,
            ErrorSource,
            ActionDate
        )
        VALUES
        (@ErrorDesc, 'sproc_cp_reservation_transaction_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
         'sproc_cp_reservation_transaction_management', GETDATE());

        SELECT 1 Code,
               'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR(10)) Message;
        RETURN;
    END CATCH;
END;
GO


