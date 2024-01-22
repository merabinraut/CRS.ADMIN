USE [CRS];
GO

/****** Object:  StoredProcedure [dbo].[sproc_club_approveandrejectbookingrequest]    Script Date: 15/12/2023 15:47:22 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

ALTER PROCEDURE [dbo].[sproc_club_approveandrejectbookingrequest]
    -- Add the parameters for the stored procedure here
    @Flag VARCHAR(10) = NULL,
    @Id VARCHAR(10) = NULL,
    @ActionUser VARCHAR(100) = NULL,
    @ActionIp VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(100) = NULL
AS
DECLARE @AgentId VARCHAR(10) = NULL,
        @NickName NVARCHAR(MAX) = NULL,
        @OTPCode VARCHAR(10),
        @UserId VARCHAR(10),
		@MobileNumber VARCHAR(15),
		@SmsEmailResponseCode INT = 1;
BEGIN
    IF ISNULL(@Flag, '') = 'abr' --approve booking request
    BEGIN
        UPDATE dbo.tbl_reservation_detail
        SET TransactionStatus = 'A',
            ActionUser = @ActionUser,
            ActionDate = GETDATE(),
            ActionIP = @ActionIp,
            ActionPlatform = @ActionPlatform
        WHERE ReservationId = @Id;

        SELECT @AgentId = a.CustomerId,
               @NickName = b.NickName,
               @OTPCode = dbo.func_generate_otp_code(6),
               @UserId = c.UserId,
			   @MobileNumber = b.MobileNumber
        FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
            INNER JOIN dbo.tbl_customer b WITH (NOLOCK)
                ON b.AgentId = a.CustomerId
            INNER JOIN dbo.tbl_users c WITH (NOLOCK)
                ON c.AgentId = b.AgentId
                   AND c.RoleType = 3
        WHERE a.ReservationId = @Id;

        EXEC dbo.sproc_email_sms_management @Flag = '6',
										    @MobileNumber = @MobileNumber,
                                            @VerificationCode = @OTPCode,
                                            @Username = @NickName,
                                            @AgentId = @AgentId,
                                            @UserId = @UserId,
                                            @ActionUser = @ActionUser,
                                            @ActionIP = @ActionIp,
                                            @ActionPlatform = @ActionPlatform,
                                            @ExtraDatailId1 = @Id,
                                            @ResponseCode = @SmsEmailResponseCode OUTPUT;
        SELECT 0 Code,
               'Request approved successfully' Message;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'rbr' --reject booking request
    BEGIN
        UPDATE dbo.tbl_reservation_detail
        SET TransactionStatus = 'R',
            ActionUser = @ActionUser,
            ActionDate = GETDATE(),
            ActionIP = @ActionIp,
            ActionPlatform = @ActionPlatform
        WHERE ReservationId = @Id;
    END;
END;
GO


