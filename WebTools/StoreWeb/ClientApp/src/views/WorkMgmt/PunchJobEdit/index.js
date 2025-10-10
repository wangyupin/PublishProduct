
import { Fragment, useContext, useMemo, useEffect } from "react"
import PunchJobList from './List'
import Single from './Single'

import { useDispatch } from 'react-redux'
import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'
import { getMachineSet } from '@utils'
import usePopstate from '@hooks/usePopstate'
import { getEmpOptions, getClientOptions, getClientOptionsF, resetContent } from './store'

const Index = ({ setProgress, isFront }) => {
    const dispatch = useDispatch()
    const { mode } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '差勤資料增修'),
            delete: ability.can('delete', '差勤資料增修'),
            update: ability.can('update', '差勤資料增修'),
            export: ability.can('export', '差勤資料增修')
        })
    }, [ability])

    const { t } = useTranslation(['workMgmt', 'common'])

    useEffect(() => {
        dispatch(getEmpOptions())
        isFront ? dispatch(getClientOptionsF({ sellBranch: getMachineSet()?.sellBranch })) : dispatch(getClientOptions())
    }, [])

    usePopstate({
        dispatchAction: resetContent({ key: 'single' })
    })

    return (
        <Fragment>
            {mode === 'List' ? <PunchJobList access={access} t={t} setProgress={setProgress} isFront={isFront} /> : <Single access={access} t={t} />}
        </Fragment>
    )
}

export default Index
