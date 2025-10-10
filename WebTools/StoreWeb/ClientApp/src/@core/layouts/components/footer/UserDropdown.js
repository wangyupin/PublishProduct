// ** React Imports
import { Link } from 'react-router-dom'
import { useEffect, useState } from 'react'

// ** Custom Components
import Avatar from '@components/avatar'

// ** Utils
import { isUserLoggedIn } from '@utils'

// ** Store & Actions
import { useDispatch } from 'react-redux'
import { handleLogout } from '@store/authentication'

// ** Third Party Components
import { User, Mail, CheckSquare, MessageSquare, Settings, CreditCard, HelpCircle, Power } from 'react-feather'

// ** Reactstrap Imports
import { UncontrolledDropdown, UncontrolledButtonDropdown, DropdownMenu, DropdownToggle, DropdownItem } from 'reactstrap'

// ** Default Avatar Image
import defaultAvatar from '@src/assets/images/portrait/small/avatar-s-6.jpg'

const UserDropdown = () => {
  // ** Store Vars
  const dispatch = useDispatch()

  // ** State
  const [userData, setUserData] = useState(null)

  //** ComponentDidMount
  useEffect(() => {
    if (isUserLoggedIn() !== null) {
      setUserData(JSON.parse(localStorage.getItem('userData')))
    }
  }, [])

  //** Vars
  const userAvatar = (userData && userData.avatar) || defaultAvatar

  return (
    <UncontrolledButtonDropdown >
      <DropdownToggle href='/' tag='a' className='text-dark fw-bloder' onClick={e => e.preventDefault()}>
        {/* <span className='user-name fw-bold'>{(userData && userData['username'])}</span>
          <span className='user-status'>{(userData && userData['userId'])}</span> */}
        {userData && userData['username']} {(userData && userData['userId'])}
        <Avatar img={defaultAvatar} className='' style={{ marginLeft : 8 }} color='light-primary' content={(userData && userData['userId']) || ' '} initials status='online' />
      </DropdownToggle>
      <DropdownMenu>
        <DropdownItem tag={Link} to='/login' onClick={() => dispatch(handleLogout())}>
          <Power size={14} className='me-75' />
          <span className='align-middle'>登出</span>
        </DropdownItem>
      </DropdownMenu>
    </UncontrolledButtonDropdown>
  )
}

export default UserDropdown
