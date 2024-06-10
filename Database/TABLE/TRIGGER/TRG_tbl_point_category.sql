USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_point_category]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER trigger [dbo].[TRG_tbl_point_category]
ON [dbo].[tbl_point_category]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_point_category_audit
		(
			Id
		   ,CategoryName
		   ,Description
		   ,RoleType
		   ,Status
		   ,IsDefault
		   ,ActionDate
		   ,ActionIp
		   ,ActionUser
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Id
			   ,CategoryName
			   ,Description
			   ,RoleType
			   ,Status
			   ,IsDefault
			   ,ActionDate
			   ,ActionIp
			   ,ActionUser
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_point_category_audit
		(
			Id
		   ,CategoryName
		   ,Description
		   ,RoleType
		   ,Status
		   ,IsDefault
		   ,ActionDate
		   ,ActionIp
		   ,ActionUser
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Id
			   ,CategoryName
			   ,Description
			   ,RoleType
			   ,Status
			   ,IsDefault
			   ,ActionDate
			   ,ActionIp
			   ,ActionUser
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
