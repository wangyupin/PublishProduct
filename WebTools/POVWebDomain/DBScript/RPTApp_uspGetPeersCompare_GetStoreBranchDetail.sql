
/****** Object:  StoredProcedure [RPTApp].[uspGetPeersCompare_GetStoreBranchDetail]    Script Date: 2022/8/30 下午 07:37:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name：RPTApp.uspGetPeersCompare_GetStoreBranchDetail
-- Desc：SP 同業業績比較-店櫃各同業明細
-- Author：Rex
-- LastModifyDTM：2022/1/20 16:52
-- Modify History：
-- 1. Initial. Rex@2022/1/20 16:52

--*********************************************************
CREATE PROCEDURE [RPTApp].[uspGetPeersCompare_GetStoreBranchDetail]
(
	@SDT AS VARCHAR(8) = '',
	@EDT AS VARCHAR(8) = '',
	@CP_SDT AS VARCHAR(8) = '',
	@CP_EDT AS VARCHAR(8) = '',
	@SBID AS VARCHAR(30) = null,
    @EBID AS VARCHAR(30) = null,
	@STORE_ID AS NVARCHAR(20)='',
	@QRY_IP AS NVARCHAR(20)  = N'',
	@OFFSET int=1,
	@PAGE_NUMBER int=1,
	@PAGE_SIZE int=1,
	@TOTAL_ROWS int OUTPUT,
	@O_MSG AS NVARCHAR(100) = N'' OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;
	SET @O_MSG = 'OK'
	DECLARE @CTE_RESULT_TMP AS TABLE -- Save CTE reault
	(   
		branch_name VARCHAR(100), 
		branch_id VARCHAR(20), 
		performance INT,
		ranking INT,
		cp_performance INT,
		cp_ranking INT,
		growth_rate FLOAT
	);
	DECLARE @CP_Day_Range AS TABLE
	(   
		DAY_DATE VARCHAR(8)
	);

	DECLARE @Day_Range AS TABLE
	(   
		DAY_DATE VARCHAR(8)
	);

	INSERT INTO @Day_Range
	SELECT convert(CHAR(8) , DAY_DATE , 112) FROM  [RPTApp].[udfDateRange] (@SDT,@EDT) 
	OPTION (RECOMPILE, MAXRECURSION 0 );

	INSERT INTO @CP_Day_Range 
	SELECT convert(CHAR(8) , DAY_DATE , 112) FROM  [RPTApp].[udfDateRange] (@CP_SDT,@CP_EDT) 
	OPTION (RECOMPILE, MAXRECURSION 0 );

	DECLARE @_tblOtherProc RPTApp.udtStrStr
	/*
	IF(@SBID IS NULL AND @EBID IS NULL)
	BEGIN
		INSERT INTO @_tblOtherProc 
		SELECT alikeBrandID, alikeBrandName FROM AlikeBrand
	END
	ELSE
	BEGIN
		INSERT INTO @_tblOtherProc 
		SELECT  ClientID, ClientShort FROM OtherProc
		WHERE 1=1
		AND (@SBID IS null OR ClientID >=@SBID)
	    AND (@EBID IS null OR ClientID <=@EBID+'ZZ')
		GROUP BY  [ClientID] ,[ClientShort]
		OPTION (RECOMPILE)
	END
	*/

	IF(@SBID IS NULL AND @EBID IS NULL)
	BEGIN
		INSERT INTO @_tblOtherProc 
		SELECT  ClientID, ClientShort FROM OtherProc
		GROUP BY  [ClientID] ,[ClientShort]
		OPTION (RECOMPILE)
	END
	ELSE
	BEGIN
		INSERT INTO @_tblOtherProc 
		SELECT  ClientID, ClientShort FROM OtherProc
		WHERE 1=1
		AND (@SBID IS null OR ClientID >=@SBID)
	    AND (@EBID IS null OR ClientID <=@EBID+'ZZ')
		GROUP BY  [ClientID] ,[ClientShort]
		OPTION (RECOMPILE)
	END

	DECLARE @SimpleBranchAchieveTmp AS TABLE
	(
		store_id VARCHAR(20),
		branch_id VARCHAR(20),
		performance INT
	);
	DECLARE @CpSimpleBranchAchieveTmp AS TABLE
	(
		branch_id VARCHAR(20),
		cp_performance INT
	);

	INSERT INTO @SimpleBranchAchieveTmp
	SELECT @STORE_ID, S.BranchID,
	SUM(ISNULL(S.Amount,0))
	FROM SimpleBranchAchieve AS S 
	JOIN @Day_Range AS D ON S.OccDate = D.DAY_DATE
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	--JOIN AlikeBrand AS A ON S.BranchID = A.alikeBrandID
	WHERE S.StoreID = @STORE_ID
	GROUP BY S.BranchID
	OPTION (RECOMPILE);

	INSERT INTO @CpSimpleBranchAchieveTmp
	SELECT S.BranchID,
	SUM(ISNULL(S.Amount,0))
	FROM SimpleBranchAchieve AS S 
	JOIN @CP_Day_Range AS D ON S.OccDate = D.DAY_DATE
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	WHERE S.StoreID = @STORE_ID
	--JOIN AlikeBrand AS A ON S.BranchID = A.alikeBrandID
	GROUP BY S.BranchID
	OPTION (RECOMPILE);

	INSERT INTO @CTE_RESULT_TMP
	SELECT O.ClientShort, 
	O.ClientID, 
	ISNULL(S.performance,0), 
	ROW_NUMBER() OVER (ORDER BY ISNULL(S.performance,0)  DESC, O.ClientID),
	ISNULL(CS.cp_performance,0),
	ROW_NUMBER() OVER (ORDER BY ISNULL(CS.cp_performance,0)  DESC),
	ISNULL(ROUND(   100  * CAST( (S.performance-CS.cp_performance) AS float)  /  NULLIF(CAST( CS.cp_performance  AS float),0) , 2),0)
	FROM @SimpleBranchAchieveTmp AS S
	FULL JOIN @CpSimpleBranchAchieveTmp  AS CS ON S.branch_id = CS.branch_id
	JOIN OtherProc AS O ON S.branch_id = O.ClientID OR CS.branch_id = O.ClientID
	WHERE (S.performance >0 OR CS.cp_performance >0) AND O.Reside = @STORE_ID
	OPTION (RECOMPILE);

	INSERT INTO @CTE_RESULT_TMP
	SELECT C.ClientShort, 
	'', 
	SUM(ISNULL(S.performance,0)), 
	0, 
	SUM(ISNULL(CS.cp_performance,0)), 
	0, 
	ISNULL(ROUND(   100  * CAST( (SUM(S.performance)-SUM(CS.cp_performance)) AS float)  /  NULLIF(CAST( SUM(CS.cp_performance)  AS float),0) , 2),0)
	FROM @SimpleBranchAchieveTmp AS S
	FULL JOIN @CpSimpleBranchAchieveTmp  AS CS ON S.branch_id = CS.branch_id
	JOIN Client AS C ON C.ClientID = @STORE_ID
	WHERE (S.performance >0 OR CS.cp_performance >0) --AND O.Reside = @STORE_ID
	GROUP BY C.ClientShort
	OPTION (RECOMPILE);

	SELECT @TOTAL_ROWS = COUNT(1) OVER() from @CTE_RESULT_TMP OPTION (RECOMPILE)
	SELECT * FROM @CTE_RESULT_TMP ORDER BY ranking  OPTION (RECOMPILE)
	SET NOCOUNT OFF;
END


GO

