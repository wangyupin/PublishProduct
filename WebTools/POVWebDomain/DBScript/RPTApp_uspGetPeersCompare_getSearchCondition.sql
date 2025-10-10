GO
/****** Object:  StoredProcedure [RPTApp].[uspGetPeersCompare_GetSearchCondition]    Script Date: 2022/8/30 �U�� 07:26:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



 --EXEC RPTApp.uspGetPeersCompare_getSearchCondition
--*********************************************************
-- Name�GRPTAppDB.uspGetPeersCompare_GetSearchCondition
-- Desc�GSP �P�~�~�Z����d�߱���
-- Author�GRex
-- LastModifyDTM�G2022/4/28
-- Modify History�G
-- 1. Initial. Rex@2022/4/28 
--*********************************************************
CREATE PROCEDURE [RPTApp].[uspGetPeersCompare_GetSearchCondition]
(
	@QRY_IP AS NVARCHAR(20)  = N''
)
AS
BEGIN
    SET NOCOUNT ON;

	SELECT  [ClientID], [ClientShort] FROM [dbo].[OtherProc]
	GROUP BY  [ClientID] ,[ClientShort]
	OPTION (RECOMPILE)

    SET NOCOUNT OFF;
END;
