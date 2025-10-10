import { Fragment, useMemo, useContext } from "react"
import EmpDutyList from './List'
import SingleEmpDuty from './Single'

import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'


const Index = () => {
    const { mode } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '班別設定'),
            delete: ability.can('delete', '班別設定'),
            update: ability.can('update', '班別設定'),
            export: ability.can('export', '班別設定')
        })
    }, [ability])
    const {t} = useTranslation(['workMgmt', 'common'])
    return (
        <Fragment>
            {mode === 'List' ? <EmpDutyList access={access} t={t} /> : <SingleEmpDuty access={access} t={t} />}
        </Fragment>
    )
}

export default Index