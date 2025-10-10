
// ** Redux Imports
import { createAsyncThunk } from '@reduxjs/toolkit'

// ** Axios Imports
import axios from 'axios'

export const UsersDataGetList = (sliceName) => createAsyncThunk(`${sliceName}/UsersDataGetList`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.get(`/api/AccountSetting/UsersDataGetList`, params)
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

export const GetGroupProgramDetail = (sliceName) => createAsyncThunk(`${sliceName}/GetGroupProgramDetail`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/GetGroupProgramDetail`, params)
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

export const AddUsersData = (sliceName) => createAsyncThunk(`${sliceName}/AddUsersData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/AddUsersData`, params)
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

export const UpdUsersData = (sliceName) => createAsyncThunk(`${sliceName}/UpdUsersData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/UpdUsersData`, params)
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

export const DelUsersData = (sliceName) => createAsyncThunk(`${sliceName}/DelUsersData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/DelUsersData`, params)
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

export const AddUserGroupData = (sliceName) => createAsyncThunk(`${sliceName}/AddUserGroupData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/AddUserGroupData`, params)
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

export const DelUserGroupData = (sliceName) => createAsyncThunk(`${sliceName}/DelUserGroupData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/DelUserGroupData`, params)
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

export const UpdUserGroupData = (sliceName) => createAsyncThunk(`${sliceName}/UpdUserGroupData`, async (params, { rejectWithValue }) => {
    try {
        const response = await axios.post(`/api/AccountSetting/UpdUserGroupData`, params)
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
