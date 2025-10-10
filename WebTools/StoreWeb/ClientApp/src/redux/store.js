// ** Redux Imports
import rootReducer from './rootReducer'
import { configureStore } from '@reduxjs/toolkit'
import { persistStore, persistReducer } from 'redux-persist'
import storage from 'redux-persist/lib/storage' // 使用 localStorage 進行存儲
import multiSliceTransform from './multiSliceTransform'

// 定義 persist 配置
const persistConfig = {
  key: 'root', // 存儲的 key
  storage, // 存儲方式，這裡使用 localStorage
  //whitelist: ['navbar'], // 想要持久化的 slice 名稱
  blacklist: ['auth'], // 不想持久化的 slice
  transforms: [multiSliceTransform]
}

// const getAccurateLocalStorageSize = () => {
//   let totalSize = 0
//   for (const key in localStorage) {
//     if (localStorage.hasOwnProperty(key)) {
//       const value = localStorage[key]
//       const itemSize = new Blob([key + value]).size // 將鍵和值轉換為 Blob 計算大小
//       totalSize += itemSize
//     }
//   }
//   return totalSize
// }

// console.log(`localStorage 精確占用大小: ${getAccurateLocalStorageSize()} bytes`)


// 包裝 reducer
const persistedReducer = persistReducer(persistConfig, rootReducer)

const store = configureStore({
  reducer: persistedReducer,
  middleware: getDefaultMiddleware => {
    return getDefaultMiddleware({
      serializableCheck: false
    })
  }
})

// 創建 persistor
const persistor = persistStore(store)

export { store, persistor }
