
/****** Object:  StoredProcedure [RPTApp].[uspGetPeersCompare_GetStore]    Script Date: 2022/8/30 下午 07:36:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name：RPTApp.uspGetPeersCompare_GetStore
-- Desc：SP 同業業績比較-店櫃總計
-- Author：Rex
-- LastModifyDTM：2022/1/20 16:52
-- Modify History：
-- 1. Initial. Rex@2022/1/20 16:52

--*********************************************************
CREATE PROCEDURE [RPTApp].[uspGetPeersCompare_GetStore]
(
	@SDT AS VARCHAR(8) = '',
	@EDT AS VARCHAR(8) = '',
	@SBID AS VARCHAR(30) = null,
    @EBID AS VARCHAR(30) = null,
	@CP_SDT AS VARCHAR(8) = '',
	@CP_EDT AS VARCHAR(8) = '',
	--@tblOtherProc AS RPTApp.udtStrStr READONLY,
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
		store_id VARCHAR(20), 
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
	DECLARE @_tblOtherProc RPTApp.udtStrStr



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



	/*IF NOT EXISTS (SELECT * from @tblOtherProc)
	BEGIN
		INSERT INTO @_tblOtherProc 
		SELECT alikeBrandID, alikeBrandName FROM AlikeBrand
	END
	ELSE
	BEGIN
		INSERT INTO @_tblOtherProc 
		SELECT str1, str2 FROM @tblOtherProc
	END*/

	/*
	INSERT INTO @_tblOtherProc
	SELECT  [ClientID], [ClientShort] FROM [POV_DS].[dbo].[OtherProc]
	GROUP BY  [ClientID] ,[ClientShort]
	OPTION (RECOMPILE)*/
	
	INSERT INTO @Day_Range
	SELECT convert(CHAR(8) , DAY_DATE , 112) FROM  [RPTApp].[udfDateRange] (@SDT,@EDT) 
	OPTION (RECOMPILE, MAXRECURSION 0 );

	INSERT INTO @CP_Day_Range 
	SELECT convert(CHAR(8) , DAY_DATE , 112) FROM  [RPTApp].[udfDateRange] (@CP_SDT,@CP_EDT) 
	OPTION (RECOMPILE, MAXRECURSION 0 );

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
    --JOIN AlikeBrand AS A ON S.BranchID = A.alikeBrandID
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	--where S.BranchID BETWEEN '0002' AND '0281'
	GROUP BY S.StoreID
	OPTION (RECOMPILE);

	INSERT INTO @CpSimpleBranchAchieveTmp
	SELECT  S.StoreID,
	SUM(ISNULL(S.Amount,0))
	FROM SimpleBranchAchieve AS S 
	JOIN @CP_Day_Range AS D ON S.OccDate = D.DAY_DATE
    --JOIN AlikeBrand AS A ON S.BranchID = A.alikeBrandID
	JOIN @_tblOtherProc AS O ON S.BranchID = O.str1
	--where S.BranchID BETWEEN '0002' AND '0281'
	GROUP BY S.StoreID
	OPTION (RECOMPILE);

	INSERT INTO @CTE_RESULT_TMP
	SELECT C.ClientShort, 
	C.ClientID, 
	ISNULL(S.performance,0), 
	ROW_NUMBER() OVER (ORDER BY ISNULL(S.performance,0)  DESC, C.ClientID),
	ISNULL(CS.cp_performance,0),
	ROW_NUMBER() OVER (ORDER BY ISNULL(CS.cp_performance,0)  DESC),
	ISNULL(ROUND(   100  * CAST( (S.performance-CS.cp_performance) AS float)  /  NULLIF(CAST( CS.cp_performance  AS float),0) , 2),0)
	FROM @SimpleBranchAchieveTmp AS S
	FULL JOIN @CpSimpleBranchAchieveTmp  AS CS ON S.store_id = CS.store_id
	JOIN Client AS C ON S.store_id = C.ClientID OR CS.store_id = C.ClientID
	WHERE S.performance >0 OR CS.cp_performance >0
	OPTION (RECOMPILE);

	INSERT INTO @CTE_RESULT_TMP
	SELECT '總計', 
	'', 
	ISNULL(SUM(S.performance),0), 
	0, 
	ISNULL(SUM(CS.cp_performance),0), 
	0, 
	ISNULL(ROUND(   100  * CAST( (SUM(S.performance)-SUM(CS.cp_performance)) AS float)  /  NULLIF(CAST( SUM(CS.cp_performance)  AS float),0) , 2),0)
	FROM @SimpleBranchAchieveTmp AS S
	FULL JOIN @CpSimpleBranchAchieveTmp  AS CS ON S.store_id = CS.store_id
	WHERE S.performance >0 OR CS.cp_performance >0
	OPTION (RECOMPILE);

	SELECT @TOTAL_ROWS = COUNT(1) OVER() from @CTE_RESULT_TMP OPTION (RECOMPILE)
	SELECT * FROM @CTE_RESULT_TMP ORDER BY ranking  OPTION (RECOMPILE)
	SET NOCOUNT OFF;
END


GO

