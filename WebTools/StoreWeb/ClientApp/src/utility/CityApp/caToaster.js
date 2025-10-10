// ** React Imports
import { Fragment } from 'react'

// ** Custom Components
import Avatar from '@components/avatar'

// ** Third Party Components
import { toast } from 'react-toastify'
import { Bell, Check, X, AlertTriangle, Info } from 'react-feather'

// ** CityApp Utilty
import { getFullTimeTW } from '@CityAppHelper'

const PrimaryToast = ({ title, message }) => (
    <Fragment>
        <div className='toastify-header'>
            <div className='title-wrapper'>
                <Avatar size='sm' color='primary' icon={<Bell size={12} />} />
                <h6 className='toast-title'>{title}</h6>
            </div>
            <small className='text-muted'>{getFullTimeTW()}</small>
        </div>
        <div className='toastify-body'>
            <span role='img' aria-label='toast-text'>{message}</span>
        </div>
    </Fragment>
)

const SuccessToast = ({ title, message }) => (
    <Fragment>
        <div className='toastify-header'>
            <div className='title-wrapper'>
                <Avatar size='sm' color='success' icon={<Check size={12} />} />
                <h6 className='toast-title'>{title}</h6>
            </div>
            <small className='text-muted'>{getFullTimeTW()}</small>
        </div>
        <div className='toastify-body'>
            <span role='img' aria-label='toast-text'>
                <span role='img' aria-label='toast-text'>{message}</span>
            </span>
        </div>
    </Fragment>
)

const ErrorToast = ({ title, message }) => (
    <Fragment>
        <div className='toastify-header'>
            <div className='title-wrapper'>
                <Avatar size='sm' color='danger' icon={<X size={12} />} />
                <h6 className='toast-title'>{title}</h6>
            </div>
            <small className='text-muted'>{getFullTimeTW()}</small>
        </div>
        <div className='toastify-body'>
            <span role='img' aria-label='toast-text'>
                <span role='img' aria-label='toast-text'>{message}</span>
            </span>
        </div>
    </Fragment>
)

const WarningToast = ({ title, message }) => (
    <Fragment>
        <div className='toastify-header'>
            <div className='title-wrapper'>
                <Avatar size='sm' color='warning' icon={<AlertTriangle size={12} />} />
                <h6 className='toast-title'>{title}</h6>
            </div>
            <small className='text-muted'>{getFullTimeTW()}</small>
        </div>
        <div className='toastify-body'>
            <span role='img' aria-label='toast-text'>
                <span role='img' aria-label='toast-text'>{message}</span>
            </span>
        </div>
    </Fragment>
)

const InfoToast = ({ title, message }) => (
    <Fragment>
        <div className='toastify-header'>
            <div className='title-wrapper'>
                <Avatar size='sm' color='info' icon={<Info size={12} />} />
                <span className='toast-title fs-5'>{title}</span>
            </div>
            <small className='text-muted'>{getFullTimeTW()}</small>
        </div>
        <div className='toastify-body'>
            <span role='img' aria-label='toast-text'>
                <span role='img' aria-label='toast-text' className='fs-3'>{message}</span>
            </span>
        </div>
    </Fragment>
)

// REF: https://fkhadra.github.io/react-toastify/introduction
// toast.info(<InfoToast title={title} message={message} />, { icon: false, autoClose: 2000, position: toast.POSITION.BOTTOM_RIGHT })
const ToastTemplate = ({ title, message, color, IConComp }) => (
    <div>
        <div className='toastify-header'>
            <div className='title-wrapper'>
                <Avatar size='sm' color={color} icon={<IConComp />} />
                <span className='toast-title fs-5'>{title}</span>
            </div>
            <small className='text-muted'>{getFullTimeTW()}</small>
        </div>
        <div className='toastify-body px-0'>
            <span role='img' aria-label='toast-text'>
                <span role='img' aria-label='toast-text' className='fs-3'>{message}</span>
            </span>
        </div>
    </div>
)
const defaultOption = { icon: false, autoClose: 2000, position: "bottom-right" }

export const ShowToast = (title, message, type = '') => {

    switch (type) {
        case 'success':
            toast.success(<ToastTemplate title={title} message={message} color='success' IConComp={Check} />, defaultOption)
            break
        case 'danger':
            toast.error(<ToastTemplate title={title} message={message} color='danger' IConComp={X} />, { ...defaultOption })
            break
        case 'warning':
            toast.warning(<ToastTemplate title={title} message={message} color='warning' IConComp={AlertTriangle} />, defaultOption)
            break
        case 'info':
            toast.info(<ToastTemplate title={title} message={message} color='info' IConComp={Info} />, defaultOption)
            break
        default:
            toast(<ToastTemplate title={title} message={message} color='primary' IConComp={Bell} />, defaultOption)
    }

} 
