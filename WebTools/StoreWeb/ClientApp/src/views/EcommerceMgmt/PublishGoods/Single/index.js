// ** React Import
import { useEffect, useMemo, Fragment, useCallback, Children } from 'react'
import { Link, json, useNavigate } from 'react-router-dom'
import axios from 'axios'
import { useDispatch, useSelector } from 'react-redux'

// ** Utils
import { selectThemeColors, selectStyle, formatSelectOptionLabelNL, getUserData } from '@utils'
import { CustomLabel, MultiSelect, Select } from '@CityAppComponents'
import { getDateTimeLocal, getFormatedDateForInput } from '@CityAppHelper'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import useDebounce from '@hooks/useDebounce'
import '@styles/react/libs/editor/editor.scss'
import '@styles/react/libs/file-uploader/file-uploader.scss'
import '@srcAssets/css/tinymce/skin.css'

// ** Third Party Components
import { get, useForm } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import { useUILoader } from '@context/Loader'
import * as yup from 'yup'
import useState from 'react-usestateref'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, Card, CardBody, CardHeader, CardTitle, Row, Col, InputGroup, InputGroupText, Table, Nav, NavItem, NavLink, TabContent, TabPane, CloseButton } from 'reactstrap'

// ** Store & Actions
import BasicInfo from './Basicinfo'
import StoreSetting from './StoreSetting'
import { resetContent, savePageStatus, submitMain, getOptionAll, getSubmitDefVal } from '../store'


const Single = ({ access, t, mode, id }) => {
    const [cancelBtnClick, setCancelBtnClick, cancelBtnClickRef] = useState(false)

    const dispatch = useDispatch()
    const store = useSelector(state => state.EcommerceMgmt_PublishGoods)
    const { status, targetDefaultValue, option, selectedValue } = store
    const { selectedGoods, selectedMode, activeTab } = status.single

    const withLoading = useUILoader()

    const navigate = useNavigate()

    const schema = yup.object({})

    const currentDate = new Date()
    const nextYear = new Date()
    nextYear.setFullYear(nextYear.getFullYear() + 1)

    const [defaultValues, setDefaultValues] = useState({
        parentID: null,
        shopCategoryId: null,
        title: '',
        mainImage: [],
        sellingStartDateTime: getDateTimeLocal(currentDate),
        sellingDateTime: null,
        sellingEndDateTime: getDateTimeLocal(nextYear),
        applyType: '一般',
        isEnableBookingPickupDate: false,
        prepareDays: 0,
        availablePickupFlag: '1', //消費者設定到貨天數or日期範圍
        availablePickupDays: 1,
        hasSku: false,
        salesModeTypeDef: [],
        suggestPrice: 0,
        price: 0,
        cost: 0,
        outerId: '',
        qty: 0,
        safetyStockQty: 0,
        onceQty: 1,
        pointsPayPairs: [{ pairsPoints: 0, pairsPrice: 0, outerPromotionCode: '' }],
        skuList: [],
        optionList: [{ name: '', options: { inputValue: '', value: [] } }],
        saleProductTagList: [{ group: null, key: [] }], //商品標籤
        isClosed: false,
        moreInfo: '',
        specifications: [],
        temperatureTypeDef: 'Normal',
        shipType_91app: [],
        payTypes: [],
        height: 0,
        wIdth: 0,
        length: 0,
        weight: 0,
        salePageSpecChartId: null,
        storeSettings: [],
        productDescription: '',
        //momo
        supGoodsName_serial: '',
        goodsType: null,
        indexList: [],
        clothDataType: null,
        clothDataUnit: null,
        clothDataSizeIndex: [],
        clothDataTryIndex: [],
        shipType_yahoo: 1, //yahoo配送方式
        expDays: null,
        isExpiringItem: false,
        productStatus: null,
        shipType_shopee: [], //shopee配送方式
        sizeImage: null,

        //official
        categoryOfficial: null
    })


    // ** Vars
    const {
        control,
        reset,
        watch,
        handleSubmit,
        getValues,
        setValue,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues
    })

    const [active, setActive, activeRef] = useState('1')
    const [arrVal, setArrVal] = useState({})

    const toggle = tab => {
        if (active !== tab) {
            setActive(tab)
        }
    }

    const handleReset = useCallback(() => {
        setValue('skuList', [])
        setValue('specifications', [])
        setValue('optionList', [])
        setValue('pointsPayPairs', [])
        reset()
    }, [])

    const onSubmit = useCallback(async (data) => {

        console.log(data)
        data.skuList = data.skuList.map(sku => ({ ...sku, originalOuterId: sku.originalOuterId || sku.outerId, originalQty: sku.originalQty || sku.qty }))
        const basicInfo = {
            shopCategoryId: data.shopCategoryId?.slice(-1)?.[0] || null,
            title: data.title,
            sellingStartDateTime: data.sellingStartDateTime,
            sellingEndDateTime: data.sellingEndDateTime,
            applyType: data.applyType,
            expectShippingDate: data.expectShippingDate,
            shippingPrepareDay: data.shippingPrepareDay,
            shipType_91app: data.shipType_91app.filter(item => item.checked).map(item => item.id),
            payTypes: data.payTypes.filter(item => item.checked).map(item => item.id),
            suggestPrice: data.suggestPrice,
            price: data.price,
            cost: data.cost,
            moreInfo: data.moreInfo,
            specifications: data.specifications,
            hasSku: data.hasSku,
            onceQty: data.hasSku ? null : data.onceQty,
            qty: data.hasSku ? null : data.qty,
            outerId: data.hasSku ? null : data.outerId,
            skuList: data.skuList,
            temperatureTypeDef: data.temperatureTypeDef,
            length: data.length,
            wIdth: data.wIdth,
            height: data.height,
            weight: data.weight,
            isEnableBookingPickupDate: data.isEnableBookingPickupDate,
            prepareDays: data.isEnableBookingPickupDate ? data.prepareDays : null,
            availablePickupDays: data.isEnableBookingPickupDate ? data.availablePickupDays : null,
            availablePickupStartDateTime: data.isEnableBookingPickupDate ? data.availablePickupStartDateTime : null,
            availablePickupEndDateTime: data.isEnableBookingPickupDate ? data.availablePickupEndDateTime : null,
            safetyStockQty: data.safetyStockQty,
            salesModeTypeDef: data.salesModeTypeDef[0].checked && data.salesModeTypeDef[1].checked ? 3 : data.salesModeTypeDef[0].checked ? 1 : 2,
            pointsPayPairs: data.salesModeTypeDef[1].checked ? data.pointsPayPairs : null,
            salePageSpecChartId: data.salePageSpecChartId?.value,
            productDescription: data.productDescription,
            supGoodsName_serial: data.supGoodsName_serial,
            goodsType: data.goodsType?.value,
            indexList: data.indexList,
            clothDataType: data.clothDataType?.value,
            clothDataUnit: data.clothDataUnit?.value,
            clothDataSizeIndex: data.clothDataSizeIndex,
            clothDataTryIndex: data.clothDataTryIndex,
            shipType_yahoo: data.shipType_yahoo,
            expDays: data.expDays,
            isExpiringItem: data.isExpiringItem,
            productStatus: data.productStatus?.value,
            shipType_shopee: data.shipType_shopee,

            mainImage: data.mainImage.map(img => ({ path: img.path || '' })),
            sizeImage: { path: data.sizeImage?.path || '' },
            categoryOfficialId: data.categoryOfficial?.[data.categoryOfficial.length - 1]
        }

        const formData = new FormData()
        formData.append('basicInfo', JSON.stringify(basicInfo))
        formData.append('parentID', id)
        formData.append('changePerson', getUserData()?.userId)
        formData.append('store', '010000016')
        formData.append('storeSettings', JSON.stringify(data.storeSettings.map(store => ({
            platformID: store.platformID,
            eStoreID: store.eStoreID,
            cost: store.cost || basicInfo.cost,
            title: store.title || basicInfo.title,
            publish: store.publish,
            needDelete: store.needDelete || false
        }))))
        formData.append('origin', process.env.NODE_ENV === 'production' ? window.location.origin : 'https://localhost:44320')

        // 檔案處理保持不變
        data.mainImage.forEach((img) => {
            if (img.file) formData.append('mainImage', img.file)
            else formData.append('mainImage', new Blob(), img.path === '' ? undefined : img.path)
        })
        data.skuList.forEach((sku) => {
            if (sku.image?.file) formData.append('skuImage', sku.image?.file)
            else formData.append('skuImage', new Blob(), sku.image?.path === '' ? undefined : sku.image?.path)
        })

        if (data.sizeImage?.file) formData.append('sizeImage', data.sizeImage?.file)
        else formData.append('sizeImage', new Blob(), data.sizeImage?.path === '' ? undefined : data.sizeImage?.path)

        withLoading(async () => {
            await dispatch(submitMain(formData))
                .then(res => {
                    if (!res.error) {
                        handleReset()
                        navigate('../MainTableMgmt/GoodsInfo/List', { replace: true })
                    }

                })
        })

    }, [])

    useEffect(() => {
        let defaultValues = selectedGoods

        if (!selectedMode) {
            defaultValues = {
                ...targetDefaultValue,
                ...selectedValue
            }
        }

        // 正規化資料：確保每個 SKU 都有 image 屬性（即使是 null）
        const normalizedDefaults = {
            ...defaultValues,
            skuList: defaultValues.skuList?.map(sku => ({
                ...sku,
                image: sku.image || null  // 關鍵：明確設定為 null
            })) || []
        }

        const arrs = {}

        // 使用 reset 而不是逐一 setValue
        reset(normalizedDefaults)

        // 只保存陣列到 arrs（如果需要的話）
        Object.keys(normalizedDefaults).forEach((key) => {
            if (Array.isArray(normalizedDefaults[key])) {
                arrs[key] = normalizedDefaults[key]
            }
        })

        setArrVal(arrs)

    }, [targetDefaultValue, selectedValue, reset])

    const [storeNumber, setStoreNumber] = useState(null)

    useEffect(() => {
        const loadAppSettings = async () => {
            try {
                const res = await fetch('/api/EcommerceStore/GetStoreNumber')

                if (!res.ok) {
                    throw new Error(`HTTP error! status: ${res.status}`)
                }

                const data = await res.json()
                setStoreNumber(data?.data?.storeNumber)
            } catch (err) {
                console.error('載入設定失敗:', err)
                setStoreNumber(null)
            }
        }

        loadAppSettings()
    }, [])

    useEffect(() => {
        if (!storeNumber) return

        if (selectedMode) {
            reset(selectedGoods)
            setActive(activeTab)
        } else {
            dispatch(getOptionAll({ storeNumber: parseInt(storeNumber) }))
                .then(() => {
                    // 選項載入完成後，再取得編輯資料
                    if (id) {
                        dispatch(getSubmitDefVal({ parentID: id }))
                    }
                })
        }

        return () => {
            if (cancelBtnClickRef.current) {
                dispatch(resetContent({ key: 'single' }))
            } else {
                dispatch(savePageStatus({
                    key: 'single',
                    status: {
                        selectedGoods: getValues(),
                        selectedMode: 'edit',
                        activeTab: activeRef.current
                    }
                }))
            }
        }
    }, [storeNumber])

    return (
        <div className='p-1'>
            <Row>
                <Col sm='9' className='d-flex align-items-center'>
                    <Nav tabs>
                        <NavItem>
                            <NavLink active={active === '1'} onClick={() => toggle('1')}>上架平台</NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink active={active === '2'} onClick={() => toggle('2')}>商品資料</NavLink>
                        </NavItem>
                    </Nav>
                </Col>
                <Col sm='3' className='d-flex justify-content-end'>
                    <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                        {t('save', { ns: 'common' })}
                    </Button>
                    <Button.Ripple className='me-1' color='flat-secondary' onClick={() => {
                        setCancelBtnClick(true)
                        navigate('../MainTableMgmt/GoodsInfo/List', { replace: true })
                    }}>
                        {t('cancel', { ns: 'common' })}
                    </Button.Ripple>

                </Col>
            </Row>
            <Form className='mt-1'>
                <TabContent className='py-50' activeTab={active}>
                    <TabPane tabId='1'>
                        <StoreSetting t={t} reset={reset} getValues={getValues} setValue={setValue} control={control} arrVal={arrVal} />
                    </TabPane>
                    <TabPane tabId='2'>
                        <BasicInfo control={control} setValue={setValue} getValues={getValues} reset={reset} errors={errors} watch={watch} t={t} id={id} defaultValues={defaultValues} arrVal={arrVal} setArrVal={setArrVal} status={status} option={option} />
                    </TabPane>
                </TabContent>
            </Form>
        </div >
    )
}

export default Single
