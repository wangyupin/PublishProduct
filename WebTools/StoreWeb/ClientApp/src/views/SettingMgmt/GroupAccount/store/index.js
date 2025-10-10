// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import { paginateArray } from '@CityAppHelper'
import axios from 'axios'

const sliceName = 'SettingMgmt_GroupAccount'

export const getAllData = createAsyncThunk(`${sliceName}/getAllData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.get(`/api/GroupAccount/GetAllData`, params)
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

export const addGroup = createAsyncThunk(`${sliceName}/addGroupPermission`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/GroupAccount/AddGroupPermission`, params)
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

export const deleteGroup = createAsyncThunk(`${sliceName}/delGroupPermission`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/GroupAccount/DelGroupPermission`, params)
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

export const updateGroup = createAsyncThunk(`${sliceName}/updGroupPermission`, async (params, { rejectWithValue, dispatch }) => {
    try {
        const response = await axios.post(`/api/GroupAccount/UpdGroupPermission`, params)
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

const filterData = (config) => {
    const {
        q = '',
        page = 1,
        role = null,
        perPage = 10,
        sort = 'asc',
        sortColumn = 'userName',
        data = []
    } = config

    const queryLowered = q.toLowerCase()

    const dataAsc = data.sort((a, b) => (a[sortColumn] < b[sortColumn] ? -1 : 1))

    const dataToFilter = sort === 'asc' ? dataAsc : dataAsc.reverse()

    const filteredData = dataToFilter.filter(
        user => (user.userID.toLowerCase().includes(queryLowered) ||
            user.userName.toLowerCase().includes(queryLowered) ||
            user.userNumber.value.toLowerCase().includes(queryLowered) ||
            user.description.toLowerCase().includes(queryLowered)
        ) && user.groupName?.find(_role => _role.value === (role || _role.value))
    )
    return {
        total: filteredData.length,
        users: paginateArray(filteredData, perPage, page)
    }
}

const initialState = () => {
    return {
        data: [],
        total: 1,
        params: {},
        allGroup: [],
        selectedGroup: null,
        isLoading: false
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        toggleIsLoading(state, { payload }) {
            const { status } = payload
            state.isLoading = status
        },
        getData(state, { payload }) {
            const result = filterData({ ...payload, data: [...state.allGroup] })
            state.data = result.users
            state.total = result.total
            state.params = payload
        },
        editGroup(state, { payload }) {
            state.selectedGroup = payload
        }
    },
    extraReducers: builder => {
        builder
            //vuexy
            .addCase(getAllData.fulfilled, (state, action) => {
                const { groupList, permissionList } = action.payload.data
                state.allGroup = groupList
                state.permissionList = permissionList
            })
            //addUser
            .addCase(addGroup.rejected, (state, action) => {
                ShowToast('新增失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(addGroup.fulfilled, (state, action) => {
                ShowToast('新增成功', '', 'success')
            })
            //deleteUser
            .addCase(deleteGroup.rejected, (state, action) => {
                ShowToast('刪除失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(deleteGroup.fulfilled, (state, action) => {
                ShowToast('刪除成功', '', 'success')
            })
            //updateUser
            .addCase(updateGroup.rejected, (state, action) => {
                ShowToast('修改失敗', action.payload.data?.msg, 'danger')
            })
            .addCase(updateGroup.fulfilled, (state, action) => {
                ShowToast('修改成功', '', 'success')
            })
    }
})

export const { toggleIsLoading, getData, editGroup } = slice.actions
export const isLoading = state => state[sliceName].isLoading
export default slice.reducer

