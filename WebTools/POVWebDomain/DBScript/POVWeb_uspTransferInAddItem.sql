SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name：[POVWeb].[uspTransferInAddItem]
-- Desc：店櫃調入作業-新增明細
-- Author：MoonFeng
-- LastModifyDTM：2022/12/14 15:56
-- Modify History：
-- 1. Initial. MoonFeng@2022/12/14 15:56
-- 
-- SAMPLE：
/*
DECLARE	@;THROW 6636001, @O_MSG, 1;_value int,
		@O_MSG nvarchar(100)=''

EXEC	@;THROW 6636001, @O_MSG, 1;_value = [POVWeb].[uspEmpOnDutyDelEmpOnDuty]
		@YearMonth = N'20227',
		@Item = 2,
		@O_MSG = @O_MSG OUTPUT

SELECT	@O_MSG as N'@O_MSG'

SELECT	';THROW 6636001, @O_MSG, 1; Value' = @;THROW 6636001, @O_MSG, 1;_value
*/
--*********************************************************

ALTER PROCEDURE [POVWeb].[uspTransferInAddItem] 
(
	@Editor VARCHAR(16) 
	, @FillPerson VARCHAR(4) = N''
	, @O_MSG AS NVARCHAR(100) = N'' OUTPUT

	, @TransferID AS VARCHAR(20) = N'' OUTPUT -- 若為空白，由系統新增編號，再回傳
	, @Item VARCHAR(3)
	, @TranDate VARCHAR(8)
	, @GoodID VARCHAR(20)
	, @SizeNo VARCHAR(1)
	, @TranNum1 FLOAT = 0, @TranNum2 FLOAT = 0, @TranNum3 FLOAT = 0, @TranNum4 FLOAT = 0, @TranNum5 FLOAT = 0
	, @TranNum6 FLOAT = 0, @TranNum7 FLOAT = 0, @TranNum8 FLOAT = 0, @TranNum9 FLOAT = 0, @TranNum10 FLOAT = 0
	, @TranNum11 FLOAT = 0, @TranNum12 FLOAT = 0, @TranNum13 FLOAT = 0, @TranNum14 FLOAT = 0, @TranNum15 FLOAT = 0
	, @TranNum16 FLOAT = 0, @TranNum17 FLOAT = 0

	, @TranPrice FLOAT = 0 -- 單價

	, @TranInStore VARCHAR(8) -- 調入庫點
	, @TranOutStore VARCHAR(8) -- 調出庫點
)
AS
BEGIN
	SET NOCOUNT ON;

	--Test Variablies*******************************************************
	--DECLARE	@YearMonth VARCHAR(8) = '202207'

	--Initial Variablies*******************************************************
	DECLARE
		@TaskTitle VARCHAR(128) = '店櫃調入作業'
	    , @ChangePerson VARCHAR(4) = @Editor
	    , @ChangeDate VARCHAR(8) = convert(varchar, getdate(), 112)
	    , @TempString NVARCHAR(MAX) = '' 
	    , @TempInt INT = 0 
	
	--調入作業
	--FactStore = 調出庫點, TranInStore = 調入庫點, TranOutStore = 中途倉
	DECLARE 
		@NEW_TranInStore VARCHAR(8) = @TranInStore
		, @NEW_TranOutStore VARCHAR(8) = @TranOutStore
		, @HalfWayStore AS VARCHAR(8) = '0ZZZZ'-- 中途倉

	--Check Parameters*********************************************************
	-- 檢查是否有新增的權限
	IF NOT (POVWeb.udfHadAuthority('店櫃調入', @ChangePerson, 'Insert') = 1) BEGIN
		SET @O_MSG = @TaskTitle + '，帳號：' + @ChangePerson +'無新增權限!'
		;THROW 6636001, @O_MSG, 1;
	END

	---- 檢查調貨單號。是否重覆 TransferID 這要靠c#來檢查
	--IF LEN(@TransferID) < 1 BEGIN
	--	--SET @O_MSG = '調貨單號[' + @TransferID +']已不允許空白!'
	--	--新增調貨單號
	--	DECLARE @PaddingSize AS SMALLINT = 4
	--	DECLARE @PaddingString AS VARCHAR(4) = '0000'

	--	SET @TransferID = 'MoonTest'
	--	SELECT MAX(SUBSTRING(TransferId, 1, LEN(TransferId) - @PaddingSize))
	--			+ right(@PaddingString + RTRIM(MAX(SUBSTRING(TransferId, 1 + LEN(TransferId) - @PaddingSize, @PaddingSize)) + 1), @PaddingSize)
	--	From Transfer 
	--	Where 
	--		TransferID like @TransferId + '%'
	--		AND ( LEN(ISNULL(@item, '')) = 0 or Item = @Item)
	--END

	-- 檢查Item是否重覆
	IF EXISTS(SELECT 1 FROM TRANSFER WHERE TransferID = @TransferID AND Item = @Item) BEGIN
		SET @O_MSG = '調撥編號：' + @TransferID + '、Item[' + @Item +']不可重覆!'
		;THROW 6636001, @O_MSG, 1;
	END

	-- 調入庫點關帳判斷 CloseAccount.ClassID=(1.進貨 2.出貨 3.調撥 4.銷貨 5調整  )
	-- 檢查調入庫點關帳日期  
	SELECT @TempString = CloseDate FROM CloseAccount WHERE CloseClass = 3 AND StoreID = @NEW_TranInStore AND CloseDate >= @ChangeDate
	IF ISNULL(@TempString, '') <> ''  BEGIN
		SET @O_MSG = '調入庫點[' + @NEW_TranInStore +']已關帳至[' + @TempString + ',數據不允許存檔!'
		;THROW 6636001, @O_MSG, 1;
	END
	-- 檢查調出庫點關帳日期  
	SELECT @TempString = CloseDate FROM CloseAccount WHERE CloseClass = 3 AND StoreID = @NEW_TranOutStore AND CloseDate >= @ChangeDate
	IF ISNULL(@TempString, '') <> ''  BEGIN
		SET @O_MSG = '調出庫點[' + @NEW_TranOutStore +']已關帳至[' + @TempString + ',數據不允許存檔!'
		;THROW 6636001, @O_MSG, 1;
	END

	-- 填單人員 FillPerson
	IF NOT EXISTS(Select EmpName From Employee Where EmpID=@ChangePerson) BEGIN
		SET @O_MSG = '員工代號輸入錯誤'
		;THROW 6636001, @O_MSG, 1;
	END

	IF (ISNULL(@FillPerson, '') = '') BEGIN
		SET @FillPerson = @ChangePerson
	END

	-- 調出庫點與調入庫點不能相同
	IF (@NEW_TranInStore = @NEW_TranOutStore) BEGIN
		SET @O_MSG = '調出庫點與調入庫點不能相同!'
		;THROW 6636001, @O_MSG, 1;
	END

	/*
	-- 前台調出作業 - 「調出庫點」禁止打01011 與01012 庫點單位
	IF  @TranOutStore IN ('01011', '01012') BEGIN
		SET @O_MSG = '前台調出作業，調出庫點禁止打01011 與01012 庫點單位'
		;THROW 6636001, @O_MSG, 1;
	END

	-- 檢查調出入庫點 TranOutStore
	IF NOT EXISTS(Select 1 From Client Where ClientID = @TranInStore) BEGIN
		SET @O_MSG = '調入庫點輸入錯誤'
		;THROW 6636001, @O_MSG, 1;
	END

	IF NOT EXISTS(Select 1 From Client Where ClientID = @TranOutStore) BEGIN
		SET @O_MSG = '調出庫點輸入錯誤'
		;THROW 6636001, @O_MSG, 1;
	END

	-- 檢查商品型號 GoodID and 商品段碼 SizeNo
	IF NOT EXISTS(SELECT 1 FROM Goods WHERE GoodID = @GoodID AND ( SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO )) BEGIN
		SET @O_MSG = '找不到商品型號[' + @GoodID + ']或商品段碼[' + @SizeNO + ']'
		;THROW 6636001, @O_MSG, 1;
	END

	-- 檢查商品型號於調出庫點是否有庫存
	IF ISNULL(@TempString, '') <> ''  BEGIN
		SET @O_MSG = '調出庫點[' + @TranInStore +']已關帳至[' + @TempString + ',數據不允許存檔!'
		;THROW 6636001, @O_MSG, 1;
	END
	IF NOT EXISTS(SELECT 1 FROM Goods WHERE GoodID = @GoodID AND ( SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO )) BEGIN
		SET @O_MSG = '找不到商品型號[' + @GoodID + ']或商品段碼[' + @SizeNO + ']'
		;THROW 6636001, @O_MSG, 1;
	END

	*/
	--BIZ**********************************************************************
	--Initial variablies for insert
	DECLARE
		@TotalTranNum AS FLOAT = @TranNum1 + @TranNum2 + @TranNum3 + @TranNum4 + @TranNum5 + @TranNum6 + @TranNum7 + @TranNum8 + @TranNum9 + @TranNum10
							+ @TranNum11 + @TranNum12 + @TranNum13 + @TranNum14 + @TranNum15 + @TranNum16 + @TranNum17
		, @Mode AS VARCHAR(1) = '2' -- 調入作業，Mode = 2
		, @GoodName AS VARCHAR(200)
		, @Brand AS VARCHAR(8)
		, @Factory AS VARCHAR(8)
		, @TranCost AS FLOAT
		, @SheetFlag AS SMALLINT = 0
		, @RemarkNew AS VARCHAR(200) = ''
		
	DECLARE @TranAmount AS FLOAT = @TranPrice * @TotalTranNum

	SELECT TOP 1
			@GoodName = GoodName, @Brand = Brand, @Factory = Factory, @TranCost = Cost
	FROM Goods 
	WHERE GoodID = @GoodID AND ( SizeNo1 = @SizeNO OR SizeNo2 = @SizeNO OR SizeNo3 = @SizeNO ) 

	BEGIN TRY
		-- 此處不作 Transaction control，由外部C#處理
		-- 新增明細，由C#作TRANSACTION
		INSERT INTO Transfer (
			TransferID, Item, TranDate, FillPerson
			, GoodID, GoodName, SizeNo, Brand, Factory
			, TranNum1, TranNum2, TranNum3, TranNum4, TranNum5, TranNum6, TranNum7, TranNum8, TranNum9, TranNum10
			, TranNum11, TranNum12, TranNum13, TranNum14, TranNum15, TranNum16, TranNum17
			, TotalTranNum, TranPrice, TranAmount, TranCost
			, ChangeDate, ChangePerson, SheetFlag, RemarkNew, Mode
			, TranOutStore, TranInStore, FactStore
			) 
		VALUES(
			@TransferID, @Item, @TranDate, @FillPerson
			, @GoodID, @GoodName, @SizeNo, @Brand, @Factory
			, @TranNum1, @TranNum2, @TranNum3, @TranNum4, @TranNum5, @TranNum6, @TranNum7, @TranNum8, @TranNum9, @TranNum10
			, @TranNum11, @TranNum12, @TranNum13, @TranNum14, @TranNum15, @TranNum16, @TranNum17
			, @TotalTranNum, @TranPrice, @TranAmount, @TranCost
			, @ChangeDate, @ChangePerson, @SheetFlag, @RemarkNew, @Mode
			--TranOutStore = 中途倉, TranInStore = 調入庫點, FactStore = 調入庫點
			, @HalfWayStore, @NEW_TranInStore, @NEW_TranOutStore
			)

		-- 新增暫存檔
		INSERT INTO GoodStorage_Update (
			GoodId, SizeNo, Store
			, Qty1, Qty2, Qty3, Qty4, Qty5, Qty6, Qty7, Qty8, Qty9, Qty10
			, Qty11, Qty12, Qty13, Qty14, Qty15, Qty16, Qty17
			, TotalQty, TotalSellNum
			, ChangeDate, ChangePerson, StoreShelfID, DoFlag
			) 
		VALUES(
			@GoodID, @SizeNo, @NEW_TranInStore
			, @TranNum1, @TranNum2, @TranNum3, @TranNum4, @TranNum5, @TranNum6, @TranNum7, @TranNum8, @TranNum9, @TranNum10
			, @TranNum11, @TranNum12, @TranNum13, @TranNum14, @TranNum15, @TranNum16, @TranNum17
			, @TotalTranNum, @TranAmount
			, @ChangeDate, @ChangePerson, '', 'POVWeb.TransferIn.Add：' + @TransferID
			)
	END TRY
	BEGIN CATCH
		--SELECT 
		--	ERROR_NUMBER() AS ErrorNumber
		--	, ERROR_SEVERITY() AS ErrorSeverity
		--	, ERROR_STATE() AS ErrorState
		--	, ERROR_PROCEDURE() AS ErrorProcedure
		--	, ERROR_LINE() AS ErrorLine
		--	, ERROR_MESSAGE() AS ErrorMessage

		SET @O_MSG = @TaskTitle + '-新增明細失敗。[單號：' + @TransferID + '][item：' + @Item + ']'
					+ ERROR_MESSAGE() 
		;THROW 6636001, @O_MSG, 1;
	END CATCH	

	--Result*******************************************************************
	--讀取資料庫的資料，並返回新增的資料

	SET NOCOUNT OFF;
END
GO