import { Link } from 'react-router-dom'
import React, { Fragment, useState } from 'react'
import {
    Row,
    Col,
    Card,
    Input,
    Button,
    DropdownMenu,
    DropdownItem,
    DropdownToggle,
    UncontrolledDropdown,
    Nav,
    NavItem,
    InputGroup,
    InputGroupText
} from 'reactstrap'
import { Share, Grid, Search, MoreHorizontal } from 'react-feather'
import AdvanceSearch from './AdvanceSearch'
import DualListModal from '@application/component/DualListModal'
import { useTranslation } from 'react-i18next'

const CustomGridHeader = React.memo(({ handleFilter, searchTerm, access, handleExcel, handleAdd, addLink, columnFilter, advanceRequest, handleAdvanceSearch, selectedRowsLength, handleDelete, handleSelectedExcel, dualListComponent, children }) => {
    const { t } = useTranslation('common')
    const [show, setShow] = useState(false)

    /*
       search: searchTerm handleFunc
       to excel 
       add
       advance search: columns initialValue, handleFunc
       select: delete, export
    */
    return (
        <div className='position-relative rdt_TableHeader' >
            <div className={selectedRowsLength ? 'grid-context-menu-visible' : 'grid-context-menu-hidden'} style={{ borderRadius: '4px 4px 0 0' }}>
                <div className='d-flex align-items-center flex-grow-1'>
                    <span>{t('selectedLabel', { count: selectedRowsLength })}</span>
                    <div className='ms-auto d-flex '>
                        {access.export &&
                            <UncontrolledDropdown className='me-1'>
                                <DropdownToggle color='secondary' caret outline>
                                    <Share className='font-small-4 me-50' />
                                    <span className='align-middle'>{t('export')}</span>
                                </DropdownToggle>
                                <DropdownMenu>
                                    <DropdownItem className='w-100' onClick={handleSelectedExcel}>
                                        <Grid className='font-small-4 me-50' />
                                        <span className='align-middle'>{t('excel')}</span>
                                    </DropdownItem>
                                </DropdownMenu>
                            </UncontrolledDropdown>
                        }
                        {access.export &&
                            <Button key="delete" className='me-1' onClick={handleDelete}>{t('delete')}</Button>
                        }
                    </div>
                </div>
            </div>
            <div style={{ borderRadius: '4px 4px 0 0' }} >
                <div className='w-100 me-1 ms-50 mt-2 mb-2' style={{ fontSize: '1rem', paddingRight: '8px' }}>
                    <Row className='m-0'>
                        <Col sm='6' className='d-flex align-items-center p-0'>
                            <InputGroup className='ms-1' style={{ width: 'auto' }}>
                                <InputGroupText className='border-end-0'>
                                    <Search size={14} />
                                </InputGroupText>
                                <Input
                                    id='search-user'
                                    type='text'
                                    className='border-start-0'
                                    value={searchTerm}
                                    onChange={e => handleFilter(e.target.value)}
                                    placeholder={t('searchPlaceholder')}
                                />
                                <InputGroupText onClick={() => setShow(!show)} tag='button'>
                                    <MoreHorizontal size={14} />
                                </InputGroupText>
                            </InputGroup>
                            {children}
                        </Col>
                        <Col
                            sm='6'
                            className='d-flex align-items-center justify-content-sm-end justify-content-start flex-sm-nowrap p-0 mt-sm-0 mt-1 ms-sm-0 ms-1'
                        >
                            <div className='d-flex align-items-center table-header-actions'>
                                {dualListComponent && access?.field &&
                                    <Button.Ripple className='me-1' color='flat-secondary' onClick={dualListComponent.toggle}>
                                        {t('fieldAdjust')}
                                    </Button.Ripple>}
                                {access.export &&
                                    <UncontrolledDropdown className='me-1'>
                                        <DropdownToggle color='secondary' caret outline>
                                            <Share className='font-small-4 me-50' />
                                            <span className='align-middle'>{t('export')}</span>
                                        </DropdownToggle>
                                        <DropdownMenu>
                                            <DropdownItem className='w-100' onClick={handleExcel} >
                                                <Grid className='font-small-4 me-50' />
                                                <span className='align-middle'>{t('excel')}</span>
                                            </DropdownItem>

                                        </DropdownMenu>
                                    </UncontrolledDropdown>
                                }
                                {/* 新增 */}
                                {access.create &&
                                    <Link to={addLink} onClick={handleAdd}>
                                        <Button className='add-new-user me-1' color='primary' >
                                            {t('add')}
                                        </Button>
                                    </Link>
                                }
                            </div>
                        </Col>
                    </Row>
                    <AdvanceSearch show={show} setShow={setShow} columnFilter={columnFilter} advanceRequest={advanceRequest} handleAdvanceSearch={handleAdvanceSearch} />
                    {dualListComponent && <DualListModal options={dualListComponent.options} open={dualListComponent.open} toggle={dualListComponent.toggle} saveSelectedColumn={dualListComponent.saveSelectedColumn} columnDefs={dualListComponent.columnDefs} defaultSelectedColumn={dualListComponent.defaultSelectedColumn} />}

                </div >
            </div>
        </div>
    )
})


export default CustomGridHeader
