// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { ShowToast } from '@CityAppExtComponents/caToaster'

import axios from 'axios'

import { clearSlice } from '@store/rootReducer'
import { createInstance } from 'i18next'
import StoreSetting from '../Single/StoreSetting'

const sliceName = 'EcommerceMgmt_PublishGoods'

export const getOptionAll = createAsyncThunk(`${sliceName}/getOptionAll`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/PublishGoods/GetOptionAll`, params)
        return {
            params,
            data: response.data.data
        }
    } catch (err) {
        if (!err.response) {
            throw err
        }
        return rejectWithValue(err.response.data)
    }
})

export const submitMain = createAsyncThunk(`${sliceName}/addUsersData`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/PublishGoods/SubmitMain`, params)
        return {
            params,
            data: response.data.data
        }
    } catch (err) {
        if (!err.response) {
            throw err
        }
        return rejectWithValue(err.response.data)
    }
})

export const getSubmitDefVal = createAsyncThunk(`${sliceName}/getSubmitDefVal`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/PublishGoods/GetSubmitDefVal`, params)
        return {
            params,
            data: response.data.data
        }
    } catch (err) {
        if (!err.response) {
            throw err
        }
        return rejectWithValue(err.response.data)
    }
})

const findCategoryPath = (categories, targetId, currentPath = []) => {
    for (const category of categories) {
        const newPath = [...currentPath, category.value]

        if (category.value === targetId) {
            return newPath
        }

        if (category.children?.length > 0) {
            const foundPath = findCategoryPath(category.children, targetId, newPath)
            if (foundPath) return foundPath
        }
    }
    return null
}


const initialState = () => {
    return {
        option: {
            ecIndexOption: [],
            shipType_91appOption: [],
            paymentOption: [],
            specChartOption: [],
            shopCategoryOption: [],
            salesModeTypeOption: [],
            sellingDateTimeOption: [],
            goodsTypeOption: [],
            clothDataTypeOption: [],
            clothDataUnitOption: [],
            clothDataSizeIndex: [],
            clothDataTryIndex: [],
            shipType_yahooOption: [],
            productStatusOption: [],
            shipType_shopeeOption: [],
            categoryOfficialOption: []
        },
        targetDefaultValue: {
            indexList: [],
            shipType_91app: [],
            payTypes: [],
            salesModeTypeDef: [],
            sellingDateTime: [],
            goodsType: null,
            shipType_yahoo: 1,
            productStatus: null,
            shipType_shopee: []
        },
        selectedValue: {},
        status: {
            single: {
                selectedGoods: {},
                selectedMode: null,
                clearSliceFlag: false
            }
        }
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            if ((key === 'single') && state.status.single.clearSliceFlag) {
                state.status.single.clearSliceFlag = false
            } else {
                state.status[key] = {
                    ...state.status[key],
                    ...status
                }
            }
        },
        resetContent(state, { payload }) {
            const { key } = payload
            if (key === 'single') state.status.single = initialState().status.single
            else if (key === 'all') Object.assign(state, initialState())
        }
    },
    extraReducers: builder => {
        builder
            //submitMain
            .addCase(submitMain.rejected, (state, action) => {
                ShowToast('上架失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(submitMain.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('上架成功', '', 'success')
                slice.caseReducers.resetContent(state, { payload: { key: 'single' } })
                state.status.single.clearSliceFlag = true
            })
            //clearSlice
            .addCase(clearSlice, (state, action) => {
                if (action.payload === sliceName) {
                    slice.caseReducers.resetContent(state, { payload: { key: 'all' } })
                    state.status.single.clearSliceFlag = true
                }
            })
            //getOptionAll
            .addCase(getOptionAll.fulfilled, (state, action) => {
                //set option
                const { ecIndex, goodsType, shipType_91app, payment, specChart, shopCategory, salesModeType, sellingDateTime, clothData, shipType_yahoo, productStatus, shipType_shopee, category_Official } = action.payload.data
                state.option.ecIndexOption = ecIndex?.options
                state.option.goodsTypeOption = goodsType?.options
                state.option.shipType_91appOption = shipType_91app?.options
                state.option.paymentOption = payment?.options
                state.option.specChartOption = specChart
                state.option.shopCategoryOption = shopCategory
                state.option.salesModeTypeOption = salesModeType?.options
                state.option.sellingDateTimeOption = sellingDateTime?.options
                state.option.clothDataTypeOption = clothData?.typeOptions
                state.option.clothDataUnitOption = clothData?.unitOptions
                state.option.clothDataSizeIndex = clothData?.sizeIndex
                state.option.clothDataTryIndex = clothData?.tryIndex
                state.option.shipType_yahooOption = shipType_yahoo?.options || []
                state.option.productStatusOption = productStatus?.options || []
                state.option.shipType_shopeeOption = shipType_shopee?.options || []
                state.option.categoryOfficialOption = category_Official || []

                //set default value
                state.targetDefaultValue.indexList = ecIndex?.indexList || []
                state.targetDefaultValue.shipType_91app = shipType_91app?.shippingTypes
                state.targetDefaultValue.payTypes = payment?.payTypes
                state.targetDefaultValue.salesModeTypeDef = salesModeType?.salesModeTypeDef
                state.targetDefaultValue.sellingDateTime = sellingDateTime?.sellingDateTime
                state.targetDefaultValue.goodsType = goodsType?.goodsType
                state.targetDefaultValue.shipType_yahoo = shipType_yahoo?.shipType?.value
                state.targetDefaultValue.productStatus = productStatus?.productStatus?.value
                state.targetDefaultValue.shipType_shopee = shipType_shopee?.shippingTypes || []
            })
            //getSubmitDefVal
            .addCase(getSubmitDefVal.fulfilled, (state, action) => {
                const {
                    defVal,
                    submitHistory,
                    optionList,
                    skuChanges,
                    platformChanges
                } = action.payload.data

                // ✨ 計算變動數量
                const changeCount = {
                    newSkus: skuChanges?.newSkus?.length || 0,
                    deletedSkus: skuChanges?.deletedSkus?.length || 0,
                    modifiedSkus: skuChanges?.modifiedSkus?.length || 0,
                    newPlatforms: platformChanges?.newPlatforms?.length || 0,
                    disabledPlatforms: platformChanges?.disabledPlatforms?.length || 0
                }

                // ✨ 一次性提示所有變動
                if (skuChanges?.hasChanges || platformChanges?.hasChanges) {
                    const messages = []
                    if (changeCount.newSkus > 0) messages.push(`新增 ${changeCount.newSkus} 個 SKU`)
                    if (changeCount.deletedSkus > 0) messages.push(`${changeCount.deletedSkus} 個 SKU 已刪除`)
                    if (changeCount.modifiedSkus > 0) messages.push(`${changeCount.modifiedSkus} 個 SKU 已變更`)
                    if (changeCount.newPlatforms > 0) messages.push(`新增 ${changeCount.newPlatforms} 個平台`)
                    if (changeCount.disabledPlatforms > 0) messages.push(`${changeCount.disabledPlatforms} 個平台已停用`)

                    ShowToast(
                        '資料變動檢測',
                        messages.join('、'),
                        'warning',
                        5000  // 顯示 5 秒
                    )
                }

                // ✨ 處理 SKU 列表
                const processedSkuList = [
                    // 1. 原有的 SKU (標記狀態)
                    ...(submitHistory?.skuList?.map(sku => {
                        // 檢查是否被刪除
                        const deletedInfo = skuChanges?.deletedSkus?.find(d => d.outerId === sku.outerId)

                        // 檢查是否被修改
                        const modifiedInfo = skuChanges?.modifiedSkus?.find(m => m.outerId === sku.outerId)

                        if (deletedInfo) {
                            return {
                                ...sku,
                                _warning: 'deleted',
                                _warningMessage: '此 SKU 在資料庫中已不存在,儲存時會被移除'
                            }
                        }

                        if (modifiedInfo) {
                            return {
                                ...sku,
                                _warning: 'modified',
                                _warningMessage: `已變更: ${modifiedInfo.changes?.join(', ')}`,
                                _suggestedValues: {  // ✨ 儲存建議值,但不直接套用
                                    price: modifiedInfo.price,
                                    cost: modifiedInfo.cost,
                                    sizeName: modifiedInfo.sizeName,
                                    colorName: modifiedInfo.colorName
                                }
                            }
                        }

                        return sku
                    }) || []),

                    // 2. 新增的 SKU
                    ...(skuChanges?.newSkus?.map(sku => ({
                        outerId: sku.goodID,
                        colDetail1: sku.sizeName ? {
                            label: sku.sizeName,
                            value: sku.sizeName
                        } : null,
                        colDetail2: sku.colorName ? {
                            label: sku.colorName,
                            value: sku.colorName
                        } : null,
                        qty: 0,
                        onceQty: 1,
                        price: sku.price,
                        cost: sku.cost,
                        suggestPrice: sku.price,
                        safetyStockQty: 0,
                        image: { path: '' },
                        _warning: 'new',
                        _warningMessage: '新的 SKU'
                    })) || [])
                ]

                // ✨ 處理 StoreSettings
                const processedStoreSettings = [
                    // 1. 原有的平台 (標記狀態)
                    ...(submitHistory?.storeSettings?.map(store => {
                        const disabledInfo = platformChanges?.disabledPlatforms?.find(
                            p => p.platformID === store.platformID
                        )

                        if (disabledInfo) {
                            return {
                                ...store,
                                isPublished: store.publish,
                                _warning: 'disabled',
                                _warningMessage: '此平台已停用,儲存時會被移除'
                            }
                        }

                        return {
                            ...store,
                            isPublished: store.publish
                        }
                    }) || []),

                    // 2. 新平台
                    ...(platformChanges?.newPlatforms?.map(platform => ({
                        platformID: platform.platformID,
                        eStoreID: platform.eStoreID,
                        platformName: platform.platformName,
                        publish: false,
                        isPublished: false,
                        needDelete: false,
                        cost: null,
                        title: null,
                        _warning: 'new',
                        _warningMessage: '新啟用的平台'
                    })) || [])
                ]

                const convertedSubmitHistory = submitHistory ? {
                    ...submitHistory,

                    shipType_91app: state.targetDefaultValue.shipType_91app.map(option => ({
                        ...option,
                        checked: submitHistory.shipType_91app?.includes(option.id) || false
                    })),

                    payTypes: state.targetDefaultValue.payTypes.map(option => ({
                        ...option,
                        checked: submitHistory.payTypes?.includes(option.id.toString()) || false
                    })),

                    salesModeTypeDef: [
                        { checked: submitHistory.salesModeTypeDef === 1 || submitHistory.salesModeTypeDef === 3 },
                        { checked: submitHistory.salesModeTypeDef === 2 || submitHistory.salesModeTypeDef === 3 }
                    ],

                    storeSettings: processedStoreSettings,

                    categoryOfficial: submitHistory.categoryOfficialId ? findCategoryPath(state.option.categoryOfficialOption, submitHistory.categoryOfficialId) : null
                } : {}

                state.selectedValue = {
                    parentID: action.payload.params.parentID,
                    ...defVal,
                    ...convertedSubmitHistory,
                    skuList: processedSkuList,
                    hasSku: optionList.length > 0,
                    optionList
                }
            })
    }
})

export const { savePageStatus, resetContent } = slice.actions
export default slice.reducer

