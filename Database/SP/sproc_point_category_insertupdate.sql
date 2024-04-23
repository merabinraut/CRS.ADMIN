USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_point_category_insertupdate]    Script Date: 4/23/2024 3:41:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sproc_point_category_insertupdate]
@Id bigint,
    @CategoryName NVARCHAR(100),
    @Description NVARCHAR(255),
    @RoleType NVARCHAR(50),
    @ActionIp VARCHAR(50),
    @ActionUser VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
	If ISNULL(@Id,'')=''
	BEGIN
	If EXISTS(
	        SELECT 
			       'x' 
			FROM 
			     tbl_point_category as pc  WITH (NOLOCK)
			INNER JOIN 
			      tbl_static_data as sd WITH (NOLOCK) on sd.StaticDataType=1 and sd.StaticDataValue=@RoleType
			WHERE 
			     CategoryName=@CategoryName and RoleType=@RoleType)
	BEGIN
	 SELECT 1 Code,
               'Duplicate category name' Message;
        RETURN;
	END

    INSERT INTO [dbo].[tbl_point_category] (
        [CategoryName],
        [Description],
        [RoleType],
        [ActionDate],
        [ActionIp],
        [ActionUser],
		status
    )
    VALUES (
        @CategoryName,
        @Description,
        @RoleType,
        GetDate(),
        @ActionIp,
        @ActionUser,
		'A'
    );
	 SELECT 0 Code,
               'Category added successfully' Message;
        RETURN;
	END
	ELSE
	BEGIN
	If EXISTS( SELECT 
			       'x' 
			FROM 
			     tbl_point_category as pc  WITH (NOLOCK)
			INNER JOIN 
			      tbl_static_data as sd WITH (NOLOCK) on sd.StaticDataType=1 and sd.StaticDataValue=@RoleType
			WHERE 
			      CategoryName=@CategoryName and RoleType=@RoleType and pc.Id <>@Id
			)
	BEGIN
	 SELECT 1 Code,
               'Duplicate category name' Message;
        RETURN;
	END

	UPDATE [dbo].[tbl_point_category]
    SET 
        CategoryName = ISNULL(@CategoryName, CategoryName),
        Description = ISNULL(@Description, Description),
        RoleType= ISNULL(@RoleType, RoleType),
        ActionDate = GETDATE(),
        ActionIp= ISNULL(@ActionIp, ActionIp),
        ActionUser= ISNULL(@ActionUser, ActionUser)
    WHERE
        Id = @Id;

	 SELECT 0 Code,
               'Category updated successfully' Message;
        RETURN;
END
END;
GO


