USE [CRS]
GO
/****** Object:  StoredProcedure [dbo].[sproc_review_and_ratings_management]    Script Date: 12/13/2023 7:13:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Paras Maharjan>
-- Create date: <2023-10-16 15:23:56.940>
-- Description:	<review and ratings for superadmin and club>
-- =============================================
ALTER PROC [dbo].[sproc_review_and_ratings_management] @Flag VARCHAR(10)
	,@SearchText NVARCHAR(512) = NULL
	,@ClubId VARCHAR(10) = NULL
	,@ReviewId VARCHAR(8) = NULL
	,@ActionUser VARCHAR(150) = NULL
	,@ActionIp VARCHAR(200) = NULL
AS
BEGIN
DECLARE @DateFormat NVARCHAR(50) = 'MMM dd, yyyy HH:mm:ss';
	DECLARE @sqlQuery NVARCHAR(MAX);

	IF ISNULL(@Flag, '') = 'sra' --get rating lists
	BEGIN
		SET @sqlQuery = N'
			SELECT a.ReviewId
				,c.ProfileImage UserImage
				,c.NickName
				,c.FirstName FullName
				,a.ClubId AS ClubId
				,b.ClubName1 ClubNameEng
				,b.ClubName2 ClubNameJap
				,a.RatingScale Rating
				,FORMAT(a.ActionDate, ''' + @DateFormat + ''') AS RatedOn
			FROM dbo.tbl_review_and_rating a WITH (NOLOCK)
			LEFT JOIN dbo.tbl_club_details b WITH (NOLOCK) ON b.AgentId = a.ClubId
			AND b.Status=''A''
			LEFT JOIN dbo.tbl_customer c WITH (NOLOCK) ON a.CustomerId= c.AgentId
			WHERE 1=1';

		IF @SearchText IS NOT NULL
		BEGIN
			SET @sqlQuery = @sqlQuery + N' AND a.ClubName1 LIKE ''%' + @SearchText + '%''';
		END;

		IF @ClubId IS NOT NULL
		BEGIN
			SET @sqlQuery = @sqlQuery + N' AND a.@ClubId = ' + CAST(@ClubId AS VARCHAR);
		END;

		IF @ReviewId IS NOT NULL
		BEGIN
			SET @sqlQuery = @sqlQuery + N' AND a.ReviewId = ' + CAST(@ReviewId AS VARCHAR);
		END;

		SET @sqlQuery = @sqlQuery + N' AND ISNULL(a.[Status], '''') = ''A'';';

		PRINT @sqlQuery;

		EXEC sp_executesql @sqlQuery;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'sre' --get review lists
	BEGIN
		SET @sqlQuery = N'
			SELECT a.ReviewId
				,c.ProfileImage UserImage
				,c.NickName
				,c.FirstName FullName
				,a.ClubId AS ClubId
				,b.ClubName1 ClubNameEng
				,b.ClubName2 ClubNameJap
				,d.StaticDataLabel AS EnglishRemark
               ,d.AdditionalValue1 AS JapaneseRemark
               ,d.AdditionalValue2 AS RemarkType
				,a.ActionDate ReviewedOn
			FROM dbo.tbl_review_and_rating a WITH (NOLOCK)
			LEFT JOIN dbo.tbl_club_details b WITH (NOLOCK) ON b.AgentId = a.ClubId
			AND b.Status=''A''
			LEFT JOIN dbo.tbl_customer c WITH (NOLOCK) ON a.CustomerId= c.AgentId
			 LEFT JOIN dbo.tbl_static_data d WITH (NOLOCK)
                ON d.StaticDataType = 24
                   AND CHARINDEX(CONVERT(VARCHAR(100), d.StaticDataValue), a.RemarkList) > 0
                   AND ISNULL(d.Status, '''') = ''A''
			WHERE 1=1';

		IF @SearchText IS NOT NULL
		BEGIN
			SET @sqlQuery = @sqlQuery + N' AND a.ClubName1 LIKE ''%' + @SearchText + '%''';
		END;

		IF @ClubId IS NOT NULL
		BEGIN
			SET @sqlQuery = @sqlQuery + N' AND a.ClubId = ' + CAST(@ClubId AS VARCHAR);
		END;

		IF @ReviewId IS NOT NULL
		BEGIN
			SET @sqlQuery = @sqlQuery + N' AND a.ReviewId = ' + CAST(@ReviewId AS VARCHAR);
		END;

		SET @sqlQuery = @sqlQuery + N' AND ISNULL(a.[Status], '''') = ''A'';';

		PRINT @sqlQuery;

		EXEC sp_executesql @sqlQuery;

		RETURN;
	END;
	ELSE IF ISNULL(@Flag, '') = 'dra' -- delete reviews
	BEGIN
		IF @ReviewId IS NULL
		BEGIN
			SELECT '1' code
				,'Review id is required' message

			RETURN;
		END

		IF NOT EXISTS (
				SELECT 'x'
				FROM tbl_review_and_rating WITH (NOLOCK)
				WHERE ReviewId = @ReviewId
					AND STATUS = 'A'
				)
		BEGIN
			SELECT '1' code
				,'Unable to delete the rating' message

			RETURN;
		END

		UPDATE tbl_review_and_rating
		SET STATUS = 'B'
		WHERE ReviewId = @ReviewId

		SELECT '0' code
			,'Ratings deleted successfully' message

		RETURN;
	END
END;
