// ** React Import
import { useState, useMemo, useEffect, useCallback } from 'react'

// ** Custom Components
import Sidebar from '@components/sidebar'

// ** Utils
import { selectThemeColors, selectStyle, getMachineSet } from '@utils'
import { getFormatedDateForInput } from '@CityAppHelper'

// ** Third Party Components
import Select from 'react-select'
import classnames from 'classnames'
import { useForm, Controller } from 'react-hook-form'
import { useDispatch, useSelector } from 'react-redux'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, InputGroup, InputGroupText, Row, Col } from 'reactstrap'

// ** Store & Actions
import { getData, savePageStatus } from '../store'

const currentDate = new Date()
const firstDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1)
const currentYearMonth = new Date().toISOString().slice(0, 7)

const defaultValues = {
    isShowDutyName: '1'

}

const SidebarNewUsers = ({ open, toggleSidebar, t, setProgress }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const store = useSelector(state => state.WorkMgmt_ShiftSchedule)
    const status = store.status.sidebar
    // ** Vars
    const {
        control,
        setValue,
        setError,
        getValues,
        reset,
        handleSubmit,
        formState: { errors }
    } = useForm({ defaultValues })

    const onSubmit = (data) => {
        dispatch(getData({
            yearMonth: data.yearMonth?.replace("-", ""),
            depID: getMachineSet()?.sellBranch || '',
            isShowDutyName: data.isShowDutyName 
        })).then(res => setProgress(100))
        toggleSidebar()
        setProgress(25)
    }
    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        setValue(name, value)
    }, [])

    useEffect(() => {
        if (status) {
            Object.entries(status).forEach((val) => setValue(val[0], val[1]))
        }
        return () => {
            dispatch(savePageStatus({ key: 'sidebar', status: getValues() }))
        }
    }, [])
    return (
        <Sidebar
            size='lg'
            open={open}
            title={t('sidebarText', { ns: 'common' }).slice(-4)}
            headerClassName='mb-1'
            bodyClassName='d-flex flex-column h-100'
            contentClassName='pt-0'
            toggleSidebar={toggleSidebar}
        >
            <Form onSubmit = {handleSubmit(onSubmit)}>
                <div className='mb-1'>
                    <Label className='form-label' for='yearMonth'>
                        {t('shiftSchedule.yearMonth', { ns: 'workMgmt' })}
                    </Label>
                    <InputGroup>
                        <Controller                          
                            name='yearMonth'
                            control={control}
                            defaultValue={currentYearMonth}
                            render={({ field }) => (
                                <Input type='month' id='yearMonth'  {...field} />
                            )}
                        />
                    </InputGroup>

                </div>
                <div className='mb-1'>
                    <Label className='form-label' for='orderByMode_1'>
                        {"顯示代號或中文"}
                    </Label>
                    <div className='d-flex flex-grow-1 align-items-center'>
                        <Controller
                            name='isShowDutyName'
                            control={control}
                            render={({ field }) => (
                                <Input type='radio' id='isShowDutyName_1'  {...field} checked={field.value === '0'} value= {'0'} onChange={onRadioChange} />
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
                        <Controller
                            name='isShowDutyName'
                            control={control}
                            render={({ field }) => (
                                <Input type='radio' id='isShowDutyName_3'  {...field} checked={field.value === '2'} value={'2'} onChange={onRadioChange} />
                            )}
                        />
                        <Label className='ms-25 me-1 form-check-label' for='isShowDutyName_3'>
                            {"時數"}
                        </Label>                       
                    </div>
                    </div>                 
            </Form>
            <div className='mt-auto'>
                <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                    {t('search', { ns: 'common' })}
                </Button>
                <Button color='flat-secondary' className='me-1' onClick={() => reset()}>
                    {t('clear', { ns: 'common' })}
                </Button>
                <Button color='flat-secondary' onClick={toggleSidebar}>
                    {t('cancel', { ns: 'common' })}
                </Button>
            </div>

        </Sidebar >
    )
}

export default SidebarNewUsers
