/*eslint-disable */
// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { getFullDate, getFormatedDateForInput, getFormatedMonthForInput } from '@CityAppHelper'

// ** Axios Imports
import axios from 'axios'
// ** Vuexy
import { getUserData, getMachineSet } from '@utils'
import { ShowToast } from '@CityAppExtComponents/caToaster'


const sliceName = 'MainTableMgmt_GoodsInfo'
import { clearSlice } from '@store/rootReducer'


export const GetGoodsHelpOffset = createAsyncThunk(`${sliceName}/GetGoodsHelpOffset`, async params => {
    try {
        const [reqParams, cancelToken] = params
        const response = await axios.post(`/api/Goods/GetGoodsHelpOffset`, reqParams, { cancelToken })
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
        dataTable: {
            recoverList: [],
            changeFlag: 0,
            data: [],
            num: 0,
            params: {
                q: '',
                page: 1,
                perPage: 50,
                sortColumn: 'GoodID',
                sort: 'asc',
                mode: ''
            },
            defaultSelectedColumn: [
                { value: 'goodName' },
                { value: 'factoryName' },
                { value: 'brandName' },
                { value: 'advicePrice' },
                { value: 'parentID' },
            ],
            selectedColumn: [
                { value: 'goodName' },
                { value: 'factoryName' },
                { value: 'brandName' },
                { value: 'advicePrice' },
                { value: 'parentID' }
            ],
            selectedGood: null,
            originalPicturePath: null,
            storeHead: {},
            modifyFlag: false
        },
        factoryOptions: {
            data: []
        },
        brandOptions: {
            data: []
        },
        sizeOptions: {
            data: []
        },
        currencyOptions: {
            data: []
        },
        materialOptions: {
            data: []
        },
        goodTagsOption: {
            data: []
        },
        goodCountryOption: {
            data: []
        },
        sizeTagsOption: {
            data: []
        },
        sortHelp: {
            sort01: [],
            sort02: [],
            sort03: [],
            sort04: [],
            sort05: []
        },
        packageOptions: {
            data: []
        }
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        toggleIsLoading(state, { payload }) {
            state.isLoading = payload
        },
        updateRequest: (state, { payload }) => {
            const key = payload.key
            const value = payload.value
            state.dataTable.params[key] = value
        },
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            state.dataTable[key] = {
                ...state.dataTable[key],
                ...status
            }
        },
        saveSelectedColumn: (state, { payload }) => {
            state.dataTable.selectedColumn = payload
        },
        clearTable(state, actions) {
            state.dataTable.selectedGood = null
        },
        deleteImg(state, action) {
            state.dataTable.selectedGood.picturePath = ''
        },
        storeFormData(state, action) {
            const { key, data } = action.payload
            state.dataTable[key] = data
        }
    },
    extraReducers: builder => {
        builder
            .addCase(GetGoodsHelpOffset.pending, (state, action) => {
                slice.caseReducers.toggleIsLoading(state, { payload: true })
            })
            .addCase(GetGoodsHelpOffset.rejected, (state, action) => {
                slice.caseReducers.toggleIsLoading(state, { payload: false })
                if (action.payload) {
                    ShowToast('執行失敗', action.payload.data?.msg || '', 'danger')
                }
            })
            .addCase(GetGoodsHelpOffset.fulfilled, (state, action) => {
                const rowData = action.payload.data.result
                if (rowData.length >= 0) {
                    //had data
                    state.dataTable.data = rowData
                    state.dataTable.num = action.payload.data.resultNum
                } else {
                    //no data
                    ShowToast('商品基本資料增修 - 查詢', '查無資料', 'warning')
                }
                slice.caseReducers.toggleIsLoading(state, { payload: false })
            })
            // 清空回初始狀態
            .addCase(clearSlice, (state, action) => {
                if (action.payload === sliceName) {
                    return initialState() // 僅在 action.payload 匹配 'sliceName' 時清空
                }
            })
    }
})

export const { toggleIsLoading, delQueryPanelData, updateRequest, savePageStatus, saveSelectedColumn, clearTable, deleteImg, storeFormData } = slice.actions

export const isAddMode = state => state[sliceName].currentMode === 'add'
export const isQueryMode = state => state[sliceName].currentMode === 'query'
export const isLoading = state => state[sliceName].isLoading
export default slice.reducer
