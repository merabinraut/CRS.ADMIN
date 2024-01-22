USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_promotion_management]    Script Date: 20/11/2023 21:27:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sproc_promotion_management] (
	@Flag CHAR(5) = NULL
	,@Title VARCHAR(200) = NULL
	,@Description VARCHAR(500) = NULL
	,@ImagePath VARCHAR(100) = NULL
	,@ImageStatus BIT = NULL
	,@ActionUser VARCHAR(100) = NULL
	,@ActionIP VARCHAR(100) = NULL
	,@Id BIGINT = NULL
	)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsDeleted BIT
		,@ReturnMessage VARCHAR(200) = NULL;

	--SELECT ALL PROMOTIONAL IMAGES
	IF @Flag = 's'
	BEGIN
		SELECT Sno
			,Title
			,ImgDescription
			,ImgPath
			,IsDeleted
			,ActionUser
			,FORMAT(CONVERT(DATETIME, ActionDate, 120), 'MMM dd, yyyy HH:mm:ss') AS ActionDate
		FROM tbl_promotional_images WITH (NOLOCK)
		ORDER BY Title ASC
		RETURN;
	END

	--SELECT PROMOTIONAL IMAGE BY ID
	IF @Flag = 'gp'
	BEGIN
		IF EXISTS (
				SELECT 'X'
				FROM tbl_promotional_images WITH (NOLOCK)
				WHERE Sno = @Id
				)
		BEGIN
			SELECT Sno
				,Title
				,ImgDescription
				,ImgPath
				,IsDeleted
				,ActionUser
				,ActionDate
			FROM tbl_promotional_images
			WHERE sNO = @Id

			RETURN;
		END
		ELSE
		BEGIN
			SELECT '1' code
				,'Promotional image not found' message

			RETURN;
		END
	END

	--ADD PROMOTIONAL IMAGE
	IF @Flag = 'a'
	BEGIN
		INSERT INTO tbl_promotional_images (
			Title
			,ImgDescription
			,ImgPath
			,ActionUser
			,ActionIP
			)
		VALUES (
			@Title
			,@Description
			,@ImagePath
			,ISNULL(@ActionUser,'superadmin')
			,@ActionIP
			)

		SELECT '0' code
			,'Promotional Image added successfully' message

		RETURN;
	END

	-- UPDATE PROMOTIONAL IMAGE
	IF @Flag = 'u'
	BEGIN
		IF NOT EXISTS (
				SELECT 'X'
				FROM tbl_promotional_images WITH (NOLOCK)
				WHERE IsDeleted <> 1
					AND Sno = @Id
				)
		BEGIN
			SELECT '1' Code,
				   'Invalid promotion details' Message;
			RETURN;
		END
		ELSE
		BEGIN
			UPDATE tbl_promotional_images
			SET Title = ISNULL(@Title, Title)
				,ImgDescription = ISNULL(@Description, ImgDescription)
				,ImgPath = ISNULL(@ImagePath, ImgPath)
			WHERE Sno = @Id;

			SELECT '0' code
				,'Promotion updated successfully' message

			RETURN;
		END
	END

	-- BLOCK/UNBLOCK PROMO IMAGE
	IF @Flag = 'bu'
	BEGIN
		IF EXISTS (
				SELECT 'X'
				FROM tbl_promotional_images
				WHERE Sno = @Id
				)
		BEGIN
			SELECT @IsDeleted = IsDeleted
			FROM tbl_promotional_images
			WHERE Sno = @Id;

			SET @ReturnMessage = CASE 
					WHEN @IsDeleted = 1
						THEN 'Promotional image unblocked successfully'
					ELSE 'Promotional image blocked successfully'
					END

			UPDATE tbl_promotional_images
			SET IsDeleted = CASE 
					WHEN @IsDeleted = 1
						THEN 0
					ELSE 1
					END
			WHERE Sno = @Id

			SELECT '0' code
				,@ReturnMessage message

			RETURN;
		END
		ELSE
		BEGIN
			SELECT '1' code
				,'Promotional image not found' message

			RETURN;
		END
	END
END
GO


