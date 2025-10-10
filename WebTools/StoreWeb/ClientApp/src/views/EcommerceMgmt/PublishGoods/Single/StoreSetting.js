// ** React Import
import { useState, useEffect, useMemo, Fragment, useCallback, Children } from 'react'
import axios from 'axios'

// ** Reactstrap Imports
import { Label, Input, Row, Col, Alert } from 'reactstrap'

// ** Icons Imports
import { Check, X } from 'react-feather'

// ** Third Party Components
import { Cascader } from 'antd'

// ** Utils
import { selectThemeColors, selectStyle, formatSelectOptionLabelNL, getUserData } from '@utils'
import { CustomLabel, MultiSelect, Select, CustomArrow } from '@CityAppComponents'
import { Controller, useFieldArray } from 'react-hook-form'

const getRootCategories = async (apiName, request) => {
    const rootCategories = []
    await axios.post(`/api/PublishGoods/${apiName}`, request)
        .then(res => {
            if (!res.error) {
                const categories = res.data.data?.data
                categories.forEach(category => {
                    const levels = category.categoryLevel.split(' > ')
                    const categoryId = category.categoryId

                    // 初始化每個層級的分類
                    levels.reduce((parent, levelName, index) => {
                        const existingCategory = parent.children.find(c => c.label === levelName)

                        if (existingCategory) {
                            return existingCategory
                        } else {
                            const newCategory = {
                                value: index === levels.length - 1 ? categoryId : levelName, // 只有最深層級的分類會有 id
                                label: levelName,
                                children: []
                            }
                            parent.children.push(newCategory)
                            return newCategory
                        }
                    }, { children: rootCategories })
                })
            }
        })
    return rootCategories
}

const StoreSetting = ({ t, reset, getValues, setValue, control, arrVal }) => {
    const [storeTag, setStoreTag] = useState([])

    useEffect(() => {
        const fetchData = async () => {
            await axios.get('/api/EcommerceStore/GetEStoreTag')
                .then((res) => {
                    reset({
                        ...getValues(),
                        storeSettings: res.data.data.result?.map(store => {
                            return {
                                platformID: store.platformID,
                                eStoreID: store.eStoreID,
                                publish: false,
                                category0: null,
                                category1: null,
                                cost: 0,
                                title: ''
                            }
                        })
                    })
                    setStoreTag(res.data.data.result)
                })

        }

        fetchData()
    }, [])

    const { fields: storeSettingFields, append: storeSettingAppend, remove: storeSettingRemove, replace: storeSettingReplace } = useFieldArray(
        {
            control,
            name: 'storeSettings'
        }
    )

    return (
        <div>

            {storeSettingFields?.length > 0 ? (
                <>
                    <Row className='d-flex align-items-center gx-0'>
                        <Col sm='3' className='d-flex'>
                            <span className='form-label'>平台</span>
                        </Col>
                        <Col sm='1'>
                            <span className='form-label'>{t('publish.cost', { ns: 'ecommerceMgmt' })}</span>
                        </Col>
                        <Col sm='4'>
                            <span className='form-label ms-50' >{t('publish.title', { ns: 'ecommerceMgmt' })}</span>
                        </Col>
                    </Row>
                    {storeSettingFields.map((item, index) => {
                        return (
                            <Row className='d-flex align-items-center g-0 mb-1' key={item.id}>
                                <Col sm='3' className='d-flex'>
                                    <div className='form-switch form-check-success'>
                                        <Controller
                                            name={`storeSettings.${index}.publish`}
                                            control={control}
                                            render={({ field }) => (
                                                <Input {...field} type='switch' id={`storeSettings.${index}.publish`} name={`storeSettings.${index}.publish`} checked={field.value} />
                                            )}
                                        />
                                        <CustomLabel htmlFor={`storeSettings.${index}.publish`} />
                                    </div>
                                    <div
                                        className='d-flex justify-content-center align-items-center'
                                        style={{
                                            width: 'auto',
                                            height: '30px',
                                            cursor: 'pointer'
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
                                                backgroundColor: `${storeTag?.[index]?.eStoreStyle}`,
                                                color: 'white',
                                                fontSize: '1rem'
                                            }}
                                        >{storeTag?.[index]?.storeName}</div>
                                    </div>
                                </Col>

                                <Col sm='1'>
                                    <Controller
                                        name={`storeSettings.${index}.cost`}
                                        control={control}
                                        render={({ field }) => (
                                            <Input {...field} onFocus={(e) => e.target.select()} />
                                        )}
                                    />
                                </Col>
                                <Col sm='4'>
                                    <Controller
                                        name={`storeSettings.${index}.title`}
                                        control={control}
                                        render={({ field }) => (
                                            <Input {...field} className='ms-50' />
                                        )}
                                    />
                                </Col>
                            </Row>
                        )
                    })}
                </>
            ) : (<Alert color='primary' transition={{
                timeout: 300,
                appear: true
            }}>
                <div className='alert-body'>
                    <span>
                        請先新增電商平台資料!
                    </span>
                </div>
            </Alert>)
            }
        </div >
    )
}

export default StoreSetting
