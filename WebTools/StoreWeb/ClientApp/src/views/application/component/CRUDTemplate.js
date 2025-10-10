// ** Third Party Components
import axios from 'axios'
import { showMessageBox } from '@CityAppHelper'
import { ShowToast } from '@CityAppExtComponents/caToaster'

export const MessageBoxTemplate = ({ viewTitle = '', jobTitle, doConfirm, doCancel }) => {
  showMessageBox({
    title: `確認${jobTitle}?`,
    icon: 'warning',
    confirmButtonText: '確認',
    showCancelButton: true,
    cancelButtonText: '取消',
    customClass: {
      confirmButton: 'btn btn-primary me-1',
      cancelButton: 'btn btn-outline-danger'
    },
    buttonsStyling: false
  }).then(function (result) {
    if (result.value) {
      doConfirm?.()
      ShowToast(viewTitle, `${jobTitle}完成`, 'info')
    } else {
      doCancel?.()
    }
  })
}

export const CRUDTemplateDefault = ({ viewTitle = '', jobTitle, request, apiPath, doStart, doError, doSuccess, debug }) => {
  if (debug) console.log(`#DoCRUDTemplate# Preview>>> ${apiPath} request=`, request)

  showMessageBox({
    title: `確認${jobTitle}?`,
    icon: 'warning',
    confirmButtonText: '確認',
    showCancelButton: true,
    cancelButtonText: '取消',
    customClass: {
      confirmButton: 'btn btn-primary me-1',
      cancelButton: 'btn btn-outline-danger'
    },
    buttonsStyling: false
  }).then(function (result) {
    if (result.value) {
      doStart?.()
      axios.post(apiPath, request)
        .then(response => {
          if (debug) console.log(`#DoCRUDTemplate# response<<< ${apiPath} response=`, response.data)
          doSuccess?.(response.data.data)
          ShowToast(viewTitle, `${jobTitle}完成`, 'info')
        }).catch(error => {
          if (debug) console.log(`#DoCRUDTemplate# <<< ${apiPath} error=`, error.response ? error.response.data : error.message)
          doError?.()
          console.log(error.response)
          showMessageBox({
            title: error?.response?.data?.data?.msg || `${jobTitle}失敗`,
            icon: 'warning',
            customClass: {
              confirmButton: 'btn btn-primary'
            },
            buttonsStyling: false
          })
        })
    } else {
      //cancel
    }
  })
}

export const CRUDTemplateSilence = ({ request, apiPath, doStart, doError, doSuccess, debug }) => {
  if (debug) console.log(`#DoCRUDTemplate# Preview >>> ${apiPath} request=`, request)
  doStart?.()
  axios.post(apiPath, request)
    .then(response => {
      if (debug) console.log(`#DoCRUDTemplate# response <<< ${apiPath} response=`, response.data)
      doSuccess?.(response.data.data)
    }).catch(error => {
      if (debug) console.log(`#DoCRUDTemplate# response <<< ${apiPath} error=`, error.response ? error.response.data : error.message)
      doError?.(error.response ? error.response.data : error.message)
    })
}
