const express = require('express');
const proxy = require('express-request-proxy');
const newProxy = require('express-http-proxy');
const cors = require('cors');
const fs = require('fs');
const http = require('http');
const https = require('https');
const request = require('request');
const config = require('config');

const CDN = config.get('Cdn');
const HOST = config.get('Host');
const HttpPort = config.get('HttpPort');
const HttpsPort = config.get('HttpsPort');

const domainMap = {};

const app = express();
// allow cross origin request so client can request CDN content directly and bypass Web Server
app.use(cors());

function getTxtContent(url) {
    return new Promise((resolve, reject) => {
        request(url, (error, response, body) => {
            var content = '';
            if (response && response.statusCode === 200) {
                content = body;
            }
            // console.log(`url ${url} returned content ${content}`);
            resolve(content);
        });
    });
}

async function getDomainMapping(domain) {
    // when do we update existing mapping cache and return early? 
    if (!domainMap[domain]) {
        const url = `${CDN}domainmapping/${domain}.txt`;
        // update cache
        domainMap[domain] = await getTxtContent(url);
    }
    return domainMap[domain];
}

async function getSiteVersion(site) {
    const url = `${CDN}sites/${site}/version.txt`;
    return await getTxtContent(url);
}

async function getSharedResourceVersion() {
    const url = `${CDN}common/version.txt`;
    return await getTxtContent(url);
}

async function populateHtml(site) {
    // get shared version
    const sharedResourceVersion = await getSharedResourceVersion();

    // get template index html
    var indexHtml = await getTxtContent(`${CDN}common/${sharedResourceVersion}/index.html`);

    // update common js inside html
    indexHtml = indexHtml.replace('%%main.js%%', `${CDN}common/${sharedResourceVersion}/main.js`);

    // update site config js inside html
    const siteVersion = await getSiteVersion(site);
    // for some reason replaceAll was not recognized
    indexHtml = indexHtml.replace('%%config.js%%', `${CDN}sites/${site}/${siteVersion}/config.js`);
    indexHtml = indexHtml.replace('%%config.js%%', `${CDN}sites/${site}/${siteVersion}/config.js`);

    // console.log(`index.html ${indexHtml}`);

    return indexHtml;
}

app.use('/:l1', function (req, res) {
    console.log(`handling request ${req.headers.host}${red.originalUrl}`);
    if (req.originalUrl !== '/favicon.ico') {
        populateHtml(req.originalUrl.substring(1)).then((indexHtml) => {
            res.set('Content-Type', 'text/html');
            res.send(indexHtml);
        });
    } else {
        res.status(404).send();
    }
});

app.use('/', function (req, res) {
    console.log(`handling request ${req.headers.host}${red.originalUrl}`);
    // only handle root request if it's from custom domain
    if (req.headers.host !== HOST) {
        getDomainMapping(req.headers.host).then((site) => {
            populateHtml(site).then((indexHtml) => {
                res.set('Content-Type', 'text/html');
                res.send(indexHtml);
            });
        });
    } else {
        res.status(404).send();
    }
});

// app.listen(process.env.PORT || 80, function () { });
var httpServer = http.createServer(app);
httpServer.listen(HttpPort);

var privateKey = fs.readFileSync('./my.key', 'utf8');
var certificate = fs.readFileSync('./my.crt', 'utf8');
var credentials = { key: privateKey, cert: certificate };
var httpsServer = https.createServer(credentials, app);
httpsServer.listen(HttpsPort);
