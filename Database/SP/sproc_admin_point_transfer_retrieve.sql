USE [CRS.UAT_V2]
GO
/****** Object:  StoredProcedure [dbo].[sproc_admin_point_transfer_retrieve]    Script Date: 5/17/2024 4:54:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sproc_admin_point_transfer_retrieve]
(     
	@Point DECIMAL(18, 2)=NULL,
	@TransactionType varchar(20)=NULL,
	@Remarks NVARCHAR(max)=NULL,
	@Image VARCHAR(max)=NULL,
	@UserTypeId bigint=NULL,
	@UserId bigint=NULL,
    @ActionUser VARCHAR(50)=NULL,
	@ActionIp VARCHAR(50)=NULL,
	@ActionUserId bigint=NULL
    
)
AS
BEGIN TRY
	SET NOCOUNT ON;
    declare @Sno bigint, @ErrorDesc VARCHAR(MAX), @adminPoint DECIMAL(18, 2)=NULL, @userPoint DECIMAL(18, 2)=NULL
	IF NOT EXISTS(
	        SELECT top(1) 'x' 
			FROM 
			    tbl_users 
			where 
			     AgentId=@UserId and RoleType=@UserTypeId
				 )
	BEGIN
	 SELECT 1 Code,
               'User details not found' Message;
			   RETURN;

	END


	If @TransactionType='Transfer'
	BEGIN
		IF NOT EXISTS(
	        SELECT top(1) 'x' 
			FROM 
			    tbl_System_Balance 			
				 )
	          BEGIN
	           SELECT 1 Code,
                      'Points not available' Message;
	           RETURN;
	          END

	   SELECT
	        @adminPoint=isnull( Amount,0)
       FROM
	        tbl_System_Balance 

		If @adminPoint < @Point
		BEGIN
		   SELECT 1 Code,
                  'Points not available' Message;
			RETURN;
		END

   declare @TxnId nvarchar(100)=NULL,@prefix varchar(10)=NULL
	,@Lastnumber bigint=0,@previousLastnumber bigint=0,@remainingLastnumber bigint=0
	--print 1
	BEGIN TRANSACTION InsertUpdate;
	
	 SELECT @Lastnumber= LastNumber,@prefix= Prefix FROM tbl_billing_type WHERE BillType='AdminTransferPoint'

	  SET @previousLastnumber=ISNULL(( select top 1 SUBSTRING(TransactionId, LEN(@prefix) + 1, LEN(TransactionId) - LEN(@prefix)) AS num_value  from tbl_AdminTransferPoint  order by id desc),@Lastnumber)
      --SET @TxnId=( select CONCAT(@prefix,@Lastnumber) )

	   IF EXISTS(SELECT 'x' from tbl_AdminTransferPoint where TransactionId=(select CONCAT(@prefix,@Lastnumber)) )
	     BEGIN
	   
	        set  @remainingLastnumber=cast( @previousLastnumber as bigint)-cast( @Lastnumber as bigint)

	        UPDATE  tbl_billing_type SET LastNumber=@Lastnumber+@remainingLastnumber  where BillType='AdminTransferPoint'

	        SET @Lastnumber=@Lastnumber+@remainingLastnumber

	        SET @TxnId=( select CONCAT(@prefix,@Lastnumber) )

	   END
	   ELSE
	   BEGIN
	       SET @TxnId=( select CONCAT(@prefix,@Lastnumber) )
		   UPDATE  tbl_billing_type SET LastNumber= cast(@Lastnumber as int) + 1  where BillType='AdminTransferPoint'
	   END
	  -- print @TxnId
	  	

        INSERT INTO [dbo].[tbl_AdminTransferPoint]
        (
           
			TransactionId,
			 AgentId,
	    	Amount,
	    	Remark,
	    	TrancationType,
			Status,
			Image,
			ActionDate,
			ActionUser
        )
        VALUES
         (  
		 
		   @TxnId,
		     @UserId,
            @Point,      
			@Remarks,
			'7',
			'A',
			@Image,
            GETDATE(),     
	    	@ActionUser
        )
		SET @Sno = SCOPE_IDENTITY();

	

		--------DR (Admin)-------------
		INSERT INTO [dbo].[tbl_PointLogs]
        (
            TranscationId,
	    	UserId,
	    	RoleType,
	    	TransactionType,
            Point,
            TransactionDate,
			TransactionMode,
			ActionUser,
			Remark,
			SystemRemark
        )
        VALUES
         (   
		    @Sno,
			 0,
			'2',
			'7',
            @Point,       
            GETDATE(),
			'DR',
	    	@ActionUser,
			@Remarks,
			''
        )

       --------CR (Users)-------------
	   INSERT INTO [dbo].[tbl_PointLogs]
        (
            TranscationId,
	    	UserId,
	    	RoleType,
	    	TransactionType,
            Point,
            TransactionDate,
			TransactionMode,
			ActionUser,
			Remark,
			SystemRemark
        )
        VALUES
         (   
		    @Sno,
			@UserId,
			@UserTypeId,
			'7',
            @Point,       
            GETDATE(),
			'CR',
	    	@ActionUser,
			@Remarks,
			''
        )


		IF NOT EXISTS(SELECT  'x' FROM tbl_agent_balance where AgentId=@UserId and RoleType=@UserTypeId )
		 BEGIN
		      INSERT INTO [dbo].[tbl_agent_balance]
              (
                AgentId,
	          	Amount,	    
				creditLimit,
	          	CreditPointUse,	
				Status,
	          	ActionUser,	
				ActionDate,
	          	RoleType
              )
              VALUES
               (  
			   @UserId,
               @Point,       
                0,
				0,
			   'A',
			   @ActionUser,
			   GETDATE(),
			   @UserTypeId
              )
		 END
		 ELSE
		 BEGIN
		 --select @PreviousPoint=Amount FROM tbl_System_Balance 
		 UPDATE tbl_agent_balance Set Amount=Amount+ @Point where AgentId=@UserId and RoleType=@UserTypeId
		 
		 END

		 UPDATE  tbl_System_Balance  Set Amount=Amount - @Point
	     SELECT 0 Code,
               'Points transfered successfully' Message;
 
END
ELSE
BEGIN
IF NOT EXISTS(
	        SELECT  'x' 
			FROM 
			    tbl_agent_balance 
		    WHERE
			    AgentId=@UserId and RoleType=@UserTypeId  			
				 )
	          BEGIN
	           SELECT 1 Code,
                      'Points not available' Message;
					   RETURN;
	          END

	   SELECT
	        @userPoint=isnull( Amount,0)
       FROM
	        tbl_agent_balance 
       WHERE
			    AgentId=@UserId and RoleType=@UserTypeId  	

		If @userPoint < @Point
		BEGIN
		   SELECT 1 Code,
                  'Points not available' Message;
				   RETURN;
		END
		BEGIN TRANSACTION InsertUpdate;
	
        INSERT INTO [dbo].[tbl_AdminTransferPoint]
        (
            AgentId,
	    	Amount,
	    	Remark,
	    	TrancationType,
			Status,
			Image,
			ActionDate,
			ActionUser
        )
        VALUES
         (  
		   @UserId,
            @Point,      
			@Remarks,
			'3',
			'A',
			@Image,
            GETDATE(),     
	    	@ActionUser
        )
		SET @Sno = SCOPE_IDENTITY();

		--------DR (Admin)-------------
		INSERT INTO [dbo].[tbl_PointLogs]
        (
            TranscationId,
	    	UserId,
	    	RoleType,
	    	TransactionType,
            Point,
            TransactionDate,
			TransactionMode,
			ActionUser,
			Remark,
			SystemRemark
        )
        VALUES
         (   
		    @Sno,
			@UserId,
			@UserTypeId,
			'3',
            @Point,       
            GETDATE(),
			'DR',
	    	@ActionUser,
			@Remarks,
			''
        )

       --------CR (Users)-------------
	   INSERT INTO [dbo].[tbl_PointLogs]
        (
            TranscationId,
	    	UserId,
	    	RoleType,
	    	TransactionType,
            Point,
            TransactionDate,
			TransactionMode,
			ActionUser,
			Remark,
			SystemRemark
        )
        VALUES
         (   
		    @Sno,
			0,
			'2',
			'3',
            @Point,       
            GETDATE(),
			'CR',
	    	@ActionUser,
			@Remarks,
			''
        )

		 --INSERT INTO [dbo].[tbl_AdminGenerateAmount]
   --     (
   --         Amount,
	  --  	GenerateDate,
	  --  	Remark,
	  --  	Createby         
   --     )
   --     VALUES
   --      (   
   --         @Point,       
   --         GETDATE(),
			--@Remarks,
	  --  	@ActionUserId

   --     )

		
		 UPDATE  
		      tbl_agent_balance  
		 SET
		     Amount=Amount - @Point  
		 WHERE
		    AgentId=@UserId and RoleType=@UserTypeId

		IF NOT EXISTS(SELECT top(1) 'x' FROM tbl_System_Balance )
		 BEGIN
		 INSERT INTO [dbo].[tbl_System_Balance]
              (
                Amount,
	          	LastUpdate	          	
              )
              VALUES
               (      
                  @Point,       
                  GETDATE()	          	
              )
		 END
		 ELSE
		 BEGIN

		 UPDATE tbl_System_Balance Set Amount=Amount+ @Point
		 
		 END



 SELECT 0 Code,
               'Points retrieved successfully' Message;
END
	  COMMIT TRANSACTION InsertUpdate;
	  

       RETURN;
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
    (@ErrorDesc, 'sproc_admin_point_transfer_retrieve', 'SQL', 'SQL', 'sproc_admin_point_transfer_retrieve',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
