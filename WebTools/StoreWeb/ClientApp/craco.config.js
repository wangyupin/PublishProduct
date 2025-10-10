const path = require('path')

module.exports = {
  reactScriptsVersion: 'react-scripts',
  style: {
    sass: {
      loaderOptions: {
        sassOptions: {
          includePaths: ['node_modules', 'src/assets']
        }
      }
    },
    postcss: {
      plugins: [require('postcss-rtl')()]
    }
  },
  experimental: { reactRefresh: true },
  webpack: {
    // devServer: {
    //   https: true,
    //   // other settings
    // },
    alias: {
      '@src': path.resolve(__dirname, 'src'),
      '@assets': path.resolve(__dirname, 'src/@core/assets'),
      '@components': path.resolve(__dirname, 'src/@core/components'),
      '@layouts': path.resolve(__dirname, 'src/@core/layouts'),
      '@store': path.resolve(__dirname, 'src/redux'),
      '@styles': path.resolve(__dirname, 'src/@core/scss'),
      '@configs': path.resolve(__dirname, 'src/configs'),
      '@utils': path.resolve(__dirname, 'src/utility/Utils'),
      '@hooks': path.resolve(__dirname, 'src/utility/hooks'),
      '@context': path.resolve(__dirname, 'src/utility/context'),

      '@srcAssets': path.resolve(__dirname, 'src/assets'),
      '@CityAppHelper': path.resolve(__dirname, 'src/utility/CityAppHelper'),
      '@CityAppComponents': path.resolve(__dirname, 'src/utility/CityAppComponents'),
      '@CityAppExtComponents': path.resolve(__dirname, 'src/utility/CityApp'),
      '@CityAppCompanySettings': path.resolve(__dirname, 'src/utility/CityAppCompanySettings'),
      '@application': path.resolve(__dirname, 'src/views/application')
    },
    configure: (webpackConfig, { env, paths }) => {
      // 處理 .mjs 文件
      webpackConfig.module.rules.push({
        test: /\.mjs$/,
        include: /node_modules/,
        type: 'javascript/auto',
      });

      return webpackConfig;
    },
  }
}
