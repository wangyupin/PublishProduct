import { Card, CardBody, CardText, Form, Input, InputGroup, InputGroupText } from 'reactstrap'

// ** Styles
import '@srcAssets/scss/views/home.scss'
import themeConfig from '@configs/themeConfig'

const Home = () => {
  return (
    <div>
      <Card
        className='home-body'
        style={{
          backgroundImage: `url(${require('@src/assets/images/banner/banner.png').default})`
        }}
      >
        <CardBody className='text-center'>
          <h2 className='text-primary'>歡迎使用{themeConfig.app.appName}</h2>
          {/* <a href='http://system.gbtech.com.tw/' target='_blank' rel='noopener noreferrer'>
          {themeConfig.app.licenseFor}
          </a> */}
          <CardText className='mb-2'>Welcome to {themeConfig.app.appName}</CardText>

        </CardBody>
      </Card>

    </div>
  )
}

export default Home
