USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_plans]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER trigger [dbo].[TRG_tbl_plans]
ON [dbo].[tbl_plans]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_plans_audit
		(
			Sno
		   ,PlanId
		   ,PlanName
		   ,PlanType
		   ,PlanTime
		   ,Price
		   ,StrikePrice
		   ,Liquor
		   ,Nomination
		   ,Remarks
		   ,PlanStatus
		   ,PlanImage
		   ,AdditionalValue1
		   ,AdditionalValue2
		   ,AdditionalValue3
		   ,PlanImage2
		   ,NoOfPeople
		   ,PlanCategory
		   ,IsStrikeOut
		   ,ActionUser
		   ,ActionIp
		   ,ActionPlatform
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,PlanId
			   ,PlanName
			   ,PlanType
			   ,PlanTime
			   ,Price
			   ,StrikePrice
			   ,Liquor
			   ,Nomination
			   ,Remarks
			   ,PlanStatus
			   ,PlanImage
			   ,AdditionalValue1
			   ,AdditionalValue2
			   ,AdditionalValue3
			   ,PlanImage2
			   ,NoOfPeople
			   ,PlanCategory
			   ,IsStrikeOut
			   ,ActionUser
			   ,ActionIp
			   ,ActionPlatform
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
		INSERT INTO dbo.tbl_plans_audit
		(
			Sno
		   ,PlanId
		   ,PlanName
		   ,PlanType
		   ,PlanTime
		   ,Price
		   ,StrikePrice
		   ,Liquor
		   ,Nomination
		   ,Remarks
		   ,PlanStatus
		   ,PlanImage
		   ,AdditionalValue1
		   ,AdditionalValue2
		   ,AdditionalValue3
		   ,PlanImage2
		   ,NoOfPeople
		   ,PlanCategory
		   ,IsStrikeOut
		   ,ActionUser
		   ,ActionIp
		   ,ActionPlatform
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)
		SELECT Sno
			   ,PlanId
			   ,PlanName
			   ,PlanType
			   ,PlanTime
			   ,Price
			   ,StrikePrice
			   ,Liquor
			   ,Nomination
			   ,Remarks
			   ,PlanStatus
			   ,PlanImage
			   ,AdditionalValue1
			   ,AdditionalValue2
			   ,AdditionalValue3
			   ,PlanImage2
			   ,NoOfPeople
			   ,PlanCategory
			   ,IsStrikeOut
			   ,ActionUser
			   ,ActionIp
			   ,ActionPlatform
			   ,ActionDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
