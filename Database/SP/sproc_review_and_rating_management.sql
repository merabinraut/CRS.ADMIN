USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_review_and_rating_management]    Script Date: 6/7/2024 11:28:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_review_and_rating_management] @Flag VARCHAR(10)
	,@ReservationId VARCHAR(10) = NULL
	,@CustomerId VARCHAR(10) = NULL
	,@ClubId VARCHAR(10) = NULL
	,@HostList VARCHAR(MAX) = NULL
	,@RemarkList VARCHAR(MAX) = NULL
	,@RatingScale INT = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	,@MVPHostId VARCHAR(10) = NULL
	,@DichotomousList VARCHAR(MAX) = NULL
AS
DECLARE @Sno VARCHAR(10) = NULL
	,@Sno2 VARCHAR(10) = NULL
	,@StringSQL NVARCHAR(MAX) = NULL
	,@TransactionName VARCHAR(200)
	,@ErrorDesc VARCHAR(MAX)
	,@CustomerName NVARCHAR(200) = NULL;

BEGIN TRY
	IF @Flag = 'grd' --get reservation details
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_customer A WITH (NOLOCK)
				INNER JOIN dbo.tbl_users B WITH (NOLOCK) ON B.AgentId = A.AgentId
					AND B.RoleType = '3'
					AND ISNULL(B.STATUS, '') = 'A'
				WHERE B.AgentId = @CustomerId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid customer' Message;

			RETURN;
		END;

		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_reservation_detail A WITH (NOLOCK)
				INNER JOIN dbo.tbl_reservation_plan_detail B WITH (NOLOCK) ON B.ReservationId = A.ReservationId
				INNER JOIN dbo.tbl_reservation_host_detail C WITH (NOLOCK) ON C.ReservationId = A.ReservationId
				INNER JOIN dbo.tbl_club_details d WITH (NOLOCK) ON d.AgentId = A.ClubId
					AND ISNULL(d.STATUS, '') = 'A'
				LEFT JOIN dbo.tbl_static_data E WITH (NOLOCK) ON E.StaticDataType = 10
					AND E.StaticDataValue = A.PaymentType
					AND ISNULL(E.STATUS, '') = 'A'
				LEFT JOIN dbo.tbl_location F WITH (NOLOCK) ON F.LocationId = d.LocationId
					AND ISNULL(F.STATUS, '') = 'A'
				WHERE A.ReservationId = @ReservationId
					AND A.CustomerId = @CustomerId
					AND ISNULL(A.TransactionStatus, '') IN ('S')
				)
		BEGIN
			SELECT 1 Code
				,'Invalid request' Message;

			RETURN;
		END;

		IF EXISTS (
				SELECT 'X'
				FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
				INNER JOIN dbo.tbl_review_and_rating b WITH (NOLOCK) ON b.ReservationId = a.ReservationId
					AND b.CustomerId = a.CustomerId
					AND ISNULL(b.STATUS, '') <> 'D'
				WHERE a.CustomerId = @CustomerId
					AND a.ReservationId = @ReservationId
				)
		BEGIN
			SELECT 1 Code
				,N'クラブはすでにレビューされました。' MESSAGE;

			--'Review already exist for the given  customer reservation' MESSAGE;
			RETURN;
		END;

		SELECT 0 Code
			,'Success' Message
			,A.CustomerId
			,A.ReservationId
			,A.VisitDate AS ReservationDate
			,
			--CONVERT(DATE, CONCAT(YEAR(GETDATE()), '-', CAST(A.VisitDate AS VARCHAR)), 120) AS ReservationDate,
			A.VisitTime AS ReservationTime
			,CONCAT (
				B.PlanName
				,' (¥'
				,CAST(ISNULL(B.Price, 0) AS INT)
				,')'
				) AS Price
			,ISNULL(A.NoOfPeople, 0) AS NoOfPeople
			,0 AS UsedPoint
			,CONCAT (
				'¥'
				,CAST(ISNULL((B.Price * A.NoOfPeople), 0) AS INT)
				) AS TotalAmount
			,
			--ISNULL(E.StaticDataLabel, '-') AS PaymentMethod,
			CASE 
				WHEN A.PaymentType = 0
					THEN '-'
				ELSE E.AdditionalValue1
				END AS PaymentMethod
			,d.AgentId AS ClubId
			,d.Logo AS ClubLogo
			,d.ClubName1 AS ClubNameEnglish
			,d.ClubName2 AS ClubNameJapanese
			,F.LocationName AS ClubLocationName
		FROM dbo.tbl_reservation_detail A WITH (NOLOCK)
		INNER JOIN dbo.tbl_reservation_plan_detail B WITH (NOLOCK) ON B.ReservationId = A.ReservationId
		--INNER JOIN dbo.tbl_reservation_host_detail C WITH (NOLOCK)
		--    ON C.ReservationId = A.ReservationId
		INNER JOIN dbo.tbl_club_details d WITH (NOLOCK) ON d.AgentId = A.ClubId
			AND ISNULL(d.STATUS, '') = 'A'
		LEFT JOIN dbo.tbl_static_data E WITH (NOLOCK) ON E.StaticDataType = 10
			AND E.StaticDataValue = A.PaymentType
			AND ISNULL(E.STATUS, '') = 'A'
		LEFT JOIN dbo.tbl_location F WITH (NOLOCK) ON F.LocationId = d.LocationId
			AND ISNULL(F.STATUS, '') = 'A'
		WHERE A.ReservationId = @ReservationId
			AND A.CustomerId = @CustomerId
			AND ISNULL(A.TransactionStatus, '') IN ('S');

		RETURN;
	END;
	ELSE IF @Flag = 'gchl' --get club host list
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_club_details A WITH (NOLOCK)
				INNER JOIN dbo.tbl_host_details B WITH (NOLOCK) ON B.AgentId = A.AgentId
					AND ISNULL(A.STATUS, '') = 'A'
					AND ISNULL(B.STATUS, '') = 'A'
				LEFT JOIN dbo.tbl_static_data C WITH (NOLOCK) ON C.StaticDataType = 12
					AND C.StaticDataValue = B.PreviousOccupation
					AND ISNULL(C.STATUS, '') = 'A'
				WHERE A.AgentId = @ClubId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid club' Message;

			RETURN;
		END;

		SELECT 0 Code
			,'Success' Message
			,A.AgentId AS ClubId
			,B.HostId
			,B.HostName
			,ISNULL(B.Rank, '') AS HostRank
			,ISNULL(c.AdditionalValue1, '') AS HostPosition
			,
			--(
			--    SELECT TOP 1
			--           ISNULL(d.ImagePath, '')
			--    FROM dbo.tbl_gallery d WITH (NOLOCK)
			--    WHERE d.AgentId = B.HostId
			--          AND d.RoleId = 5
			--          AND ISNULL(d.Status, '') = 'A'
			--    ORDER BY Sno DESC
			--) AS HostImage,
			B.Thumbnail AS HostImage
			,B.HostNameJapanese
		FROM dbo.tbl_club_details A WITH (NOLOCK)
		INNER JOIN dbo.tbl_host_details B WITH (NOLOCK) ON B.AgentId = A.AgentId
			AND ISNULL(A.STATUS, '') = 'A'
			AND ISNULL(B.STATUS, '') = 'A'
		LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK) ON c.StaticDataType = 48
			AND c.StaticDataValue = B.Position
		WHERE A.AgentId = @ClubId
		ORDER BY B.Rank
			,B.HostName ASC;

		RETURN;
	END;
	ELSE IF @Flag = 'grrl' --get review remark list
	BEGIN
		SELECT B.StaticDataValue AS RemarkId
			,B.StaticDataLabel AS RemarkLabelEnglish
			,B.AdditionalValue1 AS RemarkLabelJapanese
			,B.AdditionalValue2 AS RemarkTypeEnglish
			,B.AdditionalValue3 AS RemarkTypeJapanese
		FROM dbo.tbl_static_data_type A WITH (NOLOCK)
		INNER JOIN dbo.tbl_static_data B WITH (NOLOCK) ON B.StaticDataType = A.StaticDataType
			AND ISNULL(A.STATUS, '') = 'A'
			AND ISNULL(B.STATUS, '') = 'A'
		WHERE A.StaticDataType = 24
		ORDER BY NEWID();

		RETURN;
	END;
	ELSE IF @Flag = 'grdql' --get review dichotomous question list
	BEGIN
		SELECT B.StaticDataValue AS RemarkId
			,B.StaticDataLabel AS RemarkLabelEnglish
			,B.AdditionalValue1 AS RemarkLabelJapanese
		FROM dbo.tbl_static_data_type A WITH (NOLOCK)
		INNER JOIN dbo.tbl_static_data B WITH (NOLOCK) ON B.StaticDataType = A.StaticDataType
			AND ISNULL(A.STATUS, '') = 'A'
			AND ISNULL(B.STATUS, '') = 'A'
		WHERE A.StaticDataType = 25
		ORDER BY NEWID();

		RETURN;
	END;
	ELSE IF @Flag = 'gdal' --get dichotomous answer list
	BEGIN
		SELECT B.StaticDataValue AS RemarkId
			,B.StaticDataLabel AS RemarkLabelEnglish
			,B.AdditionalValue1 AS RemarkLabelJapanese
		FROM dbo.tbl_static_data_type A WITH (NOLOCK)
		INNER JOIN dbo.tbl_static_data B WITH (NOLOCK) ON B.StaticDataType = A.StaticDataType
			AND ISNULL(A.STATUS, '') = 'A'
			AND ISNULL(B.STATUS, '') = 'A'
		WHERE A.StaticDataType = 26
		ORDER BY B.StaticDataValue;

		RETURN;
	END;
	ELSE IF @Flag = 'mr' --manage review and rating
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_customer A WITH (NOLOCK)
				INNER JOIN dbo.tbl_users B WITH (NOLOCK) ON B.AgentId = A.AgentId
					AND ISNULL(B.STATUS, '') = 'A'
				WHERE B.AgentId = @CustomerId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid customer' Message;

			RETURN;
		END;

		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_customer A WITH (NOLOCK)
				INNER JOIN dbo.tbl_reservation_detail B WITH (NOLOCK) ON B.CustomerId = A.AgentId
					AND ISNULL(B.TransactionStatus, '') IN ('S')
				INNER JOIN dbo.tbl_users C WITH (NOLOCK) ON C.AgentId = A.AgentId
					AND C.RoleType = 3
				WHERE A.AgentId = @CustomerId
					AND B.ReservationId = @ReservationId
					AND B.ClubId = @ClubId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid request' Message;

			RETURN;
		END;

		IF EXISTS (
				SELECT 'X'
				FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
				INNER JOIN dbo.tbl_review_and_rating b WITH (NOLOCK) ON b.ReservationId = a.ReservationId
					AND b.CustomerId = a.CustomerId
					AND ISNULL(b.STATUS, '') <> 'D'
				WHERE a.CustomerId = @CustomerId
					AND a.ReservationId = @ReservationId
				)
		BEGIN
			SELECT 1 Code
				,N'クラブはすでにレビューされました。' MESSAGE;

			--'Review already exist for the given customer reservation' Message;
			RETURN;
		END;

		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_club_details a WITH (NOLOCK)
				INNER JOIN dbo.tbl_host_details b WITH (NOLOCK) ON b.AgentId = a.AgentId
					AND b.HostId IN (
						SELECT *
						FROM STRING_SPLIT(ISNULL(@HostList, ''), ',')
						)
				)
		BEGIN
			SELECT 1 Code
				,'Invalid host details' Message;

			RETURN;
		END;

		IF NOT EXISTS (
				SELECT 'X'
				FROM dbo.tbl_club_details a WITH (NOLOCK)
				INNER JOIN dbo.tbl_host_details b WITH (NOLOCK) ON b.AgentId = a.AgentId
					AND b.HostId = @MVPHostId
				)
		BEGIN
			SELECT 1 Code
				,'Invalid MVP host details' Message;

			RETURN;
		END;

		SET @TransactionName = 'Function_mr';

		BEGIN TRANSACTION @TransactionName;

		CREATE TABLE #temp_mr_1 (Value VARCHAR(MAX));

		INSERT INTO #temp_mr_1 (Value)
		SELECT *
		FROM STRING_SPLIT(ISNULL(@DichotomousList, ''), ',');

		CREATE TABLE #temp_mr_2 (
			QuestionId VARCHAR(10)
			,AnswerId VARCHAR(10)
			);

		INSERT INTO #temp_mr_2 (
			QuestionId
			,AnswerId
			)
		SELECT SUBSTRING(Value, 1, CHARINDEX(':', Value) - 1)
			,SUBSTRING(Value, CHARINDEX(':', Value) + 1, LEN(Value) - CHARINDEX(':', Value))
		FROM #temp_mr_1;

		INSERT INTO dbo.tbl_review_and_rating (
			ReservationId
			,CustomerId
			,ClubId
			,HostList
			,MVPHostId
			,RemarkList
			,RatingScale
			,STATUS
			,ActionUser
			,ActionDate
			,ActionIP
			,ActionPlatform
			)
		VALUES (
			@ReservationId
			,-- ReservationId - bigint
			@CustomerId
			,-- CustomerId - bigint
			@ClubId
			,-- ClubId - bigint
			@HostList
			,-- HostList - varchar(max)
			@MVPHostId
			,--- MVPHostId
			@RemarkList
			,-- RemarkList - varchar(max)
			@RatingScale
			,-- RatingScale - int
			'A'
			,@ActionUser
			,-- ActionUser - varchar(200)
			GETDATE()
			,-- ActionDate - bigint
			@ActionIP
			,-- ActionIP - varchar(20)
			@ActionPlatform -- ActionPlatform - varchar(20)
			);

		SET @Sno = SCOPE_IDENTITY();

		UPDATE dbo.tbl_review_and_rating
		SET ReviewId = @Sno
		WHERE Sno = @Sno;

		SET @StringSQL = N'
			INSERT INTO dbo.tbl_review_and_rating_qna
			(
				ReviewId,
				DichotomousQuestionId,
				DichotomousAnswerId,
				ActionUser,
				ActionDate,
				ActionIP,
				ActionPlatform
			)
		';
		SET @StringSQL += N'SELECT ' + CAST(@Sno AS VARCHAR) + N', a.QuestionId, a.AnswerId,''' + CAST(@ActionUser AS VARCHAR) + N''', GETDATE(),''' + @ActionIP + N''',''' + @ActionPlatform + N'''
		FROM #temp_mr_2 a WITH (NOLOCK);';

		PRINT (@StringSQL);

		EXEC (@StringSQL);

		DROP TABLE #temp_mr_1;

		DROP TABLE #temp_mr_2;

		UPDATE dbo.tbl_review_and_rating_qna
		SET QnaId = Sno
		WHERE ReviewId = @Sno;

		SELECT 0 Code
			,N'成功したレビュー' Message;

		--'Review successful' Message;
		SELECT @CustomerName = a.NickName
		FROM dbo.tbl_customer a WITH (NOLOCK)
		INNER JOIN dbo.tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
			AND b.RoleType = 3
			AND ISNULL(b.STATUS, '') = 'A'
		WHERE a.AgentId = @CustomerId;

		INSERT INTO dbo.tbl_club_notification (
			ToAgentId
			,NotificationType
			,NotificationSubject
			,NotificationBody
			,NotificationStatus
			,NotificationReadStatus
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@ClubId
			,4 --Ratings Given By Customer
			,'Ratings Given By Customer'
			,CONCAT (
				'Our valued customer '
				,COALESCE(@CustomerName, '')
				,' has awarded a '
				,COALESCE(CAST(@RatingScale AS VARCHAR(MAX)), '')
				,' star rating!'
				)
			,'A'
			,'P'
			,0
			,GETDATE()
			)

		SET @Sno2 = SCOPE_IDENTITY();

		UPDATE dbo.tbl_club_notification
		SET notificationId = @Sno2
		WHERE Sno = @Sno2;

		COMMIT TRANSACTION @TransactionName;

		RETURN;
	END;
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION @TransactionName;

	SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

	INSERT INTO dbo.tbl_error_log (
		ErrorDesc
		,ErrorScript
		,QueryString
		,ErrorCategory
		,ErrorSource
		,ActionDate
		)
	VALUES (
		@ErrorDesc
		,'sproc_review_and_rating_management(Flag: ' + ISNULL(@Flag, '') + ')'
		,'SQL'
		,'SQL'
		,'sproc_review_and_rating_management'
		,GETDATE()
		);

	SELECT 1 Code
		,'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) MESSAGE;

	RETURN;
END CATCH;
GO


