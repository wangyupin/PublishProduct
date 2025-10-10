/*eslint-disable */
// ** React Imports
import ReactDOM from 'react-dom'
import React, { Fragment, useEffect, useCallback, useRef, forwardRef, useImperativeHandle, useMemo, useState } from 'react'
import { useLocation } from 'react-router-dom'

// ** Reactstrap Imports
import { Card, CardBody, Row, Col, Button, Modal, ModalBody, ModalHeader, ModalFooter } from 'reactstrap'
import { ChevronRight, ChevronsRight, ChevronLeft, ChevronsLeft, ChevronUp, ChevronsUp, ChevronDown, ChevronsDown } from 'react-feather'

// ** CityApp Utilty
import { showMessageBox } from '@CityAppHelper'

// ** Store & Actions
import { useDispatch } from 'react-redux'
import DualListBox from 'react-dual-listbox'

// ** Third Party Components
import Cookies from 'js-cookie'

const DualListModal = React.memo(({ options, open, toggle, saveSelectedColumn, columnDefs, defaultSelectedColumn = [] }) => {
    const dispatch = useDispatch()
    const location = useLocation().pathname
    const cookieKey = location.split('/')[2]
    const cookie = Cookies.get('SelectedMemo')

    // ** Hooks
    const selectedRef = useRef(null)

    // ** state
    const defaultSelected = () => {
        const optionsCopy = options.slice()

        const defaultColumn = defaultSelectedColumn?.map(x => x.value)
        const canSelectedColumn = options?.[2]?.options?.map(x => x.value)

        const disabled = optionsCopy.filter(x => x.disabled === true)
        if (cookie && JSON.parse(cookie)[cookieKey]) {
            const memo = JSON.parse(cookie)[cookieKey]
            const selectedCol = {
                label: '自訂',
                options: memo
            }
            const result = [...disabled, selectedCol]
            return result
        } else {
            const selectedCol = {
                label: '自訂',
                options: columnDefs.filter(col => defaultColumn?.indexOf(col.value) >= 0)
            }
            const result = [...disabled, selectedCol]
            return result
        }
    }
    const [selected, setSelected] = useState(defaultSelected())

    const handleCancelButtonClickEvent = () => {
        toggle()
    }

    const handelSaveButtonClickEvent = () => {
        if (selected?.length >= defaultSelected?.length) {
            const selectedCopy = [...selected]
            const newOptions = selectedCopy.filter(x => x.disabled !== true).map(x => x.options[0])
            // const disableNum = selectedCopy.filter(x => x.disabled === true)[0].options.length
            // const disabled = options.slice()?.[0]?.options?.map(x => x.value)
            // const newOptions = selectedCopy.splice(disableNum).map(x => x.options[0])
            dispatch(saveSelectedColumn(newOptions))

            let memo = {}
            if (cookie) {
                memo = JSON.parse(Cookies.get('SelectedMemo'))
            }
            memo[cookieKey] = newOptions
            const json = JSON.stringify(memo)
            Cookies.set('SelectedMemo', json)
        }
        toggle()
    }

    const icons = {
        moveToAvailable: <ChevronLeft className='font-small-4' />,
        moveAllToAvailable: <ChevronsLeft className='font-small-4' />,
        moveToSelected: <ChevronRight className='font-small-4' />,
        moveAllToSelected: <ChevronsRight className='font-small-4' />,
        moveDown: <ChevronDown className='font-small-4' />,
        moveUp: <ChevronUp className='font-small-4' />,
        moveTop: <ChevronsUp className='font-small-4' />,
        moveBottom: <ChevronsDown className='font-small-4' />,
    }

    return !(open) ? null : (
        <Fragment>
            <Modal scrollable isOpen={open} toggle={toggle} className='modal-dialog-centered modal-lg' autoFocus={false}>
                <ModalHeader className='bg-transparent' toggle={toggle}></ModalHeader>
                <ModalBody className='px-5' style={{ height: '50vh' }}>
                    <div className='text-center mb-1'>
                        <h1>欄位調整</h1>
                    </div>
                    <Row style={{ height: '40vh' }}>
                        <Col xs={12}>
                            <DualListBox
                                className='h-100'
                                icons={icons}
                                options={options}
                                selected={selected}
                                onChange={(newValue, selection, controlKey) => {
                                    if (newValue?.length === 0 && controlKey === 'selected') {
                                        setSelected(options.filter(x => x.disabled === true))
                                    } else {
                                        setSelected(newValue)
                                    }
                                }}
                                // preserveSelectOrder
                                // showOrderButtons
                                showHeaderLabels={true}
                                lang={{ availableHeader: '可選欄位', selectedHeader: '已選欄位', }}
                                selectedRef={selectedRef}
                                simpleValue={false}
                            />

                        </Col>
                    </Row>

                </ModalBody>
                <ModalFooter className='justify-content-center'>
                    <Row>
                        <Col xs={12}>
                            <Button className='me-1' color='primary' onClick={handelSaveButtonClickEvent} >保存</Button>
                            <Button.Ripple color='flat-secondary' style={{ backgroundColor: '#80839033' }} onClick={handleCancelButtonClickEvent} outline>取消</Button.Ripple>
                        </Col>
                    </Row>
                </ModalFooter>
            </Modal>
        </Fragment>
    )
})

export default DualListModal
