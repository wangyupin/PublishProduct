// ** User List Component
// import Table from './Table'
import { Fragment, useMemo, useContext, useState, useEffect } from "react"
import { Row, Col, Button, Card, CardBody, CardHeader, CardText, CardTitle, Input, Badge, Modal, ModalHeader, ModalBody, ModalFooter, Form, Label, InputGroup, InputGroupText, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap'
import { useDispatch, useSelector } from 'react-redux'
import { Link } from 'react-router-dom'

import { editStore, copyStore, getEcommerceStoreData, delEcommerceStore, addEcommerceStore } from '../store'

import axios from 'axios'

// ** Styles
import '@styles/react/apps/app-users.scss'

import createPopper from '@popperjs/core'

const EcommerceStoreList = ({ access, t }) => {

    const store = useSelector(state => state.EcommerceMgmt_EcommerceStoreEdit)
    const dispatch = useDispatch()

    const [storeDataAll, setStoreDataAll] = useState(null)
    const [storeImage, setStoreImage] = useState(null)

    const [storeDataNum, setStoreDataNum] = useState(null)

    const [menuVisible, setMenuVisible] = useState(false)

    const menuStyles = {
        transition: 'opacity 0.25s ease, transform 0.25s ease',
        opacity: menuVisible ? 1 : 0
        // transform: menuVisible ? 'translateY(0) !important' : 'translateY(-20) !important'
    }

    const handleMouseEnterLogo = (id) => {
        const changeStyle = storeImage.map(store => {
            if (id === store.eStoreID) {
                return {
                    ...store,
                    isHovered: true
                }
            }
            return {
                ...store,
                isHovered: false
            }
        })
        setStoreImage(changeStyle)
    }

    const handleMouseLeaveLogo = (id) => {
        const changeStyle = storeImage.map(store => {
            if (id === store.eStoreID) {
                return {
                    ...store,
                    isHovered: false
                }
            }
            return store
        })
        setStoreImage(changeStyle)
    }


    const getStoreData = async () => {
        await axios.get(`/api/EcommerceStore/GetEcommerceStoreData`)
            .then(res => {
                // setStoreImage(res.data.data.storeImage)
                setStoreDataAll(res.data.data.storeData)
            })
    }

    // 顯示商店數量
    const storeNum = (tag) => {
        const keys = Object.keys(storeDataNum)
        if (keys.includes(tag)) {
            return storeDataNum[tag].length
        } else {
            return 0
        }
    }

    const delData = (store, i) => {
        const tag = store.eStoreName
        storeDataNum[tag].map((data, index) => {
            if (tag === data.storeTag && i === index) {
                dispatch(delEcommerceStore({
                    storeNumber: data.storeNumber,
                    storeName: data.storeName,
                    storeTag: tag
                }))
                    .then(() => {
                        getStoreData()
                    })
            }
        })
    }

    const storeTagStyle = (store, i) => {
        const tag = store.eStoreName
        if (storeDataNum !== null) {
            return (
                <div className='d-flex flex-row'>
                    {storeDataNum[tag]?.map((data, index) => {
                        if (data.storeTag === tag) {
                            return (
                                <UncontrolledDropdown key={index}
                                    onToggle={() => setMenuVisible(!menuVisible)}
                                >
                                    <DropdownToggle className='icon-btn hide-arrow w-100 h-100 p-0' color='transparent' data-bs-display="static">
                                        <div
                                            className='d-flex justify-content-center align-items-center'
                                            style={{
                                                width: 'auto',
                                                height: '3.5rem',
                                                cursor: 'pointer',
                                                marginRight: '5px'
                                            }}
                                        >
                                            <div
                                                className='border rounded p-1'
                                                style={{
                                                    width: '100%',
                                                    height: '100%',
                                                    display: 'flex',
                                                    justifyContent: 'center',
                                                    alignItems: 'center',
                                                    backgroundColor: `${store.eStoreStyle}`,
                                                    color: 'white',
                                                    fontSize: '1.25rem'
                                                }}
                                            >{`${data.storeTag} ${index + 1}店`}</div>
                                        </div>
                                    </DropdownToggle>
                                    <DropdownMenu
                                        className='m-5 p-0 shadow-lg'
                                        style={menuStyles}
                                    >
                                        <Link
                                            to={`../EcommerceMgmt/EcommerceStoreEdit/Single/${data.storeNumber}`}
                                            className='text-body'
                                            onClick={() => {
                                                dispatch(editStore({ storeTag: data.storeTag, selectedStore: data }))
                                                dispatch(copyStore(false))
                                            }}
                                        >
                                            <DropdownItem
                                                style={{
                                                    padding: '0.65rem 1.28rem',
                                                    width: '100%'
                                                }}
                                            >
                                                <span>編輯</span>
                                            </DropdownItem>
                                        </Link>

                                        <Link
                                            to={`../EcommerceMgmt/EcommerceStoreEdit/Single/${data.storeNumber}`}
                                            className='text-body'
                                            onClick={() => {
                                                dispatch(editStore({ storeTag: data.storeTag, eStoreID: data.eStoreID, selectedStore: data }))
                                                dispatch(copyStore(true))
                                            }}
                                        >
                                            <DropdownItem
                                                style={{
                                                    padding: '0.65rem 1.28rem',
                                                    width: '100%'
                                                }}
                                            >
                                                <span>複製</span>
                                            </DropdownItem>
                                        </Link>

                                        <DropdownItem
                                            style={{
                                                padding: '0.65rem 1.28rem',
                                                width: '100%'
                                            }}
                                            onClick={() => delData(store, index)}
                                        >
                                            <span>刪除</span>
                                        </DropdownItem>

                                    </DropdownMenu>
                                </UncontrolledDropdown>
                            )
                        }
                    })}
                </div>

            )
        }
    }

    useEffect(() => {
        const fetchData = async () => {
            await axios.get('/api/EcommerceStore/GetEStoreImage')
                .then((res) => {
                    setStoreImage(res.data.data.result)
                })
        }

        fetchData()
    }, [])

    useEffect(() => {
        getStoreData()
    }, [])

    useEffect(() => {
        if (storeDataAll !== null && storeImage !== null) {
            const storeNum = {}
            storeDataAll.map((data, index) => {
                storeImage.map((store, index) => {
                    const tag = store.eStoreName
                    if (tag === data.storeTag) {
                        if (!storeNum[tag]) {
                            storeNum[tag] = []
                        }
                        storeNum[tag].push(data)
                    }
                })
            })
            setStoreDataNum(storeNum)
        }
    }, [storeDataAll, storeImage])

    return (
        <div className='d-flex flex-column w-100'>
            {storeImage !== null && storeImage.map((store, index) => {
                return (
                    <Card key={store.eStoreID} className='border rounded'>
                        <Row style={{ padding: '1rem ', width: '100%' }}>
                            <Col
                                sm='2'
                                className='d-flex justify-content-center align-items-center border-end'
                                onMouseEnter={() => handleMouseEnterLogo(store.eStoreID)}
                                onMouseLeave={() => handleMouseLeaveLogo(store.eStoreID)}
                            >
                                <div className='d-flex justify-content-center align-items-center' style={{ width: '120px', height: '60px' }}>
                                    <UncontrolledDropdown
                                        onToggle={() => setMenuVisible(!menuVisible)}
                                    >
                                        <DropdownToggle className='icon-btn hide-arrow w-100 h-100 p-0' color='transparent'>
                                            <img
                                                src={store.eStoreSrc}
                                                style={{
                                                    width: '100%',
                                                    height: '100%',
                                                    objectFit: 'contain',
                                                    cursor: 'pointer',
                                                    transform: store.isHovered ? 'scale(1.3)' : 'none',
                                                    transition: store.isHovered ? 'all 0.5s ease 0s' : 'none'
                                                }}
                                            />
                                        </DropdownToggle>
                                        <DropdownMenu
                                            className='m-5 p-0'
                                            style={menuStyles}
                                        >
                                            <Link
                                                to={`../EcommerceMgmt/EcommerceStoreEdit/Setting/${store.eStoreID}`}
                                                className='text-body'
                                                onClick={() => dispatch(editStore({ storeTag: store.eStoreName, eStoreID: store.eStoreID, selectedStore: null }))}
                                            >
                                                <DropdownItem className="w-100">
                                                    <span>共用參數設定</span>
                                                </DropdownItem>
                                            </Link>
                                            <Link
                                                to={`../EcommerceMgmt/EcommerceStoreEdit/Single/add`}
                                                className='text-body'
                                                onClick={() => dispatch(editStore({ storeTag: store.eStoreName, eStoreID: store.eStoreID, selectedStore: null }))}
                                            >
                                                <DropdownItem className="w-100">
                                                    <span>新增店面</span>
                                                </DropdownItem>

                                            </Link>
                                        </DropdownMenu>
                                    </UncontrolledDropdown>
                                </div>
                            </Col>
                            <Col sm='9' className='d-flex flex-column'>
                                <Col style={{ marginBottom: '10px' }}>{storeTagStyle(store, index)}</Col>
                                <Col>{`${t('ecommerceStore.storeNum', { ns: 'ecommerceMgmt' })}:${storeDataNum !== null ? storeNum(store.eStoreName) : '0'}`}</Col>
                            </Col>
                        </Row>
                    </Card>
                )
            })}
        </div>
    )
}

export default EcommerceStoreList
