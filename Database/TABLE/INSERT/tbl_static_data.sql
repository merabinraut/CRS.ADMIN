INSERT INTO tbl_static_data (
	StaticDataType
	,StaticDataLabel
	,StaticDataValue
	,StaticDataDescription
	,STATUS
	,ActionUser
	,ActionDate
	)
VALUES (
	1
	,'Super Admin'
	,'1'
	,'Admin user role'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)
	,(
	1
	,'Admin'
	,'2'
	,'Admin user role'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)

INSERT INTO tbl_static_data (
	StaticDataType
	,StaticDataLabel
	,StaticDataValue
	,StaticDataDescription
	,STATUS
	,ActionUser
	,ActionDate
	)
VALUES (
	2
	,'Male'
	,'M'
	,'Gender'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)
	,(
	2
	,'Female'
	,'F'
	,'Gender'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)
	,(
	2
	,'Other'
	,'O'
	,'Gender'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)

INSERT INTO tbl_static_data (
	StaticDataType
	,StaticDataLabel
	,StaticDataValue
	,StaticDataDescription
	,STATUS
	,ActionUser
	,ActionDate
	)
VALUES (
	3
	,'Flat'
	,'F'
	,'Flat Amount'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)
	,(
	3
	,'Percentage'
	,'P'
	,'Percentage Amount'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)

---FOR PLAN MANAGEMENT
INSERT INTO [dbo].[tbl_static_data] (
	[StaticDataType]
	,[StaticDataLabel]
	,[StaticDataValue]
	,[StaticDataDescription]
	,[Status]
	,[ActionUser]
	,[ActionDate]
	)
VALUES (
	'7'
	,'Basic'
	,'B'
	,'Basic plan type'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)
	,(
	'7'
	,'Premium'
	,'P'
	,'Premium plan type'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)
	,(
	'8'
	,'90min'
	,'90m'
	,'Time duration'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)
	,(
	'8'
	,'60min'
	,'60m'
	,'Time duration'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)
	,(
	'8'
	,'30min'
	,'30m'
	,'Time duration'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)
	,(
	'9'
	,'Unlimited'
	,'U'
	,'Liquor options'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)
	,(
	'9'
	,'Limited'
	,'L'
	,'Liquor options'
	,'A'
	,ORIGINAL_LOGIN()
	,GETDATE()
	)

	INSERT INTO [dbo].[tbl_static_data] (
	[StaticDataType]
	,[StaticDataLabel]
	,[StaticDataValue]
	,[StaticDataDescription]
	,[Status]
	,[ActionUser]
	,[ActionDate]
	)
VALUES (
	'23'
	,'1'
	,'1'
	,'First page'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)
	, (
	'23'
	,'2'
	,'2'
	,'Second page'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)
	, (
	'23'
	,'3'
	,'3'
	,'Third page'
	,'A'
	,'rabin.raut'
	,GETDATE()
	)

		INSERT INTO dbo.tbl_static_data
	(
	    StaticDataType,
	    StaticDataLabel,
	    StaticDataValue,
	    StaticDataDescription,
	    AdditionalValue1,
	    Status,
	    ActionUser,
	    ActionDate
	)
	VALUES
	(   24, -- StaticDataType - bigint
	    'I met my pusher', -- StaticDataLabel - varchar(50)
	    '1', -- StaticDataValue - varchar(200)
	    'Remark', -- StaticDataDescription - varchar(512)
	    N'推しに会えた', -- AdditionalValue1 - varchar(2000)
	    'A', -- Status - char(1)
	    SESSION_USER, -- ActionUser - varchar(200)
	    GETDATE()  -- ActionDate - datetime
	    ),
		(   24, -- StaticDataType - bigint
	    'Talk is good', -- StaticDataLabel - varchar(50)
	    '2', -- StaticDataValue - varchar(200)
	    'Remark', -- StaticDataDescription - varchar(512)
	    N'トークが上手い', -- AdditionalValue1 - varchar(2000)
	    'A', -- Status - char(1)
	    SESSION_USER, -- ActionUser - varchar(200)
	    GETDATE()  -- ActionDate - datetime
	    ),
		(   24, -- StaticDataType - bigint
	    'There was a handsome man', -- StaticDataLabel - varchar(50)
	    '3', -- StaticDataValue - varchar(200)
	    'Remark', -- StaticDataDescription - varchar(512)
	    N'イケメンがいた', -- AdditionalValue1 - varchar(2000)
	    'A', -- Status - char(1)
	    SESSION_USER, -- ActionUser - varchar(200)
	    GETDATE()  -- ActionDate - datetime
	    )

		INSERT INTO dbo.tbl_static_data
(
    StaticDataType,
    StaticDataLabel,
    StaticDataValue,
    StaticDataDescription,
    AdditionalValue1,
    Status,
    ActionUser,
    ActionDate
)
VALUES
(   25, -- StaticDataType - bigint
    'Q. Were there any good-looking guys?', -- StaticDataLabel - varchar(50)
    '1', -- StaticDataValue - varchar(200)
    'Review questions', -- StaticDataDescription - varchar(512)
    N'Q.イケメンはいましたか？', -- AdditionalValue1 - varchar(2000)
    'A', -- Status - char(1)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE()  -- ActionDate - datetime
    ),
	(   25, -- StaticDataType - bigint
    'Q. Was the store well-cleaned?', -- StaticDataLabel - varchar(50)
    '2', -- StaticDataValue - varchar(200)
    'Review questions', -- StaticDataDescription - varchar(512)
    N'Q.店舗の清掃は行き届いていましたか？', -- AdditionalValue1 - varchar(2000)
    'A', -- Status - char(1)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE()  -- ActionDate - datetime
    ),
	(   25, -- StaticDataType - bigint
    'Q. Did you feel like going again?', -- StaticDataLabel - varchar(50)
    '3', -- StaticDataValue - varchar(200)
    'Review questions', -- StaticDataDescription - varchar(512)
    N'Q.また行きたいと感じましたか？', -- AdditionalValue1 - varchar(2000)
    'A', -- Status - char(1)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE()  -- ActionDate - datetime
    ),
	(   25, -- StaticDataType - bigint
    'Q.Did you extend the course on the day?', -- StaticDataLabel - varchar(50)
    '4', -- StaticDataValue - varchar(200)
    'Review questions', -- StaticDataDescription - varchar(512)
    N'Q.当日、コースを延長しましたか？', -- AdditionalValue1 - varchar(2000)
    'A', -- Status - char(1)
    SESSION_USER, -- ActionUser - varchar(200)
    GETDATE()  -- ActionDate - datetime
    )

		INSERT INTO dbo.tbl_static_data
	(
	    StaticDataType,
	    StaticDataLabel,
	    StaticDataValue,
	    StaticDataDescription,
	    AdditionalValue1,
	    Status,
	    ActionUser,
	    ActionDate
	)
	VALUES
	(   '26', -- StaticDataType - bigint
	    'Yes', -- StaticDataLabel - varchar(50)
	    '1', -- StaticDataValue - varchar(200)
	    'Yes', -- StaticDataDescription - varchar(512)
	    N'はい', -- AdditionalValue1 - varchar(2000)
	    'A', -- Status - char(1)
	    SESSION_USER, -- ActionUser - varchar(200)
	    GETDATE()  -- ActionDate - datetime
	    ),
			(   '26', -- StaticDataType - bigint
	    'Neither', -- StaticDataLabel - varchar(50)
	    '2', -- StaticDataValue - varchar(200)
	    'Neither', -- StaticDataDescription - varchar(512)
	    N'どちらでもない', -- AdditionalValue1 - varchar(2000)
	    'A', -- Status - char(1)
	    SESSION_USER, -- ActionUser - varchar(200)
	    GETDATE()  -- ActionDate - datetime
	    ),
			(   '26', -- StaticDataType - bigint
	    'Yes', -- StaticDataLabel - varchar(50)
	    '3', -- StaticDataValue - varchar(200)
	    'No', -- StaticDataDescription - varchar(512)
	    N'いいえ', -- AdditionalValue1 - varchar(2000)
	    'A', -- Status - char(1)
	    SESSION_USER, -- ActionUser - varchar(200)
	    GETDATE()  -- ActionDate - datetime
	    )

		INSERT INTO dbo.tbl_static_data
(
    StaticDataType,
    StaticDataLabel,
    StaticDataValue,
    StaticDataDescription,
    AdditionalValue1,
    AdditionalValue2,
    AdditionalValue3,
    AdditionalValue4,
    Status,
    ActionUser,
    ActionDate
)
VALUES
(   24, -- StaticDataType - bigint
    'Rude', -- StaticDataLabel - varchar(50)
    '4', -- StaticDataValue - varchar(200)
    'Remark', -- StaticDataDescription - varchar(512)
    N'失礼', -- AdditionalValue1 - nvarchar(2000)
    'Bad', -- AdditionalValue2 - varchar(2000)
    N'悪い', -- AdditionalValue3 - varchar(2000)
    NULL, -- AdditionalValue4 - varchar(2000)
    'A', -- Status - char(1)
    'rabin.raut', -- ActionUser - varchar(200)
    GETDATE()  -- ActionDate - datetime
    )