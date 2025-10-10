// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import { GetEmpOptions } from '@application/controller/EmployeeController'
import { GetClientOptionsAll } from '@application/controller/ClientController'

import axios from 'axios'
import { Sidebar } from 'react-feather'

const sliceName = 'WorkMgmt_ShiftSchedule'

export const getData = createAsyncThunk(`${sliceName}/getData`, async (params, { rejectWithValue, getState}) => {
    try {
        const status = {
            ...getState()[sliceName].status.sidebar,
            ...params
        }
        const request = {
            ...status,
            yearMonth: status.yearMonth?.replace('-', '')
        }
        const response = await axios.post(`/api/EmpOnDuty/GetEmpOnDutyList`, request)
        return {
            params: request,
            data: response.data.data
        }
    } catch (err) {
        if (!err.response) {
            throw err
        }
        return rejectWithValue(err.response.data)
    }
})

export const getCommonList = createAsyncThunk(`${sliceName}/GetCommonList`, async (params, { rejectWithValue }) => {
    try {
      const response = await axios.get(`/api/EmpOnDuty/GetCommonList`)
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

export const getSingleData = createAsyncThunk(`${sliceName}/getSingleData`, async (params, { rejectWithValue}) => {
    try {
        const response = await axios.post(`/api/PeersADJ/GetSingleData`, params)
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

export const addEmpOnDuty = createAsyncThunk(`${sliceName}/addEmpOnDuty`, async (params, { rejectWithValue}) => {
    try {
        const response = await axios.post(`/api/EmpOnDuty/AddEmpOnDuty`, params)
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

export const deleteEmpOnDuty = createAsyncThunk(`${sliceName}/deleteEmpOnDuty`, async (params, { rejectWithValue}) => {
    try {
        const response = await axios.post(`/api/EmpOnDuty/DelEmpOnDuty`, params)
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

export const updateEmpOnDuty = createAsyncThunk(`${sliceName}/updateEmpOnDuty`, async (params, { rejectWithValue}) => {
    try {
        const response = await axios.post(`/api/EmpOnDuty/UpdEmpOnDuty`, params)
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

export const getEmpOptions = GetEmpOptions(sliceName)
export const getClientOptionsAll = GetClientOptionsAll(sliceName)


const initialState = () => {
    return {
        changeFlag: false,
        data: [],
        total: 1,
        selectedEmpOnDuty: null,
        isLoading: {
            userData: false
        },
        status: {
            list: {},
            sidebar: {}
        },
        commonList: {
            empDutyTypeList: [],
            depInfoList: [],
            empOption: [],
            clientOption:[]
          }
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        toggleIsLoading(state, { payload }) {
            const { status, field } = payload
            state.isLoading[field] = status
        },
        editEmpOnDuty(state, { payload }) {
            state.selectedEmpOnDuty = payload
        },
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            state.status[key] = {
                ...state.status[key],
                ...status
            }
        },
        updateChangeFlag(state, { payload }) {
            state.changeFlag = !state.changeFlag
        }
    },
    extraReducers: builder => {
        builder
            .addCase(getData.fulfilled, (state, action) => {
                const result = action.payload.data
                const params = action.payload.params
                state.data = result
                state.status.sidebar = params
                state.changeFlag = false
            })
            .addCase(getSingleData.fulfilled, (state, action) => {
                const { empOnDuty } = action.payload.data
                state.selectedEmpOnDuty = empOnDuty
            })
            //addUser
            .addCase(addEmpOnDuty.rejected, (state, action) => {
                ShowToast('新增失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(addEmpOnDuty.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('新增成功', '', 'success')
            })
            //deleteUser
            .addCase(deleteEmpOnDuty.rejected, (state, action) => {
                ShowToast('刪除失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(deleteEmpOnDuty.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('刪除成功', '', 'success')
            })
            //updateUser
            .addCase(updateEmpOnDuty.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updateEmpOnDuty.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('修改成功', '', 'success')
            })
            .addCase(getCommonList.fulfilled, (state, action) => {
                const rowData = action.payload.data
                const params = action.payload.params
        
                if (rowData && rowData.depInfoList.length && rowData.empDutyTypeList.length) {
                  //had data
                  state.commonList.depInfoList = rowData.depInfoList
                  state.commonList.empDutyTypeList = rowData.empDutyTypeList
                } else {
                  //no data
                  ShowToast('排班資料增修 - 查詢', '查無基本資料清單', 'warning')
                }
              })
              //getEmpOptions
              .addCase(getEmpOptions.fulfilled, (state, action) => {
                const { data } = action.payload
                state.commonList.empOption = data
            })
            //getClientOptionsAll
            .addCase(getClientOptionsAll.fulfilled, (state, action) => {
                const { data } = action.payload
                state.commonList.clientOption = data
            })
    }
})

export const { toggleIsLoading, editEmpOnDuty, savePageStatus, updateChangeFlag } = slice.actions
export const isLoading = state => state[sliceName].isLoading
export default slice.reducer

