// ** React Imports
import { Fragment, useEffect } from 'react'

// ** Custom Components
import NavbarUser from './NavbarUser'

// ** Third Party Components
import { Menu } from 'react-feather'

// ** Reactstrap Imports
import { NavItem, NavLink } from 'reactstrap'

const ThemeNavbar = props => {
  // ** Props
  const { skin, setSkin, setMenuVisibility, isFront } = props

  return (
    <Fragment>
      {/* <ul className='navbar-nav d-xl-none'>
        <NavItem className='mobile-menu me-auto d-flex align-items-center'>
          <NavLink className='nav-menu-main menu-toggle hidden-xs is-active' onClick={() => setMenuVisibility(true)}>
            <Menu className='ficon' />
          </NavLink>
        </NavItem>
      </ul> */}
      <NavbarUser skin={skin} setSkin={setSkin} setMenuVisibility={setMenuVisibility} isFront={isFront} />
    </Fragment>
  )
}

export default ThemeNavbar
