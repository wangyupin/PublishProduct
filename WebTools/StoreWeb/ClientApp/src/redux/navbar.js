// ** Redux Imports
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'

// ** Axios Imports
import axios from 'axios'
import getMainMenu from '@src/navigation/vertical'
import { json } from 'react-router-dom'

export const getBookmarks = createAsyncThunk('layout/getBookmarks', async (params) => {
  const response = await axios.post('/api/machineSet/GetBookRemark', params)
  return {
    bookmarks: response.data?.data?.result || []
  }
})

export const updateBookmarked = createAsyncThunk('layout/updateBookmarked', async (params) => {
  await axios.post('/api/machineSet/UpdBookRemark', params)
  return params
})

export const layoutSlice = createSlice({
  name: 'navbar',
  initialState: {
    query: '',
    bookmarks: [],
    suggestions: [],
    breadcrumbsActive: [{ label: `首頁`, key: '/Dashboard' }],
    activeTab: { label: '', key: '/Dashboard' }
  },
  reducers: {
    handleSearchQuery: (state, action) => {
      state.query = action.payload
    },
    updateSuggestion: (state, action) => {
      state.suggestions = action.payload
    },
    updateBreadcrumbsActive: (state, action) => {
      const { title, navLink, duplicate, targetIndex } = action.payload
      if (!duplicate) {
        const tabData = [...state.breadcrumbsActive, { label: title, key: navLink }]
        state.breadcrumbsActive = tabData
      } else {
        state.breadcrumbsActive[targetIndex].key = navLink
      }
    },
    deleteBreadcrumbsActive: (state, action) => {
      const tagIndex = action.payload
      const filterData = state.breadcrumbsActive.filter((x, index) => x.key !== tagIndex)
      state.breadcrumbsActive = filterData
    },
    updateActiveTab: (state, action) => {
      const active = action.payload
      state.activeTab.key = active
    }
  },
  extraReducers: builder => {
    builder
      .addCase(getBookmarks.fulfilled, (state, action) => {
        const bookmarks = action.payload.bookmarks
        state.bookmarks = state.suggestions.filter(item => bookmarks.some(item1 => item1.id === item.id))?.map(item => ({ ...item, isBookmarked: true })) || []
        state.suggestions = state.suggestions.map(item => ({ ...item, isBookmarked: bookmarks.some(item1 => item1.id === item.id) }))
      })
      .addCase(updateBookmarked.fulfilled, (state, action) => {
        let objectToUpdate

        // ** find & update object
        state.suggestions.find(item => {
          if (item.id === action.payload.programID) {
            item.isBookmarked = !item.isBookmarked
            objectToUpdate = item
          }
        })

        // ** Get index to add or remove bookmark from array
        const bookmarkIndex = state.bookmarks.findIndex(x => x.id === action.payload.programID)

        if (bookmarkIndex === -1) {
          state.bookmarks.push(objectToUpdate)
        } else {
          state.bookmarks.splice(bookmarkIndex, 1)
        }
      })
  }
})

export const { handleSearchQuery, updateSuggestion, updateBreadcrumbsActive, deleteBreadcrumbsActive, updateActiveTab } = layoutSlice.actions

export default layoutSlice.reducer
