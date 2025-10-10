// ** Redux Imports
import { createSlice, createAsyncThunk, createAction } from '@reduxjs/toolkit'

// ** Axios Imports
import axios from 'axios'

import * as MachineSetController from '@application/controller/MachineSetController'

const sliceName = 'appSettings'

export const getMachineSet = MachineSetController.getMachineSet(sliceName)
export const UpdMachineSetFrontEndMemo = MachineSetController.UpdMachineSetFrontEndMemo(sliceName)
export const UpdHeartbeat = MachineSetController.UpdHeartbeat(sliceName)

export const appSettingsSlice = createSlice({
  name: sliceName,
  initialState: {
    machineSet: {
      isLoading: false,
      data: {}
    },
    serviceStatus: {
      hqSrv: {},
      printer: {},
      edc: {}
    },
    isSingleSizeMode: false
  },
  reducers: {
    // sync mode：standard reducer logic, with auto-generated action types per reducer
    saveMachineSet(state, action) {
      state.machineSet.data = action.payload
      localStorage.setItem('machineSet', action.payload.data)
    }
  },
  extraReducers: builder => {
    builder
      // UpdHeartbeat
      .addCase(UpdHeartbeat.fulfilled, (state, action) => {
        state.serviceStatus = action.payload.data.serviceStatus
      })
      // getMachineSet
      .addCase(getMachineSet.pending, (state, action) => {
        state.machineSet.isLoading = true
      })
      .addCase(getMachineSet.rejected, (state, action) => {
        state.machineSet.isLoading = false
      })
      .addCase(getMachineSet.fulfilled, (state, action) => {
        const stateData = state.machineSet
        stateData.data = action.payload.data
        localStorage.setItem('machineSet', action.payload.data)
        stateData.isLoading = false
      })
      // UpdMachineSetFrontEndMemo
      .addCase(UpdMachineSetFrontEndMemo.pending, (state, action) => {
        state.machineSet.isLoading = true
      })
      .addCase(UpdMachineSetFrontEndMemo.rejected, (state, action) => {
        state.machineSet.isLoading = false
      })
      .addCase(UpdMachineSetFrontEndMemo.fulfilled, (state, action) => {
        const stateData = state.machineSet
        stateData.data.frontEndMemo = action.payload.data.frontEndMemo
        localStorage.setItem('machineSet', action.payload.data)
        stateData.isLoading = false
      })
  }
})

export const { saveMachineSet } = appSettingsSlice.actions

export default appSettingsSlice.reducer
