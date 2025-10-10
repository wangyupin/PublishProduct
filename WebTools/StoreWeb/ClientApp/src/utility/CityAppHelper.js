import { useEffect, useRef, forwardRef, useCallback } from 'react'
import ReactSelect, { components } from "react-select"

// ** Styles Imports
import '@styles/react/libs/react-select/_react-select.scss'
import 'animate.css/animate.css'
import '@styles/base/plugins/extensions/ext-component-sweet-alerts.scss'

import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'
import * as Excel from "exceljs"
import { saveAs } from 'file-saver'
import Avatar from '@components/avatar'
import CryptoJS from 'crypto-js'

const MySwal = withReactContent(Swal)

export const showMessageBox = (fireInfo) => {
    return MySwal.fire(fireInfo)
}

export function StringPad(string, length, padString = '0') {
    let result = string
    while (result.length < length) {
        result = padString + result
    }
    return result
}

//roound to fix 2
export const roundToTwo = (num) => Math.round(Number((Math.abs(num) * 100).toPrecision(15))) / 100 * Math.sign(num)

export const parseIntWithReomveComma = (value) => {
    const newvalue = value?.toString().replaceAll(',', '')

    return (isNaN(newvalue) || newvalue.length < 1) ? 0 : parseInt(newvalue)
}

export const parseFloatWithReomveComma = (value) => {
    const newvalue = value.toString().replaceAll(',', '')

    return isNaN(newvalue) ? 0 : parseFloat(newvalue)
}

export const parseIntPlus = (data) => {
    const result = parseInt(data, 10)
    return isNaN(result) ? -1 : result
}

// ** data time format
// YYYY/M/D
export const getFormatedDate = () => {
    const today = new Date()
    return `${today.getFullYear()}/${(today.getMonth() + 1)}/${today.getDate()}`
}

// YYYY-MM-DD
export function getFormatedDateForInput(target = new Date()) {
    const year = target.getFullYear()
    const month = StringPad(`${target.getMonth() + 1}`, 2)
    const day = StringPad(`${target.getDate()}`, 2)
    return `${year}-${month}-${day}`
}

// YYYY-MM
export function getFormatedMonthForInput(target = new Date()) {
    const year = target.getFullYear()
    const month = StringPad(`${target.getMonth() + 1}`, 2)
    return `${year}-${month}`
}

//YYYYMMDD
export function getFullDate(target = new Date()) {
    const year = target.getFullYear()
    const month = StringPad(`${target.getMonth() + 1}`, 2)
    const day = StringPad(`${target.getDate()}`, 2)
    return year + month + day
}

//YYYYMMDD to YYYY-MM-DD
export const convertDateStringForInput = (target) => {
    // const fullDate = target || getFullDate()
    const fullDate = target
    // return `${fullDate.substring(0, 4)}-${fullDate.substring(4, 6)}-${fullDate.substring(6, 8)}`
    if (fullDate && fullDate.length === 8) {
        return `${fullDate.substring(0, 4)}-${fullDate.substring(4, 6)}-${fullDate.substring(6, 8)}`
    } else {
        return ''
    }
}

//YYYYMM to YYYY-MM
export const convertMonthStringForInput = (target) => {
    // const fullDate = target || getFullDate()
    const fullMonth = target
    // return `${fullDate.substring(0, 4)}-${fullDate.substring(4, 6)}-${fullDate.substring(6, 8)}`
    if (fullMonth && fullMonth.length === 6) {
        return `${fullMonth.substring(0, 4)}-${fullMonth.substring(4, 6)}`
    } else {
        return ''
    }
}

//hhmmss
export function getFullTime(target = new Date()) {
    const hour = StringPad(`${target.getHours()}`, 2)
    const min = StringPad(`${target.getMinutes()}`, 2)
    const sec = StringPad(`${target.getSeconds()}`, 2)
    return hour + min + sec
}

//YYYYMMDDhhmmss
export function getFullDateTime(target = new Date()) {
    return getFullDate() + getFullTime()
}

//MM/DD hh:mm:ss
export function getFullDateTimeTW(target = new Date()) {
    const year = target.getFullYear()
    const month = StringPad(`${target.getMonth() + 1}`, 2)
    const day = StringPad(`${target.getDate()}`, 2)
    const hour = StringPad(`${target.getHours()}`, 2)
    const min = StringPad(`${target.getMinutes()}`, 2)
    const sec = StringPad(`${target.getSeconds()}`, 2)
    return `${month}/${day} ${hour}:${min}:${sec}`
}

//hh:mm:ss
export function getFullTimeTW(target = new Date()) {
    const hour = StringPad(`${target.getHours()}`, 2)
    const min = StringPad(`${target.getMinutes()}`, 2)
    const sec = StringPad(`${target.getSeconds()}`, 2)
    const sss = StringPad(`${target.getMilliseconds()}`, 3)
    return `${hour}:${min}:${sec}`
}

//getDays('2022', '07') = 31
//getDays(2022, 7) = 31
export const getDays = (year, month) => {
    return new Date(year, month, 0).getDate()
}

export const calcSameDay = (dateString, yearShift) => {
    const date = new Date(dateString)
    const newDate = new Date(date.getFullYear() + yearShift, date.getMonth(), date.getDate())
    let nowDay = date.getDay()
    const newDay = newDate.getDay()
    if (nowDay < newDay) {
        nowDay = nowDay + 7
    }
    const shiftDay = nowDay - newDay
    return (
        new Date(newDate.getFullYear(), newDate.getMonth(), newDate.getDate() + shiftDay)
    )
}

export const addDays = (date, addDays) => {
    const result = new Date(date)
    result.setDate(result.getDate() + addDays)
    return result
}

export const addYears = (date, addYears) => new Date(date.getFullYear() + addYears, date.getMonth(), date.getDate())
export const addMonths = (addMonths, date = new Date()) => {
    date.setMonth(date.getMonth() + addMonths)
    return date
}

export const calcShiftDays = (startDate, endDate) => Math.abs((endDate.getTime() - startDate.getTime()) / (1000 * 3600 * 24))

/* SAMPLE
  useInterval(() => {
    const ran = (Math.floor(Math.random() * 10) + 1) * 1000
    console.log(`heartbeat - ${ran} - ${new Date()}`)
    return ran // change next interval
  }, 4000)

  useInterval(() => {
    console.log(`heartbeat 2 - ${new Date()}`)
  }, 2000)
  */
export const useInterval = (callback, delay) => {
    // console.log(`useInterval => ${new Date()}`)
    if (!delay || !callback) return
    const savedCallback = useRef()
    //Remember the latest callback.
    useEffect(() => {
        savedCallback.current = callback
    }, [callback])

    const savedDelayRef = useRef()
    //Remember the latest callback.
    useEffect(() => {
        savedDelayRef.current = delay
    }, [delay])

    //Set up the interval.
    useEffect(() => {
        let intervalId
        function tick() {
            // console.log(` tick intervalId =>${intervalId}`)
            const newDelay = savedCallback.current()
            if (newDelay && newDelay < 0) {
                clearInterval(intervalId)
            }
            savedDelayRef.current = newDelay ? newDelay : savedDelayRef.current
        }
        //   if (savedDelayRef.current !== null) {
        if (savedDelayRef.current && savedDelayRef.current > 0) {
            // const id = setInterval(tick, savedDelayRef.current)
            tick()
            intervalId = setInterval(tick, savedDelayRef.current)
            return () => {
                console.log(` clearInterval intervalId =>${intervalId}`)
                clearInterval(intervalId)
            }
        }
    }, [callback, delay])
}

// ** Bootstrap Checkbox Component
export const BootstrapCheckbox = forwardRef(({ onClick, ...rest }, ref) => (
    <div className='custom-control custom-checkbox'>
        <input type='checkbox' className='custom-control-input' ref={ref} {...rest} />
        <label className='custom-control-label' onClick={onClick} />
    </div>
))

// ** MultiSelect base on ReactSelect
export const MultiSelectComponentOption = ({ data, ...props }) => {
    return (
        <components.Option {...props}>
            <div className='d-flex flex-wrap align-items-center'>
                <div>{data.label}</div>
            </div>
        </components.Option>
    )
}

export function isMobileDevice() {
    // const mobileDevice = ['Android', 'webOS', 'iPhone', 'iPad', 'iPod', 'BlackBerry', 'Windows Phone']
    const mobileDevice = ['Android', 'webOS', 'iPhone']
    const isMobileDevice = mobileDevice.some(e => navigator.userAgent.match(e))
    return isMobileDevice
}

//below regular expression not support in Mac / Iphone....   Moon @ 2021/11/26
//https://www.codeproject.com/Questions/5274806/Regex-works-in-chrome-but-breaks-in-safari-invalid 
//export const NumberCommasFormat = num => num.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ',')
// export const NumberCommasFormat = num => num.toLocaleString()
export const NumberCommasFormat = num => {
    return (num === undefined || num === null) ? '' : (typeof num === 'string') ? num : (Math.round(num * 100) / 100).toLocaleString()
}

export const NumberCommasFormatZeroNotShow = num => {
    return (num === undefined || num === null || num === 0) ? '' : Number(num).toLocaleString()
}

export const NumberWanDallorFormat = num => {
    let result = ''
    if (num > 9999) {
        result = `${NumberCommasFormat((num / 10000).toFixed(2))}萬`
    } else {
        result = NumberCommasFormat(num)
    }
    return result
}

export function NumberWanDallorFixedFormat(num, fixed = 2) {
    return `${NumberCommasFormat((num / 10000).toFixed(fixed))}`
}

export function arrayFilterByValue(array, string) {
    return array.filter(o => Object.keys(o).some(k => o[k] !== null && o[k].toString().toLowerCase().includes(string.toString().toLowerCase())))
}

export const NumberZeroNotShow = m => {
    const pinned = m.node?.rowPinned
    const num = m.value
    return ((!pinned || pinned === 'bottom') && (num === undefined || num === null || num === 0)) ? '' : num
}

// for ag-grid valueFormatter
export const agGrid_autoSizeAllColumns = (gridColumnApi, skipHeader) => {
    const allColumnIds = []
    if (!gridColumnApi || !gridColumnApi.getAllColumns()) return

    gridColumnApi.getAllColumns().forEach(function (column) {
        allColumnIds.push(column.colId)
    })
    gridColumnApi.autoSizeColumns(allColumnIds, skipHeader)
}

export function clearEscapeSpecialChars(jsonString) {
    return jsonString.replace(/\n/g, "")
        .replace(/\r/g, "")
        .replace(/\t/g, "")
        .replace(/\f/g, "")
}

export const agGrid_valueFormatterFunction = m => NumberCommasFormat(m.value)
export const agGrid_valueFormatterFunctionZeroNotShowCommas = m => NumberCommasFormatZeroNotShow(m.value)
export const agGrid_valueFormatterFunctionZeroNotShow = m => NumberZeroNotShow(m)
export const agGrid_valueFormatterWanDallorFunction = m => NumberWanDallorFixedFormat(m.value)
export const agGrid_valueFormatterPercentageFunctionZeroNotShow = m => {
    const num = NumberCommasFormat(m.value)
    return num === '' ? '' : `${num}%`
}

export const getAgGridColumnsFormat = (columnsString, isShowWanDallor) => {
    isShowWanDallor = (isShowWanDallor === undefined) ? false : isShowWanDallor

    const objectList = JSON.parse(clearEscapeSpecialChars(columnsString))
        .map(obj => {
            if (Object.keys(obj).find(key => key === "valueFormatter")) {
                if (obj.valueFormatter.toLowerCase() === "wandallor" && isShowWanDallor) {
                    obj.valueFormatter = agGrid_valueFormatterWanDallorFunction
                    obj.headerName = `${obj.headerName}(萬)`
                } else if (obj.valueFormatter.toLowerCase() === "percentage") {
                    obj.valueFormatter = agGrid_valueFormatterPercentageFunction
                } else {
                    obj.valueFormatter = agGrid_valueFormatterFunction
                }
            }

            return obj
        })
    return objectList
}

export const HotKeyController = (event, lastInputEnterEvent, inputDefList, hotkeyDefList, toGridDef = null, toGridEvent = null) => {
    // console.log('CityAppHelper.HotKeyController=', event.key, lastInputEnterEvent, inputDefList, hotkeyDefList)
    //hotkeys
    if (hotkeyDefList) {
        const hotkey = hotkeyDefList.find((e) => e.key === event.key)
        if (hotkey) {
            // console.log(`按下${event.key}`)
            event.preventDefault()
            if (hotkey.callback) {
                hotkey.callback()
            }
            return
        }
    }

    //enter, Arrowup down
    if (event.target.nodeName === "INPUT" || event.target.nodeName === "BUTTON") {
        const form = event.target.form

        if (form === undefined || form === null) return
        const inputList = inputDefList ? inputDefList : Array.from(form.elements).filter(t => !t.disabled && t.style.display !== 'none' && t.type !== 'file' && t.offsetParent && t.id !== '')

        //const focusIndex = inputDefList ? inputDefList.findIndex((e) => e.id === event.target.id) : Array.prototype.indexOf.call(form, event.target)
        const focusIndex = inputList.findIndex((e) => e.id === event.target.id)
        if (focusIndex < 0) return
        let focusIndexNew = focusIndex

        switch (event.key) {
            case 'Enter':
                if (toGridDef && toGridEvent) {
                    if (inputList[focusIndexNew].id === toGridDef.id) {
                        toGridEvent()
                        return
                    }
                }
                if (!inputList[focusIndexNew + 1]) {
                    event.preventDefault()
                    console.log(`最終Enter Key`)
                    lastInputEnterEvent()
                    return
                }
            case 'ArrowDown':
                if (!inputList[focusIndexNew + 1]) {
                    event.preventDefault()
                    console.log(`最終ArrowDown`)
                    return
                }
                // case 'ArrowRight':
                focusIndexNew++
                break
            case 'ArrowUp':
                // case 'ArrowLeft':
                focusIndexNew = focusIndexNew > 0 ? focusIndexNew - 1 : 0
                if ((focusIndex + focusIndexNew) === 0) {
                    event.preventDefault()
                    console.log(`最上ArrowDown`)
                    return
                }
                break
            default:
                break
        }

        if (focusIndex !== focusIndexNew) {
            const element = form.elements.namedItem(inputList[focusIndexNew].id)
            element.type === 'number' ? element.select() : element.focus()
            event.preventDefault()
        }
    }
}

export const getLikeFromToCondition = (from, to, fieldName) => {

    const result = new Object()

    if (typeof from === 'number' && typeof to === 'number') {
        result[`${fieldName}_From`] = from < to ? from : to
        result[`${fieldName}_To`] = from < to ? to : from
    } else {
        if (from.length && to.length) {
            result[`${fieldName}_From`] = from < to ? from : to
            result[`${fieldName}_To`] = from < to ? to : from
        } else if (from.length + to.length) {
            result[`${fieldName}_Like`] = from || to
        }
    }

    return result
}

export const toggleFullScreen = (forceFullScreen) => {
    if (forceFullScreen) {
        document.documentElement.requestFullscreen()
        return
    }

    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen()
    } else if (document.exitFullscreen) {
        document.exitFullscreen()
    }
}

// export const toggleScreenZoomIn = (zoomIn) => {
//     const currentZoom = document.body.style.zoom ? document.body.style.zoom.replaceAll('%', '') * 1 : 100
//     let newZoom = zoomIn ? currentZoom + 5 : currentZoom - 5
//     if (newZoom < 75) newZoom = 75
//     if (newZoom > 125) newZoom = 125
//     document.body.style.zoom = `${newZoom}%`
// }

export const toggleScreenZoomIn = (zoomIn) => {
    browser.tabs.setZoom(0.5)
}

// **計算欄位名稱應有的高度
export const headerHeightGetter = () => {
    const columnHeaderTexts = [...document.querySelectorAll('.ag-header-cell-text')]
    const clientHeights = columnHeaderTexts.map(
        headerText => headerText.clientHeight
    )
    const tallestHeaderTextHeight = Math.max(...clientHeights)

    return tallestHeaderTextHeight
}

// ** 設定欄位名稱的高度
export const headerHeightSetter = (...ref) => {
    const padding = 10
    const height = headerHeightGetter() + padding
    ref.forEach(gridRef => {
        gridRef.current.api.setHeaderHeight(height)
        gridRef.current.api.resetRowHeights()
    })
}

//yyyy-mm-dd to yyyymmdd
export const convertDateBack = (date) => {
    const lst = date.split('-')
    let newDate = ''
    lst.forEach(d => { newDate += d })
    return newDate
}

//取到小數後兩位
export const agGridFloatFormatter = (params) => {
    const { value } = params
    if (value === null) return null
    return Math.round(value * 100) / 100
}


const sheetFormat_Common = (sheet) => {
    sheet.eachRow({ includeEmpty: true }, (row, rowNumber) => {
        row.eachCell({ includeEmpty: true }, (cell, colNumber) => {
            cell.font = {
                name: 'Calibri',
                bold: false,
                size: 10
            }
            cell.alignment = {
                vertical: 'top', horizontal: 'left', wrapText: true
            }

            const BORDER_STYLE = 'thin'
            cell.border = {
                top: { style: BORDER_STYLE },
                left: { style: BORDER_STYLE },
                bottom: { style: BORDER_STYLE },
                right: { style: BORDER_STYLE }
            }
            if (rowNumber === 1) {
                cell.font.bold = true
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'fff6f4d2' }
                }
            }
            if (cell.value === null || cell.value === 0) cell.value = ''
        })
    })
}


export const exportToExcel = (sheets, fileName, columnDefList, additionInfo = null) => {
    const workbook = new Excel.Workbook()
    sheets?.forEach(({ sheetData, sheetName, showGridLines }) => {
        const rowData = sheetData.map(row => {
            const obj = {}

            columnDefList?.forEach(col => {
                obj[col.key] = col.cell(row)
            })
            return obj
        })

        const sheet = workbook.addWorksheet(sheetName, { views: [{ state: 'frozen', ySplit: 1, showGridLines: showGridLines || false }] })

        sheet.columns = columnDefList.map(col => {
            return { ...col, width: col?.excelWidth || 10 }
        })

        sheet.headerFooter.differentFirst = true
        sheet.headerFooter.firstHeader = sheetName

        sheet.addRows(rowData)
        const additionRowData = additionInfo?.additionRowData
        if (additionRowData) {
            additionRowData?.top && sheet.insertRows(1, additionRowData.top)
            additionRowData?.end && sheet.addRows(additionRowData.end)
        }

        additionInfo?.beforeStyle?.(sheet)
        additionInfo?.style ? additionInfo.style(sheet) : sheetFormat_Common(sheet)
        additionInfo?.afterStyle?.(sheet)
    })

    if (workbook.worksheets?.length > 0) {
        workbook.xlsx.writeBuffer()
            .then(buffer => saveAs(new Blob([buffer]), `${fileName}.xlsx`))
            .catch(err => console.log('Error writing excel export', err))
    }
}

export const exportToTxt = (sheets, fileName, columnDefList, formatData) => {
    const result = []
    sheets?.forEach(({ sheetData }) => {
        sheetData.forEach((sheet, index) => {
            const sortData = []
            sheet.forEach((data, index) => {
                const sortSheet = {
                    position: index,
                    sortData: data
                }
                formatData.forEach((format, formatIndex) => {
                    if (formatIndex === sortSheet.position) {
                        const value = sortSheet.sortData
                        const len = value ? value.toString().length : ''.length
                        const maxLen = format.columnLength
                        if (typeof value !== "number") {
                            if (len <= maxLen) {
                                const spaceLen = maxLen - len
                                const newValue = value + " ".repeat(spaceLen)
                                sortData.push(newValue)
                            } else {
                                const newValue = value.slice(0, maxLen)
                                sortData.push(newValue)
                            }
                        } else {
                            const absValue = Math.abs(value).toString()
                            if (len <= maxLen) {
                                const spaceLen = maxLen - len
                                const newValue = "0".repeat(spaceLen) + absValue
                                sortData.push(newValue)
                            } else {
                                const newValue = absValue.slice(0, maxLen)
                                sortData.push(newValue)
                            }
                        }
                    }
                })
            })
            result.push(sortData.join(''))
        })
    })
    const resultText = result.join('\n')
    const blob = new Blob([resultText], { type: 'text/plain;charset=utf-8' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `${fileName}.txt`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(url)
}

const chars =
    [
        'A', 'B', 'C', 'D', 'E',
        'F', 'G', 'H', 'I', 'J',
        'K', 'L', 'M', 'N', 'O',
        'P', 'Q', 'R', 'S', 'T',
        'U', 'V', 'W', 'X', 'Y', 'Z'
    ]

export const numberToExcelHeader = (index) => {
    index -= 1
    const quotient = Math.floor(index / 26)
    if (quotient > 0) {
        return numberToExcelHeader(quotient) + chars[index % 26]
    }

    return chars[index % 26]
}

export const generatePinnedBottomData = (gridApi, columnApi, aggColumn, titleColumn) => {
    const result = {}
    gridApi?.getAllGridColumns()?.forEach(item => { result[item.colId] = null })
    aggColumn?.forEach(element => {
        result[element] = 0
        gridApi?.forEachNodeAfterFilter((rowNode) => {
            result[element] += Number(rowNode.data[element])
        })
    })
    result[titleColumn] = '合計'
    return result
}

export const getAgGridRowData = (api) => {
    const rowData = []
    api.forEachNode(rowNode => rowData.push(rowNode.data))
    return rowData
}

export const checkInputValidity = ({ dataSet, target, comparator }) => {
    const data = dataSet.find(el => {
        let valid = true
        for (const cp in comparator) {
            if (el?.[cp] !== target?.[cp]) {
                valid = false
                break
            }
        }
        return valid
    })
    return data
}

export const paginateArray = (array, perPage, page) => array.slice((page - 1) * perPage, page * perPage)

export const getGridSizeHeader = ({ num, prefix = 'size', pad = 2, singleSize, prop = {} }) => {
    const header = []
    for (let i = 1; i <= num; i++) {
        header.push({
            headerName: i === 1 && singleSize ? '數量' : String(i).padStart(pad, '0'),
            flex: 40,
            field: `${prefix}${String(i).padStart(pad, '0')}`,
            hide: singleSize && i > 1,
            ...prop
        })
    }
    return header
}

export const getGridKeysHeader = ({ num, prefix = 'key', pad = 2, prop = {} }) => {
    const header = []
    for (let i = 1; i <= num; i++) {
        header.push({
            headerName: `${prefix}${String(i).padStart(pad, '0')}`,
            flex: 40,
            field: `${prefix}${String(i).padStart(pad, '0')}`,
            ...prop
        })
    }
    return header
}

export const getExcelSizeHeader = ({ num, prefix = 'size', pad = 2 }) => {
    const header = []
    for (let i = 1; i <= num; i++) {
        header.push({
            header: String(i).padStart(pad, '0'),
            key: `${prefix}${String(i).padStart(pad, '0')}`,
            cell: row => row[`${prefix}${String(i).padStart(pad, '0')}`]
        })
    }
    return header
}

export const getNumericColumn = (className = '') => {
    return (
        {
            headerClass: `ag-right-aligned-header ${className}`,
            cellClass: `ag-right-aligned-cell ${className}`
        }
    )
}

export const renderGoodName = (row) => {

    if (!row.picturePath && !row.goodName) return null

    const stateNum = Math.floor(Math.random() * 6),
        states = ['light-success', 'light-danger', 'light-warning', 'light-info', 'light-primary', 'light-secondary'],
        color = states[stateNum]

    if (row?.picturePath?.length) {
        return <Avatar className='me-1 rounded overflow-hidden' img={row.picturePath} imgClassName='rounded' enablePreview={true} />
    } else {
        return <Avatar color={color || 'primary'} className='me-1 rounded overflow-hidden' content={row.goodName} initials />
    }
}

export const getDateTimeLocal = (dateTime = new Date()) => {
    dateTime.setMinutes(dateTime.getMinutes() - dateTime.getTimezoneOffset())
    return dateTime.toISOString().slice(0, 16)
}

export const decryptAES = (encryptedText, key) => {
    encryptedText = encryptedText || ''

    // 轉換密鑰為 WordArray
    const keyWordArray = CryptoJS.enc.Utf8.parse(key)
    // 轉換加密文字為 WordArray
    const encryptedWordArray = CryptoJS.enc.Base64.parse(encryptedText)

    const decrypted = CryptoJS.AES.decrypt(
        { ciphertext: encryptedWordArray },
        keyWordArray,
        {
            mode: CryptoJS.mode.ECB, // 使用 ECB 模式，請根據實際情況選擇模式
            padding: CryptoJS.pad.Pkcs7
        }
    )

    return CryptoJS.enc.Utf8.stringify(decrypted)
}

export const downloadFile = (response) => {

    const base64String = response.fileContents
    const contentType = response.contentType
    const fileName = response.fileDownloadName

    const byteCharacters = atob(base64String)
    const byteNumbers = new Array(byteCharacters.length)
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i)
    }
    const byteArray = new Uint8Array(byteNumbers)

    const blob = new Blob([byteArray], { type: contentType })

    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', fileName)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
}

export const confirmDelete = async (deleteAction) => {
    const result = await Swal.fire({
        title: '是否確定刪除?',
        showCancelButton: true,
        confirmButtonText: '確定',
        cancelButtonText: '離開',
        customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-flat-secondary ms-1'
        },
        buttonsStyling: false
    })

    if (result.isConfirmed) {
        await deleteAction()
    }
}

export const parseValueLabelObject = (obj) => {
    if (Array.isArray(obj)) {
        return obj.map(item => parseValueLabelObject(item))
    }

    if (!obj || typeof obj !== 'object') {
        return obj
    }

    if ('value' in obj) {
        return obj.value
    }

    const result = {}
    Object.keys(obj).forEach(key => {
        result[key] = parseValueLabelObject(obj[key])
    })

    return result
}
