USE [CRS_V2]
GO
/****** Object:  StoredProcedure [dbo].[sproc_club_management_approvalrejection]    Script Date: 4/23/2024 3:53:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[sproc_club_management_approvalrejection]
   @Flag VARCHAR(10) = '',
    @AgentId VARCHAR(20) = NULL,
    @ImageID VARCHAR(20) = NULL,
    @UserId VARCHAR(20) = NULL,
    @ClubId VARCHAR(20) = NULL,
    @LoginId VARCHAR(200) = NULL,
    @FirstName NVARCHAR(200) = NULL,
    @MiddleName NVARCHAR(200) = NULL,
    @LastName NVARCHAR(200) = NULL,
    @Email VARCHAR(500) = NULL,
    @MobileNumber VARCHAR(11) = NULL,
    @ClubName1 NVARCHAR(500) = NULL,
    @ClubName2 NVARCHAR(500) = NULL,
    @BusinessType VARCHAR(8) = NULL,
    @GroupName NVARCHAR(512) = NULL,
    @Description NVARCHAR(512) = NULL,
    @LocationURL VARCHAR(MAX) = NULL,
    @Longitude VARCHAR(75) = NULL,
    @Latitude VARCHAR(75) = NULL,
    @Logo VARCHAR(MAX) = NULL,
    @CoverPhoto VARCHAR(MAX) = NULL,
    @BusinessCertificate VARCHAR(MAX) = NULL,
    @Gallery VARCHAR(MAX) = NULL,
    @ActionUser VARCHAR(200) = NULL,
    @ActionIP VARCHAR(50) = NULL,
    @ActionPlatform VARCHAR(20) = NULL,
    @WebsiteLink VARCHAR(MAX) = NULL,
    @TiktokLink VARCHAR(MAX) = NULL,
    @TwitterLink VARCHAR(MAX) = NULL,
    @InstagramLink VARCHAR(MAX) = NULL,
    @ImagePath VARCHAR(MAX) = NULL,
    @ImageTitle NVARCHAR(150) = NULL,
    @ClubSno VARCHAR(10) = NULL,
    @Status CHAR(1) = NULL,
    @LocationId VARCHAR(10) = NULL,
    @CompanyName NVARCHAR(512) = NULL,
    @SearchFilter NVARCHAR(200) = NULL,
    @Skip INT = 0,
    @Take INT = 10,
    --@ClubPlanDetails  AS ClubPlanDetailsType READONLY,
    @Line VARCHAR(MAX) = NULL,
    @ceoFullName NVARCHAR(200) = NULL,
    @LastOrderTime VARCHAR(5) = NULL,
    @LastEntryTime VARCHAR(5) = NULL,
    @ClubOpeningTime VARCHAR(5) = NULL,
    @ClubClosingTime VARCHAR(5) = NULL,
    @Tax VARCHAR(10) = NULL,
    @holiday NVARCHAR(40) = NULL,
    @PostalCode VARCHAR(500) = NULL,
    @Prefecture VARCHAR(20) = NULL,
    @City NVARCHAR(200) = NULL,
    @InputStreet NVARCHAR(200) = NULL,
    @BuildingRoomNo NVARCHAR(50) = NULL,
    @RegularFee NVARCHAR(500) = NULL,
    @DesignationFee NVARCHAR(500) = NULL,
    @CompanionFee NVARCHAR(500) = NULL,
    @ExtensionFee NVARCHAR(500) = NULL,
    @VariousDrinks NVARCHAR(500) = NULL,
    @ClubPlanTypeId BIGINT = NULL,
    @Id BIGINT = NULL,
    @PlanListId BIGINT = NULL,
    @LandLineNumber VARCHAR(15) = NULL,
	@GroupNamekatakana NVARCHAR(500) = NULL,
	@CompanyAddress NVARCHAR(500) = NULL,
	@BusinessLicenseNumber NVARCHAR(50) = NULL,
	@LicenseIssuedDate datetime= NULL,
	@ClosingDate VARCHAR(50)= NULL,
	@KYCDocument  VARCHAR(MAX)= NULL,
	@Representative1_ContactName NVARCHAR(150) = NULL,
	@Representative1_MobileNo VARCHAR(11) = NULL,
	@Representative1_Email VARCHAR(500) = NULL,
	@Representative2_ContactName NVARCHAR(150) = NULL,
	@Representative2_MobileNo VARCHAR(11) = NULL,
	@Representative2_Email VARCHAR(500) = NULL,
	@holdId VARCHAR(20) = NULL

AS
DECLARE @Sno VARCHAR(10),
        @Sno2 VARCHAR(10),
        @Sno3 VARCHAR(10),
        @Sno4 VARCHAR(10),
		@Sno5 VARCHAR(10),
		@TransactionName VARCHAR(200),
        @ErrorDesc VARCHAR(MAX),
        @RandomPassword VARCHAR(20),
        @RoleId BIGINT,
        @RoleType VARCHAR(10),
        @SmsEmailResponseCode INT = 1;
DECLARE @SQLString NVARCHAR(MAX) = N'',
        @FetchQuery NVARCHAR(MAX),
        @SQLFilterParameter NVARCHAR(MAX) = N'',
		@holdtype VARCHAR(20) = NULL,
		 @holdId1 bigint,
		 @holdId2 bigint
BEGIN TRY
IF ISNULL(@Flag, '') = 'gclist' --get club list
    BEGIN
        SELECT ROW_NUMBER() OVER (ORDER BY a.ClubName1 ASC) AS SNO,
               b.LoginId,
               a.AgentId,
               a.ClubName1 AS ClubNameEng,
               a.ClubName2 AS ClubNameJap,
               a.MobileNumber,
               ISNULL(c.LocationName, '-') AS Location,
               FORMAT(a.ActionDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS CreatedDate,
               FORMAT(a.ActionDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS UpdatedDate,
               --FORMAT(a.ActionDate, 'dd MMM, yyyy hh:mm:ss') AS CreatedDate,
               --FORMAT(a.ActionDate, 'dd MMM, yyyy hh:mm:ss') AS UpdatedDate,
               ct.Tag2RankDescription AS Rank,
               '5' AS Ratings,
               a.Status,
               a.Sno,
               a.CompanyName,
               a.Logo AS ClubLogo,
               ISNULL(e.StaticDataLabel, '-') AS ClubCategory,
               COUNT(a.AgentId) OVER () AS TotalRecords,
			   (select holdStatus from tbl_club_details as cd WITH (NOLOCK) 
                  inner join tbl_club_details_hold cdh WITH (NOLOCK) on cdh.AgentId=cd.AgentId where cd.AgentId=a.AgentId and holdStatus='p')holdStatus
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 4
                   AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = a.LocationId
                   AND ISNULL(c.Status, '') = 'A'
            LEFT JOIN dbo.tbl_tag_detail d WITH (NOLOCK)
                ON d.ClubId = a.AgentId
                   AND ISNULL(d.Tag3Status, '') = 'A'
            LEFT JOIN dbo.tbl_static_data e WITH (NOLOCK)
                ON e.StaticDataType = 17
                   AND e.StaticDataValue = d.Tag3CategoryName
                   AND ISNULL(e.Status, '') = 'A'
            LEFT JOIN dbo.tbl_tag_detail ct WITH (NOLOCK)
                ON a.AgentId = ct.ClubId
        WHERE ISNULL(a.Status, '') IN ( 'A' )
              AND
              (
                  @SearchFilter IS NULL
                  OR
                  (
                      a.ClubName1 LIKE '%' + @SearchFilter + '%'
                      OR a.MobileNumber LIKE '%' + @SearchFilter + '%'
                      OR a.Email LIKE '%' + @SearchFilter + '%'
                  )
              )
        ORDER BY a.ClubName1 ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
    END;

ELSE IF ISNULL(@Flag, '') = 'gcplist' --get club pending list admin
    BEGIN
        select 
		  COUNT(a.holdId) OVER () AS TotalRecords,
          a.holdId, 
		  a.AgentId,
          a.Logo AS ClubLogo,
          a.MobileNumber,
          a.ClubName1 AS ClubNameEng,
          a.ClubName2 AS ClubNameJap,
          c.LocationName,
          FORMAT(a.ActionDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS CreatedDate,
          FORMAT(a.updatedDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS UpdatedDate,
		   a.ActionPlatform
           from tbl_club_details_hold a WITH (NOLOCK)
           LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = a.LocationId 
				where a.holdstatus='P'
              AND
              (
                  @SearchFilter IS NULL
                  OR
                  (
                      a.ClubName1 LIKE '%' + @SearchFilter + '%'
                      OR a.MobileNumber LIKE '%' + @SearchFilter + '%'
                      --OR a.Email LIKE '%' + @SearchFilter + '%'
                  )
              )
        ORDER BY a.ClubName1 ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
    END;
	ELSE IF ISNULL(@Flag, '') = 'gcrlist' --get club rejected list admin
    BEGIN
         select 
		  COUNT(a.holdId) OVER () AS TotalRecords,
          a.holdId, 
          a.Logo AS ClubLogo,
          a.MobileNumber,
          a.ClubName1 AS ClubNameEng,
          a.ClubName2 AS ClubNameJap,
          c.LocationName,
          FORMAT(a.ActionDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS CreatedDate,
          FORMAT(a.updatedDate, N'yyyy年MM月dd日 HH:mm:ss', 'ja-JP') AS UpdatedDate
           from tbl_club_details_hold a WITH (NOLOCK)
           LEFT JOIN dbo.tbl_location c WITH (NOLOCK)
                ON c.LocationId = a.LocationId 
				where a.holdstatus='R'
              AND
              (
                  @SearchFilter IS NULL
                  OR
                  (
                      a.ClubName1 LIKE '%' + @SearchFilter + '%'
                      OR a.MobileNumber LIKE '%' + @SearchFilter + '%'
                      --OR a.Email LIKE '%' + @SearchFilter + '%'
                  )
              )
        ORDER BY a.ClubName1 ASC OFFSET @Skip ROWS FETCH NEXT @Take ROW ONLY;
        RETURN;
        RETURN;
    END;	
	ELSE IF ISNULL(@Flag, '') = 'r_ch'  ---register club hold table (register club ) 
    BEGIN
	PRINT 1
        IF ISNULL(@LoginId, '') = ''
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;
          if(isnull( @AgentId,'') = '')		 
		  begin
		  IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            WHERE b.LoginId = @LoginId 
                  AND ISNULL(a.Status, '') IN ( 'A', 'B' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;

		  END
		  ELSE
		  BEGIN
		  set @holdtype='U'
		  IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            WHERE b.LoginId = @LoginId  and a.AgentId<>@AgentId
                  AND ISNULL(a.Status, '') IN ( 'A', 'B' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;

	
		  END
		

		IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details_hold a WITH (NOLOCK)              
            WHERE a.LoginId = @LoginId 
                  AND ISNULL(a.holdstatus, '') IN ( 'P' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;


        SELECT @TransactionName = 'Flag_rch'
            
        SELECT @FirstName = FirstName,
               @LastName = LastName
        FROM dbo.SplitName(@ceoFullName);

		BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_club_details_hold
        (

            FirstName,
            MiddleName,
            LastName,
            LandLineNumber,
            Email,
            MobileNumber,
            ClubName1,
            ClubName2,
            BusinessType,
            GroupName,
            Description,
            LocationURL,
            Longitude,
            Latitude,
            Status,
            Logo,
            CoverPhoto,
            BusinessCertificate,
            Gallery,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            LocationId,
            CompanyName,
            CommissionId,
            Holiday,
            LastOrderTime,
            LastEntrySyokai,
            Tax,
            InputPrefecture,
            InputCity,
            InputStreet,
            InputHouseNo,
            RegularPrice,
            NominationFee,
            AccompanyingFee,
            OnSiteNominationFee,
            VariousDrinksFee,
            InputZip,
            ClubOpeningTime,
            ClubClosingTime,
			GroupNamekatakana,
			CompanyAddress,
			BusinessLicenseNumber,
			LicenseIssuedDate,
			ClosingDate,
			KYCDocument,
			holdType ,
			holdstatus,
			LoginId

        )
        VALUES
        (   N'' + @FirstName + '', N'' + @MiddleName + '', N'' + @LastName + '', @LandLineNumber, @Email,
            @MobileNumber, @ClubName1, @ClubName2, @BusinessType, @GroupName, @Description, @LocationURL, @Longitude,
            @Latitude, 'p', --pending
            @Logo, @CoverPhoto, @BusinessCertificate, @Gallery, @ActionUser, @ActionIP, @ActionPlatform, GETDATE(),
            @LocationId, @CompanyName, 1, @holiday, @LastOrderTime, @LastEntryTime, @Tax, @Prefecture, @City,
            @InputStreet, @BuildingRoomNo, @RegularFee, @DesignationFee, @CompanionFee, @ExtensionFee, @VariousDrinks,
            @PostalCode, @ClubOpeningTime, @ClubClosingTime,@GroupNamekatakana,
			@CompanyAddress,
			@BusinessLicenseNumber,
			@LicenseIssuedDate,
			@ClosingDate,
			@KYCDocument,
			isnull(@holdtype,'I'),
			'P',
			@LoginId
			);

        SET @Sno = SCOPE_IDENTITY();


	     SELECT TOP 1
               @RoleId = a.Id,
               @RoleType = a.RoleType
        FROM dbo.tbl_roles a WITH (NOLOCK)
        WHERE a.RoleName = 'Club'
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY 1 ASC;

		If @BusinessType='1'
		begin
		 INSERT INTO dbo.tbl_Club_Representative_hold
        (
           clubholdId,
		    ClubId,
            ContactName,
            MobileNumber,
            EmailAddress,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
			holdtype,
			holdstatus
        )
        SELECT 
        @Sno,@AgentId, @Representative1_ContactName, @Representative1_MobileNo, @Representative1_Email, @ActionUser, @ActionIP, @ActionPlatform,
         GETDATE(),isnull(@holdtype,'I'),
			'P'
         UNION ALL
         SELECT 
         @Sno,@AgentId, @Representative2_ContactName, @Representative2_MobileNo, @Representative2_Email, @ActionUser, @ActionIP, @ActionPlatform,
         GETDATE(),isnull(@holdtype,'I'),
			'P'
       END
        INSERT INTO dbo.tbl_website_details_hold
        (
           clubholdId,
            WebsiteLink,
            TiktokLink,
            TwitterLink,
            InstagramLink,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleId,
            Line,
			holdtype,
			holdstatus
        )
        VALUES
        (@Sno, @WebsiteLink, @TiktokLink, @TwitterLink, @InstagramLink, @ActionUser, @ActionIP, @ActionPlatform,
			
         GETDATE(), @RoleType, @Line ,isnull(@holdtype,'I'),
			'P');

		 
        COMMIT TRANSACTION @TransactionName;
              
        SELECT 0 Code,
               'Account creation successful.' Message,
               @Sno Extra1;

        RETURN;
    END;
	ELSE IF ISNULL(@Flag, '') = 'r_cph' --register club plan hold 
    BEGIN
	 If @ClubId is not null AND @Id is not null
       begin
	   --set @status=(select status from tbl_club_details where agentId=@AgentId)
	   set @holdtype='U'
	   end
	   If @AgentId is not null 
       begin
	   set @holdtype='U'
       end
     
        IF ISNULL(@Id, '') = ''
        BEGIN
            --IF ISNULL(@ClubId, '') = ''
            --BEGIN
            --    SELECT 1 Code,
            --           'Club hold details is missing' Message;
            --    RETURN;
            --END;
            INSERT INTO [dbo].[tbl_club_plan_hold]
            (
                ClubholdId,
				ClubId,
                [ClubPlanType],
                [ClubPlanTypeId],
                [Description],
                [PlanListId],
                [CreatedDate],
                [CreatedBy],
                [CreatedPlatform],
				holdtype,
				holdstatus
            )
            VALUES
            (@ClubId,@AgentId, 38, @ClubPlanTypeId, @Description, @PlanListId, GETDATE(), @ActionUser, @ActionPlatform ,isnull(@holdtype,'I'),
			'P');

        END;
        ELSE
        BEGIN
            IF NOT EXISTS (SELECT 'x' FROM tbl_club_plan_hold WITH (NOLOCK) WHERE holdId = @Id)
            BEGIN
                SELECT 1 Code,
                       'update data not found.' Message;
                RETURN;
            END;

            UPDATE tbl_club_plan_hold
            SET
			[ClubholdId] = ISNULL(@ClubId, ClubholdId),
			[ClubPlanTypeId] = ISNULL(@ClubPlanTypeId, ClubPlanTypeId),
                [Description] = ISNULL(@Description, Description),
                UpdatedBy = @ActionUser,
                UpdatedDate = GETDATE(),
                UpdatedPlatform = @ActionPlatform,
				holdtype=isnull(@holdtype,'I'),
				holdStatus='p'
            WHERE holdId = @Id;
        END;


    END;
	ELSE IF ISNULL(@Flag, '') = 'g_chpd' --get club hold pending details 
    BEGIN
	If @holdId is not  null 
	BEGIN
	IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details_hold a WITH (NOLOCK)                
                LEFT JOIN dbo.tbl_website_details_hold c WITH (NOLOCK)
                    ON a.holdId = c.clubholdId                     
            WHERE ISNULL(a.holdstatus, '') IN ( 'P' )
                  AND a.holdId = @holdId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club hold details' Message;
            RETURN;
        END;
	END
     ELSE
	 BEGIN
	  SELECT 1 Code,
                   'Invalid club hold details' Message;
            RETURN;
        END;
		
		   CREATE  TABLE #temp_table1 (ClubholdId bigint, ContactName Nvarchar(100), MobileNumber varchar(12), EmailAddress varchar(500))
             INSERT INTO #temp_table1 (ClubholdId, ContactName, MobileNumber, EmailAddress)
             SELECT top 1 ClubholdId, ContactName, MobileNumber, EmailAddress 
             FROM tbl_Club_Representative_hold  WITH (NOLOCK)
             WHERE ClubholdId = @holdId order by holdId asc;

			CREATE  TABLE #temp_table2 (ClubholdId bigint, ContactName Nvarchar(100), MobileNumber varchar(12), EmailAddress varchar(500))
             INSERT  INTO #temp_table2 (ClubholdId, ContactName, MobileNumber, EmailAddress)
             SELECT top 1 ClubholdId, ContactName, MobileNumber, EmailAddress 
             FROM tbl_Club_Representative_hold  WITH (NOLOCK)
             WHERE ClubholdId = @holdId order by holdId desc;;

	
        SELECT 0 Code,
               'Success' Message,
			   a.holdId Id,
               a.AgentId,
               a.FirstName,
               a.MiddleName,
               a.LastName,
               CONCAT_WS(' ', ISNULL(a.FirstName, ''), ISNULL(a.LastName, '')) AS ceoFullName,
               a.Email,
               a.MobileNumber,
               a.ClubName1,
               a.ClubName2,
               a.BusinessType,
               a.GroupName,
               a.Description,
               a.LocationURL,
               a.Longitude,
               a.Latitude,
               a.Status,
               a.Logo,
               a.CoverPhoto,
               a.BusinessCertificate,
               a.Gallery,
               a.ActionUser,
               a.ActionIP,
               a.ActionPlatform,
               a.ActionDate,              
               a.Status,
               c.WebsiteLink,
               c.TiktokLink,
               c.TwitterLink,
               c.InstagramLink,
               c.Line,
               a.LocationId,
               a.CompanyName,
               a.LocationURL,
               a.Holiday,
               a.LastOrderTime,
               a.LastEntrySyokai,
               a.Tax,
               a.InputPrefecture,
               a.InputCity,
               a.InputStreet,
               a.InputHouseNo,
               a.RegularPrice,
               a.NominationFee,
               a.AccompanyingFee,
               a.OnSiteNominationFee,
               a.VariousDrinksFee,
               a.InputZip,
               a.ClubOpeningTime,
               a.ClubClosingTime,
               a.BusinessType,
               a.LandLineNumber,
			   a.GroupNamekatakana,
			   a.CompanyAddress,
			   a.BusinessLicenseNumber,
			   a.LicenseIssuedDate,
			   a.ClosingDate,
			   a.KYCDocument,
			   t1.ContactName AS Representative1_ContactName,
               t1.MobileNumber AS Representative1_MobileNo,
               t1.EmailAddress AS Representative1_Email,
               t2.ContactName AS Representative2_ContactName,
               t2.MobileNumber AS Representative2_MobileNo,
               t2.EmailAddress AS Representative2_Email,
			   a.LoginId
        FROM dbo.tbl_club_details_hold a WITH (NOLOCK)  
		    --INNER JOIN dbo.tbl_users b WITH (NOLOCK)
      --          ON b.AgentId = a.AgentId
      --             AND b.RoleType = 4
      --             AND ISNULL(b.Status, '') IN ( 'A', 'B' )
        LEFT JOIN dbo.tbl_website_details_hold c WITH (NOLOCK)
                    ON a.holdId = c.clubholdId 
	   LEFT JOIN 
               #temp_table1 t1 ON a.holdId = t1.ClubholdId 
      LEFT JOIN 
             #temp_table2 t2 ON a.holdId = t2.ClubholdId		
        WHERE ISNULL(a.holdStatus, '') IN ( 'P' )
              AND a.holdId = @holdId
DROP TABLE #temp_table1;
DROP TABLE #temp_table2;
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'g_cphpd' --get club plan hold pending  details
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'x'
            FROM dbo.tbl_club_details_hold AS ad WITH (NOLOCK)
                INNER JOIN tbl_club_plan_hold AS cp WITH (NOLOCK)
                    ON cp.ClubholdId = ad.holdId
                INNER JOIN dbo.tbl_static_data_type AS SDT WITH (NOLOCK)
                    ON SDT.StaticDataType = cp.ClubPlanType
                INNER JOIN dbo.tbl_static_data AS sd WITH (NOLOCK)
                    ON sd.StaticDataType = CAST(SDT.StaticDataType AS VARCHAR(50))
                       AND sd.StaticDataValue = cp.ClubPlanTypeId
            WHERE cp.ClubholdId = @holdId and ad.holdstatus='p'
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SELECT 0 Code,
               cp.holdId Id,
               ClubId,
               ClubPlanType,
               ClubPlanTypeId StaticDataValue,
               cp.Description,
               PlanListId,
               sd.StaticDataLabel AS English,
               sd.AdditionalValue1 AS japanese,
               CASE
                   WHEN cp.ClubPlanTypeId = '1' THEN
                       'Dropdown'
                   WHEN cp.ClubPlanTypeId = '4' THEN
                       'TextBox'
                   ELSE
                       'Time'
               END AS inputtype,
               REPLACE(sd.StaticDataLabel, ' ', '') name
        FROM dbo.tbl_club_details_hold AS ad WITH (NOLOCK)
            INNER JOIN tbl_club_plan_hold AS cp WITH (NOLOCK)
               ON cp.ClubholdId = ad.holdId
            INNER JOIN dbo.tbl_static_data_type AS SDT
                ON SDT.StaticDataType = cp.ClubPlanType
            INNER JOIN dbo.tbl_static_data AS sd WITH (NOLOCK)
                ON sd.StaticDataType = CAST(SDT.StaticDataType AS VARCHAR(50))
                   AND sd.StaticDataValue = cp.ClubPlanTypeId
        WHERE cp.ClubholdId = @holdId and ad.holdstatus='p'
        ORDER BY PlanListId,
                 sd.StaticDataValue ASC;
    END;
	ELSE IF ISNULL(@Flag, '') = 'm_ch' --update(manage) club hold(1st insert case))
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details_hold a WITH (NOLOCK)
            WHERE a.holdId = @holdId			
                  AND ISNULL(a.holdStatus, '') IN ( 'P' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid club hold details' Message;
            RETURN;
        END;

        SET @TransactionName = 'Flag_mch';

        BEGIN TRANSACTION @TransactionName;

        --IF EXISTS
        --(
        --    SELECT 1
        --    FROM dbo.tbl_club_details_hold a WITH (NOLOCK)
        --    WHERE a.AgentId = @AgentId
        --          AND ISNULL(a.Status, '') IN ( 'A' )
        --          AND
        --          (
        --              a.ClubName1 <> @ClubName1
        --              OR a.ClubName2 <> @ClubName2
        --          )
        --)
        --BEGIN
        --    EXEC dbo.sproc_clubmgt_customer_noficiation_alert @Flag = '1',
        --                                                      @ClubId = @AgentId;
       
        --END;

        UPDATE dbo.tbl_club_details_hold
        SET FirstName = ISNULL(@FirstName, FirstName),
            MiddleName = ISNULL(@MiddleName, MiddleName),
            LastName = ISNULL(@LastName, LastName),
            ClubName1 = ISNULL(@ClubName1, ClubName1),
            ClubName2 = ISNULL(@ClubName2, ClubName2),
            BusinessType = ISNULL(@BusinessType, BusinessType),
            GroupName = ISNULL(@GroupName, GroupName),
            Description = ISNULL(@Description, Description),
            LocationURL = ISNULL(@LocationURL, LocationURL),
            Longitude = ISNULL(@Longitude, Longitude),
            Latitude = ISNULL(@Latitude, Latitude),
            Logo = ISNULL(@Logo, Logo),
            CoverPhoto = ISNULL(@CoverPhoto, CoverPhoto),
            BusinessCertificate = ISNULL(@BusinessCertificate, BusinessCertificate),
            Gallery = ISNULL(@Gallery, Gallery),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE(),
            LocationId = ISNULL(@LocationId, LocationId),
            CompanyName = ISNULL(@CompanyName, CompanyName),
            Holiday = ISNULL(@holiday, Holiday),
            LastOrderTime = ISNULL(@LastOrderTime, LastOrderTime),
            LastEntrySyokai = ISNULL(@LastEntryTime, LastEntrySyokai),
            Tax = ISNULL(@Tax, Tax),
            InputPrefecture = ISNULL(@Prefecture, InputPrefecture),
            InputCity = ISNULL(@City, InputCity),
            InputStreet = ISNULL(@InputStreet, InputStreet),
            InputHouseNo = ISNULL(@BuildingRoomNo, InputHouseNo),
            RegularPrice = ISNULL(@RegularFee, RegularPrice),
            NominationFee = ISNULL(@DesignationFee, NominationFee),
            AccompanyingFee = ISNULL(@CompanionFee, AccompanyingFee),
            OnSiteNominationFee = ISNULL(@ExtensionFee, OnSiteNominationFee),
            VariousDrinksFee = ISNULL(@VariousDrinks, VariousDrinksFee),
            InputZip = ISNULL(@PostalCode, InputZip),
            ClubOpeningTime = ISNULL(@ClubOpeningTime, ClubOpeningTime),
            ClubClosingTime = ISNULL(@ClubClosingTime, ClubClosingTime),
            LandLineNumber = ISNULL(@LandLineNumber, LandLineNumber),
			GroupNamekatakana=ISNULL(@GroupNamekatakana, GroupNamekatakana),
			  CompanyAddress=ISNULL(@CompanyAddress, CompanyAddress),
			   BusinessLicenseNumber=ISNULL(@BusinessLicenseNumber, BusinessLicenseNumber),
			  LicenseIssuedDate=ISNULL(@LicenseIssuedDate, LicenseIssuedDate),
			  ClosingDate=ISNULL(@ClosingDate, ClosingDate),
			  KYCDocument=ISNULL(@KYCDocument, KYCDocument),
			  updatedUser = @ActionUser,
             updatedIP = @ActionIP,
            updatedPlatform = @ActionPlatform,
            updatedDate = GETDATE()
        WHERE holdId = @holdId
              AND ISNULL(holdStatus, '') IN ( 'p' );

        UPDATE dbo.tbl_website_details_hold
        SET WebsiteLink = ISNULL(@WebsiteLink, WebsiteLink),
            TiktokLink = ISNULL(@TiktokLink, TiktokLink),
            TwitterLink = ISNULL(@TwitterLink, TwitterLink),
            InstagramLink = ISNULL(@InstagramLink, InstagramLink),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE(),
            Line = @Line
          WHERE clubholdId = @holdId
              AND RoleId = 4;
			

	      If (@BusinessType ='1')
	      BEGIN
		      IF  EXISTS
              (
                  SELECT top (1) 'X'
                  FROM dbo.tbl_Club_Representative_hold a WITH (NOLOCK)
                  WHERE a.ClubholdId = @holdId			                      
              )
              BEGIN
			       select @holdId1=holdId from  tbl_Club_Representative_hold WITH (NOLOCK) where ClubholdId=@holdId order by holdId asc
	               select @holdId2=holdId from  tbl_Club_Representative_hold WITH (NOLOCK) where ClubholdId=@holdId order by holdId desc
	                
		           update tbl_Club_Representative_hold
	               SET     contactname = ISNULL(@Representative1_ContactName,contactname),
                              MobileNumber = ISNULL(@Representative1_MobileNo, MobileNumber),
                              EmailAddress = ISNULL(@Representative1_Email, EmailAddress),holdType='U',holdStatus='P' 
	                where 
	                       ClubholdId=@holdId and holdId=@holdId1
		           
	                update tbl_Club_Representative_hold
	                SET contactname = ISNULL(@Representative2_ContactName,contactname),
                              MobileNumber = ISNULL(@Representative2_MobileNo, MobileNumber),
                              EmailAddress = ISNULL(@Representative2_Email, EmailAddress),holdType='U',holdStatus='P' 
	                where 
		                 ClubholdId=@holdId and holdId=@holdId2
              END;
			  ELSE
			  BEGIN
                     INSERT INTO dbo.tbl_Club_Representative_hold
                     (
                     clubholdId,
                     ClubId,
                     ContactName,
                     MobileNumber,
                     EmailAddress,
                     ActionUser,
                     ActionIP,
                     ActionPlatform,
                     ActionDate,
                     holdtype,
                     holdstatus
                     )
                     SELECT 
                     @holdId,@AgentId, @Representative1_ContactName, @Representative1_MobileNo, @Representative1_Email, @ActionUser, @ActionIP, @ActionPlatform,
                     GETDATE(),'U',
                     'P'
                     UNION ALL
                     SELECT 
                     @holdId,@AgentId, @Representative2_ContactName, @Representative2_MobileNo, @Representative2_Email, @ActionUser, @ActionIP, @ActionPlatform,
                     GETDATE(),'U',
                     'P'
			  END

		     
	           
	      END
		  ELSE
		  BEGIN
		     If EXISTS(select top (1) 'x' from  tbl_Club_Representative_hold WITH (NOLOCK) where ClubholdId=@holdId )
		     BEGIN
		     UPDATE  tbl_Club_Representative_hold SET Status='D' where ClubholdId=@holdId
		     END
		  END


		  UPDATE 
		      tbl_club_details_hold 
		  SET 
		     holdType='U',holdStatus='P' 
		 WHERE
		      holdId=@holdId


		  UPDATE 
		       tbl_website_details_hold 
		  SET   
		      holdType='U',holdStatus='P' 
		  WHERE 
		      clubholdId=@holdId


		 COMMIT TRANSACTION @TransactionName;

          SELECT 0 Code,
               'Club details updated successfully' Message,@holdId  Extra1

       
        RETURN;
    END;
	ELSE IF ISNULL(@Flag, '') = 'r_cha' --  register club hold approved by admin
    BEGIN
	--IF ISNULL(@LoginId, '') = ''
 --       BEGIN
 --           SELECT 1 Code,
 --                  'Duplicate username' Message;
 --           RETURN;
 --       END;
 --       IF EXISTS
 --       (
 --           SELECT 'X'
 --           FROM dbo.tbl_club_details a WITH (NOLOCK)
 --               INNER JOIN dbo.tbl_users b WITH (NOLOCK)
 --                   ON b.AgentId = a.AgentId
 --                      AND ISNULL(b.Status, '') IN ( 'A', 'B' )
 --           WHERE b.LoginId = @LoginId
 --                 AND ISNULL(a.Status, '') IN ( 'A', 'B' )
 --       )
 --       BEGIN
 --           SELECT 1 Code,
 --                  'Duplicate username' Message;
 --           RETURN;
 --       END;

       IF NOT EXISTS(    SELECT 'x'  
       FROM
	        tbl_club_details_hold cdh WITH (NOLOCK) 
	   LEFT JOIN 
	        tbl_website_details_hold as wdh WITH (NOLOCK)  on wdh.clubholdId=cdh.holdId
	   WHERE 
	        cdh.holdId=@holdId 
	       AND  cdh.holdstatus='p')
	   BEGIN
	    SELECT 1 Code,
              'Club detail not found' Message;
            RETURN;
	   END

	   


      SELECT 
	        @FirstName =cdh.FirstName, 
			@MiddleName=cdh.MiddleName, 
			@LastName=cdh.LastName ,
			@LandLineNumber=cdh.LandLineNumber,
			@Email=cdh.Email,
            @MobileNumber=cdh.MobileNumber,
			@ClubName1=cdh.ClubName1,
			@ClubName2=cdh.ClubName2,
			@BusinessType=cdh.BusinessType, 
			@GroupName=cdh.GroupName,
			@Description=cdh.Description,
			@LocationURL=cdh.LocationURL, 
			@Longitude=cdh.Longitude,
            @Latitude=cdh.Longitude, 
            @Logo=cdh.Logo, 
			@CoverPhoto=cdh.CoverPhoto, 
			@BusinessCertificate=cdh.BusinessCertificate,
			@Gallery=cdh.Gallery,
			@ActionUser=cdh.ActionUser,
			@ActionIP=cdh.ActionIP,
			@ActionPlatform=cdh.ActionPlatform,
            @LocationId=cdh.LocationId,
			@CompanyName=cdh.CompanyName, 
			@holiday=cdh.holiday,
			@LastOrderTime=cdh.LastOrderTime,
			@LastEntryTime=cdh.LastEntrySyokai, 
			@Tax=cdh.Tax, 
			@Prefecture=cdh.InputPrefecture, 
			@City=cdh.InputCity,
            @InputStreet=cdh.InputStreet, 
			@BuildingRoomNo=cdh.InputHouseNo, 
			@RegularFee=cdh.RegularPrice,
			@DesignationFee=cdh.NominationFee, 
			@CompanionFee=cdh.AccompanyingFee, 
			@ExtensionFee=cdh.OnSiteNominationFee, 
			@VariousDrinks=cdh.VariousDrinksFee,
            @PostalCode=cdh.InputZip, 
			@ClubOpeningTime=cdh.ClubOpeningTime, 
			@ClubClosingTime=cdh.ClubClosingTime,
			@GroupNamekatakana=cdh.GroupNamekatakana,
			@CompanyAddress=cdh.CompanyAddress,
			@BusinessLicenseNumber=cdh.BusinessLicenseNumber,
			@LicenseIssuedDate=cdh.LicenseIssuedDate,
			@ClosingDate=cdh.ClosingDate,
			@KYCDocument=cdh.KYCDocument,
			@WebsiteLink=wdh.WebsiteLink, 
			@TiktokLink=wdh.TiktokLink, 
			@TwitterLink=wdh.TwitterLink, 
			@InstagramLink=wdh.InstagramLink,
			@Line=wdh.InstagramLink,
			@LoginId=cdh.LoginId

       FROM
	        tbl_club_details_hold cdh WITH (NOLOCK) 
	   LEFT JOIN 
	        tbl_website_details_hold as wdh WITH (NOLOCK)  on wdh.clubholdId=cdh.holdId
	   WHERE 
	        cdh.holdId=@holdId 
	 AND  cdh.holdstatus='p' 
	  
        SELECT @TransactionName = 'Flag_rc_a',
               @RandomPassword = dbo.func_generate_random_no(10);

   BEGIN TRANSACTION @TransactionName;

	  If @AgentId is null
	  begin
       

        --SELECT @FirstName = FirstName,
        --       @LastName = LastName
        --FROM dbo.SplitName(@ceoFullName);
		
        INSERT INTO dbo.tbl_club_details
        (
            FirstName,
            MiddleName,
            LastName,
            LandLineNumber,
            Email,
            MobileNumber,
            ClubName1,
            ClubName2,
            BusinessType,
            GroupName,
            Description,
            LocationURL,
            Longitude,
            Latitude,
            Status,
            Logo,
            CoverPhoto,
            BusinessCertificate,
            Gallery,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            LocationId,
            CompanyName,
            CommissionId,
            Holiday,
            LastOrderTime,
            LastEntrySyokai,
            Tax,
            InputPrefecture,
            InputCity,
            InputStreet,
            InputHouseNo,
            RegularPrice,
            NominationFee,
            AccompanyingFee,
            OnSiteNominationFee,
            VariousDrinksFee,
            InputZip,
            ClubOpeningTime,
            ClubClosingTime,
			GroupNamekatakana,
			CompanyAddress,
			BusinessLicenseNumber,
			LicenseIssuedDate,
			ClosingDate,
			KYCDocument

        )
        VALUES
        (   N'' + @FirstName + '', N'' + @MiddleName + '', N'' + @LastName + '', @LandLineNumber, @Email,
            @MobileNumber, @ClubName1, @ClubName2, @BusinessType, @GroupName, @Description, @LocationURL, @Longitude,
            @Latitude, 'A', --Active
            @Logo, @CoverPhoto, @BusinessCertificate, @Gallery, @ActionUser, @ActionIP, @ActionPlatform, GETDATE(),
            @LocationId, @CompanyName, 1, @holiday, @LastOrderTime, @LastEntryTime, @Tax, @Prefecture, @City,
            @InputStreet, @BuildingRoomNo, @RegularFee, @DesignationFee, @CompanionFee, @ExtensionFee, @VariousDrinks,
            @PostalCode, @ClubOpeningTime, @ClubClosingTime,@GroupNamekatakana,
			@CompanyAddress,
			@BusinessLicenseNumber,
			@LicenseIssuedDate,
			@ClosingDate,
			@KYCDocument
			);
			
        SET @Sno = SCOPE_IDENTITY();

        UPDATE dbo.tbl_club_details
        SET AgentId = @Sno
        WHERE Sno = @Sno;

        SELECT TOP 1
               @RoleId = a.Id,
               @RoleType = a.RoleType
        FROM dbo.tbl_roles a WITH (NOLOCK)
        WHERE a.RoleName = 'Club'
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY 1 ASC;

        INSERT INTO dbo.tbl_users
        (
            RoleId,
            AgentId,
            LoginId,
            Password,
            Status,
            IsPrimary,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleType,
            IsPasswordForceful
        )
        VALUES
        (   @RoleId, @Sno, @LoginId, PWDENCRYPT(@RandomPassword), 'A', --Active
            'Y', @ActionUser, @ActionIP, @ActionPlatform, GETDATE(), @RoleType, 'Y');

        SET @Sno2 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_users
        SET UserId = @Sno2
        WHERE Sno = @Sno2
              AND AgentId = @Sno;

		IF EXISTS(Select * from tbl_website_details_hold  WHERE 
	        holdId=@holdId 
	        AND  holdstatus='p' )
        BEGIN

        INSERT INTO dbo.tbl_website_details
        (
            AgentId,
            WebsiteLink,
            TiktokLink,
            TwitterLink,
            InstagramLink,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleId,
            Line
        )
        VALUES
        (@Sno, @WebsiteLink, @TiktokLink, @TwitterLink, @InstagramLink, @ActionUser, @ActionIP, @ActionPlatform,
         GETDATE(), @RoleType, @Line);

      END
	  IF EXISTS(SELECT top (1)'x'  
                      FROM
	                       tbl_club_details_hold cdh WITH (NOLOCK) 
	                  INNER JOIN 
	                       tbl_club_plan_hold as cph WITH (NOLOCK)  on cph.ClubholdId=cdh.holdId	    
	                  WHERE 
	                       cdh.holdId=@holdId 	                 
					  AND  cdh.holdstatus='p'
	        )
        BEGIN

		 INSERT INTO [dbo].[tbl_club_plan] (
            [ClubId],
            [ClubPlanType],
            [ClubPlanTypeId],
            [Description],
            [PlanListId],
            [CreatedDate],
            [CreatedBy],
            [CreatedPlatform]
         
         )
         SELECT
             @Sno,
             [ClubPlanType],
             [ClubPlanTypeId],
             [Description],
             [PlanListId],
             [CreatedDate],
             [CreatedBy],
             [CreatedPlatform]
             
         FROM 
		     [dbo].[tbl_club_plan_hold] WITH (NOLOCK)    
	     WHERE 
	         clubholdId=@holdId 

		END

	    IF (@BusinessType='1')     --- for corporation business type   
	    BEGIN
		             IF  EXISTS
                     (
                         SELECT top (1) 'X'
                         FROM dbo.tbl_Club_Representative_hold a WITH (NOLOCK)
                         WHERE a.ClubholdId = @holdId	 and isnull(Status,'')<>'D'		                      
                     )
                        BEGIN  
						   
			              INSERT INTO [dbo].[tbl_Club_Representative]
		                (
                          [ClubId],
                          [ContactName],
                          [MobileNumber],
                          [EmailAddress],
                          [ActionIP],
                          [ActionUser],
                          [ActionPlatform],
                          [Status],
                          [ActionDate]
                         )
                          SELECT top(1)  @Sno,
                            [ContactName],
                            [MobileNumber],
                            [EmailAddress],
                            [ActionIP],
                            [ActionUser],
                            [ActionPlatform],
                            [Status],
                            [ActionDate]
                        FROM 
			                 [dbo].[tbl_Club_Representative_hold] WITH (NOLOCK)
		               WHERE 
	                        clubholdId=@holdId order by holdId asc
		             
					      SET @Sno4 = SCOPE_IDENTITY();

					  UPDATE  
                     	dbo.tbl_Club_Representative_hold  
                     SET 
		                ClubId=@Sno,holdstatus ='A',RepresentativeId=@Sno4
                     		    
                     WHERE
                        clubholdId=@holdId  


		              INSERT INTO [dbo].[tbl_Club_Representative]
		                (
                          [ClubId],
                          [ContactName],
                          [MobileNumber],
                          [EmailAddress],
                          [ActionIP],
                          [ActionUser],
                          [ActionPlatform],
                          [Status],
                          [ActionDate]
                         )
                          SELECT top(1)  @Sno,
                            [ContactName],
                            [MobileNumber],
                            [EmailAddress],
                            [ActionIP],
                            [ActionUser],
                            [ActionPlatform],
                            [Status],
                            [ActionDate]
                        FROM 
			                 [dbo].[tbl_Club_Representative_hold] WITH (NOLOCK)
		               WHERE 
	                        clubholdId=@holdId order by holdId desc

						SET @Sno4 = SCOPE_IDENTITY();
                     UPDATE  
                     	dbo.tbl_Club_Representative_hold  
                     SET 
		                ClubId=@Sno,holdstatus ='A',RepresentativeId=@Sno4
                     		    
                     WHERE
                        clubholdId=@holdId  
			        END

	      END
		  ELSE
		  BEGIN
		             IF  EXISTS
                     (
                         SELECT 'X'
                         FROM dbo.tbl_Club_Representative_hold a WITH (NOLOCK)
                         WHERE a.ClubholdId = @holdId	 		                      
                     )
                        BEGIN
						  UPDATE tbl_Club_Representative_hold  set Status='D' where ClubholdId=@holdId
						END

		  END

        INSERT INTO dbo.tbl_tag_detail
        (
            ClubId,
            ActionUser,
            ActionIP,
            ActionDate
        )
        VALUES
        (   @Sno,        -- ClubId - bigint
            @ActionUser, -- ActionUser - varchar(200)
            @ActionIP,   -- ActionIP - varchar(50)
            GETDATE()    -- ActionDate - datetime
            );
        SET @Sno3 = SCOPE_IDENTITY();

        UPDATE dbo.tbl_tag_detail
        SET TagId = @Sno3
        WHERE Sno = @Sno3;

	  -------start update hold table ------

		UPDATE  
		     dbo.tbl_club_details_hold  
		SET
		    status= 'A',
			AgentId =@Sno,
			holdstatus ='A'
			--holdtype ='I'
		WHERE
		    holdId=@holdId

		UPDATE  
		     dbo.tbl_club_plan_hold  
		SET ClubId=@Sno,holdstatus ='A'
		    
		WHERE
		    clubholdId=@holdId  

	    UPDATE  
		     dbo.tbl_website_details_hold  
		SET AgentId=@Sno,holdstatus ='A'
		    
		WHERE
		    clubholdId=@holdId  

      -------end update hold table ------

       -- COMMIT TRANSACTION @TransactionName;

        CREATE TABLE #temp_rch
        (
            Code INT
        );

        INSERT INTO #temp_rch
        (
            Code
        )
        EXEC dbo.sproc_club_email_sms_management @Flag = '1',
                                                 @LoginId = @LoginId,
                                                 @Password = @RandomPassword,
                                                 @EmailSendTo = @Email,
                                                 @Username = @ClubName1,
                                                 @AgentId = @Sno,
                                                 @UserId = @Sno2,
                                                 @ActionUser = @ActionUser,
                                                 @ActionIP = @ActionIP,
                                                 @ActionPlatform = @ActionPlatform,
                                                 @ResponseCode = @SmsEmailResponseCode OUTPUT;
        DROP TABLE #temp_rch;

        EXEC dbo.sproc_clubmgt_customer_noficiation_alert @Flag = '1',
                                                          @ClubId = @Sno;
      

        SELECT 0 Code,
               'Club approved successfully' Message,
               @Sno Extra1;

       End
	   
	    COMMIT TRANSACTION @TransactionName;
	    
	  RETURN;
	 
    END;
	ELSE IF ISNULL(@Flag, '') = 'gcd' --get club details main
    BEGIN
	
        IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 4
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
                LEFT JOIN dbo.tbl_website_details c WITH (NOLOCK)
                    ON c.AgentId = b.AgentId
                       AND c.RoleId = 4
            WHERE ISNULL(a.Status, '') IN ( 'A', 'B' )
                  AND a.AgentId = @AgentId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;
		
		 CREATE  TABLE #temp_table_3 (ClubId bigint, ContactName Nvarchar(100), MobileNumber varchar(12), EmailAddress varchar(500))
             INSERT INTO #temp_table_3 (ClubId, ContactName, MobileNumber, EmailAddress)
             SELECT top 1 ClubId, ContactName, MobileNumber, EmailAddress 
             FROM tbl_Club_Representative 
             WHERE ClubId = @AgentId order by RepresentativeId asc;

			CREATE  TABLE #temp_table_4 (ClubId bigint, ContactName Nvarchar(100), MobileNumber varchar(12), EmailAddress varchar(500))
             INSERT  INTO #temp_table_4 (ClubId, ContactName, MobileNumber, EmailAddress)
             SELECT top 1 ClubId, ContactName, MobileNumber, EmailAddress 
             FROM tbl_Club_Representative
             WHERE ClubId = @AgentId order by RepresentativeId desc;;
			 
        SELECT 0 Code,
               'Success' Message,
               a.AgentId,
               a.FirstName,
               a.MiddleName,
               a.LastName,
               CONCAT_WS(' ', ISNULL(a.FirstName, ''), ISNULL(a.LastName, '')) AS ceoFullName,
               a.Email,
               a.MobileNumber,
               a.ClubName1,
               a.ClubName2,
               a.BusinessType,
               a.GroupName,
               a.Description,
               a.LocationURL,
               a.Longitude,
               a.Latitude,
               a.Status,
               a.Logo,
               a.CoverPhoto,
               a.BusinessCertificate,
               a.Gallery,
               a.ActionUser,
               a.ActionIP,
               a.ActionPlatform,
               a.ActionDate,
               b.UserId,
               b.LoginId,
               b.Status,
               c.WebsiteLink,
               c.TiktokLink,
               c.TwitterLink,
               c.InstagramLink,
               c.Line,
               a.LocationId,
               a.CompanyName,
               a.LocationURL,
               a.Holiday,
               a.LastOrderTime,
               a.LastEntrySyokai,
               a.Tax,
               a.InputPrefecture,
               a.InputCity,
               a.InputStreet,
               a.InputHouseNo,
               a.RegularPrice,
               a.NominationFee,
               a.AccompanyingFee,
               a.OnSiteNominationFee,
               a.VariousDrinksFee,
               a.InputZip,
               a.ClubOpeningTime,
               a.ClubClosingTime,
               a.BusinessType,
               a.LandLineNumber,
			   a.GroupNamekatakana,
			   a.CompanyAddress,
			   a.BusinessLicenseNumber,
			   a.LicenseIssuedDate,
			   a.ClosingDate,
			   a.KYCDocument,
			   t1.ContactName AS Representative1_ContactName,
               t1.MobileNumber AS Representative1_MobileNo,
               t1.EmailAddress AS Representative1_Email,
               t2.ContactName AS Representative2_ContactName,
               t2.MobileNumber AS Representative2_MobileNo,
               t2.EmailAddress AS Representative2_Email
        FROM dbo.tbl_club_details a WITH (NOLOCK)
            INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                ON b.AgentId = a.AgentId
                   AND b.RoleType = 4
                   AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            LEFT JOIN dbo.tbl_website_details c WITH (NOLOCK)
                ON c.AgentId = b.AgentId
                   AND c.RoleId = 4
	   LEFT JOIN 
               #temp_table_3 t1 ON a.AgentId = t1.ClubId 
      LEFT JOIN 
             #temp_table_4 t2 ON a.AgentId = t2.ClubId		
        WHERE ISNULL(a.Status, '') IN ( 'A', 'B' )
              AND a.AgentId = @AgentId;

			  drop table #temp_table_3
			   drop table #temp_table_4
        RETURN;
    END;
    ELSE IF ISNULL(@Flag, '') = 'gcpd' --get club plan list details main
    BEGIN
        IF NOT EXISTS
        (
            SELECT 'x'
            FROM dbo.tbl_club_details AS ad WITH (NOLOCK)
                INNER JOIN tbl_club_plan AS cp WITH (NOLOCK)
                    ON cp.ClubId = ad.AgentId
                INNER JOIN dbo.tbl_static_data_type AS SDT WITH (NOLOCK)
                    ON SDT.StaticDataType = cp.ClubPlanType
                INNER JOIN dbo.tbl_static_data AS sd WITH (NOLOCK)
                    ON sd.StaticDataType = CAST(SDT.StaticDataType AS VARCHAR(50))
                       AND sd.StaticDataValue = cp.ClubPlanTypeId
            WHERE cp.ClubId = @AgentId
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        SELECT 0 Code,
               cp.Id,
               ClubId,
               ClubPlanType,
               ClubPlanTypeId StaticDataValue,
               cp.Description,
               PlanListId,
               sd.StaticDataLabel AS English,
               sd.AdditionalValue1 AS japanese,
               CASE
                   WHEN cp.ClubPlanTypeId = '1' THEN
                       'Dropdown'
                   WHEN cp.ClubPlanTypeId = '4' THEN
                       'TextBox'
                   ELSE
                       'Time'
               END AS inputtype,
               REPLACE(sd.StaticDataLabel, ' ', '') name
        FROM dbo.tbl_club_details AS ad WITH (NOLOCK)
            INNER JOIN tbl_club_plan AS cp WITH (NOLOCK)
                ON cp.ClubId = ad.AgentId
            INNER JOIN dbo.tbl_static_data_type AS SDT
                ON SDT.StaticDataType = cp.ClubPlanType
            INNER JOIN dbo.tbl_static_data AS sd WITH (NOLOCK)
                ON sd.StaticDataType = CAST(SDT.StaticDataType AS VARCHAR(50))
                   AND sd.StaticDataValue = cp.ClubPlanTypeId
        WHERE cp.ClubId = @AgentId
        ORDER BY PlanListId,
                 sd.StaticDataValue ASC;
    END;
	ELSE IF ISNULL(@Flag, '') = 'r_cmth' --edit club main table data and register to hold table (only after the approval data  )
	BEGIN

	   set @status=(select status from tbl_club_details WITH (NOLOCK) where agentId=@AgentId)
	   set @holdtype='U'
	   IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND ISNULL(b.Status, '') IN ( 'A', 'B' )
            WHERE b.LoginId = @LoginId and a.AgentId<>@AgentId
                  AND ISNULL(a.Status, '') IN ( 'A', 'B' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;
		IF EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details_hold a WITH (NOLOCK)              
            WHERE a.LoginId = @LoginId and a.AgentId<>@AgentId
                  AND ISNULL(a.holdstatus, '') IN ( 'P' )
        )
        BEGIN
            SELECT 1 Code,
                   'Duplicate username' Message;
            RETURN;
        END;



		  SELECT @FirstName = FirstName,
          @LastName = LastName
        FROM dbo.SplitName(@ceoFullName);
		 SELECT @TransactionName = 'r_cmth'
		BEGIN TRANSACTION @TransactionName;

        INSERT INTO dbo.tbl_club_details_hold
        (
		AgentId,
            FirstName,
            MiddleName,
            LastName,
            LandLineNumber,
            Email,
            MobileNumber,
            ClubName1,
            ClubName2,
            BusinessType,
            GroupName,
            Description,
            LocationURL,
            Longitude,
            Latitude,
            Status,
            Logo,
            CoverPhoto,
            BusinessCertificate,
            Gallery,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            LocationId,
            CompanyName,
            CommissionId,
            Holiday,
            LastOrderTime,
            LastEntrySyokai,
            Tax,
            InputPrefecture,
            InputCity,
            InputStreet,
            InputHouseNo,
            RegularPrice,
            NominationFee,
            AccompanyingFee,
            OnSiteNominationFee,
            VariousDrinksFee,
            InputZip,
            ClubOpeningTime,
            ClubClosingTime,
			GroupNamekatakana,
			CompanyAddress,
			BusinessLicenseNumber,
			LicenseIssuedDate,
			ClosingDate,
			KYCDocument,
			holdType ,
			holdstatus,
			LoginId

        )
        VALUES
        (  @AgentId, N'' + @FirstName + '', N'' + @MiddleName + '', N'' + @LastName + '', @LandLineNumber, @Email,
            @MobileNumber, @ClubName1, @ClubName2, @BusinessType, @GroupName, @Description, @LocationURL, @Longitude,
            @Latitude, ISNULL(@status,'p'), --pending
            @Logo, @CoverPhoto, @BusinessCertificate, @Gallery, @ActionUser, @ActionIP, @ActionPlatform, GETDATE(),
            @LocationId, @CompanyName, 1, @holiday, @LastOrderTime, @LastEntryTime, @Tax, @Prefecture, @City,
            @InputStreet, @BuildingRoomNo, @RegularFee, @DesignationFee, @CompanionFee, @ExtensionFee, @VariousDrinks,
            @PostalCode, @ClubOpeningTime, @ClubClosingTime,@GroupNamekatakana,
			@CompanyAddress,
			@BusinessLicenseNumber,
			@LicenseIssuedDate,
			@ClosingDate,
			@KYCDocument,
			'U',
			'P',
			@LoginId
			);

        SET @Sno = SCOPE_IDENTITY();


	     SELECT TOP 1
               @RoleId = a.Id,
               @RoleType = a.RoleType
        FROM dbo.tbl_roles a WITH (NOLOCK)
        WHERE a.RoleName = 'Club'
              AND ISNULL(a.Status, '') = 'A'
        ORDER BY 1 ASC;

		If @BusinessType='1'
		begin
		 INSERT INTO dbo.tbl_Club_Representative_hold
        (
           clubholdId,
		    ClubId,
            ContactName,
            MobileNumber,
            EmailAddress,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
			holdtype,
			holdstatus
        )
        SELECT 
         @Sno,@AgentId, @Representative1_ContactName, @Representative1_MobileNo, @Representative1_Email, @ActionUser, @ActionIP, @ActionPlatform,
         GETDATE(),isnull(@holdtype,'U'),
			'P'
         UNION ALL
         SELECT 
         @Sno,@AgentId, @Representative2_ContactName, @Representative2_MobileNo, @Representative2_Email, @ActionUser, @ActionIP, @ActionPlatform,
         GETDATE(),isnull(@holdtype,'U'),
			'P'
       END


        INSERT INTO dbo.tbl_website_details_hold
        (
		AgentId,
           clubholdId,
            WebsiteLink,
            TiktokLink,
            TwitterLink,
            InstagramLink,
            ActionUser,
            ActionIP,
            ActionPlatform,
            ActionDate,
            RoleId,
            Line,
			holdtype,
			holdstatus
        )
        VALUES
        (@AgentId,@Sno, @WebsiteLink, @TiktokLink, @TwitterLink, @InstagramLink, @ActionUser, @ActionIP, @ActionPlatform,
			
         GETDATE(), @RoleType, @Line ,'U',
			'P');

		 If @AgentId is not null
         begin
	     --update  tbl_club_details set Status='p'   where agentId=@AgentId
		 update  tbl_club_details_hold  set AgentId=@AgentId  where holdId=@Sno
		 update  tbl_website_details_hold set AgentId=@AgentId  where clubholdId=@Sno
         end
		 
		  SELECT 0 Code,
               'Club updated successfully' Message,
               @Sno Extra1;
      
        COMMIT TRANSACTION @TransactionName;



	END
	ELSE IF ISNULL(@Flag, '') = 'rcm_a' ---approve edit table data
	BEGIN
	  IF NOT EXISTS
        (
            SELECT 'X'
            FROM dbo.tbl_club_details a WITH (NOLOCK)
                INNER JOIN dbo.tbl_users b WITH (NOLOCK)
                    ON b.AgentId = a.AgentId
                       AND b.RoleType = 4
                       AND ISNULL(b.Status, '') IN ( 'A' )
            WHERE a.AgentId = @AgentId
                  AND ISNULL(a.Status, '') IN ( 'A' )
        )
        BEGIN
            SELECT 1 Code,
                   'Invalid details' Message;
            RETURN;
        END;

        --IF EXISTS
        --(
        --    SELECT 1
        --    FROM dbo.tbl_club_details a WITH (NOLOCK)
        --    WHERE a.AgentId = @AgentId
        --          AND ISNULL(a.Status, '') IN ( 'A' )
        --          AND
        --          (
        --              a.ClubName1 <> @ClubName1
        --              OR a.ClubName2 <> @ClubName2
        --          )
        --)
        --BEGIN
        --    EXEC dbo.sproc_clubmgt_customer_noficiation_alert @Flag = '1',
        --                                                      @ClubId = @AgentId;
      
        --END;

	    SELECT 
	        @FirstName =cdh.FirstName, 
			@MiddleName=cdh.MiddleName, 
			@LastName=cdh.LastName ,
			@LandLineNumber=cdh.LandLineNumber,
			@Email=cdh.Email,
            @MobileNumber=cdh.MobileNumber,
			@ClubName1=cdh.ClubName1,
			@ClubName2=cdh.ClubName2,
			@BusinessType=cdh.BusinessType, 
			@GroupName=cdh.GroupName,
			@Description=cdh.Description,
			@LocationURL=cdh.LocationURL, 
			@Longitude=cdh.Longitude,
            @Latitude=cdh.Longitude, 
            @Logo=cdh.Logo, 
			@CoverPhoto=cdh.CoverPhoto, 
			@BusinessCertificate=cdh.BusinessCertificate,
			@Gallery=cdh.Gallery,
			@ActionUser=cdh.ActionUser,
			@ActionIP=cdh.ActionIP,
			@ActionPlatform=cdh.ActionPlatform,
            @LocationId=cdh.LocationId,
			@CompanyName=cdh.CompanyName, 
			@holiday=cdh.holiday,
			@LastOrderTime=cdh.LastOrderTime,
			@LastEntryTime=cdh.LastEntrySyokai, 
			@Tax=cdh.Tax, 
			@Prefecture=cdh.InputPrefecture, 
			@City=cdh.InputCity,
            @InputStreet=cdh.InputStreet, 
			@BuildingRoomNo=cdh.InputHouseNo, 
			@RegularFee=cdh.RegularPrice,
			@DesignationFee=cdh.NominationFee, 
			@CompanionFee=cdh.AccompanyingFee, 
			@ExtensionFee=cdh.OnSiteNominationFee, 
			@VariousDrinks=cdh.VariousDrinksFee,
            @PostalCode=cdh.InputZip, 
			@ClubOpeningTime=cdh.ClubOpeningTime, 
			@ClubClosingTime=cdh.ClubClosingTime,
			@GroupNamekatakana=cdh.GroupNamekatakana,
			@CompanyAddress=cdh.CompanyAddress,
			@BusinessLicenseNumber=cdh.BusinessLicenseNumber,
			@LicenseIssuedDate=cdh.LicenseIssuedDate,
			@ClosingDate=cdh.ClosingDate,
			@KYCDocument=cdh.KYCDocument,
			@WebsiteLink=wdh.WebsiteLink, 
			@TiktokLink=wdh.TiktokLink, 
			@TwitterLink=wdh.TwitterLink, 
			@InstagramLink=wdh.InstagramLink,
			@Line=wdh.InstagramLink,
			@LoginId=cdh.LoginId

       FROM
	        tbl_club_details_hold cdh WITH (NOLOCK) 
	   LEFT JOIN 
	        tbl_website_details_hold as wdh WITH (NOLOCK)  on wdh.clubholdId=cdh.holdId
	   WHERE 
	        cdh.holdId=@holdId and cdh.AgentId=@AgentId and cdh.holdType='U'
	 AND  cdh.holdstatus='p' 
	  
        SELECT @TransactionName = 'Flag_rcm_a',
               @RandomPassword = dbo.func_generate_random_no(10);
     BEGIN TRANSACTION @TransactionName;
	   
	    

        UPDATE dbo.tbl_club_details
        SET FirstName = ISNULL(@FirstName, FirstName),
            MiddleName = ISNULL(@MiddleName, MiddleName),
            LastName = ISNULL(@LastName, LastName),
            ClubName1 = ISNULL(@ClubName1, ClubName1),
            ClubName2 = ISNULL(@ClubName2, ClubName2),
            BusinessType = ISNULL(@BusinessType, BusinessType),
            GroupName = ISNULL(@GroupName, GroupName),
            Description = ISNULL(@Description, Description),
            LocationURL = ISNULL(@LocationURL, LocationURL),
            Longitude = ISNULL(@Longitude, Longitude),
            Latitude = ISNULL(@Latitude, Latitude),
            Logo = ISNULL(@Logo, Logo),
            CoverPhoto = ISNULL(@CoverPhoto, CoverPhoto),
            BusinessCertificate = ISNULL(@BusinessCertificate, BusinessCertificate),
            Gallery = ISNULL(@Gallery, Gallery),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE(),
            LocationId = ISNULL(@LocationId, LocationId),
            CompanyName = ISNULL(@CompanyName, CompanyName),
            Holiday = ISNULL(@holiday, Holiday),
            LastOrderTime = ISNULL(@LastOrderTime, LastOrderTime),
            LastEntrySyokai = ISNULL(@LastEntryTime, LastEntrySyokai),
            Tax = ISNULL(@Tax, Tax),
            InputPrefecture = ISNULL(@Prefecture, InputPrefecture),
            InputCity = ISNULL(@City, InputCity),
            InputStreet = ISNULL(@InputStreet, InputStreet),
            InputHouseNo = ISNULL(@BuildingRoomNo, InputHouseNo),
            RegularPrice = ISNULL(@RegularFee, RegularPrice),
            NominationFee = ISNULL(@DesignationFee, NominationFee),
            AccompanyingFee = ISNULL(@CompanionFee, AccompanyingFee),
            OnSiteNominationFee = ISNULL(@ExtensionFee, OnSiteNominationFee),
            VariousDrinksFee = ISNULL(@VariousDrinks, VariousDrinksFee),
            InputZip = ISNULL(@PostalCode, InputZip),
            ClubOpeningTime = ISNULL(@ClubOpeningTime, ClubOpeningTime),
            ClubClosingTime = ISNULL(@ClubClosingTime, ClubClosingTime),
            LandLineNumber = ISNULL(@LandLineNumber, LandLineNumber),
			GroupNamekatakana=ISNULL(@GroupNamekatakana, GroupNamekatakana),
		    CompanyAddress=ISNULL(@CompanyAddress, CompanyAddress),
			BusinessLicenseNumber=ISNULL(@BusinessLicenseNumber, BusinessLicenseNumber),
            LicenseIssuedDate=ISNULL(@LicenseIssuedDate, LicenseIssuedDate),
            ClosingDate=ISNULL(@ClosingDate, ClosingDate),
            KYCDocument=ISNULL(@KYCDocument, KYCDocument)
           
        WHERE AgentId = @AgentId
              AND ISNULL(Status, '') IN ( 'A' );
	
        UPDATE dbo.tbl_website_details
        SET WebsiteLink = ISNULL(@WebsiteLink, ''),
            TiktokLink = ISNULL(@TiktokLink, ''),
            TwitterLink = ISNULL(@TwitterLink, ''),
            InstagramLink = ISNULL(@InstagramLink, ''),
            ActionUser = @ActionUser,
            ActionIP = @ActionIP,
            ActionPlatform = @ActionPlatform,
            ActionDate = GETDATE(),
            Line = @Line
        WHERE AgentId = @AgentId
              AND RoleId = 4;

		  IF (@BusinessType ='1')
	      BEGIN
		 		
				 IF  EXISTS
                     (
                         SELECT top (1) 'X'
                         FROM dbo.tbl_Club_Representative_hold a WITH (NOLOCK)
                         WHERE a.ClubholdId = @holdId	 
						       and ClubId=@AgentId 
						       and holdStatus='P' 		                      
                     )
                        BEGIN
						      IF  EXISTS
                              (
                                  SELECT top (1) 'X'
                                  FROM dbo.tbl_Club_Representative a WITH (NOLOCK)
                                  WHERE  ClubId=@AgentId and Status<>'D' 		                      
                              )
                               BEGIN
                                    declare @rep_id1 bigint
                                    ,@rep_id2 bigint
                                    select top(1) @holdId1=holdId ,@Representative1_ContactName=ContactName,@Representative1_MobileNo=MobileNumber,@Representative1_Email=EmailAddress from  tbl_Club_Representative_hold where ClubholdId=@holdId order by holdId asc
                                    select top(1) @holdId2=holdId ,@Representative2_ContactName=ContactName,@Representative2_MobileNo=MobileNumber,@Representative2_Email=EmailAddress from  tbl_Club_Representative_hold where ClubholdId=@holdId order by holdId desc
                                    If EXISTS(select top 1 'x' from  tbl_Club_Representative where ClubId=@AgentId  and status <>'D')---if already inserted then update for that business corporation
                                    begin
                                         select top(1) @rep_id1=RepresentativeId from  tbl_Club_Representative where ClubId=@AgentId  and status <>'D' order by RepresentativeId asc
                                         select  top(1) @rep_id2=RepresentativeId from  tbl_Club_Representative where ClubId=@AgentId  and status <>'D' order by RepresentativeId desc
                                         		    
                                         			
                                         update 
									          tbl_Club_Representative
                                         SET 
									          contactname = ISNULL(@Representative1_ContactName,contactname),
                                              MobileNumber = ISNULL(@Representative1_MobileNo, MobileNumber),
                                              EmailAddress = ISNULL(@Representative1_Email, EmailAddress)
                                         where 
                                              RepresentativeId=@rep_id1 and ClubId=@AgentId 
									     
									     
                                         update  
									          tbl_Club_Representative
                                         SET 
									          contactname = ISNULL(@Representative2_ContactName,contactname),
                                              MobileNumber = ISNULL(@Representative2_MobileNo, MobileNumber),
                                              EmailAddress = ISNULL(@Representative2_Email, EmailAddress)
                                         where 
									          RepresentativeId=@rep_id2 and ClubId=@AgentId
                                         
									     
									      UPDATE  
		                                       dbo.tbl_Club_Representative_hold  
		                                  SET 
									           holdstatus ='A'		,ClubId=@AgentId    
		                                  WHERE
		                                     clubholdId=@holdId   and ClubId=@AgentId

							         END
							   ENd
							   ELSE
							   BEGIN
							    select top(1) @holdId1=holdId  from  tbl_Club_Representative_hold where ClubholdId=@holdId order by holdId asc
                                    select top(1) @holdId2=holdId  from  tbl_Club_Representative_hold where ClubholdId=@holdId order by holdId desc

                                   INSERT INTO [dbo].[tbl_Club_Representative]
                                   (
                                   [ClubId],
                                   [ContactName],
                                   [MobileNumber],
                                   [EmailAddress],
                                   [ActionIP],
                                   [ActionUser],
                                   [ActionPlatform],
                                   [Status],
                                   [ActionDate]
                                   )
                                   SELECT top(1) 
								   @AgentId,
                                   [ContactName],
                                   [MobileNumber],
                                   [EmailAddress],
                                   [ActionIP],
                                   [ActionUser],
                                   [ActionPlatform],
                                   [Status],
                                   [ActionDate]
                                   FROM 
                                   [dbo].[tbl_Club_Representative_hold] WITH (NOLOCK)
                                   WHERE 
                                   clubholdId=@holdId and ClubId=@AgentId order by holdId asc
                                   		             
                                   SET @Sno4 = SCOPE_IDENTITY();
                                   
                                   UPDATE  
                                   dbo.tbl_Club_Representative_hold  
                                   SET 
                                   ClubId=@AgentId,holdstatus ='A',RepresentativeId=@Sno4
                                                        		    
                                   WHERE
                                   holdId=@holdId1 
                                   
                                   
                                   INSERT INTO [dbo].[tbl_Club_Representative]
                                   (
                                   [ClubId],
                                   [ContactName],
                                   [MobileNumber],
                                   [EmailAddress],
                                   [ActionIP],
                                   [ActionUser],
                                   [ActionPlatform],
                                   [Status],
                                   [ActionDate]
                                   )
                                   SELECT top(1)  @AgentId,
                                   [ContactName],
                                   [MobileNumber],
                                   [EmailAddress],
                                   [ActionIP],
                                   [ActionUser],
                                   [ActionPlatform],
                                   [Status],
                                   [ActionDate]
                                   FROM 
                                   [dbo].[tbl_Club_Representative_hold] WITH (NOLOCK)
                                   WHERE 
                                   clubholdId=@holdId and ClubId=@AgentId order by holdId desc
                                   
                                   SET @Sno5 = SCOPE_IDENTITY();
                                   UPDATE  
                                   dbo.tbl_Club_Representative_hold  
                                   SET 
                                       ClubId=@AgentId, holdstatus ='A',RepresentativeId=@Sno5
                                                        		    
                                   WHERE
                                   holdId=@holdId2
                                   
                                   END
						END

						       
		  END

		  ELSE
		  BEGIN
		            IF  EXISTS
                    (
                        SELECT top (1) 'X'
                        FROM dbo.tbl_Club_Representative a WITH (NOLOCK)
                        WHERE  ClubId=@AgentId and isnull(Status,'')<>'D'                     
                    )
				    BEGIN
				    
					     UPDATE  
                                dbo.tbl_Club_Representative 
                         SET 
                             Status ='D'                                              		    
                         WHERE
                             ClubId=@AgentId

							  UPDATE  
		                          dbo.tbl_Club_Representative_hold  
		                      SET 
							        Status ='D'      
		                      WHERE
		                         clubholdId=@holdId   and ClubId=@AgentId
                         
				    END
		  END


   



         UPDATE  
		     dbo.tbl_club_details_hold  
		SET		  			
			holdstatus ='A'
			--holdtype ='I'
		WHERE
		    holdId=@holdId

		--UPDATE  
		--     dbo.tbl_club_plan_hold  
		--SET holdstatus ='A'
		    
		--WHERE
		--    clubholdId=@holdId  

	    UPDATE  
		     dbo.tbl_website_details_hold  
		SET holdstatus ='A'
		    
		WHERE
		    clubholdId=@holdId  


			 SELECT 0 Code,
               'Club edited approved successfully' Message,
               @Sno Extra1;
     COMMIT TRANSACTION @TransactionName;
	    
	  RETURN;
END
ELSE IF (@Flag='r_cp_ma')
BEGIN
--edit case (2nd case) :it will insert new row with new hold id in clubplanhold
--approve(2nd case) : it will check main table if exists then update 
 --                    clubplan otherwise inser new row in clubplan
--EXEC sproc_club_management_approvalrejection
--@Flag  = 'r_cp_ma',
--    @ClubPlanTypeId  = '1',
--    @Id  = '',
--    @PlanListId  = '0',
--	@Description='7',
--	@ClubId='111',
--   @AgentId  = '355',
--@ActionUser='',
--@ActionUser=''

If EXISTS(select 'x' from tbl_club_plan where 
           ClubId=@AgentId and ClubPlanTypeId=@ClubPlanTypeId and PlanListId=@PlanListId)
 BEGIN
  
update tbl_club_plan set  Description=@Description ,UpdatedDate=GETDATE(),UpdatedIP=@ActionIP,UpdatedBy=@ActionUser where ClubId=@AgentId and ClubPlanTypeId=@ClubPlanTypeId and PlanListId=@PlanListId

END
--If @holdtype='U' and @AgentId is not null
--BEGIN

--update tbl_club_plan set  Description=@Description  where ClubId=@AgentId and ClubPlanTypeId=@ClubPlanTypeId and PlanListId=@PlanListId
--END
ELSE
BEGIN
   INSERT INTO [dbo].[tbl_club_plan]
            (

				ClubId,
                [ClubPlanType],
                [ClubPlanTypeId],
                [Description],
                [PlanListId],
                [CreatedDate],
                [CreatedBy],
                [CreatedPlatform]
            )
            VALUES
            (@AgentId, 38, @ClubPlanTypeId, @Description, @PlanListId, GETDATE(), @ActionUser, @ActionPlatform );

END

	    UPDATE  
		     dbo.tbl_club_plan_hold  
		SET holdstatus ='A',UpdatedDate=GETDATE(),UpdatedIP=@ActionIP,UpdatedBy=@ActionUser
		    
		WHERE
		    clubholdId=@ClubId and ClubId=@AgentId and ClubPlanTypeId=@ClubPlanTypeId and PlanListId=@PlanListId

	END
ELSE IF ISNULL(@Flag, '') = 'rc_r' -- club register rejected by admin
	BEGIN
	 IF NOT EXISTS(    SELECT 'x'  
       FROM
	        tbl_club_details_hold cdh WITH (NOLOCK) 	   
	   WHERE 
	        cdh.holdId=@holdId 
	   AND  cdh.holdStatus='p')
	   BEGIN
	    SELECT 1 Code,
              'Club detail not found' Message;
            RETURN;
	   END

	   SELECT @TransactionName = 'Flag_rc_r'

	   BEGIN TRANSACTION @TransactionName;

	   UPDATE  
		     dbo.tbl_club_details_hold  
		SET
		    status= 'R',holdType ='R',holdStatus ='R',updatedDate=GetDate()
			
		WHERE
		    holdId=@holdId

			COMMIT TRANSACTION @TransactionName;

       SELECT 0 Code,
               'Club rejected successfully' Message,
               @Sno Extra1;
       RETURN;
	END
	
ELSE
    BEGIN
        SELECT 1 Code,
               'Invalid function' Message;
        RETURN;
END;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION @TransactionName;

    SET @ErrorDesc = 'SQL error found: (' + ERROR_MESSAGE() + ')' + ' at ' + CAST(ERROR_LINE() AS VARCHAR);

    INSERT INTO dbo.tbl_error_log
    (
        ErrorDesc,
        ErrorScript,
        QueryString,
        ErrorCategory,
        ErrorSource,
        ActionDate
    )
    VALUES
    (@ErrorDesc, 'sproc_club_management(Flag: ' + ISNULL(@Flag, '') + ')', 'SQL', 'SQL', 'sproc_club_management',
     GETDATE());

    SELECT 1 Code,
           'ErrorId:' + CAST(SCOPE_IDENTITY() AS VARCHAR) Message;
    RETURN;
END CATCH;
