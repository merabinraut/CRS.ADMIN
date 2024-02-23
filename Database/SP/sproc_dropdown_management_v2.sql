USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_dropdown_management_v2]    Script Date: 2/23/2024 3:21:46 PM ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO



ALTER PROC [dbo].[sproc_dropdown_management_v2]
    @Flag VARCHAR(10),
    @SearchField1 VARCHAR(10) = NULL,
    @SearchField2 VARCHAR(10) = NULL
AS
BEGIN
    IF @Flag = '1' --get customer payment type
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelEnglish,
               a.AdditionalValue1 AS StaticLabelJapanese
        FROM dbo.tbl_static_data a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK)
                ON b.StaticDataType = a.StaticDataType
                   AND ISNULL(a.Status, '') = 'A'
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.StaticDataType = 10
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
    END;



    ELSE IF ISNULL(@Flag, '') = '3' --Club prefecture 
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.AdditionalValue1 AS StaticLabelEnglish
        FROM dbo.tbl_static_data a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK)
                ON b.StaticDataType = a.StaticDataType
        WHERE b.StaticDataType = 15
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;

    END;
    ELSE IF ISNULL(@Flag, '') = '4' --Club Holiday 
    BEGIN
        SELECT Id AS StaticValue,
               EnglishDay AS StaticLabelEnglish,
               '' AS StaticLabel,
               JapaneseDay AS StaticLabelJapanese
        FROM dbo.tbl_holiday;

        RETURN;
    END;

    ELSE IF @Flag = 5 --get location list with total no of club count
    BEGIN
        SELECT a.LocationId AS StaticValue,
               CONCAT(
                         CONCAT(a.LocationName, '/', a.LocationName),
                         ' (' + CONVERT(VARCHAR, COALESCE(COUNT(a2.AgentId), 0)) + ')'
                     ) AS StaticLabel,
               CONCAT(
                         CONCAT(a.LocationName, '/', a.LocationName),
                         ' (' + CONVERT(VARCHAR, COALESCE(COUNT(a2.AgentId), 0)) + ')'
                     ) AS StaticLabelJapanese,
               CONCAT(
                         CONCAT(a.LocationName, '/', a.LocationName),
                         ' (' + CONVERT(VARCHAR, COALESCE(COUNT(a2.AgentId), 0)) + ')'
                     ) AS StaticLabelEnglish
        FROM dbo.tbl_location a
            LEFT JOIN dbo.tbl_club_details a2
                ON a2.LocationId = a.LocationId
                   AND COALESCE(a2.Status, '') = 'A'
        WHERE COALESCE(a.Status, '') = 'A'
        GROUP BY a.LocationId,
                 a.LocationName
        ORDER BY a.LocationName,
                 a.LocationId ASC;
        RETURN;
    END;

    ELSE IF @Flag = 6 --get Club Category
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.AdditionalValue1 AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 17
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;

    ELSE IF @Flag = 7 --get plan price list
    BEGIN
        SELECT DISTINCT
               CAST(COALESCE(a.Price, 0) AS INT) AS StaticValue,
               CONCAT(CAST(COALESCE(a.Price, 0) AS INT), N' 円') AS StaticLabel,
               CONCAT(CAST(COALESCE(a.Price, 0) AS INT), N' 円') AS StaticLabelJapanese,
               CONCAT(CAST(COALESCE(a.Price, 0) AS INT), N' 円') AS StaticLabelEnglish
        FROM dbo.tbl_plans a
        WHERE a.PlanCategory IN ( 1, 2 )
              AND COALESCE(a.PlanStatus, '') = 'A'
        ORDER BY StaticValue ASC;
        RETURN;
    END;

    ELSE IF @Flag = 8 --get club availability
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.AdditionalValue1 AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 36
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;

    ELSE IF @Flag = 9 --Blood Group
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.AdditionalValue1 AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 18
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;

    ELSE IF @Flag = 10 --Height list
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.StaticDataLabel AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 20
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;

    ELSE IF @Flag = 11 --Age Range
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.StaticDataLabel AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 29
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;

    ELSE IF @Flag = 12 --Zodiac signs
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.AdditionalValue1 AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 13
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;

    ELSE IF @Flag = 13 --Occupation list
    BEGIN
        SELECT a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese,
               a.AdditionalValue1 AS StaticLabelEnglish
        FROM dbo.tbl_static_data a
            INNER JOIN dbo.tbl_static_data_type b
                ON b.StaticDataType = a.StaticDataType
        WHERE COALESCE(b.Status, '') = 'A'
              AND COALESCE(a.Status, '') = 'A'
              AND b.StaticDataType = 12
        ORDER BY CAST(a.StaticDataValue AS INT) ASC;
        RETURN;
    END;
END;
GO


