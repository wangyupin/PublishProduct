// ** Redux Imports
import { createSlice, createAction, createAsyncThunk } from '@reduxjs/toolkit'

// ** UseJWT import to get config
import useJwt from '@src/auth/jwt/useJwt'
// ** Axios Imports
import axios from 'axios'

const config = useJwt.jwtConfig

const initialUser = () => {
  const item = window.localStorage.getItem('userData')
  //** Parse stored json or if none return initialValue
  return item ? JSON.parse(item) : {}
}

const initialMachineSet = () => {
  const item = window.localStorage.getItem('machineSet')
  //** Parse stored json or if none return initialValue
  return item ? JSON.parse(item) : {}
}

const doLogout = (state) => {
  localStorage.removeItem(config.storageTokenKeyName || 'accessToken')
  localStorage.removeItem(config.storageRefreshTokenKeyName || 'refreshToken')
}


const sliceName = 'auth'
import { GetClientCheck } from '@application/controller/ClientController'
export const getClientCheck = GetClientCheck(sliceName)
import { GetGroupProgram, EmployeeLogin, EmployeeLoginApi } from '@application/controller/MachineSetController'

export const getGroupProgram = GetGroupProgram(sliceName)

export const employeeLoginApi = (param) => EmployeeLoginApi(param)
export const employeeLogin = EmployeeLogin(sliceName)

export const updMacAddress = createAsyncThunk(`${sliceName}/updMacAddress`, async (params, { rejectWithValue }) => {
  try {
    const response = await axios.post(`/api/Auth/UpdMacAddress`, params)
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

export const authSlice = createSlice({
  name: 'authentication',
  initialState: {
    userData: initialUser(),
    groupProgram: {
      data: {}
    },
    machineSet: initialMachineSet()
  },
  reducers: {
    handleLogin: (state, action) => {
      const payload = action.payload
      // const data = { ...payload.userData, accessToken: payload.accessToken, refreshToken: payload.refreshToken, machineSet: payload.machineSet }
      state.userData = { ...payload.userData }
      state[config.storageTokenKeyName] = payload[config.storageTokenKeyName]
      state[config.storageRefreshTokenKeyName] = payload[config.storageRefreshTokenKeyName]
      state['machineSet'] = { ...payload.machineSet }
      localStorage.setItem('userData', JSON.stringify({ ...payload.userData }))
      localStorage.setItem(config.storageTokenKeyName, JSON.stringify(payload.accessToken))
      localStorage.setItem(config.storageRefreshTokenKeyName, JSON.stringify(payload.refreshToken))
      localStorage.setItem('machineSet', JSON.stringify(payload.machineSet))
    },
    handleLogout: state => {
      state.userData = {}
      state[config.storageTokenKeyName] = null
      state[config.storageRefreshTokenKeyName] = null
      // ** Remove user, accessToken & refreshToken from localStorage
      localStorage.removeItem('userData')
      localStorage.removeItem('menuCollapsed')
      localStorage.removeItem('i18nextLng')
      localStorage.removeItem('machineSet')
      localStorage.removeItem('templateData')

      window.dispatchEvent(new Event('userChanged'))

      useJwt.logout()
        .then((response) => {
          doLogout()
        })
        .catch(error => {
          console.log(error)
          doLogout()
        })
    }
  },
  extraReducers: builder => {
    builder
      .addCase(getClientCheck.fulfilled, (state, action) => {
        state.userData = { ...state.userData, storeName: action.payload.data[0]?.clientShort }
        localStorage.setItem('userData', JSON.stringify({ ...state.userData }))
      })
  }
})

export const { handleLogin, handleLogout } = authSlice.actions

export default authSlice.reducer
