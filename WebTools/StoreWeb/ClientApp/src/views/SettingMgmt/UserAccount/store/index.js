// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { ShowToast } from '@CityAppExtComponents/caToaster'

import axios from 'axios'

import { clearSlice } from '@store/rootReducer'

const sliceName = 'SettingMgmt_UserAccount'

export const getAllData = createAsyncThunk(`${sliceName}/getAllData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.get(`/api/UserAccount/GetAllData`, params)
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

export const getData = createAsyncThunk(`${sliceName}/getData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/UserAccount/GetData`, params)
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

export const getSingleData = createAsyncThunk(`${sliceName}/getSingleData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/UserAccount/GetSingleData`, params)
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

export const addUser = createAsyncThunk(`${sliceName}/addUsersData`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/UserAccount/AddUsersData`, params)
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

export const deleteUser = createAsyncThunk(`${sliceName}/deleteUser`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/UserAccount/DelUsersData`, params)
        await dispatch(getAllData())
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

export const updateUser = createAsyncThunk(`${sliceName}/updateUser`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/UserAccount/UpdUsersData`, params)
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
        changeFlag: false,
        data: [],
        total: 1,
        groupOption: [],
        empOption: [],
        clientOption: [],
        status: {
            search: false,
            list: {},
            single: {
                selectedUser: {},
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
        editUser(state, { payload }) {
            state.status.single.selectedUser = payload
        },
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            if ((key === 'single' || key === 'list') && state.status.single.clearSliceFlag) {
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
            .addCase(getAllData.fulfilled, (state, action) => {
                const { groupList, empList, clientList } = action.payload.data
                state.groupOption = groupList
                state.empOption = empList
                state.clientOption = clientList
            })
            .addCase(getData.fulfilled, (state, action) => {
                const { params } = action.payload
                if (params.mode !== '') return
                const { userTotal, userList } = action.payload.data
                state.data = userList
                state.total = userTotal
                state.changeFlag = false
                state.status.search = true
            })
            .addCase(getSingleData.fulfilled, (state, action) => {
                const { user } = action.payload.data
                state.status.single.selectedUser = user
            })
            //addUser
            .addCase(addUser.rejected, (state, action) => {
                ShowToast('新增失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(addUser.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('新增成功', '', 'success')
                slice.caseReducers.resetContent(state, { payload: { key: 'single' } })
                state.status.single.clearSliceFlag = true
            })
            //deleteUser
            .addCase(deleteUser.rejected, (state, action) => {
                ShowToast('刪除失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(deleteUser.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('刪除成功', '', 'success')
            })
            //updateUser
            .addCase(updateUser.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updateUser.fulfilled, (state, action) => {
                state.changeFlag = true
                ShowToast('修改成功', '', 'success')
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
    }
})

export const { editUser, savePageStatus, resetContent } = slice.actions
export default slice.reducer

