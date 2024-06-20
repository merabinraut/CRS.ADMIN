use [CRS_V2]
go

ALTER trigger [dbo].[TRG_tbl_commission_category]
ON [dbo].[tbl_commission_category]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_commission_category_audit
		(
			Sno
			,CategoryId
			,CategoryName
			,Description
			,Status
			,ActionUser
			,ActionIP
			,ActionDate
			,IsDefault
			,TriggerLogUser
			,TriggerAction
			,TriggerActionLocalDate
			,TriggerActionUTCDate
		)
		SELECT Sno
			  ,CategoryId
			  ,CategoryName
			  ,Description
			  ,Status
			  ,ActionUser
			  ,ActionIP
			  ,ActionDate
			  ,IsDefault
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_commission_category_audit
		(
			Sno
			,CategoryId
			,CategoryName
			,Description
			,Status
			,ActionUser
			,ActionIP
			,ActionDate
			,IsDefault
			,TriggerLogUser
			,TriggerAction
			,TriggerActionLocalDate
			,TriggerActionUTCDate
		)
		SELECT Sno
			  ,CategoryId
			  ,CategoryName
			  ,Description
			  ,Status
			  ,ActionUser
			  ,ActionIP
			  ,ActionDate
			  ,IsDefault
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
GO
