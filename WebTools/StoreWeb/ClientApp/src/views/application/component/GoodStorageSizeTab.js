// ** React Imports
import { useState, Fragment, useEffect } from 'react'

// ** Reactstrap Imports
import { TabContent, TabPane, Nav, NavItem, NavLink, Table } from 'reactstrap'

// ** View Components
const sizeNumList = [
  '01', '02', '03', '04', '05', '06', '07', '08', '09', '10',
  '11', '12', '13', '14', '15', '16', '17'
]

const NavTabList = ({ tabLists, activeTab, toggleTab }) => {
  return (
    <Nav tabs className='nav-left'>
      {tabLists.map((item, index) => (
        <NavItem key={index} >
          <NavLink
            active={activeTab === item.id}
            onClick={() => {
              toggleTab(item.id)
            }}
          >
            {item.label}
          </NavLink>
        </NavItem>
      ))}
    </Nav>
  )
}

const SizeTable = ({ rowData }) => {
  // ** Store Vars
  const cellStyleTrue = "fs-4"
  const cellStyleFalse = "text-danger fs-4"

  return (
    <Table responsive className="table table-sm table-bordered align-middle p-0 m-0">
      <thead className='table-dark text-center'>
        <tr>
          <th className='table-secondary fs-4'>序號</th>
          {sizeNumList.map((item, index) => (
            <th key={index}
              // className={
              //   (rowData[`StorageNum${item}`] ? (
              //     `${cellStyleTrue} ${(index % 2) ? '' : 'table-secondary'}`
              //   ) : (
              //     `${cellStyleFalse} ${(index % 2) ? '' : 'table-secondary'}`)
              //   )}>
              className='table-secondary fs-4'>
              {parseInt(item, 10)}
            </th>
          ))}
        </tr>
      </thead>
      <tbody className='text-center'>
        <tr >
          <td >尺碼</td>
          {sizeNumList.map((item, index) => (
            <td key={index}
              // className={
              //   (rowData[`StorageNum${item}`] ? (
              //     `${cellStyleTrue} ${(index % 2) ? '' : 'table-secondary'}`
              //   ) : (
              //     `${cellStyleFalse} ${(index % 2) ? '' : 'table-secondary'}`)
              //   )}>
              className='fs-4'
            >
              {rowData[`size${item}`]}</td>
          ))}
        </tr>
        <tr>
          <td >庫存</td>
          {
            sizeNumList.map((item, index) => (
              <td key={index}
                // className={
                //   (rowData[`StorageNum${item}`] ? (
                //     `${cellStyleTrue} ${(index % 2) ? '' : 'table-secondary'}`
                //   ) : (
                //     `${cellStyleFalse} ${(index % 2) ? '' : 'table-secondary'}`)
                //   )}>
                className={rowData[`storageNum${item}`] < 1 ? 'fs-4 text-danger' : 'fs-4'}>
                {rowData[`size${item}`] === '' ? '' : rowData[`storageNum${item}`]}</td>
            ))}
        </tr>
      </tbody>
    </Table>
  )
}

const TabContentList = ({ activeTab, goodStorageList }) => {
  return (
    <TabContent activeTab={activeTab} className='border border-warning p-0 x-0' >
      {goodStorageList.map((item, index) => {
        return (
          <TabPane key={index} tabId={`${index}`}>
            <SizeTable rowData={item} />
          </TabPane>
        )
      })}
    </TabContent>
  )
}

const GoodStorageSizeTab = ({ goodStorageList, activeSizeNo, vertical }) => {
  if (goodStorageList.length < 1) return null

  const tabLists = goodStorageList.map((item, index) => {
    return ({ label: `段碼 ${item.sizeNo}`, id: `${index}`, SizeNo: item.sizeNo })
  })

  // ** State
  const [active, setActive] = useState(tabLists[0].id)
  const toggle = tab => {
    if (active !== tab) {
      setActive(tab)
    }
  }

  useEffect(() => {
    const activeSizeNoIndex = tabLists.findIndex(item => item.sizeNo === activeSizeNo)
    setActive(activeSizeNoIndex < 0 ? tabLists[0].id : tabLists[activeSizeNoIndex].id)
  }, [activeSizeNo])

  return (
    // <div className='nav-vertical m-0'>
    <div className='nav mx-1'>
      {/* <div className='nav-vertical border border-info pe-1 pb-0'> */}
      {goodStorageList.length > 1 ? (
        <Fragment>
          <NavTabList tabLists={tabLists} activeTab={active} toggleTab={toggle} />
          <TabContentList activeTab={active} goodStorageList={goodStorageList} />
        </Fragment>
      ) : (
        <SizeTable rowData={goodStorageList[0]} />
      )}
    </div>
  )
}
export default GoodStorageSizeTab
