USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_error_management]    Script Date: 03/10/2023 10:06:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sproc_error_management]
(
     @flag VARCHAR(10) 
	,@Id VARCHAR(50) = NULL
	,@ErrorDesc VARCHAR(500) = NULL
	,@ErrorScript VARCHAR(500) = NULL
	,@QueryString VARCHAR(500) = NULL
	,@ErrorCategory VARCHAR(500) = NULL
	,@ErrorSource VARCHAR(500) = NULL
)
AS
BEGIN
	IF ISNULL(@flag, '') = 'i' --inert
	BEGIN
		INSERT INTO tbl_error_log
	(
		ErrorDesc
	   ,ErrorScript
	   ,QueryString
	   ,ErrorCategory
	   ,ErrorSource
	   ,ActionDate
	)

	VALUES
	(
		@ErrorDesc
	   ,@ErrorScript
	   ,@QueryString
	   ,@ErrorCategory
	   ,@ErrorSource
	   ,GETDATE()
	)

		SELECT 
		   0 Code
	      ,@ErrorDesc AS Message
	      ,@ErrorScript
	      ,@QueryString
	      ,@ErrorCategory
	      ,@ErrorSource
		  ,SCOPE_IDENTITY() Id

		RETURN;
	END
END
GO


