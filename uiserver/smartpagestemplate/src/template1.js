import React from 'react';
import PropTypes from 'prop-types';

export class Template1 extends React.Component {
  render() {
    document.title = this.props.config.title;
    const style = {
      display: "inline-flex",
    };
    return (
      <div className="smart-page-template1" style={style}>
        <h1>{this.props.config.content.header}</h1>
        <div className="image">
          <img src={this.props.config.content.image}></img>
        </div>
      </div>
    );
  }
}

Template1.PropTypes = {
  config: PropTypes.object.isRequired,
};