import { lazy } from 'react'
import { Navigate, Link } from 'react-router-dom'

const MainResult = [
  {
    path: '/EcommerceMgmt',
    breadcrumb: '電商系統'
  },
  {
    path: '/EcommerceMgmt/EcommerceStoreEdit',
    component: () => <Navigate to='/EcommerceMgmt/EcommerceStoreEdit/List' replace />,
    breadcrumb: () => <Link to='/EcommerceMgmt/EcommerceStoreEdit/List'>電商門店增修</Link>
  },
  {
    path: '/EcommerceMgmt/EcommerceStoreEdit/:mode',
    component: lazy(() => import('../../views/EcommerceMgmt/EcommerceStoreEdit')),
    meta: {
      action: 'read',
      resource: '電商門店增修'
    },
    breadcrumb: null
  },
  {
    path: '/EcommerceMgmt/EcommerceStoreEdit/Single',
    component: () => <Navigate to='/EcommerceMgmt/EcommerceStoreEdit/List' replace />,
    breadcrumb: null
  },
  {
    path: '/EcommerceMgmt/EcommerceStoreEdit/Setting/:id',
    component: lazy(() => import('../../views/EcommerceMgmt/EcommerceStoreEdit')),
    meta: ({ mode }) => ({
      action: 'update',
      resource: '平台參數設定'
    }),
    breadcrumb: null
  },
  {
    path: '/EcommerceMgmt/EcommerceStoreEdit/:mode/:id',
    component: lazy(() => import('../../views/EcommerceMgmt/EcommerceStoreEdit')),
    meta: {
      action: 'read',
      resource: '1083'
    },
    breadcrumb: ({ match }) => (match.params.id === 'add' ? '新增' : '修改')
  },
  //PublishGoods
  {
    path: '/EcommerceMgmt/PublishGoods',
    component: () => <Navigate to='/EcommerceMgmt/PublishGoods/List' replace />,
    breadcrumb: () => <Link to='/EcommerceMgmt/PublishGoods/List'>平台商品管理</Link>
  },
  {
    path: '/EcommerceMgmt/PublishGoods/:mode',
    component: () => <Navigate to='/EcommerceMgmt/PublishGoods/add/C01001' replace />,
    breadcrumb: null
  },
  {
    path: '/EcommerceMgmt/PublishGoods/:mode/:id',
    component: lazy(() => import('../../views/EcommerceMgmt/PublishGoods')),
    meta: {
      action: 'read',
      resource: '1084'
    },
    breadcrumb: ({ match }) => (match.params.mode === 'add' ? '上架' : '修改')
  }
]

export default MainResult
