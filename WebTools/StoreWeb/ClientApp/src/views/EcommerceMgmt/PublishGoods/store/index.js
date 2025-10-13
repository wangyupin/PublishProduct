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
            shipType_shopeeOption: []
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
                const { ecIndex, goodsType, shipType_91app, payment, specChart, shopCategory, salesModeType, sellingDateTime, clothData, shipType_yahoo, productStatus, shipType_shopee } = action.payload.data
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
                const { defVal: defValReq, submitHistory: submitHistoryReq, missingItems, optionList } = action.payload.data
                const submitHistory = JSON.parse(submitHistoryReq)

                const convertedSubmitHistory = submitHistory ? {
                    ...submitHistory,

                    // 轉換 shopCategoryId：從單一值轉為陣列
                    shopCategoryId: submitHistory.shopCategoryId ? [submitHistory.shopCategoryId] : [],

                    // 轉換 shipType_91app：使用 targetDefaultValue 和已選擇的 ID
                    shipType_91app: state.targetDefaultValue.shipType_91app.map(option => ({
                        ...option,
                        checked: submitHistory.shipType_91app?.includes(option.id) || false
                    })),

                    // 轉換 payTypes：使用 targetDefaultValue 和已選擇的 ID  
                    payTypes: state.targetDefaultValue.payTypes.map(option => ({
                        ...option,
                        checked: submitHistory.payTypes?.includes(option.id) || false
                    })),

                    // 轉換 salesModeTypeDef：從數字轉為checkbox陣列
                    salesModeTypeDef: [
                        { checked: submitHistory.salesModeTypeDef === 1 || submitHistory.salesModeTypeDef === 3 },
                        { checked: submitHistory.salesModeTypeDef === 2 || submitHistory.salesModeTypeDef === 3 }
                    ],

                    storeSettings: submitHistory.storeSettings ? submitHistory.storeSettings.map(item => ({
                        ...item,
                        isPublished: item.publish
                    })) : []
                } : {}

                state.selectedValue = {
                    parentID: action.payload.params.parentID,
                    ...defValReq,
                    ...convertedSubmitHistory,
                    skuList: [
                        ...(submitHistory?.skuList || []),
                        ...missingItems
                    ],
                    hasSku: optionList.length > 0,
                    optionList
                }
            })
    }
})

export const { savePageStatus, resetContent } = slice.actions
export default slice.reducer

