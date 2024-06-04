use [CRS_V2]
go

ALTER trigger [dbo].[TRG_tbl_commission_category_detail]
ON [dbo].[tbl_commission_category_detail]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_commission_category_detail_audit
		(
			Sno
			,CategoryDetailId
			,CategoryId
			,AdminCommissionTypeId
			,FromAmount
			,ToAmount
			,CommissionType
			,CommissionValue
			,CommissionPercentageType
			,MinCommissionValue
			,MaxCommissionValue
			,Status
			,ActionUser
			,ActionIP
			,ActionDate
			,TriggerLogUser
			,TriggerAction
			,TriggerActionLocalDate
			,TriggerActionUTCDate
		)
		SELECT Sno
				,CategoryDetailId
				,CategoryId
				,AdminCommissionTypeId
				,FromAmount
				,ToAmount
				,CommissionType
				,CommissionValue
				,CommissionPercentageType
				,MinCommissionValue
				,MaxCommissionValue
				,Status
				,ActionUser
				,ActionIP
				,ActionDate
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_commission_category_detail_audit
		(
			Sno
			,CategoryDetailId
			,CategoryId
			,AdminCommissionTypeId
			,FromAmount
			,ToAmount
			,CommissionType
			,CommissionValue
			,CommissionPercentageType
			,MinCommissionValue
			,MaxCommissionValue
			,Status
			,ActionUser
			,ActionIP
			,ActionDate
			,TriggerLogUser
			,TriggerAction
			,TriggerActionLocalDate
			,TriggerActionUTCDate
		)
		SELECT Sno
				,CategoryDetailId
				,CategoryId
				,AdminCommissionTypeId
				,FromAmount
				,ToAmount
				,CommissionType
				,CommissionValue
				,CommissionPercentageType
				,MinCommissionValue
				,MaxCommissionValue
				,Status
				,ActionUser
				,ActionIP
				,ActionDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
GO
