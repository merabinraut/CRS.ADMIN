USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_reservation_history_management_v2]    Script Date: 2/16/2024 5:52:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER PROC [dbo].[sproc_reservation_history_management_v2]
    @Flag VARCHAR(10),
    @ReservationId VARCHAR(10) = NULL,
    @CustomerId VARCHAR(10) = NULL
AS
DECLARE @CurrentYear INT = YEAR(GETDATE());
BEGIN
    IF ISNULL(@Flag, '') = 'grhl' --get reserved history list
    BEGIN
        SELECT rd.ReservationId,
            rd.ClubId,
            rd.ReservedDate,
            CONVERT(
                          DATE,
                          NULLIF(CAST(rd.VisitDate AS VARCHAR(MAX)), '') + '-' + CAST(@CurrentYear AS VARCHAR(4)),
                          110
                      ) AS VisitDate,
            rd.VisitTime,
            rd.InvoiceId,
            rd.TransactionStatus,
            cd.ClubName1 AS ClubNameEng,
            cd.ClubName2 AS ClubNamejp,
            rpd.Price,
            rd.CustomerId,
            cd.Logo AS ClubLogo,
            ISNULL(cd.LocationURL, '#') AS LocationURL
        FROM tbl_reservation_detail rd
            INNER JOIN dbo.tbl_club_details cd
            ON cd.AgentId = rd.ClubId
            INNER JOIN dbo.tbl_reservation_plan_detail rpd
            ON rpd.ReservationId = rd.ReservationId
        WHERE rd.CustomerId = @CustomerId
            AND TransactionStatus IN ( 'A', 'P' )
            AND FORMAT(rd.ReservedDate, 'yyyy-MM-dd') >= FORMAT(GETDATE(), 'yyyy-MM-dd');
    END;
    IF ISNULL(@Flag, '') = 'gvhl' --get visited history list
    BEGIN
        SELECT rd.ReservationId,
            rd.ClubId,
            rd.ReservedDate,
            CONVERT(
                          DATE,
                          NULLIF(CAST(rd.VisitDate AS VARCHAR(MAX)), '') + '-' + CAST(@CurrentYear AS VARCHAR(4)),
                          110
                      ) AS VisitDate,
            rd.VisitTime,
            rd.InvoiceId,
            rd.TransactionStatus,
            cd.ClubName1 AS ClubNameEng,
            cd.ClubName2 AS ClubNamejp,
            rpd.Price,
            rd.CustomerId,
            cd.Logo AS ClubLogo,
            ISNULL(cd.LocationURL, '#') AS LocationURL
        FROM tbl_reservation_detail rd
            INNER JOIN dbo.tbl_club_details cd
            ON cd.AgentId = rd.ClubId
            INNER JOIN dbo.tbl_reservation_plan_detail rpd
            ON rpd.ReservationId = rd.ReservationId
        WHERE rd.CustomerId = @CustomerId
            AND TransactionStatus IN ( 'A' )
            AND FORMAT(rd.ReservedDate, 'yyyy-MM-dd') < FORMAT(GETDATE(), 'yyyy-MM-dd');
    END;
    IF ISNULL(@Flag, '') = 'gchl' --get cancelled history list
    BEGIN
        SELECT rd.ReservationId,
            rd.ClubId,
            rd.ReservedDate,
            CONVERT(
                          DATE,
                          NULLIF(CAST(rd.VisitDate AS VARCHAR(MAX)), '') + '-' + CAST(@CurrentYear AS VARCHAR(4)),
                          110
                      ) AS VisitDate,
            rd.VisitTime,
            rd.InvoiceId,
            rd.TransactionStatus,
            cd.ClubName1 AS ClubNameEng,
            cd.ClubName2 AS ClubNamejp,
            rpd.Price,
            rd.CustomerId,
            cd.Logo AS ClubLogo,
            ISNULL(cd.LocationURL, '#') AS LocationURL
        FROM tbl_reservation_detail rd
            INNER JOIN dbo.tbl_club_details cd
            ON cd.AgentId = rd.ClubId
            INNER JOIN dbo.tbl_reservation_plan_detail rpd
            ON rpd.ReservationId = rd.ReservationId
        WHERE rd.CustomerId = @CustomerId
            AND rd.TransactionStatus IN ( 'C' );
    END;
    IF ISNULL(@Flag, '') = 'gahl' --get all history list
    BEGIN
        SELECT rd.ReservationId,
            rd.ClubId,
            rd.ReservedDate,
            CONVERT(
                          DATE,
                          NULLIF(CAST(rd.VisitDate AS VARCHAR(MAX)), '') + '-' + CAST(@CurrentYear AS VARCHAR(4)),
                          110
                      ) AS VisitDate,
            rd.VisitTime,
            rd.InvoiceId,
            rd.TransactionStatus,
            cd.ClubName1 AS ClubNameEng,
            cd.ClubName2 AS ClubNamejp,
            rpd.Price,
            rd.CustomerId,
            cd.Logo AS ClubLogo,
            ISNULL(cd.LocationURL, '#') AS LocationURL
        FROM tbl_reservation_detail rd
            INNER JOIN dbo.tbl_club_details cd
            ON cd.AgentId = rd.ClubId
            INNER JOIN dbo.tbl_reservation_plan_detail rpd
            ON rpd.ReservationId = rd.ReservationId
        WHERE rd.CustomerId = @CustomerId
            AND rd.TransactionStatus IN ( 'A', 'P', 'S', 'R', 'I', 'C' );
    END;
    IF ISNULL(@Flag,'')='grhd'--get reservation history detail
    BEGIN
        SELECT
            rd.ReservationId AS ReservationId,
            rd.CustomerId AS CustomerId,
            cd.AgentId AS ClubId,
            cd.ClubName1 AS ClubNameEng,
            cd.ClubName2 AS ClubNameJp,
            cd.Logo AS ClubLogo,
            l.LocationName,
            CONVERT(DATE, NULLIF(CAST(rd.VisitDate AS VARCHAR(MAX)), '') + '-' + CAST(@CurrentYear AS VARCHAR(4)), 110) AS VisitDate,
            rd.VisitTime AS VisitTime,
            rpd.PlanName AS PlanName,
            rpd.Price AS Price,
            rpd.Nomination AS Nomination,
            rpd.Liquor AS Liquor,
            STRING_AGG(hd.ImagePath, ', ') AS HostImages
        FROM tbl_reservation_detail rd
            INNER JOIN tbl_club_details cd ON cd.AgentId = rd.ClubId
            INNER JOIN tbl_location l ON l.LocationId = cd.LocationId
            INNER JOIN tbl_reservation_host_detail rhd ON rd.ReservationId = rhd.ReservationId
            INNER JOIN tbl_host_details hd ON hd.HostId = rhd.HostId
            INNER JOIN tbl_reservation_plan_detail rpd ON rpd.ReservationId = rd.ReservationId
        WHERE rd.CustomerId = 23 AND rd.ReservationId = 231 AND rd.TransactionStatus IN ('A', 'S','P')
        GROUP BY 
            rd.ReservationId,
            rd.CustomerId,
            cd.AgentId,
            cd.ClubName1,
            cd.ClubName2,
            cd.Logo,
            l.LocationName,
            rd.VisitDate,
            rd.VisitTime,
            rpd.PlanName,
            rpd.Price,
            rpd.Nomination,
            rpd.Liquor;
    END
END;
GO


