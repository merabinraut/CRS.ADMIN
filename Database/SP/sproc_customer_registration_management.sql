USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_customer_registration_management]    Script Date: 15/12/2023 12:31:39 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO











ALTER PROC [dbo].[sproc_customer_registration_management]
    @Flag VARCHAR(10),
    @MobileNumber VARCHAR(15) = NULL,
    @NickName NVARCHAR(400) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(200) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @VerificationCode VARCHAR(6) = NULL,
    @AgentId VARCHAR(10) = NULL,
    @UserId VARCHAR(10) = NULL,
    @Password VARCHAR(16) = NULL,
    @ReferCode VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @genVerificationCode VARCHAR(MAX),
        @agentVerificationStatus CHAR(23),
        @agentVerificationCodepr VARCHAR(30),
        @actionIpAddress VARCHAR(20),
        @sendDatetime DATETIME,
        @id BIGINT,
        @newCardNo VARCHAR(20),
        @ReferDetailId VARCHAR(10),
        @ReferralId VARCHAR(10),
        @AffiliateId VARCHAR(10),
        @SmsEmailResponseCode INT = 1;

BEGIN TRY
    IF ISNULL(@Flag, '') = 'rhc' --register hold customer
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 3
                       AND ISNULL(b.Status, '') NOT IN ( 'D' )
            WHERE a.MobileNumber = @MobileNumber
            UNION
            SELECT 'X'
            FROM dbo.tbl_customer_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
            UNION
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 5
                       AND ISNULL(b.Status, '') NOT IN ( 'D' )
            WHERE a.MobileNumber = @MobileNumber
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate mobile number' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
            WHERE a.NickName = @NickName
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate nick name' Message;
            RETURN;
        END;

        SET @TransactionName = 'Flag_rhc';
        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_customer_hold
        (
            NickName,
            MobileNumber,
            Status,
            CreatedBy,
            CreatedIP,
            CreatedPlatform,
            CreatedDate
        )
        VALUES
        (@NickName, @MobileNumber, 'P', @ActionUser, @ActionIP, @ActionPlatform, GETDATE());

        SELECT @Sno = SCOPE_IDENTITY(),
               @VerificationCode = dbo.func_generate_otp_code(6);

        --INSERT INTO dbo.tbl_verification_sent
        --(
        --    RoleId,
        --    AgentId,
        --    UserId,
        --    MobileNumber,
        --    FullName,
        --    VerificationCode,
        --    Status,
        --    CreatedBy,
        --    CreatedDate,
        --    CreatedIP,
        --    CreatedPlatform
        --)
        --VALUES
        --(   3, @Sno, @Sno, @MobileNumber, @NickName, @VerificationCode, 'S', --sent
        --    @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

        --INSERT INTO dbo.tbl_sms_sent
        --(
        --    RoleId,
        --    AgentId,
        --    UserId,
        --    DestinationNumber,
        --    Message,
        --    Status,
        --    CreatedBy,
        --    CreatedDate,
        --    CreatedIP,
        --    CreatedPlatform
        --)
        --VALUES
        --(   3,                                                           -- RoleId - bigint
        --    @Sno,                                                        -- AgentId - bigint
        --    @Sno,                                                        -- UserId - bigint
        --    @MobileNumber,                                               -- DestinationNumber - varchar(15)
        --    'Dear ' + CAST(ISNULL(@NickName, '') AS VARCHAR) + ', your verification code is'
        --    + CAST(ISNULL(@VerificationCode, '') AS VARCHAR)
        --    + '. Use this code to complete the registration. - Hostlog', -- Message - varchar(max)
        --    'P',                                                         -- Status - char(1)		   
        --    @ActionUser,                                                 -- CreatedBy - varchar(200)
        --    GETDATE(),                                                   -- CreatedDate - datetime
        --    @ActionIP,                                                   -- CreatedIP - varchar(50)
        --    @ActionPlatform                                              -- CreatedPlatform - varchar(20)
        --    );
        COMMIT TRANSACTION @TransactionName;

        EXEC dbo.sproc_email_sms_management @Flag = '2',
                                            @Username = @NickName,
                                            @AgentId = @Sno,
                                            @UserId = @Sno,
                                            @MobileNumber = @MobileNumber,
                                            @ActionUser = @ActionUser,
                                            @ActionIP = @ActionIP,
                                            @ActionPlatform = @ActionPlatform,
                                            @ResponseCode = @SmsEmailResponseCode OUTPUT;

        SELECT 0 Code,
               'Registration successfull' Message,
               @Sno AS Extra1,
               DATEADD(MINUTE, 2, GETDATE()) AS Extra2;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'vrotp' --verify registration otp
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.Sno = @AgentId
                  AND ISNULL(a.[Status], '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user details' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_verification_sent a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.AgentId = @AgentId
                  AND a.UserId = @AgentId
                  AND a.VerificationCode = @VerificationCode
                  AND ISNULL(a.Status, '') = 'S'
        )
        BEGIN
            SELECT 1 Code,
                   'OTP code did not match' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_verification_sent a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.AgentId = @AgentId
                  AND a.UserId = @AgentId
                  AND a.VerificationCode = @VerificationCode
                  --AND (DATEDIFF(SECOND, a.SentDate, GETDATE()) > 120)
                  AND ISNULL(a.Status, '') = 'S'
        )
        BEGIN
            SELECT 1 Code,
                   'OTP code has expired' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate mobile number' Message;
            RETURN;
        END;

        IF ISNULL(@ReferCode, '') <> ''
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_affiliate_refer_details a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_affiliate b WITH (NOLOCK)
                        ON b.AgentId = a.AffiliateId
                           AND a.Status = 'A'
                    INNER JOIN dbo.tbl_static_data c WITH (NOLOCK)
                        ON c.StaticDataType = 28
                           AND c.StaticDataValue = a.SnsId
                           AND ISNULL(c.Status, '') = 'A'
                WHERE a.ReferId = @ReferCode
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid refer code' Message;
                RETURN;
            END;

            SELECT @AffiliateId = b.AgentId
            FROM dbo.tbl_affiliate_refer_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_affiliate b WITH (NOLOCK)
                    ON b.AgentId = a.AffiliateId
                       AND a.Status = 'A'
                INNER JOIN dbo.tbl_static_data c WITH (NOLOCK)
                    ON c.StaticDataType = 28
                       AND c.StaticDataValue = a.SnsId
                       AND ISNULL(c.Status, '') = 'A'
            WHERE a.ReferId = @ReferCode;

            IF ISNULL(@AffiliateId, '') = ''
            BEGIN
                SELECT 1 Code,
                       'Invalid refer code' Message;
                RETURN;
            END;
        END;

        SET @TransactionName = 'Flag_vrotp';
        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_customer
        (
            NickName,
            MobileNumber,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate
        )
        SELECT a.NickName,
               MobileNumber,
               @ActionUser,
               @ActionIP,
               @ActionPlatform,
               GETDATE()
        FROM dbo.tbl_customer_hold a WITH (NOLOCK)
        WHERE a.MobileNumber = @MobileNumber
              AND a.Sno = @AgentId
              AND ISNULL(a.[Status], '') = 'P';

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_customer
        SET AgentId = @Sno
        WHERE Sno = @Sno;

        INSERT INTO dbo.tbl_users
        (
            AgentId,
            RoleId,
            LoginId,
            Status,
            IsPrimary,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate
        )
        VALUES
        (   @Sno,             -- AgentId - bigint
            3, @MobileNumber, -- LoginId - varchar(200)
            'A',              -- Status - char(1)
            'Y',              -- IsPrimary - char(1)
            @ActionUser,      -- ActionUser - varchar(200)
            @ActionIP,        -- ActionIP - varchar(50)
            @ActionPlatform,  -- ActionPlatform - varchar(20)
            GETDATE()         -- ActionDate - datetime
            );
        SET @Sno2 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_users
        SET UserId = @Sno2
        WHERE Sno = @Sno2;

        UPDATE dbo.tbl_customer_hold
        SET Status = 'A',
            UpdatedBy = @ActionUser,
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform,
            UpdatedDate = GETDATE()
        WHERE MobileNumber = @MobileNumber
              AND Sno = @AgentId
              AND ISNULL([Status], '') = 'P';

        IF ISNULL(@ReferCode, '') <> ''
           AND ISNULL(@AffiliateId, '') <> ''
        BEGIN
            INSERT INTO dbo.tbl_customer_refer_detail
            (
                ReferId,
                AffiliateId,
                CustomerId,
                Status,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            VALUES
            (@ReferCode, @AffiliateId, @Sno, 'A', @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

            SET @ReferDetailId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_customer_refer_detail
            SET ReferDetailId = @ReferDetailId
            WHERE Sno = @ReferDetailId;

            INSERT INTO dbo.tbl_affiliate_referral_detail
            (
                ReferId,
                ReferDetailId,
                AffiliateId,
                CustomerId,
                Status,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            VALUES
            (@ReferCode, @ReferDetailId, @AffiliateId, @Sno, 'I', @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

            SET @ReferralId = SCOPE_IDENTITY();

            UPDATE dbo.tbl_affiliate_referral_detail
            SET ReferralId = @ReferralId
            WHERE Sno = @ReferralId;
        END;

        COMMIT TRANSACTION @TransactionName;
        SELECT 0 Code,
               'User registration successfull' Message,
               @Sno AS Extra1,
               @Sno2 AS Extra2;

        SELECT @NickName = a.NickName
        FROM dbo.tbl_customer a WITH (NOLOCK)
        WHERE a.Sno = @Sno;
        EXEC dbo.sproc_email_sms_management @Flag = '1',
                                            @Username = @NickName,
                                            @AgentId = @AgentId,
                                            @UserId = @AgentId,
                                            @MobileNumber = @MobileNumber,
                                            @ActionUser = @ActionUser,
                                            @ActionIP = @ActionIP,
                                            @ActionPlatform = @ActionPlatform,
                                            @ResponseCode = @SmsEmailResponseCode OUTPUT;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'rrotp' --resend registration otp
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.Sno = @AgentId
                  AND ISNULL(a.[Status], '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user details' Message;
            RETURN;
        END;

        SELECT @VerificationCode = dbo.func_generate_otp_code(6);

        INSERT INTO dbo.tbl_verification_sent
        (
            RoleId,
            AgentId,
            UserId,
            MobileNumber,
            FullName,
            VerificationCode,
            Status,
            ProcessId,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        SELECT 3,
               a.AgentId,
               a.UserId,
               a.MobileNumber,
               a.FullName,
               @VerificationCode,
               'S', --sent
               GETDATE(),
               @ActionUser,
               GETDATE(),
               @ActionIP,
               @ActionPlatform
        FROM dbo.tbl_verification_sent a WITH (NOLOCK)
        WHERE a.MobileNumber = @MobileNumber
              AND a.AgentId = @AgentId
              AND a.UserId = @AgentId
              AND ISNULL(a.Status, '') = 'P';

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_verification_sent
        SET Status = 'E', --Expired
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE MobileNumber = @MobileNumber
              AND AgentId = @AgentId
              AND UserId = @AgentId
              AND ISNULL(Status, '') = 'P'
              AND Sno <> @Sno;

        SELECT 0 Code,
               'OTP code sent successfully' Message,
               DATEADD(MINUTE, 2, GETDATE()) AS Extra2;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'srp' --set registration password
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 3
                       AND ISNULL(b.[Status], '') = 'A'
            WHERE a.AgentId = @AgentId
                  AND b.UserId = @UserId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid customer details' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_users
        SET Password = PWDENCRYPT(@Password),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId
              AND UserId = @UserId
              AND RoleId = 3
              AND [Status] = 'A';

        SELECT 0 Code,
               'Customer password set successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'fp_otp' --forgot password OTP
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.Sno = @AgentId
                  AND ISNULL(a.[Status], '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user details' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
        )
        BEGIN
            SELECT 1 Code,
                   'Customer not details not found' Message;
            RETURN;
        END;

        SELECT @AgentId = AgentId,
               @NickName = NickName
        FROM dbo.tbl_customer WITH (NOLOCK)
        WHERE MobileNumber = @MobileNumber;

        SELECT @VerificationCode = dbo.func_generate_otp_code(6);

        --INSERT INTO dbo.tbl_verification_sent
        --(
        --    RoleId,
        --    AgentId,
        --    UserId,
        --    MobileNumber,
        --    FullName,
        --    VerificationCode,
        --    Status,
        --    ProcessId,
        --    CreatedBy,
        --    CreatedDate,
        --    CreatedIP,
        --    CreatedPlatform
        --)
        --VALUES
        --(3, @AgentId, @AgentId, @MobileNumber, @NickName, @VerificationCode, 'S', GETDATE(), @ActionUser, GETDATE(),
        -- @ActionIP, 'CUSTOMER');

        --SET @Sno = SCOPE_IDENTITY();

        --UPDATE dbo.tbl_verification_sent
        --SET Status = 'E', --Expired
        --    UpdatedBy = @ActionUser,
        --    UpdatedDate = GETDATE(),
        --    UpdatedIP = @ActionIP,
        --    UpdatedPlatform = @ActionPlatform
        --WHERE MobileNumber = @MobileNumber
        --      AND AgentId = @AgentId
        --      AND UserId = @AgentId
        --      AND ISNULL(Status, '') = 'P'
        --      AND Sno <> @Sno;

        EXEC dbo.sproc_email_sms_management @Flag = '3',
                                            @Username = @NickName,
                                            @AgentId = @AgentId,
                                            @UserId = @AgentId,
                                            @MobileNumber = @MobileNumber,
                                            @ActionUser = @ActionUser,
                                            @ActionIP = @ActionIP,
                                            @ActionPlatform = @ActionPlatform,
                                            @ResponseCode = @SmsEmailResponseCode OUTPUT;

        SELECT 0 Code,
               'OTP code sent successfully' Message,
               DATEADD(MINUTE, 2, GETDATE()) AS Extra2;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'vfp_otp' --Verify forgot password OTP
    BEGIN
        IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer_hold a WITH (NOLOCK)
            WHERE a.MobileNumber = @MobileNumber
                  AND a.Sno = @AgentId
                  AND ISNULL(a.[Status], '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid user details' Message;
            RETURN;
        END;

        SELECT TOP 1
               @agentVerificationCodepr = VerificationCode,
               @sendDatetime = ProcessId,
               @AgentId = AgentId
        FROM tbl_verification_sent
        WHERE MobileNumber = @MobileNumber
              AND [Status] = 'S'
        ORDER BY CreatedDate DESC;

        SELECT @UserId = UserId
        FROM dbo.tbl_customer a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleId = 3
                   AND ISNULL(b.[Status], '') = 'A'
        WHERE a.AgentId = @AgentId;

        IF (DATEDIFF(MINUTE, @sendDatetime, GETDATE()) > 2)
        BEGIN
            SELECT '1' Code,
                   'OTP has been expired' Message,
                   NULL id;

            RETURN;
        END;

        IF (@agentVerificationCodepr = @VerificationCode)
        BEGIN

            UPDATE dbo.tbl_verification_sent
            SET [Status] = 'E'
            WHERE AgentId = @AgentId
                  AND MobileNumber = @MobileNumber;

            SELECT '0' Code,
                   'User verified Succesfully' Message,
                   @AgentId AS Extra1,
                   @MobileNumber AS Extra2,
                   @UserId AS Extra3;

            RETURN;
        END;

        SELECT '1' Code,
               'Otp verification failure!' Message,
               @MobileNumber id;

        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'snp' --set new password
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleId = 3
                       AND ISNULL(b.[Status], '') = 'A'
            WHERE a.AgentId = @AgentId
                  AND b.UserId = @UserId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid customer details' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_users
        SET Password = PWDENCRYPT(@Password),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE()
        WHERE AgentId = @AgentId
              AND RoleId = 3
              AND [Status] = 'A';

        SELECT 0 Code,
               'Customer password reset successfully' Message;
        RETURN;
    END;
    ELSE IF @Flag = 'vrc' --validate referal code
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate_refer_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_affiliate b WITH (NOLOCK)
                    ON b.AgentId = a.AffiliateId
                       AND a.Status = 'A'
                INNER JOIN dbo.tbl_static_data c WITH (NOLOCK)
                    ON c.StaticDataType = 28
                       AND c.StaticDataValue = a.SnsId
                       AND ISNULL(c.Status, '') = 'A'
            WHERE a.ReferId = @ReferCode
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid refer code' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_affiliate_refer_details
        SET TotalClickCount += 1,
            UpdatedBy = 'System',
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        FROM dbo.tbl_affiliate_refer_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_affiliate b WITH (NOLOCK)
                ON b.AgentId = a.AffiliateId
                   AND a.Status = 'A'
            INNER JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = 28
                   AND c.StaticDataValue = a.SnsId
                   AND ISNULL(c.Status, '') = 'A'
        WHERE a.ReferId = @ReferCode;

        SELECT 0 Code,
               CONCAT(b.FirstName, ' ', b.LastName, ' referral') Message
        FROM dbo.tbl_affiliate_refer_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_affiliate b WITH (NOLOCK)
                ON b.AgentId = a.AffiliateId
                   AND a.Status = 'A'
            INNER JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = 28
                   AND c.StaticDataValue = a.SnsId
                   AND ISNULL(c.Status, '') = 'A'
        WHERE a.ReferId = @ReferCode;
        RETURN;
    END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_customer_registration_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_customer_registration_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


