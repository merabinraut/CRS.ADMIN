USE [CRS_V2]
GO
/****** Object:  Trigger [dbo].[TRG_AdminToClub]    Script Date: 6/4/2024 11:20:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter trigger [dbo].[TRG_AdminToClub]
ON [dbo].[AdminToClub]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM INSERTED)
	BEGIN
		INSERT INTO dbo.AdminToClub_audit
		(
			TransctionId
			,Amount
			,Points
			,ClubId
			,BankBoucher
			,Remark
			,Status
			,ActionUserId
			,ActionDate
			,TriggerLogUser
			,TriggerAction
			,TriggerActionLocalDate
			,TriggerActionUTCDate
		)
		SELECT TransctionId
			,Amount
			,Points
			,ClubId
			,BankBoucher
			,Remark
			,Status
			,ActionUserId
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
		INSERT INTO dbo.AdminToClub_audit
		(
			TransctionId
			,Amount
			,Points
			,ClubId
			,BankBoucher
			,Remark
			,Status
			,ActionUserId
			,ActionDate
			,TriggerLogUser
			,TriggerAction
			,TriggerActionLocalDate
			,TriggerActionUTCDate
		)
		SELECT TransctionId
			,Amount
			,Points
			,ClubId
			,BankBoucher
			,Remark
			,Status
			,ActionUserId
			,ActionDate
			  ,SYSTEM_USER
			  ,'DELETE'
			  ,GETDATE()
			  ,GETUTCDATE()
		FROM DELETED;
		RETURN;
	END
END;
