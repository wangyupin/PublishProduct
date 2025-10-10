// ** React Imports
import { Fragment, useState, useCallback, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link } from 'react-router-dom'

// ** Custom Components
import Avatar from '@components/avatar'

// ** Store & Actions
import { store } from '@store/store'

import { updEmpDutyData, delEmpDutyData, getEmpDutyList, editEmpDuty, changeFlag } from '../store'

// ** Icons Imports
import { Slack, User, Settings, Database, Edit2, MoreVertical, FileText, Trash2, Archive, MoreHorizontal, Edit, Trash, DollarSign, Clock, Gift } from 'react-feather'

// ** Reactstrap Imports
import { Badge, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap'

// i18n
import { useTranslation } from 'react-i18next'

import { convertDateStringForInput, NumberCommasFormat, showMessageBox, confirmDelete } from '@CityAppHelper'

const dispatch = store.dispatch

// const valueSetter = async (params) => {
//     const { data, colDef, newValue } = params
//     const fieldName = colDef.field
//     const updateData = { 
//         ...data,
//         [fieldName]:newValue
//     }
//     dispatch(updEmpDutyData({
//         ...updateData,
//         changeDate:'',
//         changePerson:''
//     })).then(() => {
//         return dispatch(getEmpDutyList())
//     })
// }

export const ColumnDefs = ({ access, t, setCurrentPage, setChangFlag }) => ([
    {
        headerName: t('employeeDuty.dutyID', { ns: 'workMgmt' }),
        sortable: false,
        field: 'dutyID',
        flex:1,
        editable: false,
        cellRenderer:({data}) => (
            <div className='d-flex justify-content-left align-items-center'>
                <div className='d-flex flex-column'>
                    <Link
                        to={`../WorkMgmt/EmpDutyTypeSetUp/Single/${data.dutyID}`}
                        className='text-body'
                        onClick={() => store.dispatch(editEmpDuty(data))}
                    >
                        <span className='fw-bolder ms-1 link-primary'>{data.dutyID}</span>
                    </Link>
                </div>
            </div>
        ),
        headerCheckboxSelection: true,
        checkboxSelection: true,
        showDisabledCheckboxes: true
    },
    {
        headerName: t('employeeDuty.dutyName', { ns: 'workMgmt' }),
        headerClass: 'center-dutyHeader',
        sortable: false,
        field: 'dutyName',
        flex:1,
        cellStyle: { justifyContent: 'center' }
        // editable: true
        // valueSetter
    },
    {
        headerName: t('employeeDuty.onDuty', { ns: 'workMgmt' }),
        headerClass: 'center-dutyHeader',
        sortable: false,
        field: 'stOnDutyTime',
        flex:1,
        cellStyle: { justifyContent: 'center' }
        // editable: true
        // valueSetter
    },
    {
        headerName: t('employeeDuty.offDuty', { ns: 'workMgmt' }),
        headerClass: 'center-dutyHeader',
        sortable: false,
        field: 'stOffDutyTime',
        flex:1,
        cellStyle: { justifyContent: 'center' }
        // editable: true
        // valueSetter
    },
    {
        headerName: t('employeeDuty.hours', { ns: 'workMgmt' }),
        headerClass: 'center-dutyHeader',
        sortable: false,
        field: 'hours',
        flex:1,
        cellStyle: { justifyContent: 'center' }
    },
    {
        headerName: t('actions', { ns: 'common' }),
        sortable: false,
        flex:1,
        hide: !(access.update || access.delete),
        cellStyle: { position : 'relative' },
        cellRenderer: ({ api, data }) => (
            <div className='d-flex flex-row align-items-center member-action'>
                <Link className='text-body me-1'
                    href='/'
                    onClick={() => {
                        confirmDelete(() => dispatch(delEmpDutyData({ DutyID: data.dutyID }))
                            .then(() => setChangFlag(true)))
                    }}
                >
                    <Trash2 size={18} />
                </Link>
            </div>
        )
    }
])

export const ColumnExport = ({ t }) => ([
    { header: t('employeeDuty.dutyID', { ns: 'workMgmt' }), key: 'dutyID', cell: row => row.dutyID },
    { header: t('employeeDuty.dutyName', { ns: 'workMgmt' }), key: 'dutyName', cell: row => row.dutyName },
    { header: t('employeeDuty.onDuty', { ns: 'workMgmt' }), key: 'onDuty', cell: row => row.stOnDutyTime },
    { header: t('employeeDuty.offDuty', { ns: 'workMgmt' }), key: 'offDuty', cell: row => row.stOffDutyTime },
    { header: t('employeeDuty.hours', { ns: 'workMgmt' }), key: 'hours', cell: row => row.hours }
])

export const ColumnFilter = ({ t }) => ([
    { label: t('employeeDuty.dutyID', { ns: 'workMgmt' }), value : 'dutyID' },
    { label: t('employeeDuty.dutyName', { ns: 'workMgmt' }), value : 'dutyName' },
    { label: t('employeeDuty.onDuty', { ns: 'workMgmt' }), value : 'onDuty' },
    { label: t('employeeDuty.offDuty', { ns: 'workMgmt' }), value: 'offDuty' },
    { label: t('employeeDuty.hours', { ns: 'workMgmt' }), value: 'hours' }
])