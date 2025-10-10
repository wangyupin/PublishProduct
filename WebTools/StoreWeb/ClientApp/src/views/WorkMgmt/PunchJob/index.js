// ** React Imports
import ReactDOM from 'react-dom'
import { Fragment, useEffect, useCallback, useRef, forwardRef, useImperativeHandle, useContext } from 'react'

// ** Reactstrap Imports
import {
  Card, CardHeader, CardTitle, CardBody, CardText, Row, Col,
  Form, Input, InputGroup, InputGroupText, Label,
  Button, ButtonGroup,
  Modal, ModalBody, ModalHeader

} from 'reactstrap'

// ** Third Party Components
import useState from 'react-usestateref'


// ** CityApp Utilty
import { ShowToast } from '@CityAppExtComponents/caToaster'

import { getFormatedDateForInput } from '@CityAppHelper'

// ** Store & Actions
import { useSelector, useDispatch } from 'react-redux'
import { savePageStatus } from './store'

// ** View Components
import { CRUDTemplateDefault, CRUDTemplateSilence } from '@application/component/CRUDTemplate'
import { getEmpOptions } from '../../PurchaseMgmt/IncomingGoods/store'
const PunchTypeInputGroupTextList = ({ punchType, setPunchType }) => {
  const punchTypeName = [
    { type: 1, name: '上班' },
    { type: 2, name: '下班' },
    { type: 3, name: '外出' },
    { type: 4, name: '返回' },
    { type: 5, name: '進入' },
    { type: 6, name: '離開' }
  ]
  return (
    punchTypeName.map((item, index) => {
      return (
        <InputGroupText key={index} className='border-0 pe-0 fw-bolder fs-3 cursor-pointer' onClick={(e) => setPunchType(item.type)} >
          <div className='form-check'>
            <Input type='radio' className='cursor-pointer' name='punchType' checked={punchType === item.type} onChange={(e) => setPunchType(item.type)} />
          </div>
          {item.name}
        </InputGroupText>
      )
    })
  )
}

const Index = ({ setProgress }) => {
  // ** Store Vars
  const dispatch = useDispatch()
  const store = useSelector(state => state.WorkMgmt_PunchJob)
  const status = store.status.index

  const machineSet = JSON.parse(localStorage.getItem('machineSet'))

  const currentDate = new Date()
  const [punchDate, setPunchDate] = useState(getFormatedDateForInput(currentDate))
  const [punchClock, setPunchClock] = useState(currentDate.toLocaleTimeString('en-GB'))

  const [staffID, setStaffID, staffIDRef] = useState('')
  const [storeID, setStoreID] = useState('')
  const [storeName, setStoreName] = useState('')
  const punchTypeName = { 1: '上班', 2: '下班', 3: '外出', 4: '返回', 5: '進入', 6: '離開' }
  const [punchType, setPunchType, punchTypeRef] = useState(1) //1 上班；2 下班；3 外出；4 返回；5 進入；6;離開

  const viewTitle = '員工打卡作業'

  const punchJobButtonOnClick = async () => {
    CRUDTemplateSilence({
      debug: true,
      viewTitle,
      jobTitle: '打卡',
      request: {
        empID: staffID,
        clockStore: storeID,
        punchType
      },
      apiPath: '/api/EmpClockIn/DoPunchJob',
      doStart: () => {
        document.getElementById('staffID').focus()
        setProgress(25)
      },
      doError: (userData) => {
        setProgress(100)
        ShowToast(viewTitle, userData?.data?.msg || `${staffID}-${punchTypeName[punchType]} 打卡失敗`, 'danger')
      },
      doSuccess: (userData) => {
        setProgress(100)
        setStaffID('')
        ShowToast(viewTitle, userData.msg || `${staffID}-${punchTypeName[punchType]} 打卡完成`, 'success')
      }
    })
  }

  // initial punch clock
  useEffect(() => {
    const interval = setInterval(() => {
      const currentDate = new Date()
      setPunchDate(getFormatedDateForInput(currentDate))
      setPunchClock(currentDate.toLocaleTimeString('en-GB'))
    }, 1000)

    return () => {
      clearInterval(interval)
    }
  }, [])

  // initial variable
  useEffect(() => {
    //initital value
    setStoreID(machineSet.sellBranch)
    setStoreName(machineSet.sellBranchName)
    setStaffID(status.staffID || '')
    setPunchType(status.punchType || 1)

    return () => {
      dispatch(savePageStatus({
        key: 'status',
        status: {
          index: {
            staffID: staffIDRef.current,
            punchType: punchTypeRef.current
          }
        }
      }))
    }
  }, [])

  return (
    <Fragment>
      <Card className='col-lg-6 col-md-8'>

        <CardBody>
          <Row className='align-items-center'>
            <Col>
              <p className='mb-0 display-4 text-center'>{storeName}</p>
            </Col>
          </Row>
          <Row className='align-items-center'>
            <Col>
              <p className='mb-0 display-4 text-center'>{punchDate}</p>
            </Col>
          </Row>
          <Row className='align-items-center'>
            <Col >
              <p className='display-4 text-center text-danger fw-bolder'>{punchClock}</p>
            </Col>
          </Row>
          <Row className='align-items-center mb-1'>
            <Col md='3'>
              <Label className='fw-bolder fs-3'>打卡方式</Label>
            </Col>
            <Col>
              <InputGroup>
                <PunchTypeInputGroupTextList punchType={punchType} setPunchType={setPunchType} />
              </InputGroup>
            </Col>
          </Row>
          <Form>
            <Row className='g-1 align-items-center'>
              <Label md='3' className='fw-bolder fs-3' for='staffID'>卡號</Label>
              <Col md='5' >
                <Input
                  autoFocus
                  type='text' className='fw-bolder fs-3' autoComplete="off" id='staffID' placeholder='請輸入卡號'
                  value={staffID} onFocus={(e) => e.target.select()} onChange={e => setStaffID(e.target.value)} />
              </Col>
              <Col>
                <Button
                  className='fs-3 px-1 me-1' color='primary'
                  onClick={punchJobButtonOnClick} >打卡</Button>
              </Col>
            </Row>
          </Form>
        </CardBody>
      </Card>
    </Fragment>
  )
}

export default Index
