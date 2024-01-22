USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_reservation_management]    Script Date: 25/12/2023 14:38:30 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO















ALTER PROC [dbo].[sproc_reservation_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,         -- ClubId - bigint
    @CustomerId VARCHAR(10) = NULL,     -- CustomerId - bigint
    @VisitDate VARCHAR(10) = NULL,      -- VisitDate - varchar(10)
    @VisitTime VARCHAR(5) = NULL,       -- VisitTime - varchar(5)
    @NoOfPeople INT = NULL,             -- NoOfPeople - int
    @PaymentType VARCHAR(10) = NULL,    -- PaymentType - varchar(10)
    @ActionUser VARCHAR(200) = NULL,    -- ActionUser - varchar(200)
    @ActionIP VARCHAR(50) = NULL,       -- ActionIP - varchar(50)
    @ActionPlatform VARCHAR(20) = NULL, -- ActionPlatform - varchar(20)
    @PlanId VARCHAR(10) = NULL,
    @HostIdList VARCHAR(MAX) = NULL,
    @ReservationId VARCHAR(10) = NULL,
    @InvoiceId VARCHAR(100) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @PlanDetailId BIGINT,
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @CommissionAmount DECIMAL(18, 2), -- CommissionAmount - decimal(18, 2)
        @SQLString VARCHAR(MAX),
        @OTPCode VARCHAR(6),
        @NickName NVARCHAR(200),
        @MobileNumber VARCHAR(15),
        @UserId VARCHAR(10),
        @CustomerMessage NVARCHAR(512),
        @EmailAddress VARCHAR(200),
        @SmsEmailResponseCode INT = 1;
DECLARE @CommissionId BIGINT,
        @Price DECIMAL(18, 2),
        @AdminCommissionAmount DECIMAL(18, 2),
        @AdminTotalCommission DECIMAL(18, 2),
        @AdminPayableCommissionAmount DECIMAL(18, 2),
        @TotalAmount DECIMAL(18, 2),
        @AdminPaymentAmount DECIMAL(18, 2),
        @ReservationType VARCHAR(10),
        @ReservationTxnId VARCHAR(10);
BEGIN
    BEGIN TRY
        IF ISNULL(@Flag, '') = 'ird' --insert reservation destails
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

            SELECT @TransactionName = 'Flag_cr',
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
                CommissionAmount,
                TransactionStatus,
                LocationVerificationStatus,
                OTPVerificationStatus,
                ActionUser,
                ActionIP,
                ActionPlatform
            )
            VALUES
            (   @InvoiceId,                   -- InvoiceId - varchar(100)
                @ClubId,                      -- ClubId - bigint
                @CustomerId,                  -- CustomerId - bigint
                @VisitDate,                   -- VisitDate - varchar(10)
                @VisitTime,                   -- VisitTime - varchar(5)
                @NoOfPeople,                  -- NoOfPeople - int
                ISNULL(@CommissionAmount, 0), -- CommissionAmount - decimal(18, 2)
                'I',                          -- TransactionStatus - char(1)
                'P',                          -- LocationVerificationStatus - char(1)
                'P',                          -- OTPVerificationStatus - char(1)
                @ActionUser,                  -- ActionUser - varchar(200)
                @ActionIP,                    -- ActionIP - varchar(50)
                @ActionPlatform               -- ActionPlatform - varchar(20)
                );

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
                  AND ISNULL(a.[TransactionStatus], '') = 'I';


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
            )
            BEGIN
                SELECT @ReservationType = 1,
                       @AdminPaymentAmount = @AdminCommissionAmount * @NoOfPeople;

            END;
            ELSE IF EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.CustomerId = @CustomerId
                      AND ISNULL(a.TransactionStatus, '') = 'S'
                      AND ISNULL(a.OTPVerificationStatus, '') = 'A'
                      AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                      BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(GETDATE(), 'yyyy-MM-dd')
                      AND a.ClubId = @ClubId
                      AND a.ReservationId <> @ReservationId
            )
            BEGIN
                SELECT @ReservationType = 2,
                       @AdminPaymentAmount = 0;

            END;
            ELSE
            BEGIN
                SELECT @ReservationType = 3,
                       @AdminPaymentAmount = @TotalAmount + @AdminTotalCommission;
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
            (   @ReservationId,         -- ReservationId - bigint
                @Price,                 -- PlanAmount - decimal(18, 2)
                @TotalAmount,           -- TotalAmount - decimal(18, 2)
                @CommissionId,          -- CommissionId - bigint
                @AdminCommissionAmount, -- CommissionAmount - decimal(18, 2)
                @AdminTotalCommission,  -- TotalCommissionAmount - decimal(18, 2)
                @AdminPaymentAmount,    -- AdminPaymentAmount - decimal(18, 2)
                'I',                    -- AdminPaymentStatus - char(1)
                @ActionUser,            -- ActionUser - nvarchar(200)
                GETDATE(),              -- ActionDate - datetime
                @ActionIP,              -- ActionIP - varchar(50)
                @ActionPlatform,        -- ActionPlatform - varchar(20)
                @ReservationType);

            SET @ReservationTxnId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_reservation_transaction_detail
            SET ReservationTxnId = @ReservationTxnId
            WHERE Sno = @ReservationTxnId;

            SELECT 0 Code,
                   'Reservation initiated' Message,
                   a.ReservationId AS Extra1,
                   b.NickName AS Extra2,
                   CONCAT(YEAR(GETDATE()), '-', a.VisitDate, ' ', a.VisitTime) AS Extra3,
                   c.PlanName AS Extra4,
                   (a.NoOfPeople * c.Price) AS Extra5
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_customer b WITH (NOLOCK)
                    ON b.AgentId = a.CustomerId
                       AND ISNULL(a.TransactionStatus, '') = 'I'
                INNER JOIN dbo.tbl_reservation_plan_detail c WITH (NOLOCK)
                    ON c.PlanDetailId = a.PlanDetailId
            WHERE a.ReservationId = @Sno
                  AND b.AgentId = @CustomerId;

            COMMIT TRANSACTION @TransactionName;
        END;
        ELSE IF ISNULL(@Flag, '') = 'grd' --get reservation details
        BEGIN
            SELECT a.ReservationId,
                   a.InvoiceId,
                   a.ClubId,
                   a.CustomerId,
                   a.ReservedDate,
                   a.VisitDate,
                   a.VisitTime,
                   a.NoOfPeople,
                   a.PlanDetailId,
                   a.HostDetailId,
                   a.PaymentType,
                   a.CommissionAmount,
                   a.TransactionStatus,
                   a.LocationVerificationStatus,
                   a.OTPVerificationStatus,
                   a.ActionDate,
                   a.ActionUser,
                   a.ActionIP,
                   a.ActionPlatform
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_reservation_plan_detail b WITH (NOLOCK)
                    ON b.ReservationId = a.ReservationId
                INNER JOIN dbo.tbl_reservation_host_detail c WITH (NOLOCK)
                    ON c.ReservationId = a.ReservationId
            WHERE a.ReservationId = @ReservationId
                  AND a.CustomerId = @CustomerId
                  AND ISNULL(a.TransactionStatus, '') NOT IN ( 'D' );
            RETURN;
        END;
        ELSE IF ISNULL(@Flag, '') = 'grh' --get reservation history
        BEGIN
            SELECT a.ReservationId,
                   a.CustomerId,
                   a.ClubId,
                   a.InvoiceId,
                   a.ReservedDate,
                   a.VisitDate,
                   a.VisitTime,
                   b.ClubName1 AS ClubName,
                   b.GroupName,
                   CASE
                       WHEN FORMAT(a.ReservedDate, 'yyyy-MM-dd') = FORMAT(GETDATE(), 'yyyy-MM-dd') THEN
                           'Today'
                       ELSE
                           'Older'
                   END AS Dated,
                   b.Logo AS ClubLogo
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
                    ON b.AgentId = a.ClubId
            WHERE a.CustomerId = @CustomerId
                  AND a.ReservationId = ISNULL(@ReservationId, a.ReservationId)
                  --AND a.InvoiceId = ISNULL(@InvoiceId, a.InvoiceId)
                  AND ISNULL(a.TransactionStatus, '') NOT IN ( 'D' );
            --AND ISNULL(a.TransactionStatus, '') NOT IN ( 'C' );
            RETURN;
        END;
        ELSE IF ISNULL(@Flag, '') = 'mtpt' -- manage transaction payment type
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
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.ReservationId = @ReservationId
                      AND a.CustomerId = @CustomerId
                      AND ISNULL(a.[TransactionStatus], '') = 'I'
                      AND ISNULL(a.PaymentType, '') = ''
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid reservation details' Message;
                RETURN;
            END;

            IF ISNULL(@PaymentType, '') NOT IN ( '0', '1' )
            BEGIN
                SELECT 1 Code,
                       'Invalid payment type' Message;
                RETURN;
            END;

            SELECT @TransactionName = 'Flag_mtpt';
            BEGIN TRANSACTION @TransactionName;

            UPDATE dbo.tbl_reservation_detail
            SET TransactionStatus = 'P',
                PaymentType = @PaymentType,
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionDate = GETDATE()
            WHERE ReservationId = @ReservationId
                  AND CustomerId = @CustomerId
                  AND ISNULL([TransactionStatus], '') = 'I';

            SELECT @MobileNumber = b.MobileNumber,
                   @NickName = (b.NickName),
                   @OTPCode = dbo.func_generate_otp_code(6),
                   @UserId = c.UserId,
                   @EmailAddress = b.EmailAddress
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_customer b WITH (NOLOCK)
                    ON b.AgentId = a.CustomerId
                INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
                       AND ISNULL(c.[Status], '') = 'A'
            WHERE a.ReservationId = @ReservationId
                  AND a.CustomerId = @CustomerId
                  AND ISNULL(a.TransactionStatus, '') = 'P';

            COMMIT TRANSACTION @TransactionName;

            CREATE TABLE #temp_mtpt
            (
                Code INT
            );

            INSERT INTO #temp_mtpt
            (
                Code
            )
            EXEC dbo.sproc_email_sms_management @Flag = '4',
                                                @EmailSendTo = @EmailAddress,
                                                @VerificationCode = @OTPCode,
                                                @Username = @NickName,
                                                @AgentId = @CustomerId,
                                                @UserId = @CustomerId,
                                                @ActionUser = @ActionUser,
                                                @ActionIP = @ActionIP,
                                                @ActionPlatform = @ActionPlatform,
                                                @ResponseCode = @SmsEmailResponseCode OUTPUT;
            DROP TABLE #temp_mtpt;

            SELECT 0 Code,
                   'Reservation successfull' Message;

            RETURN;
        END;

        ELSE IF ISNULL(@Flag, '') = 'ghlfr' --get host list for reservation
        BEGIN
            SET @SQLString
                = '
			select  (select top 1 isnull(b.ImagePath, '''') from tbl_gallery b with (nolock) where b.AgentId = a.HostId and b.RoleId = 7 AND isnull(b.Status, '''') = ''A''  order by b.sno desc) AS HostImagePath,
					     a.HostName,
						 isnull(c.StaticDataLabel, '''') AS Occupation
			from tbl_host_details a with (nolock)
			left join tbl_static_data c with (nolock) on c.StaticDataType = 12 and c.StaticDataValue = a.PreviousOccupation and isnull(c.status, '''') = ''A''
			where a.HostId in (' + CAST(@HostIdList AS VARCHAR(MAX)) + ')';

            PRINT (@SQLString);
            EXEC (@SQLString);
            RETURN;
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
        (@ErrorDesc, 'sproc_reservation_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
         'sproc_reservation_management', GETDATE());

        SELECT 1 Code,
               'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR(10)) Message;
        RETURN;
    END CATCH;
END;
GO


