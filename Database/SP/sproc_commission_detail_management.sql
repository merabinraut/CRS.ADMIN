USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_commission_detail_management]    Script Date: 11/29/2023 10:09:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_commission_detail_management] @Flag VARCHAR(10)
	,@CategoryId VARCHAR(10) = NULL
	,@CommissionValue DECIMAL(18, 2) = NULL
	,@Status CHAR(1) = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIP VARCHAR(50) = NULL
	,@CategoryDetailId VARCHAR(10) = NULL
	,@FromAmount DECIMAL(18, 2) = NULL
	,@ToAmount DECIMAL(18, 2) = NULL
	,@CommissionType VARCHAR(10) = NULL
	,@CommissionPercentageType CHAR(1) = NULL
	,@MinCommissionValue DECIMAL(18, 2) = NULL
	,@MaxCommissionValue DECIMAL(18, 2) = NULL
	,@AgentId VARCHAR(10) = NULL
AS
DECLARE @Sno BIGINT;

BEGIN
	IF ISNULL(@Flag, '') = 'gcdl' --get commission detail list
	BEGIN
		SELECT b.CategoryDetailId
			,b.CategoryId
			,b.FromAmount
			,b.ToAmount
			,b.CommissionType
			,b.CommissionValue
			,b.CommissionPercentageType
			,b.MinCommissionValue
			,b.MaxCommissionValue
			,b.[Status]
			,b.ActionUser
			,b.ActionIP
			,b.ActionDate
		FROM tbl_commission_category a WITH (NOLOCK)
		INNER JOIN tbl_commission_category_detail b WITH (NOLOCK) ON b.CategoryId = a.CategoryId
			AND ISNULL(a.STATUS, '') NOT IN (
				'D'
				,'S'
				)
		WHERE a.CategoryId = @CategoryId
			AND ISNULL(b.STATUS, '') NOT IN (
				'D'
				,'S'
				)

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'gcdid' ---get commission detail by id
	BEGIN
		SELECT b.CategoryDetailId
			,b.CategoryId
			,b.FromAmount
			,b.ToAmount
			,b.CommissionType
			,b.CommissionValue
			,b.CommissionPercentageType
			,b.MinCommissionValue
			,b.MaxCommissionValue
			,b.[Status]
			,b.ActionUser
			,b.ActionIP
			,b.ActionDate
		FROM tbl_commission_category a WITH (NOLOCK)
		INNER JOIN tbl_commission_category_detail b WITH (NOLOCK) ON b.CategoryId = a.CategoryId
			AND ISNULL(a.STATUS, '') NOT IN (
				'D'
				,'S'
				)
		WHERE b.CategoryDetailId = @CategoryDetailId
			AND ISNULL(b.STATUS, '') NOT IN (
				'D'
				,'S'
				)

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'icd' --insert commission details
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_commission_category a WITH (NOLOCK)
				WHERE a.CategoryId = @CategoryId
					AND ISNULL(a.STATUS, '') = 'A'
				)
		BEGIN
			SELECT 1 Code
				,'Invalid commission category' Message

			RETURN;
		END

		INSERT INTO tbl_commission_category_detail (
			CategoryId
			,CommissionPercentageType
			,CommissionType
			,CommissionValue
			,FromAmount
			,ToAmount
			,MinCommissionValue
			,MaxCommissionValue
			,CategoryDetailId
			,[Status]
			,ActionUser
			,ActionIP
			,ActionDate
			)
		VALUES (
			@CategoryId
			,@CommissionPercentageType
			,'FC'
			,ISNULL(@CommissionValue, '0')
			,ISNULL(@FromAmount, '0')
			,ISNULL(@ToAmount, '0')
			,ISNULL(@MinCommissionValue, '0')
			,ISNULL(@MaxCommissionValue, '0')
			,@CategoryDetailId
			,'A'
			,@ActionUser
			,@ActionIP
			,GETDATE()
			)

		SET @Sno = SCOPE_IDENTITY();

		UPDATE tbl_commission_category_detail
		SET CategoryDetailId = @Sno
		WHERE Sno = @Sno
			AND CategoryId = @CategoryId

		SELECT 0 Code
			,'Commission created successfully' Message
			,@CategoryId categoryId

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'ucd' --update commission details
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_commission_category a WITH (NOLOCK)
				INNER JOIN tbl_commission_category_detail b WITH (NOLOCK) ON b.CategoryId = a.CategoryId
					AND ISNULL(b.[Status], '') = 'A'
				WHERE a.CategoryId = @CategoryId
					AND b.CategoryDetailId = @CategoryDetailId
					AND ISNULL(a.[Status], '') = 'A'
				)
		BEGIN
			SELECT 1 Code
				,'Invalid commission detail' Message

			RETURN;
		END

		--IF EXISTS
		--(
		--	SELECT 'X'
		--	FROM tbl_commission_category_detail a WITH (NOLOCK)
		--	WHERE a.CategoryId = a.CategoryId
		--		AND a.CategoryDetailId <> @CategoryDetailId
		--		AND ISNULL(a.[Status], '') = 'A'
		--		AND a.CategoryId=@CategoryId
		--		AND ISNULL(@FromAmount, 0) BETWEEN ISNULL(a.FromAmount, 0) AND ISNULL(a.ToAmount, 0)
		--	UNION
		--	SELECT 'X'
		--	FROM tbl_commission_category_detail a WITH (NOLOCK)
		--	WHERE a.CategoryId = a.CategoryId
		--		AND a.CategoryDetailId <> @CategoryDetailId
		--		AND a.CategoryId=@CategoryId
		--		AND ISNULL(a.[Status], '') = 'A'
		--		AND ISNULL(@ToAmount, 0) BETWEEN ISNULL(a.FromAmount, 0) AND ISNULL(a.ToAmount, 0)
		--)
		--BEGIN
		--	SELECT 1 Code,
		--		   'Slab already exist' Message;
		--	RETURN;
		--END
		IF ISNULL(@FromAmount, 0) = ISNULL(@ToAmount, 0)
		BEGIN
			SELECT 1 Code
				,'From amount and to amount value cannot be same' Message;

			RETURN;
		END

		IF ISNULL(@FromAmount, 0) > ISNULL(@ToAmount, 0)
		BEGIN
			SELECT 1 Code
				,'From amount cannot be greater than to amount value' Message;

			RETURN;
		END

		IF @CommissionPercentageType = 'P'
			AND @CommissionValue > 100
		BEGIN
			SELECT 1 Code
				,'Commission type percentage cannot have more than 100 percent' Message;

			RETURN;
		END

		IF ISNULL(@CommissionValue, 0) > 0
			AND (
				ISNULL(@MaxCommissionValue, 0) <= 0
				OR ISNULL(@MinCommissionValue, 0) <= 0
				)
		BEGIN
			SELECT 1 Code
				,'Invalid minimum/maximum commission value.' Message

			RETURN;
		END

		IF ISNULL(@CommissionValue, 0) < 0
			AND (
				ISNULL(@MaxCommissionValue, 0) > 0
				OR ISNULL(@MinCommissionValue, 0) > 0
				)
		BEGIN
			SELECT 1 Code
				,'Invalid minimum/maximum commission value.' Message

			RETURN;
		END

		IF ISNULL(@MinCommissionValue, 0) > ISNULL(@MaxCommissionValue, 0)
		BEGIN
			SELECT 1 Code
				,'Minimum commission value cannot be greater than maximum commission value' Message

			RETURN;
		END

		UPDATE tbl_commission_category_detail
		SET FromAmount = ISNULL(@FromAmount, FromAmount)
			,ToAmount = ISNULL(@ToAmount, ToAmount)
			,CommissionType = ISNULL(@CommissionType, CommissionType)
			,CommissionValue = ISNULL(@CommissionValue, CommissionValue)
			,CommissionPercentageType = ISNULL(@CommissionPercentageType, CommissionPercentageType)
			,MinCommissionValue = ISNULL(@MinCommissionValue, MinCommissionValue)
			,MaxCommissionValue = ISNULL(@MaxCommissionValue, MaxCommissionValue)
			,ActionUser = @ActionUser
			,ActionIP = @ActionIP
			,ActionDate = GETDATE()
		WHERE CategoryId = @CategoryId
			AND CategoryDetailId = @CategoryDetailId
			AND ISNULL([Status], '') = 'A'

		SELECT 0 Code
			,'Commission detail updated successfully' Message
			,@CategoryId categoryId

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'dcd' --delete commission detail
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_commission_category a WITH (NOLOCK)
				INNER JOIN tbl_commission_category_detail b WITH (NOLOCK) ON b.CategoryId = a.CategoryId
					AND ISNULL(b.[Status], '') = 'A'
				WHERE a.CategoryId = @CategoryId
					AND b.CategoryDetailId = @CategoryDetailId
					AND ISNULL(a.[Status], '') = 'A'
				)
		BEGIN
			SELECT 1 Code
				,'Invalid commission detail' Message

			RETURN;
		END

		UPDATE tbl_commission_category_detail
		SET [Status] = 'D'
			,ActionUser = @ActionUser
			,ActionIP = @ActionIP
			,ActionDate = GETDATE()
		WHERE CategoryId = @CategoryId
			AND CategoryDetailId = @CategoryDetailId
			AND ISNULL([Status], '') = 'A'

		SELECT 0 Code
			,'Commission details deleted successfully' Message

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'acc' --assign commission to club
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_commission_category a WITH (NOLOCK)
				WHERE a.CategoryId = @CategoryId
					AND ISNULL(a.[Status], '') = 'A'
				)
		BEGIN
			SELECT 1 Code
				,'Invalid commission detail' Message

			RETURN;
		END

		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_club_details a WITH (NOLOCK)
				WHERE a.AgentId = @AgentId
					AND a.STATUS IN ('A')
				)
		BEGIN
			SELECT 1 Code
				,'Invalid club detail' Message

			RETURN;
		END

		UPDATE tbl_club_details
		SET CommissionId = @CategoryId
			,ActionUser = @ActionUser
			,ActionIP = @ActionIP
			,ActionDate = GETDATE()
		WHERE AgentId = @AgentId
			AND [Status] NOT IN (
				'D'
				,'S'
				)

		SELECT 0 Code
			,'Commission assigned successfully' Message;

		RETURN;
	END
END
