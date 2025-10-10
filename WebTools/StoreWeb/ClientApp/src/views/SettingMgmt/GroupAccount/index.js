// ** React Imports
import { Fragment, useContext, useEffect, useMemo } from 'react'
import { useDispatch } from 'react-redux'

// ** Context
import { AbilityContext } from '@src/utility/context/Can'

// ** Roles Components
import RoleCards from './RoleCards'

const Roles = ({ setProgress }) => {
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '群組權限管理'),
            delete: ability.can('delete', '群組權限管理'),
            update: ability.can('update', '群組權限管理')
        })
    }, [ability])

    return (
        <Fragment>
            <RoleCards access={access} />
        </Fragment>
    )
}

export default Roles
