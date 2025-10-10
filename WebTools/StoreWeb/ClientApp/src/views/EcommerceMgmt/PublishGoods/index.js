
import { Fragment, useContext, useMemo, useEffect } from "react"
import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'

import Single from "./Single"
import List from "./List"

const Index = () => {
    const { mode, id } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '平台商品管理'),
            delete: ability.can('delete', '平台商品管理'),
            update: ability.can('update', '平台商品管理')
        })
    }, [ability])

    const { t } = useTranslation(['ecommerceMgmt', 'common'])

    return (
        <Fragment>
            {mode === 'List' ? <List access={access} t={t} /> : <Single access={access} t={t} mode={mode} id={id} />}
        </Fragment>
    )
}

export default Index
