USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_affiliate_profile_management]    Script Date: 13/12/2023 23:00:53 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO



ALTER PROC [dbo].[sproc_affiliate_profile_management]
    @Flag VARCHAR(10),
    @AgentId VARCHAR(10) = NULL,
    @ProfileImage VARCHAR(500) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @Password VARCHAR(16) = NULL,
    @NewPassword VARCHAR(16) = NULL,
    @SnsId VARCHAR(10) = NULL
AS
DECLARE @ReferralLink NVARCHAR(512) = N'http://hoslog.jp/Home/Register?ReferCode=';
BEGIN
    IF @Flag = 'gap' --get affiliate profile
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 6
                       AND ISNULL(b.Status, '') = 'A'
            WHERE a.AgentId = @AgentId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SELECT a.AgentId,
               'Affiliate' AS Role,
               a.FirstName,
               a.LastName,
               c.StaticDataLabel AS Gender,
               a.MobileNumber,
               a.DOB,
               d.WebsiteLink,
               d.TiktokLink,
               d.TwitterLink,
               d.InstagramLink,
               d.FacebookLink
        FROM dbo.tbl_affiliate a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 6
                   AND ISNULL(b.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = 2
                   AND c.StaticDataValue = a.Gender
                   AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN dbo.tbl_website_details d WITH (NOLOCK)
                ON d.AgentId = a.AgentId
                   AND d.RoleId = 6
        WHERE a.AgentId = @AgentId;

        RETURN;
    END;

    ELSE IF @Flag = 'uap' --update affiliate profile image
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 6
                       AND ISNULL(b.Status, '') = 'A'
            WHERE a.AgentId = @AgentId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_affiliate
        SET ProfileImage = ISNULL(@ProfileImage, ProfileImage),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionDate = GETDATE(),
            ActionPlatform = @ActionPlatform
        WHERE AgentId = @AgentId;

        SELECT 0 Code,
               'Profile image updated successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'map' --manage affiliate password
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 6
                       AND b.AgentId = @AgentId
                       AND ISNULL(b.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 6
                       AND b.AgentId = @AgentId
                       AND ISNULL(b.Status, '') = 'A'
                       AND PWDCOMPARE(@Password, b.Password) = 1
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid old password' Message;
            RETURN;
        END;

        IF @Password = @NewPassword
        BEGIN
            SELECT 1 Code,
                   'Old password and new password cannot be same' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_users
        SET Password = PWDENCRYPT(@NewPassword),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionDate = GETDATE(),
            ActionPlatform = @ActionPlatform,
            Session = NULL
        WHERE RoleType = 6
              AND AgentId = @AgentId
              AND ISNULL(Status, '') = 'A'
              AND PWDCOMPARE(@Password, Password) = 1;

        SELECT 0 Code,
               'Password updated successfully' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'gsns' --get social network sites
    BEGIN
        SELECT a.StaticDataValue AS Id,
               a.StaticDataLabel AS EnglishLabel,
               a.AdditionalValue1 AS JapaneseLabel
        FROM dbo.tbl_static_data a WITH (NOLOCK)
        WHERE a.StaticDataType = 28
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY a.StaticDataLabel ASC;
        RETURN;
    END;

    ELSE IF @Flag = 'grlvsns' --get refer link via social network sites
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_affiliate a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 6
                       AND ISNULL(b.Status, '') = 'A'
                INNER JOIN dbo.tbl_affiliate_refer_details c WITH (NOLOCK)
                    ON c.AffiliateId = a.AgentId
                       AND ISNULL(c.Status, '') = 'A'
                INNER JOIN dbo.tbl_static_data d WITH (NOLOCK)
                    ON d.StaticDataType = 28
                       AND d.StaticDataValue = c.SnsId
                       AND ISNULL(d.Status, '') = 'A'
            WHERE a.AgentId = @AgentId
                  AND d.StaticDataValue = @SnsId
                  AND c.ReferId IS NOT NULL
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        SELECT 0 Code,
               'Success' Message,
               CONCAT(@ReferralLink, c.ReferId) AS ReferralLink
        FROM dbo.tbl_affiliate a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 6
                   AND ISNULL(b.Status, '') = 'A'
            INNER JOIN dbo.tbl_affiliate_refer_details c WITH (NOLOCK)
                ON c.AffiliateId = a.AgentId
                   AND ISNULL(c.Status, '') = 'A'
            INNER JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 28
                   AND d.StaticDataValue = c.SnsId
                   AND ISNULL(d.Status, '') = 'A'
        WHERE a.AgentId = @AgentId
              AND d.StaticDataValue = @SnsId
              AND c.ReferId IS NOT NULL;
        RETURN;
    END;
END;
GO


