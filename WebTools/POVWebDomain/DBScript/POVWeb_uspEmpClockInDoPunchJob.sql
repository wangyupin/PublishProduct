USE [TEST5_NEW]
GO
/****** Object:  StoredProcedure [POVWeb].[uspEmpClockInPunchJob]    Script Date: 2022/6/30 下午 05:07:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--*********************************************************
-- Name：[POVWeb].[uspEmpClockInDoPunchJob]
-- Desc：員工打卡作業
-- Author：MoonFeng
-- LastModifyDTM：2022/6/30 17:08
-- Modify History：
-- 1. Initial. MoonFeng@2022/6/30 17:08
-- 
-- SAMPLE：
/*
DECLARE	@return_value int,
		@O_MSG nvarchar(100)

EXEC	@return_value = [POVWeb].[uspEmpClockInDoPunchJob]
		@EmpID = N'moon',
		@ClockStore = N'010',
		@PunchType = 1,
		@O_MSG = @O_MSG OUTPUT

SELECT	@O_MSG as N'@O_MSG'

SELECT	'Return Value' = @return_value
*/
--*********************************************************

ALTER PROCEDURE [POVWeb].[uspEmpClockInDoPunchJob]
(
	 @EmpID varchar(20)
	,@ClockStore varchar(20)
	,@PunchType int -- /// 1上班；2下班；3外出；4返回；5進入；6離開

	,@O_MSG AS NVARCHAR(100) = N'' OUTPUT
)
AS
BEGIN
SET NOCOUNT ON;
--Initial Variablies*******************************************************
	DECLARE @ClockDate VARCHAR(8) = convert(varchar, getdate(), 112) 
	DECLARE @ClockTime VARCHAR(8) = convert(varchar, getdate(), 108) 

	DECLARE @Holiday NVARCHAR(8) = ''
	;WITH PunchTypeName(id, value) AS (
		SELECT 1 AS id, '上班' AS value
		UNION ALL
		SELECT 2, '下班'
		UNION ALL
		SELECT 3, '外出'
		UNION ALL
		SELECT 4, '返回'
		UNION ALL
		SELECT 5, '進入'
		UNION ALL
		SELECT 6, '離開'
	)
	SELECT @Holiday = value FROM PunchTypeName WHERE id=@PunchType

--Check Parameters*********************************************************
	IF NOT EXISTS(Select Top 1 EmpID From Employee Where EmpID=@EmpID) BEGIN
		Set @O_MSG='員工編號錯誤，請重新輸入!'
		RETURN  
	END

	--上班、下班不可重覆打卡
	IF (@PunchType = 1 or @PunchType = 2) BEGIN
		IF EXISTS(Select Top 1 EmpID From EmpClockIn Where EmpID=@EmpID And ClockStore=@ClockStore And ClockDate=@ClockDate And Holiday=@Holiday) BEGIN
			Set @O_MSG='此員工今日' + @Holiday + '已打卡，不可重覆打卡!'
			RETURN  
		END
	END

	IF (@Holiday = '') BEGIN
		Set @O_MSG = '打卡方式錯誤>>' + CAST(@PunchType AS VARCHAR(2)) 
		RETURN  
	END

--BIZ**********************************************************************
	Insert Into EmpClockIn (EmpID,ClockStore,ClockDate,Holiday,ClockTime,ChangeDate,ChangePerson) 
	VALUES( @EmpID,@ClockStore,@ClockDate,@Holiday,@ClockTime,@ClockDate,@EmpID )

--Result*******************************************************************
	SELECT @EmpID  + ' ' + @ClockTime + ' ' + @Holiday + ' 打卡完成' as msg 

SET NOCOUNT OFF;
END
