import React from 'react';
import $ from 'jquery';
// import hljs from 'highlight.js/lib/highlight';
// import javascript from 'highlight.js/lib/languages/javascript';
// import cpp from 'highlight.js/lib/languages/cpp';
// import csharp from 'highlight.js/lib/languages/csharp';
// import java from 'highlight.js/lib/languages/java';
// import python from 'highlight.js/lib/languages/python';

// hljs.registerLanguage('javascript', javascript);
// hljs.registerLanguage('cpp', cpp);
// hljs.registerLanguage('csharp', csharp);
// hljs.registerLanguage('java', java);
// hljs.registerLanguage('python', python);

export class Dialog extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      text: undefined,
      style: 'plaintext',
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
    // $('.inputTextArea').each(function(i, block) {
    //   hljs.highlightBlock(block);
    // });
  }

  styleChanged(e) {
    const languageMap = {
      text: 'plaintext',
      javascript: 'language-js',
      'c++': 'language-cpp',
      'c#': 'language-cs',
      java: 'language-java',
    };
    
    if (languageMap[e.target.innerText] !== this.state.style) {
      this.setState({ style: languageMap[e.target.innerText] });
    }
  }

  render() {    
    return (<div>
      <div className="wsConnectionStatusDiv">
        connecting...
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