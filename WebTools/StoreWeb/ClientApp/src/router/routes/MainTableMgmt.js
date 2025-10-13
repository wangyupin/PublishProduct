import { exact } from 'prop-types'
import { lazy } from 'react'
import { Navigate, Link } from 'react-router-dom'
import { Breadcrumb } from 'reactstrap'

const MainResult = [
  {
    path: '/MainTableMgmt',
    breadcrumb: '主檔系統'
  },
  {
    path: '/MainTableMgmt/GoodsInfo',
    component: () => <Navigate to='/MainTableMgmt/GoodsInfo/List' replace />,
    breadcrumb: () => <Link to='/MainTableMgmt/GoodsInfo/List'>商品基本資料</Link>
  },
  {
    path: '/MainTableMgmt/GoodsInfo/:mode',
    component: lazy(() => import('../../views/MainTableMgmt/GoodsInfo')),
    meta: {
      action: 'read',
      resource: '4'
    },
    breadcrumb: null
  },
  {
    path: '/MainTableMgmt/GoodsInfo/Single',
    component: () => <Navigate to='/MainTableMgmt/GoodsInfo/List' replace />,
    breadcrumb: null
  },
  {
    path: '/MainTableMgmt/GoodsInfo/:mode/:id',
    component: lazy(() => import('../../views/MainTableMgmt/GoodsInfo')),
    meta: ({ id }) => ({
      action: 'read',
      resource: '4'
    }),
    breadcrumb: ({ match }) => (match.params.id === 'add' ? '新增' : '修改')
  },
  {
    path: '/MainTableMgmt/GoodsInfo/:mode/:id/:copy',
    component: lazy(() => import('../../views/MainTableMgmt/GoodsInfo')),
    meta: ({ id }) => ({
      action: 'read',
      resource: '4'
    }),
    breadcrumb: ({ match }) => (match.params.copy === 'copy' ? '複製' : '')
  }
]

export default MainResult
