USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_plan_management]    Script Date: 6/7/2024 12:29:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sproc_admin_plan_management] (
	@Flag VARCHAR(5)
	,@PlanId VARCHAR(10) = NULL
	,@PlanName NVARCHAR(200) = NULL
	,@PlanType VARCHAR(20) = NULL
	,@Time VARCHAR(20) = NULL
	,@Price DECIMAL(18, 2) = 0
	,@Liquor VARCHAR(20) = NULL
	,@Nomination INT = NULL
	,@Remarks NVARCHAR(400) = NULL
	,@PlanStatus VARCHAR(1) = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIp VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	,@PlanImage VARCHAR(512) = NULL
	,@PlanImage2 VARCHAR(512) = NULL
	,@ExtraField1 NVARCHAR(200) = NULL
	,@ExtraField2 NVARCHAR(200) = NULL
	,@ExtraField3 NVARCHAR(200) = NULL
	,@StaticType VARCHAR(10) = NULL
	,@Skip INT = 0
	,@Take INT = 10
	,@SearchFilter NVARCHAR(200) = NULL
	,@NoOfPeople INT = NULL
	,@PlanCategory VARCHAR(10) = NULL
	,@StrikePrice DECIMAL(18, 2) = NULL
	,@IsStrikeOut CHAR(1) = NULL
	)
AS
DECLARE @SQLString NVARCHAR(MAX) = N''
	,@SQLFilterParameter NVARCHAR(MAX) = N'';

BEGIN
	SET NOCOUNT ON;

	DECLARE @IsPlanActive BIT
		,-- 1: TRUE 0:FALSE
		@ReturnMessage VARCHAR(200) = NULL
		,@Sno BIGINT
		,@Sno2 BIGINT;

	IF @Flag = 's'
	BEGIN
		SELECT ROW_NUMBER() OVER (
				ORDER BY tp.PlanName ASC
				) AS SNO
			,Sno PlanId
			,PlanName
			,ISNULL(A.StaticDataLabel, '') PlanType
			,ISNULL(B.StaticDataLabel, '') PlanTime
			,CAST(ISNULL(tp.Price, 0) AS INT) AS Price
			,ISNULL(C.StaticDataLabel, '') Liquor
			,Nomination
			,Remarks
			,PlanStatus
			,tp.ActionUser
			,ActionIp
			,ActionPlatform
			,FORMAT(tp.ActionDate, 'MMM dd, yyyy HH:mm:ss') ActionDate
			,tp.PlanImage
			,tp.PlanImage2
			,tp.AdditionalValue1
			,tp.AdditionalValue2
			,tp.AdditionalValue3
			,COUNT(tp.Sno) OVER () AS TotalRecords
			,tp.NoOfPeople
			,tp.PlanCategory
		FROM dbo.tbl_plans tp WITH (NOLOCK)
		LEFT JOIN dbo.tbl_static_data A WITH (NOLOCK) ON tp.PlanType = A.StaticDataValue
			AND A.StaticDataType = 7
		LEFT JOIN dbo.tbl_static_data B WITH (NOLOCK) ON tp.PlanTime = B.StaticDataValue
			AND B.StaticDataType = 8
		LEFT JOIN dbo.tbl_static_data C WITH (NOLOCK) ON tp.Liquor = C.StaticDataValue
			AND C.StaticDataType = 9
		WHERE tp.PlanStatus IN ('A')
			AND (
				@SearchFilter IS NULL
				OR (tp.PlanName LIKE '%' + @SearchFilter + '%')
				)
		ORDER BY tp.PlanName ASC OFFSET @Skip ROWS

		FETCH NEXT @Take ROW ONLY;

		RETURN;
	END;

	IF @Flag = 'sd'
	BEGIN
		SELECT Sno PlanId
			,PlanName
			,tp.PlanType
			,tp.PlanTime
			,
			--ISNULL(A.StaticDataLabel, '') PlanType,
			--ISNULL(B.StaticDataLabel, '') PlanTime,
			Price
			,tp.Liquor
			,
			--ISNULL(C.StaticDataLabel, '') Liquor,
			Nomination
			,Remarks
			,CASE 
				WHEN PlanStatus = 'A'
					THEN 'Active'
				ELSE 'Blocked'
				END PlanStatus
			,tp.ActionUser
			,ActionIp
			,ActionPlatform
			,tp.ActionDate
			,tp.PlanImage
			,tp.AdditionalValue1
			,tp.AdditionalValue2
			,tp.AdditionalValue3
			,tp.NoOfPeople
			,tp.PlanCategory
			,tp.PlanImage2
			,tp.StrikePrice
			,tp.IsStrikeOut
		FROM dbo.tbl_plans tp WITH (NOLOCK)
		--LEFT JOIN dbo.tbl_static_data A WITH (NOLOCK)
		--    ON tp.PlanType = A.StaticDataValue
		--       AND A.StaticDataType = 7
		--LEFT JOIN dbo.tbl_static_data B WITH (NOLOCK)
		--    ON tp.PlanTime = B.StaticDataValue
		--       AND B.StaticDataType = 8
		--LEFT JOIN dbo.tbl_static_data C WITH (NOLOCK)
		--    ON tp.Liquor = C.StaticDataValue
		--       AND C.StaticDataType = 9
		WHERE Sno = @PlanId
			AND tp.PlanStatus NOT IN ('D');

		RETURN;
	END;

	IF @Flag = 'i'
	BEGIN
		INSERT INTO dbo.tbl_plans (
			PlanName
			,PlanType
			,PlanTime
			,Price
			,Liquor
			,Nomination
			,Remarks
			,PlanStatus
			,ActionUser
			,ActionIp
			,ActionPlatform
			,ActionDate
			,PlanImage
			,AdditionalValue1
			,AdditionalValue2
			,AdditionalValue3
			,PlanImage2
			,NoOfPeople
			,PlanCategory
			,StrikePrice
			,IsStrikeOut
			)
		VALUES (
			@PlanName
			,@PlanType
			,@Time
			,@Price
			,@Liquor
			,@Nomination
			,@Remarks
			,'A'
			,@ActionUser
			,@ActionIp
			,@ActionPlatform
			,GETDATE()
			,@PlanImage
			,@ExtraField1
			,@ExtraField2
			,@ExtraField3
			,@PlanImage2
			,@NoOfPeople
			,@PlanCategory
			,@StrikePrice
			,@IsStrikeOut
			);

		SELECT @Sno = SCOPE_IDENTITY();

		UPDATE dbo.tbl_plans
		SET PlanId = @Sno
		WHERE Sno = @Sno;

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
			0
			,5 --System Adds Plan
			,'New Plan'
			,'We are excited to announce that a new plan, ' + ISNULL(@PlanName, '-') + ' has been successfully added to the system.'
			,'A'
			,'A'
			,0
			,GETDATE()
			)

		SET @Sno2 = SCOPE_IDENTITY();

		UPDATE dbo.tbl_club_notification
		SET notificationId = @Sno2
		WHERE Sno = @Sno2;

		SELECT '0' code
			,'Congratulations, you just added a new plan.' message;

		RETURN;
	END;

	IF @Flag = 'u'
	BEGIN
		IF NOT EXISTS (
				SELECT 'x'
				FROM dbo.tbl_plans
				WHERE Sno = @PlanId
					AND ISNULL(PlanStatus, '') = 'A'
				)
		BEGIN
			SET @ReturnMessage = 'Plan not found';

			SELECT '1' code
				,@ReturnMessage message;

			RETURN;
		END;

		UPDATE dbo.tbl_plans
		SET PlanName = ISNULL(@PlanName, PlanName)
			,PlanType = ISNULL(@PlanType, PlanType)
			,PlanTime = ISNULL(@Time, PlanTime)
			,Price = ISNULL(@Price, Price)
			,Liquor = ISNULL(@Liquor, Liquor)
			,Nomination = ISNULL(@Nomination, Nomination)
			,Remarks = ISNULL(@Remarks, Remarks)
			,ActionUser = @ActionUser
			,ActionIp = @ActionIp
			,ActionPlatform = @ActionPlatform
			,ActionDate = GETDATE()
			,PlanImage = ISNULL(@PlanImage, PlanImage)
			,PlanImage2 = ISNULL(@PlanImage2, PlanImage2)
			,AdditionalValue1 = ISNULL(@ExtraField1, AdditionalValue1)
			,AdditionalValue2 = ISNULL(@ExtraField2, AdditionalValue2)
			,AdditionalValue3 = ISNULL(@ExtraField3, AdditionalValue3)
			,NoOfPeople = ISNULL(@NoOfPeople, NoOfPeople)
			,PlanCategory = ISNULL(@PlanCategory, PlanCategory)
			,StrikePrice = ISNULL(@StrikePrice, StrikePrice)
			,IsStrikeOut = @IsStrikeOut
		WHERE Sno = @PlanId
			AND ISNULL(PlanStatus, '') = 'A';

		SET @ReturnMessage = 'Plan updated successfully';

		SELECT '0' code
			,@ReturnMessage message;

		RETURN;
	END;

	IF @Flag = 'bu'
	BEGIN
		IF EXISTS (
				SELECT 'X'
				FROM dbo.tbl_plans WITH (NOLOCK)
				WHERE Sno = @PlanId
					AND ISNULL(PlanStatus, '') = 'A'
				)
		BEGIN
			UPDATE dbo.tbl_plans
			SET PlanStatus = 'B'
				,ActionUser = @ActionUser
				,ActionIp = @ActionIp
				,ActionPlatform = @ActionPlatform
				,ActionDate = GETDATE()
			WHERE Sno = @PlanId
				AND ISNULL(PlanStatus, '') = 'A';

			SELECT '0' code
				,'Plan disabled successfully' message;

			RETURN;
		END;
		ELSE
		BEGIN
			SELECT '1' code
				,'Invalid request' message;

			RETURN;
		END;
	END;

	IF @Flag = 'gpddl' --get plan dropdown list
	BEGIN
		IF ISNULL(@StaticType, '') IN (
				7
				,8
				,9
				,35
				)
		BEGIN
			SELECT DISTINCT a.StaticDataValue AS StaticValue
				,a.StaticDataLabel AS StaticLabelEnglish
				,a.AdditionalValue1 AS StaticLabelJapanese
			FROM dbo.tbl_static_data a WITH (NOLOCK)
			INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK) ON b.StaticDataType = a.StaticDataType
			WHERE b.StaticDataType = @StaticType
				AND ISNULL(a.STATUS, '') = 'A'
			ORDER BY a.StaticDataValue ASC;

			RETURN;
		END;
		ELSE IF ISNULL(@StaticType, '') = '7'
		BEGIN
			SELECT DISTINCT a.StaticDataValue AS StaticValue
				,a.StaticDataLabel AS StaticLabelEnglish
				,a.AdditionalValue1 AS StaticLabelJapanese
			FROM dbo.tbl_static_data a WITH (NOLOCK)
			INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK) ON b.StaticDataType = a.StaticDataType
			WHERE b.StaticDataType = 7
				AND ISNULL(a.STATUS, '') = 'A'
			ORDER BY a.StaticDataValue ASC;

			RETURN;
		END;
		ELSE IF ISNULL(@StaticType, '') = '8'
		BEGIN
			SELECT DISTINCT a.StaticDataValue AS StaticValue
				,a.StaticDataLabel AS StaticLabelEnglish
				,a.AdditionalValue1 AS StaticLabelJapanese
			FROM dbo.tbl_static_data a WITH (NOLOCK)
			INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK) ON b.StaticDataType = a.StaticDataType
			WHERE b.StaticDataType = 8
				AND ISNULL(a.STATUS, '') = 'A'
			ORDER BY a.StaticDataValue ASC;

			RETURN;
		END;
		ELSE IF ISNULL(@StaticType, '') = '9'
		BEGIN
			SELECT DISTINCT a.StaticDataValue AS StaticValue
				,a.StaticDataLabel AS StaticLabelEnglish
				,a.AdditionalValue1 AS StaticLabelJapanese
			FROM dbo.tbl_static_data a WITH (NOLOCK)
			INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK) ON b.StaticDataType = a.StaticDataType
			WHERE b.StaticDataType = 9
				AND ISNULL(a.STATUS, '') = 'A'
			ORDER BY a.StaticDataValue ASC;

			RETURN;
		END;
		ELSE
		BEGIN
			SELECT '' AS StaticValue
				,'' AS StaticLabelEnglish
				,'' AS StaticLabelJapanese;

			RETURN;
		END;
	END;
END;
GO


