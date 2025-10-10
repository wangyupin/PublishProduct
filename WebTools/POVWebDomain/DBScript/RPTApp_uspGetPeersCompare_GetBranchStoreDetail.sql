/****** Object:  StoredProcedure [RPTApp].[uspGetPeersCompare_GetBranchStoreDetail]    Script Date: 2022/8/30 下午 07:31:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name：RPTApp.uspGetPeersCompare_GetBranchStoreDetail
-- Desc：SP 同業業績比較-同業各店櫃明細
-- Author：Rex
-- LastModifyDTM：2022/1/20 16:52
-- Modify History：
-- 1. Initial. Rex@2022/1/20 16:52

--*********************************************************
CREATE PROCEDURE [RPTApp].[uspGetPeersCompare_GetBranchStoreDetail]
(
	@SDT AS VARCHAR(8) = '',
	@EDT AS VARCHAR(8) = '',
	@CP_SDT AS VARCHAR(8) = '',
	@CP_EDT AS VARCHAR(8) = '',
	@BRANCH_ID AS NVARCHAR(20)='',
	@SBID AS VARCHAR(30) = null,
    @EBID AS VARCHAR(30) = null,
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
		store_name VARCHAR(100), 
		performance INT,
		ranking INT,
		ranking_store INT,
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

	DECLARE @RANK_BRANCH_STORE AS TABLE--這個品牌在各店的名次
	(
		store_id VARCHAR(20),
		branch_id VARCHAR(20),
		ranking INT
		--performance INT
	);

	INSERT INTO @RANK_BRANCH_STORE
	SELECT  S.StoreID, S.BranchID, 
	ROW_NUMBER() OVER (PARTITION BY  S.StoreID ORDER BY SUM(ISNULL(S.Amount,0))  DESC)
	FROM SimpleBranchAchieve AS S 
	JOIN @Day_Range AS D ON S.OccDate = D.DAY_DATE
	JOIN Client AS C ON S.StoreID = C.ClientID
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	GROUP BY  S.StoreID,S.BranchID
	OPTION (RECOMPILE)

	DECLARE @SimpleBranchAchieveTmp AS TABLE
	(
		store_id VARCHAR(20),
		performance INT
	);
	DECLARE @CpSimpleBranchAchieveTmp AS TABLE
	(
		store_id VARCHAR(20),
		cp_performance INT
	);

	INSERT INTO @SimpleBranchAchieveTmp
	SELECT S.StoreID, 
	SUM(ISNULL(S.Amount,0))
	FROM SimpleBranchAchieve AS S 
	JOIN @Day_Range AS D ON S.OccDate = D.DAY_DATE
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	--JOIN AlikeBrand AS A ON S.BranchID = A.alikeBrandID
	WHERE S.BranchID = @BRANCH_ID
	GROUP BY S.StoreID
	OPTION (RECOMPILE);

	INSERT INTO @CpSimpleBranchAchieveTmp
	SELECT S.StoreID, 
	SUM(ISNULL(S.Amount,0))
	FROM SimpleBranchAchieve AS S 
	JOIN @CP_Day_Range AS D ON S.OccDate = D.DAY_DATE
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	--JOIN AlikeBrand AS A ON S.BranchID = A.alikeBrandID
	WHERE S.BranchID = @BRANCH_ID
	GROUP BY S.StoreID
	OPTION (RECOMPILE);

	INSERT INTO @CTE_RESULT_TMP
	SELECT C.ClientShort, 
	ISNULL(S.performance,0), 
	ROW_NUMBER() OVER (ORDER BY ISNULL(S.performance,0)  DESC, C.ClientID),
	R.ranking,
	ISNULL(CS.cp_performance,0),
	ROW_NUMBER() OVER (ORDER BY ISNULL(CS.cp_performance,0)  DESC),
	ISNULL(ROUND(   100  * CAST( (S.performance-CS.cp_performance) AS float)  /  NULLIF(CAST( CS.cp_performance  AS float),0) , 2),0)
	FROM @SimpleBranchAchieveTmp AS S
	FULL JOIN @CpSimpleBranchAchieveTmp  AS CS ON S.store_id = CS.store_id
	JOIN Client AS C ON S.store_id = C.ClientID OR CS.store_id = C.ClientID
	JOIN @RANK_BRANCH_STORE AS R ON (S.store_id = R.store_id AND   R.branch_id =@BRANCH_ID)
	WHERE (S.performance >0 OR CS.cp_performance >0) 
	OPTION (RECOMPILE);

	INSERT INTO @CTE_RESULT_TMP
	SELECT O.ClientShort, 
	SUM(ISNULL(S.performance,0)), 
	0, 
	0,
	SUM(ISNULL(CS.cp_performance,0)), 
	0, 
	ISNULL(ROUND(   100  * CAST( (SUM(S.performance)-SUM(CS.cp_performance)) AS float)  /  NULLIF(CAST( SUM(CS.cp_performance)  AS float),0) , 2),0)
	FROM @SimpleBranchAchieveTmp AS S
	FULL JOIN @CpSimpleBranchAchieveTmp  AS CS ON S.store_id = CS.store_id
	JOIN OtherProc AS O ON (O.ClientID = @BRANCH_ID AND O.Reside = S.store_id)
	WHERE (S.performance >0 OR CS.cp_performance >0) 
	GROUP BY  O.ClientShort
	OPTION (RECOMPILE);

	SELECT @TOTAL_ROWS = COUNT(1) OVER() from @CTE_RESULT_TMP OPTION (RECOMPILE)
	SELECT * FROM @CTE_RESULT_TMP ORDER BY ranking  OPTION (RECOMPILE)
	SET NOCOUNT OFF;
END


GO

