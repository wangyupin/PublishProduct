// ** User List Component
import Table from './Table'

// ** Styles
import '@styles/react/apps/app-users.scss'

import { useSelector } from 'react-redux'
import UILoader from '@components/ui-loader'

const EmpDutyList = ({ access, t }) => {
    const store = useSelector(state => state.WorkMgmt_EmpDutyTypeSetUp)

    return (
        <UILoader blocking={store.isLoading} classname='cityapp full-screen-uiloader'>
            <Table access={access} t={t} />
        </UILoader>
    )
}

export default EmpDutyList
