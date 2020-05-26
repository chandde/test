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
      text: '',
    };

    this.createWsConnection();
  }

  createWsConnection() {
    // 4000 is the port of nginx websocket proxy
    const id = new URLSearchParams(window.location.search).get('id');
    if (!id) {
      return;
    }

    this.ws = new WebSocket(`ws://${window.location.host}:4000/wss/${id}`);
    // if (!this.ws) {
    //   this.setState({
    //     statusText: 'Fail to connect',
    //   });
    // }

    this.ws.onopen = () => {
      console.log('this.ws.onopen');
      this.setState({
        statusText: 'connected',
      });
    };
    this.ws.onmessage = (event) => {
      console.log('this.ws.onmessage: ', event.data)
      this.setState({
        text: event.data,
      });
    };
    this.ws.onerror = _.debounce((err) => {
      console.log('this.ws.onerror');
      this.setState({
        statusText: JSON.stringify(err),
      });
    }, 500);
    this.ws.onclose = _.debounce(() => {
      console.log('this.ws.onclose');
      // auto reconnect on close
      this.createWsConnection();
    }, 500);
  }

  handleResize() {
    this.setState({
      width: window.innerWidth - 50,
      height: window.innerHeight - 50,
    });
  }

  componentDidMount() {
    window.addEventListener('resize', _.debounce(this.handleResize.bind(this), 50));
  }

  onInput(e) {
    // calculate changes
    const oldText = this.state.text;
    const newText = e.target.value;

    // let left = 0;
    // const minLength = Math.min(oldText.length, newText.length);

    // for (; left < minLength; left++) {
    //   if (oldText[left] !== newText[left]) {
    //     break;
    //   }
    // }

    // let oldright = oldText.length;
    // let newright = newText.length;

    // for (; oldright >= left && newright >= left; oldright--, newright--) {
    //   if (oldText[oldright] !== newText[newright]) {
    //     break;
    //   }
    // }

    // const payload = {
    //   start: left,
    //   length: oldright - left + 1,
    //   change: newText.substring(left, newright - left + 1),
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
        onInput={this.onInput.bind(this)}
        value={this.state.text}
        id='textbox'
        style={{ height: this.state.height, width: this.state.width }}
      />
    </div>
    );
  }
}