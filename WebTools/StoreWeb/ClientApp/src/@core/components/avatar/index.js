// ** React Imports
import { forwardRef, useState, useEffect } from 'react'
import { createPortal } from 'react-dom'

// ** Third Party Components
import Proptypes from 'prop-types'
import classnames from 'classnames'

// ** Reactstrap Imports
import { Badge } from 'reactstrap'

const Avatar = forwardRef((props, ref) => {

  const [showPreview, setShowPreview] = useState(false)
  const [previewPosition, setPreviewPosition] = useState({ x: 0, y: 0 })
  const [avatarRef, setAvatarRef] = useState(null)

  // ** Props
  const {
    img,
    size,
    icon,
    color,
    status,
    badgeUp,
    content,
    tag: Tag,
    initials,
    imgWidth,
    className,
    badgeText,
    imgHeight,
    badgeColor,
    imgClassName,
    contentStyles,
    enablePreview,
    ...rest
  } = props

  // ** Function to extract initials from content
  const getInitials = str => {
    const results = []
    const wordArray = str.split(' ')
    wordArray.forEach(e => {
      results.push(e[0])
    })
    return results.join('')
  }

  const updatePreviewPosition = () => {
    if (avatarRef && showPreview) {
      const rect = avatarRef.getBoundingClientRect()
      setPreviewPosition({
        x: rect.right + 10,
        y: rect.top - 100
      })
    }
  }

  useEffect(() => {
    if (showPreview) {
      updatePreviewPosition()
      window.addEventListener('scroll', updatePreviewPosition)
      return () => window.removeEventListener('scroll', updatePreviewPosition)
    }
  }, [showPreview])

  return (
    <div
      ref={setAvatarRef}
      onMouseEnter={() => enablePreview && setShowPreview(true)}
      onMouseLeave={() => enablePreview && setShowPreview(false)}>
      <Tag
        className={classnames('avatar', {
          [className]: className,
          [`bg-${color}`]: color,
          [`avatar-${size}`]: size
        })}
        ref={ref}
        {...rest}
      >
        {img === false || img === undefined || img === null || img?.length === 0 ? (
          <span
            className={classnames('avatar-content', {
              'position-relative': badgeUp
            })}
            style={contentStyles}
          >
            {initials ? getInitials(content) : content}

            {icon ? icon : null}
            {badgeUp ? (
              <Badge color={badgeColor ? badgeColor : 'primary'} className='badge-sm badge-up' pill>
                {badgeText ? badgeText : '0'}
              </Badge>
            ) : null}
          </span>
        ) : (
          <img
            className={classnames({
              [imgClassName]: imgClassName
            })}
            src={img}
            alt='avatarImg'
            height={imgHeight && !size ? imgHeight : 32}
            width={imgWidth && !size ? imgWidth : 32}
          />
        )}
        {status ? (
          <span
            className={classnames({
              [`avatar-status-${status}`]: status,
              [`avatar-status-${size}`]: size
            })}
          ></span>
        ) : null}
      </Tag>
      {enablePreview && showPreview && img && createPortal(
        <div
          style={{
            position: 'fixed',
            left: `${previewPosition.x}px`,
            top: `${previewPosition.y}px`,
            zIndex: 9999,
            backgroundColor: 'white',
            boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
            borderRadius: '4px',
            border: '1px solid #ddd',
            pointerEvents: 'none'
          }}
        >
          <img
            src={img}
            alt="Preview"
            style={{
              width: '200px',
              height: '200px',
              objectFit: 'cover',
              display: 'block'
            }}
          />
        </div>,
        document.body
      )}
    </div>
  )
})

export default Avatar

// ** PropTypes
Avatar.propTypes = {
  icon: Proptypes.node,
  src: Proptypes.string,
  badgeUp: Proptypes.bool,
  content: Proptypes.string,
  badgeText: Proptypes.string,
  className: Proptypes.string,
  imgClassName: Proptypes.string,
  contentStyles: Proptypes.object,
  size: Proptypes.oneOf(['sm', 'lg', 'xl']),
  tag: Proptypes.oneOfType([Proptypes.func, Proptypes.string]),
  status: Proptypes.oneOf(['online', 'offline', 'away', 'busy']),
  imgHeight: Proptypes.oneOfType([Proptypes.string, Proptypes.number]),
  imgWidth: Proptypes.oneOfType([Proptypes.string, Proptypes.number]),
  badgeColor: Proptypes.oneOf([
    'primary',
    'secondary',
    'success',
    'danger',
    'info',
    'warning',
    'dark',
    'light-primary',
    'light-secondary',
    'light-success',
    'light-danger',
    'light-info',
    'light-warning',
    'light-dark'
  ]),
  color: Proptypes.oneOf([
    'primary',
    'secondary',
    'success',
    'danger',
    'info',
    'warning',
    'dark',
    'light-primary',
    'light-secondary',
    'light-success',
    'light-danger',
    'light-info',
    'light-warning',
    'light-dark'
  ]),
  enablePreview: Proptypes.bool,
  initials(props) {
    if (props['initials'] && props['content'] === undefined) {
      return new Error('content prop is required with initials prop.')
    }
    if (props['initials'] && typeof props['content'] !== 'string') {
      return new Error('content prop must be a string.')
    }
    if (typeof props['initials'] !== 'boolean' && props['initials'] !== undefined) {
      return new Error('initials must be a boolean!')
    }
  }
}

// ** Default Props
Avatar.defaultProps = {
  tag: 'div',
  enablePreview: false
}
