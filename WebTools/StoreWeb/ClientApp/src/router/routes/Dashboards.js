import { lazy } from 'react'
import { Navigate, Link } from 'react-router-dom'
import { getMachineSet } from '@utils'
import { decryptAES } from '@CityAppHelper'

const DashboardRoutes = [
  {
    path: '/',
    breadcrumb: () => <Link to='/'>{
      decryptAES(localStorage.getItem('isFront'), process.env.REACT_APP_AES_KEY) === 'true' || decryptAES(localStorage.getItem('isFront'), process.env.REACT_APP_AES_KEY) === '' ? '前台' : '後台'
    }</Link>
  },
  {
    path: '/Dashboard',
    component: ({ isFront }) => {
      return lazy(() => import('../../views/dashboard/backend'))
    },
    breadcrumb: null
  }
]

export default DashboardRoutes
