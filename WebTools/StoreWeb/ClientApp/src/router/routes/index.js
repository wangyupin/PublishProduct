import { lazy } from 'react'
import themeConfig from '@configs/themeConfig'

import PagesRoutes from './Pages'

import MainTableMgmt from './MainTableMgmt.js'
import Dashboards from './Dashboards.js'
import SettingMgmt from './SettingMgmt.js'

import EcommerceMgmt from './EcommerceMgmt.js'

// ** Document title
const TemplateTitle = `'%s - ${themeConfig.app.appName} V0.1'`

// ** Default Route
const DefaultRoute = '/Dashboard'

// ** Merge Routes
const Routes = [
  ...PagesRoutes,
  ...Dashboards,
  ...MainTableMgmt,
  ...EcommerceMgmt,
  ...SettingMgmt
]

export { DefaultRoute, TemplateTitle, Routes }
