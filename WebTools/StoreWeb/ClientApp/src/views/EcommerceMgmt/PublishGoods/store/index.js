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
                    submitHistory,  // ✨ 已經是過濾好的資料了!
                    optionList,
                    skuChanges,
                    platformChanges
                } = action.payload.data

                // ✨ Toast 提示
                if (skuChanges?.newSkus?.length > 0) {
                    ShowToast('發現新的 SKU', `已自動加入 ${skuChanges.newSkus.length} 個新的 SKU`, 'success')
                }

                if (skuChanges?.deletedSkus?.length > 0) {
                    ShowToast('SKU 已移除', `有 ${skuChanges.deletedSkus.length} 個 SKU 在資料庫中已不存在,已自動移除`, 'warning')
                }

                if (skuChanges?.modifiedSkus?.length > 0) {
                    ShowToast('SKU 已變更', `有 ${skuChanges.modifiedSkus.length} 個 SKU 的屬性或價格已變更`, 'warning')
                }

                if (platformChanges?.newPlatforms?.length > 0) {
                    ShowToast('發現新平台', `已加入 ${platformChanges.newPlatforms.length} 個新平台`, 'success')
                }

                if (platformChanges?.disabledPlatforms?.length > 0) {
                    ShowToast('平台已移除', `有 ${platformChanges.disabledPlatforms.length} 個平台已停用,已自動移除`, 'warning')
                }

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

                    // ✨ storeSettings 和 skuList 已經在後端過濾好了
                    storeSettings: submitHistory.storeSettings?.map(item => ({
                        ...item,
                        isPublished: item.publish
                    })) || [],

                    categoryOfficial: submitHistory.categoryOfficialId ? findCategoryPath(state.option.categoryOfficialOption, submitHistory.categoryOfficialId) : null
                } : {}

                state.selectedValue = {
                    parentID: action.payload.params.parentID,
                    ...defVal,
                    ...convertedSubmitHistory,
                    skuList: submitHistory?.skuList || [],  // ✨ 直接用,已經過濾好了
                    hasSku: optionList.length > 0,
                    optionList
                }
            })
    }
})

export const { savePageStatus, resetContent } = slice.actions
export default slice.reducer

