ALTER PROC sproc_commission_management @Flag VARCHAR(10)
,@CategoryName VARCHAR(100) = NULL
,@Description VARCHAR(200) = NULL
,@Status CHAR(1) = NULL
,@FromAmount DECIMAL(18,2) = NULL
,@ToAmount DECIMAL(18,2) = NULL
,@CommissionType VARCHAR(10) = NULL
,@CommissionValue DECIMAL(18,2) = NULL
,@CommissionPercentageType CHAR(1) = NULL
,@MinCommissionValue DECIMAL(18,2) = NULL
,@MaxCommissionValue DECIMAL(18,2) = NULL
,@ActionUser VARCHAR(200) = NULL
,@ActionIP VARCHAR(50) = NULL
,@ActionPlatform VARCHAR(20) = NULL
,@CategoryId VARCHAR(20) = NULL
AS
DECLARE @Sno VARCHAR(10),
		@Sno2 VARCHAR(10),
		@TransactionName VARCHAR(200),
		@ErrorDesc VARCHAR(MAX),
		@RandomPassword VARCHAR(20);
BEGIN TRY
	IF ISNULL(@Flag, '') = 'icc' --insert commission category
	BEGIN
		IF EXISTS
		(
			SELECT 'X'
			FROM tbl_commission_Category a WITH (NOLOCK)
			WHERE a.CategoryName = @CategoryName
				AND ISNULL(a.Status, '') NOT IN ('D')
		)
		BEGIN
			SELECT 1 Code,
				   'Category already exists' Message;
			RETURN;
		END

		INSERT INTO tbl_commission_Category
		(
			CategoryName
		   ,[Description]
		   ,[Status]
		   ,ActionUser
		   ,ActionIP
		   ,ActionDate
		)
		VALUES
		(
			@CategoryName
		   ,@Description
		   ,'A'
		   ,@ActionUser
		   ,@ActionIP
		   ,GETDATE()
		)

		SET @Sno = SCOPE_IDENTITY()

		UPDATE tbl_commission_Category
		SET CategoryId = @Sno
		WHERE Sno = @Sno

		SELECT 0 Code,
			   'Commission category created successfully' Message;
		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'mccs' --'manage commission category status
	BEGIN
		SET @TransactionName = 'mccs'

		BEGIN TRANSACTION @TransactionName;

		IF ISNULL(@Status, '') = 'I'
		BEGIN
			IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_commission_Category a WITH (NOLOCK)
				WHERE a.CategoryId = @CategoryId
					AND ISNULL(a.[Status], '') = 'A'
			)
			BEGIN
				SELECT 1 Code,
					   'Invalid commission category' Message;
				RETURN;
			END		
			
			UPDATE tbl_commission_category
			SET [Status] = 'I',
				ActionUser = @ActionUser,
				ActionIP = @ActionIP,
				ActionDate = GETDATE()
			WHERE CategoryId = @CategoryId

			UPDATE tbl_commission_category_detail
			SET [Status] = 'I',
				ActionUser = @ActionUser,
				ActionIP = @ActionIP,
				ActionDate = GETDATE()
			WHERE CategoryId = @CategoryId			
		END
		ELSE IF ISNULL(@Status, '') = 'A'
		BEGIN
			IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_commission_Category a WITH (NOLOCK)
				WHERE a.CategoryId = @CategoryId
					AND ISNULL(a.[Status], '') = 'I'
			)
			BEGIN
				SELECT 1 Code,
					   'Invalid commission category' Message;
				RETURN;
			END
			
			UPDATE tbl_commission_category
			SET [Status] = 'A',
				ActionUser = @ActionUser,
				ActionIP = @ActionIP,
				ActionDate = GETDATE()
			WHERE CategoryId = @CategoryId

			UPDATE tbl_commission_category_detail
			SET [Status] = 'A',
				ActionUser = @ActionUser,
				ActionIP = @ActionIP,
				ActionDate = GETDATE()
			WHERE CategoryId = @CategoryId			
		END
		ELSE
		BEGIN
			SELECT 1 Code,
				   'Invalid status' Message;

			ROLLBACK TRANSACTION @TransactionName;
			RETURN;
		END

		SELECT 0 Code,
				   'Category status updated successfully' Message;

		COMMIT TRANSACTION @TransactionName;
		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'gcac' --get commission assigned clubs
	BEGIN
		SELECT b.ClubName1 AS ClubName,
			   b.Logo,
			   b.Status,
			   b.Email AS EmailAddress,
			   b.MobileNumber
		FROM tbl_commission_category a WITH (NOLOCK) 
		INNER JOIN tbl_club_details b WITH (NOLOCK) ON b.CommissionId = a.CategoryId
		WHERE a.CategoryId = @CategoryId
			AND ISNULL(a.Status, '') NOT IN ('S', 'D')

		RETURN;
	END
	ELSE IF ISNULL(@Flag, '') = 'gccl' --get commission category list
	BEGIN
		SELECT CategoryId
			  ,CategoryName
			  ,[Description]
			  ,[Status]
			  ,ActionUser
			  ,ActionIP
			  ,ActionDate AS CreatedDate
			  ,ActionDate AS UpdatedDate
		FROM tbl_commission_category a WITH (NOLOCK)
		WHERE ISNULL(a.Status, '') NOT IN ('S', 'D')

		RETURN;
	END
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION @TransactionName;

	SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

	INSERT INTO tbl_error_log
	(
		 ErrorDesc
		,ErrorScript
		,QueryString
		,ErrorCategory
		,ErrorSource
		,ActionDate
	)
	VALUES
	(
		 @ErrorDesc
		,'sproc_commission_management(Flag: ' + ISNULL(@Flag, '') + ')'
		,'SQL'
		,'SQL'
		,'sproc_commission_management'
		,GETDATE()
	)

	SELECT 1 Code,
		   'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message
	RETURN;
END CATCH
GO