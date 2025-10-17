import { useState, useEffect, useMemo, Fragment, useCallback, forwardRef, useImperativeHandle } from 'react'
import { Link } from 'react-router-dom'
import { Controller } from 'react-hook-form'
import { Input, Button } from 'reactstrap'
import { X, Trash, Plus } from 'react-feather'
import { useDropzone } from 'react-dropzone'
import { ICellEditorParams, ICellEditorComp } from 'ag-grid-community'

const ImageCellRenderer = ({ rowIndex, value, control, data }) => {
    const fieldName = `skuList.${rowIndex}.image`

    const onDrop = useCallback((acceptedFiles, field) => {
        const file = acceptedFiles[0]
        const newBlobUrl = URL.createObjectURL(file)
        const reader = new FileReader()
        reader.onload = () => {
            if (field.value?.preview) {
                URL.revokeObjectURL(field.value.preview)
            }
            field.onChange({
                file,
                preview: newBlobUrl
            })
        }
        reader.readAsDataURL(file)
    }, [])

    return (
        <Controller
            key={`${fieldName}-${data?.outerId || rowIndex}`}
            name={fieldName}
            control={control}
            defaultValue={null}
            render={({ field }) => {
                const { getRootProps, getInputProps } = useDropzone({
                    onDrop: (acceptedFiles) => onDrop(acceptedFiles, field),
                    accept: 'image/*'
                })

                const deleteMainImage = useCallback(() => {
                    if (field.value?.preview) {
                        URL.revokeObjectURL(field.value.preview)
                    }
                    field.onChange(null)
                }, [])

                return field.value?.preview || field.value?.path ? (
                    <div className='thumb'>
                        <div className='thumbInner'>
                            <img
                                src={field.value?.path ? `${process.env.NODE_ENV === 'production' ? window.location.origin : 'https://localhost:44320'}/${field.value?.path}?v=${new Date().getTime()}` : field.value?.preview}
                                className='img'
                                alt='Preview'
                            />
                            <div style={{ position: 'absolute', top: '0px', right: '0px', padding: '5px' }}>
                                <Button color='flat-secondary' className='btn-icon p-0 h-auto' onClick={deleteMainImage}><X size={18} /></Button>
                            </div>
                        </div>
                    </div>
                ) : (
                    <div {...getRootProps({ className: 'dropzone-small' })}>
                        <input {...getInputProps()} />
                        <div className='d-flex align-items-center justify-content-center flex-column'>
                            <Plus size={40} />
                        </div>
                    </div>
                )
            }}
        />
    )
}


export const getColumnDefs = ({ t, control, getValues, setValue }) => {

    const applyAll = (field, value) => {
        const skuList = getValues('skuList')
        const updatedSkuList = skuList.map((item) => ({
            ...item,
            [field]: value
        }))
        setValue('skuList', updatedSkuList)
    }

    return [
        {
            headerName: '',
            flex: 1,
            field: 'drag',
            rowDrag: true
        },
        {
            headerName: t('publish.picture', { ns: 'ecommerceMgmt' }),
            flex: 3,
            field: 'image',
            cellRenderer: (params) => (
                <ImageCellRenderer
                    rowIndex={params.rowIndex}
                    value={params.value}
                    control={control}
                    data={params.data}
                />
            ),
            cellRendererParams: {
                // 強制 AG-Grid 在資料變更時重新渲染
                suppressCellFlash: false
            },
            valueFormatter: ({ data }) => data.path
        },
        {
            headerName: t('publish.sku', { ns: 'ecommerceMgmt' }),
            flex: 3,
            field: 'name',
            valueFormatter: ({ data }) => (data.colDetail1?.label && data.colDetail2?.label ? `${data.colDetail1.label} ; ${data.colDetail2.label}` : data.colDetail1?.label ? data.colDetail1.label : data.colDetail2?.label ? data.colDetail2.label : '')

        },
        {
            headerComponent: () => (<span>{t('publish.outerId', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'outerId'
        },
        {
            headerComponent: () => (<span>{t('publish.suggestPrice', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'suggestPrice',
            cellRenderer: ({ rowIndex, api }) => {
                const rowCount = api.getDisplayedRowCount()
                return (
                    <Controller
                        name={`skuList.${rowIndex}.suggestPrice`}
                        control={control}
                        render={({ field }) => (
                            <div className='d-flex flex-column'>
                                <Input type='number' min="0"  {...field} onFocus={e => e.target.select()} onChange={(e) => field.onChange(e.target.valueAsNumber)} />
                                {rowIndex === 0 && rowCount > 1 &&
                                    <Link className='text-body  pt-25' href='/'
                                        onClick={e => {
                                            e.preventDefault()
                                            applyAll('suggestPrice', field.value)
                                        }}>
                                        <span className='text-primary'>以下全同</span>
                                    </Link>}
                            </div>
                        )}
                    />
                )
            }
        },
        {
            headerComponent: () => (<span>{t('publish.price', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'price',
            cellRenderer: ({ rowIndex, api }) => {
                const rowCount = api.getDisplayedRowCount()
                return (
                    <Controller
                        name={`skuList.${rowIndex}.price`}
                        control={control}
                        render={({ field }) => (
                            <div className='d-flex flex-column'>
                                <Input type='number' min="0"  {...field} onFocus={e => e.target.select()} onChange={(e) => field.onChange(e.target.valueAsNumber)} />
                                {rowIndex === 0 && rowCount > 1 &&
                                    <Link className='text-body  pt-25' href='/'
                                        onClick={e => {
                                            e.preventDefault()
                                            applyAll('price', field.value)
                                        }}>
                                        <span className='text-primary'>以下全同</span>
                                    </Link>}
                            </div>
                        )}
                    />
                )
            }
        },
        {
            headerComponent: () => (<span>{t('publish.cost', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'cost',
            cellRenderer: ({ rowIndex, api }) => {
                const rowCount = api.getDisplayedRowCount()
                return (
                    <Controller
                        name={`skuList.${rowIndex}.cost`}
                        control={control}
                        render={({ field }) => (
                            <div className='d-flex flex-column'>
                                <Input type='number' min="0"  {...field} onFocus={e => e.target.select()} onChange={(e) => field.onChange(e.target.valueAsNumber)} />
                                {rowIndex === 0 && rowCount > 1 &&
                                    <Link className='text-body  pt-25' href='/'
                                        onClick={e => {
                                            e.preventDefault()
                                            applyAll('cost', field.value)
                                        }}>
                                        <span className='text-primary'>以下全同</span>
                                    </Link>}
                            </div>
                        )}
                    />
                )
            }
        },
        {
            headerComponent: () => (<span>{t('publish.qty', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'qty',
            cellRenderer: ({ rowIndex, api }) => {
                const rowCount = api.getDisplayedRowCount()
                return (
                    <Controller
                        name={`skuList.${rowIndex}.qty`}
                        control={control}
                        render={({ field }) => (
                            <div className='d-flex flex-column'>
                                <Input type='number' min="0" max="999999"  {...field} onFocus={e => e.target.select()} onChange={(e) => field.onChange(e.target.valueAsNumber)} />
                                {rowIndex === 0 && rowCount > 1 &&
                                    <Link className='text-body  pt-25' href='/'
                                        onClick={e => {
                                            e.preventDefault()
                                            applyAll('qty', field.value)
                                        }}>
                                        <span className='text-primary'>以下全同</span>
                                    </Link>}
                            </div>
                        )}
                    />
                )
            }
        },
        {
            headerComponent: () => (<span>{t('publish.safetyStockQty', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'safetyStockQty',
            cellRenderer: ({ rowIndex, api }) => {
                const rowCount = api.getDisplayedRowCount()
                return (
                    <Controller
                        name={`skuList.${rowIndex}.safetyStockQty`}
                        control={control}
                        render={({ field }) => (
                            <div className='d-flex flex-column'>
                                <Input type='number' min="0" max="999999"  {...field} onFocus={e => e.target.select()} onChange={(e) => field.onChange(e.target.valueAsNumber)} />
                                {rowIndex === 0 && rowCount > 1 &&
                                    <Link className='text-body  pt-25' href='/'
                                        onClick={e => {
                                            e.preventDefault()
                                            applyAll('safetyStockQty', Number(field.value))
                                        }}>
                                        <span className='text-primary'>以下全同</span>
                                    </Link>}
                            </div>
                        )}
                    />
                )
            }
        },
        {
            headerComponent: () => (<span>{t('publish.onceQty', { ns: 'ecommerceMgmt' })}<span className='text-danger'>*</span></span>),
            flex: 3,
            field: 'onceQty',
            cellRenderer: ({ rowIndex, api }) => {
                const rowCount = api.getDisplayedRowCount()
                return (
                    <Controller
                        name={`skuList.${rowIndex}.onceQty`}
                        control={control}
                        render={({ field }) => (
                            <div className='d-flex flex-column'>
                                <Input type='number'  {...field} onFocus={e => e.target.select()} onChange={(e) => field.onChange(e.target.valueAsNumber)} />
                                {rowIndex === 0 && rowCount > 1 &&
                                    <Link className='text-body  pt-25' href='/'
                                        onClick={e => {
                                            e.preventDefault()
                                            applyAll('onceQty', field.value)
                                        }}>
                                        <span className='text-primary'>以下全同</span>
                                    </Link>}
                            </div>
                        )}
                    />
                )
            }
        }
    ]
}


export const getSizeIndexColumnDefs = ({ control, clothDataType, option, clothDataSizeIndexFields, onRemove, onAdd }) => {
    const currentSizeItems = option.clothDataSizeIndex.find(
        item => item.value === clothDataType?.value
    )?.items || []

    const dynamicColumns = currentSizeItems.map((item, colIndex) => ({
        headerName: item,
        field: `${colIndex}`,
        flex: 3,
        cellRenderer: params => {
            const { data } = params
            const actualRowIndex = clothDataSizeIndexFields.findIndex(field => field.id === data.id)

            if (actualRowIndex === -1) return null

            return (
                <Controller
                    name={`clothDataSizeIndex.${actualRowIndex}.${colIndex}`}
                    control={control}
                    render={({ field }) => (
                        <Input
                            {...field}
                            onFocus={e => e.target.select()}
                        />
                    )}
                />
            )
        }
    }))

    const actionColumn = {
        headerName: '',
        field: 'actions',
        flex: 1,
        cellRenderer: params => {
            const { data, api } = params
            const rowCount = api.getDisplayedRowCount()
            const isLastRow = api.getRowNode(data.id).rowIndex === rowCount - 1

            return (
                <div className="d-flex justify-content-between">
                    <Link className='text-body'
                        href='/'
                        onClick={() => {
                            const indexToRemove = clothDataSizeIndexFields.findIndex(field => field.id === data.id)
                            if (indexToRemove !== -1) {
                                onRemove(indexToRemove)
                            }
                        }}
                    >
                        <Trash size={16} className='ms-50' />
                    </Link>
                    {isLastRow && (
                        <Link className='text-body'
                            href='/'
                            onClick={() => {
                                const newRow = currentSizeItems.reduce((acc, _, index) => {
                                    acc[index] = ''
                                    return acc
                                }, {})

                                onAdd(newRow)
                            }}
                        >
                            <Plus size={16} className='ms-50' />
                        </Link>
                    )}
                </div>
            )
        }
    }

    return [...dynamicColumns, actionColumn]
}

export const getTryIndexColumnDefs = ({ control, clothDataType, option, clothDataTryIndexFields, onRemove, onAdd }) => {
    const currentSizeItems = option.clothDataTryIndex.find(
        item => item.value === clothDataType?.value
    )?.items || []

    const dynamicColumns = currentSizeItems.map((item, colIndex) => ({
        headerName: item,
        field: `${colIndex}`,
        flex: 3,
        cellRenderer: params => {
            const { data } = params
            const actualRowIndex = clothDataTryIndexFields.findIndex(field => field.id === data.id)

            if (actualRowIndex === -1) return null

            return (
                <Controller
                    name={`clothDataTryIndex.${actualRowIndex}.${colIndex}`}
                    control={control}
                    render={({ field }) => (
                        <Input
                            {...field}
                            onFocus={e => e.target.select()}
                        />
                    )}
                />
            )
        }
    }))

    const actionColumn = {
        headerName: '',
        field: 'actions',
        flex: 1,
        cellRenderer: params => {
            const { data, api } = params
            const rowCount = api.getDisplayedRowCount()
            const isLastRow = api.getRowNode(data.id).rowIndex === rowCount - 1

            return (
                <div className="d-flex justify-content-between">

                    <Link className='text-body'
                        href='/'
                        onClick={() => {
                            const indexToRemove = clothDataTryIndexFields.findIndex(field => field.id === data.id)
                            if (indexToRemove !== -1) {
                                onRemove(indexToRemove)
                            }
                        }}
                    >
                        <Trash size={16} className='ms-50' />
                    </Link>

                    {isLastRow && (
                        <Link className='text-body'
                            href='/'
                            onClick={() => {
                                const newRow = currentSizeItems.reduce((acc, _, index) => {
                                    acc[index] = ''
                                    return acc
                                }, {})

                                onAdd(newRow)
                            }}
                        >
                            <Plus size={16} className='ms-50' />
                        </Link>
                    )}
                </div>
            )
        }
    }

    return [...dynamicColumns, actionColumn]
}

export const createIndexRow = (items) => {
    return items.reduce((acc, _, index) => {
        acc[index] = ''
        return acc
    }, {})
}
