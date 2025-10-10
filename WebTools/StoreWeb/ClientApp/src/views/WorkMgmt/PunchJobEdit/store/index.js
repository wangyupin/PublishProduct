// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import { getFormatedDateForInput } from '@CityAppHelper'
import { getMachineSet } from '@utils'
import { GetEmpOptions } from '@application/controller/EmployeeController'
import { GetClientMeStoreOptions, GetClientOptionsAll } from '@application/controller/ClientController'
import { clearSlice } from '@store/rootReducer'
import axios from 'axios'

const sliceName = 'WorkMgmt_PunchJobEdit'

export const getData = createAsyncThunk(`${sliceName}/getData`, async (params, { rejectWithValue, getState }) => {
    const store = getState()[sliceName]
    const oriRequest = { ...store.defaultValues.sidebar, ...store.status.sidebar, ...params }
    const request = {
        ...oriRequest,
        empID: oriRequest.emp?.value || '',
        empName: oriRequest.emp?.label || '',
        punchType: oriRequest.punchType?.value || 0,
        clockStore: oriRequest.clockStore.map(t => t.value).filter(t => t),
        loggedInStore: getMachineSet()?.sellBranch
    }
    try {
        const response = await axios.post(`/api/EmpClockIn/GetPunchJobList`, request)
        return {
            params: oriRequest,
            data: response.data.data
        }
    } catch (err) {
        if (!err.response) {
            throw err
        }
        return rejectWithValue(err.response.data)
    }
})

export const addPunchJob = createAsyncThunk(`${sliceName}/addPunchJob`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EmpClockIn/AddPunchJob`, params)
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

export const deletePunchJob = createAsyncThunk(`${sliceName}/deletePunchJob`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EmpClockIn/DelPunchJob`, params)
        await dispatch(getData({ mode: '' }))
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

export const updatePunchJob = createAsyncThunk(`${sliceName}/updatePunchJob`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/EmpClockIn/UpdPunchJob`, params)
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
export const getClientOptionsF = GetClientMeStoreOptions(sliceName)
export const getClientOptions = GetClientOptionsAll(sliceName)

const initialState = () => {
    const currentDate = new Date()
    const firstDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1)
    return {
        changeFlag: false,
        data: {
            original: [],
            filtered: []
        },
        option: { empOption: [], clientOption: [] },
        defaultValues: {
            sidebar: {
                clockDate_ST: getFormatedDateForInput(firstDate),
                clockDate_ED: getFormatedDateForInput(currentDate),
                clockStore: [{ value: getMachineSet()?.sellBranch, label: getMachineSet()?.sellBranchName }],
                clockTime_ST: '00:00',
                clockTime_ED: '23:59',
                emp: null,
                punchType: null
            }
        },
        status: {
            table: {},
            sidebar: {},
            single: {
                selectedPunchJob: {},
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
        editPunchJob(state, { payload }) {
            state.status.single.selectedPunchJob = payload
        },
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            if ((key === 'single' || key === 'table') && state.status.single.clearSliceFlag) {
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
        },
        getFilteredData(state, { payload }) {
            const { searchTerm, t } = payload
            const queryLowered = searchTerm.toLowerCase()
            state.data.filtered = state.data.original.filter(row => {
                return (
                    row.empID.toLowerCase().includes(queryLowered) ||
                    row.empName.toLowerCase().includes(queryLowered) ||
                    row.clockDate.toLowerCase().includes(queryLowered) ||
                    row.clockTime.toLowerCase().includes(queryLowered) ||
                    row.clockStore.toLowerCase().includes(queryLowered) ||
                    row.holiday.toLowerCase().includes(queryLowered)
                )
            })

        }
    },
    extraReducers: builder => {
        builder
            .addCase(getData.fulfilled, (state, action) => {
                const { params } = action.payload
                const result = action.payload.data
                state.data.original = result
                state.data.filtered = result
                state.status.sidebar = params
                state.changeFlag = false
            })
            //addPunchJob
            .addCase(addPunchJob.rejected, (state, action) => {
                ShowToast('新增失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(addPunchJob.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('新增成功', '', 'success')
                slice.caseReducers.resetContent(state, { payload: { key: 'single' } })
                state.status.single.clearSliceFlag = true
            })
            //deletePunchJob
            .addCase(deletePunchJob.rejected, (state, action) => {
                ShowToast('刪除失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(deletePunchJob.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('刪除成功', '', 'success')
            })
            //updatePunchJob
            .addCase(updatePunchJob.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updatePunchJob.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('修改成功', '', 'success')
                slice.caseReducers.resetContent(state, { payload: { key: 'single' } })
                state.status.single.clearSliceFlag = true
            })
            //getEmpOptions
            .addCase(getEmpOptions.fulfilled, (state, action) => {
                const { data } = action.payload
                state.option.empOption = data
            })
            //getClientOptions
            .addCase(getClientOptions.fulfilled, (state, action) => {
                const { data } = action.payload
                state.option.clientOption = data
            })
            .addCase(getClientOptionsF.fulfilled, (state, action) => {
                const { data } = action.payload
                state.option.clientOption = data
            })
            //clearSlice
            .addCase(clearSlice, (state, action) => {
                if (action.payload === sliceName) {
                    slice.caseReducers.resetContent(state, { payload: { key: 'all' } })
                    state.status.single.clearSliceFlag = true
                }
            })
    }
})

export const { editPunchJob, savePageStatus, resetContent, getFilteredData } = slice.actions
export default slice.reducer

