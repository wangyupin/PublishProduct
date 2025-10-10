// CustomerDisplay.js - 修改版本
import { compareByFieldSpec } from '@fullcalendar/core'
import React, { useState, useEffect, useRef } from 'react'
import {
  Button,
  Row,
  Col,
  Card,
  CardHeader,
  CardBody,
  Carousel,
  CarouselItem,
  CarouselControl,
  CarouselIndicators
} from 'reactstrap'
import { useSystemSetting } from '@context/SystemSetting'

const CustomerDisplay = () => {
  // 使用 useRef 來保持數據的持久性
  const displayDataRef = useRef({ type: 'LOGO', data: null })
  // 使用 state 來觸發重渲染
  const [, forceUpdate] = useState({})
  const [isFullscreen, setIsFullscreen] = useState(false)

  // 輪播相關狀態
  const [activeIndex, setActiveIndex] = useState(0)
  const [animating, setAnimating] = useState(false)
  const [carouselImages, setCarouselImages] = useState([])

  // 您的圖片資料夾路徑
  const imagesFolder = '/assets/image/customerDisplay/'

  // 監聽消息
  useEffect(() => {
    const handleMessage = (event) => {
      if (event.origin !== window.location.origin) return

      const { customType, data } = event.data

      if (customType) {
        // 使用 ref 存儲數據，避免因重渲染導致的狀態丟失
        displayDataRef.current = { type: customType, data }
        // 強制重新渲染
        forceUpdate({})

        // 同時將數據保存到 sessionStorage 作為備份
        try {
          sessionStorage.setItem('customerDisplayData', JSON.stringify({ type: customType, data }))
        } catch (e) {
          console.error('無法保存到 sessionStorage:', e)
        }
      }
    }

    // 嘗試從 sessionStorage 恢復數據
    try {
      const savedData = sessionStorage.getItem('customerDisplayData')
      if (savedData) {
        const parsedData = JSON.parse(savedData)
        displayDataRef.current = parsedData
        forceUpdate({})
      }
    } catch (e) {
      console.error('無法從 sessionStorage 恢復數據:', e)
    }

    // 添加消息監聽器
    window.addEventListener('message', handleMessage)

    // 通知主視窗已準備就緒
    if (window.opener) {
      window.opener.postMessage({ event: 'DISPLAY_READY' }, '*')
    }

    // 清理函數
    return () => {
      window.removeEventListener('message', handleMessage)
    }
  }, [])

  // 監聽全螢幕狀態變化
  useEffect(() => {
    const handleFullscreenChange = () => {
      setIsFullscreen(
        document.fullscreenElement !== null ||
        document.webkitFullscreenElement !== null
      )
    }

    document.addEventListener('fullscreenchange', handleFullscreenChange)
    document.addEventListener('webkitfullscreenchange', handleFullscreenChange)

    return () => {
      document.removeEventListener('fullscreenchange', handleFullscreenChange)
      document.removeEventListener('webkitfullscreenchange', handleFullscreenChange)
    }
  }, [])

  // 全螢幕功能
  const enterFullscreen = () => {
    if (document.documentElement.requestFullscreen) {
      document.documentElement.requestFullscreen()
    } else if (document.documentElement.webkitRequestFullscreen) {
      document.documentElement.webkitRequestFullscreen()
    }
  }

  const moveToOtherScreen = async () => {
    try {
      window.screenDetails = await window.getScreenDetails()

      if (!window.screenDetails) {
        console.log('API不支持或權限被拒絕!')

        enterFullscreen()
        return
      }

      // 檢查是否有多個螢幕
      if (window.screenDetails.screens.length <= 1) {
        console.log('只檢測到一個螢幕!')
        enterFullscreen()
        return
      }

      // 找到第二個螢幕
      const otherScreen = window.screenDetails.screens.find(s => s !== window.screenDetails.currentScreen)

      // 將視窗移到第二個螢幕
      const width = window.outerWidth
      const height = window.outerHeight
      const left = otherScreen.availLeft + (otherScreen.availWidth / 2) - (width / 2)
      const top = otherScreen.availTop + (otherScreen.availHeight / 2) - (height / 2)

      window.moveTo(left, top)

      // 進入全螢幕模式
      setTimeout(() => {
        enterFullscreen()
      }, 300)

    } catch (error) {
      console.error('移動視窗時出錯:', error)
      enterFullscreen()
    }
  }

  useEffect(() => {
    // 檢查圖片是否存在的函數
    const checkImageExists = (url) => {
      return new Promise((resolve) => {
        const img = new Image()

        // 成功載入圖片時觸發
        img.onload = () => {
          resolve(true)
        }

        // 載入失敗時觸發
        img.onerror = () => {
          resolve(false)
        }

        // 開始載入圖片
        img.src = url
      })
    }

    // 嘗試載入多張圖片
    const loadImages = async () => {
      const potentialImages = []

      // 也可以嘗試其他常見命名模式
      for (let i = 1; i <= 20; i++) {  // 嘗試最多20張圖片
        potentialImages.push(`${imagesFolder}slide${i}.jpg`)
      }

      const validImages = []
      for (const imgPath of potentialImages) {
        if (await checkImageExists(imgPath)) {
          validImages.push(imgPath)
        }
      }

      // 更新狀態
      if (validImages.length > 0) {
        setCarouselImages(validImages)
      } else {
        console.warn('未找到任何輪播圖片')
      }
    }

    loadImages()
  }, [])

  // 輪播控制函數
  const next = () => {
    if (animating || carouselImages.length === 0) return
    const nextIndex = activeIndex === carouselImages.length - 1 ? 0 : activeIndex + 1
    setActiveIndex(nextIndex)
  }

  const previous = () => {
    if (animating || carouselImages.length === 0) return
    const nextIndex = activeIndex === 0 ? carouselImages.length - 1 : activeIndex - 1
    setActiveIndex(nextIndex)
  }

  const goToIndex = (newIndex) => {
    if (animating) return
    setActiveIndex(newIndex)
  }

  // 設置自動輪播
  useEffect(() => {
    const interval = setInterval(() => {
      if (displayDataRef.current.type === 'LOGO' && carouselImages.length > 0) {
        next()
      }
    }, 5000)

    return () => clearInterval(interval)
  }, [activeIndex, carouselImages.length])

  // 顯示Logo
  const renderLogo = () => {

    const { setting } = useSystemSetting()
    const { dualScreenDisplay } = setting

    // 顯示影片播放器
    if (dualScreenDisplay === 'video') {
      return (
        <div className="h-100 d-flex align-items-center justify-content-center">
          <video
            controls
            autoPlay
            className="w-100"
            style={{
              maxHeight: '80vh',
              objectFit: 'contain'
            }}
            loop
          >
            <source src={`${imagesFolder}video.mp4`} type="video/mp4" />
            您的瀏覽器不支援影片播放
          </video>
        </div>
      )
    }

    // 如果沒有圖片，顯示簡單的歡迎訊息
    if (carouselImages.length === 0) {
      return (
        <div className="d-flex flex-column align-items-center justify-content-center h-100 text-center">
          <div className="display-4 mb-3">歡迎光臨！</div>
          <p className="lead text-muted">感謝您的惠顧</p>
        </div>
      )
    }

    // 有圖片時顯示輪播
    return (
      <div className="h-100">
        <Carousel
          activeIndex={activeIndex}
          next={next}
          previous={previous}
          className="h-100 d-flex align-items-center justify-content-center"
        >
          <CarouselIndicators
            items={carouselImages.map((img, idx) => ({ key: idx }))}
            activeIndex={activeIndex}
            onClickHandler={goToIndex}
          />
          {carouselImages.map((item, index) => (
            <CarouselItem
              onExiting={() => setAnimating(true)}
              onExited={() => setAnimating(false)}
              key={index}
              className="h-100 "
            >
              <img
                src={item}
                alt={'圖片錯誤'}
                className="d-block"
                style={{
                  maxWidth: '100%',
                  maxHeight: '100vh',
                  objectFit: 'contain',
                  margin: '0 auto'
                }} />
            </CarouselItem>
          ))}
          <CarouselControl direction="prev" directionText="Previous" onClickHandler={previous} />
          <CarouselControl direction="next" directionText="Next" onClickHandler={next} />
        </Carousel>
      </div>
    )
  }

  // 顯示結帳信息
  const renderCheckout = () => {
    const { data } = displayDataRef.current

    if (!data || !data.items || data.items.length === 0) {
      return (
        <div className="display-container d-flex flex-column align-items-center justify-content-center h-100 text-center">
          <h2>結帳</h2>
          <p>暫無商品</p>
        </div>
      )
    }

    const scrollToBottom = (element) => {
      if (element) {
        setTimeout(() => {
          element.scrollTop = element.scrollHeight
        }, 100)
      }
    }

    return (
      <>
        <Row className='h-100'>
          <Col sm='4' className='d-flex flex-column px-0' style={{ height: '100%' }}>
            {/* 頂部標題 */}
            <div className="order-title px-3 pt-3 pb-1 bg-white border-bottom">
              <h2 className="m-0">您的購物明細</h2>
            </div>

            {/* 中間可滾動的商品明細區域 */}
            <div
              className="items-list p-0"
              style={{
                flex: '1',
                overflow: 'auto',
                height: '0' // 強制容器採用flex佈局的高度計算
              }}
              ref={scrollToBottom}
            >
              {data.items.map((item, index) => (
                <Row
                  key={index}
                  className="mx-0 py-50 border-bottom fs-4"
                >
                  <Col xs={6} className="ps-3">{item.goodName} x {item.formattedTotalNum}</Col>
                  <Col xs={3} className="text-center">{item.zuHeTypeName || ''}</Col>
                  <Col xs={3} className="text-end pe-3 fw-bold">${item.formattedSellCash}</Col>
                </Row>
              ))}
            </div>

            {/* 底部固定的總計區域 */}
            <div className="total-section bg-white" style={{ borderTop: '1px solid #000000' }}>
              <Row className="mx-0 py-2 fs-2 fw-bold">
                <Col xs={4} className="ps-3">總計:</Col>
                <Col xs={4} className="ps-3">{`x ${data.formattedTotalNum}`}</Col>
                <Col xs={4} className="text-end pe-3">${data.formattedTotal}</Col>
              </Row>
            </div>
          </Col>

          <Col sm='8' className='d-flex align-items-center px-0'>
            <img
              src="/assets/image/customerDisplay/checkout.jpg"
              alt="中央圖片"
              className="img-fluid"
              style={{
                maxHeight: '100%',
                maxWidth: '100%',
                objectFit: 'contain'
              }}
              loading="lazy"
            />
          </Col>
        </Row>
      </>
    )
  }

  return (
    <div className="customer-display vh-100 d-flex flex-column">

      {!isFullscreen && (
        <div className='py-1 d-flex align-items-center position-relative'>
          <Button
            color="primary"
            size="sm"
            className="fullscreen-button ms-auto"
            onClick={moveToOtherScreen}
            style={{ zIndex: 9999 }}
          >
            進入全螢幕
          </Button>
        </div>
      )}


      <div className="content flex-grow-1 d-flex flex-column overflow-hidden">
        {displayDataRef.current.type === 'LOGO' && renderLogo()}
        {displayDataRef.current.type === 'CHECKOUT' && renderCheckout()}
        {!displayDataRef.current.type && renderLogo()}
      </div>
    </div>
  )
}

export default CustomerDisplay
