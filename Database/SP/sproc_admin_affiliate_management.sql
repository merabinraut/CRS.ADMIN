USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_affiliate_management]    Script Date: 14/12/2023 00:14:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER PROC [dbo].[sproc_admin_affiliate_management]
    @Flag VARCHAR(10),
    @HoldAgentId VARCHAR(10) = NULL,
    @AgentId VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(200) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @Status CHAR(1) = NULL,
	@SearchField VARCHAR(200) = NULL,
	@AffiliateId VARCHAR(10) = NULL,
	@FilterDate DATETIME = NULL
AS
DECLARE @Sno BIGINT,
        @Sno2 BIGINT,
        @TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX);
DECLARE @RandomPassword VARCHAR(20) = NULL,
        @SnsType VARCHAR(20) = NULL,
        @SnsURLLink NVARCHAR(MAX) = NULL,
		@DefaultRoleId BIGINT= 13;
DECLARE @SQLString NVARCHAR(MAX) = '',
		@SQLParameter NVARCHAR(MAX) ='';
BEGIN TRY
    IF @Flag = 'gal' --get affiliate list
    BEGIN
        WITH CTE
        AS (SELECT ISNULL(a.AgentId, '') AS AffiliateId,
                   '' AS HoldAffiliateId,
                   a.ProfileImage AS AffiliateImage,
                   CONCAT(ISNULL(a.FirstName, ''), ' ', ISNULL(a.LastName, '')) AS AffiliateFullName,
                   a.MobileNumber,
                   a.EmailAddress,
                   ISNULL(c.CreatedDate, '') AS RequestedDate,
                   CASE
                       WHEN ISNULL(b.Status, '') != ''
                            AND ISNULL(b.Status, '') = 'A' THEN
                           'Active'
                       WHEN ISNULL(b.Status, '') != ''
                            AND ISNULL(b.Status, '') = 'B' THEN
                           'Blocked'
                       ELSE
                           '-'
                   END AS Status,
                   CASE
                       WHEN ISNULL(c.Status, '') != ''
                            AND ISNULL(c.Status, '') = 'A' THEN
                           'Approved'
                       WHEN ISNULL(c.Status, '') != ''
                            AND ISNULL(c.Status, '') = 'P' THEN
                           'Pending'
                       WHEN ISNULL(c.Status, '') != ''
                            AND ISNULL(c.Status, '') = 'R' THEN
                           'Rejected'
                       ELSE
                           '-'
                   END AS ApprovedStatus,
                   c.SnsURLLink,
                   '' AS AffiliateType
            FROM dbo.tbl_affiliate a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 6
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
                LEFT JOIN dbo.tbl_affiliate_hold c WITH (NOLOCK)
                    ON c.MobileNumber = a.MobileNumber
                       AND c.EmailAddress = a.EmailAddress
                       AND ISNULL(c.Status, '') = 'A')
        SELECT ISNULL(a.AffiliateId, '') AS AffiliateId,
               a.HoldAffiliateId,
               a.AffiliateImage,
               a.AffiliateFullName,
               a.MobileNumber,
               a.EmailAddress,
               FORMAT(ISNULL(a.RequestedDate, ''), 'dd MMM, yyyy') AS RequestedDate,
               a.Status,
               a.ApprovedStatus,
               a.SnsURLLink,
               a.AffiliateType
        FROM CTE a WITH (NOLOCK)
        UNION ALL
        SELECT '' AS AffiliateId,
               a.HoldAgentId AS HoldAffiliateId,
               '' AS AffiliateImage,
               CONCAT(ISNULL(a.FirstName, ''), ' ', ISNULL(a.LastName, '')) AS AffiliateFullName,
               a.MobileNumber,
               a.EmailAddress,
               FORMAT(ISNULL(a.CreatedDate, ''), 'dd MMM, yyyy') AS RequestedDate,
               'Blocked' AS Status,
               CASE
                   WHEN ISNULL(a.Status, '') != ''
                        AND ISNULL(a.Status, '') = 'P' THEN
                       'Pending'
                   WHEN ISNULL(a.Status, '') != ''
                        AND ISNULL(a.Status, '') = 'R' THEN
                       'Rejected'
                   ELSE
                       '-'
               END AS ApprovedStatus,
               a.SnsURLLink,
               'Hold' AS AffiliateType
        FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
        WHERE a.Status IN ( 'P', 'R' );
        --AND a.MobileNumber NOT IN
        --    (
        --        SELECT a.MobileNumber FROM CTE a2 WITH (NOLOCK)
        --    );
        RETURN;
    END;

    ELSE IF @Flag = 'rarr' --reject affiliate registration request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
            WHERE a.HoldAgentId = @HoldAgentId
                  AND ISNULL(a.Status, '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_affiliate_hold
        SET Status = 'R',
            UpdatedBy = @ActionUser,
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform,
            UpdatedDate = GETDATE()
        WHERE HoldAgentId = @HoldAgentId
              AND ISNULL(Status, '') = 'P';

        SELECT 0 Code,
               'Affiliate registration request rejected successfully' Messgae;
        RETURN;
    END;

    ELSE IF @Flag = 'aarr' --approve affiliate registration request
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
            WHERE a.HoldAgentId = @HoldAgentId
                  AND ISNULL(a.Status, '') = 'P'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SELECT @TransactionName = 'Flag_aarr',
               @RandomPassword = 'Test@123'; --dbo.func_generate_random_no(8);

        BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_affiliate
        (
            FirstName,
            LastName,
            MobileNumber,
            EmailAddress,
            DOB,
            Gender,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate
        )
        SELECT a.FirstName,
               a.LastName,
               a.MobileNumber,
               a.EmailAddress,
               a.DOB,
               a.Gender,
               @ActionUser,
               @ActionIP,
               @ActionPlatform,
               GETDATE()
        FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
        WHERE a.HoldAgentId = @HoldAgentId
              AND ISNULL(a.Status, '') = 'P';

        SET @AgentId = SCOPE_IDENTITY();

        UPDATE dbo.tbl_affiliate
        SET AgentId = @AgentId
        WHERE Sno = @AgentId;

        INSERT INTO dbo.tbl_users
        (
            RoleId,
            AgentId,
            LoginId,
            Password,
            Status,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            FailedLoginAttempt,
            IsPasswordForceful,
            LastPasswordChangedDate,
            LastLoginDate,
            RoleType
        )
        SELECT @DefaultRoleId,
               @AgentId,
               a.MobileNumber,
               PWDENCRYPT(@RandomPassword),
               'A',
               @ActionUser,
               @AgentId,
               @ActionPlatform,
               GETDATE(),
               0,
               'Y',
               GETDATE(),
               NULL,
               6
        FROM dbo.tbl_affiliate a WITH (NOLOCK)
        WHERE a.AgentId = @AgentId;

        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_users
        SET UserId = @Sno
        WHERE Sno = @Sno;

        UPDATE dbo.tbl_affiliate_hold
        SET Status = 'A',
            UpdatedBy = @ActionUser,
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform,
            UpdatedDate = GETDATE()
        WHERE HoldAgentId = @HoldAgentId
              AND ISNULL(Status, '') = 'P';

        SELECT @SnsType = b.StaticDataLabel,
               @SnsURLLink = a.SnsURLLink
        FROM dbo.tbl_affiliate_hold a WITH (NOLOCK)
            LEFT JOIN dbo.tbl_static_data b WITH (NOLOCK)
                ON b.StaticDataType = 28
                   AND b.StaticDataValue = a.SnsType
                   AND ISNULL(b.Status, '') = 'A';

        IF ISNULL(@SnsType, '') != ''
           AND ISNULL(@SnsURLLink, '') != ''
        BEGIN
            IF @SnsType = 'Instagram'
            BEGIN
                INSERT INTO dbo.tbl_website_details
                (
                    AgentId,
                    InstagramLink,
                    ActionUser,
                    ActionIP,
                    ActionPlatform,
                    ActionDate,
                    RoleId
                )
                VALUES
                (   @AgentId,        -- AgentId - bigint
                    @SnsURLLink,     -- InstagramLink - varchar(max)
                    @ActionUser,     -- ActionUser - varchar(200)
                    @ActionIP,       -- ActionIP - varchar(20)
                    @ActionPlatform, -- ActionPlatform - varchar(20)
                    GETDATE(),       -- ActionDate - datetime
                    6                -- RoleId - bigint
                    );
            END;
            ELSE IF @SnsType = 'Facebook'
            BEGIN
                INSERT INTO dbo.tbl_website_details
                (
                    AgentId,
                    FacebookLink,
                    ActionUser,
                    ActionIP,
                    ActionPlatform,
                    ActionDate,
                    RoleId
                )
                VALUES
                (   @AgentId,        -- AgentId - bigint
                    @SnsURLLink,     -- InstagramLink - varchar(max)
                    @ActionUser,     -- ActionUser - varchar(200)
                    @ActionIP,       -- ActionIP - varchar(20)
                    @ActionPlatform, -- ActionPlatform - varchar(20)
                    GETDATE(),       -- ActionDate - datetime
                    6                -- RoleId - bigint
                    );
            END;
            ELSE IF @SnsType = 'Tiktok'
            BEGIN
                INSERT INTO dbo.tbl_website_details
                (
                    AgentId,
                    TiktokLink,
                    ActionUser,
                    ActionIP,
                    ActionPlatform,
                    ActionDate,
                    RoleId
                )
                VALUES
                (   @AgentId,        -- AgentId - bigint
                    @SnsURLLink,     -- InstagramLink - varchar(max)
                    @ActionUser,     -- ActionUser - varchar(200)
                    @ActionIP,       -- ActionIP - varchar(20)
                    @ActionPlatform, -- ActionPlatform - varchar(20)
                    GETDATE(),       -- ActionDate - datetime
                    6                -- RoleId - bigint
                    );
            END;
            ELSE IF @SnsType = 'Tiktok'
            BEGIN
                INSERT INTO dbo.tbl_website_details
                (
                    AgentId,
                    TwitterLink,
                    ActionUser,
                    ActionIP,
                    ActionPlatform,
                    ActionDate,
                    RoleId
                )
                VALUES
                (   @AgentId,        -- AgentId - bigint
                    @SnsURLLink,     -- InstagramLink - varchar(max)
                    @ActionUser,     -- ActionUser - varchar(200)
                    @ActionIP,       -- ActionIP - varchar(20)
                    @ActionPlatform, -- ActionPlatform - varchar(20)
                    GETDATE(),       -- ActionDate - datetime
                    6                -- RoleId - bigint
                    );
            END;
        END;

		INSERT INTO dbo.tbl_affiliate_refer_details
		(
			AffiliateId
		   ,SnsId
		   ,TotalClickCount
		   ,Status
		   ,CreatedBy
		   ,CreatedDate
		   ,CreatedIP
		   ,CreatedPlatform
		)
		SELECT @AgentId,
			   a.StaticDataValue,
			   0,
			   'A',
			   @ActionUser,
			   GETDATE(),
			   @ActionIP,
			   @ActionPlatform
		FROM dbo.tbl_static_data a WITH (NOLOCK)
		WHERE a.StaticDataType = 28
			AND a.Status = 'A'
		

		UPDATE dbo.tbl_affiliate_refer_details
		SET ReferId = Sno
		WHERE AffiliateId = @AgentId
			AND ISNULL(Status, '') = 'A';

        COMMIT TRANSACTION @TransactionName;

        SELECT 0 Code,
               'Affiliate registration request approved successfully' Messgae;
        RETURN;
    END;

    ELSE IF @Flag = 'mas' --manage affiliate status
    BEGIN
        IF ISNULL(@Status, '') = 'A'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_affiliate a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND b.RoleType = 6
                           AND b.Status = 'B'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid request' Message;
                RETURN;
            END;

            UPDATE dbo.tbl_users
            SET Status = 'A',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND RoleType = 6
                  AND Status = 'B';

            SELECT 0 Code,
                   'Affiliate status updated successfully' Message;
            RETURN;
        END;
        ELSE IF ISNULL(@Status, '') = 'B'
        BEGIN
            IF NOT EXISTS
            (
                SELECT 'X'
                FROM dbo.tbl_affiliate a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                        ON b.AgentId = a.AgentId
                           AND b.RoleType = 6
                           AND b.Status = 'A'
            )
            BEGIN
                SELECT 1 Code,
                       'Invalid request' Message;
                RETURN;
            END;

            UPDATE dbo.tbl_users
            SET Status = 'B',
                ActionUser = @ActionUser,
                ActionIP = @ActionIP,
                ActionPlatform = @ActionPlatform,
                ActionDate = GETDATE()
            WHERE AgentId = @AgentId
                  AND RoleType = 6
                  AND Status = 'A';

            SELECT 0 Code,
                   'Affiliate status updated successfully' Message;
            RETURN;
        END;
        ELSE
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;
    END;

	ELSE IF @Flag = 'grccl' --get referral converted customer
	BEGIN
		
		IF @SearchField IS NOT NULL
		BEGIN
			SET @SQLParameter += ' AND a.NickName LIKE ''%' + @SearchField + '%''';
		END

		IF ISNULL(@AffiliateId, '') <> ''
		BEGIN
			SET @SQLParameter += ' AND d.AgentId=' +  @AffiliateId
		END

		IF ISNULL(@FilterDate, '') <> ''
		BEGIN
			SET @SQLParameter += ' AND FORMAT(b.CreatedDate,''yyyy-MM-dd'') = ''' +  FORMAT(@FilterDate,'yyyy-MM-dd') + ''''; 
		END


		
		SET @SQLString = 'SELECT a.AgentId AS CustomerId,
			   b.ReferId AS ReferCode,
			   a.ProfileImage AS CustomerImage,
			   ISNULL(CONCAT(ISNULL(a.FirstName, ''''), '' '', ISNULL(a.LastName, '''')), ''-'') AS CustomerFullName,
			   a.NickName AS CustomerUserName,
			   FORMAT(b.CreatedDate, ''MMM dd, yyyy'') AS CustomerConvertedDate,
			   d.AgentId AS AffiliateId,
			   ISNULL(CONCAT(ISNULL(d.FirstName, ''''), '' '', ISNULL(d.LastName, '''')), ''-'') AS AffiliateFullName,
			   ISNULL(c.CommissionAmount, 0) AS AffiliateAmount,
			  d.ProfileImage AS AffiliateImage
		FROM dbo.tbl_customer a WITH (NOLOCK)
		INNER JOIN dbo.tbl_customer_refer_detail b WITH (NOLOCK) ON b.CustomerId = a.AgentId
			AND ISNULL(b.Status, '''') = ''A''
		INNER JOIN dbo.tbl_affiliate_referral_detail c WITH (NOLOCK) ON c.ReferDetailId = b.ReferDetailId
			AND c.CustomerId = b.CustomerId
			AND c.AffiliateId = b.AffiliateId
			AND ISNULL(c.Status, '''') NOT IN (''D'')
		INNER JOIN dbo.tbl_affiliate d WITH (NOLOCK) ON d.AgentId = b.AffiliateId WHERE 1 = 1' + @SQLParameter;
		PRINT(@SQLString);
		EXEC(@SQLString);
		RETURN;
	END
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

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
    (@ErrorDesc, 'sproc_admin_affiliate_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL',
     'sproc_admin_affiliate_management', GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
GO


