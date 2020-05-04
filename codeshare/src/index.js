import 'react-hot-loader/patch';
import 'jquery';
import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { Dialog } from './dialog';
// export { NewSessionButton } from "./newSession";

ReactDOM.render(
  <AppContainer>
    <Dialog />
  </AppContainer>,
  document.querySelector('.app-root')
);

if (module.hot) {
  module.hot.accept();
}

// export {
//   Dialog,
//   NewSessionButton,
// };