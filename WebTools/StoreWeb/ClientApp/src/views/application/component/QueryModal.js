// ** React Imports
import ReactDOM from 'react-dom'
import React, { Fragment, useEffect, useCallback, useRef, forwardRef, useImperativeHandle, useMemo } from 'react'
import useState from 'react-usestateref'

// ** Reactstrap Imports
import {
    Card, CardBody, CardText, Row, Col,
    Form, Input, InputGroup, InputGroupText, Label,
    Button, ButtonGroup,
    Modal, ModalBody, ModalHeader, ListGroup, CardHeader, CardTitle
} from 'reactstrap'

// ** Ag-grid
import { AgGridReact, AgGridColumn } from 'ag-grid-react'


// ** CityApp Utilty
import { ShowToast } from '@CityAppExtComponents/caToaster'
import { CaQueryLoader } from '@CityAppComponents'
import {
    getLikeFromToCondition, showMessageBox, HotKeyController, getFormatedDateForInput, getFullDate, getFormatedMonthForInput, exportToExcel
} from '@CityAppHelper'

// ** Store & Actions
import { useSelector, useDispatch } from 'react-redux'

const CommonQueryModal = React.memo(({ viewTitle, open, toggle, parentKeyDownHandler, storeObject, elementList, resetStore, getQueryPanelData, getQueryModalData, id, updQueryModalData, ColumnDefList, storeIsLoading, queryData = null }) => {
    const dispatch = useDispatch()
    const layoutStoreSkin = useSelector(state => state.layout).skin

    const InputGroupTextClass = 'border-0 fw-bolder fs-5 ch-10'
    const InputStartClass = 'form-control text-truncate fw-bolder fs-5 rounded-start'
    const InputEndClass = 'form-control text-truncate fw-bolder fs-5'
    const InputSize = 'sm'

    // ** Hooks
    const gridRef = useRef()

    // ** function button events
    const performButtonRef = useRef(null)
    const resetButtonRef = useRef(null)
    const closeButtonRef = useRef(null)
    const queryButtonRef = useRef(null)

    // ** state
    const currentDate = getFormatedDateForInput(new Date())
    const currentMonth = getFormatedMonthForInput()

    const [state, setState] = useState({})

    const [selectedRow, setSelectedRow] = useState(null)
    const [gridState, setGridState, gridStateRef] = useState(null)


    const getOriginalState = useCallback(() => {
        const result = new Object()
        elementList.forEach(element => {
            result[`${element.fieldName}_ST`] = element.ST ? element.ST : element.type === 'number' ? 0 : ''
            result[`${element.fieldName}_ED`] = element.ED ? element.ED : element.type === 'number' ? 999999 : element.type === 'date' ? currentDate : ''
        })
        return result
    }, [elementList])


    const resetGridState = () => {
        gridRef.current.columnApi.resetColumnState()
        setGridState(gridRef.current.columnApi.getColumnState())
    }

    const onInputChange = (e) => {
        const { id, type } = e.target
        let { value } = e.target
        if (type === 'number') {
            value ? value = parseFloat(value) : value = 0
        }
        setState({
            ...state,
            [id]: value
        })
    }

    const onCloseButtonClickEvent = () => {
        toggle()
    }

    const onResetButtonClickEvent = () => {
        resetStore()
        setState(queryData === null ? getOriginalState(elementList) : queryData)

        ShowToast(viewTitle, '清除完成', 'info')
    }

    const onPerformButtonClickEvent = () => {
        if (selectedRow === null) {
            ShowToast(viewTitle, '尚未選擇資料', 'info')
        } else {
            getQueryPanelData(selectedRow[id])
            setSelectedRow(null)
            toggle()
        }
    }

    const onQueryButtonClickEvent = () => {
        let request = new Object()
        elementList.map(element => {
            const from = element.type === 'date' && state[`${element.fieldName}_ST`] === '' ? '00000101' : element.type === 'date' ? state[`${element.fieldName}_ST`].replace(/-/g, "") : state[`${element.fieldName}_ST`]
            const to = element.type === 'date' ? state[`${element.fieldName}_ED`].replace(/-/g, "") : state[`${element.fieldName}_ED`]
            return (getLikeFromToCondition(from, to, element.fieldName))
        }).forEach(obj => {
            if (Object.keys(obj).length > 0) {
                request = { ...request, ...obj }
            }

        })
        getQueryModalData(request)
        resetGridState()
    }

    const onExportButtonClickEvent = (e) => {
        exportToExcel([{ sheetData: storeObject, sheetName: viewTitle }], viewTitle, ColumnDefList)
    }

    // ** keyboard settings
    const currentKBRef = useRef(null)

    const onRowDoubleClickedEvent = useCallback((params) => {
        getQueryPanelData(params.data[id])
        toggle()
    }, [open])

    const onSelectionChanged = useCallback(() => {
        setSelectedRow(gridRef.current.api.getSelectedRows()[0])
    }, [gridRef.current?.api])

    const onSortChanged = useCallback(() => {
        const rowData = []
        gridRef.current.api.forEachNodeAfterFilterAndSort(node => rowData.push(node.data))
        updQueryModalData(rowData)
        setGridState(gridRef.current.columnApi.getColumnState())
    }, [gridRef.current?.api, gridRef.current?.columnApi])

    const onGridReady = useCallback((params) => {
        gridRef.current = params
        params.columnApi.applyColumnState({ state: gridStateRef.current })
    }, [])

    useEffect(() => {
        if (open) {
            currentKBRef.current = {
                hotkeyDefList: [
                    { key: 'F8', callback: () => ReactDOM.findDOMNode(queryButtonRef.current).click() }, //查詢[F3]
                    { key: 'F3', callback: () => ReactDOM.findDOMNode(resetButtonRef.current).click() }, //清除[F4]
                    { key: 'Escape', callback: () => onCloseButtonClickEvent() } //結束
                ],
                lastInputEnterEvent: () => (console.log(`lastInputEnterEvent`))
            }
        }
    }, [open])

    const keyDownHandler = useCallback((event) => {
        HotKeyController(event, currentKBRef.current.lastInputEnterEvent, currentKBRef.current.inputLoopDefList, currentKBRef.current.hotkeyDefList)
    }, [])

    const backupKeyDownHandlerRef = useRef(null)
    useEffect(() => {
        if (open && !backupKeyDownHandlerRef.current) {
            backupKeyDownHandlerRef.current = parentKeyDownHandler
            document.removeEventListener("keydown", backupKeyDownHandlerRef.current)
            document.addEventListener("keydown", keyDownHandler)
        } else {
            if (backupKeyDownHandlerRef.current) {
                backupKeyDownHandlerRef.current = null
                document.removeEventListener("keydown", keyDownHandler)
                document.addEventListener("keydown", parentKeyDownHandler)
            }
        }
    }, [open])

    useEffect(() => {
        setState(queryData === null ? getOriginalState(elementList) : queryData)
    }, [queryData])

    const Content = useMemo(() => {
        return (
            <div style={{ height: 'calc(70vh)' }} className={`p-0 cityapp ag-theme-alpine${layoutStoreSkin !== 'light' ? '-dark' : ''} `} >
                <AgGridReact
                    columnDefs={ColumnDefList}
                    rowSelection='single'
                    onSelectionChanged={onSelectionChanged}
                    onRowDoubleClicked={onRowDoubleClickedEvent}
                    onSortChanged={onSortChanged}
                    onGridReady={onGridReady}
                    defaultColDef={{
                        resizable: true, sortable: true, suppressSizeToFit: true, width: 150, cellStyle: { textAlign: 'center' }, resizable: true, autoHeaderHeight: true, wrapHeaderText: true
                    }}
                    rowData={storeObject}
                    overlayNoRowsTemplate=
                    '<span class="fs-3 text-primary" >請先設定查詢條件，再按「執行[F8]」</span>'
                    headerHeight={30}
                    rowHeight={30}
                >
                </AgGridReact>
            </div>
        )
    }, [ColumnDefList, storeObject])

    return !(open) ? null : (
        <Fragment>
            <Modal scrollable isOpen={open} toggle={toggle} className='modal-dialog-centered modal-xl' backdrop={false} autoFocus={false}>
                <ModalHeader className='bg-opacity-25 rounded bg-primary fs-3' toggle={toggle} tag='div'>
                    <span className='modal-title fs-3' style={{ width: '90%' }}>{viewTitle}</span>
                    <span className='modal-title fs-3 ms-auto' style={{ width: '90%' }}>
                    </span>
                </ModalHeader>

                <ModalBody>
                    <Row >
                        <Col md='5' className='mb-1' >
                            <Card className='overflow-auto' style={{ height: '65vh' }}>
                                <CardHeader className='bg-opacity-25 bg-dark' style={{ padding: 10, marginBottom: 10 }}>
                                    <CardTitle className='fs-5'>查詢條件</CardTitle>
                                </CardHeader>
                                <CardBody style={{ padding: 8 }}>
                                    <Form>
                                        {elementList.map(element => {
                                            return (
                                                <Row className='g-1' style={{ marginBottom: 8 }} key={element.fieldName}>
                                                    <InputGroup size={InputSize}>
                                                        <InputGroupText className={InputGroupTextClass}>{element.labelName}</InputGroupText>
                                                        <Input className={InputStartClass} type={element.type || 'text'} autoComplete="off" id={`${element.fieldName}_ST`} value={state[`${element.fieldName}_ST`]} onChange={onInputChange} step={element.step} />
                                                        <Input className={InputEndClass} type={element.type || 'text'} autoComplete="off" id={`${element.fieldName}_ED`} value={state[`${element.fieldName}_ED`]} onChange={onInputChange} step={element.step} />
                                                    </InputGroup>
                                                </Row>
                                            )
                                        })}
                                    </Form>
                                </CardBody>
                            </Card>
                            <Row>
                                <Col className='d-flex flex-row-reverse'>
                                    <Button ref={queryButtonRef}
                                        className='fw-bolder fs-5' style={{ marginLeft: 5, padding: 10 }} color='info' onClick={onQueryButtonClickEvent} >查詢[F8]</Button>
                                    <Button ref={resetButtonRef}
                                        className='fw-bolder fs-5' style={{ marginLeft: 5, padding: 10 }} color='secondary' onClick={onResetButtonClickEvent}>清空[F3]</Button>
                                    <Button
                                        className='fw-bolder fs-5' style={{ marginLeft: 5, padding: 10 }} color='info' onClick={onExportButtonClickEvent} outline>Excel</Button>
                                </Col>
                            </Row>
                        </Col>
                        <Col md='7'>
                            <CaQueryLoader blocking={storeIsLoading} content={
                                Content
                            } />
                        </Col>
                    </Row>
                </ModalBody>

            </Modal>
        </Fragment>
    )
})

export default CommonQueryModal