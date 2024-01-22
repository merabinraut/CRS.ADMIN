USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_club_schedule_management]    Script Date: 10/12/2023 10:57:36 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO



ALTER PROC [dbo].[sproc_club_schedule_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @DateValue VARCHAR(10) = NULL,
    @ClubSchedule VARCHAR(10) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @ScheduleId VARCHAR(10) = NULL
AS
BEGIN
    IF @Flag = 'ccs' --create club schedule
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club' Message;
            RETURN;
        END;

        INSERT INTO dbo.tbl_club_schedule
        (
            ClubId,
            DateValue,
            ClubSchedule,
            Status,
            CreatedBy,
            CreatedDate,
            CreatedIP,
            CreatedPlatform
        )
        VALUES
        (@ClubId, @DateValue, @ClubSchedule, 'A', @ActionUser, GETDATE(), @ActionIP, @ActionPlatform);

        SET @ScheduleId = SCOPE_IDENTITY();

        UPDATE dbo.tbl_club_schedule
        SET ScheduleId = @ScheduleId
        WHERE Sno = @ScheduleId;

        SELECT 0 Code,
               'Club schedule assigned successfully' Message;
        RETURN;
    END;
    ELSE IF @Flag = 'mcs' --manage club schedule
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_schedule a WITH (NOLOCK)
            WHERE a.ScheduleId = @ScheduleId
                  AND a.ClubId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid request' Message;
            RETURN;
        END;

        UPDATE dbo.tbl_club_schedule
        SET DateValue = ISNULL(@DateValue, DateValue),
            ClubSchedule = ISNULL(@ClubSchedule, ClubSchedule),
            UpdatedBy = @ActionUser,
            UpdatedDate = GETDATE(),
            UpdatedIP = @ActionIP,
            UpdatedPlatform = @ActionPlatform
        WHERE ScheduleId = @ScheduleId
              AND ClubId = @ClubId
              AND ISNULL(Status, '') = 'A';

        SELECT 0 Code,
               'Club schedule updated successfully' Message;
        RETURN;
    END;
    ELSE IF @Flag = 'gcsl' --get club schedule list
    BEGIN
        SELECT a.ScheduleId,
               a.ClubId,
               a.DateValue,
               ISNULL(b.StaticDataLabel, '-') AS ClubSchedule
        FROM dbo.tbl_club_schedule a WITH (NOLOCK)
            LEFT JOIN tbl_static_data b WITH (NOLOCK)
                ON b.StaticDataType = 27
                   AND b.StaticDataValue = a.ClubSchedule
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.ClubId = @ClubId
              AND a.Status = 'A';
        RETURN;
    END;
    ELSE IF @Flag = 'gcws' --get club week schedule
    BEGIN
        WITH CTE
        AS (SELECT LEFT(FORMAT(GETDATE() + number, 'dddd'), 3) AS EnglishDay,
                   FORMAT(GETDATE() + number, 'yyyy-MM-dd') AS Date
            FROM master.dbo.spt_values
            WHERE type = 'P'
                  AND number
                  BETWEEN 0 AND 6
                  AND DATEPART(dw, GETDATE() + number) <> 1
            ORDER BY number OFFSET 0 ROWS FETCH NEXT 7 ROWS ONLY)
        SELECT a.*,
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
                ON b.ClubSchedule = a.Date
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


