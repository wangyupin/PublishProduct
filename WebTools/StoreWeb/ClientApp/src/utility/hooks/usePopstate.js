import { useEffect } from 'react'
import { useDispatch } from 'react-redux'

export const usePopstate = ({
    onBack,
    dispatchAction
}) => {
    const dispatch = useDispatch()

    useEffect(() => {
        const handleBackButton = (event) => {
            if (onBack) {
                onBack(event)
            }

            if (dispatchAction) {
                dispatch(dispatchAction)
            }
        }

        window.addEventListener("popstate", handleBackButton)

        return () => {
            window.removeEventListener("popstate", handleBackButton)
        }
    }, [dispatch])
}

export default usePopstate
