// ** React Imports
import { Fragment, useState } from 'react'
import { Link } from 'react-router-dom'

// ** Dropdowns Imports
import UserDropdown from './UserDropdown'

// ** Third Party Components
import {
  Sun, Moon, Wifi, WifiOff, ShoppingCart, Package, FileText,
  Airplay,
  Maximize, Minimize,
  Maximize2, Minimize2, Menu, Columns
} from 'react-feather'

// ** Reactstrap Imports
import { NavItem, NavLink, UncontrolledTooltip, Row, Col } from 'reactstrap'

// ** CityApp Utilty
import { useSkin } from '@hooks/useSkin'
import { useHeartbeatService } from '@application/service/heartbeatService'
import { toggleFullScreen, toggleScreenZoomIn } from '@CityAppHelper'
import { useSelector, useDispatch } from 'react-redux'
import Breadcrumbs from './Breadcrumb'
import NavbarSearch from './NavbarSearch'
import NavbarBookmarks from './NavbarBookmarks'
import NewTabs from './Tabs'
import { useDualScreen } from '@context/DualScreen'

const ThemeToggler = (props) => {
  // ** Props
  const { skin, setSkin } = props

  if (skin === 'dark') {
    return (
      <Fragment>
        <Sun id='themeToggler' className='ficon me-1 link-primary cursor-pointer' onClick={() => setSkin('light')} size={20} />
        <UncontrolledTooltip target='themeToggler' >活力模式</UncontrolledTooltip>
      </Fragment>
    )
  } else {
    return (
      <Fragment>
        <Moon id='themeToggler' className='ficon me-1 link-primary cursor-pointer' onClick={() => setSkin('dark')} size={20} />
        <UncontrolledTooltip target='themeToggler' >護眼模式</UncontrolledTooltip>
      </Fragment>
    )
  }
}

const FullScreenToggler = () => {
  return (
    <Fragment>
      <NavItem>
        <Airplay id='fullScreenToggler' className='ficon me-1 link-primary cursor-pointer' onClick={e => toggleFullScreen(false)} size={20} />
        <UncontrolledTooltip target='fullScreenToggler' >切換顯示</UncontrolledTooltip>
      </NavItem>
    </Fragment>
  )
}

const MenuVisibilityToggler = ({ setMenuVisibility }) => {
  return (
    <Fragment>
      <NavItem>
        <Menu id='menuVisibilityToggler' className='ficon me-1 link-success cursor-pointer' onClick={e => setMenuVisibility(true)} size={20} />
        {/* <UncontrolledTooltip target='menuVisibilityToggler' >顯示功能表</UncontrolledTooltip> */}
      </NavItem>
    </Fragment>
  )
}

const SaleToggler = () => {
  return (
    <NavItem>
      <Link to='SaleMgmt/Sale' target='_blank'>
        <ShoppingCart id='saleToggler' className='ficon me-1' size={20} />
        <UncontrolledTooltip target='saleToggler' >前台銷售作業</UncontrolledTooltip>
      </Link>
    </NavItem>
  )
}

const ProductQueryToggler = () => {
  return (
    <Link to='InventoryMgmt/ProductQuery' target='_blank'>
      <Package id='productQueryToggler' className='ficon me-1' size={20} />
      <UncontrolledTooltip target='productQueryToggler' >條碼明細查詢</UncontrolledTooltip>
    </Link>
  )
}

const SettlementRPTToggler = () => {
  return (
    <Link to='SaleMgmt/SettlementRPT' target='_blank'>
      <FileText id='SettlementRPT' className='ficon me-1' size={20} />
      <UncontrolledTooltip target='SettlementRPT'>銷售結帳表</UncontrolledTooltip>
    </Link>
  )
}

const DualScreenToggler = () => {
  const { isConnected, openDisplay, closeDisplay } = useDualScreen()
  return (
    <Fragment>
      <NavItem>
        <Columns id='dualScreenToggler' className='ficon me-1 link-primary cursor-pointer' onClick={e => {
          if (isConnected) {
            closeDisplay()
          } else {
            openDisplay()
          }
        }} size={20} />
        <UncontrolledTooltip target='dualScreenToggler' >雙螢幕顯示</UncontrolledTooltip>
      </NavItem>
    </Fragment>
  )
}

const NavbarUser = ({ setMenuVisibility, isFront }) => {
  // ** Props
  const store = useSelector(state => state.sysSettings)
  const { skin, setSkin } = useSkin()

  return (
    <Fragment>

      <Row className='w-100 m-0 p-0' >
        <Col md='8' className='p-0'>
          {/* <div className='float-md-start fs-4 fw-bolder me-4'> */}
          {/* {store.hintMSG} */}
          {/* <Breadcrumbs>
              </Breadcrumbs>
            </div> */}
          <NewTabs></NewTabs>
        </Col>
        <Col md='4' className='d-flex align-content-center p-0'>
          {/* <div className={`${getClassNameWithFontSize('float-md-start', 0)}`}>{store.hintMSG}</div> */}
          <ul className='d-flex w-100 nav navbar-nav align-items-center justify-content-end'>
            <NavbarBookmarks setMenuVisibility={setMenuVisibility} />
            <NavbarSearch isFront={isFront} />

            <SaleToggler />

            {/* <ProductQueryToggler />
              <SettlementRPTToggler /> */}

            <FullScreenToggler />

            <DualScreenToggler />

            {/* <ThemeToggler skin={skin} setSkin={setSkin} /> */}

            {window.innerWidth < 1200 ? <MenuVisibilityToggler setMenuVisibility={setMenuVisibility} /> : ''}

            <UserDropdown />
          </ul>
        </Col>
      </Row>


    </Fragment>
  )
}
export default NavbarUser
