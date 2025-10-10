// ** React Imports
import { Fragment, useState, useEffect, useMemo } from 'react'
import { Link } from 'react-router-dom'

// ** Reactstrap Imports
import {
    Row,
    Col,
    Card,
    Label,
    Input,
    Table,
    Modal,
    Button,
    CardBody,
    ModalBody,
    ModalHeader,
    FormFeedback,
    UncontrolledTooltip
} from 'reactstrap'

// ** Third Party Components
import { Copy, Info } from 'react-feather'
import { useForm, Controller, useFieldArray } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'
import { useDispatch, useSelector } from 'react-redux'
import axios from 'axios'

// ** Custom Components
import AvatarGroup from '@components/avatar-group'
import { getUserData } from '@utils'
import { confirmDelete } from '@CityAppHelper'

// ** FAQ Illustrations
import illustration from '@src/assets/images/illustration/faq-illustrations.svg'

//store
import { getAllData, addGroup, deleteGroup, updateGroup } from './store'


const RoleCards = ({ access }) => {

    // ** States
    const dispatch = useDispatch()
    const store = useSelector(state => state.SettingMgmt_GroupAccount)

    const [show, setShow] = useState(false)
    const [modalType, setModalType] = useState('新增')
    const [programArr, setProgramArr] = useState([])

    // ** Hooks
    const schema = yup.object({
        groupName: yup.string().required()
    })

    const {
        register,
        reset,
        control,
        setValue,
        getValues,
        handleSubmit,
        formState: { errors }
    } = useForm({
        defaultValues: { groupName: '', description: '', userID: getUserData()?.userId || '' },
        resolver: yupResolver(schema)
    })

    const { fields, append, prepend, remove, swap, move, insert } = useFieldArray({
        control,
        name: 'userGroupPermission'
    })

    const SubmitFunc = {
        新增: (request) => {
            dispatch(addGroup(request)).then(res => !res.error && setShow(false))
        },
        修改: (request) => {
            dispatch(updateGroup(request)).then(res => !res.error && setShow(false))
        }
    }

    const onSubmit = data => {
        const permission = []
        data.userGroupPermission.forEach(program => Object.keys(program).forEach(key => key !== 'selectAll' && program[key] && permission.push(Number(key))))
        const request = {
            ...data,
            permission
        }
        SubmitFunc[modalType](request)
    }

    const handleModalClosed = () => {
        setModalType('新增')
    }


    //get data
    const getGroupPermission = async (request) => {
        await axios.post(`/api/GroupAccount/GetGroupPermission`, request)
            .then((res) => {
                const programList = res?.data?.data?.programList || []
                programList.forEach(program_outer => {
                    const index = programArr.findIndex(program_inner => program_inner.programID === program_outer.programID)
                    program_outer.permission.forEach(({ permissionID }) => setValue(`userGroupPermission.${index}.${permissionID}`, true))
                })
            })
        setValue('groupName', request.groupName)
        setValue('description', request.description)
        setValue('originalGroupName', request.groupName)
    }

    const onSelectWholeTableChange = (e) => {
        const { checked, id } = e.target
        programArr.forEach((row, rowIndex) => {
            row.permission?.forEach((col, colIndex) => {
                setValue(`userGroupPermission.${rowIndex}.${col.permissionID}`, checked)
                setValue(`userGroupPermission.${rowIndex}.selectAll`, checked)
            })
        })
    }

    const onSelectWholeRowChange = (e) => {
        const { checked, id } = e.target
        const name = id.split('.')?.[1]
        programArr?.[name].permission.forEach((col, rowIndex) => {
            setValue(`userGroupPermission.${name}.${col.permissionID}`, checked)
        })
    }

    const TableData = useMemo(() => {
        let parent = ''
        return (
            programArr.map((program, index) => {
                return (
                    <Fragment key={index}>
                        {parent !== program.parentName && (parent = program.parentName) &&
                            <tr key={program.parentName} style={{ borderBottomWidth: '1px' }}>
                                <td className='text-nowrap fw-bolder' style={{ border: 'none' }}>{program.parentName}</td>
                            </tr>
                        }
                        <tr key={index} style={{ borderBottomWidth: '1px' }}>
                            <td className='text-nowrap fw-bolder ps-1' style={{ border: 'none' }}>
                                <div className='form-check'>
                                    <input type='checkbox' className='form-check-input' id={`userGroupPermission.${index}.selectAll`} {...register(`userGroupPermission.${index}.selectAll`)} onChange={onSelectWholeRowChange} />
                                    <Label className='form-check-label' for={`userGroupPermission.${index}.selectAll`}>
                                        {program.programName}
                                    </Label>
                                </div>
                            </td>
                            {program.permission.map((permission, innerIndex) => {
                                return (
                                    <td className='text-nowrap fw-bolder' key={innerIndex}>
                                        <div className='form-check' >
                                            <input type='checkbox' className='form-check-input' id={`userGroupPermission.${index}.${permission.permissionID}`} {...register(`userGroupPermission.${index}.${permission.permissionID}`)} />
                                            <Label className='form-check-label' for={`userGroupPermission.${index}.${permission.permissionID}`}>
                                                {permission.description}
                                            </Label>
                                        </div>
                                    </td>
                                )
                            })}
                        </tr>
                    </Fragment>
                )
            })
        )
    }, [programArr])

    useEffect(() => {
        dispatch(getAllData())
            .then(({ error, payload }) => {
                if (error) return
                setProgramArr(payload.data.permissionList)
            })
    }, [dispatch])

    useEffect(() => {
        if (show) {
            const tmp = programArr.map(row => {
                const obj = {}
                row.permission.forEach(({ permissionID }) => (obj[permissionID] = false))
                return obj
            })
            setValue('userGroupPermission', tmp)
        } else {
            reset()
        }

    }, [show, programArr])

    return (
        <Fragment>
            <Row>
                {access.create &&
                    <Col xl={4} md={6}>
                        <Card>
                            <Row>
                                <Col sm={5}>
                                    <div className='d-flex align-items-end justify-content-center h-100'>
                                        <img className='img-fluid mt-2' src={illustration} alt='Image' width={85} />
                                    </div>
                                </Col>
                                <Col sm={7}>
                                    <CardBody className='text-sm-end text-center ps-sm-0'>
                                        <Button
                                            color='primary'
                                            className='text-nowrap mb-1'
                                            onClick={() => {
                                                setModalType('新增')
                                                setShow(true)
                                            }}
                                        >
                                            新增群組
                                        </Button>
                                        <p className='mb-0'>需要更多群組，可以在此新增!</p>
                                    </CardBody>
                                </Col>
                            </Row>
                        </Card>
                    </Col>
                }
                {store.allGroup.map((item, index) => {
                    return (
                        <Col key={index} xl={4} md={6}>
                            <Card>
                                <CardBody>
                                    <div className='d-flex justify-content-between'>
                                        <span>{`共有 ${item.totalUsers} 位使用者`}</span>
                                        <AvatarGroup data={item.users} />
                                    </div>
                                    <div className='d-flex justify-content-between align-items-end mt-1 pt-25'>
                                        <div className='role-heading'>
                                            <h4 className='fw-bolder'>{item.groupName}</h4>

                                            <Link
                                                to='/'
                                                className='role-edit-modal me-1'
                                                onClick={e => {
                                                    e.preventDefault()
                                                    setModalType('修改')
                                                    setShow(true)
                                                    getGroupPermission({ groupName: item.groupName, description: item.description })
                                                }}
                                            >
                                                <small className='fw-bolder'>查看群組</small>
                                            </Link>

                                            {access.delete &&
                                                <Link
                                                    to='/'
                                                    className='role-edit-modal'
                                                    onClick={e => {
                                                        e.preventDefault()
                                                        confirmDelete(() => { dispatch(deleteGroup({ groupName: item.groupName, userID: getUserData()?.userId || '' })) })
                                                    }}
                                                >
                                                    <small className='fw-bolder'>刪除群組</small>
                                                </Link>
                                            }
                                        </div>
                                    </div>
                                </CardBody>
                            </Card>
                        </Col>
                    )
                })}
            </Row>
            <Modal
                isOpen={show}
                onClosed={handleModalClosed}
                toggle={() => setShow(!show)}
                className='modal-dialog-centered modal-xl'
            >
                <ModalHeader className='bg-transparent' toggle={() => setShow(!show)}></ModalHeader>
                <ModalBody className='px-5 pb-5'>
                    <div className='text-center mb-1'>
                        <h1>{modalType}群組</h1>
                    </div>
                    <Row tag='form' onSubmit={handleSubmit(onSubmit)}>
                        <Col xs={6}>
                            <Label className='form-label' for='groupName'>
                                群組名稱
                            </Label>
                            <Controller
                                name='groupName'
                                control={control}
                                render={({ field }) => (
                                    <Input {...field} id='groupName' placeholder='請輸入群組名稱' invalid={errors.groupName && true} />
                                )}
                            />
                        </Col>
                        <Col xs={6}>
                            <Label className='form-label' for='groupName'>
                                說明
                            </Label>
                            <Controller
                                name='description'
                                control={control}
                                render={({ field }) => (
                                    <Input {...field} id='description' />
                                )}
                            />
                        </Col>
                        <Col xs={12}>
                            <h4 className='mt-2 pt-50'>功能權限</h4>
                            <div style={{ overflow: 'auto', maxHeight: '50vh' }} >
                                <Table className='table-flush-spacing' size='sm' >
                                    <tbody>
                                        <tr>
                                            <td className='text-nowrap fw-bolder' style={{ border: 'none' }}>
                                                <div className='form-check'>
                                                    <Input type='checkbox' onChange={onSelectWholeTableChange} id='selectAll' />
                                                    <Label className='form-check-label' for={`selectAll`}>
                                                        全選
                                                    </Label>
                                                </div>
                                            </td>
                                        </tr>
                                        {
                                            TableData
                                        }
                                    </tbody>
                                </Table>
                            </div>
                        </Col>
                        {((modalType === '新增' && access.create) || (modalType === '修改' && access.update)) &&
                            <Col className='text-center mt-2' xs={12}>

                                <Button type='submit' color='primary' className='me-1'>
                                    保存
                                </Button>

                            </Col>
                        }
                    </Row>
                </ModalBody>
            </Modal>
        </Fragment >
    )
}

export default RoleCards
