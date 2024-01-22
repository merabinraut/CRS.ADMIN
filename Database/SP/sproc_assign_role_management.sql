USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_assign_role_management]    Script Date: 20/11/2023 12:47:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROC [dbo].[sproc_assign_role_management] @Flag VARCHAR(10)
,@AgentType VARCHAR(20) = NULL
,@UserId VARCHAR(10) = NULL
,@ActionUser VARCHAR(200) = NULL
,@RoleId VARCHAR(10) = NULL
,@ActionIP VARCHAR(50) = NULL 
,@AgentId VARCHAR(10) = NULL
AS
BEGIN
	IF @Flag = 'gcar' --get current assigned role
	BEGIN
		IF ISNULL(@AgentType, '') = '2'
		BEGIN
			SELECT 0 Code,
			       'Success' Message,
				   b.Id AS RoleId,
				   b.RoleName 
			FROM tbl_admin a WITH (NOLOCK) 
			INNER JOIN tbl_roles b WITH (NOLOCK) ON b.Id = a.RoleId AND b.RoleType = 2 AND b.Status = 'A' AND ISNULL(a.Status, '') = 'A'
			WHERE a.Id = @AgentId
			ORDER BY b.RoleName ASC;
			RETURN;
		END
		ELSE IF ISNULL(@AgentType, '') = '4'
		BEGIN
			SELECT 0 Code,
			       'Success' Message,
				   c.Id AS RoleId,
				   c.RoleName
			FROM tbl_club_details a WITH (NOLOCK)
			INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND a.Status = 'A' AND b.Status = 'A'
			INNER JOIN tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId AND c.RoleType = 4 AND c.Status = 'A'
			WHERE a.AgentId = @AgentId
			ORDER BY c.RoleName ASC;
			RETURN;
		END
		ELSE IF ISNULL(@AgentType, '') = '3'
		BEGIN
			SELECT 0 Code,
			       'Success' Message, 
				   c.Id AS RoleId,
				   c.RoleName
			FROM tbl_customer a WITH (NOLOCK)
			INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND b.Status = 'A'
			INNER JOIN tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId AND c.RoleType = 3 AND c.Status = 'A'
			WHERE a.AgentId = @AgentId
			ORDER BY c.RoleName ASC
			RETURN;
		END
		ELSE 
		BEGIN
			SELECT 1 Code,
			       'Invalid deatils' Message;
			RETURN;
		END
	END

	ELSE IF @Flag = 'mur' --manage user role
	BEGIN
		IF NOT EXISTS
		(
			SELECT 'X'
			FROM tbl_roles a WITH (NOLOCK)
			WHERE a.Id = @RoleId
				AND a.Status = 'A'
		)
		BEGIN
			SELECT 1 Code,
			       'Invalid Role' Message;
			RETURN;
		END
		IF ISNULL(@AgentType, '') = '2'
		BEGIN
			IF @AgentId Is NULL
			BEGIN
				UPDATE tbl_admin
				SET RoleId = @RoleId
				   ,ActionUser = @ActionUser
				   ,ActionDate = GETDATE()
				   ,ActionIP = @ActionIP
				WHERE Status = 'A' and RoleId NOT IN (1);

				SELECT 0 Code,
					   'Role updated successfully' Message;

				RETURN;
			END
			ELSE
			BEGIN
				IF NOT EXISTS
				(
					SELECT 'X'
					FROM tbl_admin a WITH (NOLOCK) 
					INNER JOIN tbl_roles b WITH (NOLOCK) ON b.Id = a.RoleId AND b.RoleType = 2 AND b.Status = 'A' AND ISNULL(a.Status, '') = 'A'
					WHERE a.Id = @AgentId	
				)
				BEGIN
					SELECT 1 Code,
						   'Invalid details' Message;
					RETURN;
				END
				
				UPDATE tbl_admin
				SET RoleId = @RoleId
				   ,ActionUser = @ActionUser
				   ,ActionDate = GETDATE()
				   ,ActionIP = @ActionIP
				WHERE Id = @AgentId
					AND RoleId NOT IN (1)
					AND Status = 'A';

				SELECT 0 Code,
					   'Role updated successfully' Message;

				RETURN;
			END
		END
		ELSE IF ISNULL(@AgentType, '') = '4'
		BEGIN
			IF @AgentId IS NULL
			BEGIN
				UPDATE tbl_users
				SET RoleId = @RoleId
				   ,ActionUser = @ActionUser
				   ,ActionDate = GETDATE()
				   ,ActionIP = @ActionIP
				WHERE RoleType = 4
				AND Status = 'A'

				SELECT 0 Code,
					   'Role updated successfully' Message; 

				RETURN;
			END
			ELSE
			BEGIN
				IF NOT EXISTS
			(
				SELECT 'X'
				FROM tbl_club_details a WITH (NOLOCK)
				INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND a.Status = 'A' AND b.Status = 'A'
				INNER JOIN tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId AND c.RoleType = 4 AND c.Status = 'A'
				WHERE a.AgentId = @AgentId
					AND b.RoleType = 4
			)	
			BEGIN
				SELECT 1 Code,
					   'Invalid details' Message;
				RETURN;
			END
				
			UPDATE tbl_users
			SET RoleId = @RoleId
			   ,ActionUser = @ActionUser
			   ,ActionDate = GETDATE()
			   ,ActionIP = @ActionIP
			WHERE AgentId = @AgentId
				AND RoleType = 4
				AND Status = 'A'

				SELECT 0 Code,
					   'Role updated successfully' Message; 

				RETURN;
			END
		END
		ELSE IF ISNULL(@AgentType, '') = '3'
		BEGIN
			IF @AgentId IS NULL
			BEGIN
				UPDATE tbl_users
				SET RoleId = @RoleId
				   ,ActionUser = @ActionUser
				   ,ActionDate = GETDATE()
				   ,ActionIP = @ActionIP
				WHERE RoleType = 3
				AND Status = 'A'

				SELECT 0 Code,
					   'Role updated successfully' Message; 

				RETURN;
			END
			ELSE
			BEGIN
				IF NOT EXISTS
				(
					SELECT 'X'
					FROM tbl_customer a WITH (NOLOCK)
					INNER JOIN tbl_users b WITH (NOLOCK) ON b.AgentId = a.AgentId AND b.Status = 'A'
					INNER JOIN tbl_roles c WITH (NOLOCK) ON c.Id = b.RoleId AND c.RoleType = 3 AND c.Status = 'A'
					WHERE a.AgentId = @AgentId
						AND b.RoleType = 3
						
				)
				BEGIN
					SELECT 1 Code,
						   'Invalid details' Message;
					RETURN;
				END
				
				UPDATE tbl_users
				SET RoleId = @RoleId
				   ,ActionUser = @ActionUser
				   ,ActionDate = GETDATE()
				   ,ActionIP = @ActionIP
				WHERE AgentId = @AgentId
					AND RoleType = 3
					AND Status = 'A'

				SELECT 0 Code,
					   'Role updated successfully' Message; 

				RETURN;
			END
		END
		ELSE 
		BEGIN
			SELECT 1 Code,
			       'Invalid deatils' Message;
			RETURN;
		END
	END
END
GO


