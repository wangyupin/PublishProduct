SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--*********************************************************
-- Name�GRPTApp.uspGetPeersCompare_GetFieldName
-- Desc�GSP �P�~�~�Z���-�U������
-- Author�GRex
-- LastModifyDTM�G2021/11/1 19:46
-- Modify History�G
-- 1. Initial. Rex@2021/11/1 19:46
-- 2.�קאּheaderName�Pfield Rex@2021/11/4 19:36
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
    "headerName": "*���d�W��",
    "field": "store_name",
	"sortable": true
  },
  {
    "hide": true,
    "headerName": "*���d",
    "field": "store_id"
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "�~�Z",
    "field": "performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "�W��",
    "field": "ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "����~�Z",
    "field": "cp_performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "���",
    "field": "cp_ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "percentage",
    "headerName": "�����v",
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
    "headerName": "*�P�~�W��",
    "field": "branch_name",
	"sortable": true
  },
  {
    "hide": true,
    "headerName": "*�P�~",
    "field": "branch_id"
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "�~�Z",
	"width": 160,
    "field": "performance",
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "�W��",
    "field": "ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "����~�Z",
    "field": "cp_performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "���",
    "field": "cp_ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "percentage",
    "headerName": "�����v",
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
    "headerName": "*���d�W��",
    "field": "store_name",
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "�~�Z",
    "field": "performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "�W��",
    "field": "ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "����",
    "field": "ranking_store",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "WanDallor",
    "headerName": "����~�Z",
    "field": "cp_performance",
	"width": 160,
	"sortable": true
  },
  {
    "valueFormatter": "number",
    "headerName": "���",
    "field": "cp_ranking",
	"width": 80,
	"sortable": true
  },
  {
    "valueFormatter": "percentage",
    "headerName": "�����v",
    "field": "growth_rate",
	"width": 110,
	"sortable": true
  }
]
' AS FieldDefine 
	SET NOCOUNT OFF;
END

GO

