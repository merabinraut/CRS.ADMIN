USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_get_reservation_details]    Script Date: 11/12/2023 20:23:19 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

ALTER PROC [dbo].[sproc_get_reservation_details]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL
AS
BEGIN
    IF @Flag = 'grtbc' --get reservation time by club
    BEGIN
        WITH CTE
        AS (SELECT LEFT(FORMAT(GETDATE() + number, 'dddd'), 3) AS EnglishDay,
                   LEFT(FORMAT(GETDATE() + number, 'MMMM'), 3) + ' ' + CAST(DAY(GETDATE() + number) AS VARCHAR)
                   + CASE
                         WHEN DAY(GETDATE() + number) IN ( 11, 12, 13 ) THEN
                             'th'
                         WHEN RIGHT(CAST(DAY(GETDATE() + number) AS VARCHAR), 1) = '1' THEN
                             'st'
                         WHEN RIGHT(CAST(DAY(GETDATE() + number) AS VARCHAR), 1) = '2' THEN
                             'nd'
                         WHEN RIGHT(CAST(DAY(GETDATE() + number) AS VARCHAR), 1) = '3' THEN
                             'rd'
                         ELSE
                             'th'
                     END AS Date,
                   FORMAT(CONVERT(DATE, FORMAT(GETDATE() + number, 'yyyy-MM-dd')), N'MM月dd日') AS JapaneseDate,
                   (GETDATE() + number) AS DateValue
            --FORMAT((GETDATE() + number), 'MM-dd') AS DateValue
            FROM master.dbo.spt_values
            WHERE type = 'P'
                  AND number
                  BETWEEN 0 AND 15
                  AND DATEPART(dw, GETDATE() + number) <> 1
            ORDER BY number OFFSET 0 ROWS FETCH NEXT 14 ROWS ONLY)
        SELECT a.EnglishDay,
               a.Date,
               a.JapaneseDate,
               FORMAT(a.DateValue, 'MM-dd') AS DateValue,
               CASE
                   WHEN b.ScheduleId IS NOT NULL THEN
                       c.StaticDataLabel
                   ELSE
                       'Unreservable'
               END AS Schedule,
               CASE
                   WHEN a.EnglishDay = 'Sat' THEN
                       N'土'
                   WHEN a.EnglishDay = 'Sun' THEN
                       N'日'
                   WHEN a.EnglishDay = 'Mon' THEN
                       N'月'
                   WHEN a.EnglishDay = 'Tue' THEN
                       N'火'
                   WHEN a.EnglishDay = 'Wed' THEN
                       N'水'
                   WHEN a.EnglishDay = 'Thu' THEN
                       N'木'
                   WHEN a.EnglishDay = 'Fri' THEN
                       N'金'
                   ELSE
                       '-'
               END AS JapaneseDay
        FROM CTE a WITH (NOLOCK)
            LEFT JOIN dbo.tbl_club_schedule b WITH (NOLOCK)
                ON b.DateValue = FORMAT(a.DateValue, 'yyyy-MM-dd')
                   AND b.ClubId = @ClubId
                   AND ISNULL(b.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = '27'
                   AND c.StaticDataValue = b.ClubSchedule
                   AND ISNULL(c.Status, '') = 'A';
        RETURN;
    END;
END;
GO


