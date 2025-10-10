
import { Fragment, useContext, useMemo, useEffect } from "react"
import { useDispatch } from "react-redux"
import UsersList from './List'
import SingleUser from './Single'
import { resetContent } from './store'
import usePopstate from '@hooks/usePopstate'
import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'

const Index = () => {
    const dispatch = useDispatch()
    const { mode } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '使用者帳號管理'),
            delete: ability.can('delete', '使用者帳號管理'),
            update: ability.can('update', '使用者帳號管理'),
            export: ability.can('export', '使用者帳號管理')
        })
    }, [ability])

    const { t } = useTranslation(['settingMgmt', 'common'])

    usePopstate({
        dispatchAction: resetContent({ key: 'single' })
    })

    return (
        <Fragment>
            {mode === 'List' ? <UsersList access={access} t={t} /> : <SingleUser access={access} t={t} />}
        </Fragment>
    )
}

export default Index
