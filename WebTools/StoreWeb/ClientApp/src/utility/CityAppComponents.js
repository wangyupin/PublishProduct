// ** React
import ReactDOMServer from "react-dom/server"
import React, { Fragment, useEffect, useRef, memo, forwardRef } from 'react'
import ReactSelect, { components, StylesConfig, createFilter } from "react-select"
import { useSelector, useDispatch } from 'react-redux'

// ** Third Party Components
import { Row, Col, Card, Button, ButtonGroup, Nav, NavItem, NavLink, Label } from 'reactstrap'
import classnames from 'classnames'
import { Settings, ZoomIn, Edit, Trash, Check, X } from 'react-feather'
import { CustomLoadingOverlayProps } from 'ag-grid-react'
import useState from 'react-usestateref'
import { FixedSizeList } from "react-window"
import { useDebounce } from 'use-debounce'
import { useTranslation } from 'react-i18next'
import { TreeSelect, ConfigProvider } from "antd"

//** Vuexy components
import { useSkin } from '@hooks/useSkin'
import UILoader from '@components/ui-loader'
import Spinner from '@components/spinner/Loading-spinner'
import { selectThemeColors, isObjEmpty, formatSelectOptionLabelSTD, selectStyle } from '@utils'
import useUpdateEffect from '@hooks/useUpdateEffect'

// ** Styles Imports
import '@styles/react/libs/react-select/_react-select.scss'

// ** Contexify
import { Menu, Item, useContextMenu, Separator } from 'react-contexify'
import 'react-contexify/dist/ReactContexify.min.css'
import '@styles/react/libs/context-menu/context-menu.scss'

// ** CityApp Utilty
export const FloatingSelect = memo(({ SelectOptions, labelText, ...props }) => {
    return (
        <Fragment>
            <div className="form-floating">
                <select className="form-select" {...props}>
                    {SelectOptions.map((item, i) => <option key={i} value={item.value}>{item.label}</option>)}
                </select>
                <label htmlFor={props.id}>{labelText}</label>
            </div>
        </Fragment>
    )
})

export const FooterMenu = memo(({
    quyeryBtnLable, queryBtnDisable, queryBtnClick, queryBtnHide,
    conditionBtnClick, conditionBtnHide,
    tabLists, activeTab, toggleTab }) => {
    // ** Hooks
    const { skin } = useSkin()
    return (
        <div
            align="center"
            style={skin === 'light' ? { backgroundColor: '#f8f8f8' } : { backgroundColor: '#000000' }}
        >
            <Row>

                {/* <Col xs={2} n="center">
          <Button.Ripple className='btn-icon' outline color='primary'>
          <Search size={20} />
          
          </Button.Ripple>
          </Col>
          <Col xs={2} align="center">
          <Button.Ripple className='btn-icon' color='warning'>
          <Inbox size={20} />
          </Button.Ripple>
        </Col> */}
                <Col xs={4} className='p-0'>
                    {conditionBtnHide ? null : (
                        <Button
                            size='sm'
                            color='primary'
                            onClick={conditionBtnClick}
                        >
                            {/* 查詢條件 */}
                            <Settings size={23} />
                        </Button>
                    )}
                    {queryBtnHide ? null : (
                        <Button

                            size='sm' color='secondary'
                            disabled={queryBtnDisable} onClick={queryBtnClick} >
                            {/* {quyeryBtnLable} */}
                            <ZoomIn size={23} />
                        </Button>
                    )}
                </Col>

                <Col xs={8} className='p-0'>
                    <NavTabList tabLists={tabLists} activeTab={activeTab} toggleTab={toggleTab} />
                </Col>
            </Row>
        </div >
    )
})

export const ContextMenuList = ({ menuList }) => {
    return (
        <Menu id='ContextMenu'>
            {/* <Separator /> */}
            {menuList.map((item, index) => (
                <Item key={index} onClick={item.onClick}>
                    {item.label}
                </Item>
            ))}
        </Menu>
    )
}

export const AgGridContextMenu = ({
    contexClickData, setCcontexClickData,
    columnDefList, setColumnDefList,
    onWanDallorShow }) => {
    const menuList = [
        {
            label: '關注欄位',
            onClick: (e) => {
                // console.log('關注欄位 Click', e)
                if (contexClickData !== null) {
                    // console.log('Had contexClickData', contexClickData)
                    const newColDef = {
                        ...contexClickData.colDef,
                        // cellStyle: { fontWeight: 'bold', fontSize: '136%', color: 'red' }
                        cellStyle: { fontWeight: 'bold', fontSize: '22px', color: 'red' }
                    }
                    // console.log('newColDef', newColDef)

                    setColumnDefList(
                        columnDefList.map(m => {
                            return ((m.field === newColDef.field) && (m.headerName === newColDef.headerName)) ? newColDef : m
                        })
                    )
                    setCcontexClickData(null)
                }
            }
        },
        {
            label: '取消關注',
            onClick: (e) => {
                // console.log('取消關注 Click', e)
                if (contexClickData !== null) {
                    // console.log('Had contexClickData', contexClickData)
                    const newColDef = {
                        ...contexClickData.colDef,
                        // cellStyle: { fontWeight: 'normal', fontSize: '100%', color: 'currentColor' }
                        cellStyle: { fontWeight: 'normal', fontSize: '18px', color: 'currentColor' }
                    }
                    // console.log('newColDef', newColDef)

                    setColumnDefList(
                        columnDefList.map(m => {
                            return ((m.field === newColDef.field) && (m.headerName === newColDef.headerName)) ? newColDef : m
                        })
                    )
                    setCcontexClickData(null)
                }
            }
        }
    ]

    if (onWanDallorShow !== undefined) {
        menuList.push({
            label: '萬元顯示',
            onClick: e => onWanDallorShow(true)
        })
        menuList.push({
            label: '取消萬元顯示',
            onClick: e => onWanDallorShow(false)
        })
    }

    return (
        <ContextMenuList menuList={menuList} />
    )
}

export const OptionComponents = ({ data, ...props }) => {
    return (
        <components.Option {...props}>
            <div className='d-flex flex-wrap align-items-center'>
                {/* <Avatar className='my-0 mr-1' size='sm' img={data.avatar} /> */}
                <div>{data.label}</div>
            </div>
        </components.Option>
    )
}

export const NavTabList = ({ navItemClassName, tabLists, activeTab, toggleTab }) => {
    return (
        <Nav tabs >
            {tabLists.map((item, index) => (
                <NavItem className={navItemClassName} key={index}>
                    <NavLink
                        active={activeTab === item.id}
                        onClick={() => {
                            toggleTab(item.id)
                        }}
                        disabled={item.disabled}
                    >
                        {item.label}
                    </NavLink>
                </NavItem>
            ))}
        </Nav>
    )
}

export const QueryLoader = ({ toggleTab }) => {
    useEffect(() => {
        if (toggleTab !== undefined) toggleTab()
    }, [])

    return (
        <Fragment>
            <div className='cityapp div-uiloader' >
                <Spinner type='grow' color='primary' />
                <h2 className='text-warning' align="center" >執行中，請稍後...</h2>
            </div>
            {/* {toggleTab()} */}
        </Fragment>
    )
}

export const CaQueryLoader = React.memo(({ blocking, toggleTab, content, className }) => {
    return (
        <UILoader className={className} blocking={blocking} loader={<QueryLoader toggleTab={toggleTab} />}>
            <Fragment>{content}</Fragment>
        </UILoader>
    )
})

export const LoginLoader = ({ props }) => {
    return (
        <Fragment>
            <div className='cityapp div-uiloader' >
                <Spinner type='grow' color='primary' />
                <h2 className='text-warning' align="center" >登入中，請稍後...</h2>
            </div>
        </Fragment>
    )
}

// ** MultiSelect base on ReactSelect
const MultiSelectComponentOption = ({ data, ...props }) => {
    return (
        <components.Option {...props}>
            <div className='d-flex flex-wrap align-items-center'>
                <div>{data.label}</div>
            </div>
        </components.Option>
    )
}

const MultiValueLabel = (props) => (
    <components.MultiValueLabel {...props}>
        {props.data.label}
    </components.MultiValueLabel>
)

export const MultiSelect = React.forwardRef((props, ref) => {
    // isOptionSelected sees previous props.value after onChange
    const valueRef = useRef(props.value)
    valueRef.current = props.value

    const selectAllOption = {
        value: "selectAll",
        label: "全選"
    }

    const isSelectAllSelected = () => valueRef.current?.length === props.options.length

    const isOptionSelected = option => (
        valueRef.current?.some(({ value }) => value === option.value) || isSelectAllSelected()
    )

    const getOptions = () => [selectAllOption, ...props.options]

    const getValue = () => {
        return isSelectAllSelected() ? [selectAllOption] : props.value
    }

    const onChange = (newValue, actionMeta) => {
        const { action, option, removedValue } = actionMeta

        if (action === "select-option" && option.value === selectAllOption.value) {
            props.onChange([...props.options], actionMeta)
        } else if (
            (action === "deselect-option" &&
                option.value === selectAllOption.value) ||
            (action === "remove-value" &&
                removedValue.value === selectAllOption.value)
        ) {
            props.onChange([], actionMeta)
        } else if (
            actionMeta.action === "deselect-option" &&
            isSelectAllSelected()
        ) {
            props.onChange(
                props.options.filter(({ value }) => value !== option.value),
                actionMeta
            )
        } else {
            props.onChange(newValue || [], actionMeta)
        }
    }

    return (
        <ReactSelect
            formatOptionLabel={formatSelectOptionLabelSTD}
            theme={selectThemeColors}
            {...props}
            isOptionSelected={isOptionSelected}
            options={getOptions()}
            value={getValue()}
            onChange={onChange}
            hideSelectedOptions={false}
            isMulti={props.isMulti === null ? true : props.isMulti}
            styles={props.styles ? props.styles : selectStyle}
            // components={{
            //     Option: MultiSelectComponentOption
            // }}
            components={{ MultiValueLabel }}
            className={`react-select ${props.className ? props.className : ''}`}
            closeMenuOnSelect={false}
            inputId={props.id ? props.id : ''}
            autoFocus={props.autoFocus}
            menuPlacement='auto'
        />
    )
})

export const AgGridActionCellRenderer = (params) => {
    // const eAction = document.createElement("div")
    // eAction.innerHTML = ReactDOMServer.renderToString(
    //     <Fragment>
    //         <Edit color='green' size={22} className='cursor-pointer me-1' data-action='edit' />
    //         <Trash color='red' size={22} className='cursor-pointer' data-action="delete" />
    //     </Fragment>
    // )
    // return eAction
    const { edit = true, del = true } = params
    return (
        <div>
            <Fragment>
                {edit && <Edit color='green' size={18} className='cursor-pointer me-1' data-action='edit' />}
                {del && <Trash color='red' size={18} className='cursor-pointer' data-action="delete" />}
            </Fragment>
        </div>
    )
}

export const CustomLabel = ({ htmlFor, left, right }) => {
    return (
        <Label className='form-check-label' htmlFor={htmlFor}>
            {left || (
                <span className='switch-icon-left'>
                    <Check size={14} />
                </span>
            )}
            {right || (
                <span className='switch-icon-right'>
                    <X size={14} />
                </span>
            )}
        </Label>
    )
}

export const AgGridDisableOverlay = (props) => {
    return (
        <div role="presentation" className="ag-overlay-loading-center bg-opacity-10 bg-primary" style={{ height: '9%' }}>
            <span> {props.message}</span>
        </div>
    )
}

//使用reactWindow超多筆資料才不會lag
const ReactWindow = ({ options, children }) => {

    if (options === null) {
        return <>Still loading...</>
    }
    return (
        <FixedSizeList
            height={200}
            itemCount={options.length}
            itemSize={38}
            style={{ fontSize: '13px' }}
            itemData={options}
            overscanCount={10}
        >
            {({ index, style }) => <div style={style}>{children[index]}</div>}
        </FixedSizeList>
    )
}

export const CustomSelection = forwardRef(({ initData, hotKeyId, placeholder = '選擇...', setSelected, selected, handleInput, isMulti = true, singleOptionLabel = false, preferValue = false, singleMenuLabel = false, field, isDisabled = false, isClearable = true, size = 36, onChange = () => { } }, ref) => {
    const dispatch = useDispatch()
    //ref condtion
    const refProp = {
        ...(ref ? { ref } : {})
    }
    //使用這個component的時候，使用useform hook有需要在不是OnSubmit發生時讀取select的值
    const seletedProps = {
        ...(selected ? { defaultValue: selected } : {}),
        ...(setSelected ? {
            onChange: (e) => {
                field?.onChange(e)
                setSelected(e)
                onChange(e)
            }
        } : {})
    }

    //input輸入搜尋時需不需要call api更新options資料
    const [searchTerm, setSearchTerm, searchTermRef] = useState('')
    const [debounceSearch] = useDebounce(searchTerm, 600)
    useUpdateEffect(() => {
        if (handleInput !== undefined) {
            dispatch(handleInput({ Q: debounceSearch }))
        }
    }, [debounceSearch])

    const handleInputChange = (e) => {
        setSearchTerm(e)
    }

    //optionLabel自訂顯示
    const formatOptionLabel = (Options, { context }) => {

        if (context === "menu") {
            //下拉選項顯示Label + Value or Label
            if (singleMenuLabel) {
                return Options.label
            } else {
                return `${Options.value} ${Options.label}`
            }
        } else if (context === "value") {
            //有multi的字體顏色是白色
            if (isMulti) {
                if (singleMenuLabel) { //已選顯示
                    return (<span className='text-white'>{Options.value}</span>)

                } else {
                    return (<span className='text-white'>{Options.label}</span>)
                }
            } else {
                if (singleOptionLabel) { //已選顯示
                    if (preferValue) {
                        return Options.value
                    } else {
                        return Options.label
                    }
                } else {
                    return `${Options.value} | ${Options.label}`
                }
            }
        }
    }

    return (
        <div className='d-flex'>
            <ReactSelect className='flex-fill'
                {...field} //useForm Controller's field
                {...seletedProps}
                {...refProp}
                isDisabled={isDisabled}
                aria-labelledby="aria-label"
                autosize={false}
                hideSelectedOptions={false}
                inputId={hotKeyId}
                theme={selectThemeColors}
                isMulti={isMulti}
                options={initData}
                formatOptionLabel={formatOptionLabel}
                onInputChange={handleInputChange}
                isLoading={false}
                styles={{
                    control: (baseStyles, { isDisabled, isFocused }) => ({
                        ...baseStyles,
                        minHeight: `${size}px`,
                        height: `${size}px`,
                        '&:focus': {
                            borderColor: isFocused ? '#7367f0' : '#d8d6de'
                        },
                        borderColor: isFocused && !isDisabled ? '#7367f0' : '#d8d6de',
                        '::-webkit-align-items': 'unset',
                        alignItems: 'unset',
                        boxShadow: 'none'
                    }),
                    valueContainer: (base) => ({
                        ...base,
                        width: '100px',
                        height: `${size}px`,
                        padding: '0px 8px',
                        overflowX: isMulti ? 'scroll' : 'unset',
                        flexWrap: 'unset',
                        '::-webkit-scrollbar': {
                            height: '5px'
                        },
                        '::-webkit-scrollbar-thumb': {
                            background: 'rgba(0, 0, 0, 0.1)'
                        }
                    }),
                    multiValue: (base) => ({
                        ...base,
                        flex: '1 0 auto',
                        justifyContent: 'space-between',
                        color: 'white'
                    }),
                    indicatorSeparator: (base, { isDisabled }) => ({
                        ...base,
                        background: isDisabled ? 'hsl(0, 0%, 95%)' : '#d8d6de'
                    }),
                    menu: (base) => ({
                        ...base,
                        zIndex: '999'
                    })
                }}
                placeholder={placeholder}
                menuShouldScrollIntoView={true}
                closeMenuOnSelect={true}
                isMenuOpen={true}
                isClearable={isClearable}
                filterOption={createFilter({ ignoreAccents: false })}
                // components={{ MenuList: ReactWindow }}
                menuPlacement="auto"
            />
        </div>

    )
})

export const CustomArrow = () => {
    return (
        <svg height="20" width="20" viewBox="0 0 20 20" aria-hidden="true" focusable="false" className="css-tj5bde-Svg"><path d="M4.516 7.548c0.436-0.446 1.043-0.481 1.576 0l3.908 3.747 3.908-3.747c0.533-0.481 1.141-0.446 1.574 0 0.436 0.445 0.408 1.197 0 1.615-0.406 0.418-4.695 4.502-4.695 4.502-0.217 0.223-0.502 0.335-0.787 0.335s-0.57-0.112-0.789-0.335c0 0-4.287-4.084-4.695-4.502s-0.436-1.17 0-1.615z"></path></svg>
    )
}

const SingleValue = (props) => {
    return <components.SingleValue {...props}>{props.data.label}</components.SingleValue>
}

export const Select = React.forwardRef((props, ref) => {
    const { t } = useTranslation(['common'])
    return (
        <ReactSelect
            ref={ref}
            classNamePrefix='select'
            theme={selectThemeColors}
            styles={selectStyle}
            formatOptionLabel={formatSelectOptionLabelSTD}
            components={{ SingleValue }}
            placeholder={t('selectPlaceholder', { ns: 'common' })}
            menuPlacement='auto'
            {...props}
        />
    )
})

export const AntdTreeSelect = React.forwardRef((props, ref) => {
    return (
        <ConfigProvider
            theme={{
                token: {
                    colorPrimary: '#7367f0'
                },
                components: {
                    Tree: {
                        nodeHoverBg: '#7367f01a'

                    },
                    TreeSelect: {
                        nodeHoverBg: '#7367f01a'
                    }
                }
            }}
        >
            <TreeSelect
                ref={ref}
                autoComplete="off"
                className="custom-tree-select"
                multiple
                treeCheckable
                treeNodeFilterProp="title"
                {...props}
            />
        </ConfigProvider>
    )
})
