// ** React Imports
import React, { useEffect, useMemo, useState } from 'react'

// ** Customs
import { selectThemeColors } from '@utils'
import useUpdateEffect from '@hooks/useUpdateEffect'

// ** Third Party Components
import { X, Plus } from 'react-feather'
import Select from 'react-select'
import { useTranslation } from 'react-i18next'
import { useForm, Controller } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'

// ** Reactstrap Imports
import { Row, Col, Form, Label, Input, Button, Modal, ModalBody, ModalHeader, InputGroup, ListGroup, ListGroupItem, Badge, ModalFooter } from 'reactstrap'


const AdvanceSearch = React.memo(({ show, setShow, columnFilter, advanceRequest, handleAdvanceSearch }) => {
    const { t } = useTranslation('common')

    // ** State
    const [keyword, setKeyword] = useState({
        type: 'text',
        interval: false
    })

    const [condition, setCondition] = useState([])

    const methodOption = useMemo(() => ([
        { label: t('advanceSearch.contains'), value: 'contains' },
        { label: t('advanceSearch.equal'), value: 'equal' },
        { label: t('advanceSearch.moreThan'), value: 'more than' },
        { label: t('advanceSearch.lessThan'), value: 'less than' },
        { label: t('advanceSearch.between'), value: 'between' }
    ]), [t])

    const schema = yup.object({
        field: yup.object().required(),
        method: yup.object().required(),
        keywordStart: yup.string().required(),
        keywordEnd: yup.string().test({
            test: (value, context) => !(context.parent.method.value === 'between' && value.length <= 0)
        })
    })

    const {
        control,
        reset,
        watch,
        handleSubmit,
        getValues,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            field: columnFilter?.[0],
            method: methodOption?.[0],
            keywordStart: '',
            keywordEnd: ''
        }
    })

    const ruleType = watch(['field', 'method'])

    useEffect(() => {
        const field = ruleType[0]
        const method = ruleType[1]
        setKeyword({
            type: field?.type || 'text',
            interval: Boolean(method.value === 'between')
        })
    }, [...ruleType])

    const onSubmit = (data) => {
        const cond = JSON.parse(JSON.stringify(data))
        setCondition([
            ...condition,
            cond
        ])
        reset()
    }

    const onDelete = (delIndex) => {
        setCondition(condition.filter((row, index) => index !== delIndex))
    }

    const onSearchBtnClick = () => {
        setShow(!show)
    }

    const onClearBtnClick = () => {
        setCondition([])
    }

    useUpdateEffect(() => {
        if (!show) {
            handleAdvanceSearch({
                conditions: condition.map(cond => ({
                    ...cond,
                    field: cond.field.value,
                    type: cond.field.type || 'text',
                    method: cond.method.value
                }))
            })
        }
    }, [show])

    useEffect(() => {
        reset({ ...getValues(), field: columnFilter?.[0], method: methodOption?.[0] })
    }, [columnFilter, methodOption])

    useEffect(() => {
        if (advanceRequest?.conditions) {
            const conditions = advanceRequest.conditions.map(cond => ({
                field: columnFilter.find(rule => rule.value === cond.field),
                method: methodOption.find(method => method.value === cond.method),
                keywordStart: cond.keywordStart,
                keywordEnd: cond.keywordEnd
            }))
            setCondition(conditions)
        }
    }, [])

    return (
        <Modal
            isOpen={show}
            toggle={() => setShow(!show)}
            className='modal-dialog-centered modal-lg'
        >
            <ModalHeader className='bg-transparent' toggle={() => setShow(!show)}></ModalHeader>
            <ModalBody className='px-3' style={{ overflow: 'auto', height: '50vh' }}>
                <div className='text-center mb-1'>
                    <h1>{t('advanceSearch.title')}</h1>
                </div>
                <Form onSubmit={handleSubmit(onSubmit)}>
                    <Row className='justify-content-between align-items-end'>
                        <Col md={4} className='mb-md-0 mb-1'>
                            <Label className='form-label' for='field'>
                                {t('advanceSearch.field')}
                            </Label>
                            <Controller
                                name='field'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        inputId='field'
                                        isClearable={false}
                                        classNamePrefix='select'
                                        options={columnFilter}
                                        theme={selectThemeColors}
                                        {...field}
                                    />
                                )}
                            />
                        </Col>
                        <Col md={2} className='mb-md-0 mb-1'>
                            <Label className='form-label' for='method'>
                                {t('advanceSearch.method')}
                            </Label>
                            <Controller
                                name='method'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        inputId='method'
                                        isClearable={false}
                                        classNamePrefix='select'
                                        options={methodOption}
                                        theme={selectThemeColors}
                                        styles={{
                                            control: (baseStyles, state) => ({
                                                ...baseStyles,
                                                backgroundColor: '#f3f2f7',
                                                borderRadius: '40px'
                                            })
                                        }}
                                        {...field}
                                    />
                                )}
                            />
                        </Col>
                        <Col md={5} className='mb-md-0 mb-1'>
                            <Label className='form-label' for={'keywordStart'}>
                                {t('advanceSearch.condition')}
                            </Label>
                            <InputGroup>
                                <Controller
                                    name='keywordStart'
                                    control={control}
                                    render={({ field }) => (
                                        <Input id='keywordStart' autoComplete="off"  {...field} type={keyword.type} className={keyword.interval ? '' : 'rounded'} />
                                    )}
                                />
                                <Controller
                                    name='keywordEnd'
                                    control={control}
                                    render={({ field }) => (
                                        <Input id='keywordEnd' autoComplete="off"  {...field} type={keyword.type} className={keyword.interval ? '' : 'd-none'} />
                                    )}
                                />
                            </InputGroup>
                        </Col>
                        <Col md={1}>
                            <Button.Ripple className='btn-icon' color='flat-primary' type='submit'>
                                <Plus size={16} />
                            </Button.Ripple>
                        </Col>
                        <Col sm={12}>
                            <hr />
                        </Col>
                    </Row>
                </Form>
                <ListGroup>
                    {condition.map((cond, index) => {
                        return (
                            <ListGroupItem className='d-flex align-items-center' key={index}>
                                <Row className='w-100 me-1'>
                                    <Col md='4'>
                                        <span>{cond.field.label}</span>
                                    </Col>
                                    <Col md='2'>
                                        <span className='bg-light-primary rounded pill' style={{ padding: '2px 8px' }}>{cond.method.label}</span>
                                    </Col>

                                    <Col md='6' className='d-flex justify-content-between'>
                                        <span >{cond.keywordStart}</span>
                                        {cond.method.value === 'between' &&
                                            <span className='me-1 bg-light-primary rounded pill' style={{ padding: '2px 8px' }}>{t('advanceSearch.conjunction')}</span>
                                        }
                                        <span >{cond.keywordEnd}</span>
                                    </Col>
                                </Row>
                                <Button.Ripple className='btn-icon' outline color='danger' style={{ padding: '0.3rem', height: 'unset' }} onClick={() => onDelete(index)}>
                                    <X size={16} />
                                </Button.Ripple>
                            </ListGroupItem>
                        )
                    })}
                </ListGroup>
            </ModalBody>
            <ModalFooter className='justify-content-center'>
                <Row>
                    <Col xs={12}>
                        <Button color='primary' onClick={onSearchBtnClick} className='me-1'>
                            {t('search')}
                        </Button>
                        <Button.Ripple color='flat-secondary' onClick={onClearBtnClick}>
                            {t('clear')}
                        </Button.Ripple>
                    </Col>
                </Row>
            </ModalFooter>
        </Modal>
    )
})

export default AdvanceSearch
