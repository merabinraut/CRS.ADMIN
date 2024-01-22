USE CRS;
GO

ALTER PROC dbo.sproc_shuffling_management
    @Flag VARCHAR(10),
    @ShufflingTimeCSValue VARCHAR(MAX) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL
AS
BEGIN
    IF @Flag = 'gstl' --get shuffling time list
    BEGIN
        SELECT b.AdditionalValue2 AS LabelName,
               b.StaticDataValue AS DisplayId,
               b.StaticDataLabel AS DisplayName,
               ISNULL(b.AdditionalValue1, 0) AS DisplayTime
        FROM dbo.tbl_static_data_type a WITH (NOLOCK)
            INNER JOIN dbo.tbl_static_data b WITH (NOLOCK)
                ON a.StaticDataType = 5
                   AND b.StaticDataType = a.StaticDataType
                   AND ISNULL(a.Status, '') = 'A'
                   AND ISNULL(b.Status, '') = 'A'
        ORDER BY b.AdditionalValue2 ASC;

        RETURN;
    END;

    ELSE IF @Flag = 'msf' --manage shuffling time
    BEGIN
        CREATE TABLE #temp_mst_1
        (
            Value VARCHAR(20) NULL
        );

        INSERT INTO #temp_mst_1
        (
            Value
        )
        SELECT *
        FROM STRING_SPLIT(ISNULL(@ShufflingTimeCSValue, ''), ',');

        CREATE TABLE #temp_mst_2
        (
            StaticDataValue VARCHAR(10),
            AdditionalValue1 VARCHAR(10)
        );

        INSERT INTO #temp_mst_2
        (
            StaticDataValue,
            AdditionalValue1
        )
        SELECT SUBSTRING(Value, 1, CHARINDEX(':', Value) - 1),
               SUBSTRING(Value, CHARINDEX(':', Value) + 1, LEN(Value) - CHARINDEX(':', Value))
        FROM #temp_mst_1;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_static_data_type a WITH (NOLOCK)
                INNER JOIN dbo.tbl_static_data b WITH (NOLOCK)
                    ON a.StaticDataType = 5
                       AND b.StaticDataType = a.StaticDataType
                       AND ISNULL(a.Status, '') = 'A'
                       AND ISNULL(b.Status, '') = 'A'
            WHERE b.StaticDataValue IN
                  (
                      SELECT t1.StaticDataValue FROM #temp_mst_2 t1 WITH (NOLOCK)
                  )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_static_data
        SET AdditionalValue1 = ISNULL(a.AdditionalValue1, b.AdditionalValue1),
            ActionUser = @ActionUser,
            ActionDate = GETDATE()
        FROM dbo.tbl_static_data b WITH (NOLOCK)
            JOIN #temp_mst_2 a WITH (NOLOCK)
                ON b.StaticDataValue = a.StaticDataValue
        WHERE b.StaticDataType = 5
              AND b.Status = 'A';

        DROP TABLE #temp_mst_1;
        DROP TABLE #temp_mst_2;

        SELECT 0 Code,
               'Shuffling time updated successfully';
        RETURN;
    END;
END;
GO


