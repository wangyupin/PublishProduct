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

import useUpdateEffect from '@hooks/useUpdateEffect'

// ** Store
import { addEcommerceStore, updEcommerceStore, copyStore, storeFormData, getEcommerceStoreDetailByID, getProjectData, getAreaCodeOptions, getClientOptionsAll, getDeliveryOption, getStoreOptions } from '../store'


const userData = JSON.parse(localStorage.getItem('userData'))

const getToday = () => {
    const nowDate = new Date()

    const year = nowDate.getFullYear()
    const month = (nowDate.getMonth() + 1)
    const date = nowDate.getDate()

    const FormatMonth = month < 10 ? `0${month}` : `${month}`
    const Formatdate = date < 10 ? `0${date}` : `${date}`

    return `${ year }${ FormatMonth }${ Formatdate }`
}

const CustomLabel = ({ htmlFor, t }) => {
    return (
      <Label className='form-check-label' htmlFor={htmlFor}>
        <span className='switch-icon-left fw-bolder' style={{ top: '3.2px', left: '0.7rem' }}>
          {`${t('ecommerceStore.open', { ns: 'ecommerceMgmt' })}`}
        </span>
        <span className='switch-icon-right fw-bolder' style={{ top: '3.2px', left: '2rem' }}>
            {`${t('ecommerceStore.close', { ns: 'ecommerceMgmt' })}`}
        </span>
      </Label>
    )
}

const EcommerceStoreContent = ({ t }) => {
    const navigate = useNavigate()
    const dispatch = useDispatch()
    const store = useSelector(state => state.EcommerceMgmt_EcommerceStoreEdit)
    const { selectedStore, storeMark, eStoreID, copyFlag, areaOption, projectOption, clientOption, deliveryOption} = store

    const taxCategoryOptions = [
        { value: '0', label: '不開' },
        { value: '1', label: '應稅內含' },
        { value: '2', label: '免稅' }
    ]

    const [zipValue, setZipValue] = useState('')
    const [wareHouseValue, setWareHouseValue] = useState('')
    const [taxValue, setTaxValue] = useState(taxCategoryOptions[0])

    // 買方運費計入銷貨明細
    const [addDetailOrder, setAddDetailOrder] = useState('0')

    // 裝箱單列印商品圖
    const [printGoodsImage, setPrintGoodsImage] = useState('0')

    const [oldID, setOldID] = useState(null)

    const { id } = useParams()

    const defaultValues = {
        storeNumber:'',
        apiKey:'',
        hostAddress:'',
        storeAlias:'',
        storeName:'',
        shippingWareHouse:'',
        sellBranch: '',
        eBackStage:'',
        customerServiceMail:'',
        customerServicePhone:'',
        eOfficialAddress:'',
        senderName:'',
        senderPhone:'',
        shippingAddress:'',
        zipID01:'',
        discountActivity:'',
        hongLi:'',
        discountCoupon:'',
        shippingCost:'',
        // 運費加入明細
        addDetail:addDetailOrder,
        // 7-11 子代號
        sevenCode:'',
        // 商品圖加入銷售單
        printImage:printGoodsImage,
        // 課稅別
        taxCategory:taxValue,
        changePerson: userData.userId,
        changeDate: getToday()
    }

    const schema = yup.object({
        storeNumber: yup.string().required(),
        apiKey: yup.string().required(),
        hostAddress:yup.string().required(),
        storeAlias:yup.string().required(),
        storeName:yup.string().required(),
        shippingWareHouse:yup.object().required()
    })

    const restore = () => {
        dispatch(storeFormData({key: 'storeHead', data: {}}))
        dispatch(storeFormData({key: 'recoverContent', data: {}}))
        dispatch(storeFormData({key: 'modifyFlag', data: false}))
    }

    const getModeSet = (selectedStore) => {
        if (selectedStore !== null && copyFlag === true) {
            return {
                title: `${t('edit', { ns: 'common' })}${t('ecommerceStore.title', { ns: 'ecommerceMgmt' })}`,
                defaultValues: { ...selectedStore, OriginalStoreID:id, StoreTag:storeMark},
                submitFunc: addEcommerceStore
            }
        } else if (selectedStore !== null && copyFlag === false) {
            return {
                title: `${t('edit', { ns: 'common' })}${t('ecommerceStore.title', { ns: 'ecommerceMgmt' })}`,
                defaultValues: { ...selectedStore, OriginalStoreID:id, StoreTag:storeMark},
                submitFunc: updEcommerceStore
            }
        } else {
            return {
                title: `${t('add', { ns: 'common' })}${t('ecommerceStore.title', { ns: 'ecommerceMgmt' })}`,
                defaultValues,
                submitFunc: addEcommerceStore
            }
        }
    }

    const modeDefine = useMemo(() => getModeSet(selectedStore, copyFlag), [selectedStore, copyFlag])
    const onSubmit = data => {
        dispatch(
            modeDefine.submitFunc({
                ...data,
                EStoreID:eStoreID,
                storeTag:storeMark,
                zipID01:data.zipID01?.value,
                deliveryWay: data.deliveryWay?.value,
                shippingWareHouse:data.shippingWareHouse?.value,
                sellBranch: data.sellBranch?.value,
                discountActivity:data.discountActivity?.value,
                hongLi:data.hongLi?.value,
                discountCoupon:data.discountCoupon?.value,
                taxCategory: data.taxCategory ? data.taxCategory.value : '',
                addDetail: addDetailOrder,
                printImage: printGoodsImage,
                changePerson: data.changePerson,
                changeDate: data.changeDate,
                originalStoreID: id !== 'add' ? id : '',
                oldID: oldID && oldID.oldID,
                isEcApiEnabled: data.isEcApiEnabled
            })
        ).then(res => {
            if (!res.error) {
                if (copyFlag) {
                    dispatch(copyStore(false))
                }
                restore()
                navigate('../EcommerceMgmt/EcommerceStoreEdit/List', { replace: true })
            } else {
                restore()
                navigate('../EcommerceMgmt/EcommerceStoreEdit/List', { replace: true })
            }
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

    const previousFields = useRef({})
    const watchedFields = watch()

    useUpdateEffect(() => {
        if (JSON.stringify(previousFields.current) !== JSON.stringify(watchedFields)) {
          dispatch(storeFormData({key: 'storeHead', data: watchedFields}))
          dispatch(storeFormData({key: 'modifyFlag', data: true}))
          previousFields.current = watchedFields
        }
    }, [watchedFields, dispatch])

    useEffect(() => {
        const fetchData = async () => {
            const project = await dispatch(getProjectData())
            const client = await dispatch(getClientOptionsAll())
            const area = await dispatch(getAreaCodeOptions())
            const delivery = await dispatch(getDeliveryOption())
            const Estore = await dispatch(getStoreOptions())

            const projectOption = project.payload.data.result
            const clientOption = client.payload.data
            const areaOption = area.payload.data
            const deliveryOption = delivery.payload.data.item1
            const storeOption = Estore.payload.data.result

            if (id && id !== 'add') {
                const res = await dispatch(getEcommerceStoreDetailByID({storeID: id}))
                const resData = res.payload.data.result[0]
                const resObj = {
                    addDetail: resData.addDetail,
                    apiKey: resData.apiKey,
                    customerServiceMail: resData.customerServiceMail,
                    customerServicePhone: resData.customerServicePhone,
                    discountActivity: resData.discountActivity,
                    discountCoupon: resData.discountCoupon,
                    eBackStage: resData.eBackStage,
                    eOfficialAddress: resData.eOfficialAddress,
                    eStoreID: resData.eStoreID,
                    hongLi: resData.hongLi,
                    hostAddress: resData.hostAddress,
                    printImage: resData.printImage,
                    senderName: resData.senderName,
                    senderPhone: resData.senderPhone,
                    sevenCode: resData.sevenCode,
                    shippingAddress: resData.shippingAddress,
                    shippingCost: resData.shippingCost,
                    shippingWareHouse: resData.shippingWarehouse,
                    sellBranch: resData.sellBranch,
                    storeAlias: resData.storeAlias,
                    storeID: resData.storeID,
                    storeName: resData.storeName,
                    storeNumber: resData.storeNumber,
                    storeTag: resData.storeTag,
                    taxCategory: resData.taxCategory,
                    zipID01: resData.zipID01,
                    deliveryWay: resData.deliveryWay,
                    isEcApiEnabled: resData.isEcApiEnabled
                }

                if (!store.recoverData.modifyFlag) {
                    reset(resObj)
                    if (clientOption.length > 0) {
                        const value = clientOption.find(x => x.value === resObj?.shippingWareHouse)
                        setValue('shippingWareHouse', value)

                        const branch = clientOption.find(x => x.value === resObj?.sellBranch)
                        setValue('sellBranch', branch)
                    }
                    if (areaOption.length > 0) {
                        const value = areaOption.find(x => x.value === resObj?.zipID01)
                        setValue('zipID01', value)
                    }
                    if (projectOption.length > 0) {
                        const discountValue = projectOption.find(x => x.value === resObj?.discountActivity)
                        const hongLiValue = projectOption.find(x => x.value === resObj?.hongLi)
                        const couponValue = projectOption.find(x => x.value === resObj?.discountCoupon)
                        // const costValue = projectOptions.find(x => x.value === resObj.shippingCost)
                        
                        setValue('discountActivity', discountValue)
                        setValue('hongLi', hongLiValue)
                        setValue('discountCoupon', couponValue)
                        // setValue('shippingCost', costValue)
                    }
                    if (taxCategoryOptions.length > 0) {
                        const value = taxCategoryOptions.find(x => x.value === resObj?.taxCategory)
                        setValue('taxCategory', value)
                    }
                    if (deliveryOption.length > 0) {
                        const value = deliveryOption.find(x => x.value === resObj?.deliveryWay)
                        setValue('deliveryWay', value)
                    }
                    if (storeOption.length > 0) {
                        const value = storeOption.find(x => x.eStoreID === eStoreID)
                        setOldID(value)
                    }
                } else {
                    const storeHead = store.recoverData.storeHead
                    reset(storeHead)
                    if (clientOption.length > 0) {
                        const value = clientOption.find(x => x.value === storeHead.shippingWareHouse?.value)
                        setValue('shippingWareHouse', value)

                        const branch = clientOption.find(x => x.value === storeHead.sellBranch?.value)
                        setValue('sellBranch', branch)
                    }
                    if (areaOption.length > 0) {
                        const value = areaOption.find(x => x.value === storeHead.zipID01?.value)
                        setValue('zipID01', value)
                    }
                    if (projectOption.length > 0) {
                        const discountValue = projectOption.find(x => x.value === storeHead.discountActivity?.value)
                        const hongLiValue = projectOption.find(x => x.value === storeHead.hongLi?.value)
                        const couponValue = projectOption.find(x => x.value === storeHead.discountCoupon?.value)
                        // const costValue = projectOptions.find(x => x.value === resObj.shippingCost)
                        
                        setValue('discountActivity', discountValue)
                        setValue('hongLi', hongLiValue)
                        setValue('discountCoupon', couponValue)
                        // setValue('shippingCost', costValue)
                    }
                    if (taxCategoryOptions.length > 0) {
                        const value = taxCategoryOptions.find(x => x.value === storeHead.taxCategory?.value)
                        setValue('taxCategory', value)
                    }
                    if (deliveryOption.length > 0) {
                        const value = deliveryOption.find(x => x.value === storeHead?.deliveryWay?.value)
                        setValue('deliveryWay', value)
                    }
                    if (storeOption.length > 0) {
                        const value = storeOption.find(x => x.eStoreID === eStoreID)
                        setOldID(value)
                    }
                }
            } else {
                reset(store.recoverData.storeHead)
                if (storeOption.length > 0) {
                    const value = storeOption.find(x => x.eStoreID === eStoreID)
                    setOldID(value)
                }
            }
        }
        fetchData()
      }, [])

    const SingleValue = (props) => {
        return <components.SingleValue {...props}>{props.data.value}</components.SingleValue>
    }

    return (
        <Card>
            <CardHeader className='bg-transparent border-bottom p-1'>
                <CardTitle tag='h4'>{modeDefine.title}</CardTitle>
                <Col sm='6' className='d-flex justify-content-end'>
                    <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                        {t('save', { ns: 'common' })}
                    </Button>
                    <Button.Ripple 
                        className='me-1' 
                        color='flat-secondary' 
                        onClick={() => {
                            restore()
                            navigate('../EcommerceMgmt/EcommerceStoreEdit/List', { replace: true })
                        }}>
                            {t('cancel', { ns: 'common' })}
                    </Button.Ripple>
                </Col>
            </CardHeader>
            <CardBody className=''>
                <Form className='mt-2 pt-50'>
                    <Row style={{ overFlow: 'hidden' }} >
                        <Col className='d-flex flex-column'>
                            <Row className='border-bottom mb-2'>
                                <Col sm='3' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='storeNumber' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.storeNumber', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                    </Label>
                                    {copyFlag ? (
                                        <Controller
                                            name='storeNumber'
                                            control={control}
                                            render={({ field }) => (
                                                <Input 
                                                    {...field}
                                                    id='storeNumber' 
                                                    autoComplete="off" 
                                                    invalid={errors.storeNumber && true} 
                                                    onChange={(e) => {
                                                        field.onChange(e.target.value)
                                                    }}
                                                />
                                            )}
                                        />
                                    ) : (
                                        <Controller
                                            name='storeNumber'
                                            control={control}
                                            render={({ field }) => (
                                                <Input 
                                                    {...field}
                                                    id='storeNumber' 
                                                    autoComplete="off" 
                                                    invalid={errors.storeNumber && true} 
                                                    placeholder={selectedStore ? "" : "請輸入商店編號"}
                                                    disabled={id !== 'add'}
                                                />
                                            )}
                                        />
                                    )}
                                </Col>

                                <Col sm='3' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='apiKey' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.apiKey', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                    </Label>
                                    <Controller
                                        name='apiKey'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field}
                                                id='apiKey' 
                                                autoComplete="off" 
                                                placeholder="請輸入電商提供的 Key"
                                                invalid={errors.apiKey && true} 
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='hostAddress' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.hostAddress', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                    </Label>
                                    <Controller
                                        name='hostAddress'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='hostAddress' 
                                                autoComplete="off" 
                                                placeholder="請輸入 API 的 Host"
                                                invalid={errors.hostAddress && true} 
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='storeAlias' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.storeAlias', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                    </Label>
                                    <Controller
                                        name='storeAlias'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='storeAlias' 
                                                autoComplete="off" 
                                                placeholder="請輸入此商店的別名"
                                                maxLength={10}
                                                invalid={errors.storeAlias && true} 
                                            />
                                        )}
                                    />
                                </Col>
                            </Row>

                            <Row className='border-bottom mb-2'>
                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='storeName' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.storeName', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                    </Label>
                                    <Controller
                                        name='storeName'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='storeName' 
                                                autoComplete="off" 
                                                placeholder="請輸入商店名稱"
                                                maxLength={10}
                                                invalid={errors.storeName && true} 
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='shippingWareHouse' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.shippingWareHouse', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                    </Label>
                                    <Controller
                                        name='shippingWareHouse'
                                        control={control}
                                        render={({ field }) => (
                                            <Controller
                                                name='shippingWareHouse'
                                                control={control}
                                                render={({ field }) => (
                                                    <Select
                                                        {...field}
                                                        type='select'
                                                        placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                        options={clientOption}
                                                        formatOptionLabel={formatSelectOptionLabel}
                                                        invalid={errors.shippingWareHouse && true} 
                                                        onChange={(e) => {
                                                            setWareHouseValue(e)
                                                            setValue('shippingWareHouse', e)
                                                        }}
                                                    >
                                                    </Select>
                                                )}
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='sellBranch' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.sellBranch', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='sellBranch'
                                        control={control}
                                        render={({ field }) => (
                                            <Select 
                                                {...field} 
                                                id='sellBranch'
                                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                options={clientOption}
                                                formatOptionLabel={formatSelectOptionLabel}
                                                onChange={(e) => {
                                                    setValue('sellBranch', e)
                                                }}
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='deliveryWay' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.deliveryWay', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='deliveryWay'
                                        control={control}
                                        render={({ field }) => (
                                            <Select 
                                                {...field} 
                                                id='deliveryWay'
                                                autoComplete="off"
                                                formatOptionLabel={formatSelectOptionLabel}
                                                options={deliveryOption}
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='customerServiceMail' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.customerServiceMail', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='customerServiceMail'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='customerServiceMail' 
                                                autoComplete="off" 
                                                placeholder="請輸入客服人員 Email"
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='customerServicePhone' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.customerServicePhone', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='customerServicePhone'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field}
                                                id='customerServicePhone' 
                                                autoComplete="off" 
                                                placeholder="請輸入客服人員電話"
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='senderName' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.senderName', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='senderName'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='senderName' 
                                                autoComplete="off"
                                                placeholder="請輸入寄件人名稱"
                                                maxLength={10}
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='senderPhone' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.senderPhone', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='senderPhone'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='senderPhone' 
                                                autoComplete="off" 
                                                placeholder="請輸入寄件人電話"
                                                maxLength={20}
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='6' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='shippingAddress'>
                                        {t('ecommerceStore.shippingAddress', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <InputGroup>
                                        <Controller
                                            name='zipID01'
                                            control={control}
                                            render={({ field }) => (
                                                <Select
                                                    {...field}
                                                    id='zipID01'
                                                    isClearable={false}
                                                    classNamePrefix='select'
                                                    options={areaOption}
                                                    placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                    styles={{
                                                        ...selectStyleSm,
                                                        control: (provided, state) => ({
                                                            ...provided,
                                                            minHeight: '30px',
                                                            height: '36px',
                                                            borderTopRightRadius: 0,
                                                            borderBottomRightRadius: 0,
                                                            width: '17ch'
                                                        })
                                                    }}
                                                    components={{ SingleValue }}
                                                    formatOptionLabel={formatSelectOptionLabel}
                                                    onChange={(data) => {
                                                        setZipValue(data)
                                                        field.onChange(data)
                                                        setValue('shippingAddress', data.label?.replaceAll(' ', ''))
                                                    }}
                                                />
                                            )}
                                        />
                                        <Controller
                                            name='shippingAddress'
                                            control={control}
                                            render={({ field }) => (
                                                <Input {...field} id='shippingAddress' autoComplete="off" />
                                            )}
                                        />
                                    </InputGroup>
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='eBackStage' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.e-backStage', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='eBackStage'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='eBackStage' 
                                                autoComplete="off" 
                                                placeholder="請輸入電商後台網址"
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='3' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='eOfficialAddress' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.e-OfficialAddress', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='eOfficialAddress'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='eOfficialAddress' 
                                                autoComplete="off" 
                                                placeholder="請輸入官網電商網址"
                                            />
                                        )}
                                    />
                                </Col>
                            </Row>

                            <Row className='border-bottom mb-2'>
                                <Col sm='12' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' style={{fontSize:'1.4rem'}}>
                                        {t('goodsSetting.title', { ns: 'ecommerceMgmt' })} 
                                    </Label>
                                </Col>

                                <Col sm='4' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='discountActivity' style={{fontSize:'1rem'}}>
                                        {t('goodsSetting.discountActivity', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='discountActivity'
                                        control={control}
                                        render={({ field }) => (
                                            <Select
                                                {...field}
                                                type='select'
                                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                formatOptionLabel={formatSelectOptionLabel}
                                                options={projectOption}
                                            >
                                            </Select>
                                        )}
                                    />
                                </Col>

                                <Col sm='4' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='hongLi' style={{fontSize:'1rem'}}>
                                        {t('goodsSetting.hongLi', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='hongLi'
                                        control={control}
                                        render={({ field }) => (
                                            <Select
                                                {...field}
                                                type='select'
                                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                formatOptionLabel={formatSelectOptionLabel}
                                                options={projectOption}
                                            >
                                            </Select>
                                        )}
                                    />
                                </Col>

                                <Col sm='4' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='discountCoupon' style={{fontSize:'1rem'}}>
                                        {t('goodsSetting.discountCoupon', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='discountCoupon'
                                        control={control}
                                        render={({ field }) => (
                                            <Select
                                                {...field}
                                                type='select'
                                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                formatOptionLabel={formatSelectOptionLabel}
                                                options={projectOption}
                                            >
                                            </Select>
                                        )}
                                    />
                                </Col>

                                <Col sm='4' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='shippingCost' style={{fontSize:'1rem'}}>
                                        {t('goodsSetting.shippingCost', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='shippingCost'
                                        control={control}
                                        render={({ field }) => (
                                            <Select
                                                {...field}
                                                type='select'
                                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                formatOptionLabel={formatSelectOptionLabel}
                                                // options={projectOptions}
                                            >
                                            </Select>
                                        )}
                                    />
                                </Col>

                                <Col sm='4' style={{ marginBottom:'20px' }} className='d-flex align-items-center'>
                                    <div style={{marginTop:'23px'}}>
                                        <Controller
                                            name='addDetail'
                                            control={control}
                                            render={({ field }) => (
                                                <Input 
                                                    {...field} 
                                                    id='addDetail' 
                                                    autoComplete="off"
                                                    className='form-check-input cursor-pointer'
                                                    style={{
                                                        width:'20px',
                                                        height:'20px',
                                                        marginRight:'8px'
                                                    }}
                                                    type='checkbox' 
                                                    checked={field.value === '1'}
                                                    onChange={(e) => {
                                                        const newValue = e.target.checked ? '1' : '0'
                                                        setAddDetailOrder(newValue)
                                                        field.onChange(newValue)
                                                    }}
                                                />
                                            )}
                                        />
                                        <Label className='form-check-label' for='addDetail' style={{ fontSize:'1.2rem' }}>
                                            {t('goodsSetting.addDetailOrder', { ns: 'ecommerceMgmt' })}
                                        </Label>
                                    </div>
                                </Col>
                            </Row>

                            <Row className='border-bottom mb-2'>
                                <Col sm='12' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' style={{fontSize:'1.4rem'}}>
                                        {t('printerSetting.title', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                </Col>

                                <Col sm='4' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='sevenCode' style={{fontSize:'1rem'}}>
                                        {t('printerSetting.7-11Code', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='sevenCode'
                                        control={control}
                                        render={({ field }) => (
                                            <Input 
                                                {...field} 
                                                id='sevenCode' 
                                                autoComplete="off" 
                                                maxLength={25}
                                            />
                                        )}
                                    />
                                </Col>

                                <Col sm='6' style={{ marginBottom:'20px' }} className='d-flex align-items-center'>
                                    <div style={{marginTop:'29px', display:'flex'}}>
                                        <Controller
                                            name='printImage'
                                            control={control}
                                            render={({ field }) => (
                                                <Input 
                                                    {...field} 
                                                    id='printImage' 
                                                    autoComplete="off"
                                                    className='form-check-input cursor-pointer'
                                                    style={{
                                                        width:'20px',
                                                        height:'20px',
                                                        marginRight:'8px'
                                                    }}
                                                    type='checkbox' 
                                                    checked={field.value === '1'}
                                                    
                                                    onChange={(e) => {
                                                        const newValue = e.target.checked ? '1' : '0'
                                                        setPrintGoodsImage(newValue)
                                                        field.onChange(newValue)
                                                    }}
                                                />
                                            )}
                                        />
                                        <Label className='form-check-label' for='printImage' style={{ fontSize:'1.2rem' }}>
                                            {t('printerSetting.printImage', { ns: 'ecommerceMgmt' })}
                                        </Label>
                                    </div>
                                </Col>
                            </Row>

                            <Row className='border-bottom mb-2'>
                                <Col sm='12' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' style={{fontSize:'1.4rem'}}>
                                        {t('billSetting.title', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                </Col>
                                
                                <Col sm='4' style={{ marginBottom:'20px' }}>
                                    <Label className='form-label' for='taxCategory' style={{fontSize:'1rem'}}>
                                        {t('billSetting.taxCategory', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <Controller
                                        name='taxCategory'
                                        control={control}
                                        render={({ field }) => (
                                            <Select
                                                {...field}
                                                type='select'
                                                menuPlacement='top'
                                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                options={taxCategoryOptions}
                                                formatOptionLabel={formatSelectOptionLabel}
                                                onChange={(e) => {
                                                    setTaxValue(e)
                                                    field.onChange(e)
                                                }}
                                            >
                                            </Select>
                                        )}
                                    />
                                </Col>
                            </Row>

                            <Row>
                                <Col sm='12' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' style={{fontSize:'1.4rem'}}>
                                        {t('ecommerceStore.otherSetting', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                </Col>
                                
                                <Col sm='4' style={{ marginBottom:'10px' }}>
                                    <Label className='form-label' for='taxCategory' style={{fontSize:'1rem'}}>
                                        {t('ecommerceStore.getFreightNumber', { ns: 'ecommerceMgmt' })}
                                    </Label>
                                    <div className='form-switch form-check-primary me-75'>
                                        <Controller 
                                            name='isEcApiEnabled'
                                            control={control}
                                            render={({ field }) => {
                                                return (
                                                    <Input 
                                                        type='switch' 
                                                        id='icon-primary' 
                                                        name='icon-primary' 
                                                        checked={field.value}
                                                        style={{ width: '4.5rem', height: '2rem' }}
                                                        onChange={(e) => {
                                                            field.onChange(!field.value)
                                                        }}
                                                    />
                                                )
                                            }}
                                        />
                                        
                                        <CustomLabel htmlFor='icon-primary' t={t} />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Form>
            </CardBody>
        </Card>
    )
}

export default EcommerceStoreContent