USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_cp_reservation_management]    Script Date: 2/16/2024 5:53:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROC [dbo].[sproc_cp_reservation_management]
    @Flag VARCHAR(10),
    @ClubId VARCHAR(10) = NULL,
    @CustomerId VARCHAR(10) = NULL,
    @SelectedDate VARCHAR(10) = NULL,
    @SelectedTime VARCHAR(5) = NULL,
    @NoOfPeople INT = 0,
    @HostListId VARCHAR(MAX) = NULL,
    @PlanId VARCHAR(10) = NULL
AS
DECLARE @SQLString NVARCHAR(MAX),
        @ReservationType VARCHAR(10),
        @CustomerCostAmount DECIMAL(18, 2),
        @CustomerUserName NVARCHAR(200);
BEGIN
    IF @Flag = 'ccrs' --check club reservation status
    BEGIN
        SELECT 0 Code,
               'Success' Message,
               10 AS TotalNoOfPeople,
               3 AS MaxNoOfPeopleAllowed;
        RETURN;
    END;

    ELSE IF @Flag = 'vngcd' --verify and get club details
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

        SELECT 0 Code,
               'Success' Message,
               a.AgentId AS ClubId,
               a.ClubName1 AS ClubNameEnglish,
               a.ClubName2 AS ClubNameJapanese,
               b.LocationName AS ClubLocationName
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_location b WITH (NOLOCK)
                ON b.LocationId = a.LocationId
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.AgentId = @ClubId
              AND ISNULL(a.Status, '') = 'A';
        RETURN;
    END;

    ELSE IF @Flag = 'gcds' --get club date schedule
    BEGIN
        WITH CTE
        AS (SELECT (GETDATE() + number) AS DateValue
            FROM master.dbo.spt_values
            WHERE type = 'P'
                  AND number
                  BETWEEN 0 AND 15
            ORDER BY number OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY)
        SELECT FORMAT(a.DateValue, 'yyyy-MM-dd') AS Date,
               CASE
                   WHEN b.ScheduleId IS NOT NULL
                        AND c.StaticDataLabel = 'Unreservable' THEN
                       'Unreservable'
                   ELSE
                       'Reservable'
               END AS Schedule
        FROM CTE a WITH (NOLOCK)
            LEFT JOIN dbo.tbl_club_schedule b WITH (NOLOCK)
                ON FORMAT(CAST(b.DateValue AS DATE), 'yyyy-MM-dd') = FORMAT(a.DateValue, 'yyyy-MM-dd')
                   AND b.ClubId = @ClubId
                   AND ISNULL(b.Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                ON c.StaticDataType = '27'
                   AND c.StaticDataValue = b.ClubSchedule
                   AND ISNULL(c.Status, '') = 'A';
        RETURN;
    END;

    ELSE IF @Flag = 'gcrtd' --get club reservation time detail
    BEGIN
        DECLARE @ClubFromTime TIME,
                @ClubToTime TIME;

        SELECT @ClubFromTime = a.ClubOpeningTime,
               @ClubToTime = a.ClubClosingTime
        FROM dbo.tbl_club_details a WITH (NOLOCK)
        WHERE a.AgentId = @ClubId
              AND ISNULL(a.Status, '') = 'A';

        CREATE TABLE #temp_gcrtd
        (
            Time VARCHAR(5),
            TimeStatus VARCHAR(6)
        );

        INSERT INTO #temp_gcrtd
        SELECT LEFT(@ClubFromTime, 5),
               'Active';

        WHILE @ClubToTime != @ClubFromTime
        BEGIN
            SET @ClubFromTime = DATEADD(MINUTE, 15, @ClubFromTime);

            INSERT INTO #temp_gcrtd
            SELECT LEFT(@ClubFromTime, 5),
                   'Active';
        END;

        SELECT *
        FROM #temp_gcrtd WITH (NOLOCK);

        DROP TABLE #temp_gcrtd;

        RETURN;
    END;

    ELSE IF @Flag = 'cirpiv' --check if the customer can proceed with the reservation process
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1
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
            SELECT 1
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 3
                       AND ISNULL(b.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid customer' Message;
            RETURN;
        END;

        SELECT 0 Code,
               'Success' Message;
        RETURN;
    END;

    ELSE IF @Flag = 'gpl' --get customer valid plan for the given club
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1
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
            SELECT 1
            FROM dbo.tbl_customer a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 3
                       AND ISNULL(b.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid customer' Message;
            RETURN;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                INNER JOIN dbo.tbl_reservation_plan_detail b WITH (NOLOCK)
                    ON b.ReservationId = a.ReservationId
                       AND b.PlanDetailId = a.PlanDetailId
                INNER JOIN dbo.tbl_plans c WITH (NOLOCK)
                    ON c.PlanId = b.PlanId
            WHERE a.CustomerId = @CustomerId
                  AND a.ClubId = @ClubId
                  AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                  BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(GETDATE(), 'yyyy-MM-dd')
            GROUP BY a.CustomerId,
                     a.ClubId
            HAVING COUNT(DISTINCT CASE
                                      WHEN c.PlanCategory IN ( 1, 2 ) THEN
                                          c.PlanCategory
                                  END
                        ) = 2
        )
        BEGIN
            SELECT 9 AS Code,
                   N'現在、プランはありません' AS Message; --'Invalid request. Please select other clubs' 
            RETURN;
        END;
        ELSE
        BEGIN
            IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.CustomerId = @CustomerId
                      AND a.ClubId = @ClubId
                      AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                      BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(GETDATE(), 'yyyy-MM-dd')
            )
            BEGIN
                SELECT 0 Code,
                       'Success' Message,
                       a.PlanId,
                       a.PlanName,
                       b.StaticDataLabel AS PlanTime,
                       CAST(ISNULL(a.Price, 0) AS INT) AS PlanPrice,
                       a.Nomination AS PlanNomination,
                       ISNULL(a.Remarks, '') AS PlanRemarks,
                       c.StaticDataLabel AS PlanLiquor
                FROM dbo.tbl_plans a WITH (NOLOCK)
                    LEFT JOIN dbo.tbl_static_data b WITH (NOLOCK)
                        ON b.StaticDataType = 8
                           AND b.StaticDataValue = a.PlanTime
                           AND ISNULL(b.Status, '') = 'A'
                    LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                        ON c.StaticDataType = 9
                           AND c.StaticDataValue = a.Liquor
                           AND ISNULL(c.Status, '') = 'A'
                WHERE a.PlanCategory IN ( 1, 2 )
                      AND ISNULL(a.PlanStatus, '') = 'A'
                ORDER BY a.Price ASC;

                RETURN;
            END;
            ELSE IF
            (
                SELECT COUNT(a.ReservationId)
                FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                WHERE a.CustomerId = @CustomerId
                      AND a.ClubId = @ClubId
                      AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                      BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(GETDATE(), 'yyyy-MM-dd')
            ) = 1
            BEGIN
                IF EXISTS
                (
                    SELECT 1
                    FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                        INNER JOIN dbo.tbl_reservation_plan_detail b WITH (NOLOCK)
                            ON b.ReservationId = a.ReservationId
                               AND b.PlanDetailId = a.PlanDetailId
                        INNER JOIN dbo.tbl_plans c WITH (NOLOCK)
                            ON c.PlanId = b.PlanId
                    WHERE c.PlanCategory = 2 -- Offer plan		
                          AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                          BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(
                                                                                                    GETDATE(),
                                                                                                    'yyyy-MM-dd'
                                                                                                )
                )
                BEGIN
                    SELECT 0 Code,
                           'Success' Message,
                           a.PlanId,
                           a.PlanName,
                           b.StaticDataLabel AS PlanTime,
                           CAST(ISNULL(a.Price, 0) AS INT) AS PlanPrice,
                           a.Nomination AS PlanNomination,
                           ISNULL(a.Remarks, '') AS PlanRemarks,
                           c.StaticDataLabel AS PlanLiquor
                    FROM dbo.tbl_plans a WITH (NOLOCK)
                        LEFT JOIN dbo.tbl_static_data b WITH (NOLOCK)
                            ON b.StaticDataType = 8
                               AND b.StaticDataValue = a.PlanTime
                               AND ISNULL(b.Status, '') = 'A'
                        LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                            ON c.StaticDataType = 9
                               AND c.StaticDataValue = a.Liquor
                               AND ISNULL(c.Status, '') = 'A'
                    WHERE a.PlanCategory = 1
                          AND ISNULL(a.PlanStatus, '') = 'A'
                    ORDER BY a.Price ASC;

                    RETURN;
                END;
                ELSE IF EXISTS
                (
                    SELECT 1
                    FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
                        INNER JOIN dbo.tbl_reservation_plan_detail b WITH (NOLOCK)
                            ON b.ReservationId = a.ReservationId
                               AND b.PlanDetailId = a.PlanDetailId
                        INNER JOIN dbo.tbl_plans c WITH (NOLOCK)
                            ON c.PlanId = b.PlanId
                    WHERE c.PlanCategory = 1 -- normal plan				
                          AND FORMAT(a.ActionDate, 'yyyy-MM-dd')
                          BETWEEN FORMAT(DATEADD(MONTH, -3, GETDATE()), 'yyyy-MM-dd') AND FORMAT(
                                                                                                    GETDATE(),
                                                                                                    'yyyy-MM-dd'
                                                                                                )
                )
                BEGIN
                    SELECT 0 Code,
                           'Success' Message,
                           a.PlanId,
                           a.PlanName,
                           b.StaticDataLabel AS PlanTime,
                           CAST(ISNULL(a.Price, 0) AS INT) AS PlanPrice,
                           a.Nomination AS PlanNomination,
                           ISNULL(a.Remarks, '') AS PlanRemarks,
                           c.StaticDataLabel AS PlanLiquor
                    FROM dbo.tbl_plans a WITH (NOLOCK)
                        LEFT JOIN dbo.tbl_static_data b WITH (NOLOCK)
                            ON b.StaticDataType = 8
                               AND b.StaticDataValue = a.PlanTime
                               AND ISNULL(b.Status, '') = 'A'
                        LEFT JOIN dbo.tbl_static_data c WITH (NOLOCK)
                            ON c.StaticDataType = 9
                               AND c.StaticDataValue = a.Liquor
                               AND ISNULL(c.Status, '') = 'A'
                    WHERE a.PlanCategory = 2
                          AND ISNULL(a.PlanStatus, '') = 'A'
                    ORDER BY a.Price ASC;

                    RETURN;
                END;
            END;

            SELECT 9 AS Code,
                   N'現在、プランはありません' AS Message; --'Invalid request. Please select other clubs' 
            RETURN;
        END;
    END;

    ELSE IF @Flag = 'hlvc' --Get host list via club
    BEGIN
        SELECT b.AgentId ClubId,
               b.HostId,
               b.HostName AS HostNameEnglish,
               b.HostNameJapanese,
               b.ImagePath AS HostImage,
               b.Position AS HostPosition
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_host_details b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND ISNULL(a.[Status], '') = 'A'
                   AND ISNULL(b.[Status], '') = 'A'
        WHERE a.AgentId = @ClubId
        ORDER BY CAST(b.Rank AS INT) ASC;

        RETURN;
    END;

    ELSE IF @Flag = 'ghlfr' --get host list for reservation
    BEGIN
        SET @SQLString
            = N'
			select a.ImagePath AS HostImage
			from dbo.tbl_host_details a with (nolock)
			where ISNULL(a.[Status], '''') = ''A'' AND a.AgentId = ' + @ClubId + N' AND a.HostId in ('
              + CAST(@HostListId AS VARCHAR(MAX)) + N')';

        PRINT (@SQLString);
        EXEC (@SQLString);
        RETURN;
    END;

    ELSE IF @Flag = 'gcrbd' --get customer reservation billing details
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_plans a WITH (NOLOCK)
            WHERE a.PlanId = @PlanId
                  AND ISNULL(a.PlanStatus, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid Plan' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
            WHERE a.AgentId = @ClubId
                  AND ISNULL(a.Status, '') = 'A'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid Club' Message;
            RETURN;
        END;

        SELECT @CustomerUserName = a.NickName
        FROM dbo.tbl_customer a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 3
                   AND ISNULL(b.Status, '') = 'A'
        WHERE a.AgentId = @CustomerId;

        SELECT @CustomerCostAmount = ISNULL(@NoOfPeople, 0) * ISNULL(a.Price, 0)
        FROM dbo.tbl_plans a WITH (NOLOCK)
        WHERE a.PlanId = @PlanId
              AND ISNULL(a.PlanStatus, '') = 'A';

        IF (@CustomerCostAmount <= 0)
        BEGIN
            SELECT 1 Code,
                   'Something went wrong. Please try again later' Message;
            RETURN;
        END;

        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
            WHERE a.CustomerId = @CustomerId
        )
        BEGIN
            SELECT @ReservationType = 1,
                   @CustomerCostAmount = 0;
        END;

        SELECT 0 COde,
               'Success' Message,
               @CustomerUserName AS CustomerUserName,
               CAST(@CustomerCostAmount AS INT) AS CustomerCostAmount,
               CASE
                   WHEN ISNULL(@ReservationType, '') <> ''
                        AND @ReservationType = '1' THEN
                       N'お知らせ: ご初回予約で、ホストとのプレミアムな時間を0円で体験してください。' --'Notice: Experience premium time with a host for 0 yen when you make your first reservation.'
                   ELSE
                       ''
               END AS ReservationRemark;
        RETURN;
    END;
END;
GO


