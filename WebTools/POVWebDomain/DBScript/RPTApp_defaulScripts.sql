
/****** Object:  Schema [RPTApp]    Script Date: 2022/8/30 ¤U¤È 07:34:08 ******/
CREATE SCHEMA [RPTApp]
GO

/****** Object:  UserDefinedTableType [RPTApp].[udtStr]    Script Date: 2022/8/30 ¤U¤È 07:33:11 ******/
CREATE TYPE [RPTApp].[udtStr] AS TABLE(
	[str1] [varchar](100) NULL
)
GO

/****** Object:  UserDefinedTableType [RPTApp].[udtStrStr]    Script Date: 2022/8/30 ¤U¤È 07:33:36 ******/
CREATE TYPE [RPTApp].[udtStrStr] AS TABLE(
	[str1] [varchar](100) NULL,
	[str2] [varchar](50) NULL
)
GO

CREATE FUNCTION [RPTApp].[udfDateRange] (
    @SDT CHAR(8),
	@EDT CHAR(8)
)
RETURNS TABLE
AS
RETURN
    WITH  
	DaysTable AS 
	(
		select CAST(@SDT as datetime)  qdate
		union all
		select DATEADD(day,1,qdate) from DaysTable where qdate < @EDT
	)
	SELECT qdate AS DAY_DATE FROM DaysTable
GO

CREATE FUNCTION [RPTApp].[udfDateRangeInclude_7days] (
    @SDT CHAR(8),
	@EDT CHAR(8)
)
RETURNS TABLE
AS
RETURN
WITH	DaysTable AS 
	(
		select CAST(@SDT as datetime)  qdate
		union all
		select DATEADD(day,1,qdate) from DaysTable where qdate < @EDT
	)
	,
	Before7Days(d) AS
	(
		SELECT 1
		UNION ALL
		SELECT 2
		UNION ALL
		SELECT 3
		UNION ALL
		SELECT 4
		UNION ALL
		SELECT 5
		UNION ALL
		SELECT 6
		UNION ALL
		SELECT 7
	)
	,
	DaysRet(D1, D2) AS
	(
		SELECT qdate, DATEADD(day,-d+1,qdate)  FROM DaysTable, Before7Days
	)
	SELECT D1 AS ODay, D2 AS Unfold_Day  FROM DaysRet
GO

CREATE FUNCTION [RPTApp].[udfPeriodDateList] (
    @SDT CHAR(8),
	@EDT CHAR(8),
	@CP_SDT CHAR(8),
	@CP_EDT CHAR(8)
)
RETURNS TABLE
AS
RETURN
    WITH  
	DaysTable AS 
	(
		select CAST(@SDT as datetime)  qdate
		union all
		select DATEADD(day,1,qdate) from DaysTable where qdate < @EDT
	)
	,
	CP_DaysTable AS 
	(
		select CAST(@CP_SDT as datetime)  qdate
		union all
		select DATEADD(day,1,qdate) from CP_DaysTable where qdate < @CP_EDT
	)
	,
	ALL_DaysTable(DAY_TYPE, DAY_DATE) AS
	(
	      SELECT 1, qdate FROM DaysTable
		  UNION ALL
		  SELECT 2, qdate FROM CP_DaysTable 
	)
	SELECT DAY_TYPE,  DAY_DATE FROM ALL_DaysTable
GO

CREATE FUNCTION [RPTApp].[udfWeekDayRange] (
    @S_DT CHAR(8),
	@E_DT CHAR(8)
)
RETURNS TABLE
AS
RETURN
 WITH 
DaysNowTable( qdate, week_cnt, day_week,  weekcnt ) 
AS 
(
		SELECT CAST(@S_DT as DATETIME)  AS qdate, DATENAME(WEEK,@S_DT) AS week_cnt, DATENAME(WEEKDAY,@S_DT)  AS day_week,  DatePart(weekday, @S_DT) AS weekcnt
		UNION ALL
		SELECT DATEADD(day, 1 , qdate)  AS qdate,  DATENAME(WEEK, DATEADD(day, 1 , qdate) ) AS weekcnt, DATENAME(WEEKDAY, DATEADD(day, 1 , qdate) )  AS day_week,  DatePart(weekday, DATEADD(day, 1 , qdate)  ) AS weekcnt
		FROM DaysNowTable WHERE qdate < @E_DT
)
SELECT ROW_NUMBER() OVER (ORDER BY qdate) as ROW_ID,  CONVERT(CHAR(8) , qdate , 112)  AS DAY_DATE, week_cnt AS WEEK_CNT, day_week AS DAY_WEEK, weekcnt AS WEEKCNT FROM DaysNowTable 
GO

