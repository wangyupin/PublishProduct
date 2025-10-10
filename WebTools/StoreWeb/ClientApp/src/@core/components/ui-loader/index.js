// ** React Imports
import { Fragment } from 'react'

// ** Third Party Components
import Proptypes from 'prop-types'
import classnames from 'classnames'

// ** Reactstrap Imports
import { Spinner } from 'reactstrap'

// ** Styles
import './ui-loader.scss'

const UILoader = ({
  children,
  blocking = false,
  loader = <Spinner color='primary' />,
  className,
  tag = 'div',
  overlayColor
}) => {

  const Tag = tag

  return (
    <Tag className={classnames('ui-loader', { [className]: className, show: blocking })}>
      {children}
      {blocking ? (
        <Fragment>
          <div
            className='overlay'
            {...(blocking && overlayColor ? { style: { backgroundColor: overlayColor } } : {})}
          ></div>
          <div className='loader'>{loader}</div>
        </Fragment>
      ) : null}
    </Tag>
  )
}

export default UILoader

UILoader.propTypes = {
  tag: Proptypes.string,
  loader: Proptypes.any,
  className: Proptypes.string,
  overlayColor: Proptypes.string,
  blocking: Proptypes.bool.isRequired
}
