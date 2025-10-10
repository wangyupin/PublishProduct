// ** React Import
import { useEffect, useMemo, useCallback } from 'react'
import { Link, useNavigate } from 'react-router-dom'

// ** Utils
import { selectThemeColors, selectStyle, selectStyleSm, getUserData, getMachineSet } from '@utils'
import { getFormatedDateForInput, parseValueLabelObject } from '@CityAppHelper'

// ** Third Party Components
import Select from 'react-select'
import classnames from 'classnames'
import { useForm, Controller } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'
import { AgGridReact } from 'ag-grid-react'
import { ChevronRight, ChevronsRight, ChevronLeft, ChevronsLeft, Search } from 'react-feather'
import useState from 'react-usestateref'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, InputGroup, InputGroupText, Input, Card, CardBody, CardHeader, CardTitle, Row, Col } from 'reactstrap'

// ** Store & Actions
import { updatePunchJob, resetContent, addPunchJob, savePageStatus } from '../store'
import { useDispatch, useSelector } from 'react-redux'
import { punchTypeOption } from '@configs/constants'

const SingleContent = ({ id, t, access }) => {

    const [cancelBtnClick, setCancelBtnClick, cancelBtnClickRef] = useState(false)

    // ** Store Vars
    const dispatch = useDispatch()
    const navigate = useNavigate()
    const store = useSelector(state => state.WorkMgmt_PunchJobEdit)
    const { option, status } = store
    const { selectedPunchJob, selectedMode } = status.single

    const currentDate = new Date()

    const defaultValues = {
        empID: getUserData()?.userId,
        clockStore: getMachineSet()?.sellBranch,
        clockDate: getFormatedDateForInput(currentDate),
        clockTime: currentDate.toLocaleTimeString('en-GB'),
        punchType: punchTypeOption[0]
    }

    const schema = yup.object({
        clockDate: yup.string().required(),
        clockTime: yup.string().required(),
        punchType: yup.object().required()
    })

    // ** Vars
    const { control, reset, watch, setValue, getValues, handleSubmit, formState: { errors } } = useForm({ resolver: yupResolver(schema), defaultValues })

    const getModeSet = (selectedPunchJob) => {
        const tmpValue = {
            ...defaultValues,
            ...selectedPunchJob
        }

        const defaultValuesNew = {
            ...tmpValue,
            empID: option.empOption?.find(opt => opt.value === tmpValue?.empID),
            clockStore: option.clientOption?.find(opt => opt.value === tmpValue?.clockStore),
            punchType: punchTypeOption?.find(opt => opt.value === tmpValue?.punchType)
        }

        if (id !== 'add') {
            return {
                title: `${t('edit', { ns: 'common' })}${t('punchJobEdit.title', { ns: 'workMgmt' })}`,
                defaultValues: defaultValuesNew,
                submitFunc: updatePunchJob,
                mode: 'edit'
            }
        } else {
            return {
                title: `${t('add', { ns: 'common' })}${t('punchJobEdit.title', { ns: 'workMgmt' })}`,
                defaultValues: defaultValuesNew,
                submitFunc: addPunchJob,
                mode: 'add'
            }
        }
    }

    const modeDefine = useMemo(() => getModeSet(selectedPunchJob), [selectedPunchJob, t, option])

    // ** Function to handle form submit
    const onSubmit = data => {
        const request = {
            ...data,
            empID: data.empID?.value,
            punchType: data.punchType?.value,
            holiday: data.punchType?.label,
            modifier: getUserData()?.userId,
            clockStore: data.clockStore?.value
        }
        dispatch(
            modeDefine.submitFunc(request)
        ).then(res => !res.error && navigate('../WorkMgmt/PunchJobEdit/List', { replace: true }))
    }

    useEffect(() => {
        id !== 'add' && selectedMode === null && (!selectedPunchJob?.empID && navigate('../WorkMgmt/PunchJobEdit/List', { replace: true }))
    }, [id])

    useEffect(() => {
        reset(modeDefine.defaultValues)
    }, [modeDefine])

    useEffect(() => {
        return () => {
            if (cancelBtnClickRef.current) {
                dispatch(resetContent({ key: 'single' }))
            } else {
                dispatch(savePageStatus({
                    key: 'single',
                    status: {
                        selectedPunchJob: parseValueLabelObject(getValues()),
                        selectedMode: modeDefine.mode
                    }
                }))
            }
        }
    }, [])

    return (
        <Card>
            <CardHeader className='border-bottom p-1'>
                <CardTitle tag='h4'>{modeDefine.title}</CardTitle>
                <Col sm='6' className='d-flex justify-content-end'>
                    {(id === 'add' ? access.create : access.update) && (
                        <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                            {t('save', { ns: 'common' })}
                        </Button>
                    )}
                    <Button.Ripple className='me-1' color='flat-secondary' onClick={() => {
                        setCancelBtnClick(true)
                        navigate('../WorkMgmt/PunchJobEdit/List', { replace: true })
                    }}>
                        {t('cancel', { ns: 'common' })}
                    </Button.Ripple>

                </Col>
            </CardHeader>
            <CardBody className='py-2 my-25'>
                <Form className='pt-50'>
                    <Row>
                        <Col sm='2' className='mb-1'>
                            <Label className='form-label' for='empName'>
                                {t('employee', { ns: 'common' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='empID'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        inputId='empID'
                                        isClearable={false}
                                        classNamePrefix='select'
                                        options={option.empOption}
                                        theme={selectThemeColors}
                                        styles={selectStyle}
                                        isDisabled={id !== 'add'}
                                        {...field}
                                    />
                                )}

                            />
                        </Col>
                        <Col sm='2' className='mb-1'>
                            <Label className='form-label' for='empName'>
                                {t('client', { ns: 'common' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='clockStore'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        inputId='clockStore'
                                        isClearable={false}
                                        classNamePrefix='select'
                                        options={option.clientOption}
                                        theme={selectThemeColors}
                                        styles={selectStyle}
                                        isDisabled={id !== 'add'}
                                        {...field}
                                    />
                                )}

                            />
                        </Col>
                        <Col sm='2' className='mb-1'>
                            <Label className='form-label' for='clockDate'>
                                {t('punchJob.clockDate', { ns: 'workMgmt' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='clockDate'
                                control={control}
                                render={({ field }) => (
                                    <Input type='date' id='clockDate' invalid={errors.clockDate && true} {...field} />
                                )}
                            />
                        </Col>
                        <Col sm='2' className='mb-1'>
                            <Label className='form-label' for='clockTime'>
                                {t('punchJob.clockTime', { ns: 'workMgmt' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='clockTime'
                                control={control}
                                render={({ field }) => (
                                    <Input type='time' id='clockTime' invalid={errors.clockTime && true} {...field} />
                                )}
                            />
                        </Col>
                        <Col sm='2' className='mb-1'>
                            <Label className='form-label' for='punchType'>
                                {t('punchJob.punchType', { ns: 'workMgmt' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='punchType'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        inputId='punchType'
                                        isClearable={false}
                                        classNamePrefix='select'
                                        options={punchTypeOption}
                                        theme={selectThemeColors}
                                        styles={selectStyle}
                                        {...field}
                                    />
                                )}

                            />
                        </Col>
                    </Row>
                </Form>
            </CardBody>
        </Card >

    )
}

export default SingleContent
