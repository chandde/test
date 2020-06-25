import _ from 'lodash';
import React from 'react';
import ReactDOM from 'react-dom';
import $ from 'jquery';

// const config = require('./config.json');

// import config from './config.json';

// import config from 'config';

// import(
//     /* webpackChunkName: "config.json" */
//     /* webpackMode: "lazy" */
//     './config.json'
// );

import { Template1 } from './template1';
import { Template2 } from './template2';


// import configPath from './config.json';

// console.log(languagesFileUrl);  // This just returns the URL of your JSON file. For example: '/languages.json'. You should then fetch the file via AJAX.

// import('./config.json').then(({ default: config }) => {
$.getJSON('./config.json').then((config) => {
    // const config = res.json();
    var template;
    switch (config.layout) {
        case "template1":
            template = <Template1 config={config} />;
            break;
        case "template2":
            template = <Template2 config={config} />;
            break;
        default:
            throw "undefined template";
    };

    ReactDOM.render(template, document.getElementById('root'));
});
