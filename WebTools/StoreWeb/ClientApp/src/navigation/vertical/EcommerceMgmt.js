import { Airplay, Circle, MoreHorizontal } from 'react-feather'

export default [
    {
        id: 'EcommerceMgmt',
        title: '電商系統',
        icon: <Airplay size={20} />,
        children: [
            {
                id: 'goodsManage',
                title: '商品管理',
                children: [
                    {
                        id: 1084,
                        title: '平台商品管理',
                        navLink: '/EcommerceMgmt/PublishGoods',
                        action: 'read',
                        resource: '1084'
                    }
                ]
            },
            {
                id: 'systemSetting',
                title: '電商系統設定',
                children: [
                    {
                        id: 1083,
                        title: '電商門店增修',
                        navLink: '/EcommerceMgmt/EcommerceStoreEdit',
                        action: 'read',
                        resource: '1083'
                    }
                ]
            }
        ]
    }
]
