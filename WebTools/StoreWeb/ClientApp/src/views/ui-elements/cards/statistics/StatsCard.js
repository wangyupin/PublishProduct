import classnames from 'classnames'
import Avatar from '@components/avatar'
import { TrendingUp, User, Box, DollarSign, TrendingDown } from 'react-feather'
import { Card, CardHeader, CardTitle, CardBody, CardText, Row, Col, Media } from 'reactstrap'

// ** CityAppHelper
import { getFormatedDate } from '@CityAppHelper'

const StatsCard = ({ cols }) => {
  const data = [
    {
      title: '$109,745',
      subtitle: '營業累計(仟)',
      color: 'light-success',
      icon: <DollarSign size={24} />
    },
    {
      title: '$60,145',
      subtitle: '營業淨利(仟)',
      color: 'light-primary',
      icon: <TrendingUp size={24} />
    },
    {
      title: '549,458',
      subtitle: '交易單量(仟)',
      color: 'light-info',
      icon: <User size={24} />
    },
    {
      title: '10,423',
      subtitle: '銷售品項(仟)',
      color: 'light-danger',
      icon: <Box size={24} />
    }
  ]

  const renderData = () => {
    return data.map((item, index) => {
      const margin = Object.keys(cols)
      return (
        <Col
          key={index}
          {...cols}
          className={classnames({
            [`mb-2 mb-${margin[0]}-0`]: index !== data.length - 1
          })}
        >
          <Media>
            <Avatar color={item.color} icon={item.icon} className='mr-2' />
            <Media className='my-auto' body>
              <h4 className='font-weight-bolder mb-0'>{item.title}</h4>
              <CardText className='font-small-2 mb-0'>{item.subtitle}</CardText>
            </Media>
          </Media>
        </Col>
      )
    })
  }

  return (
    <Card className='card-statistics'>
      <CardHeader>
        <CardTitle tag='h4'>今年累計</CardTitle>
        <CardText className='card-text font-small-2 mr-25 mb-0'>更新日 {getFormatedDate()}</CardText>
      </CardHeader>
      <CardBody className='statistics-body'>
        <Row>{renderData()}</Row>
      </CardBody>
    </Card>
  )
}

export default StatsCard
