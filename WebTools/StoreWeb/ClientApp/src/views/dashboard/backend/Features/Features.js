import { Fragment, useState, useEffect, useCallback, useMemo } from 'react'
import { Card, Row, Col, CardHeader, CardBody, CardText, CardTitle, CardSubtitle, Button } from 'reactstrap'
import { Navigate, Link } from 'react-router-dom'
import { getMachineSet, getUserData } from '@utils'

// Image 
import { useTranslation } from 'react-i18next'

const Features = () => {
    const { t } = useTranslation(['dashboardMgmt', 'common'])
    const abilityData = getUserData()?.ability

    const filterAbility = (buttons) => {
        const abilityArr = []
        abilityData.forEach(ability => {
            if (ability.action === 'read') {
                buttons.forEach(b => {
                    if (ability.subject.includes(`${t(`${b.title}`, { ns: 'dashboardMgmt' })}`)) {
                        abilityArr.push(b)
                    }
                })
            }
        })

        const sortArr = abilityArr.sort((firstItem, secondItem) => firstItem.id - secondItem.id)
        return sortArr
    }

    const [daliyButton, setDaliyButton] = useState([])

    const [queryButton, setQueryButton] = useState([])

    const [otherButton, setOtherButton] = useState([])

    const handleMouseEnter = (id) => {
        if (daliyButton.some(d => d.id === id)) {
            const changeStyle = daliyButton.map(button => {
                if (button.id === id) {
                    return {
                        ...button,
                        isHovered: true
                    }
                }
                return {
                    ...button,
                    isHovered: false
                }
            })
            return setDaliyButton(changeStyle)
        } else if (queryButton.some(q => q.id === id)) {
            const changeStyle = queryButton.map(button => {
                if (button.id === id) {
                    return {
                        ...button,
                        isHovered: true
                    }
                }
                return {
                    ...button,
                    isHovered: false
                }
            })
            return setQueryButton(changeStyle)
        } else if (otherButton.some(o => o.id === id)) {
            const changeStyle = otherButton.map(button => {
                if (button.id === id) {
                    return {
                        ...button,
                        isHovered: true
                    }
                }
                return {
                    ...button,
                    isHovered: false
                }
            })
            return setOtherButton(changeStyle)
        }
    }

    const handleMouseLeave = (id) => {
        if (daliyButton.some(d => d.id === id)) {
            const changeStyle = daliyButton.map(button => {
                if (button.id === id) {
                    return {
                        ...button,
                        isHovered: false
                    }
                }
                return button
            })
            return setDaliyButton(changeStyle)
        } else if (queryButton.some(q => q.id === id)) {
            const changeStyle = queryButton.map(button => {
                if (button.id === id) {
                    return {
                        ...button,
                        isHovered: false
                    }
                }
                return button
            })
            return setQueryButton(changeStyle)
        } else if (otherButton.some(o => o.id === id)) {
            const changeStyle = otherButton.map(button => {
                if (button.id === id) {
                    return {
                        ...button,
                        isHovered: false
                    }
                }
                return button
            })
            return setOtherButton(changeStyle)
        }
    }

    return (
        <Card className='me-1 shadow-lg' style={{ width: '100vw', height: 'auto' }}>
            <CardHeader className='mb-1 py-1 border-bottom'>
                <Row className='d-flex justify-content-evenly ps-1 w-100'>
                    <Col className='d-flex align-items-center' style={{ width: '20rem' }}>
                        <CardText>{t('dashboard.dailyWork', { ns: 'dashboardMgmt' })}</CardText>
                    </Col>
                    <Col className='d-flex align-items-center' style={{ width: '20rem' }}>
                        <CardText>{t('dashboard.inquireAndReport', { ns: 'dashboardMgmt' })}</CardText>
                    </Col>
                    <Col className='d-flex align-items-center' style={{ width: '20rem' }}>
                        <CardText>{t('dashboard.otherWork', { ns: 'dashboardMgmt' })}</CardText>
                    </Col>
                </Row>
            </CardHeader>
            <CardBody className='pt-0 pb-0'>
                <Row className='d-flex w-100'>
                    <Col sm='4'>
                        {filterAbility(daliyButton).map((button, index) => {
                            return (
                                <Col key={button.id}>
                                    <Link
                                        to={button.to}
                                        className="border border-1 rounded mb-1 d-flex justify-content-center"
                                        style={{
                                            padding: '15px 0px',
                                            height: 'auto',
                                            width: '100%',
                                            backgroundColor: 'rgb(226 223 249 / 69%)',
                                            boxShadow: button.isHovered ? '4px 4px 12px grey' : 'none',
                                            transition: 'box-shadow 0.3s ease-in-out',
                                            display: button.id === 13 ? 'none' : 'block'
                                        }}
                                        onMouseEnter={() => handleMouseEnter(button.id)}
                                        onMouseLeave={() => handleMouseLeave(button.id)}
                                    >
                                        <Button
                                            color='white'
                                            className='d-flex flex-row align-items-center justify-content-center btn btn-white w-100'
                                        >
                                            <Row className="d-flex flex-row flex-nowrap w-100 align-items-center">
                                                <img src={button.imgSrc} style={button.imgStyle} />
                                                <div className='pe-0' style={{ maxWidth: '95%' }}>
                                                    <CardTitle className="text-start fw-normal mb-0" style={{ fontSize: '16px', width: '85%' }}>{t(`${button.title}`, { ns: 'dashboardMgmt' })}</CardTitle>
                                                    <CardSubtitle className="text-start mt-50" style={{ fontSize: '12px', width: '85%', lineHeight: '1rem' }}>{button.subtitle}</CardSubtitle>
                                                </div>
                                            </Row>
                                        </Button>
                                    </Link>
                                </Col>
                            )
                        })}
                    </Col>

                    <Col sm='4'>
                        {filterAbility(queryButton).map((button, index) => {
                            return (
                                <Col key={button.id}>
                                    <Link
                                        to={button.to}
                                        className="border border-1 rounded mb-1 d-flex justify-content-center"
                                        style={{
                                            padding: '15px 0px',
                                            height: 'auto',
                                            width: '100%',
                                            backgroundColor: 'rgb(246 225 201 / 69%)',
                                            boxShadow: button.isHovered ? '4px 4px 12px grey' : 'none',
                                            transition: 'box-shadow 0.3s ease-in-out',
                                            display: button.id === 13 ? 'none' : 'block'
                                        }}
                                        onMouseEnter={() => handleMouseEnter(button.id)}
                                        onMouseLeave={() => handleMouseLeave(button.id)}
                                    >
                                        <Button
                                            color='white'
                                            className='d-flex flex-row align-items-center justify-content-center btn btn-white w-100'
                                        >
                                            <Row className="d-flex flex-row flex-nowrap w-100 align-items-center">
                                                <img src={button.imgSrc} style={button.imgStyle} />
                                                <div className='pe-0' style={{ maxWidth: '95%' }}>
                                                    <CardTitle className="text-start fw-normal mb-0" style={{ fontSize: '16px', width: '85%' }}>{t(`${button.title}`, { ns: 'dashboardMgmt' })}</CardTitle>
                                                    <CardSubtitle className="text-start mt-50" style={{ fontSize: '12px', width: '85%', lineHeight: '1rem' }}>{button.subtitle}</CardSubtitle>
                                                </div>
                                            </Row>
                                        </Button>
                                    </Link>
                                </Col>
                            )
                        })}
                    </Col>

                    <Col sm='4'>
                        {filterAbility(otherButton).map((button, index) => {
                            return (
                                <Col key={button.id}>
                                    <Link
                                        to={button.to}
                                        className="border border-1 rounded mb-1 d-flex justify-content-center"
                                        style={{
                                            padding: '15px 0px',
                                            height: 'auto',
                                            width: '100%',
                                            backgroundColor: 'rgb(195 236 241 / 69%)',
                                            boxShadow: button.isHovered ? '4px 4px 12px grey' : 'none',
                                            transition: 'box-shadow 0.3s ease-in-out',
                                            display: button.id === 13 ? 'none' : 'block'
                                        }}
                                        onMouseEnter={() => handleMouseEnter(button.id)}
                                        onMouseLeave={() => handleMouseLeave(button.id)}
                                    >
                                        <Button
                                            color='white'
                                            className='d-flex flex-row align-items-center justify-content-center btn btn-white w-100'
                                        >
                                            <Row className="d-flex flex-row flex-nowrap w-100 align-items-center">
                                                <img src={button.imgSrc} style={button.imgStyle} />
                                                <div className='pe-0' style={{ maxWidth: '95%' }}>
                                                    <CardTitle className="text-start fw-normal mb-0" style={{ fontSize: '16px', width: '85%' }}>{t(`${button.title}`, { ns: 'dashboardMgmt' })}</CardTitle>
                                                    <CardSubtitle className="text-start mt-50" style={{ fontSize: '12px', width: '85%', lineHeight: '1rem' }}>{button.subtitle}</CardSubtitle>
                                                </div>
                                            </Row>
                                        </Button>
                                    </Link>
                                </Col>
                            )
                        })}
                    </Col>
                </Row>
            </CardBody>
        </Card>
    )
}

export default Features
