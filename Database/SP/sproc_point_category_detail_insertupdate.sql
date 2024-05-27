USE [CRS_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_point_category_detail_insertupdate]    Script Date: 4/23/2024 3:40:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sproc_point_category_detail_insertupdate]
    @Id bigint=NULL,
    @CategoryId bigint=NULL,
	@RoleType int=NULL,
    @FromAmount DECIMAL(18, 2)=NULL,
    @ToAmount DECIMAL(18, 2)=NULL,
    @PointType VARCHAR(5)=NULL,
    @PointValue DECIMAL(18, 2)=NULL,
    @MinValue DECIMAL(18, 2)=NULL,
    @MaxValue DECIMAL(18, 2)=NULL,
    @ActionIp VARCHAR(50)=NULL,
	@ActionUser NVARCHAR(250)=NULL,
	@MinValue2 DECIMAL(18, 2) = NULL,
    @MaxValue2 DECIMAL(18, 2) = NULL,
	@MinValue3 DECIMAL(18, 2) = NULL,
    @MaxValue3 DECIMAL(18, 2) = NULL,
	@PointType2 VARCHAR(5)=NULL,
    @PointValue2 DECIMAL(18, 2)=NULL,
	@PointType3 VARCHAR(5)=NULL,
    @PointValue3 DECIMAL(18, 2)=NULL
AS
BEGIN TRY
    SET NOCOUNT ON;
	declare @Sno bigint, @ErrorDesc VARCHAR(MAX) 
	If ISNULL(@Id,'')=''
	BEGIN
	
	If NOT EXISTS(
	         SELECT 
			       'x' 
			  FROM 
				   tbl_point_category WITH (NOLOCK)
			  WHERE 
			      RoleType=@RoleType and Id=@CategoryId and Status='A'
			 )
			 BEGIN
			  SELECT 1 Code,
               'Invalid details' Message;
               RETURN;
			 END


	    IF ISNULL(@FromAmount, 0) = ISNULL(@ToAmount, 0)
        BEGIN
            SELECT 1 Code,
                   'From amount and to amount value cannot be same' Message;

            RETURN;
        END;

        IF ISNULL(@FromAmount, 0) > ISNULL(@ToAmount, 0)
        BEGIN
            SELECT 1 Code,
                   'From amount cannot be greater than to amount value' Message;

            RETURN;
        END;


		IF @PointType = 'P'
           AND @PointValue > 150
        BEGIN
            SELECT 1 Code,
                   'Commission type percentage cannot have more than 150 percent' Message;

            RETURN;
        END;

       IF EXISTS
        (
            SELECT 'X'
            FROM 
			      dbo.tbl_point_category_details a WITH (NOLOCK)
			INNER JOIN 
			        tbl_point_category as pc WITH (NOLOCK) on pc.Id=a.CategoryId
            WHERE a.CategoryId = a.CategoryId
                  AND ISNULL(a.[Status], '') = 'A'
                  AND a.CategoryId = @CategoryId
				  AND RoleType=@RoleType
                  AND ISNULL(@FromAmount, 0)
                  BETWEEN ISNULL(a.FromAmount, 0) AND ISNULL(a.ToAmount, 0)
            UNION
            SELECT 'X'
            FROM
			       dbo.tbl_point_category_details a WITH (NOLOCK)
			INNER JOIN 
			        tbl_point_category as pc WITH (NOLOCK) on pc.Id=a.CategoryId
            WHERE a.CategoryId = a.CategoryId
                  AND a.CategoryId = @CategoryId
				  AND RoleType=@RoleType
                  AND ISNULL(a.[Status], '') = 'A'
                  AND ISNULL(@ToAmount, 0)
                  BETWEEN ISNULL(a.FromAmount, 0) AND ISNULL(a.ToAmount, 0)
        )
        BEGIN
            SELECT 1 Code,
                   'Slab already exist' Message;
            RETURN;
        END;

		


		IF @RoleType='6'
		BEGIN

             ---check min and max value for AffiliateA
             
             IF ISNULL(@MinValue, 0) > ISNULL(@MaxValue, 0)
             BEGIN
             SELECT 1 Code,
                     'Minimum commission value cannot be greater than maximum commission value for Affiliate A' Message;
             
             RETURN;
             END;
             
             ---check min and max value for AffiliateB
             
             IF ISNULL(@MinValue2, 0) > ISNULL(@MaxValue2, 0)
             BEGIN
             SELECT 1 Code,
                     'Minimum commission value cannot be greater than maximum commission value for Affiliate B' Message;
             
             RETURN;
             END;
             
             ---check min and max value for AffiliateC
             IF ISNULL(@MinValue3, 0) > ISNULL(@MaxValue3, 0)
             BEGIN
             SELECT 1 Code,
                     'Minimum commission value cannot be greater than maximum commission value for Affiliate C' Message;
             
             RETURN;
             END;
	   END

   BEGIN TRANSACTION InsertUpdate;
   INSERT INTO [dbo].[tbl_point_category_details]
    (
        [CategoryId],
        [FromAmount],
        [ToAmount],
        [PointType],
        [PointValue],
        [MinValue],
        [MaxValue],
        [ActionDate],
        [ActionIp],
		ActionUser,
		status
    )
    VALUES
    (
        @CategoryId,
        @FromAmount,
        @ToAmount,
        @PointType,
        @PointValue,
        @MinValue,
        @MaxValue,
        GETDATE(),
        @ActionIp,
		@ActionUser,
		'A'
    )
	SET @Sno = SCOPE_IDENTITY();
	If @RoleType='6'
	BEGIN
      INSERT INTO [dbo].[tbl_point_category_sub_details]
      (
          [CategoryId]
          ,[PointValue2]
          ,[PointValue3]
          ,[PointType2]
          ,[PointType3]
          ,[MinValue2]
          ,[MaxValue2]
          ,[MinValue3]
          ,[MaxValue3]
          ,[ActionDate]
          ,[ActionIp]
      )
      VALUES
      (
          @Sno,
          @PointValue2,
          @PointValue3,
          @PointType2,
          @PointType3,
          @MinValue2,
          @MaxValue2,
      	  @MinValue3,
          @MaxValue3,
          GETDATE(),
          @ActionIp		
      )
	END
	 SELECT 0 Code,
               'Category points added successfully' Message;
	  COMMIT TRANSACTION InsertUpdate;
       RETURN;
	END
	ELSE
	BEGIN


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

	   IF ISNULL(@FromAmount, 0) = ISNULL(@ToAmount, 0)
        BEGIN
            SELECT 1 Code,
                   'From amount and to amount value cannot be same' Message;

            RETURN;
        END;

        IF ISNULL(@FromAmount, 0) > ISNULL(@ToAmount, 0)
        BEGIN
            SELECT 1 Code,
                   'From amount cannot be greater than to amount value' Message;

            RETURN;
        END;


		IF @PointType = 'P'
           AND @PointValue > 150
        BEGIN
            SELECT 1 Code,
                   'Commission type percentage cannot have more than 150 percent' Message;

            RETURN;
        END;


	 IF EXISTS
        (
            SELECT 'X'
            FROM 
			      dbo.tbl_point_category_details a WITH (NOLOCK)
			INNER JOIN 
			        tbl_point_category as pc WITH (NOLOCK) on pc.Id=a.CategoryId
            WHERE a.CategoryId = a.CategoryId
                  AND ISNULL(a.[Status], '') = 'A'
                  AND a.CategoryId = @CategoryId
				  AND RoleType=@RoleType
				  AND a.Id<>@Id
                  AND ISNULL(@FromAmount, 0)
                  BETWEEN ISNULL(a.FromAmount, 0) AND ISNULL(a.ToAmount, 0)
            UNION
            SELECT 
			      'X'
            FROM 
			       dbo.tbl_point_category_details a WITH (NOLOCK)
			INNER JOIN 
			        tbl_point_category as pc WITH (NOLOCK) on pc.Id=a.CategoryId
            WHERE a.CategoryId = a.CategoryId
                  AND a.CategoryId = @CategoryId
				  AND a.Id<>@Id
				  AND RoleType=@RoleType
                  AND ISNULL(a.[Status], '') = 'A'
                  AND ISNULL(@ToAmount, 0)
                  BETWEEN ISNULL(a.FromAmount, 0) AND ISNULL(a.ToAmount, 0)
        )
        BEGIN
            SELECT 1 Code,
                   'Slab already exist' Message;
            RETURN;
        END;

	

		IF @RoleType='6'
		BEGIN
		      If NOT EXISTS(
	               SELECT 
		      	       'x' 
		      	  FROM 
		      		   tbl_point_category_details as pcd WITH (NOLOCK)
                    INNER JOIN 
		      	        tbl_point_category as pc WITH (NOLOCK) on pc.Id=pcd.CategoryId
					INNER JOIN  
					    tbl_point_category_sub_details as pcsd WITH (NOLOCK) on pcd.Id=pcsd.CategoryId
		      	  WHERE 
		      	      RoleType=@RoleType and pcd.CategoryId=@CategoryId  and pcd.Id=@Id 
		      	 )
	          BEGIN
	          		  SELECT 1 Code,
                         'Invalid details' Message;
                         RETURN;
	          END


             ---check min and max value for AffiliateA
             
             IF ISNULL(@MinValue, 0) > ISNULL(@MaxValue, 0)
             BEGIN
             SELECT 1 Code,
                     'Minimum commission value cannot be greater than maximum commission value for Affiliate A' Message;
             
             RETURN;
             END;
             
             ---check min and max value for AffiliateB
             
             IF ISNULL(@MinValue2, 0) > ISNULL(@MaxValue2, 0)
             BEGIN
             SELECT 1 Code,
                     'Minimum commission value cannot be greater than maximum commission value for Affiliate B' Message;
             
             RETURN;
             END;
             
             ---check min and max value for AffiliateC
             IF ISNULL(@MinValue3, 0) > ISNULL(@MaxValue3, 0)
             BEGIN
             SELECT 1 Code,
                     'Minimum commission value cannot be greater than maximum commission value for Affiliate C' Message;
             
             RETURN;
             END;
	   END


 BEGIN TRANSACTION InsertUpdate;
       
	 UPDATE [dbo].[tbl_point_category_details]
    SET
        [CategoryId] =isnull( @CategoryId,CategoryId),
        [FromAmount] = isnull(@FromAmount,FromAmount),
        [ToAmount] = isnull(@ToAmount,ToAmount),
        [PointType] = isnull(@PointType,PointType),
        [PointValue] = isnull(@PointValue,PointValue),
        [MinValue] = isnull(@MinValue,MinValue),
        [MaxValue] = isnull(@MaxValue,MaxValue),
        [ActionDate] = GetDate(),
        [ActionIp] = isnull(@ActionIp,ActionIp),
		ActionUser=isnull(@ActionUser,ActionUser)
      WHERE
         [Id] = @Id 


    IF @RoleType='6'
	BEGIN
		UPDATE 
		      [dbo].[tbl_point_category_sub_details]
        SET
             [PointValue2]=isnull( @PointValue2,PointValue2),
             [PointValue3]=isnull( @PointValue3,PointValue3),
             [PointType2]=isnull( @PointType2,PointType2),
             [PointType3]=isnull( @PointType3,PointType3),
             [MinValue2]=isnull( @MinValue2,MinValue2),
             [MaxValue2]=isnull( @MaxValue2,MaxValue2),
             [MinValue3]=isnull( @MinValue3,MinValue3),
             [MaxValue3]=isnull( @MaxValue3,MaxValue3),
             [ActionDate]=GETDATE(),
             [ActionIp]=@ActionIp
      WHERE
             [CategoryId] = @Id 
	END
	 SELECT 0 Code,
               'Category points updated successfully' Message;

	COMMIT TRANSACTION InsertUpdate;
        RETURN;
END
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION InsertUpdate;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_point_category_detail_insertupdate(Id: ' + ISNULL(@Id, '') + ')', 'SQL', 'SQL', 'sproc_point_category_detail_insertupdate',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;

GO


