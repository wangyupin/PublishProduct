// ** React Imports
import ReactDOMServer from "react-dom/server"
import { Fragment, useState, useEffect, useCallback, useRef, forwardRef, useImperativeHandle } from 'react'
import { Link } from 'react-router-dom'

// ** Reactstrap Imports
import {
  UncontrolledButtonDropdown, DropdownMenu, DropdownToggle, DropdownItem,
  NavItem, NavLink, UncontrolledTooltip
} from 'reactstrap'

// ** Icons Import
import { Flag, Sun, Moon, Airplay, Menu, ShoppingCart, Package, FileText } from 'react-feather'
import themeConfig from '@configs/themeConfig'

// ** Custom Hooks
import { useSkin } from '@hooks/useSkin'

// ** CityApp Utilty
import { toggleFullScreen } from '@CityAppHelper'

// ** Store & Actions
import { useSelector, useDispatch } from 'react-redux'
import { setFontSizeLevel, FontSizeList, getClassNameWithFontSize } from '@application/store/sysSettings'

// ** Dropdowns Imports
import UserDropdown from './UserDropdown'

const FontSizeDropdown = () => {
  // ** Store Vars
  const dispatch = useDispatch()

  //** Vars
  return (
    <UncontrolledButtonDropdown>
      <DropdownToggle color='flat-dark' caret size='sm' >設定字體</DropdownToggle>
      <DropdownMenu>
        <DropdownItem className={`${FontSizeList[1]}`} tag='a' onClick={() => dispatch(setFontSizeLevel(1))}>標準</DropdownItem>
        <DropdownItem className={`${FontSizeList[2]}`} tag='a' onClick={() => dispatch(setFontSizeLevel(2))}>大</DropdownItem>
        <DropdownItem className={`${FontSizeList[3]}`} tag='a' onClick={() => dispatch(setFontSizeLevel(3))}>特大</DropdownItem>
      </DropdownMenu>
    </UncontrolledButtonDropdown>
  )
}

const ThemeToggler = (props) => {
  // ** Props
  const { skin, setSkin } = props

  if (skin === 'dark') {
    return (
      <Fragment>
        <Sun id='themeToggler' className='ficon me-1 link-primary cursor-pointer' onClick={() => setSkin('light')} />
        <UncontrolledTooltip target='themeToggler' >活力模式</UncontrolledTooltip>
      </Fragment>
    )
  } else {
    return (
      <Fragment>
        <Moon id='themeToggler' className='ficon me-1 link-primary cursor-pointer' onClick={() => setSkin('dark')} />
        <UncontrolledTooltip target='themeToggler' >護眼模式</UncontrolledTooltip>
      </Fragment>
    )
  }
}

const FullScreenToggler = () => {
  return (
    <Fragment>
      <Airplay id='fullScreenToggler' className='ficon me-1 link-primary cursor-pointer' onClick={e => toggleFullScreen(false)} />
      <UncontrolledTooltip target='fullScreenToggler' >切換顯示</UncontrolledTooltip>
    </Fragment>
  )
}

const MenuVisibilityToggler = ({ setMenuVisibility }) => {
  return (
    <Fragment>
      <Menu id='menuVisibilityToggler' className='ficon me-1 link-success cursor-pointer' onClick={e => setMenuVisibility(true)} />
      {/* <UncontrolledTooltip target='menuVisibilityToggler' >顯示功能表</UncontrolledTooltip> */}
    </Fragment>
  )
}

const SaleToggler = () => {
  return (
    <Link to='SaleMgmt/Sale' target='_blank'>
      <ShoppingCart id='saleToggler' className='ficon me-1' />
      <UncontrolledTooltip target='saleToggler' >前台銷售作業</UncontrolledTooltip>
    </Link>
  )
}

const ProductQueryToggler = () => {
  return (
    <Link to='InventoryMgmt/ProductQuery' target='_blank'>
      <Package id='productQueryToggler' className='ficon me-1' />
      <UncontrolledTooltip target='productQueryToggler' >條碼明細查詢</UncontrolledTooltip>
    </Link>
  )
}

const SettlementRPTToggler = () => {
  return (
    <Link to='SaleMgmt/SettlementRPT' target='_blank'>
      <FileText id='SettlementRPT' className='ficon me-1' />
      <UncontrolledTooltip target='SettlementRPT'>銷售結帳表</UncontrolledTooltip>
    </Link>
  )
}

const Footer = ({ setMenuVisibility }) => {
  // ** Store Vars
  const store = useSelector(state => state.sysSettings)
  const { skin, setSkin } = useSkin()

  return (
    <Fragment>
      <div className='clearfix mb-0'>
        <div className='float-md-start fs-4 fw-bolder'>
          {store.hintMSG}
        </div>
        {/* <div className={`${getClassNameWithFontSize('float-md-start', 0)}`}>{store.hintMSG}</div> */}
        <div className='float-md-end '>
          <SaleToggler />
          <ProductQueryToggler />
          <SettlementRPTToggler />
          <FullScreenToggler />
          <ThemeToggler skin={skin} setSkin={setSkin} />
          {window.innerWidth < 1200 ? <MenuVisibilityToggler setMenuVisibility={setMenuVisibility} /> : ''}
          <UserDropdown />
          {/* <FontSizeDropdown /> */}
          {/* powered by {' '}
          <a href='http://system.gbtech.com.tw/' target='_blank' rel='noopener noreferrer'>
            GBTech
          </a>
          <span className='d-none d-sm-inline-block'>, All rights Reserved</span> */}
        </div>
      </div>
    </Fragment>
  )
}

export default Footer

