// ** React Import
import { useEffect, useMemo, useCallback, Fragment } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'

// ** Utils
import { selectThemeColors, selectStyle, formatSelectOptionLabelNL } from '@utils'
import { CustomLabel, MultiSelect, Select, Radio } from '@CityAppComponents'
import { booleanOption } from '@configs/constants'

// ** Third Party Components
import classnames from 'classnames'
import { useForm, Controller } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'
import { Trash, Plus } from 'react-feather'
import useState from 'react-usestateref'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, Table, Card, CardBody, CardHeader, CardTitle, Row, Col } from 'reactstrap'

// ** Store & Actions
import { getECStore, updECStore, savePageStatus, resetStore, getOptionAllSetting } from '../store'
import { useDispatch, useSelector } from 'react-redux'

import { ShowToast } from '@CityAppExtComponents/caToaster'


const Setting = ({ access, t }) => {
    const { id } = useParams()
    const dispatch = useDispatch()
    const navigate = useNavigate()
    const store = useSelector(state => state.EcommerceMgmt_EcommerceStoreEdit)
    const { selectedSetting, status, option } = store
    const statusData = status.settingData

    const [cancelBtnClick, setCancelBtnClick, cancelBtnClickRef] = useState(false)

    const defaultValues = {
        estoreID: null,
        isRestricted_91: false,
        soldOutActionType_91: 'OutOfStock',
        status_91: 'Normal',
        isShowPurchaseList_91: false,
        isShowSoldQty_91: false,
        isShowStockQty_91: false,
        goodsType_Momo: null,
        isECWarehouse_Momo: false,
        hasAs_Momo: false,
        isCommission_Momo: false,
        isAcceptTravelCard_Momo: false,
        outplaceSeq_Momo: null,
        outplaceSeqRtn_Momo: null,
        isIncludeInstall_Momo: false,
        liveStreamYn_Momo: true,
        contentRating_Yahoo: null,
        productWarrantlyPeriod_Yahoo: '無保固',
        productWarrantlyScope_Yahoo: '',
        productWarrantlyHandler_Yahoo: null,
        productWarrantlyDescription_Yahoo: '',
        isInstallRequired_Yahoo: false,
        isLargeVolumnProductGift_Yahoo: false,
        isNeedRecycle_Yahoo: false,
        isOutrightPurchase_Yahoo: true,
        condition_Shopee: null,
        descriptionType_Shopee: null
    }

    const schema = yup.object({
        // estoreID: yup.string().required(),
        // goodsType_Momo: yup.object().required(),
        // outplaceSeq_Momo: yup.object().required(),
        // outplaceSeqRtn_Momo: yup.object().required(),
        // contentRating_Yahoo: yup.object().required(),
        // productWarrantlyHandler_Yahoo: yup.object().required()
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

    const onSubmit = data => {
        const request = {
            ...data,
            goodsType_Momo: data.goodsType_Momo?.value || '',
            outplaceSeq_Momo: data.outplaceSeq_Momo?.value || '',
            outplaceSeqRtn_Momo: data.outplaceSeqRtn_Momo?.value || '',
            contentRating_Yahoo: data.contentRating_Yahoo?.value || '',
            productWarrantlyHandler_Yahoo: data.productWarrantlyHandler_Yahoo?.value || '',
            condition_Shopee: data.condition_Shopee?.value || '',
            descriptionType_Shopee: data.descriptionType_Shopee?.value || ''
        }
        dispatch(updECStore(request))
            .then(res => {
                if (!res.error) {
                    navigate('../EcommerceMgmt/EcommerceStoreEdit/List', { replace: true })
                }
            })
    }

    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        if (value === 'true' || value === 'false') {
            setValue(name, value === 'true')
        } else {
            setValue(name, value)
        }
    }, [])

    useEffect(() => {
        if (!selectedSetting) {
            dispatch(getECStore({ estoreID: id }))
            dispatch(getOptionAllSetting({}))
        }


        return () => {
            if (cancelBtnClickRef.current) {
                dispatch(resetStore({ key: 'setting' }))
            } else {
                dispatch(savePageStatus({
                    key: 'settingData',
                    status: getValues()
                }))
            }
        }
    }, [])


    useEffect(() => {
        const tmpValue = {
            ...defaultValues,
            goodsType_Momo: option.goodsTypeMomoOption?.[0]?.value,
            contentRating_Yahoo: option.contentRatingYahooOption?.[0]?.value,
            productWarrantlyHandler_Yahoo: option.productWarrantlyHandlerYahooOption?.[0]?.value,
            ...selectedSetting,
            ...statusData
        }

        const defaultValuesNew = {
            ...tmpValue,
            goodsType_Momo: option.goodsTypeMomoOption.find(item => item.value === tmpValue.goodsType_Momo) || null,
            contentRating_Yahoo: option.contentRatingYahooOption.find(item => item.value === tmpValue.contentRating_Yahoo) || null,
            productWarrantlyHandler_Yahoo: option.productWarrantlyHandlerYahooOption.find(item => item.value === tmpValue.productWarrantlyHandler_Yahoo) || null
        }

        reset(defaultValuesNew)

    }, [JSON.stringify(selectedSetting), option])


    return (
        <Fragment>
            <Card>
                <CardHeader className='bg-transparent border-bottom p-1'>
                    <CardTitle tag='h4'> {t('ecStore.title', { ns: 'ecommerceMgmt' })}</CardTitle>
                    <Col sm='6' className='d-flex justify-content-end'>
                        <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                            {t('save', { ns: 'common' })}
                        </Button>
                        <Button.Ripple
                            className='me-1'
                            color='flat-secondary'
                            onClick={() => {
                                setCancelBtnClick(true)
                                navigate('../EcommerceMgmt/EcommerceStoreEdit/List', { replace: true })
                            }}>
                            {t('cancel', { ns: 'common' })}
                        </Button.Ripple>
                    </Col>
                </CardHeader>
                <CardBody className=''>
                    <Form className='mt-2 pt-50'>

                        {/* ** 91App */}
                        <Row className={`${id === '0001' ? '' : 'd-none'}`}>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isRestricted_0'>
                                    {t('publish.isRestricted', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isRestricted_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isRestricted_0'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isRestricted_0'>
                                        是
                                    </Label>

                                    <Controller
                                        name='isRestricted_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isRestricted_1'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isRestricted_1'>
                                        否
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='status_0'>
                                    {t('publish.status', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='status_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='status_0'  {...field} checked={field.value === 'Hidden'} value={'Hidden'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='status_0'>
                                        是
                                    </Label>

                                    <Controller
                                        name='status_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='status_1'  {...field} checked={field.value === 'Normal'} value={'Normal'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='status_1'>
                                        否
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='6' className='mb-1'>
                                <Label className='form-label' for='soldOutActionType_OutOfStock'>
                                    {t('publish.soldOutActionType', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='soldOutActionType_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='soldOutActionType_OutOfStock'  {...field} checked={field.value === 'OutOfStock'} value={'OutOfStock'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='soldOutActionType_OutOfStock'>
                                        已售完
                                    </Label>
                                    <Controller
                                        name='soldOutActionType_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='soldOutActionType_NoRestock'  {...field} checked={field.value === 'NoRestock'} value={'NoRestock'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='soldOutActionType_NoRestock'>
                                        售完不補貨
                                    </Label>
                                    <Controller
                                        name='soldOutActionType_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='soldOutActionType_Restock'  {...field} checked={field.value === 'Restock'} value={'Restock'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='soldOutActionType_Restock'>
                                        售完補貨中
                                    </Label>
                                    <Controller
                                        name='soldOutActionType_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='soldOutActionType_BackInStockAlert'  {...field} checked={field.value === 'BackInStockAlert'} value={'BackInStockAlert'} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='soldOutActionType_BackInStockAlert'>
                                        貨到通知
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isShowPurchaseList_0'>
                                    {t('publish.isShowPurchaseList', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isShowPurchaseList_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowPurchaseList_0'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowPurchaseList_0'>
                                        顯示
                                    </Label>

                                    <Controller
                                        name='isShowPurchaseList_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowPurchaseList_1'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowPurchaseList_1'>
                                        隱藏
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isShowSoldQty_0'>
                                    {t('publish.isShowSoldQty', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isShowSoldQty_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowSoldQty_0'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowSoldQty_0'>
                                        顯示
                                    </Label>

                                    <Controller
                                        name='isShowSoldQty_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowSoldQty_1'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowSoldQty_1'>
                                        隱藏
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isShowStockQty_0'>
                                    {t('publish.isShowStockQty', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isShowStockQty_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowStockQty_0'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowStockQty_0'>
                                        顯示
                                    </Label>

                                    <Controller
                                        name='isShowStockQty_91'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isShowStockQty_1'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isShowStockQty_1'>
                                        隱藏
                                    </Label>
                                </div>
                            </Col>
                        </Row>


                        {/* ** Yahoo */}
                        <Row className={`${id === '0002' ? '' : 'd-none'}`}>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isOutrightPurchase_Yahoo'>
                                    {t('publish.isOutrightPurchase', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' >
                                    <Controller
                                        name='isOutrightPurchase_Yahoo'
                                        control={control}
                                        render={({ field }) => (
                                            <Radio
                                                control={control}
                                                name="isOutrightPurchase_Yahoo"
                                                options={booleanOption}
                                                className='w-auto'
                                            />
                                        )}
                                    />
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isInstallRequired_Yahoo'>
                                    {t('publish.isInstallRequired', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' >
                                    <Controller
                                        name='isInstallRequired_Yahoo'
                                        control={control}
                                        render={({ field }) => (
                                            <Radio
                                                control={control}
                                                name="isInstallRequired_Yahoo"
                                                options={booleanOption}
                                                className='w-auto'
                                            />
                                        )}
                                    />
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isLargeVolumnProductGift_Yahoo'>
                                    {t('publish.isLargeVolumnProductGift', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' >
                                    <Controller
                                        name='isLargeVolumnProductGift_Yahoo'
                                        control={control}
                                        render={({ field }) => (
                                            <Radio
                                                control={control}
                                                name="isLargeVolumnProductGift_Yahoo"
                                                options={booleanOption}
                                                className='w-auto'
                                            />
                                        )}
                                    />
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isNeedRecycle_Yahoo'>
                                    {t('publish.isNeedRecycle', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' >
                                    <Controller
                                        name='isNeedRecycle_Yahoo'
                                        control={control}
                                        render={({ field }) => (
                                            <Radio
                                                control={control}
                                                name="isNeedRecycle_Yahoo"
                                                options={booleanOption}
                                                className='w-auto'
                                            />
                                        )}
                                    />
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='contentRating_Yahoo'>
                                    {t('publish.contentRating', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='contentRating_Yahoo'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.contentRatingYahooOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.contentRating_Yahoo && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='productWarrantlyHandler_Yahoo'>
                                    {t('publish.productWarrantlyHandler', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='productWarrantlyHandler_Yahoo'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.productWarrantlyHandlerYahooOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.productWarrantlyHandler_Yahoo && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='productWarrantlyPeriod_Yahoo'>
                                    {t('publish.productWarrantlyPeriod', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <Controller
                                    name='productWarrantlyPeriod_Yahoo'
                                    control={control}
                                    render={({ field }) => (
                                        <Input {...field} />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='productWarrantlyScope_Yahoo'>
                                    {t('publish.productWarrantlyScope', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <Controller
                                    name='productWarrantlyScope_Yahoo'
                                    control={control}
                                    render={({ field }) => (
                                        <Input {...field} />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='productWarrantlyDescription_Yahoo'>
                                    {t('publish.productWarrantlyDescription', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <Controller
                                    name='productWarrantlyDescription_Yahoo'
                                    control={control}
                                    render={({ field }) => (
                                        <Input {...field} type='textarea' />
                                    )}
                                />
                            </Col>
                        </Row>

                        {/* ** Momo */}
                        <Row className={`${id === '0003' ? '' : 'd-none'}`}>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='goodsType_Momo'>
                                    {t('publish.goodsType', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='goodsType_Momo'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.goodsTypeMomoOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.goodsType_Momo && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='outplaceSeq_Momo'>
                                    {t('publish.outplaceSeq', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='outplaceSeq_Momo'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.outplaceMomoOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.outplaceSeq_Momo && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='outplaceSeqRtn_Momo'>
                                    {t('publish.outplaceSeqRtn', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='outplaceSeqRtn_Momo'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.outplaceMomoOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.outplaceSeqRtn_Momo && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isECWarehouse_1'>
                                    {t('publish.isECWarehouse', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isECWarehouse_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isECWarehouse_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isECWarehouse_1'>
                                        是
                                    </Label>
                                    <Controller
                                        name='isECWarehouse_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isECWarehouse_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isECWarehouse_0'>
                                        否
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='hasAs_1'>
                                    {t('publish.hasAs', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='hasAs_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='hasAs_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='hasAs_1'>
                                        有
                                    </Label>
                                    <Controller
                                        name='hasAs_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='hasAs_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='hasAs_0'>
                                        無
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isCommission_1'>
                                    {t('publish.isCommission', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isCommission_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isCommission_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isCommission_1'>
                                        是
                                    </Label>
                                    <Controller
                                        name='isCommission_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isCommission_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isCommission_0'>
                                        否
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isAcceptTravelCard_1'>
                                    {t('publish.isAcceptTravelCard', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isAcceptTravelCard_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isAcceptTravelCard_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isAcceptTravelCard_1'>
                                        是
                                    </Label>
                                    <Controller
                                        name='isAcceptTravelCard_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isAcceptTravelCard_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isAcceptTravelCard_0'>
                                        否
                                    </Label>
                                </div>
                            </Col>

                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='isIncludeInstall_1'>
                                    {t('publish.isIncludeInstall', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='isIncludeInstall_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isIncludeInstall_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isIncludeInstall_1'>
                                        是
                                    </Label>
                                    <Controller
                                        name='isIncludeInstall_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='isIncludeInstall_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='isIncludeInstall_0'>
                                        否
                                    </Label>
                                </div>
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='liveStreamYn_1'>
                                    {t('publish.liveStreamYn', { ns: 'ecommerceMgmt' })}
                                </Label>
                                <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                                    <Controller
                                        name='liveStreamYn_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='liveStreamYn_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='liveStreamYn_1'>
                                        是
                                    </Label>
                                    <Controller
                                        name='liveStreamYn_Momo'
                                        control={control}
                                        render={({ field }) => (
                                            <Input type='radio' id='liveStreamYn_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                        )}
                                    />
                                    <Label className='ms-25 me-1 form-check-label' for='liveStreamYn_0'>
                                        否
                                    </Label>
                                </div>
                            </Col>
                        </Row>
                        {/* ** Shopee */}
                        <Row className={`${id === '0004' ? '' : 'd-none'}`}>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='condition_Shopee'>
                                    {t('publish.condition', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='condition_Shopee'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.conditionShopeeOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.condition_Shopee && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                            <Col sm='3' className='mb-1'>
                                <Label className='form-label' for='descriptionType_Shopee'>
                                    {t('publish.descriptionType', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                                </Label>
                                <Controller
                                    name='descriptionType_Shopee'
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            options={option.descriptionTypeShopeeOption}
                                            formatOptionLabel={formatSelectOptionLabelNL}
                                            className={classnames('react-select', { 'is-invalid': errors.descriptionType_Shopee && true })}
                                            {...field}
                                        />
                                    )}
                                />
                            </Col>
                        </Row>
                    </Form>
                </CardBody>
            </Card >
        </Fragment>
    )
}

export default Setting
