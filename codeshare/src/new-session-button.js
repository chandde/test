import $ from 'jquery';
import 'react-hot-loader/patch';
import 'jquery';
import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';

export class NewSessionButton extends React.Component {
    generateNewSessionAndGo() {
        const response = $.ajax({
            type: 'POST',
            url: `http://${window.location.host}/newsession`,
            success: (data) => {
                console.log(data);
                window.location.assign(`/pad?id=${data.newSessionId}`);
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