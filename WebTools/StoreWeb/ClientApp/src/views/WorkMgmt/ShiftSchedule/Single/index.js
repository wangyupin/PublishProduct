
import { Fragment, useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import UserContent from './EmpOnDutyContent'

const SingleEmpOnDuty = ({ t, access }) => {

    const { id } = useParams()
    return (

        <Fragment>
            <UserContent id={id} t={t} access={access}/>
        </Fragment>
    )
}
export default SingleEmpOnDuty