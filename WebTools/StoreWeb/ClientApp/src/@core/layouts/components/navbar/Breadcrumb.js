/* eslint-disable */
import { Fragment } from 'react'
import useBreadcrumbs from 'use-react-router-breadcrumbs'
import { Link } from 'react-router-dom'
import { Breadcrumb, BreadcrumbItem } from 'reactstrap'
import { Routes } from '../../../../router/routes'

const BreadcrumbComponent2 = ({ breadcrumbs }) => {
  return (
    <Breadcrumb>
      {breadcrumbs.map(({ breadcrumb }, index) => {
        const active = index === breadcrumbs.length - 1
        return (
          <BreadcrumbItem tag='li' key={index} className={active ? 'text-secondary' : 'text-primary'}>
            {breadcrumb}
          </BreadcrumbItem>
        )
      })}
    </Breadcrumb>
  )
}

const Breadcrumbs = () => {
  const breadcrumbs = useBreadcrumbs(Routes)

  return (
    < Fragment >
      <BreadcrumbComponent2 breadcrumbs={breadcrumbs} />
    </Fragment >
  )
}

// const Breadcrumbs = withBreadcrumbs(routes, { excludePaths: ['/'] })(({ breadcrumbs }) => (
//   <div className='d-flex'>
//     {breadcrumbs.map(({
//       match,
//       breadcrumb
//     }, index) => (
//       <span key={match.url}>
//         {breadItem(index, match, breadcrumb)}
//       </span>
//     ))}
//   </div>
// ))

export default Breadcrumbs