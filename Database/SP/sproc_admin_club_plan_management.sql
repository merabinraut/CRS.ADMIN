USE [CRS_V2]
GO
/****** Object:  StoredProcedure [dbo].[sproc_admin_club_plan_management]    Script Date: 5/23/2024 1:03:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sproc_admin_club_plan_management] 
    @Flag VARCHAR(20) = '',
    @AgentId VARCHAR(20) = NULL,
	@ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
	@ClubPlanTypeId BIGINT = NULL,
    @Id BIGINT = NULL,
    @PlanListId BIGINT = NULL,
	@Description NVARCHAR(512) = NULL
AS
BEGIN TRY
	DECLARE @Sno VARCHAR(10),        
		@TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX)
	 SET NOCOUNT ON;

 IF ISNULL(@Flag, '') = 'cpi' -- club plan ientity
    BEGIN
        SELECT sd.StaticDataLabel AS English,
               sd.StaticDataValue,
               sd.AdditionalValue1 AS japanese,
               CASE
                   WHEN sd.StaticDataLabel = 'plan' THEN
                       'Dropdown'
                   WHEN sd.StaticDataLabel = 'Maximum No Of People' THEN
                       'TextBox'
                   WHEN sd.StaticDataLabel = 'Status' THEN
                       'Toggle'
                   ELSE
                       'Time'
               END AS inputtype,
               REPLACE(sd.StaticDataLabel, ' ', '') name
        FROM dbo.tbl_static_data_type st
            JOIN dbo.tbl_static_data AS sd
                ON sd.StaticDataType = st.StaticDataType
        WHERE st.StaticDataType = 38
              AND sd.Status = 'A';
END;



ELSE IF ISNULL(@Flag, '') = 'planlst' --  admin plan list
    BEGIN
       SELECT 
	         planId,PlanName 
	   FROM 
	         tbl_plans WITH (NOLOCK) 
	   WHERE PlanCategory IN ('1','2') AND PlanStatus='A' 
    END;
 
 ELSE IF ISNULL(@Flag, '') = 'plan_nf' --  get plan list not in club plan
    BEGIN
     SELECT 
	        planId,PlanName 
	FROM tbl_plans Where planStatus='A' AND  PlanCategory IN ('1','2') and planid not in  ( select Description from  tbl_club_plan where 
				 ClubId = @AgentId and ClubPlanTypeId=1) 
    END;

ELSE IF ISNULL(@Flag, '') = 'gpld' --  admin plan list
    BEGIN
        SELECT 0 Code,
               cp.Id,
               ClubId,
               ClubPlanType,
               ClubPlanTypeId StaticDataValue,
               cp.Description,
               cp.PlanListId,
               sd.StaticDataLabel AS English,
               sd.AdditionalValue1 AS japanese,
               CASE
                   WHEN cp.ClubPlanTypeId = '1' THEN
                       'Dropdown'
                   WHEN cp.ClubPlanTypeId = '4' THEN
                       'TextBox'
                   ELSE
                       'Time'
               END AS inputtype,
               REPLACE(sd.StaticDataLabel, ' ', '') name,
			 case when cp.PlanListId = ( SELECT a.PlanListId
					FROM dbo.tbl_club_plan a WITH (NOLOCK)
					INNER JOIN dbo.tbl_plans b WITH (NOLOCK) ON b.PlanId = a.Description AND ClubPlanTypeId = 1
					WHERE ISNULL(b.PlanStatus, '') in ('B')  
						AND a.ClubId = @AgentId and a.PlanListId=cp.PlanListId
						  ) then 'B' else  PlanStatus end PlanStatus
        FROM dbo.tbl_club_details AS ad WITH (NOLOCK)
            INNER JOIN tbl_club_plan AS cp WITH (NOLOCK)
                ON cp.ClubId = ad.AgentId
			LEFT JOIN tbl_plans  AS p WITH (NOLOCK)
                ON cp.Description = cast(p.PlanId as varchar(20)) and cp.ClubPlanTypeId='1'
            INNER JOIN dbo.tbl_static_data_type AS SDT
                ON SDT.StaticDataType = cp.ClubPlanType
            INNER JOIN dbo.tbl_static_data AS sd WITH (NOLOCK)
                ON sd.StaticDataType = CAST(SDT.StaticDataType AS VARCHAR(50))
                   AND sd.StaticDataValue = cp.ClubPlanTypeId		
        WHERE cp.ClubId = @AgentId
        ORDER BY cp.PlanListId,
                 sd.StaticDataValue ASC;
    END;
  

ELSE IF (@Flag='getplanlist')
BEGIN
SELECT 
    COALESCE(t1.ClubId, 415) AS ClubId,
    COALESCE(t1.ClubPlanType, 38) AS ClubPlanType,
    t2.StaticDataLabel AS English,
    t2.AdditionalValue1 AS Japanese,
    t2.StaticDataValue,
    t2.StaticDataDescription,
    t1.PlanListId,
    CASE
        WHEN t1.ClubPlanTypeId = '1' THEN 'Dropdown'
        WHEN t1.ClubPlanTypeId = '4' THEN 'TextBox'
		WHEN t1.ClubPlanTypeId = '5' THEN 'toggle'
        ELSE 'Time'
    END AS inputtype,
    t1.Description,
	REPLACE(t2.StaticDataLabel, ' ', '') name,
			 case when t1.PlanListId = ( SELECT a.PlanListId
					FROM dbo.tbl_club_plan a WITH (NOLOCK)
					INNER JOIN dbo.tbl_plans b WITH (NOLOCK) ON b.PlanId = a.Description AND ClubPlanTypeId = 1
					WHERE ISNULL(b.PlanStatus, '') in ('B')  
						AND a.ClubId = @AgentId and a.PlanListId=t1.PlanListId
						  ) then 'B' else  PlanStatus end PlanStatus,
	 t1.Id

FROM 
    tbl_club_plan t1
JOIN
    tbl_static_data t2 ON t1.ClubPlanTypeId = t2.StaticDataValue
	LEFT JOIN tbl_plans  AS p WITH (NOLOCK)
                ON t1.Description = cast(p.PlanId as varchar(20)) and t1.ClubPlanTypeId='1'
WHERE 
    t2.StaticDataType = 38
    AND t1.ClubId = @AgentId
 
UNION
 
SELECT
    @AgentId AS ClubId,
    38 AS ClubPlanType,
    COALESCE(defaults.StaticDataLabel, 'Status') AS English,
    COALESCE(defaults.AdditionalValue1, 'Status') AS Japanese,
    5 AS StaticDataValue,
    COALESCE(defaults.StaticDataDescription, 'Status') AS StaticDataDescription,
    t.PlanListId,
    'toggle' AS inputtype,
    'B' AS Description,
	'Status'  name,
	(SELECT  case when planstatus='B' then 'B' else NULL end  FROM tbl_club_plan cp inner join tbl_plans as p on p.PlanId=cp.description 
			   WHERE cp.ClubId = @AgentId and cp.planlistid=t.planlistid and clubplantypeid=1)planstatus,
	'' Id

FROM 
    (SELECT DISTINCT PlanListId FROM tbl_club_plan WHERE ClubId = @AgentId) t
LEFT JOIN
    tbl_static_data AS defaults ON defaults.StaticDataValue = 5 AND defaults.StaticDataType = 38
WHERE NOT EXISTS (
    SELECT 1 
    FROM tbl_club_plan t1
    JOIN tbl_static_data t2 ON t1.ClubPlanTypeId = t2.StaticDataValue
    WHERE t2.StaticDataType = 38
    AND t1.ClubId = @AgentId
    AND t1.PlanListId = t.PlanListId
    AND t2.StaticDataValue = 5
)
ORDER BY 
    PlanListId, StaticDataValue;

END
ELSE IF (@Flag='rcp')   --register club plan
BEGIN

  SELECT @TransactionName = 'rcp'
       BEGIN TRANSACTION @TransactionName;

               If EXISTS(SELECT  
                              'x' 
               		      FROM 
               		           tbl_club_plan WITH (NOLOCK) 
               		      WHERE 
                                  ClubId=@AgentId 
               		      AND ClubPlanTypeId=@ClubPlanTypeId 
               		      AND PlanListId=@PlanListId 
               		      )
               BEGIN  
			       If EXISTS(SELECT  
                              'x' 
               		      FROM 
               		           tbl_club_plan WITH (NOLOCK) 
               		      WHERE 
                                  ClubId=@AgentId 
               		      AND ClubPlanTypeId=@ClubPlanTypeId 
               		      AND PlanListId=@PlanListId and ClubPlanTypeId <>'1')
						  BEGIN
                           UPDATE tbl_club_plan
                           SET 
                                Description=@Description ,
                           	 UpdatedDate=GETDATE(),
                           	 UpdatedIP=@ActionIP,
                           	 UpdatedBy=@ActionUser 
                           WHERE 
                                ClubId=@AgentId 
                           AND 
                           	 ClubPlanTypeId=@ClubPlanTypeId 
                           AND
                                PlanListId=@PlanListId
					  END
                         
               END
               
               ELSE
               BEGIN
                  INSERT INTO [dbo].[tbl_club_plan]
                           (
               
               				ClubId,
                               [ClubPlanType],
                               [ClubPlanTypeId],
                               [Description],
                               [PlanListId],
                               [CreatedDate],
                               [CreatedBy],
                               [CreatedPlatform]
                           )
                           VALUES
                           (
               			@AgentId, 
               			38, 
               			@ClubPlanTypeId, 
               			@Description, 
               			@PlanListId, 
               			GETDATE(),
               			@ActionUser, 
               			@ActionPlatform 
               			);		
               
               END
               
        COMMIT TRANSACTION @TransactionName;

 SELECT 0 Code,
               'Club plan added successfully' Message;
        RETURN;	    

END

ELSE
    BEGIN
        SELECT 1 Code,
               'Invalid function' Message;
        RETURN;
END;	
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO dbo.tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_admin_club_plan_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL', 'sproc_admin_club_plan_management',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
