// ** React Imports
import { Link } from 'react-router-dom'

// ** Custom Components
import Avatar from '@components/avatar'

// ** Store & Actions
import { store } from '@store/store'
import { deleteEmpOnDuty, editEmpOnDuty, updateChangeFlag } from '../store'
import { getFormatedMonthForInput, addMonths, showMessageBox, arrayFilterByValue, HotKeyController } from '@CityAppHelper'
import { getUserData } from '@utils'

// ** Icons Imports
import { Slack, User, Settings, Database, Edit2, MoreVertical, FileText, Trash2, Archive } from 'react-feather'

// ** Reactstrap Imports
import { Badge, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap'
import { Fragment } from 'react'

// ** Renders Client Columns
const renderClient = row => {
    const stateNum = Math.floor(Math.random() * 6),
        states = ['light-success', 'light-danger', 'light-warning', 'light-info', 'light-primary', 'light-secondary'],
        color = states[stateNum]

    return <Avatar color={color || 'primary'} className='me-1' content={row.empName} initials />

}

// ** Renders Role Columns
const renderRole = row => {
    const group = row.groupName
    const roleObj = {
        admin: {
            color: 'light-primary'
        }
    }
    return (
        <span className='text-truncate text-capitalize align-middle'>
            {
                group.map(_r => {
                    const _role = _r.value?.toLowerCase()
                    const Icon = roleObj[_role] ? roleObj[_role].icon : Edit2
                    return (
                        <Fragment key={_role}>
                            <Badge color={`${roleObj[_role] ? roleObj[_role].color : 'light-success'} me-50`} className=' ms-auto me-50'>{_role}</Badge>
                        </Fragment>
                    )
                })
            }
        </span>
    )
}

const weekList = ['日', '一', '二', '三', '四', '五', '六']

const getDayList = (yearMonth, empDutyTypeList, isShowDutyName) => {
    const dateString = (yearMonth || getFormatedMonthForInput()).replace('-', '')
    const year = parseInt(dateString.substr(0, 4), 10)
    const month = parseInt(dateString.substr(4, 2), 10)
    const formatedDateString = `${year}-${month.toString().padStart(2, '0')}-`
    const len = new Date(year, month, 0).getDate()
    const list = []

    for (let i = 1; i <= len; i++) {
        list.push({
            width: 50,
            headerName: i.toString().padStart(2, '0'),
            headerComponentParams: (params) => {
                const weekDay = new Date(formatedDateString + params.displayName).getDay()
                const spanStyle = (weekDay === 0 || weekDay === 6) ? ';color:red' : ''

                return {
                    template:
                        '<div>' +
                        `<span style="text-align: center; display:block; font-size:100%${spanStyle}">${params.displayName}</span>` +
                        `<span style="text-align: center; display:block; font-size:100%${spanStyle}">${weekList[weekDay]}</span>` +
                        '</div>'
                }
            },
            valueFormatter: (params) => {
                if (isShowDutyName === '1') {
                    const DutyName = empDutyTypeList.find(item => item.dutyID === params.value)?.dutyName
                    return DutyName || params.value
                } else if (isShowDutyName === '0') {
                    return params.value
                } else if (isShowDutyName === '2') {
                    const DutyName = empDutyTypeList.find(item => item.dutyID === params.value)?.hours
                    return DutyName || params.value
                }
            },
            cellRenderer: (params) => {
                const currentDepID = params.data[params.column.colId?.replace('day', 'depID')]
                const spanStyle = { textAlign: 'center', display: 'block', fontSize: '100%' }
                if (currentDepID !== params.data.depID) {
                    spanStyle.backgroundColor = '#00aaff'
                }
                if (!empDutyTypeList.find(item => item.dutyID === params.value)?.stOnDutyTime) {
                    spanStyle.color = 'red'
                }

                return (
                    <div>
                        <span style={spanStyle}>{params.valueFormatted}</span>
                    </div>
                )
            },
            field: `day${i}`
        })
    }
    return list
}

export const ColumnDefs = ({ access, t, yearMonth, empDutyTypeInfo, isShowDutyName, handleEdit}) => {
    // const {list, hoursCount} = getDayList(yearMonth, empDutyTypeInfo, isShowDutyName)
    return [
        {
            headerName: t('shiftSchedule.empName', { ns: 'workMgmt' }),
            sortable: true,
            minWidth: 120,
            field: 'empName',
            cellRenderer: ({ data }) => (
                <div className='d-flex justify-content-left align-items-center'>
                    {/* {renderClient(data)}
                    <div className='d-flex flex-column'> */}
                    <Link
                        href='/'
                        className='user_name text-truncate text-body'
                        onClick={(e) => {
                            e.preventDefault()
                            handleEdit(data)
                        }}
                    >
                        <span className='text-primary'>{data.empName}</span>
                    </Link>
                </div>
                // </div>
            )
        },
        {
            headerName: t('shiftSchedule.empID', { ns: 'workMgmt' }),
            sortable: true,
            minWidth: 100,
            field: 'empID'
        },
        {
            headerName: t('shiftSchedule.depID', { ns: 'workMgmt' }),
            sortable: true,
            minWidth: 100,
            field: 'depID'
        },
        {
            headerName: t('shiftSchedule.hours', { ns: 'workMgmt' }),
            sortable: true,
            minWidth: 80,
            field: 'hours'
        },
        {
            headerName: t('shiftSchedule.depName', { ns: 'workMgmt' }),
            sortable: true,
            minWidth: 100,
            field: 'depName'
        },
        ...getDayList(yearMonth, empDutyTypeInfo, isShowDutyName),
        {
            headerName: t('actions', { ns: 'common' }),
            minWidth: 90,
            sortable: false,
            hide: !(access.update || access.delete),
            cellRenderer: ({ api, data }) => (
                <Link className='text-body'
                    href='/'
                    onClick={e => {
                        const rowCount = api.getDisplayedRowCount()
                        e.preventDefault()
                        store.dispatch(deleteEmpOnDuty({ yearMonth: data.yearMonth, item: data.item, modifier: getUserData().userId }))
                        store.dispatch(updateChangeFlag())
                    }}>
                    <Trash2 size={18} className='me-50' />
                </Link>
            )
        }
    ]
}

export const ColumnDefsAdd = ({ access, t, yearMonth, empDutyTypeInfo, isShowDutyName, setNewData, editRow }) => ([
    {
        headerName: t('shiftSchedule.empName', { ns: 'workMgmt' }),
        sortable: true,
        width: 100,
        field: 'empName',
        cellRenderer: ({ data }) => (
            <div className='d-flex justify-content-left align-items-center'>
                <Link
                    href='/'
                    className='user_name text-truncate text-body'
                    onClick={(e) => {
                        e.preventDefault()
                        editRow(data)
                    }}
                >
                    <span className='fw-bolder'>{data.empName}</span>
                </Link>
            </div>
            // </div>
        )
    },
    {
        headerName: t('shiftSchedule.empID', { ns: 'workMgmt' }),
        sortable: true,
        width: 80,
        field: 'empID'
    },
    {
        headerName: t('shiftSchedule.depID', { ns: 'workMgmt' }),
        sortable: true,
        width: 80,
        field: 'depID'
    },
    {
        headerName: t('shiftSchedule.depName', { ns: 'workMgmt' }),
        sortable: true,
        width: 80,
        field: 'depName'
    },
    ...getDayList(yearMonth, empDutyTypeInfo, isShowDutyName),
    {
        headerName: t('actions', { ns: 'common' }),
        minWidth: 90,
        sortable: false,
        hide: !(access.update || access.delete),
        cellRenderer: ({ api, data }) => (
            <Link className='text-body'
                href='/'
                onClick={e => {
                    e.preventDefault()
                    setNewData(prev => prev.filter(row => row.empID !== data.empID))
                }}>
                <Trash2 size={18} className='me-50' />
            </Link>
        )
    }
])

export const ColumnExport = ({ t }) => {
    const baseCols = [
        { header: t('shiftSchedule.empName', { ns: 'workMgmt' }), key: 'empName', cell: row => row.empName },
        { header: t('shiftSchedule.empID', { ns: 'workMgmt' }), key: 'empID', cell: row => row.empID },
        { header: t('shiftSchedule.depID', { ns: 'workMgmt' }), key: 'depID', cell: row => row.depID },
        { header: t('shiftSchedule.depName', { ns: 'workMgmt' }), key: 'depName', cell: row => row.depName }
    ]

    const dayCols = []
    for (let i = 1; i <= 31; i++) {
        dayCols.push({
            header: i.toString().padStart(2, '0'),
            key: `day${i}`,
            cell: row => row[`day${i}`]
        })
    }

    return [...baseCols, ...dayCols]
}

export const ColumnFilter = ({ t }) => ([
    { label: t('shiftSchedule.empName', { ns: 'workMgmt' }), value: 'empName' },
    { label: t('shiftSchedule.empID', { ns: 'workMgmt' }), value: 'empID' },
    { label: t('shiftSchedule.depID', { ns: 'workMgmt' }), value: 'depID' },
    { label: t('shiftSchedule.depName', { ns: 'workMgmt' }), value: 'depName' }
])