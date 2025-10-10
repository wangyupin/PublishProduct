
import { Fragment, useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import EcommerceStoreContent from './EcommerceStoreContent'

const SingleEcommerceStore = ({ t }) => {
    
    const { id } = useParams()
    return (

        <Fragment>
            <EcommerceStoreContent id={id} t={t} />
        </Fragment>
    )
}
export default SingleEcommerceStore