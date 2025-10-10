SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name�G[POVWeb].[uspTransferInAddItem]
-- Desc�G���d�դJ�@�~-�s�W����
-- Author�GMoonFeng
-- LastModifyDTM�G2022/12/14 15:56
-- Modify History�G
-- 1. Initial. MoonFeng@2022/12/14 15:56
-- 
-- SAMPLE�G
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

	, @TransferID AS VARCHAR(20) = N'' OUTPUT -- �Y���ťաA�Ѩt�ηs�W�s���A�A�^��
	, @Item VARCHAR(3)
	, @TranDate VARCHAR(8)
	, @GoodID VARCHAR(20)
	, @SizeNo VARCHAR(1)
	, @TranNum1 FLOAT = 0, @TranNum2 FLOAT = 0, @TranNum3 FLOAT = 0, @TranNum4 FLOAT = 0, @TranNum5 FLOAT = 0
	, @TranNum6 FLOAT = 0, @TranNum7 FLOAT = 0, @TranNum8 FLOAT = 0, @TranNum9 FLOAT = 0, @TranNum10 FLOAT = 0
	, @TranNum11 FLOAT = 0, @TranNum12 FLOAT = 0, @TranNum13 FLOAT = 0, @TranNum14 FLOAT = 0, @TranNum15 FLOAT = 0
	, @TranNum16 FLOAT = 0, @TranNum17 FLOAT = 0

	, @TranPrice FLOAT = 0 -- ���

	, @TranInStore VARCHAR(8) -- �դJ�w�I
	, @TranOutStore VARCHAR(8) -- �եX�w�I
)
AS
BEGIN
	SET NOCOUNT ON;

	--Test Variablies*******************************************************
	--DECLARE	@YearMonth VARCHAR(8) = '202207'

	--Initial Variablies*******************************************************
	DECLARE
		@TaskTitle VARCHAR(128) = '���d�դJ�@�~'
	    , @ChangePerson VARCHAR(4) = @Editor
	    , @ChangeDate VARCHAR(8) = convert(varchar, getdate(), 112)
	    , @TempString NVARCHAR(MAX) = '' 
	    , @TempInt INT = 0 
	
	--�դJ�@�~
	--FactStore = �եX�w�I, TranInStore = �դJ�w�I, TranOutStore = ���~��
	DECLARE 
		@NEW_TranInStore VARCHAR(8) = @TranInStore
		, @NEW_TranOutStore VARCHAR(8) = @TranOutStore
		, @HalfWayStore AS VARCHAR(8) = '0ZZZZ'-- ���~��

	--Check Parameters*********************************************************
	-- �ˬd�O�_���s�W���v��
	IF NOT (POVWeb.udfHadAuthority('���d�դJ', @ChangePerson, 'Insert') = 1) BEGIN
		SET @O_MSG = @TaskTitle + '�A�b���G' + @ChangePerson +'�L�s�W�v��!'
		;THROW 6636001, @O_MSG, 1;
	END

	---- �ˬd�ճf�渹�C�O�_���� TransferID �o�n�ac#���ˬd
	--IF LEN(@TransferID) < 1 BEGIN
	--	--SET @O_MSG = '�ճf�渹[' + @TransferID +']�w�����\�ť�!'
	--	--�s�W�ճf�渹
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

	-- �ˬdItem�O�_����
	IF EXISTS(SELECT 1 FROM TRANSFER WHERE TransferID = @TransferID AND Item = @Item) BEGIN
		SET @O_MSG = '�ռ��s���G' + @TransferID + '�BItem[' + @Item +']���i����!'
		;THROW 6636001, @O_MSG, 1;
	END

	-- �դJ�w�I���b�P�_ CloseAccount.ClassID=(1.�i�f 2.�X�f 3.�ռ� 4.�P�f 5�վ�  )
	-- �ˬd�դJ�w�I���b���  
	SELECT @TempString = CloseDate FROM CloseAccount WHERE CloseClass = 3 AND StoreID = @NEW_TranInStore AND CloseDate >= @ChangeDate
	IF ISNULL(@TempString, '') <> ''  BEGIN
		SET @O_MSG = '�դJ�w�I[' + @NEW_TranInStore +']�w���b��[' + @TempString + ',�ƾڤ����\�s��!'
		;THROW 6636001, @O_MSG, 1;
	END
	-- �ˬd�եX�w�I���b���  
	SELECT @TempString = CloseDate FROM CloseAccount WHERE CloseClass = 3 AND StoreID = @NEW_TranOutStore AND CloseDate >= @ChangeDate
	IF ISNULL(@TempString, '') <> ''  BEGIN
		SET @O_MSG = '�եX�w�I[' + @NEW_TranOutStore +']�w���b��[' + @TempString + ',�ƾڤ����\�s��!'
		;THROW 6636001, @O_MSG, 1;
	END

	-- ���H�� FillPerson
	IF NOT EXISTS(Select EmpName From Employee Where EmpID=@ChangePerson) BEGIN
		SET @O_MSG = '���u�N����J���~'
		;THROW 6636001, @O_MSG, 1;
	END

	IF (ISNULL(@FillPerson, '') = '') BEGIN
		SET @FillPerson = @ChangePerson
	END

	-- �եX�w�I�P�դJ�w�I����ۦP
	IF (@NEW_TranInStore = @NEW_TranOutStore) BEGIN
		SET @O_MSG = '�եX�w�I�P�դJ�w�I����ۦP!'
		;THROW 6636001, @O_MSG, 1;
	END

	/*
	-- �e�x�եX�@�~ - �u�եX�w�I�v�T�01011 �P01012 �w�I���
	IF  @TranOutStore IN ('01011', '01012') BEGIN
		SET @O_MSG = '�e�x�եX�@�~�A�եX�w�I�T�01011 �P01012 �w�I���'
		;THROW 6636001, @O_MSG, 1;
	END

	-- �ˬd�եX�J�w�I TranOutStore
	IF NOT EXISTS(Select 1 From Client Where ClientID = @TranInStore) BEGIN
		SET @O_MSG = '�դJ�w�I��J���~'
		;THROW 6636001, @O_MSG, 1;
	END

	IF NOT EXISTS(Select 1 From Client Where ClientID = @TranOutStore) BEGIN
		SET @O_MSG = '�եX�w�I��J���~'
		;THROW 6636001, @O_MSG, 1;
	END

	-- �ˬd�ӫ~���� GoodID and �ӫ~�q�X SizeNo
	IF NOT EXISTS(SELECT 1 FROM Goods WHERE GoodID = @GoodID AND ( SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO )) BEGIN
		SET @O_MSG = '�䤣��ӫ~����[' + @GoodID + ']�ΰӫ~�q�X[' + @SizeNO + ']'
		;THROW 6636001, @O_MSG, 1;
	END

	-- �ˬd�ӫ~������եX�w�I�O�_���w�s
	IF ISNULL(@TempString, '') <> ''  BEGIN
		SET @O_MSG = '�եX�w�I[' + @TranInStore +']�w���b��[' + @TempString + ',�ƾڤ����\�s��!'
		;THROW 6636001, @O_MSG, 1;
	END
	IF NOT EXISTS(SELECT 1 FROM Goods WHERE GoodID = @GoodID AND ( SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO OR SizeNo1 = @SizeNO )) BEGIN
		SET @O_MSG = '�䤣��ӫ~����[' + @GoodID + ']�ΰӫ~�q�X[' + @SizeNO + ']'
		;THROW 6636001, @O_MSG, 1;
	END

	*/
	--BIZ**********************************************************************
	--Initial variablies for insert
	DECLARE
		@TotalTranNum AS FLOAT = @TranNum1 + @TranNum2 + @TranNum3 + @TranNum4 + @TranNum5 + @TranNum6 + @TranNum7 + @TranNum8 + @TranNum9 + @TranNum10
							+ @TranNum11 + @TranNum12 + @TranNum13 + @TranNum14 + @TranNum15 + @TranNum16 + @TranNum17
		, @Mode AS VARCHAR(1) = '2' -- �դJ�@�~�AMode = 2
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
		-- ���B���@ Transaction control�A�ѥ~��C#�B�z
		-- �s�W���ӡA��C#�@TRANSACTION
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
			--TranOutStore = ���~��, TranInStore = �դJ�w�I, FactStore = �դJ�w�I
			, @HalfWayStore, @NEW_TranInStore, @NEW_TranOutStore
			)

		-- �s�W�Ȧs��
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
			, @ChangeDate, @ChangePerson, '', 'POVWeb.TransferIn.Add�G' + @TransferID
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

		SET @O_MSG = @TaskTitle + '-�s�W���ӥ��ѡC[�渹�G' + @TransferID + '][item�G' + @Item + ']'
					+ ERROR_MESSAGE() 
		;THROW 6636001, @O_MSG, 1;
	END CATCH	

	--Result*******************************************************************
	--Ū����Ʈw����ơA�ê�^�s�W�����

	SET NOCOUNT OFF;
END
GO