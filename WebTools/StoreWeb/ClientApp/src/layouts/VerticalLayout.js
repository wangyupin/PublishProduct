// ** React Imports
import { useState, useEffect } from 'react'

// ** Core Layout Import
// !Do not remove the Layout import
import Layout from '@layouts/VerticalLayout'

// ** Menu Items Array
import getMainMenu from '@src/navigation/vertical'

// ** Store & Actions
import { useSelector, useDispatch } from 'react-redux'

const VerticalLayout = props => {

  // ** Store Vars
  const authStore = useSelector(state => state.auth)

  const [menuData, setMenuData] = useState([])

  // ** For ServerSide navigation
  useEffect(() => {
    const menu = getMainMenu
    setMenuData(menu)
  }, [authStore.userData])

  return menuData.length > 0 ? (
    <Layout menuData={menuData} {...props}>
      {props.children}
    </Layout>
  ) : null
}

export default VerticalLayout
