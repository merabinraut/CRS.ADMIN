USE [CRS];
GO

CREATE PROC dbo.sproc_admin_recommendation_location_page @Flag VARCHAR(10)
AS
BEGIN
    IF @Flag = 'location'
    BEGIN
        SELECT a.LocationId,
               a.LocationName
        FROM dbo.tbl_location a WITH (NOLOCK)
        WHERE ISNULL(a.Status, '') = 'A'
        ORDER BY a.LocationName,
                 a.LocationId ASC;
        RETURN;
    END;

    ELSE IF @Flag = 'Page'
    BEGIN
        SELECT b.StaticDataValue AS PageId,
               b.StaticDataLabel AS PageLabel
        FROM dbo.tbl_static_data_type a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data b WITH (NOLOCK)
                ON a.StaticDataType = 6
                   AND b.StaticDataType = a.StaticDataType
                   AND ISNULL(a.Status, '') = 'A'
                   AND ISNULL(b.Status, '') = 'A'
        ORDER BY b.StaticDataLabel ASC;
    END;
END;
GO



