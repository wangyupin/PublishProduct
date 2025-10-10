import axios from 'axios'

//** Vuexy components
import jwtDefaultConfig from './jwtDefaultConfig'

// ** CityApp Utilty
import { showMessageBox } from '@CityAppHelper'

export default class JwtService {
  // ** jwtConfig <= Will be used by this service
  jwtConfig = { ...jwtDefaultConfig }

  // ** For Refreshing Token
  isAlreadyFetchingAccessToken = false

  // ** For Refreshing Token
  subscribers = []

  requestPool = {}

  constructor(jwtOverrideConfig) {
    this.jwtConfig = { ...this.jwtConfig, ...jwtOverrideConfig }

    // ** Request Interceptor
    axios.interceptors.request.use(
      config => {
        if (!(config.url.includes(this.jwtConfig.loginEndpoint) ||
          config.url.includes(this.jwtConfig.refreshEndpoint) ||
          config.url.includes(this.jwtConfig.appInfoEndpoint)
        )) {
          // ** Get token from localStorage
          const accessToken = this.getToken()

          // ** If token is present add it to request's Authorization Header
          if (accessToken) {
            // ** eslint-disable-next-line no-param-reassign
            config.headers.Authorization = `${this.jwtConfig.tokenType} ${accessToken}`
          }
        }

        if (this.requestPool[config.url]) {
          const controller = this.requestPool[config.url]
          delete this.requestPool[config.url]
          controller.abort()
        }

        const controller = new AbortController()
        config.signal = controller.signal
        this.requestPool[config.url] = controller
        return config
      },
      error => Promise.reject(error)
    )

    // ** Add request/response interceptor
    axios.interceptors.response.use(
      response => response,
      error => {
        // ** const { config, response: { status } } = error
        const { config, response, message } = error
        const originalRequest = config

        // ** if (status === 401) {
        if (response && response.status === 401) {
          if (!this.isAlreadyFetchingAccessToken) {
            this.isAlreadyFetchingAccessToken = true
            this.refreshToken().then(r => {
              this.isAlreadyFetchingAccessToken = false

              // ** Update accessToken in localStorage
              this.setToken(r.data.accessToken)
              this.setRefreshToken(r.data.refreshToken)

              this.onAccessTokenFetched(r.data.accessToken)
            })
          }
          const retryOriginalRequest = new Promise(resolve => {
            this.addSubscriber(accessToken => {
              // ** Make sure to assign accessToken according to your response.
              // ** Check: https://pixinvent.ticksy.com/ticket/2413870
              // ** Change Authorization header
              originalRequest.headers.Authorization = `${this.jwtConfig.tokenType} ${accessToken}`
              // resolve(this.axios(originalRequest))
              resolve(axios(originalRequest))
            })
          })
          return retryOriginalRequest
        } else if (response && response.status === 440) {
          // 440 登入逾時，請重新登入
          // localStorage.removeItem('userData')

          showMessageBox({
            title: '登入逾時',
            text: "請重新登入",
            icon: 'warning',
            customClass: {
              confirmButton: 'btn btn-primary'
            },
            buttonsStyling: false
          }).then(func => {
            this.clearLocalStorage(config)
          })
        } else if (response && response.status === 441) {
          // 441 本帳號已從其它裝置登入。
          // useDispatch(handleLogout())

          showMessageBox({
            title: '即將登出',
            text: "本帳號已從其它裝置登入",
            icon: 'warning',
            customClass: {
              confirmButton: 'btn btn-primary'
            },
            buttonsStyling: false
          }).then(func => {
            this.clearLocalStorage(config)
          })
        } else if (response && response.status === 500) {
          // 500 登入逾時，請重新登入
          // showMessageBox({
          //   title: '即將登出-code 500',
          //   text: "本帳號已從其它裝置登入",
          //   icon: 'warning',
          //   customClass: {
          //     confirmButton: 'btn btn-primary'
          //   },
          //   buttonsStyling: false
          // }).then(func => {
          //   this.clearLocalStorage(config)
          // })
          this.clearLocalStorage(config)
          // this.clearLocalStorage()
        } else if (response && response.status === 404) {
          // Handle by each request. 2022/6/29 
          // return Promise.reject({
          //   data: {
          //     data: { msg: Object.values(response.data.errors)[0][0] },
          //     succeeded: false
          //   }
          // })
        } else if (response && response.status === 400) {
          // Handle by each request. 2022/6/29 
          // .net DataAnnotations error handle
          if (response.data.errors && Object.values(response.data.errors).length && Object.values(response.data.errors)[0][0]) {
            return Promise.reject({
              response: {
                data: {
                  data: { msg: Object.values(response.data.errors)[0][0] },
                  succeeded: false
                }
              }
            })
          } else if (error.response.data.data?.msg) {

          } else {
            showMessageBox({
              title: response.data.data?.msg || '通訊異常, 400',
              icon: 'error',
              customClass: {
                confirmButton: 'btn btn-primary'
              },
              buttonsStyling: false
            }).then(func => {

            })
          }
        } else {
          if (!message === 'canceled') {
            showMessageBox({
              title: `通訊異常，${response.status}`,
              text: "請重試",
              icon: 'error',
              customClass: {
                confirmButton: 'btn btn-primary'
              },
              buttonsStyling: false
            }).then(func => {

            })
          }

        }
        return Promise.reject(error)
      }
    )
  }

  clearLocalStorage(config) {
    localStorage.removeItem('userData')
    localStorage.removeItem('templateData')
    localStorage.removeItem('menuCollapsed')
    localStorage.removeItem(config.storageTokenKeyName || 'accessToken')
    localStorage.removeItem(config.storageRefreshTokenKeyName || 'refreshToken')
    localStorage.removeItem('machineSet')
    document.location.href = "/login"
  }

  onAccessTokenFetched(accessToken) {
    this.subscribers = this.subscribers.filter(callback => callback(accessToken))
  }

  addSubscriber(callback) {
    this.subscribers.push(callback)
  }

  getToken() {
    if (this.isAlreadyFetchingAccessToken) {
      return null
    } else {
      return localStorage.getItem(this.jwtConfig.storageTokenKeyName)
    }
  }

  getRefreshToken() {
    return localStorage.getItem(this.jwtConfig.storageRefreshTokenKeyName)
  }

  setToken(value) {
    localStorage.setItem(this.jwtConfig.storageTokenKeyName, value)
  }

  setRefreshToken(value) {
    localStorage.setItem(this.jwtConfig.storageRefreshTokenKeyName, value)
  }

  login(...args) {
    return axios.post(this.jwtConfig.loginEndpoint, ...args)
  }

  logout(...args) {
    return axios.post(this.jwtConfig.logoutEndpoint, ...args)
  }

  register(...args) {
    return axios.post(this.jwtConfig.registerEndpoint, ...args)
  }

  getAppInfo(...args) {
    return axios.get(this.jwtConfig.appInfoEndpoint, ...args)
  }

  refreshToken() {
    return axios.post(this.jwtConfig.refreshEndpoint, {
      refreshToken: this.getRefreshToken()
    })

    // axios({
    //   url: this.jwtConfig.refreshEndpoint,
    //   method: 'POST',
    //   transformRequest: [
    //     (data, headers) => { 
    //       delete headers.common.Authorization
    //       return JSON.stringify({refreshToken: this.getRefreshToken()})
    //     }
    //   ]
    // })

    // axios.post(this.jwtConfig.refreshEndpoint, {
    //   transformRequest: [
    //     (date, headers) => {
    //       delete headers.common.Authorization
    //       refreshToken = this.getRefreshToken()
    //       return refreshToken
    //     }
    //   ]
    // })
  }
}
