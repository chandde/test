import React  from 'react';
import $ from 'jquery';

export class Dialog extends React.Component {
  constructor(props) {
    super(props);

    this.ws = new WebSocket('ws://localhost:4010/');
    this.ws.onopen = () => {
      $('.wsConnectionStatusDiv').innerText = 'connected';
    };
    // this.onmessage = (event) => console.log('message from server: ', event.data);
  }

  onInput(e) {
    this.ws.send(e.target.value);
  }

  render() {
    return (<div>
      <div className="wsConnectionStatusDiv">
        connecting...        
      </div>
      <textarea className='myDialog' onInput={this.onInput} /></div>);
  }
}