// ** React Import
import { useState, useEffect, useMemo, useCallback } from 'react'
import { Link, useNavigate } from 'react-router-dom'

// ** Utils
import { getUserData, getMachineSet } from '@utils'
import { CustomLabel, MultiSelect } from '@CityAppComponents'
import { getFormatedMonthForInput, addMonths } from '@CityAppHelper'
import axios from 'axios'

// ** Third Party Components
import Select from 'react-select'
import classnames from 'classnames'
import { useForm, Controller } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'
import { AgGridReact } from 'ag-grid-react'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, Card, CardBody, CardHeader, CardTitle, Row, Col } from 'reactstrap'

// ** Store & Actions
import { addEmpOnDuty, updateEmpOnDuty, getSingleData } from '../store'
import { ColumnDefsAdd } from '../List/columns'
import { useDispatch, useSelector } from 'react-redux'

import EmpDetailModal from './EmpDetailModal'

const getNextMonth = () => {
    const date = new Date()
    date.setMonth(date.getMonth() + 1)
    const year = date.getFullYear()
    const month = (`0${date.getMonth() + 1}`).slice(-2) // 確保月份是兩位數
    return `${year}-${month}`
}
const defaultMonth = getNextMonth()


const EmpOnDutyContent = ({ id, t, access }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const navigate = useNavigate()
    const store = useSelector(state => state.WorkMgmt_ShiftSchedule)
    const empDutyTypeList = store.commonList.empDutyTypeList

    const [newData, setNewData] = useState([])
    const [offSetTop, setOffSetTop] = useState(0)
    const [empModOpen, setEmpModOpen] = useState(false)
    const [empModInfo, setEmpModInfo] = useState({})

    const defaultValues = {
        yearMonth: defaultMonth,
        isShowDutyName: '1',
        depID: getMachineSet()?.sellBranch || '',
        depName: getMachineSet()?.sellBranchName || ''
    }

    const { empOption, groupOption, clientOption, selectedEmpOnDuty } = store

    const schema = yup.object({
        yearMonth: yup.string().required(),
        depID: yup.string().required()
    })


    // ** Vars
    const {
        control,
        reset,
        watch,
        setValue,
        getValues,
        handleSubmit,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues
    })

    const watchFields = watch(['yearMonth', 'isShowDutyName'])

    const editRow = (data) => {
        setEmpModOpen(true)
        setEmpModInfo({
            mode: 'edit',
            header: {
                empID: { value: data.empID, label: data.empName },
                depID: { value: data.depID, label: data.depName },
                yearMonth: data.yearMonth
            },
            data
        })
    }

    const columnDefs = useMemo(() => {
        return ColumnDefsAdd({ access, t, yearMonth: watchFields[0], empDutyTypeInfo: empDutyTypeList, isShowDutyName: watchFields[1], setNewData, editRow })
    }, [access, t, JSON.stringify(watchFields), JSON.stringify(empDutyTypeList)])


    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        setValue(name, value)
    }, [])


    const getModeSet = (selectedEmpOnDuty) => {
        if (selectedEmpOnDuty) {
            return {
                title: `${t('edit', { ns: 'common' })}${t('shiftSchedule.title', { ns: 'workMgmt' })}`,
                defaultValues: { ...selectedEmpOnDuty, originalEmpID: selectedEmpOnDuty?.empID },
                empOption: [...empOption],
                submitFunc: updateEmpOnDuty
            }
        } else {
            return {
                title: `${t('add', { ns: 'common' })}${t('shiftSchedule.title', { ns: 'workMgmt' })}`,
                defaultValues,
                empOption,
                submitFunc: addEmpOnDuty
            }
        }
    }
    const getLastMonthData = async (data) => {
        const request = {
            yearMonth: getFormatedMonthForInput(addMonths(-1, new Date(data.yearMonth))).replaceAll('-', ''),
            depID: data.depID
        }
        await axios.post(`/api/EmpOnDuty/GetEmpOnDutyList`, request)
            .then(res => !res.error && setNewData(res.data.data))
    }


    const addDetail = (data) => {
        setEmpModInfo({
            mode: 'add',
            header: data
        })
        setEmpModOpen(true)
    }

    const modeDefine = useMemo(() => getModeSet(selectedEmpOnDuty), [selectedEmpOnDuty, empOption, groupOption])
    // ** Function to handle form submit
    const onSubmit = data => {
        dispatch(
            modeDefine.submitFunc({
                items: newData.map(row => ({ ...row, yearMonth: data.yearMonth.replace('-', ''), modifier: getUserData().userId }))
            })
        ).then(res => !res.error && navigate('../WorkMgmt/ShiftSchedule/List', { replace: true }))
    }

    useEffect(() => {
        id !== 'add' && dispatch(getSingleData({ empID: id })).then(res => res.error && navigate('../WorkMgmt/ShiftSchedule/List', { replace: true }))
    }, [id])

    useEffect(() => {
        reset(modeDefine.defaultValues)
    }, [modeDefine])

    // viewport handle
    useEffect(() => {
        setOffSetTop(document.getElementById('main-content').getBoundingClientRect().top)
    }, [store.data.length])

    return (
        <>
            <Card>
                <CardHeader className='border-bottom p-1'>
                    <CardTitle tag='h4'>{modeDefine.title}</CardTitle>
                    <Col sm='6' className='d-flex justify-content-end'>
                        {(id === 'add' ? access.create : access.update) && (
                            <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                                {t('save', { ns: 'common' })}
                            </Button>
                        )}
                        <Button.Ripple className='me-1' color='flat-secondary' onClick={() => navigate('../WorkMgmt/ShiftSchedule/List', { replace: true })}>
                            {t('cancel', { ns: 'common' })}
                        </Button.Ripple>

                    </Col>
                </CardHeader>
                <CardBody className='py-2 my-25'>
                    <Form>
                        <Row>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='yearMonth'>
                                    {t('shiftSchedule.yearMonth', { ns: 'workMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='yearMonth'
                                    control={control}
                                    render={({ field }) => (
                                        <Input id='yearMonth' type='month' autoComplete="off" invalid={errors.yearMonth && true} {...field} />
                                    )}
                                />
                            </Col>

                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='depID'>
                                    {t('shiftSchedule.depID', { ns: 'workMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='depName'
                                    control={control}
                                    render={({ field }) => (
                                        <Input id='depName' autoComplete="off" invalid={errors.depID && true} disabled={true} {...field} />
                                    )}
                                />
                            </Col>

                            <Col sm='6' className='mb-1 d-flex flex-column'>
                                <Label className='form-label' for='orderByMode_1'>
                                    {"顯示代號或中文"}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center'>
                                    <Controller
                                        name='isShowDutyName'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowDutyName_1'  {...field} checked={field.value === '0'} value={'0'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowDutyName_1'>
                                        {"代號"}
                                    </Label>
                                    <Controller
                                        name='isShowDutyName'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowDutyName_2'  {...field} checked={field.value === '1'} value={'1'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowDutyName_2'>
                                        {"中文"}
                                    </Label>
                                    <div className='ms-auto'>

                                        <Button.Ripple className='me-1' color='flat-primary' onClick={handleSubmit(addDetail)}>
                                            {"新增明細"}
                                        </Button.Ripple>
                                        <Button color='flat-secondary' onClick={handleSubmit(getLastMonthData)}>
                                            {"複製上月"}
                                        </Button>
                                    </div>
                                </div>
                            </Col>

                        </Row>

                    </Form>
                    <div
                        id='main-content'
                        className="ag-theme-quartz"
                        style={{ height: `calc(100vh - ${offSetTop}px - 3.5rem)` }}
                    >
                        <AgGridReact
                            rowData={newData}
                            columnDefs={columnDefs}
                            rowHeight={36}
                        />
                    </div>
                </CardBody>
            </Card >
            <EmpDetailModal show={empModOpen} setShow={setEmpModOpen} t={t} info={empModInfo} setNewData={setNewData} />
        </>
    )
}

export default EmpOnDutyContent
