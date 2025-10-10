import { useCallback } from "react"

// ** Checks if an object is empty (returns boolean)
export const isObjEmpty = obj => Object.keys(obj).length === 0

// ** Returns K format from a number
export const kFormatter = num => (num > 999 ? `${(num / 1000).toFixed(1)}k` : num)

// ** Converts HTML to string
export const htmlToString = html => html.replace(/<\/?[^>]+(>|$)/g, '')

// ** Checks if the passed date is today
const isToday = date => {
  const today = new Date()
  return (
    /* eslint-disable operator-linebreak */
    date.getDate() === today.getDate() &&
    date.getMonth() === today.getMonth() &&
    date.getFullYear() === today.getFullYear()
    /* eslint-enable */
  )
}

/**
 ** Format and return date in Humanize format
 ** Intl docs: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/DateTimeFormat/format
 ** Intl Constructor: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/DateTimeFormat/DateTimeFormat
 * @param {String} value date to format
 * @param {Object} formatting Intl object to format with
 */
export const formatDate = (value, formatting = { month: 'short', day: 'numeric', year: 'numeric' }) => {
  if (!value) return value
  return new Intl.DateTimeFormat('en-US', formatting).format(new Date(value))
}

// ** Returns short month of passed date
export const formatDateToMonthShort = (value, toTimeForCurrentDay = true) => {
  const date = new Date(value)
  let formatting = { month: 'short', day: 'numeric' }

  if (toTimeForCurrentDay && isToday(date)) {
    formatting = { hour: 'numeric', minute: 'numeric' }
  }

  return new Intl.DateTimeFormat('en-US', formatting).format(new Date(value))
}

/**
 ** Return if user is logged in
 ** This is completely up to you and how you want to store the token in your frontend application
 *  ? e.g. If you are using cookies to store the application please update this function
 */
export const isUserLoggedIn = () => localStorage.getItem('userData') && localStorage.getItem('machineSet')
export const getUserData = () => JSON.parse(localStorage.getItem('userData'))
export const getMachineSet = () => JSON.parse(localStorage.getItem('machineSet'))
export const getTemplateData = () => JSON.parse(localStorage.getItem('templateData'))
export const clearUserData = () => localStorage.clear()

/**
 ** This function is used for demo purpose route navigation
 ** In real app you won't need this function because your app will navigate to same route for each users regardless of ability
 ** Please note role field is just for showing purpose it's not used by anything in frontend
 ** We are checking role just for ease
 * ? NOTE: If you have different pages to navigate based on user ability then this function can be useful. However, you need to update it.
 * @param {String} userRole Role of user
 */
export const getHomeRouteForLoggedInUser = userRole => {
  if (userRole === 'admin') return '/'
  if (userRole === 'client') return '/AgGridMgmt/Practice'
  return '/login'
}

// ** React Select Theme Colors
export const selectThemeColors = theme => ({
  ...theme,
  colors: {
    ...theme.colors,
    primary25: '#7367f01a', // for option hover bg-color
    primary: '#7367f0', // for selected option bg-color
    neutral10: '#7367f0', // for tags bg-color
    neutral20: '#d8d6de', // for input border-color
    neutral30: '#d8d6de' // for input hover border-color
  }
})

export const selectStyle = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '36px',
    height: '36px'
  }),
  valueContainer: (baseStyles, state) => ({
    ...baseStyles,
    height: '32px',
    bottom: '1px'
  }),
  menu: (provided, state) => ({
    ...provided,
    zIndex: 7
  })
}

export const selectStyleNHL = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '36px'
  }),
  valueContainer: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '32px',
    bottom: '1px'
  }),
  menu: (provided, state) => ({
    ...provided,
    zIndex: 7
  })
}

export const selectStyleST = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '36px',
    height: '36px',
    borderTopRightRadius: '0 !important',
    borderBottomRightRadius: '0 !important'
  }),
  container: (baseStyles, state) => ({
    ...baseStyles,
    flex: '1 1 auto',
    width: '1%'
  }),
  valueContainer: (baseStyles, state) => ({
    ...baseStyles,
    height: '32px',
    bottom: '1px'
  })
}

export const selectStyleED = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '36px',
    height: '36px',
    marginLeft: '-1px',
    borderTopLeftRadius: '0 !important',
    borderBottomLeftRadius: '0 !important'
  }),
  container: (baseStyles, state) => ({
    ...baseStyles,
    flex: '1 1 auto',
    width: '1%'
  }),
  valueContainer: (baseStyles, state) => ({
    ...baseStyles,
    height: '32px',
    bottom: '1px'
  })
}

export const selectStyleSm = {
  control: (provided, state) => ({
    ...provided,
    minHeight: '32px',
    height: '32.13px'
  }),
  valueContainer: (provided, state) => ({
    ...provided,
    height: '30px',
    padding: '0 6px'
  }),

  input: (provided, state) => ({
    ...provided,
    margin: '0px'
  }),
  indicatorSeparator: state => ({
    display: 'none'
  }),
  indicators: (provided, state) => ({
    ...provided,
    height: '30px'
  }),
  indicatorsContainer: (provided, state) => ({
    ...provided,
    height: '30px'
  }),
  dropdownIndicator: (provided, state) => ({
    ...provided,
    padding: '4px'
  }),
  clearIndicator: (provided, state) => ({
    ...provided,
    padding: '4px'
  }),
  singleValue: (provided, state) => ({
    ...provided
    // fontSize: '0.857rem'
  }),
  placeholder: (provided, state) => ({
    ...provided
    // fontSize: '0.857rem'
  }),
  menu: (provided, state) => ({
    ...provided,
    zIndex: 7
    // fontSize: '0.857rem'
    // fontSize: '1rem'
  }),
  groupHeading: (provided, state) => ({
    ...provided
    // fontSize: '0.857rem'
  })
}

export const selectStyleSmST = {
  control: (provided, state) => ({
    ...provided,
    minHeight: '32px',
    height: '32.13px',
    borderTopRightRadius: '0 !important',
    borderBottomRightRadius: '0 !important'
  }),
  container: (baseStyles, state) => ({
    ...baseStyles,
    flex: '1 1 auto',
    width: '1%'
  }),
  valueContainer: (provided, state) => ({
    ...provided,
    height: '30px',
    padding: '0 6px'
  }),
  dropdownIndicator: (provided, state) => ({
    ...provided,
    padding: '4px'
  }),
  input: (provided, state) => ({
    ...provided,
    margin: '0px'
  }),
  indicatorSeparator: state => ({
    display: 'none'
  }),
  indicatorsContainer: (provided, state) => ({
    ...provided,
    height: '30px'
  }),
  singleValue: (provided, state) => ({
    ...provided
  }),
  placeholder: (provided, state) => ({
    ...provided
  }),
  menu: (provided, state) => ({
    ...provided,
    zIndex: 7
  }),
  groupHeading: (provided, state) => ({
    ...provided
  })
}

export const selectStyleSmED = {
  control: (provided, state) => ({
    ...provided,
    minHeight: '32px',
    height: '32.13px',
    marginLeft: '-1px',
    borderTopLeftRadius: '0 !important',
    borderBottomLeftRadius: '0 !important'
  }),
  container: (baseStyles, state) => ({
    ...baseStyles,
    flex: '1 1 auto',
    width: '1%'
  }),
  valueContainer: (provided, state) => ({
    ...provided,
    height: '30px',
    padding: '0 6px'
  }),
  dropdownIndicator: (provided, state) => ({
    ...provided,
    padding: '4px'
  }),
  input: (provided, state) => ({
    ...provided,
    margin: '0px'
  }),
  indicatorSeparator: state => ({
    display: 'none'
  }),
  indicatorsContainer: (provided, state) => ({
    ...provided,
    height: '30px'
  }),
  singleValue: (provided, state) => ({
    ...provided
  }),
  placeholder: (provided, state) => ({
    ...provided
  }),
  menu: (provided, state) => ({
    ...provided,
    zIndex: 7
  }),
  groupHeading: (provided, state) => ({
    ...provided
  })
}

export const selectStyleEDG = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '36px',
    height: '36px',
    border: 'none',
    backgroundColor: 'transparent'
  }),
  container: (baseStyles, state) => ({
    ...baseStyles,
    flex: '1 1 auto',
    width: '1%'
  }),
  valueContainer: (baseStyles, state) => ({
    ...baseStyles,
    height: '32px',
    bottom: '1px'
  })
}

export const selectStyleEDGOverflow = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    minHeight: '36px',
    height: '36px',
    border: 'none',
    backgroundColor: 'transparent'
  }),
  container: (baseStyles, state) => ({
    ...baseStyles,
    flex: '1 1 auto',
    width: '1%'
  }),
  valueContainer: (baseStyles, state) => ({
    ...baseStyles,
    height: '32px',
    bottom: '1px',
    overflow: 'auto'
  })
}

export const formatSelectOptionLabel = (option) => {
  return `${option.value} | ${option.label}`
}

export const formatSelectOptionLabelSTD = (option) => {
  return `${option.value} ${option.label}`
}

export const formatSelectOptionLabelNL = (option) => {
  return `${option.label}`
}
