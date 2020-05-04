import $ from 'jquery';
import 'react-hot-loader/patch';
import 'jquery';
import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { Dialog } from './dialog';
// export { NewSessionButton } from "./newSession";

export class NewSessionButton extends React.Component {
    generateNewSessionAndGo() {
        const response = $.ajax({
            type: 'POST',
            url: 'http://localhost:4000/newsession',
            success: (data) => {
                console.log(data);
                window.location.assign(`/${data.newSessionId}`);
            },
        });
    }

    render() {
        return <button
            onClick={this.generateNewSessionAndGo}>
                Let's go
        </button>
    }
}

ReactDOM.render(
  <AppContainer>
    <NewSessionButton />
  </AppContainer>,
  document.querySelector('.newSessionButtonWrapper')
);

if (module.hot) {
  module.hot.accept();
}