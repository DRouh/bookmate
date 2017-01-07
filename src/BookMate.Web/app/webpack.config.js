const path = require('path');
const webpack = require('webpack');
const CommonsChunkPlugin = require('webpack/lib/optimize/CommonsChunkPlugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const DefinePlugin = require('webpack/lib/DefinePlugin');

const ENV = process.env.NODE_ENV = 'development';
const BASEPATH = process.env.BASEPATH = 'BookMate.Web';
const HOST = process.env.HOST || 'localhost';
const PORT = process.env.PORT || 8080;

const metadata = {
  env: ENV,
  basepath: BASEPATH,
  host: HOST,
  port: PORT
}

module.exports = {
  devServer: {
    contentBase: 'src',
    historyApiFallback: true,
    host: metadata.host,
    baseURL: 'BookMate.Web',
    port: metadata.port
  },
  entry: {
    'main': './src/main.ts',
    'vendor': './src/vendor.ts'
  },
  output: {
    path: '../app_built',
    filename: 'bundle.js'
  },
  plugins: [
    new CommonsChunkPlugin({ name: 'vendor', filename: 'vendor.bundle.js' }),
    new CopyWebpackPlugin([{ from: './src/index.html', to: '../app_built/index.html' }]),
    new DefinePlugin({'webpack': {'ENV': JSON.stringify(metadata.env), 'BASEPATH': JSON.stringify(metadata.basepath)}})
  ],
  resolve: {
    extensions: ['', '.ts', '.js']
  },
  module: {
    loaders: [
      { test: /\.html$/, loader: 'raw' },
      {
        test: /\.ts$/,
        loaders: ['awesome-typescript-loader', 'angular2-template-loader'],
        exclude: [/\.(spec|e2e)\.ts$/]
      }
    ],
    noParse: [path.join(__dirname, 'node_modules', 'angular2', 'bundles')]
  },
  devtool: 'source-map'
};