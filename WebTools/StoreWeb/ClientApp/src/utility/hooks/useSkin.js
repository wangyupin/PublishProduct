// ** React Imports
import { useEffect } from 'react'

// ** Store Imports
import { handleSkin } from '@store/layout'
import { useDispatch, useSelector } from 'react-redux'

export const useSkin = () => {
  // ** Hooks
  const dispatch = useDispatch()
  const store = useSelector(state => state.layout)

  const setSkin = type => {
    dispatch(handleSkin(type))
  }

  useEffect(() => {
    // ** Get Body Tag
    const element = window.document.body

    // ** Define classnames for skins
    const classNames = {
      dark: 'dark-layout',
      bordered: 'bordered-layout',
      'semi-dark': 'semi-dark-layout'
    }

    // ** Remove all classes from Body on mount
    element.classList.remove(...element.classList)

    // Vuexy Version
    // ** If skin is not light add skin class
    // if (store.skin !== 'light') {
    //   element.classList.add(classNames[store.skin])
    // }

    //Add by Moon 2022/1/5
    // let agThemeOld = "ag-theme-alpine"
    // let agThemeNew = "ag-theme-alpine-dark"
    // if (store.skin !== 'light') {
    //   element.classList.add(classNames[store.skin])
    // } else {
    //   agThemeOld = "ag-theme-alpine-dark"
    //   agThemeNew = "ag-theme-alpine"
    // }
    // //Add-End

    // //Set ag-theme
    // const agGridElements = Array.from(document.getElementsByClassName(agThemeOld))
    // agGridElements.map(e => {
    //   e.classList.remove(...e.classList)
    //   e.classList.add(agThemeNew)
    // })

    //Add by Moon 2022/1/20
    if (store.skin !== 'light') {
      element.classList.add(classNames[store.skin])
      // element.classList.add("ag-theme-alpine-dark")
    } else {
      // element.classList.add("ag-theme-alpine")
    }
  }, [store.skin])

  return { skin: store.skin, setSkin }
}
