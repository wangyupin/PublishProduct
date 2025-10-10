import { Fragment, useMemo, useContext } from "react"
import EcommerceStoreList from './List'
import SingleEcommerceStore from './Single'
import Setting from "./Setting"

import { Row, Col, Button, Card, CardBody, CardHeader, CardText, CardTitle, Input, Badge, Modal, ModalHeader, ModalBody, ModalFooter, Form, Label, InputGroup, InputGroupText } from 'reactstrap'
import { useParams, Link } from 'react-router-dom'
import { AbilityContext } from '@src/utility/context/Can'
import { useTranslation } from 'react-i18next'


const Index = () => {
    const { mode } = useParams()
    const ability = useContext(AbilityContext)
    const access = useMemo(() => {
        return ({
            create: ability.can('create', '電商門店增修'),
            delete: ability.can('delete', '電商門店增修'),
            update: ability.can('update', '電商門店增修')
            // export: ability.can('export', '電商門店增修')
        })
    }, [ability])
    const { t } = useTranslation(['ecommerceMgmt', 'common'])
    return (
        <Fragment>
            {mode === 'List' || mode === 'Single' ? (
                <Row sm='12' className='h-auto'>
                    <Col sm='12' className='d-flex justify-content-between w-100'>
                        {mode === 'List' ? <EcommerceStoreList access={access} t={t} /> : <SingleEcommerceStore access={access} t={t} />}
                    </Col>
                </Row>
            ) : (
                <Setting access={access} t={t} />
            )}
        </Fragment>
    )
}

export default Index
