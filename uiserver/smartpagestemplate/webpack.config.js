const webpack = require('webpack');
const CopyPlugin = require('copy-webpack-plugin');

module.exports = {
    // loaders: [
    // ],
    // entry: {
    //     'config.json': './src/config.json'
    // },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader"
                }
            },
            // {
            //     test: /config.json$/,
            //     use: {
            //         loader: 'file-loader?name=config.json!web-app-manifest-loader'
            //     }
            // }
            // {
            //     type: 'javascript/auto',
            //     test: /\.json$/,
            //     exclude: /node_modules/,
            //     use: [{
            //         loader: 'file-loader',
            //         options: { name: '[name].[ext]' },
            //     }],
            // }
            // {
            //     test: /\.json$/,
            //     loader: 'file-loader',
            //     type: 'javascript/auto',
            //     exclude: /node_modules/
            // },
        ]
    },
    optimization: {
        minimize: false
    },
    plugins: [
        // new webpack.IgnorePlugin(/^\.\/config\.json$/),
        // new CopyPlugin({
        //     patterns: [
        //         { from: 'src/config.json' }
        //     ],
        // }),
    ],
    // externals: {
    //     'config': 'JSON.stringify(require(\'./config.json\'))'// "require('./config.json')"
    // }
};