// ** React Imports
import { useEffect, useState, useContext } from 'react'

// ** Third Party Components
import axios from 'axios'
import classnames from 'classnames'
import * as Icon from 'react-feather'
import { AbilityContext } from '@src/utility/context/Can'

// ** Reactstrap Imports
import { NavItem, NavLink } from 'reactstrap'

// ** Store & Actions
import { useDispatch } from 'react-redux'
import { handleSearchQuery, updateSuggestion } from '@store/navbar'

// ** Custom Components
import Autocomplete from '@components/autocomplete'

import { getUserData } from '@utils'
import getMainMenu from '@src/navigation/vertical'

const NavbarSearch = ({ isFront }) => {
  const ability = useContext(AbilityContext)

  const canViewMenuGroup = item => {

    const hasAnyVisibleChild = item.children && item.children.some(i => (i.children ? (i.children && i.children.some(j => ability.can(j.action, j.resource))) : ability.can(i.action, i.resource)))

    if (!(item.action && item.resource)) {
      return hasAnyVisibleChild
    }
    return ability.can(item.action, item.resource) && hasAnyVisibleChild
  }

  const canViewMenuItem = item => {
    return ability.can(item.action, item.resource)
  }


  // ** Store Vars
  const dispatch = useDispatch()

  // ** States
  const [suggestions, setSuggestions] = useState([])
  const [navbarSearch, setNavbarSearch] = useState(false)

  // ** Removes query in store
  const handleClearQueryInStore = () => dispatch(handleSearchQuery(''))

  // ** Function to handle external Input click
  const handleExternalClick = () => {
    if (navbarSearch === true) {
      setNavbarSearch(false)
      handleClearQueryInStore()
    }
  }

  // ** Function to clear input value
  const handleClearInput = setUserInput => {
    if (!navbarSearch) {
      setUserInput('')
      handleClearQueryInStore()
    }
  }

  // ** Function to close search on ESC & ENTER Click
  const onKeyDown = e => {
    if (e.keyCode === 27 || e.keyCode === 13) {
      setTimeout(() => {
        setNavbarSearch(false)
        handleClearQueryInStore()
      }, 1)
    }
  }

  // ** Function to handle search suggestion Click
  const handleSuggestionItemClick = () => {
    setNavbarSearch(false)
    handleClearQueryInStore()
  }

  // ** Function to handle search list Click
  const handleListItemClick = (func, link, e) => {
    func(link, e)
    setTimeout(() => {
      setNavbarSearch(false)
    }, 1)
    handleClearQueryInStore()
  }

  const flattenMenuL2 = (menu) => {
    const result = []
    menu.forEach(item => {
      if (!canViewMenuGroup(item)) return
      const topItem = {
        groupTitle: item.title,
        data: []
      }
      result.push(topItem)
      const collectChildren = (menuItem) => {
        if (menuItem.children) {
          menuItem.children.forEach(child => {
            if (child.children && canViewMenuGroup(child)) {
              collectChildren(child)
            } else {
              if (!canViewMenuItem(child)) return
              topItem.data.push({ title: child.title, link: child.navLink, id: child.id })
            }
          })
        }
      }
      collectChildren(item)
    })

    return result
  }

  const collectChildren = (result, menuItem) => {
    if (menuItem.children) {
      menuItem.children.forEach(child => {
        if (child.children && canViewMenuGroup(child)) {
          collectChildren(result, child)
        } else {
          if (!canViewMenuItem(child)) return
          result.push({ title: child.title, link: child.navLink, id: child.id })
        }
      })
    }
  }

  useEffect(() => {
    const arr = flattenMenuL2(getMainMenu)
    const arr1 = []
    collectChildren(arr1, { children: getMainMenu })
    dispatch(updateSuggestion(arr1))
    setSuggestions(arr)
  }, [])

  return (
    <NavItem className='nav-search' onClick={() => setNavbarSearch(true)}>
      <Icon.Search className='ficon me-1 link-primary cursor-pointer' size={20} />
      <div
        className={classnames('navbar-search search-input', {
          open: navbarSearch === true
        })}
      >
        <div className='search-input-icon'>
          <Icon.Search />
        </div>
        {navbarSearch ? (
          <Autocomplete
            className='form-control'
            suggestions={suggestions}
            filterKey='title'
            filterHeaderKey='groupTitle'
            grouped={true}
            placeholder='查找功能...'
            autoFocus={true}
            onSuggestionItemClick={handleSuggestionItemClick}
            externalClick={handleExternalClick}
            clearInput={(userInput, setUserInput) => handleClearInput(setUserInput)}
            onKeyDown={onKeyDown}
            onChange={e => dispatch(handleSearchQuery(e.target.value))}
            customRender={(item, i, filteredData, activeSuggestion, onSuggestionItemClick, onSuggestionItemHover) => {
              const IconTag = Icon[item.icon ? item.icon : 'X']
              return (
                <li
                  className={classnames('suggestion-item', {
                    active: filteredData.indexOf(item) === activeSuggestion
                  })}
                  key={i}
                  onClick={e => handleListItemClick(onSuggestionItemClick, item.link, e)}
                  onMouseEnter={() => onSuggestionItemHover(filteredData.indexOf(item))}
                >
                  <div
                    className={classnames({
                      'd-flex justify-content-between align-items-center': item.file || item.img
                    })}
                  >
                    <div className='item-container d-flex'>
                      {item.icon ? (
                        <IconTag size={17} />
                      ) : item.file ? (
                        <img src={item.file} height='36' width='28' alt={item.title} />
                      ) : item.img ? (
                        <img className='rounded-circle mt-25' src={item.img} height='28' width='28' alt={item.title} />
                      ) : null}
                      <div className='item-info ms-1'>
                        <p className='align-middle mb-0'>{item.title}</p>
                        {item.by || item.email ? (
                          <small className='text-muted'>{item.by ? item.by : item.email ? item.email : null}</small>
                        ) : null}
                      </div>
                    </div>
                    {item.size || item.date ? (
                      <div className='meta-container'>
                        <small className='text-muted'>{item.size ? item.size : item.date ? item.date : null}</small>
                      </div>
                    ) : null}
                  </div>
                </li>
              )
            }}
          />
        ) : null}
        <div className='search-input-close'>
          <Icon.X
            className='ficon'
            onClick={e => {
              e.stopPropagation()
              setNavbarSearch(false)
              handleClearQueryInStore()
            }}
          />
        </div>
      </div>
    </NavItem>
  )
}

export default NavbarSearch
