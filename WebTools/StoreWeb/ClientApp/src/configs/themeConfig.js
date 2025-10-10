// You can customize the template with the help of this file

//Template config options
const themeConfig = {
  app: {
    appName: 'WebPos',
    appLogoImage: '/gbtech.ico',
    companyName: '旌泓股份有限公司',
    companyLocation: {
      latitude: 25.0886782,
      longitude: 121.4896698
    },
    licenseFor: '亞億科技有限公司'
  },
  layout: {
    isRTL: false,
    skin: 'light', // light, dark, bordered, semi-dark
    routerTransition: 'fadeIn', // fadeIn, fadeInLeft, zoomIn, none or check this for more transition https://animate.style/
    type: 'vertical', // vertical, horizontal
    contentWidth: 'full', // full, boxed
    menu: {
      isHidden: false,
      isCollapsed: true
    },
    navbar: {
      // ? For horizontal menu, navbar type will work for navMenu type
      type: 'sticky', // static , sticky , floating, hidden
      backgroundColor: 'white' // BS color options [primary, success, etc]
    },
    footer: {
      type: 'hidden' // static, sticky, hidden
    },
    customizer: false,
    scrollTop: true // Enable scroll to top button
  }
}

export default themeConfig
