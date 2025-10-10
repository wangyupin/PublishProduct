import axios from 'axios'
import React, { createContext, useContext, useState, useEffect } from 'react'
import { getUserData } from '@utils'

const SystemSetting = createContext()

export const SystemSettingContext = ({ children }) => {
    const [setting, setSetting] = useState({})
    const [loading, setLoading] = useState(true)
    const [currentUser, setCurrentUser] = useState(JSON.parse(localStorage.getItem('userData'))?.userId || null || null)

    const fetchSetting = async () => {
        try {
            await axios.post('/api/SystemSetting/GetSettings', {
                userID: getUserData()?.userId
            }).then((response) => {
                setSetting(response.data.data.settings || {})

            })
        } catch (error) {
            console.error('Failed to fetch setting:', error)
        } finally {
            setLoading(false)
        }
    }

    const refreshSetting = async () => {
        setLoading(true)
        await fetchSetting()
    }

    useEffect(() => {

        const handleUserChange = () => {
            const newUserId = JSON.parse(localStorage.getItem('userData'))?.userId || null
            if (newUserId !== currentUser) {
                setCurrentUser(newUserId)
                refreshSetting()
            }
        }

        window.addEventListener('userChanged', handleUserChange)

        return () => {
            window.removeEventListener('userChanged', handleUserChange)
        }
    }, [currentUser])

    useEffect(() => {
        fetchSetting()
    }, [])

    return (
        <SystemSetting.Provider value={{ setting, loading, refreshSetting }}>
            {children}
        </SystemSetting.Provider>
    )
}

export const useSystemSetting = () => {
    const context = useContext(SystemSetting)
    if (!context) {
        throw new Error('useSystemSetting must be used within SystemSettingProvider')
    }
    return context
}
