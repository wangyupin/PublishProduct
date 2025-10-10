// ** React Imports
import { Link } from 'react-router-dom'

// ** Custom Components
import Avatar from '@components/avatar'

// ** Store & Actions
import { store } from '@store/store'
import { deleteUser, editUser } from '../store'

// ** Icons Imports
import { Slack, User, Settings, Database, Edit2, MoreVertical, FileText, Trash2, Archive } from 'react-feather'

// ** Reactstrap Imports
import { Badge, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap'
import { Fragment } from 'react'
import { confirmDelete } from '@CityAppHelper'

// ** Renders Client Columns
const renderClient = row => {
    const stateNum = Math.floor(Math.random() * 6),
        states = ['light-success', 'light-danger', 'light-warning', 'light-info', 'light-primary', 'light-secondary'],
        color = states[stateNum]

    if (row.avatar?.length) {
        return <Avatar className='me-1' img={row.avatar} width='32' height='32' />
    } else {
        return <Avatar color={color || 'primary'} className='me-1' content={row.userName} initials />
    }
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

export const ColumnDefs = ({ access, t, setCurrentPage }) => ([
    {
        headerName: t('userAccount.userName', { ns: 'settingMgmt' }),
        sortable: true,
        flex: 300,
        minWidth: 300,
        field: 'userName',
        cellRenderer: ({ data }) => (
            <div className='d-flex justify-content-left align-items-center'>
                {renderClient(data)}
                <div className='d-flex flex-column'>
                    <Link
                        to={`../SettingMgmt/UserAccount/Single/${data.userID}`}
                        className='user_name text-truncate text-body'
                        onClick={() => store.dispatch(editUser({ userID: data.userID }))}
                    >
                        <span className='text-primary'>{data.userName}</span>
                    </Link>
                    <small className='text-truncate text-muted mb-0'>{data.email}</small>
                </div>
            </div>
        ),
        headerCheckboxSelection: true,
        checkboxSelection: true,
        showDisabledCheckboxes: true
    },
    {
        headerName: t('userAccount.userID', { ns: 'settingMgmt' }),
        sortable: true,
        flex: 50,
        field: 'userID'
    },
    {
        headerName: t('userAccount.userNumber', { ns: 'settingMgmt' }),
        sortable: true,
        flex: 70,
        field: 'userNumber',
        valueFormatter: ({ data }) => data?.userNumber?.value
    },
    {
        headerName: t('userAccount.groupName', { ns: 'settingMgmt' }),
        sortable: true,
        flex: 180,
        field: 'groupName',
        cellDataType: false,
        cellRenderer: ({ data }) => renderRole(data)
    },
    {
        headerName: t('userAccount.description', { ns: 'settingMgmt' }),
        flex: 120,
        sortable: false,
        field: 'description'
    },
    {
        headerName: t('actions', { ns: 'common' }),
        flex: 70,
        sortable: false,
        cellRenderer: ({ api, data }) => (
            <div>
                {access.delete && (
                    <Link className='text-body'
                        href='/'
                        onClick={e => {
                            e.preventDefault()
                            confirmDelete(() => {
                                const rowCount = api.getDisplayedRowCount()
                                store.dispatch(deleteUser({ delList: [{ userID: data.userID }] }))
                                    .then(res => !res.error && rowCount === 1 && setCurrentPage(prev => (prev - 1 > 0 ? prev - 1 : 1)))
                            })
                        }}>
                        <Trash2 size={18} className='me-50' />
                    </Link>
                )}
            </div>
        )
    }
])

export const ColumnExport = ({ t }) => ([
    { header: t('userAccount.userName', { ns: 'settingMgmt' }), key: 'userName', cell: row => row.userName },
    { header: t('userAccount.userID', { ns: 'settingMgmt' }), key: 'userID', cell: row => row.userID },
    { header: t('userAccount.userNumber', { ns: 'settingMgmt' }), key: 'userNumber', cell: row => row.userNumber.value },
    { header: t('userAccount.groupName', { ns: 'settingMgmt' }), key: 'groupName', cell: row => row.groupName.map(obj => obj.value)?.join(', ') },
    { header: t('userAccount.description', { ns: 'settingMgmt' }), key: 'description', cell: row => row.description },
    { header: t('userAccount.email', { ns: 'settingMgmt' }), key: 'email', cell: row => row.email }

])

export const ColumnFilter = ({ t }) => ([
    { label: t('userAccount.userName', { ns: 'settingMgmt' }), value: 'userName' },
    { label: t('userAccount.userID', { ns: 'settingMgmt' }), value: 'userID' },
    { label: t('userAccount.userNumber', { ns: 'settingMgmt' }), value: 'userNumber' },
    { label: t('userAccount.groupName', { ns: 'settingMgmt' }), value: 'groupName' },
    { label: t('userAccount.description', { ns: 'settingMgmt' }), value: 'description' }
])
