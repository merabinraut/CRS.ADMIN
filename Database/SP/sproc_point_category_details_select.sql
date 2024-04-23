USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_point_category_details_select]    Script Date: 4/23/2024 3:41:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sproc_point_category_details_select]
( 
    @RoleTypeId bigint=NULL,
	 @CategoryId bigint=NULL,
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
			     pc.CategoryName,
				 cast(FromAmount as int) as FromAmount,
				 cast(ToAmount as int) as ToAmount, 
				 cast(PointValue as int) as PointValue,
				 PointType ,
				 pc.RoleType,
				 pc.Id CategoryId,
				 pcd.Id,
				 cast(PointValue2 as int) as PointValue2,
				 PointType2 ,
				 cast(PointValue3 as int) as PointValue3,
				 PointType3,
				 cast(MinValue as int) as MinValue,
				 cast(MaxValue as int) as MaxValue,
				 cast(MinValue2 as int) as MinValue2,
				 cast(MaxValue2 as int) as MaxValue2,
				 cast(MinValue3 as int) as MinValue3,
				 cast(MaxValue3 as int) as MaxValue3
		    FROM  
			     tbl_point_category_details as pcd WITH (NOLOCK)
            INNER JOIN 
			     tbl_point_category as pc WITH (NOLOCK) on pc.Id=pcd.CategoryId
			LEFT JOIN
			     tbl_point_category_sub_details as pcsd WITH (NOLOCK) on pcsd.CategoryId=pcd.Id
			 WHERE 
			      RoleType=@RoleTypeId and pcd.CategoryId=@CategoryId and isnull(pcd.Status,'')<>'D'
              ORDER BY
			      pcd.ActionDate ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
            RETURN;
	     END
	  ELSE
	     BEGIN

            SELECT
			       pc.CategoryName,
				 cast(FromAmount as int) as FromAmount,
				 cast(ToAmount as int) as ToAmount, 
				 cast(PointValue as int) as PointValue,
				 PointType ,
				 pc.RoleType,
				 pc.Id CategoryId,
				 pcd.Id,
				 cast(PointValue2 as int) as PointValue2,
				 PointType2 ,
				 cast(PointValue3 as int) as PointValue3,
				 PointType3,
				 cast(MinValue as int) as MinValue,
				 cast(MaxValue as int) as MaxValue,
				 cast(MinValue2 as int) as MinValue2,
				 cast(MaxValue2 as int) as MaxValue2,
				 cast(MinValue3 as int) as MinValue3,
				 cast(MaxValue3 as int) as MaxValue3
		    FROM 
			    tbl_point_category_details as pcd WITH (NOLOCK)
            INNER JOIN 
			     tbl_point_category as pc WITH (NOLOCK) on pc.Id=pcd.CategoryId and isnull(pcd.Status,'')<>'D'
             LEFT JOIN
			     tbl_point_category_sub_details as pcsd WITH (NOLOCK) on pcsd.CategoryId=pcd.Id
			 WHERE 
			      RoleType=@RoleTypeId and pcd.CategoryId=@CategoryId  AND pcd.Id=@Id         
            RETURN;
	     END
END;
GO


