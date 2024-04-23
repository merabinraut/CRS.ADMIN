USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_point_category_select]    Script Date: 4/23/2024 3:42:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sproc_point_category_select]
( 
    @RoleTypeId bigint=NULL,
     @Id bigint=NULL,
     @Skip INT = 0,
     @Take INT = 10
)
AS
BEGIN
	SET NOCOUNT ON;
	If isnull(@Id ,'') =''
	   BEGIN
             SELECT 
                 pc.Id,
                 pc.CategoryName,
                 pc.Description,
                 pc.RoleType,
				 sd.StaticDataLabel RoleName,
                 pc.ActionDate,
                 pc.ActionIp,
                 pc.ActionUser,
				 pc.Status
             FROM
                 [dbo].[tbl_point_category] as pc  WITH (NOLOCK)
			INNER JOIN 
				 tbl_static_data as sd WITH (NOLOCK) on sd.StaticDataType=1 and sd.StaticDataValue=@RoleTypeId
			 WHERE 
			      RoleType=@RoleTypeId	 
              ORDER BY
			      CategoryName ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
            RETURN;
	     END
	  ELSE
	     BEGIN

            SELECT 
                 pc.Id,
                 pc.CategoryName,
                 pc.Description,
                 pc.RoleType,
				 sd.StaticDataLabel RoleName,
                 pc.ActionDate,
                 pc.ActionIp,
                 pc.ActionUser,
				 pc.Status
            FROM
                [dbo].[tbl_point_category] as pc  WITH (NOLOCK)
			INNER JOIN 
				 tbl_static_data as sd WITH (NOLOCK) on sd.StaticDataType=1 and sd.StaticDataValue=@RoleTypeId
			WHERE 
			     RoleType=@RoleTypeId and pc.Id=@Id            
            RETURN;
	     END
END;
GO


