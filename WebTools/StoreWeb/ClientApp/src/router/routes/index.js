import { lazy } from 'react'
import themeConfig from '@configs/themeConfig'

import PagesRoutes from './Pages'

import MainTableMgmt from './MainTableMgmt.js'
import Dashboards from './Dashboards.js'
import SettingMgmt from './SettingMgmt.js'

// ** Document title
const TemplateTitle = `'%s - ${themeConfig.app.appName} V0.1'`

// ** Default Route
const DefaultRoute = '/Dashboard'

// ** Merge Routes
const Routes = [
  ...PagesRoutes,
  ...Dashboards,
  ...MainTableMgmt,
  ...SettingMgmt
]

export { DefaultRoute, TemplateTitle, Routes }
