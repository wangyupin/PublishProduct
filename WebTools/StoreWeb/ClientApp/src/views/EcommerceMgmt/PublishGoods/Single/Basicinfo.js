// ** React Import
import { useState, useEffect, useMemo, Fragment, useCallback, Children } from 'react'
import { Link, json, useNavigate } from 'react-router-dom'
import axios from 'axios'

// ** Utils
import { selectThemeColors, selectStyle, formatSelectOptionLabelNL, selectStyleNHL, getUserData } from '@utils'
import { CustomLabel, MultiSelect, Select, CustomArrow, Radio } from '@CityAppComponents'
import { getDateTimeLocal, getFormatedDateForInput } from '@CityAppHelper'

// ** Third Party Components
import CreatableSelect from 'react-select/creatable'
import classnames from 'classnames'
import { Controller, get, useFieldArray, useWatch } from 'react-hook-form'
import { X, Trash, Plus } from 'react-feather'
import { AgGridReact } from 'ag-grid-react'
import ReactSelect from "react-select"
import { useDropzone } from 'react-dropzone'
import { Cascader } from 'antd'
import ReactQuill from 'react-quill'
import 'react-quill/dist/quill.snow.css'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, Card, CardBody, CardHeader, CardTitle, Row, Col, InputGroup, InputGroupText, Table, Nav, NavItem, NavLink, TabPane, TabContent } from 'reactstrap'

// ** Store & Actions
import { getColumnDefs, getSizeIndexColumnDefs, getTryIndexColumnDefs, createIndexRow } from '../columns'

const createOption = (label) => ({
    label,
    value: label
})

const render91App = ({ arrVal, control, setValue, getValues, errors, t, watch, option }) => {

    const temperatureTypeDef = watch('temperatureTypeDef')
    const pointCheck = watch('salesModeTypeDef')?.[1]

    const { fields: pointsPayFields, append: pointsPayAppend, remove: pointsPayRemove, replace: pointsPayPayReplace } = useFieldArray(
        {
            control,
            name: 'pointsPayPairs'
        }
    )

    const { fields: productTagFields, append: productTagAppend, remove: productTagRemove, replace: productTagReplace } = useFieldArray(
        {
            control,
            name: 'saleProductTagList'
        }
    )

    const renderCheckboxes = useCallback((list, name) => {
        return list.map((item, idx) => {
            return (
                <div className='form-check d-flex align-items-center' key={`${name}.${idx}.checked`} >
                    <Controller
                        name={`${name}.${idx}.checked`}
                        control={control}
                        render={({ field }) => (
                            <Input type='checkbox' id={`${name}.${idx}.checked`} className='form-check-input' checked={field.value} {...field} />
                        )}

                    />
                    <Label className='form-check-label ms-25 me-1 ' for={`${name}.${idx}.checked`}>
                        {item.name}
                    </Label>
                </div>
            )
        })
    }, [])

    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        if (value === 'true' || value === 'false') {
            setValue(name, value === 'true')
        } else {
            setValue(name, value)
        }
    }, [])

    useEffect(() => {
        productTagReplace(arrVal.saleProductTagList)
        pointsPayPayReplace(arrVal.pointsPayPairs)
    }, [arrVal])

    return (
        <Fragment>
            <Row className='mb-1'>
                <Label className='form-label d-block' for='shopId'>
                    {t('publish.shopCategoryId', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                </Label>
                <Col sm='3' className='d-flex'>
                    <Controller
                        name={`shopCategoryId`}
                        control={control}
                        render={({ field }) => (
                            <Cascader {...field} options={option.shopCategoryOption} placeholder={t('selectPlaceholder', { ns: 'common' })} className="custom-cascader flex-grow-1 me-50" suffixIcon={<CustomArrow />} />
                        )}
                    />
                </Col>
            </Row>
            <Row className='mb-1'>
                <Col sm='3' className='mb-1'>
                    <Label className='form-label'>
                        {t('publish.salesModeTypeDef', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                    </Label>
                    <div className='d-flex' style={{ lineHeight: '36px' }}>
                        {renderCheckboxes(option.salesModeTypeOption, 'salesModeTypeDef')}
                    </div>
                </Col>
                <Col sm='12'>
                    <Table bordered responsive>
                        <tbody>
                            {pointCheck?.checked && (
                                <tr>
                                    <td>
                                        <Label className='form-label m-0'>
                                            {t('publish.pointsPayPairs', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                        </Label>
                                    </td>
                                    <td>
                                        {pointsPayFields.map((item, index) => {
                                            return (
                                                <div className='mb-25' key={item.id}>
                                                    <Controller
                                                        name={`pointsPayPairs.${index}.pairsPoints`}
                                                        control={control}
                                                        render={({ field }) => (
                                                            <Input {...field} style={{ width: '7em' }} className='d-inline' />
                                                        )}
                                                    />
                                                    <span> 點 + NT$ </span>
                                                    <Controller
                                                        name={`pointsPayPairs.${index}.pairsPrice`}
                                                        control={control}
                                                        render={({ field }) => (
                                                            <Input {...field} style={{ width: '7em' }} className='d-inline' />
                                                        )}
                                                    />
                                                    <span> ，活動代號 : </span>
                                                    <Controller
                                                        name={`pointsPayPairs.${index}.outerPromotionCode`}
                                                        control={control}
                                                        render={({ field }) => (
                                                            <Input {...field} style={{ width: '13em' }} className='d-inline' />
                                                        )}
                                                    />
                                                    {index > 0 && (
                                                        <Link className='text-body'
                                                            href='/'
                                                            onClick={() => pointsPayRemove(index)}>
                                                            <Trash size={16} className='ms-50' />
                                                        </Link>
                                                    )}
                                                    {index === pointsPayFields?.length - 1 && pointsPayFields?.length < 5 && (
                                                        <Link className='text-body'
                                                            href='/'
                                                            onClick={e => {
                                                                e.preventDefault()
                                                                pointsPayAppend({ pairsPoints: 0, pairsPrice: 0, outerPromotionCode: '' })
                                                            }}>
                                                            <Plus size={16} className='ms-50' />
                                                        </Link>
                                                    )}
                                                </div>
                                            )
                                        })}

                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </Table>
                </Col>
            </Row>
            <Row className='mb-1'>
                <Col sm='3' className='mb-1'>
                    <Label className='form-label' for='salePageSpecChartId'>
                        {t('publish.salePageSpecChart', { ns: 'ecommerceMgmt' })}
                    </Label>
                    <Controller
                        name='salePageSpecChartId'
                        control={control}
                        render={({ field }) => (
                            <Select
                                options={option.specChartOption}
                                formatOptionLabel={formatSelectOptionLabelNL}
                                placeholder={'選擇規格表'}
                                {...field}
                            />
                        )}
                    />
                </Col>
                <Col sm='12' className='mb-1'>
                    <Label className='form-label' for='saleProductTag'>
                        {t('publish.saleProductTag', { ns: 'ecommerceMgmt' })}
                    </Label>
                    {productTagFields.map((item, index) => {
                        return (
                            <Row className='mb-25' key={item.id}>
                                <Col sm='3'>
                                    <Controller
                                        name={`productTagList.${index}.group`}
                                        control={control}
                                        render={({ field }) => (
                                            <Select
                                                options={[]}
                                                formatOptionLabel={formatSelectOptionLabelNL}
                                                placeholder={'選擇群組'}
                                                {...field}
                                            />
                                        )}
                                    />
                                </Col>
                                <Col sm='6'>
                                    <Controller
                                        name={`productTagList.${index}.key`}
                                        control={control}
                                        render={({ field }) => (
                                            <ReactSelect
                                                classNamePrefix='select'
                                                theme={selectThemeColors}
                                                styles={selectStyle}
                                                hideSelectedOptions={false}
                                                isMulti
                                                closeMenuOnSelect={false}
                                                formatOptionLabel={formatSelectOptionLabelNL}
                                                placeholder={'請選擇標籤'}
                                                {...field}
                                            />
                                        )}
                                    />
                                </Col>
                                <Col sm='1' className='d-flex align-items-center'>
                                    {index > 0 && (
                                        <Link className='text-body'
                                            href='/'
                                            onClick={() => productTagRemove(index)}>
                                            <Trash size={16} className='ms-50' />
                                        </Link>
                                    )}
                                    {index === productTagFields?.length - 1 && productTagFields?.length < 30 && (
                                        <Link className='text-body'
                                            href='/'
                                            onClick={e => {
                                                e.preventDefault()
                                                productTagAppend({ group: null, key: [] })
                                            }}>
                                            <Plus size={16} className='ms-50' />
                                        </Link>
                                    )}
                                </Col>
                            </Row>
                        )
                    })}
                </Col>

                <Col sm='3' className='mb-1'>
                    <Label className='form-label' for='isClosed'>
                        {t('publish.isClosed', { ns: 'ecommerceMgmt' })}
                    </Label>
                    <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                        <Controller
                            name='isClosed'
                            control={control}
                            render={({ field }) => (
                                <Input type='radio' id='isClosed_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                            )}
                        />
                        <Label className='ms-25 me-1 form-check-label' for='isClosed_0'>
                            開啟
                        </Label>

                        <Controller
                            name='isClosed'
                            control={control}
                            render={({ field }) => (
                                <Input type='radio' id='isClosed_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                            )}
                        />
                        <Label className='ms-25 me-1 form-check-label' for='isClosed_1'>
                            關閉
                        </Label>
                    </div>
                </Col>

            </Row>

            <Row className='mb-1'>
                <Col sm='12' className='mb-1'>
                    <Label className='form-label'>
                        {t('publish.payTypes', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                    </Label>

                    <div className='d-flex' style={{ lineHeight: '36px' }}>
                        {renderCheckboxes(option.paymentOption, 'payTypes')}
                    </div>
                </Col>
                <Col sm='12' className='mb-1'>
                    <Label className='form-label'>
                        {t('publish.shippingTypes', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                    </Label>

                    <div className='d-flex' style={{ lineHeight: '36px' }}>
                        {renderCheckboxes(option.shipType_91appOption, 'shipType_91app')}
                    </div>
                </Col>
            </Row>
        </Fragment>

    )
}

const renderOfficialPlatform = ({ arrVal, control, setValue, getValues, errors, t, watch, option }) => {

    const renderCheckboxes = useCallback((list, name) => {
        return list.map((item, idx) => {
            return (
                <div className='form-check d-flex align-items-center' key={`${name}.${idx}.checked`} >
                    <Controller
                        name={`${name}.${idx}.checked`}
                        control={control}
                        render={({ field }) => (
                            <Input type='checkbox' id={`${name}.${idx}.checked`} className='form-check-input' checked={field.value} {...field} />
                        )}

                    />
                    <Label className='form-check-label ms-25 me-1 ' for={`${name}.${idx}.checked`}>
                        {item.name}
                    </Label>
                </div>
            )
        })
    }, [])

    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        if (value === 'true' || value === 'false') {
            setValue(name, value === 'true')
        } else {
            setValue(name, value)
        }
    }, [])

    return (
        <Fragment>
            <Row className='mb-1'>
                <Label className='form-label d-block' for='categoryOfficial'>
                    商品類別
                </Label>
                <Col sm='3' className='d-flex'>
                    <Controller
                        name='categoryOfficial'
                        control={control}
                        render={({ field }) => (
                            <Cascader {...field}
                                options={option.categoryOfficialOption || []}
                                placeholder={t('selectPlaceholder', { ns: 'common' })}
                                className="custom-cascader flex-grow-1 me-50"
                                suffixIcon={<CustomArrow />}
                            />
                        )}
                    />
                </Col>
            </Row>

            <Row className='mb-1'>
                <Col sm='3' className='mb-1'>
                    <Label className='form-label' for='isClosed'>
                        {t('publish.isClosed', { ns: 'ecommerceMgmt' })}
                    </Label>
                    <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                        <Controller
                            name='isClosed'
                            control={control}
                            render={({ field }) => (
                                <Input type='radio' id='isClosed_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                            )}
                        />
                        <Label className='ms-25 me-1 form-check-label' for='isClosed_0'>
                            開啟
                        </Label>

                        <Controller
                            name='isClosed'
                            control={control}
                            render={({ field }) => (
                                <Input type='radio' id='isClosed_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                            )}
                        />
                        <Label className='ms-25 me-1 form-check-label' for='isClosed_1'>
                            關閉
                        </Label>
                    </div>
                </Col>

            </Row>

            <Row className='mb-1'>
                <Col sm='12' className='mb-1'>
                    <Label className='form-label'>
                        {t('publish.payTypes', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                    </Label>

                    <div className='d-flex' style={{ lineHeight: '36px' }}>
                        {renderCheckboxes(option.paymentOption, 'payTypes')}
                    </div>
                </Col>
                <Col sm='12' className='mb-1'>
                    <Label className='form-label'>
                        {t('publish.shippingTypes', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                    </Label>

                    <div className='d-flex' style={{ lineHeight: '36px' }}>
                        {renderCheckboxes(option.shipType_91appOption, 'shipType_91app')}
                    </div>
                </Col>
            </Row>
        </Fragment>

    )
}


const renderShop = ({ arrVal, control, setValue, getValues, errors, t, watch, option }) => {
    const [active, setActive] = useState('1')

    const toggle = tab => {
        if (active !== tab) {
            setActive(tab)
        }
    }
    return (
        <Card className='mb-2'>
            <CardBody className='pt-50'>
                <Nav tabs>
                    <NavItem>
                        <NavLink
                            active={active === '1'}
                            onClick={() => {
                                toggle('1')
                            }}
                        >
                            官網
                        </NavLink>
                    </NavItem>
                </Nav>
                <TabContent activeTab={active}>
                    <TabPane tabId='1'>
                        {renderOfficialPlatform({ arrVal, control, setValue, getValues, errors, t, watch, option })}
                    </TabPane>
                </TabContent>

            </CardBody>
        </Card>
    )
}

const renderInfo = ({ arrVal, control, setValue, getValues, watch, t, option }) => {
    const [imageVersion, setImageVersion] = useState(Date.now())
    const isEnableBookingPickupDate = watch('isEnableBookingPickupDate')
    const availablePickupFlag = watch('availablePickupFlag')
    const hasSku = watch('hasSku')

    const { fields: specificationsFields, append: specificationsAppend, remove: specificationsRemove, replace: specificationsReplace } = useFieldArray(
        {
            control,
            name: 'specifications'
        }
    )

    const { fields: indexListFields, append: indexListAppend, remove: indexListRemove, replace: indexListReplace } = useFieldArray({
        control,
        name: 'indexList'
    })


    const { fields: optionFields, append: optionAppend, remove: optionRemove, replace: optionReplace } = useFieldArray(
        {
            control,
            name: 'optionList'
        }
    )


    const { fields: skuFields, append: skuAppend, remove: skuRemove, move: skuMove, replace: skuReplace } = useFieldArray(
        {
            control,
            name: 'skuList'
        }
    )

    useEffect(() => {
        arrVal.specifications ? specificationsReplace(arrVal.specifications) : specificationsReplace({ key: '', value: '' })
        arrVal.indexList && indexListReplace(arrVal.indexList)
        arrVal.skuList && skuReplace(arrVal.skuList)
        arrVal.optionList && optionReplace(arrVal.optionList)
    }, [arrVal])

    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        if (value === 'true' || value === 'false') {
            setValue(name, value === 'true')
        } else {
            setValue(name, value)
        }
    }, [])

    const components = {
        DropdownIndicator: null
    }

    const columnDefs = useMemo(() => getColumnDefs({ t, control, getValues, setValue }), [t, control, getValues, setValue])

    const onRowDragEnd = ({ node, overIndex, api }) => {
        const newOrder = []
        api.forEachNode(node => {
            newOrder.push(node.data)
        })

        const oldIndex = skuFields.findIndex(field => field.id === node.data.id)
        const newIndex = newOrder.findIndex(item => item.id === node.data.id)

        skuMove(oldIndex, newIndex)

        api.refreshCells({
            columns: ['image', 'qty', 'safetyStockQty', 'onceQty', 'suggestPrice', 'price', 'cost'],
            force: true
        })
    }

    // 定義 Quill 的工具列設定
    const quillModules = {
        toolbar: [
            [{ font: [] }],  // 字型
            [{ size: ['small', false, 'large', 'huge'] }],  // 字體大小
            ['bold', 'italic'],  // 粗體、斜體
            [{ color: [] }],  // 文字顏色
            [{ align: [] }],  // 對齊
            [{ list: 'ordered' }, { list: 'bullet' }],  // 列表
            [{ indent: '-1' }, { indent: '+1' }],  // 縮排
            ['link', 'image'],  // 連結、圖片
            ['code-block'],  // 程式碼
            ['clean']  // 清除格式
        ]
    }

    const quillFormats = [
        'font', 'size',
        'bold', 'italic',
        'color',
        'align',
        'list', 'bullet',
        'indent',
        'link', 'image',
        'code-block'
    ]

    return (
        <Card className='mb-2'>
            <CardBody>
                <Row className='mb-1'>
                    <Col sm='8'>
                        <Label className='form-label' for='title'>
                            {t('publish.title', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='title'
                            control={control}
                            render={({ field }) => (
                                <Input {...field} />
                            )}

                        />
                    </Col>
                </Row>
                <Row className='mb-1'>
                    <Col sm='12'>
                        <Label className='form-label' for='mainImage'>
                            {t('publish.mainImage', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='mainImage'
                            control={control}
                            render={({ field }) => {
                                const { getRootProps, getInputProps } = useDropzone({
                                    multiple: true,
                                    accept: 'image/*',
                                    onDrop: (acceptedFiles) => {
                                        field.onChange([
                                            ...field.value,
                                            ...acceptedFiles.map(file => ({
                                                file,
                                                preview: URL.createObjectURL(file)
                                            }))
                                        ])
                                    }
                                })

                                const deleteMainImage = (index) => {
                                    field.onChange(field.value.filter((_, i) => i !== index))
                                }

                                const thumbs = field.value.map((file, index) => (
                                    <div className='thumb' key={`${file.name}-${index}`}>
                                        <div className='thumbInner'>
                                            <img
                                                src={file.path ? `${process.env.NODE_ENV === 'production' ? window.location.origin : 'https://localhost:44320'}/${file.path}?v=${imageVersion}` : file.preview}
                                                className='img'
                                            />
                                            <div style={{ position: 'absolute', top: '0px', right: '0px', padding: '5px' }}>
                                                <Button color='flat-secondary' className='btn-icon p-0 h-auto' onClick={() => deleteMainImage(index)}><X size={18} /></Button>
                                            </div>
                                        </div>
                                    </div>
                                ))
                                return (
                                    <section className='thumbsContainer'>
                                        <div {...getRootProps({ className: 'dropzone-small' })}>
                                            <input {...getInputProps()} />
                                            <div className='d-flex align-items-center justify-content-center flex-column'>
                                                <Plus size={40} />
                                            </div>
                                        </div>
                                        {thumbs}
                                    </section>
                                )
                            }}

                        />
                    </Col>
                </Row>
                <Row className='mb-1'>
                    <Col sm='12'>
                        <Label className='form-label' for='isEnableBookingPickupDate_0'>
                            {t('publish.isEnableBookingPickupDate', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                            <Controller
                                name='isEnableBookingPickupDate'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='isEnableBookingPickupDate_0'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='isEnableBookingPickupDate_0'>
                                {t('publish.open', { ns: 'ecommerceMgmt' })}
                            </Label>

                            <Controller
                                name='isEnableBookingPickupDate'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='isEnableBookingPickupDate_1'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='isEnableBookingPickupDate_1'>
                                {t('publish.close', { ns: 'ecommerceMgmt' })}
                            </Label>
                        </div>
                    </Col>
                    {isEnableBookingPickupDate && (
                        <Col sm='12'>
                            <div>
                                <Row className='mx-0 form-control d-flex px-0 py-1'>
                                    <Col sm='3'>
                                        <Label className='form-label' for='prepareDays'>
                                            {t('publish.prepareDays', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                        </Label>
                                        <Controller
                                            name='prepareDays'
                                            control={control}
                                            render={({ field }) => (
                                                <Input type='number' min='0' max='60' {...field} onFocus={e => e.target.select()} />
                                            )}

                                        />
                                    </Col>
                                    <Col sm='3'>
                                        <Label className='form-label' for='prepareDays'>
                                            {t('publish.availablePickupText', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                        </Label>
                                        <div className='d-flex flex-grow-1 align-items-center'>
                                            <Controller
                                                name='availablePickupFlag'
                                                control={control}
                                                render={({ field }) => (
                                                    <Input type='radio' id='availablePickupFlag_1'  {...field} checked={field.value === '1'} value='1' onChange={onRadioChange} />
                                                )}
                                            />
                                            <Label className='ms-25 me-1 form-check-label' for='availablePickupFlag_1'>
                                                {(t('publish.availablePickupDate', { ns: 'ecommerceMgmt' }))}
                                            </Label>
                                            {
                                                availablePickupFlag === '1' && (
                                                    <Controller
                                                        name='availablePickupStartDateTime'
                                                        control={control}
                                                        render={({ field }) => (
                                                            <Input type='date' id='availablePickupStartDateTime' className='w-auto' {...field} />
                                                        )}
                                                    />
                                                )
                                            }
                                        </div>
                                    </Col>
                                </Row>
                            </div>
                        </Col>
                    )}
                </Row>
                <Row className='mb-1'>
                    <Col sm='2'>
                        <Label className='form-label'>
                            {t('publish.length', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='length'
                            control={control}
                            render={({ field }) => (
                                <Input {...field} type='number' onFocus={e => e.target.select()} />
                            )}
                        />
                    </Col>
                    <Col sm='2'>
                        <Label className='form-label'>
                            {t('publish.width', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='wIdth'
                            control={control}
                            render={({ field }) => (
                                <Input {...field} type='number' onFocus={e => e.target.select()} />
                            )}
                        />
                    </Col>
                    <Col sm='2'>
                        <Label className='form-label'>
                            {t('publish.height', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='height'
                            control={control}
                            render={({ field }) => (
                                <Input {...field} type='number' onFocus={e => e.target.select()} />
                            )}
                        />
                    </Col>
                    <Col sm='2'>
                        <Label className='form-label'>
                            {t('publish.weight', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='weight'
                            control={control}
                            render={({ field }) => (
                                <Input {...field} type='number' onFocus={e => e.target.select()} />
                            )}
                        />
                    </Col>
                </Row>
                <Row className='mb-1'>
                    <Col sm='4'>
                        <Label className='form-label' for='temperatureTypeDef'>
                            {t('publish.temperatureTypeDef', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                            <Controller
                                name='temperatureTypeDef'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='temperatureTypeDef_Normal'  {...field} checked={field.value === 'Normal'} value={'Normal'} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='temperatureTypeDef_Normal'>
                                常溫
                            </Label>
                            <Controller
                                name='temperatureTypeDef'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='temperatureTypeDef_Refrigerator'  {...field} checked={field.value === 'Refrigerator'} value={'Refrigerator'} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='temperatureTypeDef_Refrigerator'>
                                冷藏
                            </Label>
                            <Controller
                                name='temperatureTypeDef'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='temperatureTypeDef_Freezer'  {...field} checked={field.value === 'Freezer'} value={'Freezer'} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='temperatureTypeDef_Freezer'>
                                冷凍
                            </Label>
                        </div>
                    </Col>
                </Row>
                <Row className='mb-1'>
                    <Col sm='12'>
                        <Label className='form-label' for='productDescription'>
                            {t('publish.productDescription', { ns: 'ecommerceMgmt' })}
                        </Label>
                        <Controller
                            name='productDescription'
                            control={control}
                            render={({ field }) => (
                                <Input {...field} type='textarea' />
                            )}

                        />
                    </Col>
                </Row>
                <Row className='mb-1'>
                    <Col sm='12'>
                        <Label className='form-label' for='hasSku_0'>
                            {t('publish.hasSku', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span>
                        </Label>
                        <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                            <Controller
                                name='hasSku'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='hasSku_0'  {...field} checked={field.value === false} value={false} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='hasSku_0'>
                                無選項
                            </Label>

                            <Controller
                                name='hasSku'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='hasSku_1'  {...field} checked={field.value === true} value={true} onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='hasSku_1'>
                                有選項
                            </Label>

                        </div>
                    </Col>

                    <Col sm='12' className='mb-1'>
                        <div>
                            <Row className='mx-0 form-control d-flex px-0 py-1'>
                                <Col sm='12'>
                                    <Table bordered responsive>
                                        <tbody>
                                            {!hasSku && (
                                                <>
                                                    <tr>
                                                        <td style={{ width: '25%' }}>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.suggestPrice', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='suggestPrice'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input type='number' id='suggestPrice' style={{ width: '15em' }} {...field} onFocus={e => e.target.select()} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.price', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='price'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input type='number' id='price' style={{ width: '15em' }} {...field} onFocus={e => e.target.select()} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.cost', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='cost'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input type='number' id='cost' style={{ width: '15em' }} {...field} onFocus={e => e.target.select()} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.outerId', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='outerId'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input id='outerId' style={{ width: '15em' }} {...field} disabled={true} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.qty', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='qty'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input type='number' id='qty' min="0" max="999999" style={{ width: '15em' }} {...field} onFocus={e => e.target.select()} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.safetyStockQty', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='safetyStockQty'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input type='number' id='safetyStockQty' min="0" max="999999" style={{ width: '15em' }} {...field} onFocus={e => e.target.select()} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <Label className='form-label m-0'>
                                                                {t('publish.onceQty', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                                                            </Label>
                                                        </td>
                                                        <td>
                                                            <Controller
                                                                name='onceQty'
                                                                control={control}
                                                                render={({ field }) => (
                                                                    <Input type='number' id='onceQty' min="1" max="50" style={{ width: '15em' }} {...field} onFocus={e => e.target.select()} />
                                                                )}
                                                            />
                                                        </td>
                                                    </tr>
                                                </>
                                            )}
                                        </tbody>
                                    </Table>
                                </Col>
                                {hasSku && (
                                    <>
                                        <small className='fw-light mt-1 mb-25'>• 輸入選項後，項目請至少輸入一項，選項及項目名稱不支援半形符號，可使用全形符號（如＋,－,＆）替代。</small>
                                        {optionFields.map((item, index) => {
                                            return (
                                                <Row className='mb-25' key={item.id}>
                                                    <Col sm='3' className='mb-25'>
                                                        <Controller
                                                            name={`optionList.${index}.name`}
                                                            control={control}
                                                            render={({ field }) => (
                                                                <Input {...field} placeholder='請輸入選項，如：尺寸' disabled={true} />
                                                            )}
                                                        />
                                                    </Col>
                                                    <Col sm='8' className='mb-25'>
                                                        <Controller
                                                            name={`optionList.${index}.options`}
                                                            control={control}
                                                            render={({ field }) => (
                                                                <CreatableSelect
                                                                    {...field}
                                                                    classNamePrefix='select'
                                                                    components={components}
                                                                    inputValue={field.value.inputValue}
                                                                    isClearable
                                                                    isDisabled={true}
                                                                    isMulti
                                                                    menuIsOpen={false}
                                                                    onChange={(newValue) => field.onChange({ ...field.value, value: newValue })}
                                                                    onInputChange={(newValue) => field.onChange({ ...field.value, inputValue: newValue })}
                                                                    onKeyDown={(event) => {
                                                                        if (!field.value.inputValue) return
                                                                        switch (event.key) {
                                                                            case 'Enter':
                                                                            case 'Tab':
                                                                                field.onChange({ inputValue: '', value: [...field.value.value, createOption(field.value.inputValue)] })
                                                                                event.preventDefault()
                                                                        }
                                                                    }}
                                                                    placeholder="請輸入項目，如：S，並按Tab新增"
                                                                    value={field.value.value}
                                                                    theme={selectThemeColors}
                                                                    styles={selectStyle}
                                                                />
                                                            )}
                                                        />
                                                    </Col>
                                                </Row>
                                            )
                                        })}
                                        {skuFields.length > 0 && (
                                            <Col sm='12'>
                                                <div className="ag-theme-quartz has-border my-1" >
                                                    <AgGridReact
                                                        rowData={skuFields}
                                                        columnDefs={columnDefs}
                                                        domLayout='autoHeight'
                                                        rowDragManaged={true}
                                                        rowHeight={110}
                                                        onRowDragEnd={onRowDragEnd}
                                                        getRowId={params => params.data.id}
                                                    />
                                                </div>
                                            </Col>
                                        )}

                                    </>
                                )}
                            </Row>
                        </div>
                    </Col>

                    <Col sm='12' className='mb-1'>
                        <Label className='form-label' for='moreInfo'>
                            {t('publish.moreInfo', { ns: 'ecommerceMgmt' })}
                        </Label>
                        <Controller
                            name='moreInfo'
                            control={control}
                            render={({ field }) => (
                                <ReactQuill
                                    theme="snow"
                                    value={field.value || ''}
                                    onChange={field.onChange}
                                    modules={quillModules}
                                    formats={quillFormats}
                                    style={{ height: '400px', marginBottom: '50px' }}  // 預留工具列空間
                                />
                            )}
                        />
                    </Col>
                    <Col sm='12'>
                        <Label className='form-label' for='specifications'>
                            {t('publish.specifications', { ns: 'ecommerceMgmt' })}
                        </Label>
                        {indexListFields.map((item, index) => {
                            const ecIndex = option.ecIndexOption.find(option => option.value === item.keyNo)
                            const options = ecIndex?.options || []
                            const isMultipleSelect = ecIndex?.isMultipleSelect

                            return (
                                <Row className='mb-25' key={item.id}>
                                    <Col sm='3'>
                                        <Controller
                                            name={`indexList.${index}.key`}
                                            control={control}
                                            render={({ field }) => (
                                                <Input {...field} disabled={true} />
                                            )}
                                        />
                                    </Col>
                                    <Col sm='6'>
                                        <Controller
                                            name={`indexList.${index}.value`}
                                            control={control}
                                            render={({ field }) => {
                                                return isMultipleSelect ? (
                                                    <MultiSelect
                                                        formatOptionLabel={formatSelectOptionLabelNL}
                                                        isMulti={isMultipleSelect}
                                                        isClearable={true}
                                                        closeMenuOnSelect={false}
                                                        classNamePrefix='select'
                                                        options={options}
                                                        theme={selectThemeColors}
                                                        styles={selectStyleNHL}
                                                        className={classnames('react-select')}
                                                        placeholder={t('selectPlaceholder', { ns: 'common' })}
                                                        {...field}
                                                    />
                                                ) : (
                                                    <Select
                                                        options={options}
                                                        formatOptionLabel={formatSelectOptionLabelNL}
                                                        {...field}
                                                        value={Array.isArray(field.value) && field.value.length ? field.value[0] : field.value}
                                                        onChange={(value) => field.onChange([value])}
                                                    />
                                                )
                                            }}
                                        />
                                    </Col>
                                    <Col sm='1' className='d-flex align-items-center'>
                                        {index === indexListFields?.length - 1 && (
                                            <Link className='text-body'
                                                href='/'
                                                onClick={e => {
                                                    e.preventDefault()
                                                    specificationsAppend({ key: '', value: '', platformSource: 'custom' })
                                                }}>
                                                <Plus size={16} className='ms-50' />
                                            </Link>
                                        )}
                                    </Col>
                                </Row>
                            )
                        })}
                    </Col>
                    {indexListFields.length === 0 && specificationsFields.length === 0 && (
                        <Col sm='12' className='mb-1 d-flex align-items-center'>
                            <Link className='text-body'
                                href='/'
                                onClick={e => {
                                    e.preventDefault()
                                    specificationsAppend({ key: '', value: '', platformSource: 'custom' })
                                }}>
                                <Plus size={16} className='ms-50' />
                            </Link>
                        </Col>
                    )}
                    <Col sm='12' className='mb-1'>
                        {specificationsFields.map((item, index) => {
                            return (
                                <Row className='mb-25' key={item.id}>
                                    <Col sm='3'>
                                        <Controller
                                            name={`specifications.${index}.key`}
                                            control={control}
                                            render={({ field }) => (
                                                <Input {...field} placeholder='如:材質' />
                                            )}
                                        />
                                    </Col>
                                    <Col sm='6'>
                                        <Controller
                                            name={`specifications.${index}.value`}
                                            control={control}
                                            render={({ field }) => (
                                                <Input {...field} placeholder='如:鋁合金' />
                                            )}
                                        />

                                    </Col>
                                    <Col sm='1' className='d-flex align-items-center'>
                                        <Link className='text-body'
                                            href='/'
                                            onClick={() => specificationsRemove(index)}>
                                            <Trash size={16} className='ms-50' />
                                        </Link>
                                        {index === specificationsFields?.length - 1 && (
                                            <Link className='text-body'
                                                href='/'
                                                onClick={e => {
                                                    e.preventDefault()
                                                    specificationsAppend({ key: '', value: '', platformSource: 'custom' })
                                                }}>
                                                <Plus size={16} className='ms-50' />
                                            </Link>
                                        )}
                                    </Col>
                                </Row>
                            )
                        })}
                    </Col>
                </Row>
            </CardBody>
        </Card >
    )
}

const renderAddiInfo = ({ arrVal, control, setValue, getValues, watch, t, option }) => {

    const applyType = watch('applyType')

    const onRadioChange = useCallback(({ target }) => {
        const { name, value } = target
        if (value === 'true' || value === 'false') {
            setValue(name, value === 'true')
        } else {
            setValue(name, value)
        }
    }, [])

    useEffect(() => {
        const currentDate = new Date()
        switch (applyType) {
            case '預購(指定出貨日)':
                setValue('expectShippingDate', getFormatedDateForInput(new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() + 3)))
                break
            case '預購(指定工作天)':
            case '訂製':
                setValue('shippingPrepareDay', 3)
                break
            default:
                break
        }
    }, [applyType])

    return (
        <Card className='mb-2'>
            <CardBody>
                <Row className='mb-1'>
                    <Col sm='3'>
                        <Label className='form-label d-flex' for='sellingStartDateTime'>
                            {t('publish.sellingStartDateTime', { ns: 'ecommerceMgmt' })} <span className='text-danger me-1'>*</span>
                            {/* <Avatar className='ms-50' img={'/assets/image/ecommerce/91app_logo.png'} imgHeight='20' imgWidth='20' />
                            <Avatar className='ms-50' img={'/assets/image/ecommerce/yahoo_logo.webp'} imgHeight='20' imgWidth='20' /> */}
                        </Label>
                        <Controller
                            name='sellingStartDateTime'
                            control={control}
                            render={({ field }) => (
                                <Input type='datetime-local' {...field} />
                            )}

                        />
                    </Col>
                    <Col sm='3'>
                        <Label className='form-label' for='sellingEndDateTime'>
                            {t('publish.sellingEndDateTime', { ns: 'ecommerceMgmt' })} <span className='text-danger'>*</span>
                        </Label>
                        <Controller
                            name='sellingDateTime'
                            control={control}
                            render={({ field }) => (
                                <Select
                                    options={option.sellingDateTimeOption}
                                    formatOptionLabel={formatSelectOptionLabelNL}
                                    {...field}
                                    onChange={(opt) => {
                                        field.onChange(opt)
                                        const currentDateTime = new Date(getValues('sellingStartDateTime'))
                                        currentDateTime.setFullYear(currentDateTime.getFullYear() + (opt.value === 0 ? 1 : opt.value === 1 ? 5 : opt.value === 2 ? 25 : 0))
                                        const newDateTimeValue = getDateTimeLocal(currentDateTime)
                                        setValue('sellingEndDateTime', newDateTimeValue)
                                    }}
                                />
                            )}
                        />
                    </Col>
                    <Col sm='3' className='align-self-end'>
                        <Controller
                            name='sellingEndDateTime'
                            control={control}
                            render={({ field }) => (
                                <Input type='datetime-local' {...field} />
                            )}

                        />
                    </Col>

                </Row>
                <Row className='mb-1'>
                    <Col sm='12'>
                        <Label className='form-label d-flex' for='applyType_0'>
                            {t('publish.applyType', { ns: 'ecommerceMgmt' })}<span className='text-danger me-1'>*</span>
                            {/* <Avatar className='ms-50' img={'/assets/image/ecommerce/91app_logo.png'} imgHeight='20' imgWidth='20' />
                            <Avatar className='ms-50' img={'/assets/image/ecommerce/yahoo_logo.webp'} imgHeight='20' imgWidth='20' /> */}
                        </Label>
                        <div className='d-flex flex-grow-1 align-items-center' style={{ lineHeight: '36px' }}>
                            <Controller
                                name='applyType'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='applyType_0'  {...field} checked={field.value === '一般'} value='一般' onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='applyType_0'>
                                一般 <small className='fw-light text-danger'>最晚出貨日前須出貨</small>
                            </Label>

                            <Controller
                                name='applyType'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='applyType_1'  {...field} checked={field.value === '預購(指定出貨日)'} value='預購(指定出貨日)' onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='applyType_1'>
                                預購(指定出貨日)
                            </Label>
                            {
                                applyType === '預購(指定出貨日)' && (
                                    <>
                                        <span className='fw-light me-25'>預定出貨日</span>
                                        <Controller
                                            name='expectShippingDate'
                                            control={control}
                                            render={({ field }) => (
                                                <Input type='date' id='expectShippingDate' className='w-auto me-1 ' {...field} />
                                            )}
                                        />
                                    </>
                                )
                            }

                            <Controller
                                name='applyType'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='applyType_2'  {...field} checked={field.value === '預購(指定工作天)'} value='預購(指定工作天)' onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='applyType_2'>
                                預購(指定工作天)
                            </Label>
                            {
                                applyType === '預購(指定工作天)' && (
                                    <>
                                        <span className='fw-light me-25'>付款完成後</span>
                                        <Controller
                                            name='shippingPrepareDay'
                                            control={control}
                                            render={({ field }) => (
                                                <Input type='number' min='3' max='120' id='shippingPrepareDay' style={{ width: '60px' }} className='me-25' {...field} onFocus={e => e.target.select()} />
                                            )}
                                        />
                                        <span className='fw-light me-1'>個工作天出貨</span>
                                    </>
                                )
                            }


                            <Controller
                                name='applyType'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='applyType_3'  {...field} checked={field.value === '訂製'} value='訂製' onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='applyType_3'>
                                訂製
                            </Label>
                            {
                                applyType === '訂製' && (
                                    <>
                                        <span className='fw-light me-25'>付款完成後</span>
                                        <Controller
                                            name='shippingPrepareDay'
                                            control={control}
                                            render={({ field }) => (
                                                <Input type='number' min='3' max='120' id='shippingPrepareDay' style={{ width: '60px' }} className='me-25' {...field} onFocus={e => e.target.select()} />
                                            )}
                                        />
                                        <span className='fw-light me-1'>個工作天出貨</span>
                                    </>
                                )
                            }

                            <Controller
                                name='applyType'
                                control={control}
                                render={({ field }) => (
                                    <Input type='radio' id='applyType_4'  {...field} checked={field.value === '客約'} value='客約' onChange={onRadioChange} />
                                )}
                            />
                            <Label className='ms-25 me-1 form-check-label' for='applyType_4'>
                                客約
                            </Label>
                        </div>
                    </Col>
                </Row>
                {/* <Row className='mb-1'>
                    <Col sm='3'>
                        <Label className='form-label d-flex' for='expDays'>
                            {t('publish.expDays', { ns: 'ecommerceMgmt' })} <span className='text-danger me-1'>*</span>
                            <Avatar className='ms-50' img={'/assets/image/ecommerce/momo_logo.png'} imgHeight='20' imgWidth='20' />
                            <Avatar className='ms-50' img={'/assets/image/ecommerce/yahoo_logo.webp'} imgHeight='20' imgWidth='20' />
                        </Label>
                        <Controller
                            name='expDays'
                            control={control}
                            render={({ field }) => (
                                <Input type='number' {...field} />
                            )}

                        />
                    </Col>
                </Row> */}
            </CardBody>
        </Card >
    )
}

const BasicInfo = ({ control, setValue, getValues, reset, errors, watch, t, id, arrVal, setArrVal, status, option }) => {

    return (
        <Row className='g-0'>
            <Col sm='12'>
                <h6 className='section-label'>{t('publish.info', { ns: 'ecommerceMgmt' })}</h6>
                {renderInfo({ arrVal, control, setValue, getValues, errors, watch, t, option })}
            </Col>
            <Col sm='12'>
                <h6 className='section-label'>{t('publish.addiInfo', { ns: 'ecommerceMgmt' })}</h6>
                {renderAddiInfo({ arrVal, control, setValue, getValues, errors, watch, t, option })}
            </Col>
            <Col sm='12'>
                <h6 className='section-label'>{t('publish.shop', { ns: 'ecommerceMgmt' })}</h6>
                {renderShop({ arrVal, control, setValue, getValues, errors, watch, t, option })}
            </Col>
        </Row>
    )
}

export default BasicInfo
