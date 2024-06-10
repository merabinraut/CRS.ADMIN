USE [CRS.UAT_V2]
GO

/****** Object:  StoredProcedure [dbo].[sproc_email_sms_management]    Script Date: 6/7/2024 10:36:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_email_sms_management] @Flag VARCHAR(10)
	,@EmailSendTo VARCHAR(4000) = NULL
	,@EmailSendToCC VARCHAR(4000) = NULL
	,@EmailSendToBCC VARCHAR(4000) = NULL
	,@MobileNumber VARCHAR(15) = NULL
	,@ActionUser NVARCHAR(200) = NULL
	,@ActionIP VARCHAR(200) = NULL
	,@ActionPlatform VARCHAR(20) = NULL
	,@Username NVARCHAR(200) = NULL
	,@AgentId VARCHAR(10) = NULL
	,@UserId VARCHAR(10) = NULL
	,@VerificationCode VARCHAR(6) = NULL
	,@ExtraDatailId1 VARCHAR(10) = NULL
	,@RandomPassword VARCHAR(50) = NULL
	,@ClubId VARCHAR(10) = NULL
	,@ResponseCode INT OUTPUT
AS
DECLARE @Sno BIGINT
	,@Sno2 BIGINT
	,@Sno3 BIGINT
	,@StringSQL VARCHAR(MAX)
	,@StringSQL2 VARCHAR(MAX)
	,@TransactionName VARCHAR(200)
	,@ErrorDesc VARCHAR(MAX)
	,@EmailContent NVARCHAR(MAX);
DECLARE @MessageContent NVARCHAR(MAX)
	,@EmailSendBy VARCHAR(200)
	,@ErrorResponseCode INT = 1
	,@SuccessResponseCode INT = 0;

BEGIN TRY
	IF @Flag = '1' -- customer :- Registration( E-mail & SMS)
	BEGIN
		DECLARE @LoginLink VARCHAR(MAX) = ' https://uatcustomer.hoslog.jp/login ';

		SELECT @MessageContent = ISNULL(@Username, '') + N' 様、当サイトのHoslogにご登録いただきありがとうございます。お客様のアカウントは現在使える状態になりましたら、' + ISNULL(@LoginLink, '') + N'でご自身の認証情報を使用してログインし、ご利用を開始してください。[株式会社システム、ホスログ]
				'
			,@EmailSendBy = 'info@hoslog.jp';

		INSERT INTO dbo.tbl_sms_sent (
			RoleId
			,AgentId
			,UserId
			,DestinationNumber
			,Message
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,-- RoleId - bigint
			@AgentId
			,-- AgentId - bigint
			@UserId
			,-- UserId - bigint
			@MobileNumber
			,-- DestinationNumber - varchar(15)
			@MessageContent
			,'P'
			,-- Status - char(1)		   
			@ActionUser
			,-- CreatedBy - varchar(200)
			GETDATE()
			,-- CreatedDate - datetime
			@ActionIP
			,-- CreatedIP - varchar(50)
			@ActionPlatform -- CreatedPlatform - varchar(20)
			);

		INSERT INTO tbl_customer_notification (
			ToAgentId
			,NotificationType
			,NotificationSubject
			,NotificationBody
			,NotificationStatus
			,NotificationReadStatus
			,CreatedBy
			,CreatedDate
			,NotificationURL
			,AdditionalDetail1
			,ToAgentType
			)
		VALUES (
			@AgentId
			,'Registration Message'
			,N'会員登録ありがとうございます'
			,N'ホストクラブ初回来店サイト「ホスログ」が、業界初のネット予約に対応しました。本日より一部先行プレオープンいたします！
☆バグ，不具合改め修中'
			,'A'
			,'P'
			,@ActionUser
			,GETDATE()
			,'#'
			,''
			,@ActionPlatform
			);

		SET @Sno2 = SCOPE_IDENTITY();

		UPDATE tbl_customer_notification
		SET notificationId = @Sno2
		WHERE Sno = @Sno2;

		SET @ResponseCode = @SuccessResponseCode;

		SELECT @ResponseCode;

		RETURN;
	END;
	ELSE IF @Flag = '2' -- Customer :- Registration OTP (SMS)
	BEGIN
		SELECT @MessageContent = ISNULL(@Username, '') + N' 様、あなたのOTPコードは：' + ISNULL(@VerificationCode, '') + N'です。有効期限は10分間です。このコードを誰とも共有しないでください。-[株式会社システム、ホスログ]';

		INSERT INTO dbo.tbl_sms_sent (
			RoleId
			,AgentId
			,UserId
			,DestinationNumber
			,Message
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,-- RoleId - bigint
			@AgentId
			,-- AgentId - bigint
			@UserId
			,-- UserId - bigint
			@MobileNumber
			,-- DestinationNumber - varchar(15)
			@MessageContent
			,'P'
			,-- Status - char(1)		   
			@ActionUser
			,-- CreatedBy - varchar(200)
			GETDATE()
			,-- CreatedDate - datetime
			@ActionIP
			,-- CreatedIP - varchar(50)
			@ActionPlatform -- CreatedPlatform - varchar(20)
			);

		INSERT INTO dbo.tbl_verification_sent (
			RoleId
			,AgentId
			,UserId
			,MobileNumber
			,FullName
			,VerificationCode
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,@AgentId
			,@UserId
			,@MobileNumber
			,@Username
			,@VerificationCode
			,'S'
			,--sent
			@ActionUser
			,GETDATE()
			,@ActionIP
			,@ActionPlatform
			);

		SET @Sno = SCOPE_IDENTITY();

		UPDATE dbo.tbl_verification_sent
		SET STATUS = 'E'
			,--Expired
			UpdatedBy = @ActionUser
			,UpdatedDate = GETDATE()
			,UpdatedIP = @ActionIP
			,UpdatedPlatform = @ActionPlatform
		WHERE MobileNumber = @MobileNumber
			AND AgentId = @AgentId
			AND UserId = @UserId
			AND ISNULL(STATUS, '') = 'P'
			AND Sno <> @Sno;

		SET @ResponseCode = @SuccessResponseCode;

		SELECT @ResponseCode;

		RETURN;
	END;
	ELSE IF @Flag = '3' -- Customer :-  Forgot Password (Option between Email & SMS) 
	BEGIN
		SELECT @MessageContent = ISNULL(@Username, '') + N' 様、あなたのワンタイムパスワードは：' + ISNULL(@VerificationCode, '') + N'です。パスワードリセットを完了するために、次の[指定された時間]内にこのコードを入力してください。
本メールがご自身宛でない場合、他の方が誤って同じメールアドレスを登録したものと考えられます。
お心当たりのない方は、お手数ですがメール本文を削除くださいますようお願いいたします。ありがとうございます、[株式会社システム、ホスログ]'
			,@EmailContent = N'<!DOCTYPE html>
<html lang="en,jp">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Forgot Password</title>
    <script src="https://cdn.tailwindcss.com"></script>
  </head>
  <body>
    <div class="email container" style="max-width: 500px;   
    margin: 0 auto; padding: 32px; border-radius: 12px;
    background-color: #faf2f6;">
      <header class="email-header">
        <a href="#">
          <img style="height: 48px; width:auto;" src="https://uatcustomer.hoslog.jp/Content/assets/images/logo.svg" alt="Hoslog" />
        </a>
      </header>
      <div class="email-body" style="margin-top: 12px;">
        <h2 style="color:#2b2b2b; 
            font-family: ''Times New Roman'', Times, serif; ">' + ISNULL(@Username, '') + 
			N'様、</h2>
        <p style="color:#2b2b2b; margin-top: 12px;; line-height: 1.35; "> あなたのワンタイムパスワードは： <br>
        <div class="otp-container" style="padding: 12px; margin-top: 12px; background-color: #ffffff;
                border-radius: 5px;    text-align: center; ">
          <span class="otp" style="color: #D75A8B;">' + ISNULL(@VerificationCode, '') + 
			N' </span>
        </div>
        <br> パスワードリセットを完了するために、次の[指定された時間]内にこのコードを入力してください。 <br> 本メールがご自身宛でない場合、他の方が誤って同じメールアドレスを登録したものと考えられます。 <br> お心当たりのない方は、お手数ですがメール本文を削除くださいますようお願いいたします。 <br>
        </p>
        <br>
        <p style="margin-top: 4px; font-size: semi-bold; color: #D75A8B;"> ありがとうございます、 <br> [株式会社システム、ホスログ] </p>
      </div>
    </div>
  </body>
</html>'
			,@EmailSendBy = 'info@hoslog.jp';

		INSERT INTO dbo.tbl_sms_sent (
			RoleId
			,AgentId
			,UserId
			,DestinationNumber
			,Message
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,-- RoleId - bigint
			@AgentId
			,-- AgentId - bigint
			@UserId
			,-- UserId - bigint
			@MobileNumber
			,-- DestinationNumber - varchar(15)
			@MessageContent
			,'P'
			,-- Status - char(1)		   
			@ActionUser
			,-- CreatedBy - varchar(200)
			GETDATE()
			,-- CreatedDate - datetime
			@ActionIP
			,-- CreatedIP - varchar(50)
			@ActionPlatform -- CreatedPlatform - varchar(20)
			);

		INSERT INTO dbo.tbl_verification_sent (
			RoleId
			,AgentId
			,UserId
			,MobileNumber
			,FullName
			,VerificationCode
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,@AgentId
			,@UserId
			,@MobileNumber
			,@Username
			,@VerificationCode
			,'S'
			,--sent
			@ActionUser
			,GETDATE()
			,@ActionIP
			,@ActionPlatform
			);

		SET @Sno = SCOPE_IDENTITY();

		UPDATE dbo.tbl_verification_sent
		SET STATUS = 'E'
			,--Expired
			UpdatedBy = @ActionUser
			,UpdatedDate = GETDATE()
			,UpdatedIP = @ActionIP
			,UpdatedPlatform = @ActionPlatform
		WHERE MobileNumber = @MobileNumber
			AND AgentId = @AgentId
			AND UserId = @UserId
			AND ISNULL(STATUS, '') = 'P'
			AND Sno <> @Sno;

		IF ISNULL(@EmailSendBy, '') != ''
			AND ISNULL(@EmailSendTo, '') != ''
		BEGIN
			INSERT INTO dbo.tbl_email_request (
				EmailSubject
				,EmailText
				,EmailSendBy
				,EmailSendTo
				,EmailSendToCC
				,EmailSendToBCC
				,EmailSendStatus
				,IsImportant
				,STATUS
				,CreatedBy
				,CreatedDate
				,CreatedIP
				,CretaedPlatform
				)
			VALUES (
				'Forgot Password-OTP'
				,-- EmailSubject - nvarchar(600)
				ISNULL(@EmailContent, '')
				,-- EmailText - nvarchar(max)
				@EmailSendBy
				,-- EmailSendBy - varchar(256)
				@EmailSendTo
				,-- EmailSendTo - varchar(5000)
				@EmailSendToCC
				,-- EmailSendToCC - varchar(5000)
				@EmailSendToBCC
				,-- EmailSendToBCC - varchar(5000)
				'P'
				,-- EmailSendStatus - char(1)
				'Y'
				,-- IsImportant - char(1)
				'P'
				,-- Status - char(1)
				@ActionUser
				,-- CreatedBy - nvarchar(200)
				GETDATE()
				,-- CreatedDate - datetime
				@ActionIP
				,-- CreatedIP - varchar(50)
				@ActionPlatform -- CretaedPlatform - varchar(20)
				);
		END;

		SET @ResponseCode = @SuccessResponseCode;

		SELECT @ResponseCode;

		RETURN;
	END;
	ELSE IF @Flag = '4' --Customer :- Booking Details ( E-Mail)
	BEGIN
		SELECT @MessageContent = ISNULL(@Username, '') + N' 様、当サイトホスログへのご予約をしていただき誠にありがとうございます。お客様の予約は完了しました。'
			,@EmailSendBy = 'booking@hoslog.jp';

		IF ISNULL(@EmailSendBy, '') != ''
			AND ISNULL(@EmailSendTo, '') != ''
		BEGIN
			INSERT INTO dbo.tbl_email_request (
				EmailSubject
				,EmailText
				,EmailSendBy
				,EmailSendTo
				,EmailSendToCC
				,EmailSendToBCC
				,EmailSendStatus
				,IsImportant
				,STATUS
				,CreatedBy
				,CreatedDate
				,CreatedIP
				,CretaedPlatform
				)
			VALUES (
				'Booking Details'
				,-- EmailSubject - nvarchar(600)
				@MessageContent
				,-- EmailText - nvarchar(max)
				@EmailSendBy
				,-- EmailSendBy - varchar(256)
				@EmailSendTo
				,-- EmailSendTo - varchar(5000)
				@EmailSendToCC
				,-- EmailSendToCC - varchar(5000)
				@EmailSendToBCC
				,-- EmailSendToBCC - varchar(5000)
				'P'
				,-- EmailSendStatus - char(1)
				'Y'
				,-- IsImportant - char(1)
				'P'
				,-- Status - char(1)
				@ActionUser
				,-- CreatedBy - nvarchar(200)
				GETDATE()
				,-- CreatedDate - datetime
				@ActionIP
				,-- CreatedIP - varchar(50)
				@ActionPlatform -- CretaedPlatform - varchar(20)
				);

			SET @ResponseCode = @SuccessResponseCode;

			SELECT @ResponseCode;

			RETURN;
		END;
	END;
	ELSE IF @Flag = '5' --Customer :- Customer Validation (SMS)( Feedback link attached.)
	BEGIN
		DECLARE @ReviewRedirectURL VARCHAR(MAX);

		SELECT @ReviewRedirectURL = 'https://uatcustomer.hoslog.jp/ReviewManagement/Review?CustomerId=' + CAST(a.CustomerId AS VARCHAR) + '&&ReservationId=' + CAST(a.ReservationId AS VARCHAR)
		FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
		WHERE a.ReservationId = @ExtraDatailId1
			AND ISNULL(a.[TransactionStatus], '') = 'S';

		IF ISNULL(@ReviewRedirectURL, '') <> ''
		BEGIN
			SELECT @MessageContent = ISNULL(@ReviewRedirectURL, '')
				,@EmailSendBy = 'info@hoslog.jp';

			INSERT INTO dbo.tbl_sms_sent (
				RoleId
				,AgentId
				,UserId
				,DestinationNumber
				,Message
				,STATUS
				,CreatedBy
				,CreatedDate
				,CreatedIP
				,CreatedPlatform
				)
			VALUES (
				3
				,-- RoleId - bigint
				@AgentId
				,-- AgentId - bigint
				@UserId
				,-- UserId - bigint
				@MobileNumber
				,-- DestinationNumber - varchar(15)
				@MessageContent
				,'P'
				,-- Status - char(1)		   
				@ActionUser
				,-- CreatedBy - varchar(200)
				GETDATE()
				,-- CreatedDate - datetime
				@ActionIP
				,-- CreatedIP - varchar(50)
				@ActionPlatform -- CreatedPlatform - varchar(20)
				);

			INSERT INTO dbo.tbl_customer_notification (
				ToAgentId
				,NotificationType
				,NotificationSubject
				,NotificationBody
				,NotificationStatus
				,NotificationReadStatus
				,CreatedBy
				,CreatedDate
				,NotificationURL
				,AdditionalDetail1
				)
			SELECT a.CustomerId
				,N'レビュー'
				,--Review
				N'クラブのレビュー'
				,--Club review
				N'ご確認ください'
				,--Please review
				'A'
				,'P'
				,@ActionUser
				,GETDATE()
				,ISNULL(@ReviewRedirectURL, '#')
				,@ExtraDatailId1
			FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
			WHERE a.ReservationId = @ExtraDatailId1
				AND ISNULL(a.[TransactionStatus], '') = 'S';

			SET @Sno = SCOPE_IDENTITY();

			UPDATE dbo.tbl_customer_notification
			SET notificationId = @Sno
			WHERE Sno = @Sno;
		END;

		SET @ResponseCode = @SuccessResponseCode;

		SELECT @ResponseCode;

		RETURN;
	END;
	ELSE IF @Flag = '6' --Customer :- Booking confirmation.( SMS)
	BEGIN
		SELECT @MessageContent = ISNULL(@Username, '') + N' 様、予約リクエストが承認されました。お越しいただけるのを楽しみにしています！確認のためのOTP：' + ISNULL(@VerificationCode, '') + N'にお会いできることを楽しみにしています。予約の変更が必要な場合や、その他の質問がある場合は、お気軽にお問い合わせください。ホスログを選んでいただき、ありがとうございます。訪問日にお会いしましょう！ありがとうございます、[株式会社システム、ホスログ]'
			,@EmailContent = N'<!DOCTYPE html>
<html lang="en,jp">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Forgot Password</title>
    <script src="https://cdn.tailwindcss.com"></script>
  </head>
  <body>
    <div class="email container" style="max-width: 500px;   
    margin: 0 auto; padding: 32px; border-radius: 12px;
    background-color: #faf2f6;">
      <header class="email-header">
        <a href="#">
          <img style="height: 48px; width:auto;" src="https://uatcustomer.hoslog.jp/Content/assets/images/logo.svg" alt="Hoslog" />
        </a>
      </header>
      <div class="email-body" style="margin-top: 12px;">
        <h2 style="color:#2b2b2b; 
            font-family: ''Times New Roman'', Times, serif; ">' + ISNULL(@Username, '') + 
			N'様、</h2>
        
        <br> 予約リクエストが承認されました。 <br> お越しいただけるのを楽しみにしています！ 
        <p style="color:#2b2b2b; margin-top: 12px;; line-height: 1.35; "> 確認のためのOTP： <br>
        <div class="otp-container" style="padding: 12px; margin-top: 12px; background-color: #ffffff;
                border-radius: 5px;    text-align: center; ">
          <span class="otp" style="color: #D75A8B;">' + ISNULL(@VerificationCode, '') + 
			N' </span>
        </div>
        <br> 予約の変更が必要な場合や、その他の質問がある場合は、お気軽にお問い合わせください。<br>
        ホスログを選んでいただき、ありがとうございます。<br>
        </p>
        <p  style="color: #D75A8B;">
         <br>訪問日にお会いしましょう！ありがとうございます、
        <br> [株式会社システム、ホスログ]</p>
      </div>
    </div>
  </body>
</html>'
			,@EmailSendBy = 'booking@hoslog.jp';

		INSERT INTO dbo.tbl_club_notification (
			ToAgentId
			,NotificationType
			,NotificationSubject
			,NotificationBody
			,NotificationStatus
			,NotificationReadStatus
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@ClubId
			,1 --Reservation Notification
			,'Reservation Notification'
			,ISNULL(@Username, 'Customer') + ' has made a reservation.'
			,'A'
			,'P'
			,0
			,GETDATE()
			)

		SET @Sno3 = SCOPE_IDENTITY();

		UPDATE dbo.tbl_club_notification
		SET notificationId = @Sno3
		WHERE Sno = @Sno3;

		IF ISNULL(@MobileNumber, '') <> ''
		BEGIN
			INSERT dbo.tbl_sms_sent (
				RoleId
				,AgentId
				,UserId
				,DestinationNumber
				,Message
				,STATUS
				,CreatedBy
				,CreatedDate
				,CreatedIP
				,CreatedPlatform
				,SMSType
				)
			VALUES (
				3
				,-- RoleId - bigint
				@AgentId
				,-- AgentId - bigint
				@UserId
				,-- UserId - bigint
				@MobileNumber
				,-- DestinationNumber - varchar(15)
				N'' + @MessageContent + ''
				,-- Message - varchar(max)
				'S'
				,-- Status - char(1)
				@ActionUser
				,-- CreatedBy - varchar(200)
				GETDATE()
				,-- CreatedDate - datetime
				@ActionIP
				,-- CreatedIP - varchar(50)
				@ActionPlatform
				,-- CreatedPlatform - varchar(20)
				'Res-OTP' -- SMSType - varchar(20)
				);

			SET @Sno = SCOPE_IDENTITY();

			INSERT INTO dbo.tbl_customer_reservation_otp (
				ReservationId
				,SMSId
				,CustomerId
				,MobileNumber
				,FullName
				,OTPCode
				,STATUS
				,CreatedBy
				,CreatedDate
				,CreatedIP
				,CreatedPlatform
				)
			VALUES (
				@ExtraDatailId1
				,-- ReservationId - bigint
				@Sno
				,-- SMSId - bigint
				@AgentId
				,-- CustomerId - bigint
				@MobileNumber
				,-- MobileNumber - varchar(15)
				N'' + @Username + ''
				,-- FullName - varchar(256)
				@VerificationCode
				,-- OTPCode - varchar(6)
				'P'
				,-- Status - char(1)
				@ActionUser
				,-- CreatedBy - varchar(200)
				GETDATE()
				,-- CreatedDate - datetime
				@ActionIP
				,-- CreatedIP - varchar(50)
				@ActionPlatform -- CreatedPlatform - varchar(20)
				);
				--INSERT INTO dbo.tbl_customer_notification
				--(
				--    ToAgentId,
				--    NotificationType,
				--    NotificationSubject,
				--    NotificationBody,
				--    NotificationStatus,
				--    NotificationReadStatus,
				--    CreatedBy,
				--    CreatedDate,
				--    NotificationURL
				--)
				--SELECT a.CustomerId,
				--       'Review',
				--       'Club review',
				--       'Please review',
				--       'A',
				--       'P',
				--       @ActionUser,
				--       GETDATE(),
				--       @ReviewRedirectURL
				--FROM dbo.tbl_reservation_detail a WITH (NOLOCK)
				--WHERE a.ReservationId = @ExtraDatailId1
				--      AND ISNULL(a.[TransactionStatus], '') = 'A';
				--SET @Sno = SCOPE_IDENTITY();
				--UPDATE dbo.tbl_customer_notification
				--SET notificationId = @Sno
				--WHERE Sno = @Sno;
		END;

		IF ISNULL(@EmailSendBy, '') != ''
			AND ISNULL(@EmailSendTo, '') != ''
		BEGIN
			INSERT INTO dbo.tbl_email_request (
				EmailSubject
				,EmailText
				,EmailSendBy
				,EmailSendTo
				,EmailSendToCC
				,EmailSendToBCC
				,EmailSendStatus
				,IsImportant
				,STATUS
				,CreatedBy
				,CreatedDate
				,CreatedIP
				,CretaedPlatform
				)
			VALUES (
				'Booking confirmation-OTP'
				,-- EmailSubject - nvarchar(600)
				N'' + @EmailContent + ''
				,-- EmailText - nvarchar(max)
				@EmailSendBy
				,-- EmailSendBy - varchar(256)
				@EmailSendTo
				,-- EmailSendTo - varchar(5000)
				@EmailSendToCC
				,-- EmailSendToCC - varchar(5000)
				@EmailSendToBCC
				,-- EmailSendToBCC - varchar(5000)
				'P'
				,-- EmailSendStatus - char(1)
				'Y'
				,-- IsImportant - char(1)
				'P'
				,-- Status - char(1)
				@ActionUser
				,-- CreatedBy - nvarchar(200)
				GETDATE()
				,-- CreatedDate - datetime
				@ActionIP
				,-- CreatedIP - varchar(50)
				@ActionPlatform -- CretaedPlatform - varchar(20)
				);

			SET @ResponseCode = @SuccessResponseCode;

			SELECT @ResponseCode;

			RETURN;
		END;

		SET @ResponseCode = @SuccessResponseCode;

		SELECT @ResponseCode;

		RETURN;
	END;
	ELSE IF @Flag = '7' -- customer :- Reset Password(SMS)
	BEGIN
		-- DECLARE @LoginLink VARCHAR(MAX) = ' https://uatcustomer.hoslog.jp/Home/INDEX ';
		SELECT @MessageContent = 'Dear ' + @Username + ' ,Your password is ' + @RandomPassword
			,@EmailSendBy = 'info@hoslog.jp';

		INSERT INTO dbo.tbl_sms_sent (
			RoleId
			,AgentId
			,UserId
			,DestinationNumber
			,Message
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,-- RoleId - bigint
			@AgentId
			,-- AgentId - bigint
			@UserId
			,-- UserId - bigint
			@MobileNumber
			,-- DestinationNumber - varchar(15)
			@MessageContent
			,'P'
			,-- Status - char(1)		   
			@ActionUser
			,-- CreatedBy - varchar(200)
			GETDATE()
			,-- CreatedDate - datetime
			@ActionIP
			,-- CreatedIP - varchar(50)
			@ActionPlatform -- CreatedPlatform - varchar(20)
			);
	END
	ELSE IF @Flag = '8' -- club :- Reset Password(SMS)
	BEGIN
		-- DECLARE @LoginLink VARCHAR(MAX) = ' https://uatcustomer.hoslog.jp/Home/INDEX ';
		SELECT @MessageContent = ISNULL(@Username, '') + ' ,Your password is ' + @RandomPassword
			,@EmailSendBy = 'info@hoslog.jp';

		INSERT INTO dbo.tbl_sms_sent (
			RoleId
			,AgentId
			,UserId
			,DestinationNumber
			,Message
			,STATUS
			,CreatedBy
			,CreatedDate
			,CreatedIP
			,CreatedPlatform
			)
		VALUES (
			3
			,-- RoleId - bigint
			@AgentId
			,-- AgentId - bigint
			@UserId
			,-- UserId - bigint
			@MobileNumber
			,-- DestinationNumber - varchar(15)
			@MessageContent
			,'P'
			,-- Status - char(1)		   
			@ActionUser
			,-- CreatedBy - varchar(200)
			GETDATE()
			,-- CreatedDate - datetime
			@ActionIP
			,-- CreatedIP - varchar(50)
			@ActionPlatform -- CreatedPlatform - varchar(20)
			);
	END
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION @TransactionName;

	SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

	INSERT INTO dbo.tbl_error_log (
		ErrorDesc
		,ErrorScript
		,QueryString
		,ErrorCategory
		,ErrorSource
		,ActionDate
		)
	VALUES (
		@ErrorDesc
		,'sproc_email_sms_management(Flag: ' + ISNULL(@Flag, '') + ')'
		,'SQL'
		,'SQL'
		,'sproc_email_sms_management'
		,GETDATE()
		);

	SET @ResponseCode = @ErrorResponseCode;

	SELECT @ResponseCode;

	RETURN;
END CATCH;
GO


