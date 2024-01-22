USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_bookmark_management]    Script Date: 23/11/2023 21:28:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROC [dbo].[sproc_bookmark_management] @Flag VARCHAR(10)
,@AgentType VARCHAR(10) = NULL
,@ClubId VARCHAR(10) = NULL
,@HostId VARCHAR(10) = NULL
,@CustomerId VARCHAR(10) = NULL
,@ActionUser VARCHAR(200) = NULL
,@ActionPlatform VARCHAR(20) = NULL
,@ActionIP VARCHAR(50) = NULL
,@Status CHAR(1) = NULL
AS
DECLARE @Sno BIGINT;
BEGIN
	IF @Flag = 'mb' -- manage bookmark
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_customer a WITH (NOLOCK)
			INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
				AND b.RoleId = 3 AND b.Status = 'A'
			WHERE a.AgentId = @CustomerId
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid request' Message;
			RETURN;
		END

		IF @AgentType IN ('Club', '5')
		BEGIN
			IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_club_details a WITH (NOLOCK)
				WHERE a.AgentId = @ClubId
					AND a.Status = 'A'
			)
			BEGIN
				SELECT 1 Code,
					   'Invalid club' Message;
				RETURN;
			END

			IF EXISTS
			(
				SELECT 'X'
				FROM tbl_bookmark a WITH (NOLOCK)
				WHERE a.CustomerId = @CustomerId
					AND a.ClubId = @ClubId
					AND a.AgentType = 'Club' 
					AND a.HostId IS NULL
			)
			BEGIN	
				IF @Status = 'A'
				BEGIN	
					IF NOT EXISTS
					(
						SELECT 'X'
						FROM tbl_bookmark a WITH (NOLOCK)
						WHERE a.CustomerId = @CustomerId
							AND a.ClubId = @ClubId
							AND a.AgentType = 'Club' 
							AND a.Status = 'B'
							AND a.HostId IS NULL
					)
					BEGIN
						SELECT 1 Code,
							   'Invalid request' Message;
						RETURN;
					END

					UPDATE tbl_bookmark
					SET Status = 'A',
						UpdatedBy = @ActionUser,
						UpdatedDate = GETDATE(),
						UpdatedIP = @ActionIP,
						UpdatedPlatform = @ActionPlatform
					WHERE CustomerId = @CustomerId
							AND ClubId = @ClubId
							AND AgentType = 'Club' 
							AND Status = 'B'
							AND HostId IS NULL;

					SELECT 0 Code,
						'Club bookmarked successfully' Message;					   
					RETURN;
				END
				ELSE IF @Status = 'B'
				BEGIN
					IF NOT EXISTS
					(
						SELECT 'X'
						FROM tbl_bookmark a WITH (NOLOCK)
						WHERE a.CustomerId = @CustomerId
							AND a.ClubId = @ClubId
							AND a.AgentType = 'Club' 
							AND a.Status = 'A'
							AND a.HostId IS NULL
					)
					BEGIN
						SELECT 1 Code,
							   'Invalid request' Message;
						RETURN;
					END

					UPDATE tbl_bookmark
					SET Status = 'B',
						UpdatedBy = @ActionUser,
						UpdatedDate = GETDATE(),
						UpdatedIP = @ActionIP,
						UpdatedPlatform = @ActionPlatform
					WHERE CustomerId = @CustomerId
							AND ClubId = @ClubId
							AND AgentType = 'Club'
							AND Status = 'A'
							AND HostId IS NULL;

					SELECT 0 Code,
					   'Club bookmark removed successfully' Message;
					RETURN;
				END
				ELSE
				BEGIN
					SELECT 1 Code,
							'Invalid request' Message;
					RETURN;
				END
			END
			ELSE
			BEGIN
				INSERT INTO tbl_bookmark
				(
					CustomerId
				   ,ClubId
				   ,HostId
				   ,AgentType
				   ,Status
				   ,CreatedBy
				   ,CreatedDate
				   ,CreatedPlatform
				   ,CreatedIP
				)
				VALUES
				(
					@CustomerId
				   ,@ClubId
				   ,NULL
				   ,'Club'
				   ,'A'
				   ,@ActionUser
				   ,GETDATE()
				   ,@ActionPlatform
				   ,@ActionIP
				)

				SET @Sno = SCOPE_IDENTITY();

				UPDATE tbl_bookmark
				SET BookmarkId = @Sno
				WHERE Sno = @Sno;

				SELECT 0 Code,
					   'Club bookmarked successfully' Message;
				RETURN;
			END
		END
		ELSE IF @AgentType IN ('Host', '7')
		BEGIN
			IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_host_details a WITH (NOLOCK)
				WHERE a.AgentId = @ClubId
					AND a.HostId = @HostId
					AND a.Status = 'A'
			)
			BEGIN
				SELECT 1 Code,
					   'Invalid host' Message;
				RETURN;
			END

			IF EXISTS
			(
				SELECT 'X'
				FROM tbl_bookmark a WITH (NOLOCK)
				WHERE a.CustomerId = @CustomerId
					AND a.ClubId = @ClubId
					AND a.HostId = @HostId
					AND a.AgentType = 'Host' 
			)
			BEGIN				
				IF @Status = 'A'
				BEGIN	
					IF NOT EXISTS
					(
						SELECT 'X'
						FROM tbl_bookmark a WITH (NOLOCK)
						WHERE a.CustomerId = @CustomerId
							AND a.ClubId = @ClubId
							AND a.HostId = @HostId
							AND a.AgentType = 'Host' 
							AND a.Status = 'B'
					)
					BEGIN
						SELECT 1 Code,
							   'Invalid request' Message;
						RETURN;
					END

					UPDATE tbl_bookmark
					SET Status = 'A',
						UpdatedBy = @ActionUser,
						UpdatedDate = GETDATE(),
						UpdatedIP = @ActionIP,
						UpdatedPlatform = @ActionPlatform
					WHERE CustomerId = @CustomerId
							AND ClubId = @ClubId
							AND HostId = @HostId
							AND AgentType = 'Host' 
							AND Status = 'B';

					SELECT 0 Code,
						'Host bookmarked successfully' Message;
					RETURN;
				END
				ELSE IF @Status = 'B'
				BEGIN
					IF NOT EXISTS
					(
						SELECT 'X'
						FROM tbl_bookmark a WITH (NOLOCK)
						WHERE a.CustomerId = @CustomerId
							AND a.ClubId = @ClubId
							AND a.HostId = @HostId
							AND a.AgentType = 'Host' 
							AND a.Status = 'A'
					)
					BEGIN
						SELECT 1 Code,
							   'Invalid request' Message;
						RETURN;
					END

					UPDATE tbl_bookmark
					SET Status = 'B',
						UpdatedBy = @ActionUser,
						UpdatedDate = GETDATE(),
						UpdatedIP = @ActionIP,
						UpdatedPlatform = @ActionPlatform
					WHERE CustomerId = @CustomerId
							AND ClubId = @ClubId
							AND HostId = @HostId
							AND AgentType = 'Host'
							AND Status = 'A';

					SELECT 0 Code,					   
					   'Host bookmark removed successfully' Message;
					RETURN;
				END
				ELSE
				BEGIN
					SELECT 1 Code,
							'Invalid request' Message;
					RETURN;
				END
			END
			ELSE
			BEGIN
				INSERT INTO tbl_bookmark
				(
					CustomerId
				   ,ClubId
				   ,HostId
				   ,AgentType
				   ,Status
				   ,CreatedBy
				   ,CreatedDate
				   ,CreatedPlatform
				   ,CreatedIP
				)
				VALUES
				(
					@CustomerId
				   ,@ClubId
				   ,@HostId
				   ,'Host'
				   ,'A'
				   ,@ActionUser
				   ,GETDATE()
				   ,@ActionPlatform
				   ,@ActionIP
				)

				SET @Sno = SCOPE_IDENTITY();

				UPDATE tbl_bookmark
				SET BookmarkId = @Sno
				WHERE Sno = @Sno;

				SELECT 0 Code,
					   'Host bookmarked successfully' Message;
				RETURN;
			END
		END
		ELSE
		BEGIN
			SELECT 1 Code,
				   'Invalid agent' Message;
			RETURN;
		END
		
	END

	--Get club list via bookmark and customer id
    IF @Flag = 'clvb'
    BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_customer a WITH (NOLOCK)
			INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId
				AND b.RoleId = 3 AND b.Status = 'A'
			WHERE a.AgentId = @CustomerId
		)
		BEGIN
			SELECT 1 Code,
				   'Invalid request' Message;
			RETURN;
		END;

        SELECT a.AgentId AS ClubId,
               a.LocationId AS LocationId,
               a.ClubName1 AS ClubName,
               a.ClubName2 AS ClubNameJapanese,
               a.GroupName,
               (ISNULL(a.FirstName, '') + ISNULL(a.MiddleName, '') + ' ' + ISNULL(a.LastName, '')) AS FullName,
               a.Logo AS ClubLogo,
               a.CoverPhoto AS ClubCoverPhoto,
               a.Description AS ClubDescription,
               c.LocationName AS Tag1,
               CASE
                   WHEN ISNULL(b.Tag2Status, '') = 'A' THEN
                       t2.StaticDataLabel
                   ELSE
                       ''
               END AS Tag2,
               CASE
                   WHEN ISNULL(b.Tag3Status, '') = 'A' THEN
                       t3.StaticDataLabel
                   ELSE
                       ''
               END AS Tag3,
               CASE
                   WHEN ISNULL(b.Tag4Status, '') = 'A' THEN
                       'Excellent'
                   ELSE
                       '' --'Not Excellent'
               END AS Tag4,
               CASE
                   WHEN ISNULL(b.Tag5Status, '') = 'A' THEN
                       t5.StaticDataLabel
                   ELSE
                       ''
               END AS Tag5,
               a.ClubOpeningTime,
               a.ClubClosingTime
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            LEFT JOIN dbo.tbl_tag_detail b WITH (NOLOCK)
                ON b.ClubId = a.AgentId
                   AND ISNULL(a.[Status], '') = 'A'
            LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = b.Tag1Location
                   AND b.Tag1Status = 'A'
                   AND ISNULL(c.[Status], '') = 'A'
            LEFT JOIN dbo.tbl_static_data t5 WITH (NOLOCK)
                ON t5.StaticDataType = 21
                   AND t5.StaticDataValue = b.Tag5StoreName
                   AND t5.Status = 'A'
            LEFT JOIN dbo.tbl_static_data t3 WITH (NOLOCK)
                ON t3.StaticDataType = 17
                   AND t3.StaticDataValue = b.Tag3CategoryName
                   AND t3.Status = 'A'
            LEFT JOIN dbo.tbl_static_data t2 WITH (NOLOCK)
                ON t2.StaticDataType = 14
                   AND t2.StaticDataValue = b.Tag2RankName
                   AND t2.Status = 'A'
			INNER JOIN tbl_bookmark d WITH (NOLOCK) ON d.ClubId = a.AgentId
				AND d.AgentType = 'Club'
				AND d.Status = 'A'				
        WHERE d.CustomerId = @CustomerId;
        RETURN;
    END;

	ELSE IF ISNULL(@Flag, '') = 'gcgilvc' --get club gallery image list vai club
    BEGIN        
		SELECT TOP 3
		       b.ImagePath
		FROM tbl_club_details a WITH (NOLOCK)
		    INNER JOIN tbl_gallery b WITH (NOLOCK)
		        ON b.AgentId = a.AgentId
		           AND b.RoleId = 5
				   AND a.AgentId = @ClubId
		           AND ISNULL(b.Status, '') = 'A'
		           AND ISNULL(a.Status, '') = 'A'		           
		ORDER BY NEWID();
		RETURN;
    END;

    --Get host list via bookmark and customer id
    ELSE IF @Flag = 'hlvb'
    BEGIN
        SELECT a.LocationId,
               b.AgentId ClubId,
               b.HostId,
               b.HostName,
               d.StaticDataLabel AS Occupation,
               b.Rank,
               a.ClubName1 AS ClubName,
               a.Logo AS ClubLogo,
               (
                   SELECT TOP 1
                          ImagePath
                   FROM tbl_gallery c WITH (NOLOCK)
                   WHERE c.AgentId = b.HostId
                         AND c.RoleId = 7
                         AND ISNULL(c.Status, '') = 'A'
                   ORDER BY c.Sno DESC
               ) AS HostImage
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND ISNULL(a.[Status], '') = 'A'
            LEFT JOIN tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 12
                   AND d.StaticDataValue = b.PreviousOccupation
                   AND ISNULL(d.Status, '') = 'A'
			INNER JOIN tbl_bookmark e WITH (NOLOCK) ON e.ClubId = a.AgentId
				AND e.HostId = b.HostId
				AND e.AgentType = 'Host'
				AND e.Status = 'A'	
        WHERE e.CustomerId = @CustomerId
              AND a.AgentId = ISNULL(@ClubId, a.AgentId)
              AND ISNULL(b.[Status], '') = 'A';

        RETURN;
    END;

	ELSE IF ISNULL(@Flag, '') = 'ghgilvh' --get host gallery image list via host
    BEGIN
        SELECT b.ImagePath
        FROM tbl_host_details a WITH (NOLOCK)
            INNER JOIN tbl_gallery b WITH (NOLOCK)
                ON b.AgentId = a.HostId
                   AND b.RoleId = 7
				   AND a.HostId = @HostId
				   AND a.AgentId = @ClubId
                   AND ISNULL(b.Status, '') = 'A'
                   AND ISNULL(a.Status, '') = 'A'                   
        ORDER BY NEWID();
        RETURN;
    END;
END
GO


