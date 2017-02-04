const path               = require('path');
const CommonsChunkPlugin = require('webpack/lib/optimize/CommonsChunkPlugin');
const CopyWebpackPlugin  = require('copy-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const DefinePlugin = require('webpack/lib/DefinePlugin');

const ENV = process.env.NODE_ENV = 'development';
const HOST = process.env.HOST || 'localhost';
const PORT = process.env.PORT || 8080;

const metadata = {
  env: ENV,
  host: HOST,
  port: PORT
}
module.exports = {
  entry: {
    'main'  : path.resolve(__dirname, './client/main.ts'),
    'vendor': path.resolve(__dirname,'./client/vendor.ts')
  },
  output: {
    path: path.resolve(__dirname, './public/dist'),
    publicPath: '/dist/',
    filename: 'bundle.js'
  },
  plugins: [
    new CleanWebpackPlugin(['dist'], {
      root: path.resolve(__dirname, './public'),
      verbose: true, 
      dry: false,
      exclude: ['shared.js']
    }),
    new CommonsChunkPlugin({ name: 'vendor', filename: 'vendor.bundle.js' }),
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
  devServer: {
    contentBase: path.resolve(__dirname, './client'),
    historyApiFallback: true
  },
  devtool: 'source-map'
};