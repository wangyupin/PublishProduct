// ** Utils
import { isUserLoggedIn } from '@utils'

// ** CityApp Utilty
import { useInterval, getFullDateTimeTW } from '@CityAppHelper'

// ** Store & Actions
import { useSelector, useDispatch } from 'react-redux'
import { UpdHeartbeat } from '@application/store/appSettings'
import { useEffect, useRef, useState } from 'react'

// export const useHeartbeatService = () => {
//     const dispatch = useDispatch()

//     const callback = () => {
//         console.log(` heartbeat =>${getFullDateTimeTW()}`)

//         if (isUserLoggedIn()) {
//             dispatch(UpdHeartbeat({
//                 sellBranch: "010", 
//                 terminalID: "a",
//                 Status: "0000"
//             }))
//         } else {
//             // disable heartbeat and logout....
//             console.log(` heartbeat disabled => ${getFullDateTimeTW()} `)
//             return -1
//         }
//     }
//     // callback()

//     if (isUserLoggedIn()) {
//         console.log(` heartbeat start =>${getFullDateTimeTW()}`)
//         useInterval(callback, 5000)
//     } 

//   }

export const useHeartbeatService = (action) => {
    const dispatch = useDispatch()

    const heartbeatEnabledRef = useRef(true)
    const setHeartbeatEnabledHander = (enabled) => {
        // setHeartbeatEnabled(enabled)
        heartbeatEnabledRef.current = enabled
    }

    const callback = () => {
        console.log(` heartbeat =>${getFullDateTimeTW()}`)
        if (heartbeatEnabledRef.current) {
            if (isUserLoggedIn()) {
                dispatch(UpdHeartbeat({
                    sellBranch: "010",
                    terminalID: "a",
                    Status: "0000"
                }))
            } else {
                // disable heartbeat and logout....
                console.log(` heartbeat disabled -> user logout=> ${getFullDateTimeTW()} `)
                return -1
            }
        } else {
            // disable heartbeat and logout....
            console.log(` heartbeat disabled -> setHeartbeatEnabledRef => ${getFullDateTimeTW()} `)
            return -1
        }
    }
    // callback()

    if (isUserLoggedIn() && heartbeatEnabledRef.current) {
        console.log(` heartbeat start =>${getFullDateTimeTW()}`)
        useInterval(callback, 60 * 1000)
    }

    return [setHeartbeatEnabledHander]
}

export const usePIDService = (action) => {
    const dispatch = useDispatch()

    const heartbeatEnabledRef = useRef(true)
    const setHeartbeatEnabledHander = (enabled) => {
        // setHeartbeatEnabled(enabled)
        heartbeatEnabledRef.current = enabled
    }

    const callback = () => {
        console.log(` PID heartbeat =>${getFullDateTimeTW()}`)
        if (heartbeatEnabledRef.current && action) {
            action()
        } else {
            // disable heartbeat and logout....
            console.log(` PID heartbeat disabled -> setHeartbeatEnabledRef => ${getFullDateTimeTW()} `)
            return -1
        }
    }
    // callback()

    if (isUserLoggedIn() && heartbeatEnabledRef.current) {
        console.log(` heartbeat start =>${getFullDateTimeTW()}`)
        useInterval(callback, 3 * 1000)
    }

    return [setHeartbeatEnabledHander]
}