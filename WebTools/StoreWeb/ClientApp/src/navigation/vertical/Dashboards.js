import { Home, Radio, Tool, Circle, MoreHorizontal } from 'react-feather'
export default [
  {
    id: 'dashboards',
    title: '戰情室',
    icon: <Radio size={20} />,
    badge: 'light-success',
    badgeText: '客制only',
    navLink: '/Dashboard',
    action: 'read',
    resource: '戰情室',
    children: [
      {
        id: 'Revenue',
        title: '銷售戰報',
        navLink: '/Dashboard'
      },
      {
        id: 1030,
        title: '公告作業',
        navLink: '/dashboard/revenue/Announcement',
        action: 'read',
        resource: '公告作業'
      }
    ]
  }
]
