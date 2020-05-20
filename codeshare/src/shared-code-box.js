import React from 'react';
import $ from 'jquery';
import _ from 'lodash';

export class SharedCodeBox extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      statusText: 'connecting...',
      style: 'plaintext',
      width: window.innerWidth - 50,
      height: window.innerHeight - 50,
    };

    this.createWsConnection();
  }

  createWsConnection() {
    this.ws = new WebSocket(`ws://${window.location.host}:4000/wss${window.location.pathname}`);
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
    this.ws.onerror = (err) => {
      this.setState({
        statusText: JSON.stringify(err),
      });      
    }    
    this.ws.onclose = () => {
      let countDown = 5000;
      const retryWorker = () => {
        if (countDown <= 0) {
          this.createWsConnection();
          return;
        }
        this.setState({
          statusText: `disconnected, reconnect in ${countDown/1000} seconds`,
        });
        countDown -= 1000;
        setTimeout(retryWorker, 1000);
      };
      setTimeout(retryWorker, 1000);
    }
  }
  
  handleResize() {
    this.setState({
      width: window.innerWidth - 50,
      height: window.innerHeight - 50,
    });
  }

  componentDidMount() {
    $('.inputTextArea').height(window.innerHeight - 80);
    $('.inputTextArea').width(window.innerWidth - 80);

    window.addEventListener('resize', _.debounce(this.handleResize.bind(this), 50));
  }
  
  calculateDiff(prev, current) {
    const length = Math.min(prev.length, current.length);
    let left = 0;
    for(; left < length.length; ++left) {
      if(prev[left] == current[left]) {
        continue;
      }
    }

    let right = length - 1;
    for(; right >= left; --right) {
      if(prev[right] == current[right + current.length - prev.length]) {
        continue;
      }
    }

    return longer.substring(left, right - left + 1);
  }

  onInput(e) {
    // const current = e.target.value;
    // const prev = this.state.text;
    // let diff;
    // if (current.length > prev.length) {
    //   diff = { change: this.calculateDiff(prev, current), op: 'Add' };
    // } else {
    //   diff = { change: this.calculateDiff(current, prev), op: 'Remove' };      
    // }
    this.ws.send(e.target.value);
    this.setState({
      text: e.target.value,
    });
  }

  onKeyPress(e) {
    console.log(e);
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
        onInput={_.debounce(this.onInput.bind(this), 1000)}
        // onKeyPress={this.onKeyPress.bind(this)}
        value={this.state.text}
        id='textbox'
        style={ { height: this.state.height, width : this.state.width } }
      />
    </div>
    );
  }
}