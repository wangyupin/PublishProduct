
import { Fragment, useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import EmpDutyContent from './EmpDutyContent'

const SingleEmpDuty = ({ t, access }) => {
    const { id } = useParams()
    return (
        <Fragment>
            <EmpDutyContent id={id} t={t} access={access} />
        </Fragment>
    )
}
export default SingleEmpDuty