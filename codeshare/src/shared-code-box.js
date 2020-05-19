import React from 'react';
import $ from 'jquery';

export class SharedCodeBox extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      statusText: 'connecting...',
      style: 'plaintext',
    };

    this.ws = new WebSocket(`ws://${window.location.host}/wss${window.location.pathname}`);
    this.ws.onopen = () => {
      this.setState({
        statusText: 'connected',
      });
    };
    this.ws.onmessage = (event) => {
      console.log('message from server: ', event.data)
      this.setState({
        text: event.data,
      });
    };
    this.ws.onclose = () => {
      this.setState({
        statusText: 'disconnected',
      });
    }
    this.ws.onerror = (err) => {
      this.setState({
        statusText: JSON.stringify(err),
      });      
    }
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

  // styleChanged(e) {
  //   const languageMap = {
  //     text: 'plaintext',
  //     javascript: 'language-js',
  //     'c++': 'language-cpp',
  //     'c#': 'language-cs',
  //     java: 'language-java',
  //   };
    
  //   if (languageMap[e.target.innerText] !== this.state.style) {
  //     this.setState({ style: languageMap[e.target.innerText] });
  //   }
  // }

  render() {    
    return (<div>
      <div className="wsConnectionStatusDiv">
        {this.state.statusText}
      </div>
      <div className="codeStyleingControlButtons">
        {/* <button onClick={this.styleChanged.bind(this)}>text</button>
        <button onClick={this.styleChanged.bind(this)}>javascript</button>
        <button onClick={this.styleChanged.bind(this)}>c++</button>
        <button onClick={this.styleChanged.bind(this)}>c#</button>
        <button onClick={this.styleChanged.bind(this)}>java</button> */}
      </div>
      <textarea
        className='inputTextArea' // {`inputTextArea ${this.state.style}`}
        onInput={this.onInput.bind(this)}
        value={this.state.text}
        id='textbox'
      />
    </div>
    );
  }
}