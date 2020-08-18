function log(...message) {
    // turn off on PROD env
    // console.log(...message);
}

log("smart page server is running");

const express = require('express');
// const proxy = require('express-request-proxy');
// const newProxy = require('express-http-proxy');
// const cors = require('cors');
// const fs = require('fs');
const http = require('http');
// const https = require('https');
const request = require('request');
const config = require('config');
// const { domain } = require('process');
// const httpToHttps = require('express-http-to-https');

// https://docs.microsoft.com/en-us/javascript/api/overview/azure/keyvault-certificates-readme?view=azure-node-latest#getting-a-key-vault-certificate
const { DefaultAzureCredential } = require("@azure/identity");
const { SecretClient } = require("@azure/keyvault-secrets");

async function getCert() {
    const credential = new DefaultAzureCredential();
    const url = `https://${config.get('SecretKeyVaultName')}.vault.azure.net`;
    const client = new SecretClient(url, credential);
    const secretName = config.get('SecretName');

    return await client.getSecret(secretName);
}

const HttpPort = 8080; // config.get('HttpPort');
const HttpsPort = process.env.PORT || config.get('HttpsPort');
log(`https listening on port ${HttpsPort}`);

const Cdn = config.get('Cdn');
const Host = config.get('Host');
const Blob = config.get('Blob');
const WWWDomain = config.get('WWWDomain');
const NakedDomain = config.get('NakedDomain');
const TrafficManager = config.get('TrafficManager');

const app = express();
// allow cross origin request so client can request CDN content directly and bypass Web Server
// app.use(cors());
// app.use(httpToHttps.redirectToHTTPS(httpToHttps.ignoreHosts, httpToHttps.ignoreRoutes));

function getTxtContent(url) {
    return new Promise((resolve, reject) => {
        request(url, (error, response, body) => {
            var content = '';
            if (response && response.statusCode === 200) {
                content = body;
            }
            // log(`url ${url} returned content ${content}`);
            resolve(content);
        });
    });
}

async function getSiteVersion(site) {
    const url = `${Blob}pages/${site}/version.txt`;
    return await getTxtContent(url);
}

async function getSharedResourceVersion() {
    const url = `${Blob}common/version.txt`;
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
    indexHtml = indexHtml.replace('%%config.js%%', `${Cdn}pages/${site}/${siteVersion}/config.js`);
    indexHtml = indexHtml.replace('%%config.js%%', `${Cdn}pages/${site}/${siteVersion}/config.js`);

    log(`index.html ${indexHtml}`);

    return indexHtml;
}

// only support l1 request if user is accessing by host or by custom domain
// e.g. www.smartpage.com/site1
// or smartpage.centralus.cloudapp.azure.net/site1
// or contoso.smartpage.com/suffix
app.use('/:l1', function (req, res) {
    log(`handling request ${req.headers.host}${req.originalUrl}`);
    if (req.headers.host === WWWDomain
        || req.headers.host === Host
        || req.headers.host === TrafficManager
        || req.headers.host.indexOf(NakedDomain) > 0
    ) {
        // user is accessing x.smartpage.com/y
        const subdomain = req.headers.host.substring(0, req.headers.host.indexOf(NakedDomain) - 1);
        const suffix = req.originalUrl.substring(1);
        populateHtml(subdomain + '/' + suffix).then((indexHtml) => {
            res.send(indexHtml);
        });
    }
    else {
        res.status(404).send("Page not found!");
    }
    // }
});

// only allow root access from subdomain, or domain from customer
// e.g. cars.smartpage.com, in this case we need to find the mapping
// between cars -> site1 and return content from site1
// or www.cars.com, we need to find the mapping www.cars.com -> site2
app.use('/', function (req, res) {
    log(`handling request ${req.headers.host}${req.originalUrl}`);
    // log(req);
    if (req.headers.host !== WWWDomain && req.headers.host.indexOf(NakedDomain) > 0) {
        const subdomain = req.headers.host.substring(0, req.headers.host.indexOf(NakedDomain) - 1);
        // getSubdomainMapping(subdomain).then((site) => {
        populateHtml(subdomain).then((indexHtml) => {
            res.send(indexHtml);
        });
    } else {
        res.status(404).send("Page not found!");
    }
});

var httpServer = http.createServer(app);
httpServer.listen(HttpPort);

// getCert().then((certificate) => {
//     // certificate.value is BASE64, we need to convert it into binary first
//     const bCert = new Buffer(certificate.value, 'base64');
//     var options = {
//         pfx: bCert, // fs.readFileSync('./keyvaultwildcard.pfx'),
//         passphrase: '',
//     };
//     var httpsServer = https.createServer(options, app);
//     httpsServer.listen(HttpsPort);
// });