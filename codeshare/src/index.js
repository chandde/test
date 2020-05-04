import 'react-hot-loader/patch';
import 'jquery';
import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { SharedCodeBox } from './shared-code-box';

ReactDOM.render(
  <AppContainer>
    <SharedCodeBox />
  </AppContainer>,
  document.querySelector('.app-root')
);

if (module.hot) {
  module.hot.accept();
}
