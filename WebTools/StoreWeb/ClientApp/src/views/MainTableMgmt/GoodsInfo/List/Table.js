/*eslint-disable */
// ** React Imports
import { Fragment, useEffect, useRef, useMemo, useCallback, forwardRef } from 'react'
import { useSelector, useDispatch } from 'react-redux'

// ** Table Columns

// ** Third Party Components
import { useNavigate } from 'react-router-dom'
import useState from 'react-usestateref'
import Flatpickr from 'react-flatpickr'
import ReactPaginate from 'react-paginate'
import { ChevronDown, MoreVertical, FileText, Archive, Trash2 } from 'react-feather'
import DataTable from 'react-data-table-component'
import { useDebounce } from 'use-debounce'
import axios from 'axios'
import { AgGridReact } from 'ag-grid-react'
import Cookies from 'js-cookie'

// ** Reactstrap Imports
import { Card, CardHeader, CardBody, CardTitle, Input, Button } from 'reactstrap'

import { toggleIsLoading, GetGoodsHelpOffset, saveSelectedColumn, clearTable, savePageStatus } from '../store'
import { CRUDTemplateDefault } from '@application/component/CRUDTemplate'
import { showHintMSG } from '@application/store/sysSettings'
// ** Styles
import '@styles/react/libs/tables/react-dataTable-component.scss'

import { exportToExcel } from '@CityAppHelper'
import useUpdateEffect from '@hooks/useUpdateEffect'
import Columns, { options, columnFilter } from './Column'
import CustomGridHeader from '@application/component/CustomGridHeader'
import CustomGridPagination from '@application/component/CustomGridPagination'

const Table = ({ access, t }) => {
  const navigate = useNavigate()
  const dispatch = useDispatch()
  const gridApi = useRef(null)
  const store = useSelector(state => state.MainTableMgmt_GoodsInfo)
  const dataTable = store.dataTable.data
  const changeFlag = store.dataTable.changeFlag
  const paginationNum = store.dataTable.num
  const request = store.dataTable.params
  const selectedColumn = store.dataTable.selectedColumn
  const defaultSelectedColumn = store.dataTable.defaultSelectedColumn
  const [offSetTop, setOffSetTop] = useState(0)
  const currentColumnDef = useRef(null)

  // ** States
  const [dualListModalOpen, setDualListModalOpen] = useState(false)
  const toggleDualListModal = () => setDualListModalOpen(!dualListModalOpen)
  const [sort, setSort, sortRef] = useState(request?.sort || 'asc')
  const [searchTerm, setSearchTerm, searchTermRef] = useState(request.q || '')
  const [currentPage, setCurrentPage, currentPageRef] = useState(request.page || 1)
  const [sortColumn, setSortColumn, sortColumnRef] = useState(request.sortColumn)
  const [rowsPerPage, setRowsPerPage, rowsPerPageRef] = useState(Cookies.get('rowsPerPage') || request.perPage || 10)
  const [advanceRequest, setAdvanceRequest, advanceRequestRef] = useState(request.advanceRequest || null)
  const [selectedRows, setSelectedRows, selectedRowsRef] = useState([])
  const [debounceSearch] = useDebounce(searchTerm, 600)

  // ** Initial Defs
  const columnDefs = useMemo(() => {
    return Columns({ access, t, setCurrentPage, navigate })
  }, [access, t])

  //欄位調整
  const updateGridColumn = (columnDefs, selectedColumn) => {
    const columnDefCopy = [...columnDefs]
    const defaultColumn = options?.[0]?.options?.map(x => x.value)
    const fixedColumn = columnDefCopy.filter(col => defaultColumn?.indexOf(col.field) > -1)
    const actionColumn = columnDefCopy[columnDefCopy?.length - 1]
    const customColumn = selectedColumn.map(col => {
      const column = columnDefs.find(x => x.field === col.value)
      column.hide = false
      return column
    })
    const result = [...fixedColumn, ...customColumn, actionColumn]
    //只搜尋AGGrid上面有的欄位
    currentColumnDef.current = [...fixedColumn, ...customColumn].map(x => x.field)
    return result
  }



  //欄位調整
  useUpdateEffect(() => {
    if (selectedColumn) {
      const result = updateGridColumn(columnDefs, selectedColumn)
      gridApi.current?.setGridOption("columnDefs", result)
    }
  }, [selectedColumn, gridApi.current])

  const getTableData = useCallback(async (params) => {
    let result
    const source = axios.CancelToken.source()
    const columnName = currentColumnDef.current ? currentColumnDef.current : columnDefs.map(x => x.field).filter(x => x !== undefined)
    columnName.push('Bar')
    await dispatch(
      GetGoodsHelpOffset([{
        sort: sortRef.current,
        sortColumn: sortColumnRef.current,
        q: searchTermRef.current,
        page: currentPageRef.current,
        perPage: rowsPerPageRef.current,
        advanceRequest: advanceRequestRef.current,
        mode: params?.mode || '',
        searchTerm: { field: columnName }
      },
      source.token
      ])
    ).then(res => {
      result = res
    })
    return result
  }, [])

  useUpdateEffect(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, [debounceSearch])
  useUpdateEffect(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, [advanceRequest])
  useUpdateEffect(() => changeFlag && getTableData(), [changeFlag])
  useUpdateEffect(() => getTableData(), [sort, sortColumn, currentPage, rowsPerPage])

  // viewport handle
  useEffect(() => {
    if (offSetTop) return
    setOffSetTop(document.getElementById('main-content')?.offsetTop +
      document.getElementsByClassName('rdt_TableHeader')?.[0]?.clientHeight +
      document.getElementsByClassName('rdt_TableFooter')?.[0]?.clientHeight)
  }, [store.dataTable.data.length])

  const [toggleCleared, setToggleCleared] = useState(false)


  // ** Sort Handle
  const handleSort = useCallback(({ columns }) => {
    const sortColumn = columns.find(col => col.sort)
    setSort(sortColumn?.sort || 'asc')
    setSortColumn(sortColumn?.colId || columnDefs?.[0].field)
  }, [])
  // ** Function in get data on page change
  const handlePagination = useCallback(page => {
    setCurrentPage(page.selected + 1)
  }, [])

  // ** Function in get data on rows per page
  const handlePerPage = useCallback(e => {
    const value = parseInt(e.currentTarget.value)
    Cookies.set('rowsPerPage', value)
    setRowsPerPage(value)
    setCurrentPage(1)
  }, [])

  // ** Function in get data on advance search query change
  const handleAdvanceSearch = useCallback(val => {
    setAdvanceRequest(val)
  }, [])

  // ** Function in get data on search query change
  const handleFilter = useCallback(val => {
    setSearchTerm(val)
  }, [])

  const handleDelete = () => {
    const requestParams = selectedRows.map(x => ({ goodID: x.goodID }))
    CRUDTemplateDefault({
      // debug: true,
      viewTitle: '',
      jobTitle: '整筆刪除',
      request: requestParams,
      apiPath: '/api/Goods/DelGoodsInfoMulti',
      doStart: () => {
        dispatch(toggleIsLoading(true))
      },
      doError: () => {
        dispatch(toggleIsLoading(false))
      },
      doSuccess: () => {
        showHintMSG('刪除成功')
        setToggleCleared(!toggleCleared)
        dispatch(GetGoodsHelpOffset(request))
      }
    })
  }

  //** Delete Handle
  const handleRowSelected = useCallback(({ api }) => {
    setSelectedRows(api.getSelectedRows())
  }, [])

  const downloadXLSX = async ({ selectedData = null, getTableData = null }) => {
    const fileName = '商品'
    let sheetColumn = []
    if (selectedColumn.length === 0) {
      const temp = columnDefs.map(x => ({ header: x.headerName, key: x.field, cell: (row) => row[x.field] }))
      temp.pop()
      sheetColumn = temp
      sheetColumn = temp
    }
    else {
      const defaultCol = options?.[0]?.options.map(x => ({ header: columnDefs.find(d => d.field === x.value)?.headerName, key: x.value, cell: (row) => row[x.value] })) || []
      const selectedCol = [...selectedColumn].map(x => ({ header: columnDefs.find(d => d.field === x.value)?.headerName, key: x.value, cell: (row) => row[x.value] }))
      sheetColumn = [...defaultCol, ...selectedCol]
      sheetColumn.push({ header: '條碼', key: 'bar', cell: (row) => row['bar'] })
    }
    let sheetData = selectedData
    if (getTableData) {
      const result = await getTableData({ mode: 'all' })
      if (result.error) return
      sheetData = result.payload.data.result
    }

    exportToExcel([{ sheetData, sheetName: fileName }], fileName, sheetColumn)
  }

  const onGridReady = useCallback((params) => {
    gridApi.current = params.api
    params.api?.applyColumnState({
      state: [{ colId: sortColumn, sort }],
      defaultState: { sort: null }
    })

  }, [])

  useEffect(() => {
    getTableData()
  }, [])

  useUpdateEffect(() => {
    dispatch(savePageStatus({
      key: 'params',
      status: {
        sort: sortRef.current,
        sortColumn: sortColumnRef.current,
        q: searchTermRef.current,
        page: currentPageRef.current,
        perPage: rowsPerPageRef.current,
        advanceRequest: advanceRequestRef.current
      }
    }))
  }, [sortRef.current, sortColumnRef.current, searchTermRef.current, currentPageRef.current, rowsPerPageRef.current, advanceRequestRef.current])

  //有cookie的話欄位調整(重新render起始欄位)
  useUpdateEffect(() => {
    const json = Cookies.get('SelectedMemo')
    if (json) {
      const memo = JSON.parse(json)
      if (memo?.GoodsInfo) {
        dispatch(saveSelectedColumn(memo.GoodsInfo))
        const result = updateGridColumn(columnDefs, memo.GoodsInfo)
        gridApi.current.setGridOption("columnDefs", result)
      }
    }

  }, [gridApi.current])


  const dualListObj = {
    options,
    open: dualListModalOpen,
    toggle: toggleDualListModal,
    saveSelectedColumn,
    columnDefs: columnDefs.filter(x => x.pinned !== true).map(x => ({ label: x.headerName, value: x.field })),
    defaultSelectedColumn
  }


  return (
    <Fragment>
      <Card id='main-content' className='mb-0'>
        <CustomGridHeader
          handleFilter={handleFilter}
          searchTerm={searchTerm}
          access={access}
          handleExcel={() => downloadXLSX({ getTableData })}
          handleAdd={() => { dispatch(clearTable()) }}
          addLink='../MainTableMgmt/GoodsInfo/Single/add'
          columnFilter={columnFilter}
          advanceRequest={advanceRequest}
          handleAdvanceSearch={handleAdvanceSearch}
          selectedRowsLength={selectedRows.length}
          handleDelete={handleDelete}
          handleSelectedExcel={() => downloadXLSX({ selectedData: selectedRowsRef.current })}
          dualListComponent={dualListObj}
        />
        <div
          className="ag-theme-quartz"
          style={{ height: `calc(100vh - ${offSetTop}px - 1rem)` }}
        >
          <AgGridReact
            rowData={store.dataTable.data}
            columnDefs={columnDefs}
            rowSelection="multiple"
            suppressRowClickSelection={true}
            pagination={true}
            suppressPaginationPanel={true}
            enableServerSideSorting={true}
            onGridReady={onGridReady}
            onSelectionChanged={handleRowSelected}
            onSortChanged={handleSort}
          />
        </div>
        <CustomGridPagination
          total={paginationNum}
          rowsPerPage={rowsPerPage}
          handlePerPage={handlePerPage}
          handlePagination={handlePagination}
          currentPage={currentPage}
        />
      </Card>
    </Fragment>
  )
}

export default Table
