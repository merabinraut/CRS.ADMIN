ALTER PROC sproc_admin_payment_management @Flag VARCHAR(20),
@Date DATETIME = '2023-11-18',
@ClubId VARCHAR(10) = NULL,
@SearchField VARCHAR(200) = NULL,
@PaymentStatus VARCHAR(10) = NULL
AS
DECLARE @SQL VARCHAR(MAX) = '',
		@SQLParameter VARCHAR(MAX) = '',
		@SQLParameter2 VARCHAR(MAX) = '';
BEGIN
	IF ISNULL(@Flag, '') = 'gpl' --get payment log
	BEGIN
		IF @Date IS NULL SET @Date = GETDATE();

		IF @Date IS NOT NULL
		BEGIN
			SET @SQLParameter += ' AND FORMAT(a.ActionDate,''yyyy-MM-dd'') = ''' +  FORMAT(@Date,'yyyy-MM-dd') + ''''; 
		END

		IF @ClubId IS NOT NULL
		BEGIN
			SET @SQLParameter += ' AND a.ClubId=' + @ClubId;
		END

		IF @SearchField IS NOT NULL
		BEGIN
			SET @SQLParameter2 += ' AND b.ClubName1 LIKE ''%' + @SearchField + '%''';
		END

		SET @SQL = '
		WITH CTE AS (
		SELECT a.ClubId,
			  SUM(ISNULL(b.Price, 0) * a.NoOfPeople) AS TotalAmount,
			  SUM(ISNULL(a.CommissionAmount, 0)) AS TotalCommissionAmount,
			  FORMAT(a.ActionDate,''yyyy-MM-dd'') AS TransactionDate
		FROM tbl_reservation_detail a WITH (NOLOCK)
		INNER JOIN tbl_reservation_plan_detail b WITH (NOLOCK) ON b.ReservationId = a.ReservationId		
		WHERE 1 = 1 ' + @SQLParameter + '
		GROUP BY a.ClubId, FORMAT(a.ActionDate,''yyyy-MM-dd'')
		)

		SELECT a.*,
			   b.ClubName1 AS ClubName,
			   b.Logo AS ClubLogo,
				''Platinum'' AS ClubCategory,
			   b.MobileNumber AS ClubMobileNumber,
			  CONCAT(b.InputHouseNo, '' , '', b.InputStreet, '' , '', b.InputCity) AS Location,
			  ''Processing'' AS PaymentStatus,
			  (a.TotalAmount + a.TotalCommissionAmount) AS GrandTotal
		FROM CTE a WITH (NOLOCK)
		INNER JOIN tbl_club_details b WITH (NOLOCK) ON b.AgentId = a.ClubId
		WHERE 1 = 1
		' + @SQLParameter2

		PRINT(@SQL);
		EXEC(@SQL);

		RETURN;
	END

	ELSE IF ISNULL(@Flag, '') = 'gpld' --get payment ledger detail
	BEGIN
		IF @Date IS NULL
		BEGIN
			SELECT 1 Code,
				   'Invalid date' Message;
			RETURN;
		END
		ELSE
		BEGIN
			SET @SQLParameter += ' AND FORMAT(a.ActionDate,''yyyy-MM-dd'') = ''' +  FORMAT(@Date,'yyyy-MM-dd') + ''''; 
		END

		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_club_details a WITH (NOLOCK)
			WHERE a.AgentId = @ClubId
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid club details' Message;
			RETURN;
		END
		ELSE
		BEGIN
			SET @SQLParameter += ' AND a.ClubId=' +  @ClubId
		END

		IF @SearchField IS NOT NULL
		BEGIN
			SET @SQLParameter += ' AND c.NickName LIKE ''%' + @SearchField + '%''';
		END

		SET @SQL = 'select 0 Code, ''Success'' Message, CONCAT(c.FirstName, '' '', c.LastName) AS CustomerName,
				   c.NickName AS CustomerNickName,
				   c.ProfileImage AS CustomerImage,
				   b.PlanName,
				   a.NoOfPeople,
				   a.VisitDate,
				   a.VisitTime,
				   d.StaticDataLabel AS PaymentType,
				   0 AS Commission,
				   0 AS AdminPayment
			FROM tbl_reservation_detail a WITH (NOLOCK)
			INNER JOIN tbl_reservation_plan_detail b WITH (NOLOCK) ON b.ReservationId = a.ReservationId
			INNER JOIN tbl_customer c WITH (NOLOCK) ON c.AgentId = a.CustomerId 
			INNER JOIN tbl_static_data d WITH (NOLOCK) on d.StaticDataType = 10 AND d.StaticDataValue = a.PaymentType
			WHERE 1 = 1 ' + @SQLParameter;

		PRINT(@SQL);
		EXEC(@SQL);

		RETURN;
	END

	ELSE IF ISNULL(@Flag, '') = 'gpmo' --get payment management overview
	BEGIN
		SELECT 0 AS ReceivedPayments,
			   0 AS DuePayments,
			   0 AS AffiliatePayment
		RETURN;
	END
END
GO



