import React from 'react'
import {
    Input,
    Nav,
    NavItem
} from 'reactstrap'
import ReactPaginate from 'react-paginate'
import { useTranslation } from 'react-i18next'
import { NumberCommasFormat } from '@CityAppHelper'

const CustomGridPagination = React.memo(({ total, rowsPerPage, handlePerPage, handlePagination, currentPage, rowPerPageShow = true }) => {
    const { t } = useTranslation('common', { keyPrefix: 'pagination' })
    const count = Number(Math.ceil(total / rowsPerPage))
    return (
        <div className='rdt_TableFooter'>
            <div style={{ borderRadius: '0 0 4px 4px', border: 'solid 1px transparent' }}>
                <Nav className='align-items-center justify-content-end'>
                    <NavItem className='ps-2 me-auto'>
                        <div className='d-flex align-items-center w-100 me-auto'>{`${t('labelTotal')}${NumberCommasFormat(total)}`}</div>
                    </NavItem>

                    <NavItem>
                        {rowPerPageShow && (
                            <div className='d-flex align-items-center w-100 me-1'>
                                {/* <label htmlFor='rows-per-page'>{t('labelStart')}</label> */}
                                <Input
                                    className='mx-50'
                                    type='select'
                                    id='rows-per-page'
                                    value={rowsPerPage}
                                    onChange={handlePerPage}
                                    style={{ width: '8rem' }}
                                >
                                    <option value='10' label={`10 ${t('labelStart')}/${t('labelEnd')}`}>10</option>
                                    <option value='25' label={`25 ${t('labelStart')}/${t('labelEnd')}`}>25</option>
                                    <option value='50' label={`50 ${t('labelStart')}/${t('labelEnd')}`}>50</option>
                                </Input>
                                {/* <label htmlFor='rows-per-page'>{t('labelEnd')}</label> */}
                            </div>
                        )}
                    </NavItem>

                    <ReactPaginate
                        previousLabel={''}
                        nextLabel={''}
                        pageCount={count || 1}
                        activeClassName='active'
                        forcePage={currentPage !== 0 ? currentPage - 1 : 0}
                        onPageChange={page => handlePagination(page)}
                        pageClassName={'page-item'}
                        nextLinkClassName={'page-link'}
                        nextClassName={'page-item next'}
                        previousClassName={'page-item prev'}
                        previousLinkClassName={'page-link'}
                        pageLinkClassName={'page-link'}
                        breakClassName={'page-item'}
                        breakLinkClassName={'page-link'}
                        containerClassName={'pagination react-paginate justify-content-end my-2 pe-1'}
                    />
                </Nav>
            </div>
        </div>
    )
})

export default CustomGridPagination