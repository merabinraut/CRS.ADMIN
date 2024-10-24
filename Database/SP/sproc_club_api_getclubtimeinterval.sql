declare  @ClubId BIGINT = 441

BEGIN
    SET NOCOUNT ON
    DECLARE @starttime TIME(0),
            @endtime TIME(0),
            @interval INT = 30;
    SELECT @StartTime = ClubOpeningTime,
           @EndTime = ClubClosingTime
    FROM tbl_club_details
    WHERE AgentId = @ClubId;
    WITH cte (TimeInterval)
AS (
    SELECT CAST(CONVERT(CHAR(8), @starttime, 108) AS TIME)
    UNION ALL
    SELECT CAST(DATEADD(minute, @interval, TimeInterval) AS TIME)
    FROM cte
    WHERE (
        (CAST(DATEADD(minute, @interval, TimeInterval) AS TIME) < @endtime)
        OR
        (@starttime > @endtime AND CAST(DATEADD(minute, @interval, TimeInterval) AS TIME) > @starttime)
    )
)
    SELECT CAST(TimeInterval AS CHAR(5)) AS TimeInterval
    FROM cte
END
