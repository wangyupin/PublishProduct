// ** React Imports
import React, { Fragment, useEffect, useCallback, useMemo } from 'react'


// ** Table Columns
import { ColumnDefs, ColumnExport, ColumnFilter } from '../columns'

// ** Store & Actions
import { getData, editPunchJob, savePageStatus, getFilteredData } from '../store'
import { useDispatch, useSelector } from 'react-redux'
import Sidebar from './Sidebar'

// ** Third Party Components
import useState from 'react-usestateref'
import { AgGridReact } from 'ag-grid-react'
import Cookies from 'js-cookie'


// ** Reactstrap Imports
import { Card, Row, Col, Button } from 'reactstrap'

// ** Styles
import '@styles/react/libs/tables/react-dataTable-component.scss'

// ** Helper
import { exportToExcel } from '@CityAppHelper'
import useDebounce from '@hooks/useDebounce'
import useUpdateEffect from '@hooks/useUpdateEffect'
import CustomGridHeaderRep from '@application/component/CustomGridHeaderRep'


const UsersList = ({ access, t, setProgress, isFront }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const store = useSelector(state => state.WorkMgmt_PunchJobEdit)
    const { filtered, original } = store.data
    const tableStatus = store.status.table

    // ** States
    const [searchTerm, setSearchTerm, searchTermRef] = useState(tableStatus?.q || '')
    const [offSetTop, setOffSetTop] = useState(0)
    const [sidebarOpen, setSidebarOpen] = useState(false)
    const toggleSidebar = () => setSidebarOpen(!sidebarOpen)

    const columnDefs = useMemo(() => {
        return ColumnDefs({ access, t })
    }, [access, t])


    const getTableData = useCallback(async () => {
        await dispatch(getData({ isFront }))
    }, [dispatch])

    useUpdateEffect(() => store.changeFlag && getTableData(), [store.changeFlag])

    useDebounce(() => dispatch(getFilteredData({ searchTerm, t })), 600, [searchTerm])
    useUpdateEffect(() => dispatch(getFilteredData({ searchTerm, t })), [original])

    const handleFilter = useCallback(val => {
        setSearchTerm(val)
    }, [])

    const handleExcel = () => {
        const sheetName = t('punchJobEdit.title', { ns: 'workMgmt' })
        const ColumnDefList = ColumnExport({ t })
        exportToExcel([{ sheetData: filtered, sheetName }], sheetName, ColumnDefList)
    }

    const onGridPreDestroyed = useCallback((params) => {
        const { state } = params
        dispatch(savePageStatus({
            key: 'table',
            status: {
                q: searchTermRef.current,
                gridState: state
            }
        }))
    }, [])

    //status handle
    useEffect(() => {
        getTableData()
    }, [])


    // viewport handle
    useEffect(() => {
        if (offSetTop) return
        setOffSetTop(document.getElementById('main-content')?.offsetTop + document.getElementsByClassName('rdt_TableHeader')?.[0]?.clientHeight)
    }, [store.data.length])

    return (
        <Fragment>
            <Card id='main-content' className='mb-0'>
                <CustomGridHeaderRep
                    handleFilter={handleFilter}
                    searchTerm={searchTerm}
                    access={access}
                    handleExcel={handleExcel}
                    handleAdd={null}
                    addLink='../WorkMgmt/PunchJobEdit/Single/add'
                    toggleSidebar={toggleSidebar}
                />
                <div
                    className="ag-theme-quartz"
                    style={{ height: `calc(100vh - ${offSetTop}px - 1rem)` }}
                >
                    <AgGridReact
                        rowData={filtered}
                        columnDefs={columnDefs}
                        onGridPreDestroyed={onGridPreDestroyed}
                        initialState={tableStatus.gridState}
                        overlayNoRowsTemplate={t('agGridNoRowText', { ns: 'common' })}
                        rowHeight={36}
                    />
                </div>
            </Card>
            <Sidebar open={sidebarOpen} toggleSidebar={toggleSidebar} t={t} setProgress={setProgress} isFront={isFront} />
        </Fragment>
    )
}

export default UsersList
