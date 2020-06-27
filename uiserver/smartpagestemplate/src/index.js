import _ from 'lodash';
import React from 'react';
import ReactDOM from 'react-dom';
import $ from 'jquery';

import { Template1 } from './template1';
import { Template2 } from './template2';

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
