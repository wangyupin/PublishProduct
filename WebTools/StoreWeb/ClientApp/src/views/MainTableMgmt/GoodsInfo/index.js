
import { Fragment, useMemo, useContext } from "react"
import List from './List'

import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'


const Index = () => {
    const { mode } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            ecommerceCreate: ability.can('create', '1084')
        })
    }, [ability])

    const { t } = useTranslation(['mainTableMgmt', 'common'])

    return (
        <Fragment>
            {<List access={access} t={t} />}
        </Fragment>
    )
}

export default Index
