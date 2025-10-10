/*eslint-disable */
import { useState, useRef, useMemo, useEffect } from 'react'
import { Tabs } from "antd"
import { useSelector, useDispatch } from 'react-redux'
import { useParams, useNavigate, useLocation, Route } from 'react-router-dom'
import { deleteBreadcrumbsActive, updateBreadcrumbsActive, updateActiveTab } from '@store/navbar'
import './tab.scss'
import Routes from '@src/navigation/vertical'
import useUpdateEffect from '@hooks/useUpdateEffect'
import { ShowToast } from '@CityAppExtComponents/caToaster'
import { clearSlice } from '@store/rootReducer'

const NewTabs = () => {
    const store = useSelector(state => state.navbar)
    const dispatch = useDispatch()
    const navigate = useNavigate()
    const location = useLocation()
    const prevTabPath = useRef('')

    const [activeKey, setActiveKey] = useState(store.activeTab.key)
    const [items, setItems] = useState(store.breadcrumbsActive)

    const onChange = (key) => {
        setActiveKey(key)
    }

    // const add = () => {
    //     const newActiveKey = `newTab${newTabIndex.current++}`
    //     setItems([
    //         ...items,
    //         { label: "New Tab", children: "New Tab Pane", key: newActiveKey }
    //     ])
    //     setActiveKey(newActiveKey)
    // }

    const remove = (targetKey) => {
        if (targetKey === "/" || targetKey === "/Dashboard") return
        const targetIndex = items.findIndex((pane) => pane.key === targetKey)
        const newPanes = items.filter((pane) => pane.key !== targetKey)
        if (newPanes.length && targetKey === activeKey) {
            const { key } =
                newPanes[
                targetIndex === newPanes.length ? targetIndex - 1 : targetIndex
                ]
            setActiveKey(key)
        }
        setItems(newPanes)

        dispatch(deleteBreadcrumbsActive(targetKey))
        if (store.breadcrumbsActive.length - 1 > 1) {
            if (store.breadcrumbsActive[store.breadcrumbsActive.length - 1].key === targetKey) {
                navigate(`..${store.breadcrumbsActive[store.breadcrumbsActive.length - 2]?.key}`, { replace: true })
            } else {
                navigate(`..${store.breadcrumbsActive[store.breadcrumbsActive.length - 1]?.key}`, { replace: true })
            }
        } else {
            navigate(`/`, { replace: true })
        }
        const splitArr = targetKey.split("/")
        const sliceName = `${splitArr[1]}_${splitArr[2]}`
        dispatch(clearSlice(sliceName))
    }

    const onEdit = (targetKey, action) => {
        if (action === "add") {
            // add()
        } else {
            remove(targetKey)
        }
    }
    const onTabClick = (key) => {
        navigate(`..${key}`, { replace: true })
    }

    useUpdateEffect(() => {
        if (store.breadcrumbsActive.length > 0) {
            setItems(store.breadcrumbsActive)
        }

        return () => {
        }
    }, [store.breadcrumbsActive])

    useUpdateEffect(() => {
        if (store.breadcrumbsActive.length > 0) {
            setActiveKey(store.breadcrumbsActive[store.breadcrumbsActive.length - 1]?.key)
        }

        return () => {
        }
    }, [store.breadcrumbsActive.length])

    useEffect(() => {
        let flatArray = Routes.flat().map(x => x.children).flat()
        flatArray = flatArray.flatMap(x => {
            if (x.children) {

                return x.children
            }
            return x
        })
        const linkTag = flatArray.find(x => {
            const locationSplitArr = location.pathname.split("/")
            const navLinkSplitArr = x.navLink?.split("/")
            let result = false
            if(!(locationSplitArr && navLinkSplitArr)) return
            if (locationSplitArr[1] === navLinkSplitArr[1] && locationSplitArr[2] === navLinkSplitArr[2]) {
                result = true
            }
            return result
        })
        const duplicateIndex = store.breadcrumbsActive.findIndex(x => {
            const navLinkSplitArr = x.key.split("/")
            const locationSplitArr = location?.pathname.split("/")
            if (navLinkSplitArr[1] === locationSplitArr[1] && navLinkSplitArr[2] === locationSplitArr[2]) {
                return true
            } else return false
        })

        const duplicate = duplicateIndex !== -1 ? true : false

        if (!duplicate) {
            if(store.breadcrumbsActive.length >= 15) {
                navigate(`..${store.breadcrumbsActive[store.breadcrumbsActive.length - 1]?.key}`, { replace: true })
                ShowToast("error", "最多只能開啟15個分頁")
                return
            }   else {
                dispatch(updateBreadcrumbsActive({ title: linkTag.title, navLink: location?.pathname, duplicate }))
            }
        } else {
            dispatch(updateBreadcrumbsActive({ title: linkTag.title, navLink: location?.pathname, duplicate, targetIndex: duplicateIndex }))
            setActiveKey(location?.pathname)
        }
        dispatch(updateActiveTab(location?.pathname))
        prevTabPath.current = location?.pathname

        return () => {
        }
    }, [location.pathname])


    //重整頁面
    useEffect(() => {
        const handleBeforeUnload = (event) => {
            const splitArr = activeKey.split("/")
            const sliceName = `${splitArr[1]}_${splitArr[2]}`
            dispatch(clearSlice(sliceName))
        }

        window.addEventListener("beforeunload", handleBeforeUnload)

        return () => {
            window.removeEventListener("beforeunload", handleBeforeUnload)
        }
    }, [dispatch])


    return (
        <div style={{ maxWidth: '900px' }}>
            <Tabs
                className='mb-0'
                hideAdd
                onChange={onChange}
                activeKey={activeKey}
                type="editable-card"
                onEdit={onEdit}
                items={items}
                onTabClick={onTabClick}
            />
        </div>
    )
}

export default NewTabs
