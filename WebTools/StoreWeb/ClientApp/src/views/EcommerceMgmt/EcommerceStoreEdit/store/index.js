// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { getFullDate, getFormatedDateForInput } from '@CityAppHelper'

// ** Axios Imports
import axios from 'axios'
// ** Vuexy
import { getUserData, getMachineSet } from '@utils'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import { clearSlice } from '@store/rootReducer'


const sliceName = 'EcommerceMgmt_EcommerceStoreEdit'

export const getEcommerceStoreData = createAsyncThunk(`${sliceName}/getEcommerceStoreData`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/GetEcommerceStoreData`, params)
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

export const getSingleData = createAsyncThunk(`${sliceName}/getSingleData`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/GetSingleData`, params)
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

export const addEcommerceStore = createAsyncThunk(`${sliceName}/addEcommerceStore`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/AddEcommerceStoreData`, params)
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

export const updEcommerceStore = createAsyncThunk(`${sliceName}/updEcommerceStore`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/UpdEcommerceStore`, params)
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

export const delEcommerceStore = createAsyncThunk(`${sliceName}/delEcommerceStore`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/DelEcommerceStoreData`, params)
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

export const copyEcommerceStore = createAsyncThunk(`${sliceName}/copyEcommerceStore`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/CopyEcommerceStoreData`, params)
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

export const getEcommerceStoreDetailByID = createAsyncThunk(`${sliceName}/GetEcommerceStoreDetailByID`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/GetEcommerceStoreDetailByID`, params)
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


export const getClientOptionsAll = createAsyncThunk(`${sliceName}/GetClientOptionsAll`, async () => {
    const response = await axios.get(`/api/Client/GetClientOptionsAll`)
    return {
        data: response.data.data
    }
})

export const getAreaCodeOptions = createAsyncThunk(`${sliceName}/GetAreaCodeOptions`, async () => {
    const response = await axios.get(`/api/AreaCode/GetAreaCodeOptions`)
    return {
        data: response.data.data
    }
})

export const getProjectData = createAsyncThunk(`${sliceName}/GetProjectData`, async () => {
    const response = await axios.get(`/api/ProjectA/GetProjectData`)
    return {
        data: response.data.data
    }
})

export const getDeliveryOption = createAsyncThunk(`${sliceName}/getDeliveryOption`, async () => {
    const response = await axios.get(`/api/Parameter/GetFreightOptions`)
    return {
        data: response.data.data
    }
})

export const getStoreOptions = createAsyncThunk(`${sliceName}/getStoreOptions`, async () => {
    const response = await axios.get(`/api/EcommerceStore/GetStoreOption`)
    return {
        data: response.data.data
    }
})

export const getECStore = createAsyncThunk(`${sliceName}/GetECStore`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/GetECStore`, params)
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


export const updECStore = createAsyncThunk(`${sliceName}/UpdECStore`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/UpdECStore`, params)
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

export const getOptionAllSetting = createAsyncThunk(`${sliceName}/getOptionAllSetting`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EcommerceStore/GetOptionAllSetting`, params)
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
    const currentDate = getFormatedDateForInput(new Date())
    return {
        changeFlag: false,
        data: [],
        storeMark: null,
        eStoreID: null,
        copyFlag: false,
        selectedStore: null,
        selectedSetting: null,
        isLoading: {
            StoreData: false
        },
        num: 0,
        status: {
            list: {},
            settingData: {},
            settingClearSliceFlag: false
        },
        recoverData: {
            storeHead: {},
            modifyFlag: false
        },
        areaOption: [],
        projectOption: [],
        clientOption: [],
        deliveryOption: [],
        option: {
            goodsTypeMomoOption: [],
            outplaceMomoOption: [],
            productWarrantlyHandlerYahooOption: [],
            contentRatingYahooOption: [],
            conditionShopeeOption: [],
            descriptionTypeShopeeOption: []
        }
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        toggleCurrentMode(state, { payload }) {
            state.currentMode = payload
        },
        toggleIsLoading(state, { payload }) {
            state.isLoading = payload
        },
        resetStore(state, { payload }) {
            switch (payload) {
                case 'query':
                    state.queryPanel = initialState().queryPanel
                    break
                case 'add':
                    state.addPanel = initialState().addPanel
                    break
                case 'queryModal':
                    state.queryModal = initialState().queryModal
                    break
                case 'editModal':
                    state.editModal = initialState().editModal
                    break
                case 'addModal':
                    state.addModal = initialState().addModal
                    break
                case 'setting':
                    state.selectedSetting = initialState().selectedSetting
                    state.status.settingData = initialState().status.settingData
                default:
                    Object.assign(state, initialState())
            }
        },
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            if ((key === 'settingData') && state.status.settingClearSliceFlag) {
                state.status.settingClearSliceFlag = false
            } else {
                state.status[key] = {
                    ...state.status[key],
                    ...status
                }
            }

        },
        editStore(state, { payload }) {
            state.selectedStore = payload.selectedStore
            state.storeMark = payload.storeTag
            state.eStoreID = payload.eStoreID
        },
        copyStore(state, { payload }) {
            state.copyFlag = payload
        },
        storeFormData(state, action) {
            const { key, data } = action.payload
            state.recoverData[key] = data
        }
    },
    extraReducers: builder => {
        builder
            .addCase(getEcommerceStoreData.fulfilled, (state, action) => {
                const rowData = action.payload.data.result
                if (rowData.length >= 0) {
                    state.data = rowData
                    state.status.list = action.payload.params
                    state.num = action.payload.data.resultNum
                    state.changeFlag = false
                }
            })

            .addCase(addEcommerceStore.rejected, (state, action) => {
                ShowToast('新增失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(addEcommerceStore.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('新增成功', '', 'success')
            })

            .addCase(delEcommerceStore.rejected, (state, action) => {
                ShowToast('刪除失敗', '', 'danger')
            })
            .addCase(delEcommerceStore.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('刪除成功', '', 'success')
            })

            .addCase(updEcommerceStore.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updEcommerceStore.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('修改成功', '', 'success')
            })

            .addCase(copyEcommerceStore.rejected, (state, action) => {
                ShowToast('複製失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(copyEcommerceStore.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('複製成功', '', 'success')
            })
            .addCase(getAreaCodeOptions.fulfilled, (state, action) => {
                state.areaOption = action.payload.data
            })
            .addCase(getProjectData.fulfilled, (state, action) => {
                state.projectOption = action.payload.data.result
            })
            .addCase(getClientOptionsAll.fulfilled, (state, action) => {
                state.clientOption = action.payload.data
            })
            .addCase(getDeliveryOption.fulfilled, (state, action) => {
                state.deliveryOption = action.payload.data.item1
            })
            //clearSlice
            .addCase(clearSlice, (state, action) => {
                if (action.payload === sliceName) {
                    slice.caseReducers.resetStore(state, { payload: 'all' })
                    state.status.settingClearSliceFlag = true
                }
            })

            .addCase(getECStore.fulfilled, (state, action) => {
                state.selectedSetting = action.payload.data
            })
            .addCase(updECStore.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updECStore.fulfilled, (state, action) => {
                ShowToast('修改成功', '', 'success')
            })
            //getOptionAllSetting
            .addCase(getOptionAllSetting.fulfilled, (state, action) => {
                const { goodsType, outplace, productWarrantlyHandler, contentRating, condition, descriptionType } = action.payload.data
                state.option.goodsTypeMomoOption = goodsType?.options
                state.option.outplaceMomoOption = outplace
                state.option.productWarrantlyHandlerYahooOption = productWarrantlyHandler?.options
                state.option.contentRatingYahooOption = contentRating?.options
                state.option.conditionShopeeOption = condition?.options
                state.option.descriptionTypeShopeeOption = descriptionType?.options
            })
    }
})

export const { storeFormData, editStore, copyStore, delBirthValue, toggleCurrentMode, resetStore, savePageStatus } = slice.actions

export const isModify = state => {
    const store = state[sliceName]
    let ret = false
    switch (store.currentMode) {
        case 'query':
            ret = JSON.stringify(store.queryPanel.data) !== JSON.stringify(store.queryPanel.originalData)
            break
        case 'add':
            ret = JSON.stringify(store.addPanel.data) !== JSON.stringify(initialState().addPanel.data)
            break
    }
    return ret
}

export const hadData = state => {
    const store = state[sliceName]
    let ret = false
    switch (store.currentMode) {
        case 'query':
            ret = store.queryPanel.originalData.detail.length > 0
            break
        case 'add':
            ret = store.addPanel.data.detail.length > 0
            break
    }
    return ret
}

export const isAddMode = state => state[sliceName].currentMode === 'add'
export const isQueryMode = state => state[sliceName].currentMode === 'query'
export const isLoading = state => state[sliceName].isLoading
export default slice.reducer
