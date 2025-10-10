// ** React Imports
import React, { Fragment, useEffect, useCallback, useMemo } from 'react'


// ** Table Columns
import { ColumnDefs, ColumnExport, ColumnFilter } from './columns'

// ** Store & Actions
import { getData, deleteUser, editUser, savePageStatus } from '../store'
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
import { exportToExcel, confirmDelete } from '@CityAppHelper'
import useDebounce from '@hooks/useDebounce'
import useUpdateEffect from '@hooks/useUpdateEffect'
import CustomGridHeader from '@application/component/CustomGridHeader'
import CustomGridPagination from '@application/component/CustomGridPagination'


const UsersList = ({ access, t }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const store = useSelector(state => state.SettingMgmt_UserAccount)
    const status = store.status.list


    // ** States
    const [sort, setSort, sortRef] = useState(status.list?.sort || 'asc')
    const [searchTerm, setSearchTerm, searchTermRef] = useState(status?.q || '')
    const [currentPage, setCurrentPage, currentPageRef] = useState(status?.page || 1)
    const [sortColumn, setSortColumn, sortColumnRef] = useState(status?.sortColumn || 'userName')
    const [rowsPerPage, setRowsPerPage, rowsPerPageRef] = useState(Cookies.get('rowsPerPage') || 10)
    const [advanceRequest, setAdvanceRequest, advanceRequestRef] = useState(status?.advanceRequest || null)

    const [selectedRows, setSelectedRows, selectedRowsRef] = useState([])
    const [offSetTop, setOffSetTop] = useState(0)

    // ** Initial Defs
    const columnDefs = useMemo(() => {
        return ColumnDefs({ access, t, setCurrentPage })
    }, [access, t])


    const columnFilter = useMemo(() => {
        return ColumnFilter({ t })
    }, [t])

    const columnExport = useMemo(() => {
        return ColumnExport({ t })
    }, [t])

    const getTableData = useCallback(async (params) => {
        let result
        await dispatch(
            getData({
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
        setSearchTerm(val)
    }, [])

    // ** Function in get data on advance search query change
    const handleAdvanceSearch = useCallback(val => {
        setAdvanceRequest(val)
    }, [])

    const handleDelete = useCallback(() => {
        confirmDelete(() => {
            const delList = { delList: selectedRowsRef.current.map(_row => ({ userID: _row.userID })) }
            dispatch(deleteUser(delList))
        })
    }, [])

    const handleExcel = useCallback(async () => {
        const result = await getTableData({ mode: 'all' })
        if (result.error) return
        const sheetData = result.payload.data.userList

        const fileName = t('userAccount.title', { ns: 'settingMgmt' })
        exportToExcel([{ sheetData, sheetName: fileName }], fileName, columnExport)

    }, [columnExport])

    const handleSelectedExcel = useCallback(() => {
        const sheetData = selectedRowsRef.current
        const fileName = t('userAccount.title', { ns: 'settingMgmt' })
        exportToExcel([{ sheetData, sheetName: fileName }], fileName, columnExport)
    }, [columnExport])

    const onGridReady = useCallback((params) => {
        params.api?.applyColumnState({
            state: [{ colId: sortColumn, sort }],
            defaultState: { sort: null }
        })
    }, [])

    //status handle
    useEffect(() => {
        if (!store.status.search || store.changeFlag) getTableData()

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
            document.getElementsByClassName('rdt_TableHeader')?.[0]?.clientHeight +
            document.getElementsByClassName('rdt_TableFooter')?.[0]?.clientHeight)
    }, [store.data.length])

    return (
        <Fragment>
            <Card id='main-content' className='mb-0'>
                <CustomGridHeader
                    handleFilter={handleFilter}
                    searchTerm={searchTerm}
                    access={access}
                    handleExcel={handleExcel}
                    handleAdd={null}
                    addLink='../SettingMgmt/UserAccount/Single/add'
                    columnFilter={columnFilter}
                    advanceRequest={advanceRequest}
                    handleAdvanceSearch={handleAdvanceSearch}
                    selectedRowsLength={selectedRows.length}
                    handleDelete={handleDelete}
                    handleSelectedExcel={handleSelectedExcel}
                />
                <div
                    className="ag-theme-quartz"
                    style={{ height: `calc(100vh - ${offSetTop}px - 1rem)` }}
                >
                    <AgGridReact
                        rowData={store.data}
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
                    total={store.total}
                    rowsPerPage={rowsPerPage}
                    handlePerPage={handlePerPage}
                    handlePagination={handlePagination}
                    currentPage={currentPage}
                />

            </Card>
        </Fragment>
    )
}

export default UsersList
