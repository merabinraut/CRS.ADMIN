USE CRS;
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
        ORDER BY a.StaticDataValue ASC;
    END;

	

	ELSE IF ISNULL(@Flag, '') = '3' --Club prefecture 
    BEGIN
       SELECT   a.StaticDataValue AS StaticValue,
               a.StaticDataLabel AS StaticLabel,
               a.StaticDataLabel AS StaticLabelJapanese ,
               a.AdditionalValue1 AS StaticLabelEnglish  FROM dbo.tbl_static_data a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data_type b WITH (NOLOCK)  ON b.StaticDataType = a.StaticDataType
	   WHERE b.StaticDataType=15   AND ISNULL(a.Status, '') = 'A'
        ORDER BY a.StaticDataValue ASC;
        
    END;
END;
GO