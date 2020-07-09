const express = require('express');
const proxy = require('express-request-proxy');
const newProxy = require('express-http-proxy');
const cors = require('cors');
const fs = require('fs');
const http = require('http');
const https = require('https');
const request = require('request');
const config = require('config');
const { domain } = require('process');

const HttpPort = config.get('HttpPort');
const HttpsPort = config.get('HttpsPort');

const Cdn = config.get('Cdn');
const Host = config.get('Host');
const FullCustomDomain = config.get('FullCustomDomain');
const CustomDomain = config.get('CustomDomain');
const TrafficManager = config.get('TrafficManager');

const customDomainMap = {};
const subdomainMap = {};

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

async function getCustomDomainMapping(domain) {
    // when do we update existing mapping cache and return early? 
    if (!customDomainMap[domain]) {
        const url = `${Cdn}customdomainmapping/${domain}.txt`;
        // update cache
        customDomainMap[domain] = await getTxtContent(url);
    }
    return customDomainMap[domain];
}

async function getSubdomainMapping(domain) {
    // when do we update existing mapping cache and return early? 
    if (!subdomainMap[domain]) {
        const url = `${Cdn}subdomainmapping/${domain}.txt`;
        // update cache
        subdomainMap[domain] = await getTxtContent(url);
    }
    return subdomainMap[domain];
}

async function getSiteVersion(site) {
    const url = `${Cdn}sites/${site}/version.txt`;
    return await getTxtContent(url);
}

async function getSharedResourceVersion() {
    const url = `${Cdn}common/version.txt`;
    return await getTxtContent(url);
}

async function populateHtml(site) {
    // get shared version
    const sharedResourceVersion = await getSharedResourceVersion();

    // get template index html
    var indexHtml = await getTxtContent(`${Cdn}common/${sharedResourceVersion}/index.html`);

    // update common js inside html
    indexHtml = indexHtml.replace('%%main.js%%', `${Cdn}common/${sharedResourceVersion}/main.js`);

    // update site config js inside html
    const siteVersion = await getSiteVersion(site);
    // for some reason replaceAll was not recognized
    indexHtml = indexHtml.replace('%%config.js%%', `${Cdn}sites/${site}/${siteVersion}/config.js`);
    indexHtml = indexHtml.replace('%%config.js%%', `${Cdn}sites/${site}/${siteVersion}/config.js`);

    // console.log(`index.html ${indexHtml}`);

    return indexHtml;
}

// only support l1 request if user is accessing by host or by custom domain
// e.g. www.smartpage.com/site1
// or smartpage.centralus.cloudapp.azure.net/site1
app.use('/:l1', function (req, res) {
    console.log(`handling request ${req.headers.host}${req.originalUrl}`);
    if (req.headers.host === FullCustomDomain
        || req.headers.host === Host
        || req.headers.host === TrafficManager
    ) {
        // user is accessing www.smartpage.com/site1
        populateHtml(req.originalUrl.substring(1)).then((indexHtml) => {
            res.send(indexHtml);
        });
    }
    else {
        res.status(404).send();
    }
    //  else {
    //     console.log(`handling request ${req.headers.host}${req.originalUrl}`);
    //     if (req.originalUrl !== '/favicon.ico') {
    //         populateHtml(req.originalUrl.substring(1)).then((indexHtml) => {
    //             res.send(indexHtml);
    //         });
    //     } else {
    //         res.status(404).send();
    //     }
    // }
});

// only allow root access from subdomain, or domain from customer
// e.g. cars.smartpage.com, in this case we need to find the mapping
// between cars -> site1 and return content from site1
// or www.cars.com, we need to find the mapping www.cars.com -> site2
app.use('/', function (req, res) {
    console.log(`handling request ${req.headers.host}${req.originalUrl}`);
    if (req.headers.host !== FullCustomDomain && req.headers.host.indexOf(CustomDomain) > 0) {
        const subdomain = req.headers.host.substring(0, req.headers.host.indexOf(CustomDomain) - 1);
        getSubdomainMapping(subdomain).then((site) => {
            populateHtml(site).then((indexHtml) => {
                res.send(indexHtml);
            });
        });
    }
    // custom domain
    else if (req.headers.host !== Host) {
        getCustomDomainMapping(req.headers.host).then((site) => {
            populateHtml(site).then((indexHtml) => {
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
