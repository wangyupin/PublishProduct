SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--*********************************************************
-- Name：RPTApp.uspGetPeersCompare_GetFieldName
-- Desc：SP 同業業績比較-各欄位顯示
-- Author：Rex
-- LastModifyDTM：2021/11/1 19:46
-- Modify History：
-- 1. Initial. Rex@2021/11/1 19:46
-- 2.修改為headerName與field Rex@2021/11/4 19:36
--*********************************************************
CREATE PROCEDURE [RPTApp].[uspGetPeersCompare_GetFieldName]
(
	@QRY_IP AS NVARCHAR(20)  = N'',
	@O_MSG AS NVARCHAR(100) = N'' OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT ' [
  {
    "pinned": "left",
    "width": 140,
    "headerName": "*店櫃名稱",
    "field": "store_name",
	"sortable": true
  },
  {
    "hide": true,
    "headerName": "*店櫃",
    "field": "store_id"
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "業績",
    "field": "performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "名次",
    "field": "ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "比較業績",
    "field": "cp_performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "比較",
    "field": "cp_ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "percentage",
    "headerName": "成長率",
    "field": "growth_rate",
	"width": 110,
	"sortable": true
  }
]
' AS FieldDefine 

	SELECT ' [
  {
    "pinned": "left",
    "width": 160,
    "headerName": "*同業名稱",
    "field": "branch_name",
	"sortable": true
  },
  {
    "hide": true,
    "headerName": "*同業",
    "field": "branch_id"
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "業績",
	"width": 160,
    "field": "performance",
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "名次",
    "field": "ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "比較業績",
    "field": "cp_performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "比較",
    "field": "cp_ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "percentage",
    "headerName": "成長率",
    "field": "growth_rate",
	"width": 110,
	"sortable": true
  }
]
' AS FieldDefine 

	SELECT ' [
  {
    "pinned": "left",
    "width": 140,
    "headerName": "*店櫃名稱",
    "field": "store_name",
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "業績",
    "field": "performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "名次",
    "field": "ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "店內",
    "field": "ranking_store",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "比較業績",
    "field": "cp_performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "比較",
    "field": "cp_ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "percentage",
    "headerName": "成長率",
    "field": "growth_rate",
	"width": 110,
	"sortable": true
  }
]
' AS FieldDefine 
	SET NOCOUNT OFF;
END

GO

