// ** React Import
import { useEffect, useMemo } from 'react'
import { Link, useNavigate } from 'react-router-dom'

// ** Utils
import { selectThemeColors, selectStyleNHL, formatSelectOptionLabelNL } from '@utils'
import { CustomLabel, MultiSelect, Select } from '@CityAppComponents'
import { parseValueLabelObject } from '@CityAppHelper'

// ** Third Party Components
import classnames from 'classnames'
import { useForm, Controller } from 'react-hook-form'
import { yupResolver } from "@hookform/resolvers/yup"
import * as yup from 'yup'
import useState from 'react-usestateref'

// ** Reactstrap Imports
import { Button, Label, FormText, Form, Input, Card, CardBody, CardHeader, CardTitle, Row, Col } from 'reactstrap'

// ** Store & Actions
import { addUser, updateUser, getSingleData, getAllData, resetContent, savePageStatus } from '../store'
import { useDispatch, useSelector } from 'react-redux'


const defaultValues = {
    userID: '',
    password: '',
    passwordConfirm: '',
    description: '',
    groupName: [],
    userNumber: '',
    costPermission: false,
    clientPermission: []
}


const UserContent = ({ id, t, access }) => {

    const [cancelBtnClick, setCancelBtnClick, cancelBtnClickRef] = useState(false)

    // ** Store Vars
    const dispatch = useDispatch()
    const navigate = useNavigate()
    const store = useSelector(state => state.SettingMgmt_UserAccount)
    const { empOption, groupOption, clientOption, status } = store
    const { selectedUser, selectedMode } = status.single


    const defaultPhotoShot = require('@src/assets/images/avatars/avatar-blank.png').default
    const [avatar, setAvatar] = useState(selectedUser?.avatar || defaultPhotoShot)

    const schema = yup.object({
        userID: yup.string().required(),
        groupName: yup.array().required(),
        userNumber: yup.object().required(),
        password: yup.string().required(),
        passwordConfirm: yup.string().oneOf([yup.ref('password'), null]).required()
    })

    // ** Vars
    const {
        control,
        reset,
        watch,
        getValues,
        handleSubmit,
        formState: { errors }
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues
    })

    const userNumber = watch('userNumber')

    const getModeSet = (selectedUser) => {
        const tmpValue = {
            ...defaultValues,
            ...selectedUser
        }

        const defaultValuesNew = {
            ...tmpValue,
            passwordConfirm: tmpValue?.password,
            originalUserID: tmpValue?.userID
        }

        if (id !== 'add') {
            return {
                title: `${t('edit', { ns: 'common' })}${t('userAccount.title', { ns: 'settingMgmt' })}`,
                defaultValues: defaultValuesNew,
                empOption: [...empOption, selectedUser?.userNumber],
                submitFunc: updateUser,
                mode: 'edit'
            }
        } else {
            return {
                title: `${t('add', { ns: 'common' })}${t('userAccount.title', { ns: 'settingMgmt' })}`,
                defaultValues: defaultValuesNew,
                empOption,
                submitFunc: addUser,
                mode: 'add'
            }
        }
    }

    const modeDefine = useMemo(() => getModeSet(selectedUser), [selectedUser, empOption, groupOption])
    // ** Function to handle form submit
    const onSubmit = data => {
        dispatch(
            modeDefine.submitFunc({
                originalUserID: data.originalUserID,
                userID: data.userID,
                password: data.password,
                description: data.description,
                groupName: data.groupName.sort((_g1, _g2) => (_g2.value > _g1.value ? 1 : -1)).map(_g => _g.value).join(','),
                userNumber: data.userNumber.value,
                costPermission: data.costPermission,
                clientPermission: data.clientPermission.sort((_c1, _c2) => (_c2.value > _c1.value ? 1 : -1)).map(_c => _c.value).join(',')
            })
        ).then(res => !res.error && navigate('../SettingMgmt/UserAccount/List', { replace: true }))
    }

    useEffect(() => {
        dispatch(getAllData())
    }, [dispatch])

    useEffect(() => {
        id !== 'add' && selectedMode === null && dispatch(getSingleData({ userID: id })).then(res => res.error && navigate('../SettingMgmt/UserAccount/List', { replace: true }))
    }, [id])

    useEffect(() => {
        setAvatar(selectedUser?.avatar || defaultPhotoShot)
        reset(modeDefine.defaultValues)
    }, [modeDefine])

    useEffect(() => {
        setAvatar(userNumber?.avatar || defaultPhotoShot)
    }, [userNumber])

    useEffect(() => {

        return () => {
            if (cancelBtnClickRef.current) {
                dispatch(resetContent({ key: 'single' }))
            } else {
                dispatch(savePageStatus({
                    key: 'single',
                    status: {
                        selectedUser: getValues(),
                        selectedMode: modeDefine.mode
                    }
                }))
            }
        }
    }, [])

    return (
        <Card>
            <CardHeader className='border-bottom p-1'>
                <CardTitle tag='h4'>{modeDefine.title}</CardTitle>
                <Col sm='6' className='d-flex justify-content-end'>
                    {(id === 'add' ? access.create : access.update) && (
                        <Button className='me-1' color='primary' onClick={handleSubmit(onSubmit)}>
                            {t('save', { ns: 'common' })}
                        </Button>
                    )}
                    <Button.Ripple className='me-1' color='flat-secondary' onClick={() => {
                        setCancelBtnClick(true)
                        navigate('../SettingMgmt/UserAccount/List', { replace: true })
                    }}>
                        {t('cancel', { ns: 'common' })}
                    </Button.Ripple>

                </Col>
            </CardHeader>
            <CardBody className='py-2 my-25'>
                <div className='d-flex'>
                    <div className='me-25'>
                        <img className='rounded me-50' src={avatar} alt='' height='100' width='100' />
                    </div>
                </div>
                <Form className='mt-2 pt-50'>
                    <Row>
                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='userNumber'>
                                {t('userAccount.userName', { ns: 'settingMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='userNumber'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        inputId='userNumber'
                                        options={modeDefine.empOption}
                                        className={classnames('react-select', { 'is-invalid': errors.userNumber && true })}
                                        {...field}
                                    />
                                )}

                            />
                        </Col>
                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='userID'>
                                {t('userAccount.userID', { ns: 'settingMgmt' })}<span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='userID'
                                control={control}
                                render={({ field }) => (
                                    <Input id='userID' autoComplete="off" placeholder='0001' invalid={errors.userID && true} {...field} />
                                )}
                            />
                        </Col>
                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='password'>
                                {t('userAccount.password', { ns: 'settingMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='password'
                                control={control}
                                render={({ field }) => (
                                    <Input id='password' placeholder='0001' type='password' invalid={errors.password && true} {...field} />
                                )}
                            />
                        </Col>
                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='passwordConfirm'>
                                {t('userAccount.passwordConfirm', { ns: 'settingMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='passwordConfirm'
                                control={control}
                                render={({ field }) => (
                                    <Input id='passwordConfirm' placeholder='請重複輸入密碼' type='password' invalid={errors.passwordConfirm && true} {...field} />
                                )}
                            />
                        </Col>
                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='groupName'>
                                {t('userAccount.groupName', { ns: 'settingMgmt' })} <span className='text-danger'>*</span>
                            </Label>
                            <Controller
                                name='groupName'
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        formatOptionLabel={formatSelectOptionLabelNL}
                                        inputId='groupName'
                                        isMulti
                                        closeMenuOnSelect={false}
                                        options={groupOption}
                                        className={classnames('react-select', { 'is-invalid': errors.groupName && true })}
                                        {...field}
                                    />
                                )}
                            />
                        </Col>
                        <Col sm='6' className='mb-1'>
                            <Label className='form-label' for='description'>
                                {t('userAccount.description', { ns: 'settingMgmt' })}
                            </Label>
                            <Controller
                                name='description'
                                control={control}
                                render={({ field }) => (
                                    <Input id='description' autoComplete="off" {...field} />
                                )}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Label className='form-label' for='clientPermission'>
                                {t('userAccount.clientPermission', { ns: 'settingMgmt' })}
                            </Label>
                            <Controller
                                name='clientPermission'
                                control={control}
                                render={({ field }) => (
                                    <MultiSelect
                                        formatOptionLabel={formatSelectOptionLabelNL}
                                        inputId='clientPermission'
                                        isMulti
                                        isClearable={false}
                                        closeMenuOnSelect={false}
                                        classNamePrefix='select'
                                        options={clientOption}
                                        theme={selectThemeColors}
                                        styles={selectStyleNHL}
                                        className={classnames('react-select')}
                                        placeholder={t('selectPlaceholder', { ns: 'common' })}
                                        {...field}
                                    />
                                )}
                            />
                        </Col>
                        <Col>
                            {/* <Label className='form-label' for='description'>
                                {t('userAccount.dataPermission', { ns: 'settingMgmt' })}
                            </Label> */}
                            <Label for='costPermission' className='form-label'>
                                成本權限
                            </Label>
                            <div className='form-switch form-check-primary'>
                                <Controller
                                    name='costPermission'
                                    control={control}
                                    render={({ field }) => (
                                        <Input type='switch' id='costPermission' name='icon-primary' checked={field.value} {...field} />
                                    )}
                                />
                                <CustomLabel htmlFor='costPermission' />
                            </div>
                        </Col>
                    </Row>
                </Form>
            </CardBody>
        </Card >

    )
}

export default UserContent
