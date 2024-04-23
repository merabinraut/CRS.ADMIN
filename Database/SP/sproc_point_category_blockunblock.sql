USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_point_category_blockunblock]    Script Date: 4/23/2024 3:39:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sproc_point_category_blockunblock]
(
@Id bigint,
@RoleType NVARCHAR(50),
@ActionIp VARCHAR(50),
@ActionUser VARCHAR(50),
@Status VARCHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

	If NOT EXISTS( SELECT 
			       'x' 
			   FROM 
			        tbl_point_category as pc  WITH (NOLOCK)
			   INNER JOIN 
			      tbl_static_data as sd WITH (NOLOCK) on sd.StaticDataType=1 and sd.StaticDataValue=@RoleType 
			   WHERE  
			        RoleType=@RoleType and pc.Id =@Id
			  )
	BEGIN
	 SELECT 1 Code,
               'Category detail not found' Message;
        RETURN;
	END
	Declare @message nvarchar(50)
	If @Status='A'
	BEGIN
	set @message='Category detail unblocked successfully'
	END
	ELSE 
	BEGIN
	set @message='Category detail blocked successfully'
	END

	UPDATE [dbo].[tbl_point_category]
    SET 
        Status=@Status,
        ActionDate = GETDATE(),
        ActionIp= ISNULL(@ActionIp, ActionIp),
        ActionUser= ISNULL(@ActionUser, ActionUser)
    WHERE
        Id = @Id and RoleType=@RoleType 

	 SELECT 0 Code,
               @message Message;
     RETURN;
END;
GO


