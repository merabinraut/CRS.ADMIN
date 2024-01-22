USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_admin_location_management]    Script Date: 20/11/2023 19:05:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Paras Maharjan>
-- Create date: <2023-10-16 15:23:56.940>
-- Description:	<location management for admin pannel>
-- =============================================
ALTER PROCEDURE [dbo].[sproc_admin_location_management] (
	@Flag VARCHAR(10) = NULL
	,@LocationId VARCHAR(20) = NULL
	,@LocationName NVARCHAR(200) = NULL
	,@LocationImage VARCHAR(200) = NULL
	,@LocationURL VARCHAR(500) = NULL
	,@LocationStatus CHAR(1) = 'A'
	,@Latitude VARCHAR(200) = NULL
	,@Longitude VARCHAR(200) = NULL
	,@ActionUser VARCHAR(200) = NULL
	,@ActionIp VARCHAR(50) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsLocationActive BIT = 1 -- 1: TRUE 0:FALSE
		,@ReturnMessage VARCHAR(200) = NULL
		,@sNo BIGINT = NULL;

	IF @Flag = 's'
	BEGIN
		SELECT l.LocationID
			,l.LocationName
			,l.LocationImage
			,ISNULL(COUNT(c.Sno), 0) AS ClubCount
			,l.STATUS locationStatus
		FROM tbl_location l WITH (NOLOCK)
		LEFT JOIN tbl_club_details c ON c.LocationId = l.LocationID
		GROUP BY l.LocationID
			,l.LocationName
			,l.LocationImage
			,l.STATUS

		RETURN;
	END

	IF @Flag = 'sd'
	BEGIN
		IF @LocationId IS NULL
		BEGIN
			SET @ReturnMessage = 'Location id is requred';

			SELECT '1' code
				,@ReturnMessage message

			RETURN;
		END

		IF NOT EXISTS (
				SELECT 1
				FROM tbl_location WITH (NOLOCK)
				WHERE LocationId = @LocationId
				)
		BEGIN
			SET @ReturnMessage = 'Location not found';

			SELECT '1' code
				,@ReturnMessage message

			RETURN;
		END

		SELECT LocationId
			,LocationName
			,LocationImage
			,LocationURL
			,STATUS
			,Latitude
			,Longitude
			,ActionUser
			,ActionDate
			,ActionIp
		FROM tbl_location WITH (NOLOCK)
		WHERE LocationId = @LocationId

		RETURN;
	END

	IF @FLAG = 'i'
	BEGIN
		INSERT INTO [dbo].[tbl_location] (
			[LocationName]
			,LocationImage
			,LocationURL
			,[Status]
			,Latitude
			,Longitude
			,[ActionUser]
			,[ActionDate]
			,[ActionIp]
			)
		VALUES (
			@LocationName
			,@LocationImage
			,@LocationURL
			,@LocationStatus
			,@Latitude
			,@Longitude
			,@ActionUser
			,GETDATE()
			,@ActionIp
			)

		SET @sNo = SCOPE_IDENTITY();

		UPDATE tbl_location
		SET LocationId = @sNo
		WHERE Sno = @sNo;

		SET @ReturnMessage = 'Congratulations, you just added a new location.';

		SELECT '0' code
			,@ReturnMessage message

		RETURN;
	END

	IF @Flag = 'u'
	BEGIN
		IF NOT EXISTS (
				SELECT 'x'
				FROM tbl_location
				WHERE LocationID = @LocationId
				)
		BEGIN
			SET @ReturnMessage = 'Location not found';

			SELECT '1' code
				,@ReturnMessage message;

			RETURN;
		END

		UPDATE tbl_location
		SET LocationName = ISNULL(@LocationName, LocationName)
			,LocationImage = ISNULL(@LocationImage, LocationImage)
			,LocationURL = ISNULL(@LocationURL, LocationURL)
			,STATUS = ISNULL(@LocationStatus, STATUS)
			,Latitude = ISNULL(@Latitude, Latitude)
			,Longitude = ISNULL(@Longitude, Longitude)
			,ActionUser = ISNULL(@ActionUser, ActionUser)
			,ActionIp = ISNULL(@ActionIp, ActionIp)
			,ActionDate = GETDATE()
		WHERE LocationId = @LocationId

		SET @ReturnMessage = 'Congratulations, you have successfully updated the location detail.';

		SELECT '0' code
			,@ReturnMessage message

		RETURN;
	END

	IF @Flag = 'bu'
	BEGIN
		IF EXISTS (
				SELECT 'X'
				FROM tbl_location WITH (NOLOCK)
				WHERE LocationId = @LocationID
				)
		BEGIN
			SELECT @IsLocationActive = CASE 
					WHEN STATUS = 'A'
						THEN 1
					ELSE 0
					END
			FROM tbl_location WITH (NOLOCK)
			WHERE LocationId = @LocationID;

			SET @ReturnMessage = CASE 
					WHEN @IsLocationActive = 1
						THEN 'Congratulations, you have successfully blocked the location.'
					ELSE 'Congratulations, you have successfully unblocked the location.'
					END

			UPDATE tbl_location
			SET STATUS = CASE 
					WHEN @IsLocationActive = 1
						THEN 'B'
					ELSE 'A'
					END,
				ActionUser = @ActionUser,
				ActionDate = GETDATE(),
				ActionIp = @ActionIp
			WHERE LocationId = @LocationID

			SELECT '0' code
				,@ReturnMessage message

			RETURN;
		END
		ELSE
		BEGIN
			SELECT '1' code
				,'Location not found' message

			RETURN;
		END
	END
END
GO


