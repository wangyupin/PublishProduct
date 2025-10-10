// ** React Imports
import React, { Fragment, useEffect, useCallback, useMemo } from 'react'
import { Link } from 'react-router-dom'

// ** Table Columns
import { ColumnDefs, ColumnExport, ColumnFilter } from './columns'

// ** Store & Actions
import { savePageStatus, editEmpDuty, getEmpDutyList, delSelectData, toggleIsLoading, saveSearchTerm } from '../store'
import { useDispatch, useSelector } from 'react-redux'

// ** Third Party Components
import useState from 'react-usestateref'
import { AgGridReact } from 'ag-grid-react'
import Cookies from 'js-cookie'
import { Box, Layers, DollarSign, X, CreditCard, Smartphone, LogOut, Share, Grid, Search, MoreHorizontal} from 'react-feather'


// ** Reactstrap Imports
import { Card, Input, Button, Row, Col, UncontrolledDropdown, DropdownItem, DropdownToggle, DropdownMenu, InputGroup, InputGroupText  } from 'reactstrap'

// ** Styles
import '@styles/react/libs/tables/react-dataTable-component.scss'

// ** Helper
import { exportToExcel } from '@CityAppHelper'
import useDebounce from '@hooks/useDebounce'
import useUpdateEffect from '@hooks/useUpdateEffect'
import CustomGridHeader from '@application/component/CustomGridHeader'
import CustomGridPagination from '@application/component/CustomGridPagination'

// Sale Store
import { getMemberDetail, selectMember } from '../../../SaleMgmt/Sale/store'

import AdvanceSearch from '@application/component/AdvanceSearch'

const MemberList = ({ access, t }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const store = useSelector(state => state.WorkMgmt_EmpDutyTypeSetUp)
    const { selectedDuty, num } = store
    const rowData = store.data
    const status = store.status.list
    

    // ** States
    const [sort, setSort, sortRef] = useState(status?.sort || 'asc')
    const [searchTerm, setSearchTerm, searchTermRef] = useState(store.searchTerm || '')
    const [currentPage, setCurrentPage, currentPageRef] = useState(status?.page || 1)
    const [sortColumn, setSortColumn, sortColumnRef] = useState(status?.sortColumn || 'dutyID')
    const [rowsPerPage, setRowsPerPage, rowsPerPageRef] = useState(Cookies.get('rowsPerPage') || 10)
    const [advanceRequest, setAdvanceRequest, advanceRequestRef] = useState(status?.advanceRequest || null)
    const [selectedRows, setSelectedRows, selectedRowsRef] = useState([])
    const [offSetTop, setOffSetTop] = useState(0)
    const [show, setShow] = useState(false)
    const [changeFlag, setChangFlag] = useState(false)

    // ** Initial Defs
    const columnDefs = useMemo(() => {
        return ColumnDefs({ access, t, setCurrentPage, setChangFlag })
    }, [access, t])


    const columnFilter = useMemo(() => {
        return ColumnFilter({ t })
    }, [t])

    const columnExport = useMemo(() => {
        return ColumnExport({ t })
    }, [t])

    const getTableData = useCallback(async (params) => {
        dispatch(toggleIsLoading())
        let result
        await dispatch(
            getEmpDutyList({
                sort: sortRef.current,
                sortColumn: sortColumnRef.current,
                q: searchTermRef.current,
                page: currentPageRef.current,
                perPage: rowsPerPageRef.current,
                advanceRequest: advanceRequestRef.current,
                mode: params?.mode || ''
            })
        ).then(res => {
            result = res
            setChangFlag(false)
        })
        return result
    }, [dispatch])
    useUpdateEffect(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, [advanceRequest])
    useUpdateEffect(() => store.changeFlag && getTableData(), [store.changeFlag])
    useDebounce(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, 600, [searchTerm])
    useDebounce(() => getTableData(), 50, [currentPage, rowsPerPage, sort, sortColumn])

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

    // ** Function in get data on page change
    const handlePagination = useCallback(page => {
        setCurrentPage(page.selected + 1)
    }, [])

    // ** Function in get data on rows per page
    const handlePerPage = useCallback(e => {
        const value = parseInt(e.currentTarget.value)
        setRowsPerPage(value)
        setCurrentPage(1)
        value && Cookies.set('rowsPerPage', value)
    }, [])

    // ** Function in get data on search query change
    const handleFilter = useCallback(val => {
        dispatch(saveSearchTerm(val))
        setSearchTerm(val)
    }, [])

    // ** Function in get data on advance search query change
    const handleAdvanceSearch = useCallback(val => {
        setAdvanceRequest(val)
    }, [])

    const handleExcel = useCallback(async () => {
        const result = await getTableData({ mode: 'all' })
        if (result.error) return
        const sheetData = result.payload.data.result

        const fileName = t('employeeDuty.title', { ns: 'workMgmt' })
        exportToExcel([{ sheetData, sheetName: fileName }], fileName, columnExport)

    }, [columnExport])

    const handleDelete = useCallback(() => {
        const delList = { delList: selectedRowsRef.current.map(_row => ({ dutyID: _row.dutyID })) }
        dispatch(delSelectData(delList))
            .then(() => setChangFlag(true))
    }, [])

    const handleAdd = useCallback(() => {
        dispatch(editEmpDuty(null))
    }, [])

    const handleSelectedExcel = useCallback(() => {
        const sheetData = selectedRowsRef.current
        const fileName = t('employeeDuty.title', { ns: 'workMgmt' })
        exportToExcel([{ sheetData, sheetName: fileName }], fileName, columnExport)
    }, [columnExport])

    const onGridReady = useCallback((params) => {
        params.api?.applyColumnState({
            state: [{ colId: sortColumn, sort }],
            defaultState: { sort: null }
        })
    }, [])

    const onCellValueChanged = useCallback((params) => {
        const colId = params.column.getId
    })

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

    useEffect(() => {
        if (changeFlag) {
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
        }
    }, [changeFlag])

    // viewport handle
    useEffect(() => {
        if (offSetTop) return
        setOffSetTop(document.getElementById('main-content')?.offsetTop +
            document.getElementsByClassName('rdt_TableHeader')?.[0]?.clientHeight +
            document.getElementsByClassName('rdt_TableFooter')?.[0]?.clientHeight)
    }, [store.data.length])

    return (
        <Fragment>
            <Card id='main-content'>
                {/* <div className='position-relative rdt_TableHeader' >
                    <div style={{ backgroundColor: "#fff", borderRadius: '4px 4px 0 0' }} >
                        <Row className='d-flex justify-content-end ms-1 me-1 pt-2 pb-2'>
                            <Col sm='6' className='d-flex align-items-center justify-content-sm-end justify-content-start flex-sm-nowrap p-0 mt-sm-0 mt-1 ms-sm-0 ms-1'>
                                <div className='d-flex'>
                                    {
                                        (() => {
                                            if (access.create) {
                                                return (
                                                <Link to={'../WorkMgmt/EmpDutyTypeSetUp/Single/add'}>
                                                    <Button className='add-new-user me-1' color='primary' onClick={() => dispatch(handleAdd)}>
                                                        {t('add', { ns: 'common' })}
                                                    </Button>
                                                </Link>
                                                )
                                            }
                                        })()
                                    }
                                </div>
                            </Col>
                        </Row>
                        <AdvanceSearch show={show} setShow={setShow} columnFilter={columnFilter} advanceRequest={advanceRequest} handleAdvanceSearch={handleAdvanceSearch} />
                    </div>
                </div> */}
                <CustomGridHeader
                    handleFilter={handleFilter}
                    searchTerm={store.searchTerm}
                    access={access}
                    handleExcel={handleExcel}
                    handleAdd={handleAdd}
                    addLink='../WorkMgmt/EmpDutyTypeSetUp/Single/add'
                    columnFilter={columnFilter}
                    advanceRequest={advanceRequest}
                    handleAdvanceSearch={handleAdvanceSearch}
                    selectedRowsLength={selectedRows.length}
                    handleDelete={handleDelete}
                    handleSelectedExcel={handleSelectedExcel}
                />
                <div
                    className="ag-theme-quartz"
                    style={{ height: `calc(88vh - ${offSetTop}px - 1rem)` }}
                >
                    <AgGridReact  style={{ overflowX: 'hidden' }} 
                            rowData={rowData}
                            columnDefs={columnDefs}
                            rowSelection="multiple"
                            suppressRowClickSelection={true}
                            pagination={true}
                            suppressPaginationPanel={true}
                            enableServerSideSorting={true}
                            onSelectionChanged={handleRowSelected}
                            onSortChanged={handleSort}
                            // onCellValueChanged={onCellValueChanged}
                            rowHeight={36}
                        />
                    </div>
                <CustomGridPagination
                    total={num}
                    rowsPerPage={rowsPerPage}
                    handlePerPage={handlePerPage}
                    handlePagination={handlePagination}
                    currentPage={currentPage}
                />
            </Card>
        </Fragment>
    )
}

export default MemberList
