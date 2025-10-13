import { Database, Circle, MoreHorizontal } from 'react-feather'

export default [
  {
    id: 'MainTableMgmt',
    title: '主檔系統',
    icon: <Database size={20} />,
    children: [
      {
        id: 4,
        title: '商品資料增修',
        navLink: '/MainTableMgmt/GoodsInfo',
        action: 'read',
        resource: '4'
      }
    ]
  }
]
