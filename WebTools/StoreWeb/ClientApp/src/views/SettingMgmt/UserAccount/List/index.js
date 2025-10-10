// ** User List Component
import Table from './Table'

// ** Styles
import '@styles/react/apps/app-users.scss'

const UsersList = ({ access, t }) => {

    return (
        <div>
            <Table access={access} t={t} />
        </div>
    )
}

export default UsersList
