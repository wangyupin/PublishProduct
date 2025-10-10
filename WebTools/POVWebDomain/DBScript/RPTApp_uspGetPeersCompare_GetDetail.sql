
/****** Object:  StoredProcedure [RPTApp].[uspGetPeersCompare_GetDetail]    Script Date: 2022/8/30 下午 07:35:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name：RPTApp.uspGetPeersCompare_GetDetail
-- Desc：SP 同業業績比較
-- Author：Rex
-- LastModifyDTM：2021/11/1 19:49
-- Modify History：
-- 1. Initial. Rex@2021/11/1 19:49

--*********************************************************
CREATE PROCEDURE [RPTApp].[uspGetPeersCompare_GetDetail]
(
	@SDT AS VARCHAR(8) = '',
	@EDT AS VARCHAR(8) = '',
	@CP_SDT AS VARCHAR(8) = '',
	@CP_EDT AS VARCHAR(8) = '',
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
	    total_count INT,
		otherproc VARCHAR(100), 
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
	SELECT convert(CHAR(8) , DAY_DATE , 112) FROM  [RPTApp].[udfDateRange] (@SDT,@EDT) OPTION (RECOMPILE, MAXRECURSION 0 );

	INSERT INTO @CP_Day_Range 
	SELECT convert(CHAR(8) , DAY_DATE , 112) FROM  [RPTApp].[udfDateRange] (@CP_SDT,@CP_EDT) OPTION (RECOMPILE, MAXRECURSION 0 );

	DECLARE @ClientList AS TABLE
	(   
		client_id VARCHAR(15),
		client_short VARCHAR(80)
	);

	INSERT INTO @ClientList
	SELECT DISTINCT ClientID ,ClientShort FROM OtherProc OPTION (RECOMPILE);
	
	DECLARE @SellTmp AS TABLE
	(
		client_id VARCHAR(15),
		factamt INT
	);
	DECLARE @CpSellTmp AS TABLE
	(
		client_id VARCHAR(15),
		factamt INT
	);
	WITH SellFactAmt(cid, factamt) AS
	(
		SELECT S.BranchID ,SUM(S.Amount)
		FROM SimpleBranchAchieve AS S
		JOIN @Day_Range AS D ON S.OccDate = D.DAY_DATE
		GROUP BY S.BranchID
	)
	INSERT INTO @SellTmp 
	SELECT * FROM SellFactAmt OPTION (RECOMPILE);

	WITH SellFactAmt(cid, factamt) AS
	(
		SELECT S.BranchID ,SUM(S.Amount)
		FROM SimpleBranchAchieve AS S
		JOIN @CP_Day_Range AS D ON S.OccDate = D.DAY_DATE
		GROUP BY S.BranchID
	)
	INSERT INTO @CpSellTmp 
	SELECT * FROM SellFactAmt OPTION (RECOMPILE);

	WITH ProcessStore( otherproc, factamt, ranking,cp_factamt, cp_ranking) AS
	(
		SELECT C.client_short, ISNULL(S.factamt,0), ROW_NUMBER() OVER (ORDER BY  S.factamt DESC) , ISNULL(CP.factamt,0),  ROW_NUMBER() OVER (ORDER BY  CP.factamt  DESC)  FROM 
		@ClientList AS C LEFT JOIN @SellTmp AS S ON C.client_id = S.client_id
		LEFT JOIN @CpSellTmp AS CP ON C.client_id = CP.client_id
	)
	,
	SellFactAmtAll(factamt) AS
	(
		SELECT SUM(S.Amount)
		FROM SimpleBranchAchieve AS S
	    JOIN @Day_Range AS D ON S.OccDate= D.DAY_DATE
	)
	,
	CP_SellFactAmtAll(factamt) AS
	(
		SELECT SUM(S.Amount)
		FROM SimpleBranchAchieve AS S
	    JOIN @CP_Day_Range AS D ON S.OccDate = DAY_DATE
	)
	,
	TMP_CTE(otherproc, performance, ranking ,cp_performance, cp_ranking, growth_rate) AS
	(
		/*SELECT 
		P.otherproc,
		FORMAT(P.factamt,'N0') ,  
		P.ranking, 
		FORMAT(cp_factamt,'N0'), 
		P.cp_ranking,
		ISNULL(100 *ROUND(   CAST( (P.factamt-P.cp_factamt) AS float)  /  NULLIF(CAST( P.cp_factamt  AS float),0) , 2),0)
		FROM ProcessStore AS P
		UNION ALL

		SELECT
		'總計', 
		FORMAT(S.factamt,'N0'),
		0,
		FORMAT(CP.factamt,'N0'), 
		0,
		ISNULL(100 *ROUND(   CAST(S.factamt - CP.factamt AS float)  /  NULLIF(CAST( CP.factamt  AS float),0) , 2),0)
		FROM  SellFactAmtAll AS S, CP_SellFactAmtAll AS CP*/
		SELECT 
		P.otherproc,
		P.factamt ,  
		P.ranking, 
		cp_factamt, 
		P.cp_ranking,
		ISNULL(ROUND(   100  * CAST( (P.factamt-P.cp_factamt) AS float)  /  NULLIF(CAST( P.cp_factamt  AS float),0) , 2),0)
		FROM ProcessStore AS P
		UNION ALL

		SELECT
		'總計', 
		S.factamt,
		0,
		CP.factamt, 
		0,
		ISNULL(ROUND(   100 *CAST(S.factamt - CP.factamt AS float)  /  NULLIF(CAST( CP.factamt  AS float),0) , 2),0)
		FROM  SellFactAmtAll AS S, CP_SellFactAmtAll AS CP
	)
	INSERT INTO @CTE_RESULT_TMP
	SELECT totoal_count = COUNT(1) OVER(), TMP_CTE.*
	FROM TMP_CTE ORDER BY ranking
	OPTION (RECOMPILE,MAXRECURSION 0)

	SELECT @TOTAL_ROWS = (SELECT TOP(1) total_count from @CTE_RESULT_TMP)
	OPTION (RECOMPILE);

    SELECT otherproc, performance, ranking, cp_performance, cp_ranking, growth_rate FROM @CTE_RESULT_TMP ORDER BY ranking
	OPTION (RECOMPILE);

	SET NOCOUNT OFF;
END


GO

