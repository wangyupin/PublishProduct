import { lazy } from 'react'

const PagesRoutes = [
  {
    path: '/login',
    component: lazy(() => import('../../views/pages/authentication/Login')),
    layout: 'BlankLayout',
    meta: {
      authRoute: true
    }
  },
  // {
  //   path: '/forgot-password',
  //   component: lazy(() => import('../../views/pages/authentication/ForgotPassword')),
  //   layout: 'BlankLayout',
  //   meta: {
  //     authRoute: true
  //   }
  // },
  // {
  //   path: '/pages/reset-password-v1',
  //   component: lazy(() => import('../../views/pages/authentication/ResetPasswordV1')),
  //   layout: 'BlankLayout'
  // },
  // {
  //   path: '/pages/account-settings',
  //   component: lazy(() => import('../../views/pages/account-settings'))
  // },
  {
    path: '/misc/coming-soon',
    component: lazy(() => import('../../views/pages/misc/ComingSoon')),
    layout: 'BlankLayout',
    meta: {
      publicRoute: true
    }
  },
  {
    path: '/misc/not-authorized',
    component: lazy(() => import('../../views/pages/misc/NotAuthorized')),
    layout: 'BlankLayout',
    meta: {
      publicRoute: true
    }
  },
  {
    path: '/misc/maintenance',
    component: lazy(() => import('../../views/pages/misc/Maintenance')),
    layout: 'BlankLayout'
  },
  {
    path: '/error',
    component: lazy(() => import('../../views/pages/misc/Error')),
    layout: 'BlankLayout'
  },
  {
    path: '/customer-display',
    component: lazy(() => import('../../views/pages/others/CustomerDisplay')),
    layout: 'BlankLayout'
  }
]

export default PagesRoutes
