// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { clearSlice } from '@store/rootReducer'

const sliceName = 'WorkMgmt_PunchJob'

const initialState = () => {
    return {
        status: {
            index: {},
            clearSliceFlag: false
        }
    }
}

export const slice = createSlice({
    name: sliceName,
    initialState: initialState(),
    reducers: {
        savePageStatus(state, { payload }) {
            const { key, status } = payload
            if (key === 'status' && state.status.clearSliceFlag) {
                state.status.clearSliceFlag = false
            } else {
                state[key] = {
                    ...state[key],
                    ...status
                }
            }
        },
        resetContent(state, { payload }) {
            Object.assign(state, initialState())
        }
    },
    extraReducers: builder => {
        builder
            //clearSlice
            .addCase(clearSlice, (state, action) => {
                if (action.payload === sliceName) {
                    slice.caseReducers.resetContent(state, { payload: { key: 'all' } })
                    state.status.clearSliceFlag = true
                }
            })
    }
})


export const { savePageStatus, resetContent } = slice.actions
export default slice.reducer
