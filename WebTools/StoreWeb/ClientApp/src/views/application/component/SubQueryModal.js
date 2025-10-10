// ** React Imports
import { Fragment, useEffect, useRef, useMemo, useCallback } from 'react'

// ** Reactstrap Imports
import {
    Card, CardBody, CardText, Row,
    Col, Input, InputGroup, InputGroupText, Label,
    Button,
    Modal, ModalBody, ModalHeader, ModalFooter
} from 'reactstrap'

// ** Ag-grid
import { AgGridReact } from 'ag-grid-react'

// ** Third Party Components
import { useTranslation } from 'react-i18next'
import Cookies from 'js-cookie'
import axios from 'axios'

// ** CityApp Utilty
import { arrayFilterByValue } from '@CityAppHelper'
import useState from 'react-usestateref'

// ** Store & Actions
import { useSelector, use } from 'react-redux'

import CustomGridPagination from '@application/component/CustomGridPagination'
import useUpdateEffect from '@hooks/useUpdateEffect'
import useDebounce from '@hooks/useDebounce'

const getColumnDefs = (t, type, multiSelct) => {
    switch (type) {
        case 'goods':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('goodsInfo.goodID', { ns: 'mainTableMgmt' }), flex: 1, field: 'goodID' },
                { headerName: t('goodsInfo.goodName', { ns: 'mainTableMgmt' }), flex: 1, field: 'goodName' },
                { headerName: t('goodsInfo.brand', { ns: 'mainTableMgmt' }), flex: 1, field: 'brandName' }
            ]
        case 'factory':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('factoryInfo.factoryID', { ns: 'mainTableMgmt' }), flex: 1, field: 'factoryID' },
                { headerName: t('factoryInfo.factoryShort', { ns: 'mainTableMgmt' }), flex: 1, field: 'factoryShort' }
            ]
        case 'store':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('client.clientID', { ns: 'mainTableMgmt' }), flex: 1, field: 'clientID' },
                { headerName: t('client.clientShort', { ns: 'mainTableMgmt' }), flex: 1, field: 'clientShort' }
            ]
        case 'order':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('order.orderID', { ns: 'orderMgmt' }), flex: 1, field: 'orderID' },
                { headerName: t('order.orderDate', { ns: 'orderMgmt' }), flex: 1, field: 'orderDate' }
            ]
        case 'brand':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('brandInfo.brandID', { ns: 'mainTableMgmt' }), flex: 1, field: 'brandID' },
                { headerName: t('brandInfo.brandName', { ns: 'mainTableMgmt' }), flex: 1, field: 'brandName' }
            ]
        // 款式
        case 'shape':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('goodsInfo.styleRemark', { ns: 'mainTableMgmt' }), flex: 1, field: 'styleRemark' },
                { headerName: t('goodsInfo.goodName', { ns: 'mainTableMgmt' }), flex: 1, field: 'goodName' },
                { headerName: t('goodsInfo.brand', { ns: 'mainTableMgmt' }), flex: 1, field: 'brandName' }
            ]
        default: return []
    }
}

const SubQueryModal = ({ show, setShow, title, type, getRowDataApi, onSubmit, advanceRequest = null, multiSelct = false, initStr = '' }) => {
    const { t } = useTranslation(['common', 'mainTableMgmt', 'orderMgmt'])

    const columnDefs = useMemo(() => getColumnDefs(t, type, multiSelct), [t, type])
    const gridRef = useRef(null)
    const [rowData, setRowData] = useState([])
    const [searchTerm, setSearchTerm, searchTermRef] = useState('')
    const [currentPage, setCurrentPage, currentPageRef] = useState(1)
    const [rowsPerPage, setRowsPerPage, rowsPerPageRef] = useState(50)
    const [total, setTotal] = useState(0)

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


    const onConfirmBtnAdd = () => {
        if (gridRef.current.api.getSelectedRows().length > 0) {
            const data = multiSelct ? gridRef.current.api.getSelectedRows() : gridRef.current.api.getSelectedRows()[0]
            onSubmit(data)
            setShow(!show)
        }
    }
    const onLeaveBtnClick = () => {
        setShow(!show)
    }

    const getRowData = async (params) => {
        await axios.post(getRowDataApi.endpoint, params)
            .then(res => {
                if (!res.error) {
                    const result = res.data.data[getRowDataApi.resultKey]
                    const total = res.data.data[getRowDataApi.totalKey]
                    setRowData(result)
                    setTotal(total)
                }
            })
            .catch(err => {

            })
    }

    const getTableData = useCallback(async () => {
        getRowData({
            sort: 'asc',
            sortColumn: columnDefs.map(col => col.field)?.[1],
            q: searchTermRef.current,
            page: currentPageRef.current,
            perPage: rowsPerPageRef.current,
            advanceRequest,
            mode: '',
            searchTerm: { field: columnDefs.map(col => col.field).filter(col => col !== undefined) }
        })
    }, [JSON.stringify(advanceRequest || ''), columnDefs])

    useEffect(() => {
        if (show) getTableData()
    }, [show])

    useUpdateEffect(() => getTableData(), [currentPage])
    useDebounce(() => { currentPage === 1 ? getTableData() : setCurrentPage(1) }, 600, [searchTerm])

    useEffect(() => {
        if (!show) {
            setSearchTerm('')
            setCurrentPage(1)
        }
    }, [show])

    useUpdateEffect(() => {
        if (show) {
            setSearchTerm(initStr)
        }
    }, [show])

    return (
        <Fragment>
            <Modal
                isOpen={show}
                toggle={() => setShow(!show)}
                className='modal-dialog-centered modal-lg'
                autoFocus={false}
            >
                <ModalHeader className='bg-transparent'></ModalHeader>
                <ModalBody className='px-2' style={{ overflow: 'auto', height: '65vh' }}>
                    <div className='text-center mb-1'>
                        <h1>{title}</h1>
                    </div>
                    <Row className='mb-1'>
                        <Col>
                            <Input size='sm' id='searchTerm' value={searchTerm}
                                autoComplete='off'
                                placeholder={t('searchPlaceholder', { ns: 'common' })}
                                onFocus={e => e.target.select()}
                                onChange={e => {
                                    setSearchTerm(e.target.value)
                                }}
                                autoFocus={true}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <div
                            className="ag-theme-quartz ag-hover-active"
                            style={{ height: `54vh` }}
                        >
                            <AgGridReact
                                ref={gridRef}
                                rowData={rowData}
                                columnDefs={columnDefs}
                                defaultColDef={{
                                    sortable: false
                                }}
                                rowHeight={36}
                                overlayNoRowsTemplate={t('agGridNoRowText', { ns: 'common' })}
                                rowSelection={multiSelct ? "multiple" : "single"}
                                onRowDoubleClicked={onConfirmBtnAdd}
                            />
                        </div>

                    </Row>
                </ModalBody>
                <ModalFooter className='m-0 p-0'>
                    <Row className='w-100 align-items-center'>
                        <Col xs={9} className='p-0'>
                            <CustomGridPagination
                                total={total}
                                rowsPerPage={rowsPerPage}
                                handlePerPage={handlePerPage}
                                handlePagination={handlePagination}
                                currentPage={currentPage}
                                rowPerPageShow={false}
                            />
                        </Col>
                        <Col xs={3} className='ps-0 pe-2 d-flex justify-content-end'>
                            <Button.Ripple color='primary' className='me-1' onClick={onConfirmBtnAdd}>
                                {t('confirm', { ns: 'common' })}
                            </Button.Ripple>
                            <Button.Ripple color='flat-secondary' onClick={onLeaveBtnClick}>
                                {t('leave', { ns: 'common' })}
                            </Button.Ripple>
                        </Col>
                    </Row>
                </ModalFooter>
            </Modal>
        </Fragment>
    )
}

export default SubQueryModal
