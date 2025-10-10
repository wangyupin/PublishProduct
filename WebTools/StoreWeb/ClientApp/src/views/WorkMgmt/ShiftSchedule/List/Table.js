// ** React Imports
import React, { Fragment, useEffect, useCallback, useMemo, useRef } from 'react'


// ** Table Columns
import { ColumnDefs, ColumnExport, ColumnFilter } from './columns'

// ** Store & Actions
import { getData, deleteEmpOnDuty, editEmpOnDuty, savePageStatus, updateEmpOnDuty } from '../store'
import { useDispatch, useSelector } from 'react-redux'

// ** Third Party Components
import useState from 'react-usestateref'
import { AgGridReact } from 'ag-grid-react'
import Cookies from 'js-cookie'


// ** Reactstrap Imports
import { Card } from 'reactstrap'

// ** Styles
import '@styles/react/libs/tables/react-dataTable-component.scss'

// ** Helper
import { exportToExcel } from '@CityAppHelper'
import useDebounce from '@hooks/useDebounce'
import useUpdateEffect from '@hooks/useUpdateEffect'
import CustomGridHeader from '@application/component/CustomGridHeader'
import CustomGridHeaderRep from '@application/component/CustomGridHeaderRep'
import CustomGridPagination from '@application/component/CustomGridPagination'

import Sidebar from './Sidebar'
import EmpDetailModal from '../Single/EmpDetailModal'

const EmpOnDutyList = ({ access, t, setProgress }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const store = useSelector(state => state.WorkMgmt_ShiftSchedule)
    const status = store.status.list
    const empDutyTypeList = store.commonList.empDutyTypeList
    const depInfoList = store.commonList.depInfoList

    // ** States
    const [sort, setSort, sortRef] = useState(status.list?.sort || 'asc')
    const [searchTerm, setSearchTerm, searchTermRef] = useState(status?.q || '')
    const [currentPage, setCurrentPage, currentPageRef] = useState(status?.page || 1)
    const [sortColumn, setSortColumn, sortColumnRef] = useState(status?.sortColumn || 'empName')
    const [rowsPerPage, setRowsPerPage, rowsPerPageRef] = useState(Cookies.get('rowsPerPage') || 10)
    const [advanceRequest, setAdvanceRequest, advanceRequestRef] = useState(status?.advanceRequest || null)

    const [rowData, setRowData] = useState(null)
    const [selectedRows, setSelectedRows, selectedRowsRef] = useState([])
    const [offSetTop, setOffSetTop] = useState(0)
    const [sidebarOpen, setSidebarOpen] = useState(true)
    const toggleSidebar = () => setSidebarOpen(!sidebarOpen)

    const [empModOpen, setEmpModOpen] = useState(false)
    const [empModInfo, setEmpModInfo] = useState({})
    const gridApi = useRef(null)
    
    const handleEdit = (data) => {
        setEmpModOpen(true)
        setEmpModInfo({
            mode: 'edit',
            header: {
                empID: {value: data.empID, label: data.empName},
                depID: {value: data.depID, label: data.depName},
                yearMonth: data.yearMonth
            },
            data
        })
    }

    // ** Initial Defs
    const columnDefs = useMemo(() => {
        return ColumnDefs({ access, t, yearMonth: store.status.sidebar.yearMonth, empDutyTypeInfo: empDutyTypeList, isShowDutyName: store.status.sidebar.isShowDutyName, handleEdit })
    }, [access, t, store.status.sidebar.isShowDutyName, store.status.sidebar.yearMonth, JSON.stringify(empDutyTypeList)])


    const columnFilter = useMemo(() => {
        return ColumnFilter({ t })
    }, [t])

    const columnExport = useMemo(() => ColumnExport({ t }), [t])

    const getTableData = useCallback(async (params) => {
        const res = await dispatch(getData(params))
        return res
    }, [dispatch])

    useUpdateEffect(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, [advanceRequest])
    useUpdateEffect(() => store.changeFlag && getTableData(), [store.changeFlag])
    useDebounce(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, 600, [searchTerm])
    useDebounce(() => getTableData(), 50, [currentPage, rowsPerPage, sort, sortColumn])
    useUpdateEffect(async() => {
        const res = await dispatch(getData({}))
        setRowData(res.payload?.data || [])
    }
    , [store.changeFlag])


    // ** Sort Handle
    const handleSort = useCallback(({ columns }) => {
        const sortColumn = columns.find(col => col.sort)
        setSort(sortColumn?.sort || 'asc')
        setSortColumn(sortColumn?.colId || columnDefs?.[0].field)
    }, [])

    //** Delete Handle
    const handleRowSelected = useCallback(({ api }) => {
        setSelectedRows(api.getSelectedRows())
    }, [])

    // ** Function in get data on search query change
    const handleFilter = useCallback(val => {
        setSearchTerm(val)
    }, [])

    const handleExcel = useCallback(async () => {
        const result = await getTableData({ mode: 'all' })
        console.log('🧪 匯出資料內容 result.payload.data:', result.payload?.data)
        if (!result || result.error) {
          console.error('❌ 匯出失敗，無法取得資料：', result)
          return
        }
      
        const sheetData = result.payload?.data
        if (!sheetData || !Array.isArray(sheetData)) {
          console.error('❌ 匯出失敗，資料格式錯誤')
          return
        }
      
        if (!columnExport || !Array.isArray(columnExport)) {
          console.error('❌ 匯出失敗，columnExport 欄位定義錯誤：', columnExport)
          return
        }
      
        const fileName = t('shiftSchedule.title', { ns: 'workMgmt' })
        exportToExcel([{ sheetData, sheetName: fileName }], fileName, columnExport)
      }, [columnExport])

    const handleAdd = useCallback(() => {
        dispatch(editEmpOnDuty(null))
    }, [])

    const handleUpdate = useCallback((request) => {
        dispatch(updateEmpOnDuty(request))
        .then(res => !res.error && setEmpModOpen(false))
    })

    const onGridReady = useCallback((params) => {
        params.api?.applyColumnState({
            state: [{ colId: sortColumn, sort }],
            defaultState: { sort: null }
        })
        params.api?.sizeColumnsToFit()
        gridApi.current = params.api

    }, [])

    //status handle
    useEffect(() => {
        getTableData()
        return () => {
            dispatch(savePageStatus({
                key: 'list',
                status: {
                    sort: sortRef.current,
                    sortColumn: sortColumnRef.current,
                    q: searchTermRef.current,
                    page: currentPageRef.current,
                    perPage: rowsPerPageRef.current,
                    advanceRequest: advanceRequestRef.current
                }
            }))
        }
    }, [])


    // viewport handle
    useEffect(() => {
        if (offSetTop) return
        setOffSetTop(document.getElementById('main-content')?.offsetTop +
            document.getElementsByClassName('rdt_TableHeader')?.[0]?.clientHeight)
    }, [store.data.length])

    useEffect(() => {
        if (gridApi.current) {
            const agGridData = []
            store.data.forEach((x) => {
                let hoursCount = 0 
                for (let i = 1; i <= 31; i++) {
                    hoursCount += empDutyTypeList.find(item => item.dutyID === x[`day${i}`])?.hours  
                }
                const cloneData = structuredClone(x)
                cloneData.hours = hoursCount

                agGridData.push(cloneData)
            })
            setRowData(agGridData)
        
        }
    }, [store.data])


    return (
        <Fragment>
            <Card id='main-content' className='mb-0'>
                <CustomGridHeaderRep
                    handleFilter={handleFilter}
                    searchTerm={searchTerm}
                    access={access}
                    handleExcel={handleExcel}
                    handleAdd={handleAdd}
                    addLink='../WorkMgmt/ShiftSchedule/Single/add'
                    toggleSidebar={toggleSidebar}
                />
                <div
                    className="ag-theme-quartz"
                    style={{ height: `calc(100vh - ${offSetTop}px - 1rem)` }}
                >
                    <AgGridReact
                        rowData={rowData}
                        columnDefs={columnDefs}
                        rowSelection="multiple"
                        suppressRowClickSelection={true}
                        pagination={true}
                        suppressPaginationPanel={true}
                        enableServerSideSorting={true}
                        onGridReady={onGridReady}
                        onSelectionChanged={handleRowSelected}
                        onSortChanged={handleSort}
                        rowHeight={36}
                    />
                </div>
            </Card>
            <Sidebar open={sidebarOpen} toggleSidebar={toggleSidebar} t={t} setProgress={setProgress} />
            <EmpDetailModal show={empModOpen} setShow={setEmpModOpen} t={t} info={empModInfo} updData={handleUpdate}/>
        </Fragment>
    )
}

export default EmpOnDutyList
