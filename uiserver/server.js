const express = require('express');
const proxy = require('express-request-proxy');
const newProxy = require('express-http-proxy');
const app = express();

// const SERVER = 'bingadssmartpagetest1.centralus.cloudapp.azure.com';
const CDN = "https://bingadssmartpagetest.azureedge.net/"

const customDomainMap = {
    "www.test.com": "/server1",
    "www.chandlerdeng.com": "/server1",
    'bingadssmartpagetest1.centralus.cloudapp.azure.com': '', // for the site itself, do not add ex
};

function mapDomain (req) {
    console.log(req.originalUrl); // /123?x=1&y=2
    console.log(req._parsedUrl.search); // ?x=1&y=2
    const customDomain = customDomainMap[req.headers.host];
    console.log(customDomain);
    const redirectPath = customDomain + req.originalUrl;
    console.log(redirectPath);
    return redirectPath;
}
app.use('/*', newProxy(CDN, {
    proxyReqPathResolver: mapDomain
}));

app.listen(process.env.PORT || 80, function () { });