USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_affiliate]    Script Date: 6/14/2024 3:46:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER TRIGGER [dbo].[TRG_tbl_affiliate]
ON [dbo].[tbl_affiliate]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_affiliate_audit
		(
			Sno
		   ,AgentId
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,EmailAddress
		   ,DOB
		   ,Gender
		   ,ProfileImage
		   ,PointsCategoryId
		   ,PostalCode
		   ,Prefecture
		   ,City
		   ,Street
		   ,BuildingNo
		   ,BusinessType
		   ,CEOName
		   ,CEONameFurigana
		   ,CompanyName
		   ,CompanyAddress
		   ,AffiliateNameFurigana
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate

		)

		SELECT Sno
			  ,AgentId
			  ,FirstName
			  ,LastName
			  ,MobileNumber
			  ,EmailAddress
			  ,DOB
			  ,Gender
			  ,ProfileImage
			  ,PointsCategoryId
			  ,PostalCode
		      ,Prefecture
		      ,City
		      ,Street
		      ,BuildingNo
		      ,BusinessType
		      ,CEOName
		      ,CEONameFurigana
		      ,CompanyName
		      ,CompanyAddress
			  ,AffiliateNameFurigana
			  ,ActionUser
			  ,ActionIP
			  ,ActionPlatform
			  ,ActionDate
			  ,SYSTEM_USER
			  ,CASE WHEN EXISTS (SELECT 1 FROM DELETED) THEN 'UPDATE' ELSE 'INSERT' END
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM INSERTED;
		RETURN; 
	END
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_affiliate_audit
		(
			Sno
		   ,AgentId
		   ,FirstName
		   ,LastName
		   ,MobileNumber
		   ,EmailAddress
		   ,DOB
		   ,Gender
		   ,ProfileImage
		   ,PointsCategoryId
		   ,PostalCode
		   ,Prefecture
		   ,City
		   ,Street
		   ,BuildingNo
		   ,BusinessType
		   ,CEOName
		   ,CEONameFurigana
		   ,CompanyName
		   ,CompanyAddress
		   ,AffiliateNameFurigana
		   ,ActionUser
		   ,ActionIP
		   ,ActionPlatform
		   ,ActionDate
		   ,TriggerLogUser
		   ,TriggerAction
		   ,TriggerActionLocalDate
		   ,TriggerActionUTCDate
		)

		SELECT Sno
			  ,AgentId
			  ,FirstName
			  ,LastName
			  ,MobileNumber
			  ,EmailAddress
			  ,DOB
			  ,Gender
			  ,ProfileImage
			  ,PointsCategoryId
			  ,PostalCode
		      ,Prefecture
		      ,City
		      ,Street
		      ,BuildingNo
		      ,BusinessType
		      ,CEOName
		      ,CEONameFurigana
		      ,CompanyName
		      ,CompanyAddress
			  ,AffiliateNameFurigana
			  ,ActionUser
			  ,ActionIP
			  ,ActionPlatform
			  ,ActionDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN; 
	END
END
