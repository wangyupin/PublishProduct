// ** React Imports
import { useEffect, useState, useContext, Fragment } from 'react'
import { Link, useNavigate } from 'react-router-dom'

// ** Custom Hooks
import { useSkin } from '@hooks/useSkin'
import useJwt from '@src/auth/jwt/useJwt'

// ** Third Party Components
import { useSelector, useDispatch } from 'react-redux'
import { toast, Slide } from 'react-toastify'
import { useForm, Controller } from 'react-hook-form'
import { Facebook, Twitter, Mail, GitHub, HelpCircle, Coffee } from 'react-feather'
import axios from 'axios'

// ** Configs
import themeConfig from '@configs/themeConfig'

// ** Actions
import { handleLogin, getClientCheck, getGroupProgram, employeeLogin, employeeLoginApi, updMacAddress } from '@store/authentication'

// ** Context
import { AbilityContext } from '@src/utility/context/Can'

// ** Custom Components
import Avatar from '@components/avatar'
import InputPasswordToggle from '@components/input-password-toggle'
import UILoader from '@components/ui-loader'

// ** Utils
import { getHomeRouteForLoggedInUser, isObjEmpty } from '@utils'

// ** Reactstrap Imports
import { Row, Col, Form, Input, Label, Alert, Button, CardText, CardTitle, UncontrolledTooltip } from 'reactstrap'

// ** CityApp Utilty
import { showMessageBox, isMobileDevice, decryptAES } from '@CityAppHelper'
import { LoginLoader } from '@CityAppComponents'
import { useHeartbeatService } from '@application/service/heartbeatService'

// ** Styles
import '@styles/react/pages/page-authentication.scss'

const ToastContent = ({ username, userId }) => (
  <Fragment>
    <div className='toastify-header'>
      <div className='title-wrapper'>
        <Avatar size='sm' color='success' icon={<Coffee size={12} />} />
        <h6 className='toast-title fw-bold'>您好, {username}</h6>
      </div>
    </div>
    <div className='toastify-body'>
      <span>您已成功使用 {userId} 帳號登入, 開始使用吧!</span>
    </div>
  </Fragment>
)

const defaultValues = {
  password: '',
  userId: '',
  empId: '',
  empPassword: ''
}

const Login = () => {
  // login information
  const [companyId, setCompanyId] = useState('Cityapp')
  const [companyName, setCompanyName] = useState('旌泓股份有限公司')
  const [appVer, setAppVer] = useState('V0.1')
  const [startDate, setStartDate] = useState('2021/11/30')
  const [licenseQuantity, setLicenseQuantity] = useState('無限制')
  const [imageError, setImageError] = useState(false)

  // const [userId, setUserId] = useState('')
  // const [password, setPassword] = useState('')

  const [isLoading, setIsLoading] = useState(false)
  // ** Hooks
  const { skin } = useSkin()
  const dispatch = useDispatch()
  const navigate = useNavigate()
  const ability = useContext(AbilityContext)

  const {
    control,
    setError,
    handleSubmit,
    setValue,
    formState: { errors }
  } = useForm({ defaultValues })
  const illustration = skin === 'dark' ? 'login-v2-dark.svg' : 'login-v2.svg',
    source = require(`@src/assets/images/pages/${illustration}`).default

  const doLoginSuccess = async (res, empInfo) => {
    const userData = { ...empInfo.data.data.empData }
    const userAbility = empInfo.data.data.detailList
    const machineId = res.data.userData.userId
    localStorage.setItem('machineId', machineId)
    localStorage.setItem('isFront', res.data.isFront)

    res.data.userData = {
      ...res.data.userData,
      ability: userAbility,
      role: userData.groupName,
      userId: userData.userID,
      username: userData.userName,
      machineId,
      clientPermission: empInfo.data.data.clientPermission
    }
    dispatch(handleLogin(res.data))
    ability.update(userAbility)
    navigate(getHomeRouteForLoggedInUser(userData.groupName))

    toast.success(
      <ToastContent username={userData.userName} userId={userData.userID} />,
      { icon: false, transition: Slide, hideProgressBar: true, autoClose: 3000 }
    )

    window.dispatchEvent(new Event('userChanged'))
  }

  const updMac = async (data, macAddress) => {
    if (macAddress) {
      const res = await dispatch(updMacAddress({ companyId, userId: data.userId, password: data.password, force: true, MacAddress: macAddress }))
        .catch(err => {
          return err
        })
      if (res?.payload?.message) {
        throw new Error(res?.payload?.message)
      } else {
        return res
      }
    }
  }

  const doLoginFaliure = err => {
    setIsLoading(false)
    showMessageBox({
      title: '登入失敗，請重試',
      text: err,
      icon: 'error',
      timer: 5000,
      customClass: {
        confirmButton: 'btn btn-primary'
      },
      buttonsStyling: false
    })
  }

  const onSubmit = async data => {
    if (Object.values(data).every(field => field.length > 0)) {
      setIsLoading(true)

      useJwt
        .login({ companyId, userId: data.userId, password: data.password, force: false })
        .then(res => {
          setIsLoading(false)
          if (res.data.loginDuplication) {
            showMessageBox({
              title: '重覆登入',
              text: "是否強制移除前次登入?",
              icon: 'warning',
              confirmButtonText: '強制登入',
              showCancelButton: true,
              cancelButtonText: '取消登入',
              customClass: {
                confirmButton: 'btn btn-primary me-1',
                cancelButton: 'btn btn-outline-danger'
              },
              buttonsStyling: false
            }).then(function (result) {
              if (result.value) {
                useJwt
                  .login({ companyId, userId: data.userId, password: data.password, force: true })
                  .then(res2 => {
                    res = res2
                    return employeeLoginApi({ empId: data.empId, empPassword: data.empPassword })
                  })
                  .then(async (empInfo) => {
                    return empInfo
                  })
                  .then((empInfo) => {
                    doLoginSuccess(res, empInfo)
                  })
                  .catch(err => {
                    doLoginFaliure(err)
                  })
              } else {
                setIsLoading(false)
              }
            })
          } else {
            employeeLoginApi({ empId: data.empId, empPassword: data.empPassword })
              .then(async (empInfo) => {
                const res = await updMac(data)
                return empInfo
              })
              .then((empInfo) => {
                doLoginSuccess(res, empInfo)
              })
              .catch(err => {
                doLoginFaliure(err)
              })
          }
        })
        .catch(err => {
          doLoginFaliure(err.response.data.message)
        })
    } else {
      for (const key in data) {
        if (data[key].length === 0) {
          setError(key, {
            type: 'manual',
            message: `${key} error`
          })
        }
      }
    }
  }

  useEffect(() => {
    useJwt.getAppInfo().then(function (res) {

      setCompanyId(res.data.data.companyId)
      setCompanyName(res.data.data.companyName)
      setAppVer(res.data.data.appVer)
      setStartDate(res.data.data.startDate)
      setLicenseQuantity(res.data.data.licenseQuantity)
      // setUserId(res.data.data.defaultUID)
      // setPassword(res.data.data.defaultPSW)
      localStorage.setItem('companyName', res.data.data.companyName)
      localStorage.setItem('goodsSize', res.data.data.goodsSize)
      localStorage.setItem('hasWebSite', res.data.data.hasWebSite)

      setValue('userId', localStorage.getItem('machineId') || res.data.data.defaultUID)
      setValue('password', res.data.data.defaultPSW)
      setValue('empId', res.data.data.defaultEUID)
      setValue('empPassword', res.data.data.defaultEPSW)
    })
  }, [])

  return (
    <UILoader blocking={isLoading} loader={<LoginLoader />}>
      <div className='auth-wrapper auth-cover' style={{ backgroundImage: "url('/assets/image/bg.png')" }}>
        <Row className='auth-inner m-0'>
          <Col className='d-flex align-items-center auth-bg px-2 p-lg-5 ms-auto' lg='4' sm='12'>
            <Col className='px-xl-2 mx-auto' sm='8' md='6' lg='12'>
              {!imageError ? (
                <img
                  src="/login.ico"
                  alt="Welcome"
                  className="mb-2"
                  onError={() => setImageError(true)}
                />
              ) : (
                <CardTitle tag='h2' className='fw-bold mb-1'>
                  {`Welcome to ${themeConfig.app.appName}! 👋`}
                </CardTitle>
              )}

              <Alert color='primary'>
                <div className='alert-body font-small-2'>
                  <p>
                    <small className='mr-50'>
                      <span className='font-weight-bold'>授權公司：</span>{companyName}
                    </small>
                  </p>
                  <p>
                    <small className='mr-50'>
                      <span className='font-weight-bold'>授權人數：</span>{licenseQuantity}
                    </small>
                  </p>
                </div>
                <HelpCircle
                  id='login-tip'
                  className='position-absolute'
                  size={18}
                  style={{ top: '10px', right: '10px' }}
                />
                <UncontrolledTooltip target='login-tip' placement='left'>
                  <div>軟體版號：{appVer}<br />授權日期：{startDate}</div>
                </UncontrolledTooltip>
              </Alert>
              <Form className='auth-login-form mt-2' onSubmit={handleSubmit(onSubmit)}>
                <div className='mb-1 d-none'>
                  <Label className='form-label' for='login-userId'>
                    機台帳號
                  </Label>
                  <Controller
                    name='userId'
                    control={control}
                    render={({ field }) => (
                      <Input
                        autoFocus
                        type='text'
                        placeholder='請輸入機台帳號'
                        invalid={errors.userId && true}
                        {...field}
                      />
                    )}
                  />
                </div>
                <div className='mb-1 d-none'>
                  <div className='d-flex justify-content-between'>
                    <Label className='form-label' for='login-password'>
                      機台密碼
                    </Label>
                    <a href='https://www.microsoft.com/zh-tw/security/mobile-authenticator-app' target='_blank' rel='noopener noreferrer'>
                      <small>或使用Microsoft Authenticator</small>
                    </a>
                  </div>
                  <Controller
                    name='password'
                    control={control}
                    render={({ field }) => (
                      <InputPasswordToggle
                        className='input-group-merge'
                        placeholder='請輸入機台密碼'
                        invalid={errors.password && true}
                        {...field}
                      />
                    )}
                  />
                </div>
                <div className='mb-1'>
                  <Label className='form-label' for='login-empId'>
                    員工帳號
                  </Label>
                  <Controller
                    name='empId'
                    control={control}
                    render={({ field }) => (
                      <Input
                        autoFocus
                        type='text'
                        placeholder='請輸入員工帳號'
                        invalid={errors.empId && true}
                        {...field}
                      />
                    )}
                  />
                </div>
                <div className='mb-1'>
                  <div className='d-flex justify-content-between'>
                    <Label className='form-label' for='login-empPassword'>
                      員工密碼
                    </Label>
                  </div>
                  <Controller
                    name='empPassword'
                    control={control}
                    render={({ field }) => (
                      <InputPasswordToggle
                        className='input-group-merge'
                        placeholder='請輸入員工密碼'
                        invalid={errors.empPassword && true}
                        {...field}
                      />
                    )}
                  />
                </div>

                <Button type='submit' color='primary' block>
                  登 入
                </Button>
              </Form>
            </Col>
          </Col>
        </Row>
      </div>
    </UILoader>
  )
}

export default Login
