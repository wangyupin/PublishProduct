// ** Redux Imports
import { combineReducers, createAction } from '@reduxjs/toolkit'

// ** Reducers Imports
import navbar from './navbar'
import layout from './layout'
import auth from './authentication'

/* CityAPP */
import appSettings from '@src/views/application/store/appSettings'
import sysSettings from '@src/views/application/store/sysSettings'

//SettingMgmt
import SettingMgmt_UserAccount from '@src/views/SettingMgmt/UserAccount/store'
import SettingMgmt_GroupAccount from '@src/views/SettingMgmt/GroupAccount/store'

//EcommerceMgmt
import EcommerceMgmt_EcommerceStore from '@src/views/EcommerceMgmt/EcommerceStoreEdit/store'
import EcommerceMgmt_PublishGoods from '@src/views/EcommerceMgmt/PublishGoods/store'

//MainTableMgmt
import MainTableMgmt_GoodsInfo from '@src/views/MainTableMgmt/GoodsInfo/store'

const cityappReducer = {
  appSettings,
  sysSettings,

  // EcommerceMgmt
  EcommerceMgmt_EcommerceStore,
  EcommerceMgmt_PublishGoods,

  // SettingMgmt
  SettingMgmt_UserAccount,
  SettingMgmt_GroupAccount,

  // MainTableMgmt
  MainTableMgmt_GoodsInfo
}

// 定義一個全域清空 slice 的 action
export const clearSlice = createAction('slice/clear', (sliceName) => ({
  payload: sliceName
}))

const appReducer = combineReducers({
  /* Vuexy */
  auth,
  navbar,
  layout,
  /* CityAPP */
  ...cityappReducer
})

const rootReducer = (state, action) => {
  if (action.type === 'authentication/handleLogin' || action.type === 'authentication/handleLogout') {
    return appReducer(undefined, action)
  }
  return appReducer(state, action)
}

export default rootReducer

