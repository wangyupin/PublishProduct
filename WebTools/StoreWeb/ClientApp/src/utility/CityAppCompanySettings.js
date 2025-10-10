
// ** CityApp Utilty
import { getUserData } from '@utils'

export const getCompanyId = () => {
    return getUserData() ? getUserData().companyId.toUpperCase() : ''
}

export const isDemoSite = () => {
    const demoCompany = ['CITYAPP', '53222348']
    return demoCompany.indexOf(getCompanyId()) > -1
}

const companyList = [
    {
        name: "亞億科技有限公司",
        id: "53222348",
        label: "cityapp"

    },
    {
        name: "旌泓股份有限公司",
        id: "CITYAPP", //id: "82923777",
        label: "gbtech"

    },
    {
        name: "尚智運動世界",
        id: "05419237",
        label: "hotshoes"

    },
    {
        name: "依賞企業有限公司",
        id: "84627914",
        label: "hilltop"
    },
    {
        name: "東笙實業股份有限公司",
        id: "22738236",
        label: "magy"
    }
]

export const getCompanyLabel = () => {
    const companyId = getCompanyId()
    const company = companyList.find(item => item.id === companyId)
    return company ? company.label : ""
}
