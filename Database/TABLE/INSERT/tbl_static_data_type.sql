INSERT INTO tbl_static_data_type (
	StaticDataType
	,StaticDataLabel
	,StaticDataName
	,StaticDataDescription
	,ActionUser
	,ActionDate
	)
VALUES (
	1
	,'Role Types'
	,'Role Types'
	,'Types of role'
	,'rabin.raut'
	,GETDATE()
	)

INSERT INTO tbl_static_data_type (
	StaticDataType
	,StaticDataName
	,StaticDataDescription
	,ActionUser
	,ActionDate
	)
VALUES (
	3
	,'Commission Percent Type'
	,'Commission Percent Types'
	,'rabin.raut'
	,GETDATE()
	)

--FOR PLAN MANAGEMENT
INSERT INTO [dbo].[tbl_static_data_type] (
	[StaticDataType]
	,[StaticDataName]
	,[StaticDataDescription]
	,[ActionUser]
	,[ActionDate]
	,[Status]
	)
VALUES (
	'7'
	,'Plan Type'
	,'Plan types for plan management'
	,ORIGINAL_LOGIN()
	,GETDATE()
	,'A'
	)
	,(
	'8'
	,'Time'
	,'Time options for plan management'
	,ORIGINAL_LOGIN()
	,GETDATE()
	,'A'
	)
	,(
	'9'
	,'Liquor'
	,'Liquor options for plan management'
	,ORIGINAL_LOGIN()
	,GETDATE()
	,'A'
	)

INSERT INTO tbl_static_data_type (
	StaticDataType
	,StaticDataName
	,StaticDataDescription
	,ActionUser
	,ActionDate
	,status
	)
VALUES (
	23
	,'Recommendation Group Order'
	,'Recommendation group display order'
	,'rabin.raut'
	,GETDATE()
	,'A'
	)

	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   24, -- StaticDataType - bigint
    'Review Remark', -- StaticDataName - varchar(200)
    'Review & rating remarks', -- StaticDataDescription - varchar(256)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )

	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   25, -- StaticDataType - bigint
    'Dichotomous Question', -- StaticDataName - varchar(200)
    'Dichotomous question for review', -- StaticDataDescription - varchar(256)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )

	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   '26', -- StaticDataType - bigint
    'Dichotomous Answer', -- StaticDataName - varchar(200)
    'Dichotomous answer list', -- StaticDataDescription - varchar(256)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )
	
	
INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   31, -- StaticDataType - bigint
    'Host Skills', -- StaticDataName - varchar(200)
    'Club host skills', -- StaticDataDescription - varchar(256)
    SYSTEM_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )
	
	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   32, -- StaticDataType - bigint
    'Host Personality', -- StaticDataName - varchar(200)
    'Club host personality', -- StaticDataDescription - varchar(256)
    SYSTEM_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )
	
	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   33, -- StaticDataType - bigint
    'Host Lifestyle', -- StaticDataName - varchar(200)
    'Club host lifestyle', -- StaticDataDescription - varchar(256)
    SYSTEM_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )
	
	
	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   34, -- StaticDataType - bigint
    'Skills', -- StaticDataName - varchar(200)
    'Skills list', -- StaticDataDescription - varchar(256)
    SYSTEM_USER, -- ActionUser - varchar(200)
    GETDATE(), -- ActionDate - datetime
    'A'  -- Status - char(1)
    )
	
	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(   '35', 
    'Plan Category', 
    'Plan category', 
    SYSTEM_USER, 
    GETDATE(), 
    'A'  
    )
	
	INSERT INTO dbo.tbl_static_data_type
(
    StaticDataType,
    StaticDataName,
    StaticDataDescription,
    ActionUser,
    ActionDate,
    Status
)
VALUES
(36, 'Club Availability', 'Club availability', SYSTEM_USER, GETDATE(), 'A');

INSERT INTO tbl_static_data_type ( [StaticDataType], [StaticDataName], [StaticDataDescription], [ActionUser], [ActionDate], [Status])
VALUES
( 37, 'Event Type', 'CEvent Type', 'kiran.acharya', N'2024-02-12T15:39:25.57', 'A' )

