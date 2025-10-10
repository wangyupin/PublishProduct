--*********************************************************
-- Name：[POVWeb].[uspStoreQueryGetStockDetail]
-- Desc：撈取庫存明細
-- Author：MoonFeng
-- LastModifyDTM：2022/10/26 16:22
-- Modify History：
-- 1. Initial. MoonFeng@2022/10/18 16:22
-- 
-- SAMPLE：
/*
DECLARE	@return_value int,
		@O_MSG nvarchar(100)

EXEC	@return_value = [POVWeb].[uspStoreQueryGetStockDetail]
		@CalcDate = N'20221024',
		@USER_ID = N'gb',
		@CALC_METHOD = 1,
		@SQL_WHERE_GoodID = N'AND Goods.GoodID >= ''01001'' AND Goods.GoodID <= ''C1515''',
		@SQL_WHERE_Brand = N'AND Goods.Brand >= ''A'' AND Goods.Brand <= ''F''',
		@O_MSG = @O_MSG OUTPUT

SELECT	@O_MSG as N'@O_MSG'

SELECT	'Return Value' = @return_value
*/
/*********************************************************/

ALTER PROCEDURE [POVWeb].[uspStoreQueryGetStockDetail]
	(
	@CalcDate AS VARCHAR(8)='',
	@USER_ID AS VARCHAR(8),
	@CALC_METHOD AS TINYINT = 1, --1：依成本、2：依建議價、3:依特價、4依最後進價
	@SQL_WHERE_Brand AS NVARCHAR(max) = '',
	@SQL_WHERE_GoodID AS NVARCHAR(max) = '',
	@SQL_WHERE_OpenDate AS NVARCHAR(max) = '',
	@SQL_WHERE_Season AS NVARCHAR(max) = '',
	@SQL_WHERE_Cost AS NVARCHAR(max) = '',
	@SQL_WHERE_Sort01 AS NVARCHAR(max) = '',
	@SQL_WHERE_Sort02 AS NVARCHAR(max) = '',
	@SQL_WHERE_Sort03 AS NVARCHAR(max) = '',
	@SQL_WHERE_Sort04 AS NVARCHAR(max) = '',
	@SQL_WHERE_Sort05 AS NVARCHAR(max) = '',
	@SQL_WHERE_StorageTotalNum AS NVARCHAR(max) = '',
	@SQL_WHERE_SizeNo AS NVARCHAR(max) = '',

	@O_MSG AS NVARCHAR(100) = N'' OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

--Test Variablies*******************************************************
/*
##### POVSQL 查詢條件 #####
//GOODS 可查到的條件
	AND G.Season IN ('2021W','2021S','2020W','2020S','2019W','2019S') 
	AND G.OpenDate>='20150101' AND G.OpenDate<='20211231'  
	AND G.Sort01 IN ('0','1','2','3','4','5')  AND G.Sort02 IN ('E01','H01','L01')  
	AND G.Sort03 IN ('13') 
	AND IsNull(G.Cost, 0) >= 0 AND IsNull(G.Cost, 0) <= 1688

品牌區間 Brand：A ~ F 
型號區間 GoodID：01001 ~ C1515
季別區間 Season：2019S, 2019W, 2020S, 2020W, 2021S, 2021W
建檔日期 OpenDate：20150101 ~ 20211231
單價區間 Cost：0 ~ 1688

尺寸區間：XL, 尺寸數量：1~99
數量區間：-99999 ~ 99999
計算至當日庫存：20210531

依總計

類別1：0, 1, 2, 3, 4，5
類別2：A01, A02, A03
類別3：1, 11, 12, 13, 14
類別4：N/A
類別5：N/A

計算方式：依成本
*/
	--DECLARE @CalcDate AS VARCHAR(8) = '20210531'
	----DECLARE @CalcDate AS VARCHAR(8) = '20221021'
	--DECLARE @USER_ID AS VARCHAR(8) = 'gb'
	--DECLARE @CALC_METHOD AS TINYINT = 1 --1：依成本、2：依建議價、3:依特價、4依最後進價

	-- Goods where conditions 
	--DECLARE @SQL_WHERE_GoodID AS NVARCHAR(MAX) = 'AND Goods.GoodID >= ''01001'' AND Goods.GoodID <= ''C1515'''
	--DECLARE @SQL_WHERE_Brand AS NVARCHAR(MAX) = N'AND Goods.Brand >= ''A'' AND Goods.Brand <= ''F'''
	--DECLARE @SQL_WHERE_OpenDate AS NVARCHAR(MAX) = N'AND Goods.OpenDate >= ''20150101'' AND Goods.OpenDate <= ''20211231'''
	--DECLARE @SQL_WHERE_Season AS NVARCHAR(MAX) = N'AND Goods.Season IN (''2021W'',''2021S'',''2020W'',''2020S'',''2019W'',''2019S'')'
	--DECLARE @SQL_WHERE_Cost AS NVARCHAR(MAX) = N'AND Goods.Cost >= 1 AND Goods.Cost <= 1688'
	--DECLARE @SQL_WHERE_Sort01 AS NVARCHAR(MAX) = N'AND Goods.Sort01 IN (''0'',''1'',''2'',''3'',''4'',''5'')'
	--DECLARE @SQL_WHERE_Sort02 AS NVARCHAR(MAX) = N'AND Goods.Sort02 IN (''E01'',''H01'',''L01'')'
	--DECLARE @SQL_WHERE_Sort03 AS NVARCHAR(MAX) = N'AND Goods.Sort03 IN (''13'')'
	--DECLARE @SQL_WHERE_Sort04 AS NVARCHAR(MAX) = N''
	--DECLARE @SQL_WHERE_Sort05 AS NVARCHAR(MAX) = N''
	---- GoodStorage where conditions 
	--DECLARE @SQL_WHERE_StorageTotalNum AS NVARCHAR(MAX) = N'AND GS.StorageTotalNum>=-99999 AND GS.StorageTotalNum<=99999'
	--DECLARE @SQL_WHERE_SizeNo AS NVARCHAR(MAX) = N'And GS.SizeNo In (''1'') '

	--Initial Variablies*******************************************************
	IF (LEN(@CalcDate) <> 8) SET @CalcDate = convert(varchar, getdate(), 112)

	-- Goods where conditions 
	DECLARE @SQL_WHERE_Goods AS NVARCHAR(MAX) = N' ' 
		+ @SQL_WHERE_Brand 
		+ ' ' +  @SQL_WHERE_GoodID 
		+ ' ' + @SQL_WHERE_OpenDate
		+ ' ' + @SQL_WHERE_Season
		+ ' ' + @SQL_WHERE_Cost
		+ ' ' + @SQL_WHERE_Sort01
		+ ' ' + @SQL_WHERE_Sort02
		+ ' ' + @SQL_WHERE_Sort03
		+ ' ' + @SQL_WHERE_Sort04
		+ ' ' + @SQL_WHERE_Sort05
		+ ' '

	-- GoodStorage where conditions 
	DECLARE @SQL_WHERE_GoodStorage AS NVARCHAR(MAX) = N' '
		+ @SQL_WHERE_StorageTotalNum 
		+ ' ' + @SQL_WHERE_SizeNo 
		+ ' '

	DECLARE @SQL_STRING AS NVARCHAR(MAX) =''
	--DECLARE @TempTableID AS VARCHAR(16) = FORMAT (SYSDATETIME(), 'yyyyMMddHHmmssffff') 
	DECLARE @TempTableID AS VARCHAR(16) = FORMAT (SYSDATETIME(), 'HHmmssfffff') 
	DECLARE @TempTable_StoreTmpgb AS VARCHAR(64) = 'POVWeb_Temp_' + @TempTableID + '_StoreTmpgb'
	DECLARE @TempTable_Pov414TmpGB AS VARCHAR(64) = 'POVWeb_Temp_' + @TempTableID + '_Pov414TmpGB'
	DECLARE @TempTable_Pov401ResultgbTmp VARCHAR(64) = 'POVWeb_Temp_' + @TempTableID + '_Pov401ResultgbTmp'
	DECLARE @TempTable_Pov401MinDategbTmp AS VARCHAR(64) = 'POVWeb_Temp_' + @TempTableID + '_Pov401MinDategbTmp'

	DECLARE @TempTable_TargetGoods AS VARCHAR(64) = 'POVWeb_Temp_' + @TempTableID + '_TargetGoods'

	BEGIN TRY
		SET @SQL_STRING = N'
		SELECT * INTO ' + @TempTable_StoreTmpgb + ' From GoodStorage Where 1=0'
		EXEC sp_executesql @SQL_STRING

		--BIZ**********************************************************************

		-- 要將統計資料寫入暫存檔@TempTable_Pov401ResultgbTmp
		SET @SQL_STRING = N'
		CREATE TABLE ' + @TempTable_Pov401ResultgbTmp +' ( GoodID Varchar(25) Default '''', Store Varchar(15) Default '''', SizeNo Varchar(1) Default '''',
				StorageNum01 Float Default 0,StorageNum02 Float Default 0,StorageNum03 Float Default 0,StorageNum04 Float Default 0,StorageNum05 Float Default 0,
				StorageNum06 Float Default 0,StorageNum07 Float Default 0,StorageNum08 Float Default 0,StorageNum09 Float Default 0,StorageNum10 Float Default 0,
				StorageNum11 Float Default 0,StorageNum12 Float Default 0,StorageNum13 Float Default 0,StorageNum14 Float Default 0,StorageNum15 Float Default 0,
				StorageNum16 Float Default 0,StorageNum17 Float Default 0, StorageTotalNum Float Default 0,
				Size01 Varchar(10) Default '''',Size02 Varchar(10) Default '''',Size03 Varchar(10) Default '''',Size04 Varchar(10) Default '''',Size05 Varchar(10) Default '''',
				Size06 Varchar(10) Default '''',Size07 Varchar(10) Default '''',Size08 Varchar(10) Default '''',Size09 Varchar(10) Default '''',Size10 Varchar(10) Default '''',
				Size11 Varchar(10) Default '''',Size12 Varchar(10) Default '''',Size13 Varchar(10) Default '''',Size14 Varchar(10) Default '''',Size15 Varchar(10) Default '''',
				Size16 Varchar(10) Default '''',Size17 Varchar(10) Default '''',
				Cost Float Default 0, TotalMoney Float Default 0, TCost Float Default 0, TAdvicePrice Float Default 0, TGSpecialPrice Float Default 0, TCSpecialPrice Float Default 0,
				GoodName Varchar(120) Default '''', ClientShort Varchar(120) Default '''', Sort01 Varchar(10) Default '''', Brand Varchar(10) Default '''', Factory Varchar(10) Default '''',
				Season Varchar(10) Default '''', OpenDate Varchar(8) Default '''', Sort05Name Varchar(40) Default '''', GoodID10 Varchar(25) Default '''', StoreShelfID Varchar(5) Default '''',
				Position Varchar(2) Default '''', Discount Float Default 0, EndStockDate Varchar(8) Default '''', TPrice Float Default 0, ParentID Varchar(20) Default '''', TotalSellNum Float Default 0)'
		EXEC sp_executesql @SQL_STRING

		IF @CalcDate != convert(varchar, getdate(), 112) BEGIN -- 計算至當日庫存、如果沒有找到當月的資料,後面繼續用最後進價算
			--寫入目前庫存資訊
			--SET @SQL_STRING = N'
			--CREATE TABLE ' + @TempTable_Pov414TmpGB + ' ([GoodID] [varchar] (20) NULL ,[SizeNo] [varchar] (1) NULL ,[StoreID] [varchar] (20) NULL ,[Num01] [Float] NULL ,[Num02] [Float] NULL ,[Num03] [Float] NULL ,[Num04] [Float] NULL ,[Num05] [Float] NULL ,[Num06] [Float] NULL ,[Num07] [Float] NULL ,[Num08] [Float] NULL ,[Num09] [Float] NULL ,[Num10] [Float] NULL ,[Num11] [Float] NULL ,[Num12] [Float] NULL ,[Num13] [Float] NULL ,[Num14] [Float] NULL ,[Num15] [Float] NULL ,[Num16] [Float] NULL ,[Num17] [Float] NULL ,TotalNum [Float] NULL ,[IntoNum01] [Float] NULL ,[IntoNum02] [Float] NULL ,[IntoNum03] [Float] NULL ,[IntoNum04] [Float] NULL ,[IntoNum05] [Float] NULL ,[IntoNum06] [Float] NULL ,[IntoNum07] [Float] NULL ,[IntoNum08] [Float] NULL ,[IntoNum09] [Float] NULL ,[IntoNum10] [Float] NULL ,[IntoNum11] [Float] NULL ,[IntoNum12] [Float] NULL ,[IntoNum13] [Float] NULL ,[IntoNum14] [Float] NULL ,[IntoNum15] [Float] NULL ,[IntoNum16] [Float] NULL ,[IntoNum17] [Float] NULL ,TotalIntoNum [Float] NULL ,[UserID] [Varchar] (8) NULL ,[ShelfID] [Varchar] (20) NULL )
			--INSERT INTO ' + @TempTable_Pov414TmpGB +' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			--SELECT GS.GoodID, GS.SizeNo, GS.Store AS StoreID,
			--	   StorageNum01,StorageNum02,StorageNum03,StorageNum04,StorageNum05,StorageNum06,StorageNum07,StorageNum08,StorageNum09,StorageNum10,
			--	   StorageNum11,StorageNum12,StorageNum13,StorageNum14,StorageNum15,StorageNum16,StorageNum17,
			--	   StorageTotalNum
			--FROM GoodStorage AS GS Left Join Goods on GS.GoodID = Goods.GoodID
			--WHERE 1=1 '
			--+ @SQL_WHERE_Goods
			--EXEC sp_executesql @SQL_STRING

			SET @SQL_STRING = N'
			CREATE TABLE ' + @TempTable_Pov414TmpGB + ' ([GoodID] [varchar] (20) NULL ,[SizeNo] [varchar] (1) NULL ,[StoreID] [varchar] (20) NULL ,[Num01] [Float] NULL ,[Num02] [Float] NULL ,[Num03] [Float] NULL ,[Num04] [Float] NULL ,[Num05] [Float] NULL ,[Num06] [Float] NULL ,[Num07] [Float] NULL ,[Num08] [Float] NULL ,[Num09] [Float] NULL ,[Num10] [Float] NULL ,[Num11] [Float] NULL ,[Num12] [Float] NULL ,[Num13] [Float] NULL ,[Num14] [Float] NULL ,[Num15] [Float] NULL ,[Num16] [Float] NULL ,[Num17] [Float] NULL ,TotalNum [Float] NULL ,[IntoNum01] [Float] NULL ,[IntoNum02] [Float] NULL ,[IntoNum03] [Float] NULL ,[IntoNum04] [Float] NULL ,[IntoNum05] [Float] NULL ,[IntoNum06] [Float] NULL ,[IntoNum07] [Float] NULL ,[IntoNum08] [Float] NULL ,[IntoNum09] [Float] NULL ,[IntoNum10] [Float] NULL ,[IntoNum11] [Float] NULL ,[IntoNum12] [Float] NULL ,[IntoNum13] [Float] NULL ,[IntoNum14] [Float] NULL ,[IntoNum15] [Float] NULL ,[IntoNum16] [Float] NULL ,[IntoNum17] [Float] NULL ,TotalIntoNum [Float] NULL ,[UserID] [Varchar] (8) NULL ,[ShelfID] [Varchar] (20) NULL )
			INSERT INTO ' + @TempTable_Pov414TmpGB +' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT GS.GoodID, GS.SizeNo, GS.Store AS StoreID,
				   StorageNum01,StorageNum02,StorageNum03,StorageNum04,StorageNum05,StorageNum06,StorageNum07,StorageNum08,StorageNum09,StorageNum10,
				   StorageNum11,StorageNum12,StorageNum13,StorageNum14,StorageNum15,StorageNum16,StorageNum17,
				   StorageTotalNum
			FROM GoodStorage AS GS Left Join Goods on GS.GoodID = Goods.GoodID
			WHERE 1=1 '
			+ @SQL_WHERE_Goods
			EXEC sp_executesql @SQL_STRING

			--進貨明細
			--StockMode(異動代碼)：1進貨2退貨
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB +' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT StockDetail.GoodID AS GoodID,StockDetail.SizeNo AS SizeNo,StockDetail.StockStore AS StoreID,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum1 ELSE (-1)*StockDetail.StockNum1 END) AS Num01,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum2 ELSE (-1)*StockDetail.StockNum2 END) AS Num02,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum3 ELSE (-1)*StockDetail.StockNum3 END) AS Num03,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum4 ELSE (-1)*StockDetail.StockNum4 END) AS Num04,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum5 ELSE (-1)*StockDetail.StockNum5 END) AS Num05,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum6 ELSE (-1)*StockDetail.StockNum6 END) AS Num06,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum7 ELSE (-1)*StockDetail.StockNum7 END) AS Num07,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum8 ELSE (-1)*StockDetail.StockNum8 END) AS Num08,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum9 ELSE (-1)*StockDetail.StockNum9 END) AS Num09,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum10 ELSE (-1)*StockDetail.StockNum10 END) AS Num10,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum11 ELSE (-1)*StockDetail.StockNum11 END) AS Num11,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum12 ELSE (-1)*StockDetail.StockNum12 END) AS Num12,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum13 ELSE (-1)*StockDetail.StockNum13 END) AS Num13,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum14 ELSE (-1)*StockDetail.StockNum14 END) AS Num14,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum15 ELSE (-1)*StockDetail.StockNum15 END) AS Num15,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum16 ELSE (-1)*StockDetail.StockNum16 END) AS Num16,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockNum17 ELSE (-1)*StockDetail.StockNum17 END) AS Num17,
				   Sum(CASE WHEN StockDetail.StockMode=''2'' THEN StockDetail.StockTotalNum ELSE (-1)*StockDetail.StockTotalNum END) AS TotalNum
			  FROM StockDetail Left Join Goods on StockDetail.GoodID = Goods.GoodID
			 WHERE StockDetail.StockDate > @CalcDate '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY StockDetail.StockStore,StockDetail.GoodID,StockDetail.SizeNo'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			--用CTE改變統一GoodID名稱
			--D MEmpInStore.MaterialID>='01001-35' AND MEmpInStore.MaterialID<='01101-24'
			SET @SQL_STRING = N'
				WITH MEmpInStoreCTE (GoodID, SizeNo, StoreID, EmpInDate, EmpInMode, 
									EmpInNum01, EmpInNum02, EmpInNum03, EmpInNum04, EmpInNum05, EmpInNum06, EmpInNum07, EmpInNum08, EmpInNum09, EmpInNum10, 
									EmpInNum11, EmpInNum12, EmpInNum13, EmpInNum14, EmpInNum15, EmpInNum16, EmpInNum17, EmpInNumTotal) AS (
					SELECT MaterialID AS GoodID, SizeNo, StoreID, EmpInDate, EmpInMode,
						EmpInNum01, EmpInNum02, EmpInNum03, EmpInNum04, EmpInNum05, EmpInNum06, EmpInNum07, EmpInNum08, EmpInNum09, EmpInNum10, 
						EmpInNum11, EmpInNum12, EmpInNum13, EmpInNum14, EmpInNum15, EmpInNum16, EmpInNum17, EmpInNumTotal
					FROM MEmpInStore )
				INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
				SELECT MEmpInStoreCTE.GoodID, MEmpInStoreCTE.SizeNo, MEmpInStoreCTE.StoreID ,
						  (-1)*Sum(EmpInNum01) AS Num01,(-1)*Sum(EmpInNum02) AS Num02,(-1)*Sum(EmpInNum03) AS Num03,(-1)*Sum(EmpInNum04) AS Num04,(-1)*Sum(EmpInNum05) AS Num05,
						  (-1)*Sum(EmpInNum06) AS Num06,(-1)*Sum(EmpInNum07) AS Num07,(-1)*Sum(EmpInNum08) AS Num08,(-1)*Sum(EmpInNum09) AS Num09,(-1)*Sum(EmpInNum10) AS Num10,
						  (-1)*Sum(EmpInNum11) AS Num11,(-1)*Sum(EmpInNum12) AS Num12,(-1)*Sum(EmpInNum13) AS Num13,(-1)*Sum(EmpInNum14) AS Num14,(-1)*Sum(EmpInNum15) AS Num15,
						  (-1)*Sum(EmpInNum16) AS Num16,(-1)*Sum(EmpInNum17) AS Num17,(-1)*Sum(EmpInNumTotal) AS TotalNum
					 From MEmpInStoreCTE Left Join Goods on MEmpInStoreCTE.GoodID = Goods.GoodID
					WHERE EmpInDate>@CalcDate  AND EmpInMode=''1'' '
					+ @SQL_WHERE_Goods
					+ N' GROUP BY MEmpInStoreCTE.StoreID, MEmpInStoreCTE.GoodID, MEmpInStoreCTE.SizeNo'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			-- 出貨明細 => 退貨
			-- ShipMode(異動代號)：3出貨4退貨；DealMode(交易方式)：1買斷2寄賣
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT ShipDetail.GoodID AS GoodID,ShipDetail.SizeNo AS SizeNo,ShipDetail.ShipStore AS StoreID,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum1 ELSE ShipDetail.ShipNum1 END) AS Num01,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum2 ELSE ShipDetail.ShipNum2 END) AS Num02,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum3 ELSE ShipDetail.ShipNum3 END) AS Num03,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum4 ELSE ShipDetail.ShipNum4 END) AS Num04,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum5 ELSE ShipDetail.ShipNum5 END) AS Num05,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum6 ELSE ShipDetail.ShipNum6 END) AS Num06,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum7 ELSE ShipDetail.ShipNum7 END) AS Num07,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum8 ELSE ShipDetail.ShipNum8 END) AS Num08,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum9 ELSE ShipDetail.ShipNum9 END) AS Num09,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum10 ELSE ShipDetail.ShipNum10 END) AS Num10,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum11 ELSE ShipDetail.ShipNum11 END) AS Num11,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum12 ELSE ShipDetail.ShipNum12 END) AS Num12,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum13 ELSE ShipDetail.ShipNum13 END) AS Num13,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum14 ELSE ShipDetail.ShipNum14 END) AS Num14,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum15 ELSE ShipDetail.ShipNum15 END) AS Num15,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum16 ELSE ShipDetail.ShipNum16 END) AS Num16,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.ShipNum17 ELSE ShipDetail.ShipNum17 END) AS Num17,
				   SUM(CASE WHEN ShipDetail.ShipMode=''4'' THEN (-1)*ShipDetail.TotalShipNum ELSE ShipDetail.TotalShipNum END) AS TotalNum
			  FROM ShipDetail Left Join Goods on ShipDetail.GoodID = Goods.GoodID
			 WHERE ShipDetail.ShipDate>@CalcDate '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY ShipDetail.Shipstore,ShipDetail.GoodID,ShipDetail.SizeNo;'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			-- 出貨明細 => 寄賣出貨
			-- ShipMode(異動代號)：3出貨4退貨；DealMode(交易方式)：1買斷2寄賣
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT ShipDetail.GoodID AS GoodID,ShipDetail.SizeNo AS SizeNo,ShipDetail.CustID AS StoreID,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum1 ELSE ShipDetail.ShipNum1 END) AS Num01,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum2 ELSE ShipDetail.ShipNum2 END) AS Num02,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum3 ELSE ShipDetail.ShipNum3 END) AS Num03,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum4 ELSE ShipDetail.ShipNum4 END) AS Num04,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum5 ELSE ShipDetail.ShipNum5 END) AS Num05,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum6 ELSE ShipDetail.ShipNum6 END) AS Num06,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum7 ELSE ShipDetail.ShipNum7 END) AS Num07,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum8 ELSE ShipDetail.ShipNum8 END) AS Num08,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum9 ELSE ShipDetail.ShipNum9 END) AS Num09,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum10 ELSE ShipDetail.ShipNum10 END) AS Num10,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum11 ELSE ShipDetail.ShipNum11 END) AS Num11,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum12 ELSE ShipDetail.ShipNum12 END) AS Num12,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum13 ELSE ShipDetail.ShipNum13 END) AS Num13,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum14 ELSE ShipDetail.ShipNum14 END) AS Num14,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum15 ELSE ShipDetail.ShipNum15 END) AS Num15,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum16 ELSE ShipDetail.ShipNum16 END) AS Num16,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.ShipNum17 ELSE ShipDetail.ShipNum17 END) AS Num17,
				   SUM(CASE WHEN ShipDetail.ShipMode=''3'' THEN (-1)*ShipDetail.TotalShipNum ELSE ShipDetail.TotalShipNum END) AS TotalNum
			  FROM ShipDetail Left Join Goods on ShipDetail.GoodID = Goods.GoodID
			 WHERE DealMode=''2'' AND ShipDetail.ShipDate>@CalcDate '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY ShipDetail.CustID,ShipDetail.GoodID,ShipDetail.SizeNo;'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			--調轉表 - 調出作業(因group by TranOutStore)
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT Transfer.GoodID AS GoodID,Transfer.SizeNo AS SizeNo,Transfer.TranOutStore AS StoreID,
				   Sum(Transfer.TranNum1) AS Num01,Sum(Transfer.TranNum2) AS Num02,Sum(Transfer.TranNum3) AS Num03,Sum(Transfer.TranNum4) AS Num04,Sum(Transfer.TranNum5) AS Num05,
				   Sum(Transfer.TranNum6) AS Num06,Sum(Transfer.TranNum7) AS Num07,Sum(Transfer.TranNum8) AS Num08,Sum(Transfer.TranNum9) AS Num09,Sum(Transfer.TranNum10) AS Num10,
				   Sum(Transfer.TranNum11) AS Num11,Sum(Transfer.TranNum12) AS Num12,Sum(Transfer.TranNum13) AS Num13,Sum(Transfer.TranNum14) AS Num14,Sum(Transfer.TranNum15) AS Num15,
				   Sum(Transfer.TranNum16) AS Num16,Sum(Transfer.TranNum17) AS Num17,
				   Sum(Transfer.TotalTranNum) AS TotalNum
			  From Transfer Left Join Goods on Transfer.GoodID = Goods.GoodID
			 WHERE Transfer.TranDate>@CalcDate '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY Transfer.GoodID,Transfer.SizeNo,Transfer.TranOutStore;'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			--調轉表 - 調入作業((因group by TranInStore))
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT Transfer.GoodID AS GoodID,Transfer.SizeNo AS SizeNo,Transfer.TranInStore AS StoreID,
				   (-1)*Sum(ISNULL(Transfer.TranNum1,0)) AS Num01,(-1)*Sum(ISNULL(Transfer.TranNum2,0)) AS Num02,(-1)*Sum(ISNULL(Transfer.TranNum3,0)) AS Num03,(-1)*Sum(ISNULL(Transfer.TranNum4,0)) AS Num04,(-1)*Sum(ISNULL(Transfer.TranNum5,0)) AS Num05,
				   (-1)*Sum(ISNULL(Transfer.TranNum6,0)) AS Num06,(-1)*Sum(ISNULL(Transfer.TranNum7,0)) AS Num07,(-1)*Sum(ISNULL(Transfer.TranNum8,0)) AS Num08,(-1)*Sum(ISNULL(Transfer.TranNum9,0)) AS Num09,(-1)*Sum(ISNULL(Transfer.TranNum10,0)) AS Num10,
				   (-1)*Sum(ISNULL(Transfer.TranNum11,0)) AS Num11,(-1)*Sum(ISNULL(Transfer.TranNum12,0)) AS Num12,(-1)*Sum(ISNULL(Transfer.TranNum13,0)) AS Num13,(-1)*Sum(ISNULL(Transfer.TranNum14,0)) AS Num14,(-1)*Sum(ISNULL(Transfer.TranNum15,0)) AS Num15,
				   (-1)*Sum(ISNULL(Transfer.TranNum16,0)) AS Num16,(-1)*Sum(ISNULL(Transfer.TranNum17,0)) AS Num17,
				   (-1)*Sum(ISNULL(Transfer.TotalTranNum,0)) AS TotalNum
			  From Transfer Left Join Goods on Transfer.GoodID = Goods.GoodID
			 WHERE Transfer.TranDate>@CalcDate '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY Transfer.GoodID,Transfer.SizeNo,Transfer.TranInStore'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			--調整表
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT Modify.GoodID AS GoodID,Modify.SizeNo AS SizeNo,Modify.ModifyStore AS StoreID,
				   (-1)*Sum(ISNULL(Modify.DiffNum1,0)) AS Num01,(-1)*Sum(ISNULL(Modify.DiffNum2,0)) AS Num02,(-1)*Sum(ISNULL(Modify.DiffNum3,0)) AS Num03,(-1)*Sum(ISNULL(Modify.DiffNum4,0)) AS Num04,(-1)*Sum(ISNULL(Modify.DiffNum5,0)) AS Num05,
				   (-1)*Sum(ISNULL(Modify.DiffNum6,0)) AS Num06,(-1)*Sum(ISNULL(Modify.DiffNum7,0)) AS Num07,(-1)*Sum(ISNULL(Modify.DiffNum8,0)) AS Num08,(-1)*Sum(ISNULL(Modify.DiffNum9,0)) AS Num09,(-1)*Sum(ISNULL(Modify.DiffNum10,0)) AS Num10,
				   (-1)*Sum(ISNULL(Modify.DiffNum11,0)) AS Num11,(-1)*Sum(ISNULL(Modify.DiffNum12,0)) AS Num12,(-1)*Sum(ISNULL(Modify.DiffNum13,0)) AS Num13,(-1)*Sum(ISNULL(Modify.DiffNum14,0)) AS Num14,(-1)*Sum(ISNULL(Modify.DiffNum15,0)) AS Num15,
				   (-1)*Sum(ISNULL(Modify.DiffNum16,0)) AS Num16,(-1)*Sum(ISNULL(Modify.DiffNum17,0)) AS Num17,
				   (-1)*Sum(ISNULL(Modify.TotalDiffNum,0)) AS TotalNum
			  From Modify Left Join Goods on Modify.GoodID = Goods.GoodID
			 WHERE Modify.ModifyDate>@CalcDate '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY Modify.GoodID,Modify.SizeNo,Modify.ModifyStore;'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			--交易檔
			--SellMode(銷售別)：1銷貨2退貨3拍賣4員購5訂金6取貨7贈品8消訂
			--?? 'A', 'C'是什麼意思呢??
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT Sell.GoodID AS GoodID,Sell.SizeNo AS SizeNo,Sell.SellStore AS StoreID,
				   Sum(Sell.SellNum1) AS Num01,Sum(Sell.SellNum2) AS Num02,Sum(Sell.SellNum3) AS Num03,Sum(Sell.SellNum4) AS Num04,Sum(Sell.SellNum5) AS Num05,
				   Sum(Sell.SellNum6) AS Num06,Sum(Sell.SellNum7) AS Num07,Sum(Sell.SellNum8) AS Num08,Sum(Sell.SellNum9) AS Num09,Sum(Sell.SellNum10) AS Num10,
				   Sum(Sell.SellNum11) AS Num11,Sum(Sell.SellNum12) AS Num12,Sum(Sell.SellNum13) AS Num13,Sum(Sell.SellNum14) AS Num14,Sum(Sell.SellNum15) AS Num15,
				   Sum(Sell.SellNum16) AS Num16,Sum(Sell.SellNum17) AS Num17,
				   Sum(Sell.TotalSellNum) AS TotalNum
			  From Sell Left Join Goods on Sell.GoodID = Goods.GoodID
			 WHERE Sell.SellDate>@CalcDate AND Sell.SellMode NOT IN (''2'', ''5'', ''8'', ''A'', ''C'') '
			 + @SQL_WHERE_Goods
			 + ' GROUP BY Sell.GoodID,Sell.SizeNo,Sell.SellStore;'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			--交易檔
			--SellMode(銷售別)：1銷貨2退貨3拍賣4員購5訂金6取貨7贈品8消訂
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_Pov414TmpGB + ' (GoodID,SizeNo,StoreID,Num01,Num02,Num03,Num04,Num05,Num06,Num07,Num08,Num09,Num10,Num11,Num12,Num13,Num14,Num15,Num16,Num17,TotalNum) 
			SELECT Sell.GoodID AS GoodID,Sell.SizeNo AS SizeNo,Sell.SellStore AS StoreID,
				   (-1)*Sum(Sell.SellNum1) AS Num01,(-1)*Sum(Sell.SellNum2) AS Num02,(-1)*Sum(Sell.SellNum3) AS Num03,(-1)*Sum(Sell.SellNum4) AS Num04,(-1)*Sum(Sell.SellNum5) AS Num05,
				   (-1)*Sum(Sell.SellNum6) AS Num06,(-1)*Sum(Sell.SellNum7) AS Num07,(-1)*Sum(Sell.SellNum8) AS Num08,(-1)*Sum(Sell.SellNum9) AS Num09,(-1)*Sum(Sell.SellNum10) AS Num10,
				   (-1)*Sum(Sell.SellNum11) AS Num11,(-1)*Sum(Sell.SellNum12) AS Num12,(-1)*Sum(Sell.SellNum13) AS Num13,(-1)*Sum(Sell.SellNum14) AS Num14,(-1)*Sum(Sell.SellNum15) AS Num15,
				   (-1)*Sum(Sell.SellNum16) AS Num16,(-1)*Sum(Sell.SellNum17) AS Num17,
				   (-1)*Sum(Sell.TotalSellNum) AS TotalNum
			  From Sell Left Join Goods on Sell.GoodID = Goods.GoodID
			 WHERE Sell.SellDate>@CalcDate AND Sell.SellMode IN (''2'',''A'', ''C'') '
			 + @SQL_WHERE_Goods
			 + N' GROUP BY Sell.GoodID,Sell.SizeNo,Sell.SellStore;'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate
		
			--統計各異動檔
			SET @SQL_STRING = N'
			INSERT INTO ' + @TempTable_StoreTmpgb + '(GoodID,SizeNo,Store,StorageNum01,StorageNum02,StorageNum03,StorageNum04,StorageNum05,StorageNum06,StorageNum07,StorageNum08,StorageNum09,StorageNum10,StorageNum11,StorageNum12,StorageNum13,StorageNum14,StorageNum15,StorageNum16,StorageNum17,StorageTotalNum,StockNum01,StockNum02,StockNum03,StockNum04,StockNum05,StockNum06,StockNum07,StockNum08,StockNum09,StockNum10,StockNum11,StockNum12,StockNum13,StockNum14,StockNum15,StockNum16,StockNum17,StockTotalNum,StoreShelfID,changedate) 
			SELECT IsNull(A.GoodID,''''),IsNull(A.SizeNo,''''),IsNull(A.StoreID,''''),
				   Sum(ISNULL(A.Num01,0)) AS StorageNum01,Sum(ISNULL(A.Num02,0)) AS StorageNum02,Sum(ISNULL(A.Num03,0)) AS StorageNum03,Sum(ISNULL(A.Num04,0)) AS StorageNum04,Sum(ISNULL(A.Num05,0)) AS StorageNum05,
				   Sum(ISNULL(A.Num06,0)) AS StorageNum06,Sum(ISNULL(A.Num07,0)) AS StorageNum07,Sum(ISNULL(A.Num08,0)) AS StorageNum08,Sum(ISNULL(A.Num09,0)) AS StorageNum09,Sum(ISNULL(A.Num10,0)) AS StorageNum10,
				   Sum(ISNULL(A.Num11,0)) AS StorageNum11,Sum(ISNULL(A.Num12,0)) AS StorageNum12,Sum(ISNULL(A.Num13,0)) AS StorageNum13,Sum(ISNULL(A.Num14,0)) AS StorageNum14,Sum(ISNULL(A.Num15,0)) AS StorageNum15,
				   Sum(ISNULL(A.Num16,0)) AS StorageNum16,Sum(ISNULL(A.Num17,0)) AS StorageNum17,
				   Sum(ISNULL(A.TotalNum,0)) AS StorageTotalNum,
				   Sum(ISNULL(A.IntoNum01,0)) AS StockNum01,Sum(ISNULL(A.IntoNum02,0)) AS StockNum02,Sum(ISNULL(A.IntoNum03,0)) AS StockNum03,Sum(ISNULL(A.IntoNum04,0)) AS StockNum04,Sum(ISNULL(A.IntoNum05,0)) AS StockNum05,
				   Sum(ISNULL(A.IntoNum06,0)) AS StockNum06,Sum(ISNULL(A.IntoNum07,0)) AS StockNum07,Sum(ISNULL(A.IntoNum08,0)) AS StockNum08,Sum(ISNULL(A.IntoNum09,0)) AS StockNum09,Sum(ISNULL(A.IntoNum10,0)) AS StockNum10,
				   Sum(ISNULL(A.IntoNum11,0)) AS StockNum11,Sum(ISNULL(A.IntoNum12,0)) AS StockNum12,Sum(ISNULL(A.IntoNum13,0)) AS StockNum13,Sum(ISNULL(A.IntoNum14,0)) AS StockNum14,Sum(ISNULL(A.IntoNum15,0)) AS StockNum15,
				   Sum(ISNULL(A.IntoNum16,0)) AS StockNum16,Sum(ISNULL(A.IntoNum17,0)) AS StockNum17,
				   Sum(ISNULL(A.TotalIntoNum,0)) AS StockTotalNum,
				   IsNull(A.ShelfID,'''') AS StoreShelfID,@CalcDate as ChangeDate
			  FROM ' + @TempTable_Pov414TmpGB + ' AS A
			  WHERE A.GOODID IS NOT NULL
			GROUP BY A.GoodID,A.SizeNo,A.StoreID,A.ShelfID'
			EXEC sp_executesql @SQL_STRING, N'@CalcDate VARCHAR(8)', @CalcDate

			-- 過濾條件
			-- 因@TempTable_StoreTmpgb 已過濾Goods的條件，所以此處不用再過濾
			SET @SQL_STRING = N'
				INSERT INTO ' + @TempTable_Pov401ResultgbTmp +' (
					GoodID,Store,SizeNo,
					StorageNum01,StorageNum02,StorageNum03,StorageNum04,StorageNum05,StorageNum06,StorageNum07,StorageNum08,StorageNum09,StorageNum10,StorageNum11,StorageNum12,StorageNum13,StorageNum14,StorageNum15,StorageNum16,StorageNum17,
					StorageTotalNum,
					Size01,Size02,Size03,Size04,Size05,Size06,Size07,Size08,Size09,Size10,Size11,Size12,Size13,Size14,Size15,Size16,Size17,
					TCost,TAdvicePrice,TGSpecialPrice,GoodName,ClientShort,Sort01,Brand,Factory,Season,OpenDate,Sort05Name,GoodID10,EndStockDate,TPrice,ParentID) 
				SELECT 
					RTrim(GS.GoodID) As GoodID,upper(RTrim(GS.Store)) As Store, GS.SizeNo,
					GS.StorageNum01,GS.StorageNum02,GS.StorageNum03,GS.StorageNum04,GS.StorageNum05,GS.StorageNum06,GS.StorageNum07,GS.StorageNum08,GS.StorageNum09,GS.StorageNum10,GS.StorageNum11,GS.StorageNum12,GS.StorageNum13,GS.StorageNum14,GS.StorageNum15,GS.StorageNum16,GS.StorageNum17,
					GS.StorageTotalNum,
					S.Size01,S.Size02,S.Size03,S.Size04,S.Size05,S.Size06,S.Size07,S.Size08,S.Size09,S.Size10,S.Size11,S.Size12,S.Size13,S.Size14,S.Size15,S.Size16,S.Size17,
					G.Cost,G.AdvicePrice,G.SpecialPrice,G.GoodName,C.ClientShort,G.Sort01,G.Brand,G.Factory,G.Season,G.OpenDate,
					S5.Sort05Name,Left(GS.GoodID,10) As GoodID10,G.OpenDate,G.Price,G.ParentID 
				  FROM ' + @TempTable_StoreTmpgb + ' GS Left Join Client C ON Gs.Store = C.ClientID 
					   Left Join [Size] S On Gs.SizeNo = S.SizeNo 
					   Left Join Goods G ON Gs.GoodID = G.GoodID 
					   Left Join Sort05 S5 ON G.Sort05 = S5.Sort05ID  
				 WHERE 1=1 
					   AND (Gs.StorageNum01<>0 OR Gs.StorageNum02<>0 OR Gs.StorageNum03<>0 OR Gs.StorageNum04<>0 OR Gs.StorageNum05<>0 OR Gs.StorageNum06<>0 OR Gs.StorageNum07<>0 OR Gs.StorageNum08<>0 OR Gs.StorageNum09<>0 OR Gs.StorageNum10<>0 OR Gs.StorageNum11<>0 OR Gs.StorageNum12<>0 OR Gs.StorageNum13<>0 OR Gs.StorageNum14<>0 OR Gs.StorageNum15<>0 OR Gs.StorageNum16<>0 OR Gs.StorageNum17<>0)
					   '
				+ @SQL_WHERE_GoodStorage
			EXEC sp_executesql @SQL_STRING
		END ELSE BEGIN
			SET @SQL_STRING = N'
				INSERT INTO ' + @TempTable_Pov401ResultgbTmp +' (
					GoodID,Store,SizeNo,
					StorageNum01,StorageNum02,StorageNum03,StorageNum04,StorageNum05,StorageNum06,StorageNum07,StorageNum08,StorageNum09,StorageNum10,
					StorageNum11,StorageNum12,StorageNum13,StorageNum14,StorageNum15,StorageNum16,StorageNum17,StorageTotalNum,
					Size01,Size02,Size03,Size04,Size05,Size06,Size07,Size08,Size09,Size10,Size11,Size12,Size13,Size14,Size15,Size16,Size17,
					TCost,TAdvicePrice,TGSpecialPrice,GoodName,ClientShort,Sort01,Brand,Factory,Season,OpenDate,Sort05Name,GoodID10,EndStockDate,TPrice,ParentID) 
				SELECT RTrim(GS.GoodID) As GoodID,upper(RTrim(GS.Store)) As Store,
					   GS.SizeNo,GS.StorageNum01,GS.StorageNum02,GS.StorageNum03,GS.StorageNum04,GS.StorageNum05,GS.StorageNum06,GS.StorageNum07,GS.StorageNum08,GS.StorageNum09,GS.StorageNum10,
					   GS.StorageNum11,GS.StorageNum12,GS.StorageNum13,GS.StorageNum14,GS.StorageNum15,GS.StorageNum16,GS.StorageNum17,GS.StorageTotalNum,
					   S.Size01,S.Size02,S.Size03,S.Size04,S.Size05,S.Size06,S.Size07,S.Size08,S.Size09,S.Size10,S.Size11,S.Size12,S.Size13,S.Size14,S.Size15,S.Size16,S.Size17,
					   Goods.Cost,Goods.AdvicePrice,Goods.SpecialPrice,Goods.GoodName,C.ClientShort,Goods.Sort01,Goods.Brand,Goods.Factory,Goods.Season,Goods.OpenDate,S5.Sort05Name,Left(GS.GoodID,10) As GoodID10,Goods.OpenDate,Goods.Price,Goods.ParentID 
				FROM GoodStorage GS Left Join Client C ON Gs.Store = C.ClientID 
					 Left Join [Size] S On Gs.SizeNo = S.SizeNo 
					 Left Join Goods ON Gs.GoodID = Goods.GoodID 
					 Left Join Sort05 S5 ON Goods.Sort05 = S5.Sort05ID 
				WHERE 1=1 '
				+ @SQL_WHERE_Goods
				+ @SQL_WHERE_GoodStorage
				+ N'AND (Gs.StorageNum01<>0 OR Gs.StorageNum02<>0 OR Gs.StorageNum03<>0 OR Gs.StorageNum04<>0 OR Gs.StorageNum05<>0 OR Gs.StorageNum06<>0 OR Gs.StorageNum07<>0 OR Gs.StorageNum08<>0 OR Gs.StorageNum09<>0 OR   Gs.StorageNum10<>0 OR Gs.StorageNum11<>0 OR Gs.StorageNum12<>0 OR Gs.StorageNum13<>0 OR Gs.StorageNum14<>0 OR Gs.StorageNum15<>0 OR Gs.StorageNum16<>0 OR Gs.StorageNum17<>0)'
			EXEC sp_executesql @SQL_STRING
		END

		-- 更新暫存檔@TempTable_Pov401ResultgbTmp的內容

		--1：依成本、2：依建議價、3:依特價、4依最後進價
		/* 因算法有重覆流程，故採用State Pattern的寫法。moonfeng @ 2022/10/7 19:20 */
		WHILE ( @CALC_METHOD > 0 )
		BEGIN
			IF @CALC_METHOD = 1 BEGIN --1：依成本
				SET @CALC_METHOD = 0 -- 只執行一次，除非「算法」中，有另外的算法。
				DECLARE @CalcMonth AS VARCHAR(6) = ''
				IF @CalcDate = convert(varchar, getdate(), 112) BEGIN -- 計算至當日庫存、如果沒有找到當月的資料,後面繼續用最後進價算
					SELECT @CalcMonth = ISNULL(MAX([MONTH]),'') FROM GoodsMonth
				END ELSE BEGIN -- 計算至指定日庫存，撈取最後一日的庫存統計
					SELECT TOP 1 @CalcMonth = [MONTH] FROM GoodsMonth WHERE [MONTH] = SUBSTRING(@CalcDate, 1, 6)
				END

				IF @CalcMonth = '' BEGIN --如果無月份資料，採用「最後進價」, 重新LOOP一次
					SET @CALC_METHOD = 4 -- ###### 切到「最後進價」的算法，重新執行 ######
				END ELSE BEGIN 
					SET @SQL_STRING = N'
					UPDATE T SET T.Cost=IsNull(GM.MonthEndCost,0)
					FROM ' + @TempTable_Pov401ResultgbTmp + ' AS T INNER JOIN GoodsMonth GM ON T.GoodID=GM.GoodID AND T.SizeNo=GM.SizeNo AND T.Store=GM.Store
					WHERE GM.Month = @CalcMonth'
					EXEC sp_executesql @SQL_STRING, N'@CalcMonth VARCHAR(6)', @CalcMonth
				END
			END ELSE IF @CALC_METHOD = 2 BEGIN --2：依建議價
				SET @CALC_METHOD = 0 -- 只執行一次，除非「算法」中，有另外的算法。
				SET @SQL_STRING = N'Update ' + @TempTable_Pov401ResultgbTmp + ' Set Cost=IsNull(TAdvicePrice,0)'
				EXEC sp_executesql @SQL_STRING
			END ELSE IF @CALC_METHOD = 3 BEGIN --3：依特價
				SET @CALC_METHOD = 0 -- 只執行一次，除非「算法」中，有另外的算法。
				SET @SQL_STRING = N'Update ' + @TempTable_Pov401ResultgbTmp + ' Set Cost=IsNull(TCSpecialPrice,0)'
				EXEC sp_executesql @SQL_STRING
			END ELSE IF @CALC_METHOD = 4 BEGIN --4：依最後進價
				SET @CALC_METHOD = 0 -- 只執行一次，除非「算法」中，有另外的算法。
				--IF 有成本權限
				--IF EXISTS(
				--	SELECT TOP 1 'YES'
				--	FROM [Users] AS U LEFT JOIN [GroupProgram] AS G ON U.GroupName = G.Group_Name
				--	WHERE U.UserID = @USER_ID
				--		  AND G.Program_Name = @PROGRAM_NAME
				--		  AND G.Cost_Flag = 1
				--	) BEGIN
				IF POVWeb.udfHadAuthority('庫存明細查詢', @USER_ID, 'Cost') = 1 BEGIN
					SET @SQL_STRING = N'Update ' + @TempTable_Pov401ResultgbTmp + ' Set Cost=IsNull(TCost,0)'
					EXEC sp_executesql @SQL_STRING
				END ELSE BEGIN
					SET @SQL_STRING = N'Update ' + @TempTable_Pov401ResultgbTmp + ' Set Cost=IsNull(TAdvicePrice,0)'
					EXEC sp_executesql @SQL_STRING
				END
			END
		END

		--依批價
		/**
		Update #POVWeb_Pov401ResultgbTmp Set Cost=IsNull(TPrice,0) 
		**/

		SET @SQL_STRING = N'Update ' + @TempTable_Pov401ResultgbTmp + ' Set TotalMoney=StorageTotalNum*Cost, 
						   Discount=(Case When IsNull(TAdvicePrice,0)=0 Then 0 Else Cost*100/TAdvicePrice End)'
		EXEC sp_executesql @SQL_STRING

		SET @SQL_STRING = N'Update T Set T.StoreShelfID=IsNull(P.StoreShelfID,'''') From ' + @TempTable_Pov401ResultgbTmp + ' T Left Join GoodStorage P ON T.Store=P.Store And T.GoodID=P.GoodID And T.SizeNO=P.SizeNO'
		EXEC sp_executesql @SQL_STRING

		SET @SQL_STRING = N'UPDATE T Set T.TotalSellNum=IsNull(T1.TotalSellNum,0) 
							From ' + @TempTable_Pov401ResultgbTmp + ' T Left Join (Select S.SellStore,S.GoodID,S.SizeNo, 
							Sum((Case S.SellMode When ''2'' Then -1 When ''A'' then -1 When ''C'' Then -1 When ''5'' Then 0 When ''8'' Then 0 Else 1 End)*S.TotalSellNum) As TotalSellNum 
							From Sell S 
							Group By S.SellStore,S.GoodID,S.SizeNo) T1 On T.GoodID=T1.GoodID And T.SizeNo=T1.SizeNo And T.Store=T1.SellStore' 
		EXEC sp_executesql @SQL_STRING


		SET @SQL_STRING = N'SELECT T.GoodID As GoodID,Min(SD.StockDate) As MinDate,Max(SD.StockDate) As MaxDate Into ' + @TempTable_Pov401MinDategbTmp + ' 
							From ' + @TempTable_Pov401ResultgbTmp + ' T Inner Join StockDetail SD ON T.GoodID=SD.GoodID Group By T.GoodID'
		EXEC sp_executesql @SQL_STRING

		SET @SQL_STRING = N'UPDATE T Set T.OpenDate=(Case When IsNull(M.MinDate,'''')='''' Then T.OpenDate Else M.MinDate End), T.EndStockDate=IsNull(M.MaxDate,'''') 
							From ' + @TempTable_Pov401ResultgbTmp + ' T Inner Join ' + @TempTable_Pov401MinDategbTmp + ' M ON T.GoodID=M.GoodID' 
		EXEC sp_executesql @SQL_STRING

		--Result*******************************************************************
		--最終結果
		-- 畫面顯示「總數量」「總金額」
		SET @SQL_STRING = N'
		SELECT ISNULL(Sum(StorageTotalNum),0) As totalQuantity, ISNULL(Sum(TotalMoney),0) As totalAmount FROM ' + @TempTable_Pov401ResultgbTmp
		EXEC sp_executesql @SQL_STRING

		-- 「商品庫存總計」頁
		SET @SQL_STRING = N'
		SELECT T.GoodID,T.SizeNo,
				ISNULL(Sum(StorageNum01),0) AS StorageNum01,ISNULL(Sum(StorageNum02),0) AS StorageNum02,ISNULL(Sum(StorageNum03),0) AS StorageNum03,ISNULL(Sum(StorageNum04),0) AS StorageNum04,ISNULL(Sum(StorageNum05),0) AS StorageNum05,
				ISNULL(Sum(StorageNum06),0) AS StorageNum06,ISNULL(Sum(StorageNum07),0) AS StorageNum07,ISNULL(Sum(StorageNum08),0) AS StorageNum08,ISNULL(Sum(StorageNum09),0) AS StorageNum09,ISNULL(Sum(StorageNum10),0) AS StorageNum10,
				ISNULL(Sum(StorageNum11),0) AS StorageNum11,ISNULL(Sum(StorageNum12),0) AS StorageNum12,ISNULL(Sum(StorageNum13),0) AS StorageNum13,ISNULL(Sum(StorageNum14),0) AS StorageNum14,ISNULL(Sum(StorageNum15),0) AS StorageNum15,
				ISNULL(Sum(StorageNum16),0) AS StorageNum16,ISNULL(Sum(StorageNum17),0) AS StorageNum17,
				ISNULL(Sum(StorageTotalNum),0) As StorageTotalNum,
				Size01 As Size01,Size02 As Size02,Size03 As Size03,Size04 As Size04,Size05 As Size05,Size06 As Size06,Size07 As Size07,Size08 As Size08,Size09 As Size09,Size10 As Size10,
				Size11 As Size11,Size12 As Size12,Size13 As Size13,Size14 As Size14,Size15 As Size15,Size16 As Size16,Size17 As Size17,
				Cost,Discount,GoodName,ISNULL(Sum(TotalMoney),0) As TotalMoney,GoodID10,Sort01,Sort05Name,Position,OpenDate,EndStockDate,T1.TotalSellNum,
				Count(Store) As CountNum 
			FROM ' + @TempTable_Pov401ResultgbTmp +' T  Left Join (
			SELECT S.GoodID,S.SizeNo, 
					Sum((Case S.SellMode When ''2'' Then -1 When ''A'' then -1 When ''C'' Then -1 When ''5'' Then 0 When ''8'' Then 0 Else 1 End)*S.TotalSellNum) As TotalSellNum
			FROM Sell S
			GROUP BY S.GoodID,S.SizeNo) T1 On T.GoodID=T1.GoodID And T.SizeNo=T1.SizeNo
		GROUP BY T.GoodID,T.SizeNo,Size01,Size02,Size03,Size04,Size05,Size06,Size07,Size08,Size09,Size10,Size11,Size12,Size13,Size14,Size15,Size16,Size17,Cost,Discount,GoodName,GoodID10,Sort01,Sort05Name,Position,OpenDate,EndStockDate,T1.TotalSellNum 
		ORDER By T.GoodID,T.SizeNo 
		'
		EXEC sp_executesql @SQL_STRING

		-- 「商品庫存明細」頁，POVWeb 目前沒用到，先註解。已串接至React Web。 Moon @ 2022/10/28 18:58
		--SET @SQL_STRING = N'
		--SELECT * From ' + @TempTable_Pov401ResultgbTmp + ' Order By GoodID,SizeNo,Store'
		--EXEC sp_executesql @SQL_STRING

	END TRY
	BEGIN CATCH
		--SELECT 
		--	ERROR_NUMBER() AS ErrorNumber
		--	, ERROR_SEVERITY() AS ErrorSeverity
		--	, ERROR_STATE() AS ErrorState
		--	, ERROR_PROCEDURE() AS ErrorProcedure
		--	, ERROR_LINE() AS ErrorLine
		--	, ERROR_MESSAGE() AS ErrorMessage

		SET @O_MSG = '查詢失敗。' + 'MSG:' + ERROR_MESSAGE() 
		RETURN
	END CATCH

	--drop temp tables*******************************************************
	IF Object_id(@TempTable_StoreTmpgb) IS NOT NULL BEGIN
		SET @SQL_STRING = 'DROP TABLE ' + @TempTable_StoreTmpgb
		EXEC(@SQL_STRING)
	END

	IF Object_id(@TempTable_Pov414TmpGB) IS NOT NULL BEGIN
		SET @SQL_STRING = 'DROP TABLE ' + @TempTable_Pov414TmpGB
		EXEC(@SQL_STRING)
	END

	IF Object_id(@TempTable_Pov401ResultgbTmp) IS NOT NULL BEGIN
		SET @SQL_STRING = 'DROP TABLE ' + @TempTable_Pov401ResultgbTmp
		EXEC(@SQL_STRING)
	END

	IF Object_id(@TempTable_Pov401MinDategbTmp) IS NOT NULL BEGIN
		SET @SQL_STRING = 'DROP TABLE ' + @TempTable_Pov401MinDategbTmp
		EXEC(@SQL_STRING)
	END


	SET NOCOUNT OFF;
END