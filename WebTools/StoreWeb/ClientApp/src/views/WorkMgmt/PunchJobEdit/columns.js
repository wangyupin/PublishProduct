// ** React Imports
import { Link } from 'react-router-dom'

// ** Custom Components
import { agGrid_valueFormatterFunction, getNumericColumn, confirmDelete } from '@CityAppHelper'
import { getUserData } from '@utils'

// ** Store & Actions
import { store } from '@store/store'
import { deletePunchJob, editPunchJob } from './store'

// ** Icons Imports
import { Trash2 } from 'react-feather'

export const ColumnDefs = ({ access, t }) => ([
    {
        headerName: t('employee', { ns: 'common' }),
        flex: 1,
        field: 'empID',
        cellRenderer: ({ data }) => (
            <div className='d-flex justify-content-left align-items-center'>
                <div className='d-flex flex-column'>
                    <Link
                        to={`../WorkMgmt/PunchJobEdit/Single/${data.empID}`}
                        className='user_name text-truncate text-body'
                        onClick={() => store.dispatch(editPunchJob(data))}
                    >
                        <span className='text-primary'>{`${data.empID}${data.empName ? ` | ${data.empName}` : ''}`}</span>
                    </Link>
                </div>
            </div>
        )
    },
    {
        headerName: t('punchJob.clockStore', { ns: 'workMgmt' }),
        flex: 1,
        field: 'clockStore',
        valueFormatter: ({ value, data }) => `${value}${data.clientShort ? ` | ${data.clientShort}` : ''}`
    },
    {
        headerName: t('punchJob.clockDate', { ns: 'workMgmt' }),
        flex: 1,
        field: 'clockDate'
    },
    {
        headerName: t('punchJob.clockTime', { ns: 'workMgmt' }),
        flex: 1,
        field: 'clockTime'
    },
    {
        headerName: t('punchJob.punchType', { ns: 'workMgmt' }),
        flex: 1,
        field: 'holiday'
    },
    {
        headerName: t('actions', { ns: 'common' }),
        flex: 1,
        sortable: false,
        cellRenderer: ({ api, data }) => (
            <div>
                {access.delete && (
                    <Link className='text-body'
                        href='/'
                        onClick={e => {
                            e.preventDefault()
                            confirmDelete(() => { store.dispatch(deletePunchJob({ ...data, modifier: getUserData()?.userId })) })
                        }}>
                        <Trash2 size={18} className='me-50' />
                    </Link>
                )}
            </div>
        )
    }
])

export const getColumnForAddDetail = ({ t }) => (
    [
        {
            headerName: t('goodID', { ns: 'common' }),
            field: 'goodID',
            flex: 2,
            headerCheckboxSelection: true,
            checkboxSelection: true,
            showDisabledCheckboxes: true
        },
        {
            headerName: t('goodName', { ns: 'common' }),
            field: 'goodName',
            flex: 2
        },
        {
            headerName: t('brand', { ns: 'common' }),
            field: 'brandName',
            flex: 1
        }
    ]
)

export const ColumnExport = ({ t }) => ([
    { header: t('employee', { ns: 'common' }), key: 'empID', cell: row => row.empID },
    { header: `${t('employee', { ns: 'common' })}${t('name', { ns: 'common' })}`, key: 'empName', cell: row => row.empName },
    { header: t('punchJob.clockStore', { ns: 'workMgmt' }), key: 'clockStore', cell: row => row.clockStore },
    { header: `${t('client', { ns: 'common' })}${t('name', { ns: 'common' })}`, key: 'clientShort', cell: row => row.clientShort },
    { header: t('punchJob.clockDate', { ns: 'workMgmt' }), key: 'clockDate', cell: row => row.clockDate },
    { header: t('punchJob.clockTime', { ns: 'workMgmt' }), key: 'clockTime', cell: row => row.clockTime },
    { header: t('punchJob.punchType', { ns: 'workMgmt' }), key: 'punchType', cell: row => row.holiday }
])
