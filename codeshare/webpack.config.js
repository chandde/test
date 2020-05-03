const HtmlWebpackPlugin = require('html-webpack-plugin'); //installed via npm
const webpack = require('webpack'); //to access built-in plugins

module.exports = {
  module: {
    rules: [
      {
        test: /\.(js|mjs|jsx|ts|tsx)$/,
        // include: paths.appSrc,
        loader: require.resolve('babel-loader'),
        // test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        // use: {
        //   loader: "babel-loader"
        // },
        query: {
          presets: ['@babel/react'] // @babel/preset-es2015
        }
      },
      { test: /\.txt$/, use: 'raw-loader' },
      // { test: /\.ts$/, use: 'ts-loader' }
    ]
  },
  optimization: {
    minimize: false
  },
  // plugins: [
  //   new HtmlWebpackPlugin({template: './dist/index.html'})
  // ]
};