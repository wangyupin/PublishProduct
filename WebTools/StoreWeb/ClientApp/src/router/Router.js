// ** React Imports
import { Suspense, useContext, lazy, Fragment, useRef, useState, useEffect } from 'react'
import { useSelector, useDispatch } from 'react-redux'

// ** Utils
import { isUserLoggedIn, clearUserData } from '@utils'
import { useLayout } from '@hooks/useLayout'
import { AbilityContext } from '@src/utility/context/Can'
import { useRouterTransition } from '@hooks/useRouterTransition'

// ** Custom Components
import LayoutWrapper from '@layouts/components/layout-wrapper'

// ** Router Components
import { BrowserRouter as AppRouter, Route, Routes, Navigate, resolvePath, Outlet, useParams, createBrowserRouter, RouterProvider } from 'react-router-dom'

// ** Routes & Default Routes
import { DefaultRoute, Routes as RouteSet } from './routes'

import { decryptAES } from '@CityAppHelper'


// ** Layouts
import BlankLayout from '@layouts/BlankLayout'
import VerticalLayout from '@src/layouts/VerticalLayout'
import HorizontalLayout from '@src/layouts/HorizontalLayout'

// ** CityApp Utilty
import { useHeartbeatService } from '@application/service/heartbeatService'

const Router = () => {
  // ** Hooks
  const { layout, setLayout, setLastLayout } = useLayout()
  const { transition, setTransition } = useRouterTransition()

  // ** ACL Ability Context
  const ability = useContext(AbilityContext)

  // ** Auth Store
  const authStore = useSelector(state => state.auth)
  const accessList = authStore.groupProgram.data.detailList
  useEffect(() => {
    if (accessList) ability.update(accessList)

    return (() => { })
  }, [accessList])

  // ** Default Layout
  const DefaultLayout = layout === 'horizontal' ? 'HorizontalLayout' : 'VerticalLayout'

  // ** All of the available layouts
  const Layouts = { BlankLayout, VerticalLayout, HorizontalLayout }

  // ** Current Active Item
  const currentActiveItem = null

  // ** Return Filtered Array of Routes & Paths
  const LayoutRoutesAndPaths = layout => {
    const LayoutRoutes = []
    const LayoutPaths = []

    if (RouteSet) {
      RouteSet.filter(route => {
        // ** Checks if Route layout or Default layout matches current layout
        if (route.layout === layout || (route.layout === undefined && DefaultLayout === layout)) {
          LayoutRoutes.push(route)
          LayoutPaths.push(route.path)
        }
      })
    }

    return { LayoutRoutes, LayoutPaths }
  }

  const NotAuthorized = lazy(() => import('@src/views/pages/misc/NotAuthorized'))

  // ** Init Error Component
  const Error = lazy(() => import('@src/views/pages/misc/Error'))

  /**
   ** Final Route Component Checks for Login & User Role and then redirects to the route
   */
  const FinalRoute = props => {
    const route = props.route

    let action, resource
    // ** Assign vars based on route meta
    if (route.meta) {
      action = route.meta.action ? route.meta.action : null
      resource = route.meta.resource ? route.meta.resource : null
    }
    if (
      (!isUserLoggedIn() && route.meta === undefined) ||
      (!isUserLoggedIn() && route.meta && !route.meta.authRoute && !route.meta.publicRoute)
    ) {
      /**
       ** If user is not Logged in & route meta is undefined
       ** OR
       ** If user is not Logged in & route.meta.authRoute, !route.meta.publicRoute are undefined
       ** Then redirect user to login
       */
      return <Navigate to='/login' />
    } else if (route.meta && route.meta.authRoute && isUserLoggedIn()) {
      // ** If route has meta and authRole and user is Logged in then redirect user to home page (DefaultRoute)
      return <Navigate to='/' />
    } else if (isUserLoggedIn() && route.meta && !ability.can(action || 'read', resource)) {
      // ** If user is Logged in and doesn't have ability to visit the page redirect the user to Not Authorized
      //clearUserData()
      return <Navigate to='/misc/not-authorized' />
    } else {
      // ** If none of the above render component
      useHeartbeatService()

      return <route.component {...props} />
    }
  }

  const ViewPage = ({ route, routerProps }) => {
    const params = useParams()
    let meta = route.meta
    if (typeof route.meta === 'function') meta = route.meta(params)
    Object.assign(routerProps, {
      meta
    })


    // ** Handle ProgressBar
    const [progress, setProgress] = useState(0)
    const LazyLoad = () => {
      useEffect(() => {
        setProgress(25)
        return () => setProgress(100)
      })
      return null
    }

    return (
      <Fragment>
        {route.layout === 'BlankLayout' ? (
          <Fragment>
            <FinalRoute route={route} />
          </Fragment>
        ) : (
          <LayoutWrapper
            layout={DefaultLayout}
            transition={transition}
            setTransition={setTransition}
            progress={progress}

            {...(route.appLayout ? {
              appLayout: route.appLayout
            } : {})}
            {...(route.meta ? {
              routeMeta: meta
            } : {})}
            {...(route.className ? {
              wrapperClass: route.className
            } : {})}
            {...(route.component ? {
              component: route.component
            } : {})}

          >
            <Suspense fallback={<LazyLoad />}>
              <FinalRoute route={{ ...route, meta }} setProgress={setProgress} isFront={route.isFront} />
            </Suspense>
          </LayoutWrapper>
        )}
      </Fragment>

    )
  }

  // ** Return Route to Render

  const ResolveRoutesV6 = () => {
    return Object.keys(Layouts).map((layout, index) => {

      // ** Convert Layout parameter to Layout Component
      // ? Note: make sure to keep layout and component name equal

      const LayoutTag = Layouts[layout]

      // ** Get Routes and Paths of the Layout
      const { LayoutRoutes, LayoutPaths } = LayoutRoutesAndPaths(layout)

      const routerProps = {}

      const isFront = decryptAES(localStorage.getItem('isFront'), process.env.REACT_APP_AES_KEY) === 'true' || decryptAES(localStorage.getItem('isFront'), process.env.REACT_APP_AES_KEY) === ''

      return ({
        element: <LayoutTag key={index}
          layout={layout}
          setLayout={setLayout}
          transition={transition}
          routerProps={routerProps}
          setLastLayout={setLastLayout}
          setTransition={setTransition}
          currentActiveItem={currentActiveItem}
          isFront={isFront}
        >
          <Outlet />
        </LayoutTag>,
        children: LayoutRoutes.map((route, index) => {

          let component = route.component
          if (typeof route.component === 'function') {
            const lazyComponent = route.component({ isFront })
            if (lazyComponent.$$typeof === Symbol.for('react.lazy')) component = lazyComponent
          }

          return {
            path: route.path,
            exact: route.exact === true,
            element: <ViewPage route={{ ...route, component, isFront }} routerProps={routerProps} />,
            errorElement: <Error />
          }
        })

      })
    })
  }


  const router = createBrowserRouter([
    {
      path: '/',
      element: isUserLoggedIn() ? <Navigate to={DefaultRoute} /> : <Navigate to='/login' />
    },
    {
      path: '/misc/not-authorized',
      element: <Layouts.BlankLayout>
        <NotAuthorized />
      </Layouts.BlankLayout>
    },
    ...ResolveRoutesV6(),
    {
      path: '*',
      element: <Error />
    }
  ])

  return (
    <RouterProvider router={router} />
  )
}

export default Router
