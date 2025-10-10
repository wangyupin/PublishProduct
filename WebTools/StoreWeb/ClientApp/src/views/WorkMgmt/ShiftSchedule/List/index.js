// ** User List Component
import Table from './Table'

// ** Styles
import '@styles/react/apps/app-users.scss'

const EmpOnDutyList = ({ access, t, setProgress }) => {
    return (
        <div>
            <Table access={access} t={t} setProgress={setProgress}/>
        </div>
    )
}

export default EmpOnDutyList
