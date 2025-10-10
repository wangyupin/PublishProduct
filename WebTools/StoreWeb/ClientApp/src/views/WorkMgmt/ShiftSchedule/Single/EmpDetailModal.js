// ** React Imports
import { Fragment, useState, useEffect, useMemo, useCallback } from 'react'
import { useDispatch, useSelector } from 'react-redux'

// ** Reactstrap Imports
import { Row, Col, Button, Card, CardBody, Modal, ModalHeader, ModalBody, ModalFooter, Label, Input, Form, InputGroupText } from 'reactstrap'
import { selectThemeColors, selectStyleSm, formatSelectOptionLabel, getMachineSet, getUserData} from '@utils'
import Select from 'react-select'
import { useForm, Controller } from 'react-hook-form'
import { getFormatedMonthForInput, getDays } from '@CityAppHelper'
import { json } from 'react-router-dom'


const EmpDetailModal = ({ show, setShow, t, info, setNewData, updData = null}) => {
    const dispatch = useDispatch()
    const options = useSelector(state => state.WorkMgmt_ShiftSchedule).commonList

    const onCancelBtnClick = () => {
        setShow(!show)
    }

    const onAddBtnClick = async () => {
        
    }

    const defaultValues = { }

    const {
        control,
        setValue,
        setError,
        getValues,
        reset,
        handleSubmit,
        watch,
        formState: { errors }
    } = useForm({defaultValues})

    const getDetail =  async (data) => {
        for (let day = 0; day <= 30; day++) {
            setValue(`day${day + 1}`, data.dutyID)
            setValue(`depID${day + 1}`, data.depID)
        }
    }

    const getDetailToContent =  async (data) => {
        const save = {}
        for (const [key, value] of Object.entries(data)) {
            value && (save[key] = value.value)
          }
          save['empName'] = data.empID.label
          save['depName'] = data.depID.label
          save['yearMonth'] = info.header.yearMonth

          if (updData) {
            updData({
                item: info.data.item,
                yearMonth: info.data.yearMonth,
                modifier: getUserData().userId,
                ...save
            })
          } else {
            setNewData(prev => {
                const newData = []
                let repeat = false
                prev.forEach(row => {
                    if (row.empID === save.empID) {
                        newData.push(save)
                        repeat = true
                    } else newData.push(row)
                })
                !repeat && newData.push(save)
                return newData
             })
          }
          
          setShow(!show)
          reset()
    }

    const watchFields = watch(['depID', 'dutyID'])

    useEffect(() => {
        if (options.empDutyTypeList.length > 0) reset({...defaultValues, dutyID: {value: options.empDutyTypeList[0].dutyID, label: options.empDutyTypeList[0].dutyName}})
    }, [options.empDutyTypeList])

    useEffect(() => {
    if (info.data) {
        for (const [key, value] of Object.entries(info.data)) {
            let optionValue = null
            if (key.includes('dep')) optionValue = options.clientOption.find(opt => opt.value === value)
            else if (key.includes('day')) {
                const dutyOpt = options.empDutyTypeList.find(opt => opt.dutyID === value)
                optionValue = {value: dutyOpt.dutyID, label: dutyOpt.dutyName}
            }
            key !== 'dutyID' && setValue(key, optionValue)
        }
        setValue('empID', info.header.empID)
        setValue('depID', info.header.depID)
    }
    }, [info])

    const weekList = ['日', '一', '二', '三', '四', '五', '六']
    const DutyDailyList = ({ watchFields, yearMonth, empDutyTypeList }) => {
        if (!yearMonth || empDutyTypeList.length === 0) return []
        const days = getDays(yearMonth.substr(0, 4), yearMonth.substr(4, 2))
        const dateString = (yearMonth || getFormatedMonthForInput()).replace('-', '')
        const formatedDateString = `${dateString.substr(0, 4)}-${dateString.substr(4, 2)}-`
        const formatedDateStringMM = `${dateString.substr(4, 2)}-`
      
        const result = []
        for (let index = 0; index < days; index++) {
            const dayIndex = index + 1
            const dutyDate = formatedDateString + dayIndex.toString().padStart(2, '0')
            const dutyDateMMDD = formatedDateStringMM + dayIndex.toString().padStart(2, '0')
            const weekDay = new Date(dutyDate).getDay()
            const weekDayStyle = (weekDay === 0 || weekDay === 6) ? 'text-danger' : ''
         //  const dutyStyle = empDutyTypeList.find(e => e.dutyID === watchFields[1]?.value)?.stOnDutyTime ? '' : 'text-danger'
          // const depStyle = mainDepID === watchFields[0]?.value ? '' : 'text-info'

            result.push((
<Row key={index}>
                                    <Col sm='3' className='mb-1 fw-bolder d-flex justify-content-around'>
                                    <span className={weekDayStyle}>{dutyDateMMDD}</span>
                                    <span className={weekDayStyle}>{weekList[weekDay]}</span>
                                    </Col>
            
                                    <Col sm='6' className='mb-1'>
                                        <Controller
                                            name={`day${index + 1}`}
                                            control={control}
                                            render={({ field }) => (
                                                <Select
                                                        isClearable={false}
                                                        classNamePrefix='select'
                                                        options={options.empDutyTypeList?.map(opt => { return ({value: opt.dutyID, label: opt.dutyName}) })}
                                                        theme={selectThemeColors}
                                                        classNames={{
                                                            singleValue: (state) => {
                                                                return empDutyTypeList.find(e => e.dutyID ===  state.data.value)?.stOnDutyTime ? '' : 'text-danger'
                                                            }
                                                        }}
                                                        styles={selectStyleSm}
                                                        formatOptionLabel={formatSelectOptionLabel}
                                                        placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                        {...field}
                                                    />
                                            )}
                                        />
                                    </Col>
            
                                    <Col sm='3' className='mb-1'>
                                            <Controller
                                                name={`depID${index + 1}`}
                                                control={control}
                                                render={({ field }) => (
                                                    <Select
                                                        isClearable={false}
                                                        classNamePrefix='select'
                                                        options={options.clientOption}
                                                        theme={selectThemeColors}
                                                        classNames={{
                                                            singleValue: (state) => {
                                                                return watchFields[0]?.value === state.data.value ? '' : 'text-info'
                                                            }
                                                        }}
                                                        styles={selectStyleSm}
                                                        formatOptionLabel={formatSelectOptionLabel}
                                                        placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                        {...field}
                                                    />
                                                )}
            
                                            />
                                        </Col>
                </Row>
            ))
        }
        return result
      }
      

    return (
        <Modal
            isOpen={show}
            toggle={() => setShow(!show)}
            className='modal-dialog-centered modal-lg'
            autoFocus={false}
        >
            <ModalHeader className='bg-transparent'></ModalHeader>
            <ModalBody className='px-2' style={{ overflow: 'auto', height: '65vh' }}>
                <div className='text-center mb-1'>
                    <h1>{t(info.mode, {ns: 'common'})}明細 - {info.header?.yearMonth}</h1>
                </div>
<Form>
    <Row>
    <Col sm='3' className='mb-1'>
                            <Label className='form-label' for='empID'>
                                {t('employee', { ns: 'common' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='empID'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                            isClearable={false}
                                            classNamePrefix='select'
                                            options={options.empOption}
                                            theme={selectThemeColors}
                                            styles={selectStyleSm}
                                            formatOptionLabel={formatSelectOptionLabel}
                                            placeholder={t('selectPlaceholder', { ns: 'common' })}
                                            {...field}
                                            onChange={(e) => {
                                                field.onChange(e)
                                                setValue('depID', {value: e.label1, label: options.clientOption.find(opt => opt.value === e.label1)?.label})
                                            }}
                                        />
                                )}
                            />
                        </Col>

                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='dutyID'>
                                {t('shiftSchedule.dutyID', { ns: 'workMgmt' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='dutyID'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                            isClearable={false}
                                            classNamePrefix='select'
                                            options={options.empDutyTypeList?.map(opt => { return ({value: opt.dutyID, label: opt.dutyName}) })}
                                            theme={selectThemeColors}
                                            styles={selectStyleSm}
                                            formatOptionLabel={formatSelectOptionLabel}
                                            placeholder={t('selectPlaceholder', { ns: 'common' })}
                                            {...field}
                                        />
                                )}
                            />
                        </Col>

                        <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='depID'>
                                    {t('client', { ns: 'common' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='depID'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            isClearable={false}
                                            classNamePrefix='select'
                                            options={options.clientOption}
                                            theme={selectThemeColors}
                                            styles={selectStyleSm}
                                            formatOptionLabel={formatSelectOptionLabel}
                                            placeholder={t('selectPlaceholder', { ns: 'common' })}
                                            isDisabled={true}
                                            {...field}
                                        />
                                    )}

                                />
                            </Col>
                            <Col xs={3} className='d-flex mb-1'>
                        <Button.Ripple size='sm' color='flat-info' className='align-self-end' onClick={handleSubmit(getDetail)}>
                            {"套用"}
                        </Button.Ripple>
                    </Col>
    </Row>
    {DutyDailyList({watchFields, yearMonth: info.header?.yearMonth, empDutyTypeList: options.empDutyTypeList})}
</Form>
            </ModalBody>
            <ModalFooter className='justify-content-end'>
                <Row>
                    <Col xs={12}>
                        <Button.Ripple color='primary' className='me-1' onClick={handleSubmit(getDetailToContent)}>
                            {t('save', { ns: 'common' })}
                        </Button.Ripple>
                        <Button.Ripple color='flat-secondary' onClick={onCancelBtnClick}>
                            {t('cancel', { ns: 'common' })}
                        </Button.Ripple>
                    </Col>
                </Row>
            </ModalFooter>
        </Modal>
    )
}

export default EmpDetailModal