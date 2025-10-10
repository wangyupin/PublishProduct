import { Fragment, useState, useRef } from 'react'
import { toast } from 'react-toastify'
import { Bell, Check, X, AlertTriangle, Info } from 'react-feather'

import Avatar from '@components/avatar'
import XLSX from 'xlsx'
import * as FileSaver from 'file-saver'

// ** CityApp Utilty
import { getFullDateTime, getFullDateTimeTW } from '@CityAppHelper'
import { getUserData } from '@utils'

// for test
// const dataListTmp = [
//     {
//         name: 'Leanne Graham',
//         age: 36
//     },
//     {
//         name: 'Ervin Howell',
//         age: 36
//     },
//     {
//         name: 'Clementine Bauch',
//         age: 4951
//     }
// ]

// const columnListTmp = [
//     {
//         pinned: 'left',
//         headerName: '姓名',
//         field: 'name'
//     },
//     {
//         headerName: '年紀',
//         field: 'age'
//     }
// ]

const SuccessToast = ({fullFileName}) => {
    return (
        <Fragment>
            <div className='toastify-header'>
                <div className='title-wrapper'>
                    <Avatar size='sm' color='success' icon={<Check size={12} />} />
                    <h6 className='toast-title'>匯出完成!</h6>
                </div>
                <small className='text-muted'>{getFullDateTimeTW()}</small>
            </div>
            <div className='toastify-body'>
                <span role='img' aria-label='toast-text'>
                    {fullFileName}
                </span>
            </div>
        </Fragment>
    )
}

// export function ExeclExporter (dataList = [...dataListTmp], columnList = [...columnListTmp], fileFormat = 'xlsx', fileName = 'test123') {
export function ExeclExporter(dataList, columnList, fileFormat = 'xlsx', fileName) {
    if (dataList.length < 0) return
    const sheetHeader = Object.keys(dataList[0])

    const headerData = {}
    for (const [key, value] of Object.entries(columnList)) {
        //console.log(`field=${value.field}, headerName=${value.headerName}`)
        headerData[value.field] = value.headerName
    }
    // getColumnKeyValueList(columnList)
    const sheetData = [headerData, ...dataList]

    const ws = XLSX.utils.json_to_sheet(sheetData, { header: sheetHeader, skipHeader: true })
    const wb = XLSX.utils.book_new()
    
    // XLSX.utils.book_append_sheet(wb, ws, "CityApp Exporter")
    XLSX.utils.book_append_sheet(wb, ws, `${getUserData().userId}@${getUserData().companyId}`)
    const wbout = XLSX.write(wb, { bookType: fileFormat, bookSST: true, type: 'binary' })

    const s2ab = s => {
        const buf = new ArrayBuffer(s.length)
        const view = new Uint8Array(buf)
        for (let i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xff
        return buf
    }
    const fullFileName = fileName.length ? `${fileName}_${getFullDateTime()}.${fileFormat}` : `ExeclExporter.${fileFormat}`

    FileSaver.saveAs(new Blob([s2ab(wbout)], { type: 'application/octet-stream' }), fullFileName)

    return toast.success(<SuccessToast fullFileName={fullFileName} />, { hideProgressBar: false, autoClose:2688 })
}
