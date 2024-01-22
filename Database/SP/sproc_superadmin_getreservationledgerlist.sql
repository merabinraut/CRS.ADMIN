USE [CRS]
GO

/****** Object:  StoredProcedure [dbo].[sproc_superadmin_getreservationledgerlist]    Script Date: 21/11/2023 14:58:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sproc_superadmin_getreservationledgerlist]
    -- Add the parameters for the stored procedure here
    @Flag VARCHAR(10) = NULL
AS
BEGIN

    IF ISNULL(@Flag, '') = 'grll' --get reservation ledger list
    BEGIN
        SELECT rd.Sno AS Id,
               CONVERT(DATE, CONCAT(YEAR(GETDATE()), '-', CAST(rd.VisitDate AS VARCHAR)), 120) AS VisitedDate,
               sd.StaticDataLabel AS PaymentOption,
               ISNULL(rd.NoOfPeople, 0) AS TotalVisitor,
               rpd.Price AS AdminPayment,
               cd.ClubName1 AS ClubName,
               cd.Logo AS Image,
			   rd.TransactionStatus
        FROM dbo.tbl_reservation_detail rd WITH (NOLOCK)
            LEFT JOIN dbo.tbl_club_details cd WITH (NOLOCK)
                ON cd.AgentId = rd.ClubId
            LEFT JOIN dbo.tbl_static_data sd WITH (NOLOCK)
                ON sd.StaticDataType = 10
                   AND sd.StaticDataValue = rd.PaymentType
            LEFT JOIN dbo.tbl_reservation_plan_detail rpd WITH (NOLOCK)
                ON rpd.PlanDetailId = rd.PlanDetailId
		ORDER BY rd.ReservationId DESC;

		RETURN;
    END;

END;
GO


