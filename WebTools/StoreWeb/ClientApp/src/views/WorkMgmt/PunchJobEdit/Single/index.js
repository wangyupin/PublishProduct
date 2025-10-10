
import { Fragment, useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import SingleContent from './SingleContent'

const Single = ({ t, access }) => {

    const { id } = useParams()
    return (

        <Fragment>
            <SingleContent id={id} t={t} access={access} />
        </Fragment>
    )
}
export default Single