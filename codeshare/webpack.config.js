const HtmlWebpackPlugin = require('html-webpack-plugin'); //installed via npm
const webpack = require('webpack'); //to access built-in plugins
const MiniCssExtractPlugin = require('mini-css-extract-plugin')

module.exports = {
  entry: {
    main: './src/index.js',
    newSession: './src/new-session-button.js',
  },
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
      {
        test: /\.scss|css$/,
        use: [
            { loader: MiniCssExtractPlugin.loader },
            'css-loader',
            'sass-loader'
        ],
      },
      // { test: /\.ts$/, use: 'ts-loader' }
    ],
  },
  optimization: {
    minimize: false
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: '[name].css',
    }),
  ]
};