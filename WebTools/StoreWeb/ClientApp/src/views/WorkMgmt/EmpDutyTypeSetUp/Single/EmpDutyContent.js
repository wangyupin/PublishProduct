// ** React Imports
import { Fragment, useState, useEffect, useCallback, useMemo, useRef } from 'react'
import { useDispatch, useSelector } from 'react-redux'

// ** Reactstrap Imports
import { Row, Col, Button, Card, CardBody, CardHeader, CardText, CardTitle, Input, Badge, Modal, ModalHeader, ModalBody, ModalFooter, Form, Label, InputGroup, InputGroupText } from 'reactstrap'
import { X } from 'react-feather'

// ** Third Party
import { useTranslation } from 'react-i18next'
import Select, { components } from 'react-select'
import AsyncSelect from 'react-select/async'
import axios from 'axios'
import { MdCake } from "react-icons/md"
import { useForm, Controller } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'
import { Link, useNavigate, useParams } from 'react-router-dom'
import classnames from 'classnames'

// ** Utils
import { selectThemeColors, selectStyleSm, formatSelectOptionLabel, getMachineSet, getUserData, selectStyle } from '@utils'
import { convertDateStringForInput, getFormatedDateForInput } from '@CityAppHelper'

import { ShowToast } from '@CityAppExtComponents/caToaster'

// ** Store
import { addEmpDutyData, updEmpDutyData, storeFormData, getEmpDutyDetailByID } from '../store'

import useUpdateEffect from '@hooks/useUpdateEffect'

const userData = JSON.parse(localStorage.getItem('userData'))

const getToday = () => {
    const nowDate = new Date()

    const year = nowDate.getFullYear()
    const month = (nowDate.getMonth() + 1)
    const date = nowDate.getDate()

    const FormatMonth = month < 10 ? `0${month}` : `${month}`
    const Formatdate = date < 10 ? `0${date}` : `${date}`

    return `${year}${FormatMonth}${Formatdate}`
}


const defaultValues = {
    dutyID: '',
    dutyName: '',
    changeDate: '',
    changePerson: '',
    onDuty_hour: '',
    onDuty_min: '',
    offDuty_hour: '',
    offDuty_min: '',
    stOnDutyTime: '',
    stOffDutyTime: '',
    lunchTime: ''
}

const EmpDutyContent = ({ t, access }) => {
    const navigate = useNavigate()
    const dispatch = useDispatch()
    const store = useSelector(state => state.WorkMgmt_EmpDutyTypeSetUp)
    const { selectedDuty, changeFlag } = store
    const [dutyID, setDutyID] = useState('')
    const [dutyName, setDutyName] = useState('')
    const [onDuty_hour, setOnDuty_hour] = useState('')
    const [onDuty_min, setOnDuty_min] = useState('')
    const [offDuty_hour, setOffDuty_hour] = useState('')
    const [offDuty_min, setOffDuty_min] = useState('')
    const [hoursValue, setHoursValue] = useState(0)
    const [lunchTime, setLunchTime] = useState(0)
    const { id } = useParams()

    const schema = yup.object({
        dutyID: yup.string().required(),
        dutyName: yup.string().required(),
        onDuty_hour: yup.string().required(),
        onDuty_min: yup.string().required(),
        offDuty_hour: yup.string().required(),
        offDuty_min: yup.string().required(),
        lunchTime: yup.string().required()
    })

    const getModeSet = (selectedDuty) => {
        if (selectedDuty) {
            return {
                title: `${t('edit', { ns: 'common' })}${t('employeeDuty.title', { ns: 'workMgmt' })}`,
                defaultValues: { ...selectedDuty },
                submitFunc: updEmpDutyData
            }
        } else {
            return {
                title: `${t('add', { ns: 'common' })}${t('employeeDuty.title', { ns: 'workMgmt' })}`,
                defaultValues,
                submitFunc: addEmpDutyData
            }
        }
    }

    const restore = () => {
        dispatch(storeFormData({ key: 'storeHead', data: {} }))
        dispatch(storeFormData({ key: 'recoverContent', data: {} }))
        dispatch(storeFormData({ key: 'modifyFlag', data: false }))
    }

    const modeDefine = useMemo(() => getModeSet(selectedDuty), [selectedDuty])
    const onSubmit = data => {
        const request = {
            dutyID: data.dutyID,
            dutyName: data.dutyName,
            changeDate: data.changeDate ? data.changeDate : getToday(),
            changePerson: data.changePerson ? data.changePerson : userData.username,
            stOnDutyTime: data.onDuty_hour !== '' && data.onDuty_min !== '' ? `${data.onDuty_hour}:${data.onDuty_min}` : '',
            stOffDutyTime: data.offDuty_hour !== '' && data.offDuty_min !== '' ? `${data.offDuty_hour}:${data.offDuty_min}` : '',
            hours: data.hours,
            lunchTime: data.lunchTime
        }
        dispatch(
            modeDefine.submitFunc({
                ...request
            })
        ).then(res => {
            restore()
            !res.error && navigate('../WorkMgmt/EmpDutyTypeSetUp/List', { replace: true })
        })
    }

    // ** Vars
    const {
        control,
        setValue,
        reset,
        watch,
        handleSubmit,
        register,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: modeDefine.defaultValues
    })

    // useEffect(() => {
    //     reset(modeDefine.defaultValues)
    // }, [modeDefine])

    const SingleValue = (props) => {
        return <components.SingleValue {...props}>{props.data.value}</components.SingleValue>
    }

    const sliceTime = (Time, msg) => {
        if (msg === 'onDutyTime') {
            if (Time.length === 4) {
                const hour = Time.slice(0, 1)
                const min = Time.slice(2, 4)
                setOnDuty_hour(hour)
                setOnDuty_min(min)
            } else if (Time.length === 5) {
                const hour = Time.slice(0, 2)
                const min = Time.slice(3, 5)
                setOnDuty_hour(hour)
                setOnDuty_min(min)
            }
        } else if (msg === 'OffDutyTime') {
            if (Time.length === 4) {
                const hour = Time.slice(0, 1)
                const min = Time.slice(2, 4)
                setOffDuty_hour(hour)
                setOffDuty_min(min)
            } else if (Time.length === 5) {
                const hour = Time.slice(0, 2)
                const min = Time.slice(3, 5)
                setOffDuty_hour(hour)
                setOffDuty_min(min)
            }
        }
    }

    // useEffect(() => {
    //     if (selectedDuty !== null) {
    //         const stOnDutyTime = selectedDuty.stOnDutyTime
    //         const stOffDutyTime = selectedDuty.stOffDutyTime
    //         setDutyID(selectedDuty.dutyID)
    //         setDutyName(selectedDuty.dutyName)
    //         if (stOnDutyTime !== null && stOffDutyTime !== null) {
    //             sliceTime(stOnDutyTime, 'onDutyTime')
    //             sliceTime(stOffDutyTime, 'OffDutyTime')
    //         }
    //         setHoursValue(selectedDuty.hours)
    //     }
    // }, [selectedDuty])

    const previousFields = useRef({})
    const watchedFields = watch()

    useUpdateEffect(() => {
        if (JSON.stringify(previousFields.current) !== JSON.stringify(watchedFields)) {
            dispatch(storeFormData({ key: 'storeHead', data: watchedFields }))
            dispatch(storeFormData({ key: 'modifyFlag', data: true }))
            previousFields.current = watchedFields
        }
    }, [watchedFields, dispatch])

    useEffect(() => {
        const fetchData = async () => {
            if (id && id !== 'add') {
                // 另外寫 API
                const res = await dispatch(getEmpDutyDetailByID({ dutyID: id }))
                const resData = res.payload.data.result[0]
                const resObj = {
                    dutyID: resData.dutyID,
                    dutyName: resData.dutyName,
                    changeDate: resData.changeDate,
                    changePerson: resData.changePerson,
                    stOnDutyTime: resData.stOnDutyTime,
                    stOffDutyTime: resData.stOffDutyTime,
                    hours: resData.hours,
                    lunchTime: resData.lunchTime
                }

                if (!store.recoverData.modifyFlag) {
                    reset(resObj)
                    if (resObj.stOnDutyTime) {
                        setOnDuty_hour(resObj.stOnDutyTime.substring(0, 2))
                        setOnDuty_min(resObj.stOnDutyTime.substring(3, 5))
                        setValue('onDuty_hour', resObj.stOnDutyTime.substring(0, 2))
                        setValue('onDuty_min', resObj.stOnDutyTime.substring(3, 5))
                    }
                    if (resObj.stOffDutyTime) {
                        setOffDuty_hour(resObj.stOffDutyTime.substring(0, 2))
                        setOffDuty_min(resObj.stOffDutyTime.substring(3, 5))
                        setValue('offDuty_hour', resObj.stOffDutyTime.substring(0, 2))
                        setValue('offDuty_min', resObj.stOffDutyTime.substring(3, 5))
                    }
                    if (resObj.lunchTime) {
                        setLunchTime(resObj.lunchTime)
                        setValue('lunchTime', resObj.lunchTime)
                    }
                } else {
                    const storeHead = store.recoverData.storeHead
                    reset(storeHead)
                    if (storeHead.stOnDutyTime) {
                        setOnDuty_hour(storeHead.stOnDutyTime.substring(0, 2))
                        setOnDuty_min(storeHead.stOnDutyTime.substring(3, 5))
                        setValue('onDuty_hour', storeHead.stOnDutyTime.substring(0, 2))
                        setValue('onDuty_min', storeHead.stOnDutyTime.substring(3, 5))
                    }
                    if (storeHead.stOffDutyTime) {
                        setOffDuty_hour(storeHead.stOffDutyTime.substring(0, 2))
                        setOffDuty_min(storeHead.stOffDutyTime.substring(3, 5))
                        setValue('offDuty_hour', storeHead.stOffDutyTime.substring(0, 2))
                        setValue('offDuty_min', storeHead.stOffDutyTime.substring(3, 5))
                    }
                    if (storeHead.lunchTime) {
                        setLunchTime(storeHead.lunchTime)
                        setValue('lunchTime', storeHead.lunchTime)
                    }
                }
            } else {
                reset(store.recoverData.storeHead)
            }
        }
        fetchData()
    }, [])

    useEffect(() => {
        if (onDuty_hour !== '' && onDuty_min !== '' && offDuty_hour !== '' && offDuty_min !== '') {
            const time_ST = Number(onDuty_hour)
            const time_ED = Number(offDuty_hour)
            if (time_ST < time_ED) {
                const hour = ((((60 * Number(offDuty_hour)) + Number(offDuty_min)) - ((60 * Number(onDuty_hour)) + Number(onDuty_min))) / 60) - Number(lunchTime)
                setValue('hours', hour)
            } else if (time_ST > time_ED) {
                // 表示隔天
                const hour = ((((60 * (Number(offDuty_hour) + 24)) + Number(offDuty_min)) - ((60 * Number(onDuty_hour)) + Number(onDuty_min))) / 60) - Number(lunchTime)
                setValue('hours', hour)
            }
        }
    }, [onDuty_hour, onDuty_min, offDuty_hour, offDuty_min, lunchTime])

    return (
        <Card>
            <CardHeader className='bg-transparent border-bottom p-1'>
                <CardTitle tag='h4'>{modeDefine.title}</CardTitle>
                <Col sm='6' className='d-flex justify-content-end'>
                    {(id === 'add' ? access.create : access.update) && (
                        <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                            {t('save', { ns: 'common' })}
                        </Button>
                    )}
                    <Button.Ripple
                        className='me-1'
                        color='flat-secondary'
                        onClick={() => {
                            restore()
                            navigate('../WorkMgmt/EmpDutyTypeSetUp/List', { replace: true })
                        }}>
                        {t('cancel', { ns: 'common' })}
                    </Button.Ripple>
                </Col>
            </CardHeader>
            <CardBody className='d-flex flex-column justify-content-center'>
                <Form className='mt-2 pt-50'>
                    <Row style={{ overFlow: 'hidden' }} >
                        <Col sm='3' style={{ marginBottom: '10px' }}>
                            <Label className='form-label' for='dutyID'>
                                {t('employeeDuty.dutyID', { ns: 'workMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='dutyID'
                                control={control}
                                render={({ field }) => (
                                    <Input
                                        {...field}
                                        id='dutyID'
                                        autoComplete="off"
                                        invalid={errors.dutyID && true}
                                    />
                                )}
                            />
                        </Col>
                        <Col sm='3' style={{ marginBottom: '10px' }}>
                            <Label className='form-label' for='dutyName'>
                                {t('employeeDuty.dutyName', { ns: 'workMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='dutyName'
                                control={control}
                                render={({ field }) => (
                                    <Input
                                        {...field}
                                        id='dutyName'
                                        autoComplete="off"
                                        invalid={errors.dutyName && true}
                                    />
                                )}
                            />
                        </Col>
                        <Col sm='3' style={{ marginBottom: '10px' }}>
                            <Label className='form-label' for='onDuty_hour'>
                                {t('employeeDuty.onDuty', { ns: 'workMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <InputGroup>
                                <Controller
                                    name='onDuty_hour'
                                    control={control}
                                    render={({ field }) => (
                                        <Input
                                            {...field}
                                            id='onDuty_hour'
                                            type="text"
                                            maxLength='2'
                                            onChange={(e) => {
                                                const newValue = e.target.value
                                                if (/^\d*$/.test(newValue)) {
                                                    setOnDuty_hour(newValue ? newValue : '')
                                                    setValue('onDuty_hour', newValue)
                                                }
                                            }}
                                            invalid={errors.onDuty_hour && true}
                                        />
                                    )}
                                />
                                <InputGroupText>
                                    :
                                </InputGroupText>
                                <Controller
                                    name='onDuty_min'
                                    control={control}
                                    render={({ field }) => (
                                        <Input
                                            {...field}
                                            id='onDuty_min'
                                            type="text"
                                            maxLength='2'
                                            onChange={(e) => {
                                                const newValue = e.target.value
                                                if (/^\d*$/.test(newValue)) {
                                                    setOnDuty_min(newValue ? newValue : '')
                                                    setValue('onDuty_min', newValue)
                                                }
                                            }}
                                            invalid={errors.onDuty_min && true}
                                        />
                                    )}
                                />
                            </InputGroup>
                        </Col>

                        <Col sm='3' style={{ marginBottom: '10px' }}>
                            <Label className='form-label' for='offDuty_hour'>
                                {t('employeeDuty.offDuty', { ns: 'workMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <InputGroup>
                                <Controller
                                    name='offDuty_hour'
                                    control={control}
                                    render={({ field }) => (
                                        <Input
                                            {...field}
                                            id='offDuty_hour'
                                            type="text"
                                            maxLength='2'
                                            onChange={(e) => {
                                                const newValue = e.target.value
                                                if (/^\d*$/.test(newValue)) {
                                                    setOffDuty_hour(newValue ? newValue : '')
                                                    setValue('offDuty_hour', newValue)
                                                }
                                            }}
                                            invalid={errors.offDuty_hour && true}
                                        />
                                    )}
                                />
                                <InputGroupText>
                                    :
                                </InputGroupText>
                                <Controller
                                    name='offDuty_min'
                                    control={control}
                                    render={({ field }) => (
                                        <Input
                                            {...field}
                                            id='offDuty_min'
                                            type="text"
                                            maxLength='2'
                                            onChange={(e) => {
                                                const newValue = e.target.value
                                                if (/^\d*$/.test(newValue)) {
                                                    setOffDuty_min(newValue ? newValue : '')
                                                    setValue('offDuty_min', newValue)
                                                }
                                            }}
                                            invalid={errors.offDuty_min && true}
                                        />
                                    )}
                                />
                            </InputGroup>
                        </Col>

                        <Col sm='3' style={{ marginBottom: '10px' }}>
                            <Label className='form-label' for='hours'>
                                {t('employeeDuty.hours', { ns: 'workMgmt' })}
                            </Label>
                            <Controller
                                name='hours'
                                control={control}
                                render={({ field }) => (
                                    <Input
                                        {...field}
                                        id='hours'
                                        autoComplete="off"
                                        disabled
                                    />
                                )}
                            />
                        </Col>

                        <Col sm='3' style={{ marginBottom: '10px' }}>
                            <Label className='form-label' for='lunchTime'>
                                {t('employeeDuty.lunchTime', { ns: 'workMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='lunchTime'
                                control={control}
                                render={({ field }) => (
                                    <Input
                                        {...field}
                                        id='lunchTime'
                                        autoComplete="off"
                                        onChange={(e) => {
                                            setLunchTime(e.target.value)
                                            setValue('lunchTime', e.target.value)
                                        }}
                                        invalid={errors.lunchTime && true}
                                    />
                                )}
                            />
                        </Col>
                    </Row>
                </Form>
            </CardBody>
        </Card>
    )
}

export default EmpDutyContent