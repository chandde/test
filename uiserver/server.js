const express = require('express');
const proxy = require('express-request-proxy');
const newProxy = require('express-http-proxy');
const cors = require('cors');
const fs = require('fs');
const http = require('http');
const https = require('https');
const request = require('request');

const app = express();

// const SERVER = 'bingadssmartpagetest1.centralus.cloudapp.azure.com';
const CDN = "https://bingadssmartpagetest2.azureedge.net/";

const HOST = 'bingadssmartpagetest2.centralus.cloudapp.azure.com';

app.use(cors());

app.use('/*', newProxy(CDN, {
    proxyReqPathResolver: function (req) {
        return new Promise((resolve, reject) => {
            if (req.headers.host === HOST) {
                resolve(req.originalUrl);
            } else {
                var mapSource = "https://bingadssmartpagetest2.azureedge.net/common/custom-domain-mapping.json";
                request(mapSource, (error, response, body) => {
                    if (response.statusCode === 200) {
                        console.log(`original mapping downloaded: ${body}`);
                        var mapping = JSON.parse(body);
                        console.log(`req.originalUrl: ${req.originalUrl}`); // /123?x=1&y=2
                        // console.log(req._parsedUrl.search); // ?x=1&y=2
                        const customDomain = mapping[req.headers.host];
                        console.log(`customDomain: ${customDomain}`);
                        const redirectPath = (customDomain || '') + req.originalUrl;
                        console.log(`redirectPath = ${redirectPath}`);
                        resolve(redirectPath);
                    } else {
                        reject();
                    }
                });
            }
        });
    }
}));

// app.listen(process.env.PORT || 80, function () { });
var httpServer = http.createServer(app);
httpServer.listen(80);

var privateKey = fs.readFileSync('./my.key', 'utf8');
var certificate = fs.readFileSync('./my.crt', 'utf8');
var credentials = { key: privateKey, cert: certificate };
var httpsServer = https.createServer(credentials, app);
httpsServer.listen(443);
