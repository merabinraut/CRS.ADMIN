USE [CRS_V2]
GO
/****** Object:  StoredProcedure [dbo].[sproc_club_plan_list]    Script Date: 5/31/2024 3:42:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sproc_club_plan_list]	
	 @AgentId VARCHAR(20) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @columns NVARCHAR(MAX), @sql NVARCHAR(MAX);

Create table #tempColumns
(
Id varchar(10),
PlanId varchar(10),
LastOrderTime varchar(10),
LastEntryTime varchar(10),
NoofPeople varchar(10)
--status varchar(20) DEFAULT 'Unknown' -- Set a default value for the status column
)

-- Get distinct PlanListId to use as columns
SELECT @columns = COALESCE(@columns + ', ', '') + QUOTENAME(ClubPlanTypeId  )
FROM (
    SELECT DISTINCT ClubPlanTypeId
    FROM tbl_club_plan
    WHERE ClubId = @AgentId and ClubPlanTypeId<>'5'
) AS s;
--select @columns
-- Construct dynamic SQL query
SET @sql = '
SELECT *
FROM (

    SELECT 
	
        ClubPlanTypeId,
        Description,
        planlistid       
        
    FROM 
        tbl_club_plan
		inner join tbl_static_data sd on sd.staticdatatype=38 and sd.staticdatavalue= ClubPlanTypeId
    WHERE 
        ClubId = '+@AgentId+' 
) AS src
PIVOT (
    MAX(Description) FOR ClubPlanTypeId IN (' + @columns + ')
) AS pivoted';

-- Execute the dynamic SQL

Insert into #tempColumns
 
EXEC sp_executesql @sql;
--select * from #tempColumns;
--

Create table #tempCreateddate(
Id varchar(10),
CreatedDate datetime

)
Create table #tempUpdatedate(
Id varchar(10),
updatedDate datetime

)
--------------------------------------Start region to get status value---------------------------------------

Create table #tempStatus(
Id varchar(10),
Status varchar(20)

)

Insert into #tempStatus
SELECT 
    t.PlanListId,
    ISNULL(t.Description, 'B') AS status
FROM 
    (
        SELECT 
            COALESCE(t1.ClubPlanType, 38) AS ClubPlanType,
            t2.StaticDataValue,
            t1.PlanListId,
            t1.Description
        FROM 
            tbl_club_plan t1
        JOIN
            tbl_static_data t2 ON t1.ClubPlanTypeId = t2.StaticDataValue
        WHERE 
            t2.StaticDataType = 38
            AND t1.ClubId = @AgentId
 
        UNION
 
        SELECT
            38 AS ClubPlanType,
            5 AS StaticDataValue,
            t.PlanListId,
            '' AS Description
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
    ) AS t 
WHERE 
    t.StaticDataValue = 5;


----------------End region to get status value-----------------------






;WITH CTE  AS(
    SELECT
        PlanListId,
        CreatedDate,
        ROW_NUMBER() OVER (PARTITION BY PlanListId ORDER BY CreatedDate DESC) AS RowNum
    FROM
        tbl_club_plan
    WHERE
        ClubId = @AgentId
)
Insert into #tempCreateddate
SELECT
PlanListId Id,
    CreatedDate
FROM
    CTE
WHERE
    RowNum = 1



;WITH CTEupdate AS (
    SELECT   
	PlanListId,
        updateddate,
        ROW_NUMBER() OVER (PARTITION BY PlanListId ORDER BY updateddate DESC) AS RowNum
    FROM
        tbl_club_plan
    WHERE
        ClubId = @AgentId
)
Insert into #tempUpdatedate
SELECT  
PlanListId Id,
    updateddate
FROM
    CTEupdate
WHERE
    RowNum = 1


--select * from CTEupdate
SELECT * from (
select p.planName,case when ts.status = 'A' then 'A' else 'B' end as status,temp.*,cdate.createddate CreatedDate,udate.updateddate UpdatedDate,@AgentId ClubId from #tempColumns as temp 
 inner join  #tempCreateddate as cdate on temp.id=cdate.Id
 inner join #tempUpdatedate as udate on temp.id=udate.Id
 LEFT join tbl_plans  as p on p.planid=temp.planid  and planstatus='A'
 Left Join #tempStatus as ts on ts.Id=temp.Id 
 ) as t   where isnull(t.planName,'') <> ''
 order by t.Id asc
  drop table #tempColumns
  drop table #tempCreateddate
  drop table #tempUpdatedate



END
