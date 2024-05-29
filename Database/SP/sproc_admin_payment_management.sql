USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_payment_management]    Script Date: 5/29/2024 4:25:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROC [dbo].[sproc_admin_payment_management]
    @Flag VARCHAR(20),
    @Date DATETIME = NULL,
    @ClubId VARCHAR(10) = NULL,
    @SearchField NVARCHAR(200) = NULL,
    @PaymentStatus VARCHAR(10) = NULL,
    @Skip INT = 0,
    @Take INT = 10,
    @SearchFilter NVARCHAR(200) = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @LocationId VARCHAR(10) = NULL
AS
DECLARE @SQL NVARCHAR(MAX) = N'',
        @SQLParameter NVARCHAR(MAX) = N'',
        @SQLParameter2 NVARCHAR(MAX) = N'';
BEGIN
    IF @Flag ='gpl'
	  BEGIN
        IF ISNULL(@FromDate, '') = ''
			BEGIN
            SELECT @FromDate = GETDATE(),
                @ToDate = GETDATE();
        END

        IF ISNULL(@ToDate, '') = ''
			BEGIN
            SET @ToDate = DATEADD(DAY, 1 , @FromDate);
        END

			;
        WITH
            CTE
            AS
            (
                SELECT a.ClubId,
                    CAST(SUM(b.TotalPlanAmount) AS INT) AS TotalPlanAmount,
                    CAST(SUM(b.TotalAdminPlanCommissionAmount) AS INT) AS TotalAdminPlanCommissionAmount,
                    CAST(SUM(b.TotalAdminCommissionAmount) AS INT) AS TotalAdminCommissionAmount,
                    CAST(SUM(b.TotalAdminPayableAmount) AS INT) AS GrandTotal,
                    FORMAT(a.ActionDate, 'yyyy-MM-dd') AS TransactionDate
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_reservation_transaction_detail b WITH (NOLOCK)
                    ON b.ReservationId = a.ReservationId
                WHERE 1 = 1 AND a.TransactionStatus IN ('A', 'S')
                    AND (@Date IS NULL OR FORMAT(a.ActionDate,'yyyy-MM-dd') = FORMAT(@Date, 'yyyy-MM-dd'))
                    AND ((@FromDate IS NULL OR @ToDate IS NULL) OR (FORMAT(a.ActionDate,'yyyy-MM-dd') BETWEEN FORMAT(@FromDate, 'yyyy-MM-dd') AND FORMAT(@ToDate, 'yyyy-MM-dd')))
                    AND (@ClubId IS NULL OR a.ClubId = @ClubId)
                GROUP BY a.ClubId,
						 FORMAT(a.ActionDate, 'yyyy-MM-dd')
            )
        SELECT a.ClubId,
            b.ClubName1 AS ClubName,
            b.Logo AS ClubLogo,
            c.StaticDataLabel AS ClubCategory,
            d.LocationName AS ClubLocation,
            a.TotalPlanAmount,
            a.TotalAdminPlanCommissionAmount,
            a.TotalAdminCommissionAmount,
            a.GrandTotal,
            a.TransactionDate,
            FORMAT(CAST(a.TransactionDate AS DATE), 'MMM dd, yyyy') AS TransactionFormattedDate,
            ROW_NUMBER() OVER (ORDER BY b.ClubName1 ASC) AS SNO,
            COUNT(a.ClubId) OVER() AS TotalRecords
        FROM CTE a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
            ON b.AgentId = a.ClubId
            LEFT JOIN dbo.tbl_tag_detail b2 WITH (NOLOCK) ON b2.ClubId = b.AgentId
                AND ISNULL(b2.Tag3Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
            ON c.StaticDataType = 17
                AND c.StaticDataValue = b2.Tag3CategoryName
                AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN dbo.tbl_location d WITH (NOLOCK)
            ON d.LocationId = b.LocationId
                AND ISNULL(d.Status, '') = 'A'
        WHERE 1 = 1
            AND (@SearchFilter IS NULL OR b.ClubName1 LIKE '%' + @SearchFilter+'%')
            AND (@LocationId IS NULL OR d.LocationId = @LocationId)
        order by b.ClubName1 ASC
			OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
    END
	  
    ELSE IF ISNULL(@Flag, '') = 'gpld' --get payment ledger detail
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
        FROM dbo.tbl_club_details a WITH (NOLOCK)
        WHERE a.AgentId = @ClubId
        )
        BEGIN
            SELECT 1 Code,
                'Invalid club details' Message;
            RETURN;
        END;

        IF ISNULL(@FromDate, '') = ''
	    BEGIN
            SELECT @FromDate = GETDATE(),
                @ToDate = GETDATE();
        END

        IF ISNULL(@ToDate, '') = ''
	    BEGIN
            SET @ToDate = DATEADD(DAY, 1 , @FromDate);
        END

        SELECT 0 Code,
            'Success' Message,
            CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
            c.NickName AS CustomerNickName,
            d.PlanName,
            a.NoOfPeople,
            a.VisitTime,
            a.VisitDate,
            e.StaticDataLabel AS PaymentType,
            c.ProfileImage AS CustomerImage,
            f.StaticDataLabel AS ReservationType,
            ROW_NUMBER() OVER (ORDER BY a.ActionDate DESC) AS SNO,
            COUNT(a.ClubId) OVER() AS TotalRecords
			  , CAST(b.PlanAmount AS INT) AS PlanAmount
			  , CAST(b.TotalPlanAmount AS INT) AS TotalPlanAmount
			  , CAST(b.TotalClubPlanAmount AS INT) AS TotalClubPlanAmount
			  , CAST(b.AdminPlanCommissionAmount AS INT) AS AdminPlanCommissionAmount
			  , CAST(b.TotalAdminPlanCommissionAmount AS INT) AS TotalAdminPlanCommissionAmount
			  , CAST(b.AdminCommissionAmount AS INT) AS AdminCommissionAmount
			  , CAST(b.TotalAdminCommissionAmount AS INT) AS TotalAdminCommissionAmount
			  , CAST(b.TotalAdminPayableAmount AS INT) AS TotalAdminPayableAmount
        FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_reservation_transaction_detail b WITH (NOLOCK)
            ON b.ReservationId = a.ReservationId
            INNER JOIN dbo.tbl_customer c WITH (NOLOCK) ON c.AgentId = a.CustomerId
            INNER JOIN dbo.tbl_reservation_plan_detail d WITH (NOLOCK) ON d.ReservationId = a.ReservationId
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK) ON e.StaticDataType = 10
                AND e.StaticDataValue = a.PaymentType
                AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data f WITH (NOLOCK) ON f.StaticDataType = 30
                AND f.StaticDataValue = b.ReservationType
                AND ISNULL(f.Status, '') = 'A'
        WHERE 1 = 1 AND a.TransactionStatus IN ('A', 'S')
            AND (@Date IS NULL OR FORMAT(a.ActionDate,'yyyy-MM-dd') = FORMAT(@Date, 'yyyy-MM-dd'))
            AND (@ClubId IS NULL OR a.ClubId= @ClubId)
            AND ((@FromDate IS NULL OR @ToDate IS NULL) OR (FORMAT(a.ActionDate,'yyyy-MM-dd') BETWEEN FORMAT(@FromDate, 'yyyy-MM-dd') AND FORMAT(@ToDate, 'yyyy-MM-dd')))
            AND (@SearchFilter IS NULL OR c.NickName LIKE '%'+@SearchFilter+'%')
        ORDER BY a.ActionDate DESC
		 OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;

        RETURN;
    END;

    ELSE IF ISNULL(@Flag, '') = 'gpmo' --get payment management overview
    BEGIN
        SELECT 0 AS ReceivedPayments,
            0 AS DuePayments,
            0 AS AffiliatePayment;
        RETURN;
    END;
END;
GO


