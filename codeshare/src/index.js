import 'react-hot-loader/patch';
import 'jquery';
import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { Dialog } from './dialog';

ReactDOM.render(
  <AppContainer>
    <Dialog />
  </AppContainer>,
  document.querySelector('.app-root')
);


if (module.hot) {
  module.hot.accept();
}
