// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import axios from 'axios'

import { clearSlice } from '@store/rootReducer'

const sliceName = 'WorkMgmt_EmpDutyTypeSetUp'

// 搜尋資料
export const getEmpDutyList = createAsyncThunk(`${sliceName}/getEmpDutyList`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/GetEmpDutyList`, params)
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


export const empDutyTypeSetupUpd = createAsyncThunk(`${sliceName}/EmpDutyTypeSetupUpd`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/EmpDutyTypeSetupUpd`, params)
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

export const addEmpDutyData = createAsyncThunk(`${sliceName}/addEmpDutyData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/AddEmpDutyData`, params)
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

export const updEmpDutyData = createAsyncThunk(`${sliceName}/updEmpDutyData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/UpdEmpDutyData`, params)
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

// Single Row Delete
export const delEmpDutyData = createAsyncThunk(`${sliceName}/delEmpDutyData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/DeleteEmpDutyData`, params)
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

// Select Row Delete
export const delSelectData = createAsyncThunk(`${sliceName}/delSelectData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/DelSelectEmpDutyData`, params)
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

export const getEmpDutyDetailByID = createAsyncThunk(`${sliceName}/getEmpDutyDetailByID`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/EmpDutyTypeSetup/GetEmpDutyDetailByID`, params)
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
        changeFlag:false,
        data:[],
        num: 0,
        selectedDuty: null,
        isLoading: false,
        status:{
            list:{}
        },
        recoverData: {
            storeHead: {},
            modifyFlag: false
        },
        searchTerm: ''
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        updStoreData(state, { payload }) {
            const { index, id, newValue } = payload
            state.data[index][id] = newValue
        },
        resetStoreDate(state, { payload }) {
            state.data = state.originalData
        },
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            state.status[key] = {
                ...state.status[key],
                ...status
            }
        },
        editEmpDuty(state, { payload }) {
            state.selectedDuty = payload
            state.data = []
            state.num = 0
        },
        changeData(state, { payload }) {
            state.changeFlag = payload
        },
        toggleIsLoading(state, action) {
            state.isLoading = !state.isLoading
        },
        storeFormData(state, action) {
            const {key, data} = action.payload
            state.recoverData[key] = data
        },
        saveSearchTerm(state, action) {
            state.searchTerm = action.payload
        }

    },
    extraReducers: builder => {
        builder
            .addCase(getEmpDutyList.rejected, (state, action) => {
                ShowToast('查詢班表設定資料失敗', action.payload?.data?.msg || '', 'danger')
            })
            .addCase(getEmpDutyList.fulfilled, (state, action) => {
                state.status.list = action.payload.params
                state.data = action.payload.data.result
                state.num = action.payload.data.resultNum
                state.selectedDuty = action.payload.data.result
                state.isLoading = false
            })
            .addCase(addEmpDutyData.rejected, (state, action) => {
                ShowToast('新增失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(addEmpDutyData.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('新增成功', '', 'success')
            })
            .addCase(updEmpDutyData.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updEmpDutyData.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('修改成功', '', 'success')
            })
            // Single Row Delete
            .addCase(delEmpDutyData.rejected, (state, action) => {
                ShowToast('刪除失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(delEmpDutyData.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('刪除成功', '', 'success')
            })
            // Select Row Delete
            .addCase(delSelectData.rejected, (state, action) => {
                ShowToast('刪除失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(delSelectData.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('刪除成功', '', 'success')
            })
            //clearSlice
            .addCase(clearSlice, (state, action) => {
                if (action.payload === sliceName) {
                    return initialState() // 僅在 action.payload 匹配 'sliceName' 時清空
                }
            })
    }
})


export const isModify = state => {
    const store = state[sliceName]
    let ret = false
    switch (store.currentMode) {
        case 'query':
            ret = false
            ret = JSON.stringify(store.data) !== JSON.stringify(store.originalData)
            break
    }
    return ret
}

export const isAddMode = state => state[sliceName].currentMode === 'add'
export const isQueryMode = state => state[sliceName].currentMode === 'query'
export const isLoading = state => state[sliceName].isLoading

export default slice.reducer
export const { saveSearchTerm, storeFormData, toggleIsLoading, updStoreData, resetStoreDate, savePageStatus, editEmpDuty, changeData } = slice.actions
