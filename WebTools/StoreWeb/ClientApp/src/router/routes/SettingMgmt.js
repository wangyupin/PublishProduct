import { lazy } from 'react'
import { Navigate, Link } from 'react-router-dom'


const MainResult = [
    {
        path: '/SettingMgmt',
        breadcrumb: '系統支援'
    },
    {
        path: '/SettingMgmt/UserAccount',
        exact: true,
        component: () => <Navigate to='/SettingMgmt/UserAccount/List' replace />,
        breadcrumb: () => <Link to='/SettingMgmt/UserAccount/List'>使用者帳號管理</Link>
    },
    {
        path: '/SettingMgmt/UserAccount/:mode',
        component: lazy(() => import('../../views/SettingMgmt/UserAccount')),
        meta: {
            action: 'read',
            resource: '15'
        },
        breadcrumb: null
    },
    {
        path: '/SettingMgmt/UserAccount/Single',
        exact: true,
        component: () => <Navigate to='/SettingMgmt/UserAccount/List' replace />,
        breadcrumb: null

    },
    {
        path: '/SettingMgmt/UserAccount/:mode/:id',
        component: lazy(() => import('../../views/SettingMgmt/UserAccount')),
        meta: {
            action: 'read',
            resource: '15'
        },
        breadcrumb: ({ match }) => (match.params.id === 'add' ? '新增' : '修改')
    },
    {
        path: '/SettingMgmt/GroupAccount',
        component: lazy(() => import('../../views/SettingMgmt/GroupAccount')),
        meta: {
            action: 'read',
            resource: '16'
        },
        breadcrumb: () => <Link to='/SettingMgmt/GroupAccount'>群組權限管理</Link>
    }
]

export default MainResult
