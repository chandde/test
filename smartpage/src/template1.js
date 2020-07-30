import React from 'react';
import PropTypes from 'prop-types';



const config1 = {
  "Id": 13743895347203,
  "Status": 109,
  "SmartPageProperties": {
      "SiteSuffix": "",
      "Name": "Testing 41a28252-7fb6-4f40-995f-857059350d19",
      "Description": "My first Smart Page test",
      "SubDomain": "smartpagesubdomain",
      "Attributes": {
          "key1": "value1"
      }
  },
  "SmartPageAssetIds": {
      "ImageIds": [1],
      "VideoIds": [3]
  },
  "Images": null
}

export class Template1 extends React.Component {
  render() {
    document.title = this.props.config.SmartPageProperties.Name;
    const style = {
      display: "inline-flex",
    };
    return (
      <div className="smart-page-template1" style={style}>
        <h1>{this.props.config.SmartPageProperties.Name}</h1>
        <br/>
        <h1>{this.props.config.SmartPageProperties.Description}</h1>
        <br/>
        <h2>{this.props.config.SmartPageProperties.SubDomain}</h2>
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