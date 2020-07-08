import React from 'react';
import PropTypes from 'prop-types';

export class Template2 extends React.Component {
  render() {
    document.title = this.props.config.title;
    const style = {
      display: "block",
    };
    return (
      <div className="smart-page-template2" style={style}>
        <div className="image">
          <img src={this.props.config.content.image}></img>
        </div>
        <h1>{this.props.config.content.header}</h1>
      </div>
    );
  }
}

Template2.PropTypes = {
  config: PropTypes.object.isRequired,
};