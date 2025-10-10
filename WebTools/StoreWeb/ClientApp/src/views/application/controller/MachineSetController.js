
// ** Redux Imports
import { createAsyncThunk } from '@reduxjs/toolkit'

// ** Axios Imports
import axios from 'axios'

export const getMachineSet = (sliceName) => createAsyncThunk(`${sliceName}/GetMachineSet`, async params => {
  const response = await axios.post(`/api/machineSet/GetMachineSet`, params)
  return {
    data: response.data.data
  }
})

export const UpdMachineSetFrontEndMemo = (sliceName) => createAsyncThunk(`${sliceName}/UpdMachineSetFrontEndMemo`, async params => {
  const response = await axios.post(`/api/machineSet/UpdMachineSetFrontEndMemo`, params)
  return {
    data: response.data.data
  }
})

export const UpdHeartbeat = (sliceName) => createAsyncThunk(`${sliceName}/UpdHeartbeat`, async params => {
  const response = await axios.post(`/api/machineSet/UpdHeartbeat`, params)
  return {
    data: response.data.data
  }
})

export const GetGroupProgram = (sliceName) => createAsyncThunk(`${sliceName}/GetGroupProgram`, async params => {
  const response = await axios.post(`/api/machineSet/GetGroupProgram`, params)
  return {
    data: response.data.data
  }
})


export const EmployeeLoginApi = (params) => axios.post(`/api/machineSet/EmployeeLogin`, params)
export const EmployeeLogin = (sliceName) => createAsyncThunk(`${sliceName}/EmployeeLogin`, async params => {
  const response = await axios.post(`/api/machineSet/EmployeeLogin`, params)
  return {
    data: response.data.data
  }
})