import { createContext, useContext, useState, useEffect, useRef } from 'react'
import { NumberCommasFormat } from '@CityAppHelper'
import { useSelector } from 'react-redux'

const DualScreenContext = createContext(null)
const DISPLAY_WINDOW_ID_KEY = 'customer_display_window_id'

export const DualScreenProvider = ({ children, brandConfig }) => {
  const [isConnected, setIsConnected] = useState(false)
  const displayWindowRef = useRef(null)

  // 直接從 Redux 獲取購物車資料
  const { shoppingCart: sale_shoppingCart } = { shoppingCart: [] }
  const { originalSale: return_return, shoppingCart: return_shoppingCart } = { originalSale: [], shoppingCart: [] }

  // 嘗試尋找已存在的窗口
  const findExistingWindow = (windowId) => {
    try {
      return window.open('', windowId)
    } catch (error) {
      console.error('尋找現有窗口時出錯:', error)
      return null
    }
  }

  // 發送消息到客戶顯示
  const sendMessage = (type, data = null) => {
    if (displayWindowRef.current && !displayWindowRef.current.closed) {
      displayWindowRef.current.postMessage(
        { customType: type, data },
        window.location.origin
      )
      return true
    }
    return false
  }

  // 顯示Logo
  const showLogo = () => {
    sendMessage('LOGO')
  }

  // 顯示結帳信息
  const showCheckout = (orderData) => {
    const formattedData = JSON.parse(JSON.stringify(orderData))

    const calcSellMode = (sellMode) => {
      if (sellMode === '2' || sellMode === '8') return -1
      else return 1
    }

    formattedData.items.forEach(item => {
      item.formattedTotalNum = NumberCommasFormat(item.totalSellNum)
      item.formattedSellCash = NumberCommasFormat(calcSellMode(item.sellMode) * item.sellCash)
    })

    formattedData.formattedTotal = NumberCommasFormat(formattedData.summary.sellCash)
    formattedData.formattedTotalNum = NumberCommasFormat(formattedData.summary.totalSellNum)

    // 發送格式化後的數據
    sendMessage('CHECKOUT', formattedData)
  }

  // 顯示退換貨信息
  const showReturn = (orderDataTmp) => {
    const orderData = JSON.parse(JSON.stringify(orderDataTmp))
    const { returnData, shoppingCart } = orderData

    const formattedData = {
      items: [],
      formattedTotal: 0,
      formattedTotalNum: 0
    }

    const calcSellMode = (sellMode) => {
      if (sellMode === '2' || sellMode === '8') return -1
      else return 1
    }

    returnData.saleList.forEach(item => {
      formattedData.items.push({ goodName: item.goodName, formattedTotalNum: NumberCommasFormat(-item.totalSellNum), formattedSellCash: NumberCommasFormat(-calcSellMode(item.sellMode) * item.sellCash) })
    })

    shoppingCart.data.forEach(item => {
      formattedData.items.push({ goodName: item.goodName, formattedTotalNum: NumberCommasFormat(item.totalSellNum), formattedSellCash: NumberCommasFormat(calcSellMode(item.sellMode) * item.sellCash) })
    })

    formattedData.formattedTotal = NumberCommasFormat(shoppingCart.summary.sellCash - returnData.summary.sellCash)
    formattedData.formattedTotalNum = NumberCommasFormat(shoppingCart.summary.totalSellNum - returnData.summary.totalSellNum)
    // 發送格式化後的數據
    sendMessage('CHECKOUT', formattedData)
  }

  // 同步顯示狀態到客戶顯示
  const syncDisplayState = () => {
    if (!isConnected) return

    // 檢查購物車是否有商品，決定顯示模式
    if (sale_shoppingCart.data && sale_shoppingCart.data.length > 0) {
      showCheckout({ items: sale_shoppingCart.data, summary: sale_shoppingCart.summary })
    } else if (return_return.saleList && return_return.saleList.length > 0) {
      showReturn({ returnData: return_return, shoppingCart: return_shoppingCart })
    } else {
      showLogo()
    }
  }

  // 監控購物車變化，自動同步顯示
  useEffect(() => {
    syncDisplayState()
  }, [sale_shoppingCart, return_return, return_shoppingCart, isConnected])

  // 初始化時嘗試恢復連接
  useEffect(() => {
    const savedWindowId = localStorage.getItem(DISPLAY_WINDOW_ID_KEY)

    if (savedWindowId) {
      const existingWindow = findExistingWindow(savedWindowId)

      if (existingWindow) {
        displayWindowRef.current = existingWindow
        setIsConnected(true)

        // 發送重新連接消息
        existingWindow.postMessage(
          { customType: 'RECONNECT', data: null },
          window.location.origin
        )

        // 重要：這裡不需要立即同步狀態，因為 syncDisplayState 會在 isConnected 變化後自動運行
      } else {
        localStorage.removeItem(DISPLAY_WINDOW_ID_KEY)
      }
    }
  }, [])

  // 處理來自客戶顯示的消息
  useEffect(() => {
    const handleMessage = (event) => {
      if (event.origin !== window.location.origin) return

      const { event: eventType } = event.data

      if (eventType === 'DISPLAY_READY') {
        // 客戶顯示準備好了，同步當前狀態
        syncDisplayState()
      }
    }

    window.addEventListener('message', handleMessage)
    return () => window.removeEventListener('message', handleMessage)
  }, [sale_shoppingCart])

  // 打開客戶顯示
  const openDisplay = () => {
    if (displayWindowRef.current && !displayWindowRef.current.closed) {
      displayWindowRef.current.focus()
      return true
    }

    const windowId = `customerDisplay_${Date.now()}`
    const newWindow = window.open(
      `${window.location.origin}/customer-display`,
      windowId,
      'width=1024,height=768,left=0,top=0'
    )

    if (newWindow) {
      displayWindowRef.current = newWindow
      setIsConnected(true)
      localStorage.setItem(DISPLAY_WINDOW_ID_KEY, windowId)

      newWindow.addEventListener('beforeunload', () => {
        displayWindowRef.current = null
        setIsConnected(false)
        localStorage.removeItem(DISPLAY_WINDOW_ID_KEY)
      })

      return true
    } else {
      console.error('無法開啟客戶顯示視窗。請確認瀏覽器允許彈出視窗。')
      return false
    }
  }

  // 關閉客戶顯示
  const closeDisplay = () => {
    if (displayWindowRef.current && !displayWindowRef.current.closed) {
      displayWindowRef.current.close()
      displayWindowRef.current = null
      setIsConnected(false)
      localStorage.removeItem(DISPLAY_WINDOW_ID_KEY)
    }
  }

  // 檢查連接狀態
  useEffect(() => {
    const checkConnection = () => {
      if (displayWindowRef.current && displayWindowRef.current.closed) {
        displayWindowRef.current = null
        setIsConnected(false)
        localStorage.removeItem(DISPLAY_WINDOW_ID_KEY)
      }
    }

    const interval = setInterval(checkConnection, 1000)
    return () => clearInterval(interval)
  }, [])

  const value = {
    isConnected,
    openDisplay,
    closeDisplay,
    showLogo,
    showCheckout,
    syncDisplayState
  }

  return (
    <DualScreenContext.Provider value={value}>
      {children}
    </DualScreenContext.Provider>
  )
}

// 使用客戶顯示的Hook
export function useDualScreen() {
  const context = useContext(DualScreenContext)
  if (!context) {
    throw new Error('useDualScreen必須在DualScreenProvider內使用')
  }
  return context
}
