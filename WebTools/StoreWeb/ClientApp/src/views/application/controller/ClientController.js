
// ** Redux Imports
import { createAsyncThunk } from '@reduxjs/toolkit'

// ** Axios Imports
import axios from 'axios'

export const GetClientList = (sliceName) => createAsyncThunk(`${sliceName}/GetClientList`, async params => {
  const response = await axios.post(`/api/Client/GetClientList`, params)
  return {
    data: response.data.data
  }
})

export const GetClientHelp = (sliceName) => createAsyncThunk(`${sliceName}/GetClientHelp`, async params => {
  const response = await axios.post(`/api/Client/GetClientHelp`, params)
  return {
    data: response.data.data
  }
})

export const GetClientCheck = (sliceName) => createAsyncThunk(`${sliceName}/GetClientCheck`, async params => {
  const response = await axios.post(`/api/Client/GetClientCheck`, params)
  return {
    data: response.data.data
  }
})

export const GetCatenationIDHelp = (sliceName) => createAsyncThunk(`${sliceName}/GetCatenationIDHelp`, async params => {
  const response = await axios.post(`/api/Client/GetCatenationIDHelp`, params)
  return {
    data: response.data.data
  }
})

//New
export const GetClientOptionsAll = (sliceName) => createAsyncThunk(`${sliceName}/GetClientOptionsAll`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.get(`/api/Client/GetClientOptionsAll`)
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

export const GetClientMeStoreOptions = (sliceName) => createAsyncThunk(`${sliceName}/GetClientMeStoreOptions`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.post(`/api/Client/GetClientMeStoreOptions`, params)
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

export const GetClientOptionsOnlyMainWH = (sliceName) => createAsyncThunk(`${sliceName}/GetClientOptionsOnlyMainWH`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.post(`/api/Client/GetClientOptionsOnlyMainWH`, params)
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

export const GetClientOptionsExcludeMainWH = (sliceName) => createAsyncThunk(`${sliceName}/GetClientOptionsExcludeMainWH`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.post(`/api/Client/GetClientOptionsExcludeMainWH`, params)
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


export const GetCatenationIDOptionsAll = (sliceName) => createAsyncThunk(`${sliceName}/GetCatenationIDOptionsAll`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.get(`/api/Client/GetCatenationIDOptionsAll`)
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

export const GetEStoreOptionsAll = (sliceName) => createAsyncThunk(`${sliceName}/GetEStoreOptionsAll`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.get(`/api/Client/GetEStoreOptionsAll`)
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

export const GetDepOptionsAll = (sliceName) => createAsyncThunk(`${sliceName}/GetDepOptionsAll`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.get(`/api/Client/GetDepOptionsAll`)
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
