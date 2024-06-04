USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_tbl_host_details]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter trigger [dbo].[TRG_tbl_host_details]
ON [dbo].[tbl_host_details]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.tbl_host_details_audit
		(
			Sno
		   ,AgentId
		   ,HostId
		   ,HostName
		   ,Position
		   ,Rank
		   ,DOB
		   ,ConstellationGroup
		   ,Height
		   ,BloodType
		   ,PreviousOccupation
		   ,LiquorStrength
		   ,Status
		   ,ImagePath
		   ,Address
		   ,HostNameJapanese
		   ,HostIntroduction
		   ,Title
		   ,OtherPosition
		   ,Thumbnail
		   ,Icon
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
		   ,HostId
		   ,HostName
		   ,Position
		   ,Rank
		   ,DOB
		   ,ConstellationGroup
		   ,Height
		   ,BloodType
		   ,PreviousOccupation
		   ,LiquorStrength
		   ,Status
		   ,ImagePath
		   ,Address
		   ,HostNameJapanese
		   ,HostIntroduction
		   ,Title
		   ,OtherPosition
		   ,Thumbnail
		   ,Icon
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
	END;
	ELSE IF EXISTS (SELECT 1 FROM DELETED)
	BEGIN
		INSERT INTO dbo.tbl_host_details_audit
		(
			Sno
		   ,AgentId
		   ,HostId
		   ,HostName
		   ,Position
		   ,Rank
		   ,DOB
		   ,ConstellationGroup
		   ,Height
		   ,BloodType
		   ,PreviousOccupation
		   ,LiquorStrength
		   ,Status
		   ,ImagePath
		   ,Address
		   ,HostNameJapanese
		   ,HostIntroduction
		   ,Title
		   ,OtherPosition
		   ,Thumbnail
		   ,Icon
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
		   ,HostId
		   ,HostName
		   ,Position
		   ,Rank
		   ,DOB
		   ,ConstellationGroup
		   ,Height
		   ,BloodType
		   ,PreviousOccupation
		   ,LiquorStrength
		   ,Status
		   ,ImagePath
		   ,Address
		   ,HostNameJapanese
		   ,HostIntroduction
		   ,Title
		   ,OtherPosition
		   ,Thumbnail
		   ,Icon
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
END;
