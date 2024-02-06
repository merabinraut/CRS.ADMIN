USE CRS;
GO

ALTER PROC [dbo].[sproc_host_identity_detail_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @HostId VARCHAR(10) = NULL,
    @IdentityType VARCHAR(10) = NULL,
    @IdentityValue VARCHAR(10) = NULL,
    @IdentityDescription NVARCHAR(512) = NULL,
    @ActionUser NVARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL
AS
BEGIN
    IF @Flag = 'mhid' --manage host identity detail
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 4
                       AND ISNULL(b.Status, '') = 'A'
                       AND ISNULL(a.Status, '') = 'A'
            WHERE a.AgentId = @ClubId
        )
        BEGIN
            SELECT '1' Code,
                   'Invalid club' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_host_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND a.HostId = @HostId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT '1' Code,
                   'Invalid host' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_static_data a WITH (NOLOCK)
                INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK)
                    ON b.StaticDataType = a.StaticDataType
                       AND ISNULL(a.Status, '') = 'A'
                       AND ISNULL(b.Status, '') = 'A'
            WHERE b.StaticDataType = @IdentityType
                  AND a.StaticDataValue = @IdentityValue
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid identity type' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_host_identity_details a WITH (NOLOCK)
            WHERE a.ClubId = @ClubId
                  AND a.HostId = @HostId
                  AND a.IdentityType = @IdentityType
                  AND a.IdentityValue = @IdentityValue
        )
        BEGIN
            INSERT INTO dbo.tbl_host_identity_details
            (
                ClubId,
                HostId,
                IdentityType,
                IdentityValue,
                IdentityDescription,
                CreatedBy,
                CreatedDate,
                CreatedIP,
                CreatedPlatform
            )
            VALUES
            (@ClubId, @HostId, @IdentityType, @IdentityValue, @IdentityDescription, @ActionUser, GETDATE(), @ActionIP,
             @ActionPlatform);
        END;
        ELSE
        BEGIN
            UPDATE dbo.tbl_host_identity_details
            SET IdentityDescription = ISNULL(@IdentityDescription, IdentityDescription),
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE(),
                UpdatedIP = @ActionIP,
                UpdatedPlatform = @ActionPlatform
            WHERE ClubId = @ClubId
                  AND HostId = @HostId
                  AND IdentityType = @IdentityType
                  AND IdentityValue = @IdentityValue;
        END;

        SELECT 0 Code,
               'Success' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'ghid' --get host identity detail
    BEGIN
        SELECT a.Sno AS Id
			  ,a.ClubId
			  ,a.HostId
			  ,a.IdentityType
			  ,a.IdentityValue
			  ,a.IdentityDescription
        FROM dbo.tbl_host_identity_details a WITH (NOLOCK)
        WHERE a.ClubId = @ClubId
              AND a.HostId = @HostId;

		RETURN;
    END;
END;
GO