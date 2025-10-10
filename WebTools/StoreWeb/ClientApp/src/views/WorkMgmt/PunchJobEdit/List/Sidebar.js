// ** React Import
import { useState, useMemo, useEffect, useCallback } from 'react'

// ** Custom Components
import Sidebar from '@components/sidebar'

// ** Utils
import { selectThemeColors, selectStyle, selectStyleNHL, getMachineSet, formatSelectOptionLabelNL } from '@utils'

// ** Third Party Components
import { Select, MultiSelect } from '@CityAppComponents'
import classnames from 'classnames'
import { useForm, Controller, get } from 'react-hook-form'
import { useDispatch, useSelector } from 'react-redux'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, InputGroup, InputGroupText, Row, Col } from 'reactstrap'

// ** Store & Actions
import { getData, savePageStatus } from '../store'
import { punchTypeOption } from '@configs/constants'


const SidebarAdv = ({ open, toggleSidebar, t, setProgress, isFront }) => {

    // ** Store Vars
    const dispatch = useDispatch()
    const store = useSelector(state => state.WorkMgmt_PunchJobEdit)
    const defaultValues = store.defaultValues.sidebar
    const status = store.status.sidebar
    const option = store.option

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
        const request = {
            ...data,
            isFront
        }
        dispatch(getData(request)).then(res => setProgress(100))
        toggleSidebar()
        setProgress(25)
    }

    useEffect(() => {
        if (status) {
            Object.entries(status).forEach((val) => setValue(val[0], val[1]))
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
            <Form onSubmit={handleSubmit(onSubmit)}>
                <div className='mb-1'>
                    <Label className='form-label' for='clockStore'>
                        {`${t('client', { ns: 'common' })}`}
                    </Label>
                    <Controller
                        name='clockStore'
                        control={control}
                        render={({ field }) => (
                            <MultiSelect
                                formatOptionLabel={formatSelectOptionLabelNL}
                                inputId='clockStore'
                                isMulti
                                isClearable={false}
                                closeMenuOnSelect={false}
                                classNamePrefix='select'
                                options={option.clientOption}
                                theme={selectThemeColors}
                                styles={selectStyleNHL}
                                className={classnames('react-select')}
                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                {...field}
                            />
                        )}
                    />
                </div>
                <div className='mb-1'>
                    <Label className='form-label' for='emp'>
                        {`${t('employee', { ns: 'common' })}`}
                    </Label>
                    <Controller
                        name='emp'
                        control={control}
                        render={({ field }) => (
                            <Select
                                isClearable={true}
                                classNamePrefix='select'
                                options={option.empOption}
                                theme={selectThemeColors}
                                styles={selectStyle}
                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                {...field}
                            />
                        )}
                    />
                </div>
                <div className='mb-1'>
                    <Label className='form-label' for='punchType'>
                        {`${t('punchJob.punchType', { ns: 'workMgmt' })}`}
                    </Label>
                    <Controller
                        name='punchType'
                        control={control}
                        render={({ field }) => (
                            <Select
                                isClearable={true}
                                classNamePrefix='select'
                                options={punchTypeOption}
                                theme={selectThemeColors}
                                styles={selectStyle}
                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                {...field}
                            />
                        )}
                    />
                </div>

                <div className='mb-1'>
                    <Label className='form-label' for='clockDate_ST'>
                        {t('punchJob.clockDate', { ns: 'workMgmt' })}
                    </Label>
                    <InputGroup>
                        <Controller
                            name='clockDate_ST'
                            control={control}
                            render={({ field }) => (
                                <Input type='date' id='clockDate_ST'  {...field} />
                            )}
                        />
                        <InputGroupText>
                            ~
                        </InputGroupText>
                        <Controller
                            name='clockDate_ED'
                            control={control}
                            render={({ field }) => (
                                <Input type='date' id='clockDate_ED' {...field} />
                            )}
                        />
                    </InputGroup>
                </div>

                <div className='mb-1'>
                    <Label className='form-label' for='clockTime_ST'>
                        {t('punchJob.clockTime', { ns: 'workMgmt' })}
                    </Label>
                    <InputGroup>
                        <Controller
                            name='clockTime_ST'
                            control={control}
                            render={({ field }) => (
                                <Input type='time' id='clockTime_ST'  {...field} />
                            )}
                        />
                        <InputGroupText>
                            ~
                        </InputGroupText>
                        <Controller
                            name='clockTime_ED'
                            control={control}
                            render={({ field }) => (
                                <Input type='time' id='clockTime_ED' {...field} />
                            )}
                        />
                    </InputGroup>
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

export default SidebarAdv
