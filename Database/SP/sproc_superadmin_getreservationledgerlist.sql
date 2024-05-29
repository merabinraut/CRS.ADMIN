USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_superadmin_getreservationledgerlist]    Script Date: 5/29/2024 4:29:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROCEDURE [dbo].[sproc_superadmin_getreservationledgerlist]
    -- Add the parameters for the stored procedure here
    @Flag VARCHAR(10) = NULL,
    @Date DATETIME = NULL,
    @ClubId VARCHAR(10) = NULL,
    @SearchField NVARCHAR(200) = NULL,
    @Skip INT = 0,
    @Take INT = 10,
    @SearchFilter NVARCHAR(200) = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
DECLARE @SQL NVARCHAR(MAX) = N'',
        @SQLParameter NVARCHAR(MAX) = N'',
        @SQLParameter2 NVARCHAR(MAX) = N'';
BEGIN

    IF ISNULL(@Flag, '') = 'grll' --get reservation ledger list
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
                    CAST(SUM(b.TotalAdminPayableAmount) AS INT) AS AdminPayment,
                    SUM(a.NoOfPeople) AS TotalVisitors,
                    FORMAT(a.ActionDate, 'yyyy-MM-dd') AS TransactionDate
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                    INNER JOIN dbo.tbl_reservation_transaction_detail b WITH (NOLOCK)
                    ON b.ReservationId = a.ReservationId
                WHERE 1 = 1 AND a.TransactionStatus <> 'D'
                    AND (@ClubId IS NULL OR a.ClubId = @ClubId)
                    AND ( @Date IS NULL OR FORMAT(a.ActionDate,'yyyy-MM-dd') = FORMAT(@Date, 'yyyy-MM-dd'))
                    AND ((@FromDate IS NULL OR @ToDate IS NULL) OR (FORMAT(a.ActionDate,'yyyy-MM-dd') BETWEEN FORMAT(@FromDate, 'yyyy-MM-dd') AND FORMAT(@ToDate, 'yyyy-MM-dd')))
                GROUP BY a.ClubId,
                      FORMAT(a.ActionDate, 'yyyy-MM-dd')
            )
        SELECT a.ClubId,
            b.ClubName1 AS ClubName,
            b.Logo AS ClubLogo,
            c.StaticDataLabel AS ClubCategory,
            a.AdminPayment,
            a.TransactionDate,
            FORMAT(CAST(a.TransactionDate AS DATE), 'MMM dd, yyyy') AS TransactionFormattedDate,
            a.TotalVisitors,
            ROW_NUMBER() OVER (ORDER BY b.ClubName1 ASC) AS SNO,
            COUNT(a.ClubId) OVER() AS TotalRecords
        FROM CTE a WITH (NOLOCK)
            INNER JOIN dbo.tbl_club_details b WITH (NOLOCK)
            ON b.AgentId = a.ClubId
            LEFT JOIN dbo.tbl_tag_detail b2 WITH (NOLOCK)
            ON b2.ClubId = b.AgentId
                AND ISNULL(b2.Tag3Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
            ON c.StaticDataType = 17
                AND c.StaticDataValue = b2.Tag3CategoryName
                AND ISNULL(c.Status, '') = 'A'
        WHERE 1 =1
            AND (@SearchFilter IS NULL OR b.ClubName1 LIKE '%'+ @SearchFilter +'%')
        ORDER BY b.ClubName1 ASC
			OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
    END;

    ELSE IF @Flag = 'grld' --get reservation ledger detail
    BEGIN
        IF @Date IS NULL
        BEGIN
            SELECT 1 Code,
                'Invalid date' Message;
            RETURN;
        END;
        ELSE
        BEGIN
            SET @SQLParameter = N' AND FORMAT(a.ActionDate,''yyyy-MM-dd'') = ''' + FORMAT(@Date, 'yyyy-MM-dd') + N'''';
        END;

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
        ELSE
        BEGIN
            SET @SQLParameter += N' AND a.ClubId=' + @ClubId;
        END;

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
            CASE
				   WHEN ISNULL(a.OTPVerificationStatus, '') = 'A' THEN
					   'Verified'
				   WHEN ISNULL(a.OTPVerificationStatus, '') = 'P' THEN
					   'Pending'
				   ELSE
					   'Rejected'
			   END AS ClubVerification,
            CASE
				   WHEN ISNULL(a.TransactionStatus, '') = 'S' THEN
					   'Success'
			       WHEN ISNULL(a.TransactionStatus, '') = 'R' THEN
					   'Rejected'
				  WHEN ISNULL(a.TransactionStatus, '') = 'C' THEN
					   'Cancelled'
				   ELSE
					   'Pending'
			   END AS TransactionStatus,
            a.ClubId,
            ROW_NUMBER() OVER (ORDER BY a.ActionDate DESC) AS SNO,
            COUNT(a.ClubId) OVER() AS TotalRecords,
            g.StaticDataLabel AdminRemarks,
            CAST(b.PlanAmount AS INT) PlanAmount,
            CAST(b.TotalPlanAmount AS INT) TotalPlanAmount,
            CAST(b.TotalClubPlanAmount AS INT) TotalClubPlanAmount,
            CAST(b.AdminPlanCommissionAmount AS INT) AdminPlanCommissionAmount,
            CAST(b.TotalAdminPlanCommissionAmount AS INT) TotalAdminPlanCommissionAmount,
            CAST(b.AdminCommissionAmount AS INT) AdminCommissionAmount,
            CAST(b.TotalAdminCommissionAmount AS INT) TotalAdminCommissionAmount,
            CAST(b.TotalAdminPayableAmount AS INT) TotalAdminPayableAmount
        FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_reservation_transaction_detail b WITH (NOLOCK)
            ON b.ReservationId = a.ReservationId
            --AND ISNULL(a.PaymentType, '') <> ''
            INNER JOIN dbo.tbl_customer c WITH (NOLOCK) ON c.AgentId = a.CustomerId
            INNER JOIN dbo.tbl_reservation_plan_detail d WITH (NOLOCK) ON d.ReservationId = a.ReservationId
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK) ON e.StaticDataType = 10
                AND e.StaticDataValue = a.PaymentType
                AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data f WITH (NOLOCK) ON f.StaticDataType = 30
                AND f.StaticDataValue = b.ReservationType
                AND ISNULL(f.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data g WITH (NOLOCK) ON g.StaticDataType = 41
                AND g.StaticDataValue = a.ManualRemarkId
                AND ISNULL(g.Status, '') = 'A'
        WHERE 1 = 1 AND a.TransactionStatus <> 'D'
            AND (@ClubId IS NULL OR a.ClubId = @ClubId)
            AND (@Date IS NULL OR FORMAT(a.ActionDate,'yyyy-MM-dd') = FORMAT(@Date, 'yyyy-MM-dd'))
            AND ((@FromDate IS NULL OR @ToDate IS NULL) OR (FORMAT(a.ActionDate,'yyyy-MM-dd') BETWEEN FORMAT(@FromDate, 'yyyy-MM-dd') AND FORMAT(@ToDate, 'yyyy-MM-dd')))
            AND (@SearchFilter IS NULL OR c.NickName LIKE '%' + @SearchFilter+'%')
        ORDER BY a.ActionDate DESC
		 OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
    END;
END;
GO


