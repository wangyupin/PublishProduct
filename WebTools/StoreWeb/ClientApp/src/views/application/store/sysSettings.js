// ** Redux Imports
import { createSlice, createAsyncThunk, createAction } from '@reduxjs/toolkit'

// ** Axios Imports
import axios from 'axios'

// ** Store & Actions
import { useSelector } from 'react-redux'

export const FontSizeList = ['text-truncate fw-bolder fs-5', 'text-truncate fw-bolder fs-4', 'text-truncate fw-bolder fs-3', 'text-truncate fw-bolder fs-2', 'text-truncate fw-bolder fs-1']

export const getClassNameWithFontSize = (className, fontSizeLevel, shiftFontSizeLevel = 0) => {
  const newFontSizeLevel = (fontSizeLevel + shiftFontSizeLevel) > FontSizeList.length ? FontSizeList.length - 1 : (fontSizeLevel + shiftFontSizeLevel) < 0 ? 0 : fontSizeLevel + shiftFontSizeLevel

  return `${className} ${FontSizeList[newFontSizeLevel]}`
}

const sliceName = 'sysSettings'

export const sysSettingsSlice = createSlice({
  name: sliceName,
  initialState: {
    hintMSG: '歡迎使用WebPos',
    fontSizeLevel: 2
  },
  reducers: {
    // sync mode：standard reducer logic, with auto-generated action types per reducer
    showHintMSG(state, action) {
      state.hintMSG = action.payload ? action.payload : '歡迎使用WebPos'
    },
    setFontSizeLevel(state, action) {
      state.fontSizeLevel = action.payload ? action.payload : 2
    }
  }
  // ,
  // extraReducers: builder => {
  //   builder
  //   // UpdMachineSetFrontEndMemo
  //   .addCase(UpdMachineSetFrontEndMemo.pending, (state, action) => {
  //     state.machineSet.isLoading = true
  //   })
  //   .addCase(UpdMachineSetFrontEndMemo.rejected, (state, action) => {
  //     state.machineSet.isLoading = false
  //   })
  //   .addCase(UpdMachineSetFrontEndMemo.fulfilled, (state, action) => {
  //     const stateData = state.machineSet
  //     stateData.data.frontEndMemo = action.payload.data.frontEndMemo
  //     localStorage.setItem('machineSet', action.payload.data)
  //     stateData.isLoading = false
  //   })
  // }
})

export const { showHintMSG, setFontSizeLevel } = sysSettingsSlice.actions

export default sysSettingsSlice.reducer
