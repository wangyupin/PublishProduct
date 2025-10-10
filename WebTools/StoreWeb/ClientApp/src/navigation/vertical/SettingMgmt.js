import { Settings, Circle, MoreHorizontal } from 'react-feather'
export default [
    {
        id: 'SettingMgmt',
        title: '系統支援',
        icon: <Settings size={20} />,
        children: [
            {
                id: 15,
                title: '使用者帳號',
                navLink: '/SettingMgmt/UserAccount',
                action: 'read',
                resource: '15'
            },
            {
                id: 16,
                title: '群組權限管理',
                navLink: '/SettingMgmt/GroupAccount',
                action: 'read',
                resource: '16'
            }
        ]
    }
]
