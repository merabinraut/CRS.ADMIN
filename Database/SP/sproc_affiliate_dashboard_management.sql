ALTER PROC [dbo].[sproc_affiliate_dashboard_management]
    @Flag VARCHAR(10),
    @SearchField VARCHAR(200) = NULL,
    @AffiliateId VARCHAR(10) = NULL,
    @FilterDate DATETIME = NULL
AS
DECLARE @SQLString NVARCHAR(MAX) = N'',
        @SQLParameter NVARCHAR(MAX) = N'';
BEGIN
    IF @Flag = 'grci' --get referred customer information
    BEGIN
        IF @SearchField IS NOT NULL
        BEGIN
            SET @SQLParameter += N' AND a.NickName LIKE ''%' + @SearchField + N'%''';
        END;

        IF ISNULL(@AffiliateId, '') <> ''
        BEGIN
            SET @SQLParameter += N' AND d.AgentId=' + @AffiliateId;
        END;

        IF ISNULL(@FilterDate, '') <> ''
        BEGIN
            SET @SQLParameter += N' AND FORMAT(b.CreatedDate,''yyyy-MM-dd'') = ''' + FORMAT(@FilterDate, 'yyyy-MM-dd')
                                 + N'''';
        END;



        SET @SQLString
            = N'SELECT a.AgentId AS CustomerId,
			   b.ReferId AS ReferCode,
			   a.ProfileImage AS CustomerImage,
			   ISNULL(CONCAT(ISNULL(a.FirstName, ''''), '' '', ISNULL(a.LastName, '''')), ''-'') AS CustomerFullName,
			   a.NickName AS CustomerUserName,
			   FORMAT(b.CreatedDate, ''MMM dd, yyyy'') AS CustomerConvertedDate,
			   ISNULL(CONCAT(ISNULL(d.FirstName, ''''), '' '', ISNULL(d.LastName, '''')), ''-'') AS AffiliateFullName,
			   ISNULL(c.CommissionAmount, 0) AS AffiliateAmount
		FROM dbo.tbl_customer a WITH (NOLOCK)
		INNER JOIN dbo.tbl_customer_refer_detail b WITH (NOLOCK) ON b.CustomerId = a.AgentId
			AND ISNULL(b.Status, '''') = ''A''
		INNER JOIN dbo.tbl_affiliate_referral_detail c WITH (NOLOCK) ON c.ReferDetailId = b.ReferDetailId
			AND c.CustomerId = b.CustomerId
			AND c.AffiliateId = b.AffiliateId
			AND ISNULL(c.Status, '''') NOT IN (''D'')
		INNER JOIN dbo.tbl_affiliate d WITH (NOLOCK) ON d.AgentId = b.AffiliateId WHERE 1 = 1' + @SQLParameter;
        PRINT (@SQLString);
        EXEC (@SQLString);
        RETURN;
    END;
END;
GO
