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

import UILoader from '@components/ui-loader'
import { ShowToast } from '@CityAppExtComponents/caToaster'

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
        case 'custom':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('goodsTablePrint.stockDate', { ns: 'purchaseMgmt' }), flex: 1, field: 'stockDate' },
                { headerName: t('goodsTablePrint.stockID', { ns: 'purchaseMgmt' }), flex: 1, field: 'stockID' },
                { headerName: t('goodsTablePrint.stockStore', { ns: 'purchaseMgmt' }), flex: 1, field: 'stockStore' },
                { headerName: t('goodsTablePrint.stockFactory', { ns: 'purchaseMgmt' }), flex: 1, field: 'stockFactory' },
                { headerName: t('goodsTablePrint.variable', { ns: 'purchaseMgmt' }), flex: 1, field: 'variable' }
            ]
        case 'purchase':
            return [
                {
                    editable: true,
                    width: 40,
                    hide: !multiSelct,
                    headerName: '',
                    checkboxSelection: true,
                    headerCheckboxSelection: true
                },
                { headerName: t('goodsTablePrint.purchaseDate', { ns: 'purchaseMgmt' }), flex: 1, field: 'purchaseDate' },
                { headerName: t('goodsTablePrint.purchaseID', { ns: 'purchaseMgmt' }), flex: 1, field: 'purchaseID' },
                { headerName: t('goodsTablePrint.purchaseStore', { ns: 'purchaseMgmt' }), flex: 1, field: 'purchaseStore' },
                { headerName: t('goodsTablePrint.purchaseFactory', { ns: 'purchaseMgmt' }), flex: 1, field: 'purchaseFactory' },
                { headerName: t('goodsTablePrint.purchaseNum', { ns: 'purchaseMgmt' }), flex: 1, field: 'purchaseNum1' }
            ]
        default: return []
    }
}

const ClientQueryGoodsModal = ({ show, setShow, title, type, work, getRowDataApi, onSubmit, advanceRequest = null, multiSelct = false, initStr = '', client }) => {
    const { t } = useTranslation(['common', 'mainTableMgmt'])
    const columnDefs = useMemo(() => getColumnDefs(t, type, multiSelct), [t, type])
    const gridRef = useRef(null)
    const [rowData, setRowData] = useState([])
    const [searchTerm, setSearchTerm, searchTermRef] = useState('')
    const [currentPage, setCurrentPage, currentPageRef] = useState(1)
    const [rowsPerPage, setRowsPerPage, rowsPerPageRef] = useState(50)
    const [clientID, setClientID, clientIDRef] = useState('')
    const [total, setTotal] = useState(0)
    // const [isLoading, setIsLoading] = useState(false)

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

    const getRowData = async (req, work, api) => {
        // setIsLoading(true)
        if (api && work && req) {
            switch (work) {
                case '0':
                    await axios.post(api.endpoint, req)
                        .then(res => {
                            if (!res.error) {
                                const result = res.data.data[api.resultKey]
                                const total = res.data.data[api.totalKey]
                                setRowData(result)
                                setTotal(total)
                                // setIsLoading(false)
                            }
                        })
                    break
                case '1':
                    await axios.post(api.endpoint, req)
                        .then(res => {
                            if (!res.error) {
                                const result = res.data.data[api.resultKey]
                                const total = res.data.data[api.totalKey]
                                setRowData(result)
                                setTotal(total)
                            }
                        })
                    break
                case '2':
                    await axios.post(api.endpoint, req)
                        .then(res => {
                            if (!res.error) {
                                const result = res.data.data[api.resultKey]
                                const total = res.data.data[api.totalKey]
                                setRowData(result)
                                setTotal(total)
                            }
                        })
                    break
            }
        }
    }

    const getTableData = useCallback(async (work, getRowDataApi, columnDefs) => {
        const req = {
            sort: 'asc',
            sortColumn: columnDefs.map(col => col.field)?.[1],
            q: searchTermRef.current,
            page: currentPageRef.current,
            perPage: rowsPerPageRef.current,
            advanceRequest,
            mode: '',
            searchTerm: { field: columnDefs.map(col => col.field).filter(col => col !== undefined) },
            client: work === '0' ? clientIDRef.current : ''
        }
        const workState = work
        const api = getRowDataApi
        getRowData(req, workState, api)
    }, [JSON.stringify(advanceRequest || '')])


    useUpdateEffect(() => getTableData(work, getRowDataApi, columnDefs), [currentPage])
    useDebounce(() => { currentPage === 1 ? getTableData(work, getRowDataApi, columnDefs) : setCurrentPage(1) }, 600, [searchTerm])

    useEffect(() => {
        if (work === '0' && clientID && show) {
            if (show) {
                if (clientID && clientID !== '') {
                    getTableData(work, getRowDataApi, columnDefs)
                } else {
                    ShowToast('查詢失敗', '請先選擇店櫃', 'danger')
                }
            }
        } else if (work === '1' && clientID && show) {
            getTableData(work, getRowDataApi, columnDefs)
        } else if (work === '2' && clientID && show) {
            getTableData(work, getRowDataApi, columnDefs)
        }
    }, [show])

    useEffect(() => {
        if (!show) {
            setSearchTerm('')
            setCurrentPage(1)
        }
    }, [show])

    // useUpdateEffect(() => {
    //     if (show) {
    //         setSearchTerm(initStr)
    //     }
    // }, [show])

    useEffect(() => {
        if (client) {
            setClientID(client)
        }
    }, [client])

    return (
        <Fragment>
            {/* <UILoader blocking={isLoading} classname='cityapp full-screen-uiloader'> */}
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
                                rowSelection={"multiple"}
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
            {/* </UILoader> */}
        </Fragment>
    )
}

export default ClientQueryGoodsModal
