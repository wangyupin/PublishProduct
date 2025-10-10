// ** User List Component
import Table from './Table'

// ** Styles
import '@styles/react/apps/app-users.scss'

const UsersList = ({ access, t, setProgress, isFront }) => {

    return (
        <div>
            <Table access={access} t={t} setProgress={setProgress} isFront={isFront} />
        </div>
    )
}

export default UsersList
