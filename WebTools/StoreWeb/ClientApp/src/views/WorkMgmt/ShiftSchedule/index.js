
import { Fragment, useContext, useMemo, useEffect } from "react"
import UsersList from './List'
import SingleUser from './Single'
import { getCommonList, getEmpOptions, getClientOptionsAll } from './store'
import { useSelector, useDispatch } from 'react-redux'
import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'

const Index = ({setProgress}) => {
    const { mode } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '排班資料增修'),
            delete: ability.can('delete', '排班資料增修'),
            update: ability.can('update', '排班資料增修'),
            export: ability.can('export', '排班資料增修')
        })
    }, [ability])

    const { t } = useTranslation(['workMgmt', 'common'])
    const dispatch = useDispatch()

    useEffect(() => {
        dispatch(getCommonList())
        dispatch(getEmpOptions())
        dispatch(getClientOptionsAll())
        // dispatch(toggleCurrentMode('add')) //For dev
      }, [])

    return (
        <Fragment>
            {mode === 'List' ? <UsersList access={access} t={t} setProgress={setProgress}/> : <SingleUser access={access} t={t} />}
        </Fragment>
    )
}

export default Index