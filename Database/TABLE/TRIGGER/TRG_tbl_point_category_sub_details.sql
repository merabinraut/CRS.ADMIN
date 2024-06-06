USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_point_category_sub_details]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter trigger [dbo].[TRG_tbl_point_category_sub_details]
ON [dbo].[tbl_point_category_sub_details]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_point_category_sub_details_audit
		(
			Id
		    ,CategoryId
		    ,PointValue2
		    ,PointValue3
		    ,PointType2
		    ,PointType3
		    ,MinValue2
		    ,MaxValue2
		    ,MinValue3
		    ,MaxValue3
		    ,ActionDate
		    ,ActionIp
		    ,TriggerLogUser
		    ,TriggerAction
		    ,TriggerActionLocalDate
		    ,TriggerActionUTCDate
		)
		SELECT Id
				,CategoryId
				,PointValue2
				,PointValue3
				,PointType2
				,PointType3
				,MinValue2
				,MaxValue2
				,MinValue3
				,MaxValue3
				,ActionDate
				,ActionIp
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN;
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_point_category_sub_details_audit
		(
			Id
		    ,CategoryId
		    ,PointValue2
		    ,PointValue3
		    ,PointType2
		    ,PointType3
		    ,MinValue2
		    ,MaxValue2
		    ,MinValue3
		    ,MaxValue3
		    ,ActionDate
		    ,ActionIp
		    ,TriggerLogUser
		    ,TriggerAction
		    ,TriggerActionLocalDate
		    ,TriggerActionUTCDate
		)
		SELECT Id
				,CategoryId
				,PointValue2
				,PointValue3
				,PointType2
				,PointType3
				,MinValue2
				,MaxValue2
				,MinValue3
				,MaxValue3
				,ActionDate
				,ActionIp
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
