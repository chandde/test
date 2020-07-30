import React from 'react';
import PropTypes from 'prop-types';


export class Template1 extends React.Component {
  render() {
    document.title = this.props.config.SmartPageProperties.Name;
    const style = {
    };
    return (
      <div className="smart-page-template1" style={style}>
        <h1>{`Name = ${this.props.config.SmartPageProperties.Name}`}</h1>
        <br/>
        <h1>{`Desc = ${this.props.config.SmartPageProperties.Description}`}</h1>
        <br/>
        <h2>{`Subdomain = ${this.props.config.SmartPageProperties.SubDomain}`}</h2>
        <br/>
        <div className="image">
          {/* <img src={this.props.config.content.image}></img> */}
        </div>
      </div>
    );
  }
}

Template1.PropTypes = {
  config: PropTypes.object.isRequired,
};