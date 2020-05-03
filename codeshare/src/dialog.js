import React  from 'react';
import $ from 'jquery';

export class Dialog extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      text: undefined,
    };

    this.ws = new WebSocket(`ws://localhost:4000${window.location.pathname}`);
    this.ws.onopen = () => {
      const statusel = $('.wsConnectionStatusDiv')
      statusel.text('websocket connected');
    };
    this.ws.onmessage = (event) => {
      console.log('message from server: ', event.data)
      this.setState({
        text: event.data,
      });
    };
  }

  componentDidMount() {
    $('.inputTextArea').height(window.innerHeight - 80);
    $('.inputTextArea').width(window.innerWidth - 80);
  }

  onInput(e) {
    this.ws.send(e.target.value);
    this.setState({
      text: e.target.value,
    });
  }

  render() {
    return (<div>
      <div className="wsConnectionStatusDiv">
        connecting...        
      </div>
      <textarea className='inputTextArea' onInput={this.onInput.bind(this)} value={this.state.text}/></div>);
  }
}