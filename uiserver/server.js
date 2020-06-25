const express = require('express');
const proxy = require('express-request-proxy');
const app = express();

app.use(
    "/pages/:l1/:l2/:l3/:l4",
    proxy({
        cache: false,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1/:l2/:l3/:l4',
    })
);

app.use(
    "/pages/:l1/:l2/:l3",
    proxy({
        cache: false,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1/:l2/:l3',
    })
);

app.use(
    "/pages/:l1/:l2",
    proxy({
        cache: false,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1/:l2',
    })
);

app.use(
    "/pages/:l1",
    proxy({
        cache: false,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1',
    })
);

const greeting = 'Welcome to Microsoft Ads Smart Page! It seems you do not have a smart page created. Please create a new account or sign into https://ads.microsoft.com/ to start!';

app.use('/', function (req, res) {
    res.send(greeting);
});

app.use('/home', function (req, res) {
    res.send(greeting);
});

app.listen(process.env.PORT || 8080, function () {});
