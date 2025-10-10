import { Fragment, useEffect } from 'react'
import { Card, Row, Col } from 'reactstrap'
import { getMachineSet, getUserData } from '@utils'

import Features from './Features/Features'
import axios from 'axios'

import { ShowToast } from '@CityAppExtComponents/caToaster'


const Index = () => {
    const templateData = localStorage.getItem('templateData')

    return (
        <Fragment>
            <Row sm='12' className='d-flex justify-content-between' style={{ marginTop: '0px', height: 'auto' }}>
                <Col sm='12' className='d-flex flex-row justify-content-between'>
                    <Features />
                </Col>
            </Row>
        </Fragment>
    )
}

export default Index
