const express = require('express');
const proxy = require('express-request-proxy');
const newProxy = require('express-http-proxy');
const cors = require('cors');
var fs = require('fs');
// var http = require('http');
var https = require('https');

const app = express();

// const SERVER = 'bingadssmartpagetest1.centralus.cloudapp.azure.com';
const CDN = "https://bingadssmartpagetest2.azureedge.net/"

const customDomainMap = {
    "www.chandlerdeng.com": "/site1",
    'bingadssmartpagetest2.centralus.cloudapp.azure.com': '', // for the site itself, do not add ex
};

function mapDomain (req) {
    console.log(req.originalUrl); // /123?x=1&y=2
    console.log(req._parsedUrl.search); // ?x=1&y=2
    const customDomain = customDomainMap[req.headers.host];
    console.log(customDomain);
    const redirectPath = (customDomain || '') + req.originalUrl;
    console.log(redirectPath);
    return redirectPath;
}

app.use(cors());

app.use('/*', newProxy(CDN, {
    proxyReqPathResolver: mapDomain
}));

app.listen(process.env.PORT || 80, function () { });

var privateKey  = fs.readFileSync('./my.key', 'utf8');
var certificate = fs.readFileSync('./my.crt', 'utf8');
var credentials = {key: privateKey, cert: certificate};
var httpsServer = https.createServer(credentials, app);
// httpServer.listen(8080);
httpsServer.listen(443);
