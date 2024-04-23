USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_point_category_detail_delete]    Script Date: 4/23/2024 3:40:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sproc_point_category_detail_delete]
    @Id bigint=NULL,
    @CategoryId bigint=NULL,
	@RoleType int=NULL,   
	@Status varchar(5)=NULL,   
    @ActionIp VARCHAR(50)=NULL,
	@ActionUser NVARCHAR(250)=NULL
AS
BEGIN
    SET NOCOUNT ON;


	If NOT EXISTS(
	         SELECT 
			       'x' 
			  FROM 
				   tbl_point_category_details as pcd WITH (NOLOCK)
              INNER JOIN 
			        tbl_point_category as pc WITH (NOLOCK) on pc.Id=pcd.CategoryId
			  WHERE 
			      RoleType=@RoleType and pcd.CategoryId=@CategoryId  and pcd.Id=@Id
			 )
			 BEGIN
			  SELECT 1 Code,
               'Invalid details' Message;
               RETURN;
	END

	If  EXISTS(
	         SELECT 
			       'x' 
			  FROM 
				   tbl_point_category_details as pcd WITH (NOLOCK)
              INNER JOIN 
			        tbl_point_category as pc WITH (NOLOCK) on pc.Id=pcd.CategoryId
			  WHERE 
			      RoleType=@RoleType and pcd.CategoryId=@CategoryId  and pcd.Id=@Id and pcd.Status='D'
			 )
			 BEGIN
			  SELECT 1 Code,
                    'Already deleted' Message;
               RETURN;
	END

   Declare @message nvarchar(50)
   If @Status='D'
	BEGIN
	  SET @message='Category point slab deleted successfully'
	END
	
	

	 UPDATE [dbo].[tbl_point_category_details]
    SET
        Status=@Status,
        [ActionDate] = GetDate(),
        [ActionIp] = @ActionIp,
		ActionUser=@ActionUser
    WHERE
         [Id] = @Id 
	 SELECT 0 Code,
               @message  Message;
        RETURN;

END;
GO


