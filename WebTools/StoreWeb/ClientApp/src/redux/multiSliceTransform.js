import createTransform from 'redux-persist/es/createTransform'

const multiSliceTransform = createTransform(
    // 保存到 storage 前
    (inboundState, key) => {
      if (key === 'navbar') {
        // 僅保存 auth 的 token
        return { breadcrumbsActive: inboundState.breadcrumbsActive, activeTab: inboundState.activeTab}
      }
      return inboundState // 對其他 Slice 不進行改動
    },
    // 從 storage 恢復前
    (outboundState, key) => {
      return outboundState
    },
    { whitelist: ['navbar'] } // 僅對指定的 Slice 生效
)

export default multiSliceTransform