import React, { createContext, useContext, useState } from 'react'
import UILoader from '@components/ui-loader'

const UILoaderContext = createContext()

export const UILoaderProvider = ({ children }) => {
    const [blocking, setBlocking] = useState(false)

    const withLoading = async (action) => {
        setBlocking(true)
        try {
            const result = await action()
            return result
        } finally {
            setBlocking(false)
        }
    }

    return (
        <UILoaderContext.Provider value={{ blocking, withLoading }}>
            <UILoader blocking={blocking} classname='cityapp full-screen-uiloader'>
                {children}
            </UILoader>

        </UILoaderContext.Provider>
    )
}

export const useUILoader = () => {
    const context = useContext(UILoaderContext)
    if (!context) {
        throw new Error('useUILoader must be used within UILoaderProvider')
    }
    return context.withLoading
}
