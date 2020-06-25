const express = require('express');
const proxy = require('express-request-proxy');
const app = express();
// const port = 8000;

// redirect
// app.use('/pages/*', (req, res) => {
// redirect
// console.log(req.params);
// if (req.params['0'] && req.params['0'].length > 0) {
//     const redirectUrl = `https://chandler123456.z13.web.core.windows.net/${req.params['0']}/index.html`;
//     console.log(redirectUrl);
//     res.redirect(301, redirectUrl);
// } else {
//     res.send('It seems you do not have a smart page created. Please sign into https://ads.microsoft.com/ to start!');
// }
// });

// proxy

app.use(
    "/pages/:l1/:l2/:l3/:l4",
    proxy({
        cache: false, // redis.createClient(),
        // cacheMaxAge: 60,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1/:l2/:l3/:l4',
    })
);

app.use(
    "/pages/:l1/:l2/:l3",
    proxy({
        cache: false, // redis.createClient(),
        // cacheMaxAge: 60,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1/:l2/:l3',
    })
);

app.use(
    "/pages/:l1/:l2",
    proxy({
        cache: false, // redis.createClient(),
        // cacheMaxAge: 60,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1/:l2',
    })
);

app.use(
    "/pages/:l1",
    proxy({
        cache: false, // redis.createClient(),
        // cacheMaxAge: 60,
        url: 'https://chandler123456.z13.web.core.windows.net/:l1',
    })
);

const greeting = 'Welcome to Microsoft Ads Smart Page! It seems you do not have a smart page created. Please create a new account or sign into https://ads.microsoft.com/ to start!';

app.use('/', (req, res) => {
    res.send(greeting);
});

app.use('/home', (req, res) => {
    res.send(greeting);
});

// proxy

// app.use('/notfound', (req, res) => {
//     console.log('/notfound');
//     res.send("the page cannot be found");
// });

// app.use('/*', proxy('bogus', {
//     proxyReqPathResolver: (req) => {
//         console.log(req);
//         if (req.params['0'] && req.params['0'].length > 0) {
//             const redirectUrl = `chandler123456.z13.web.core.windows.net/${req.params['0']}/index.html`;
//             console.log(`proxy ${req.url} to ${redirectUrl}`);
//             return redirectUrl;
//         } else {
//             return `localhost:${port}/notfound`;
//         }
//         // var parts = req.url.split('?');
//         // var queryString = parts[1];
//         // var updatedPath = parts[0].replace(/test/, 'tent');
//         // return updatedPath + (queryString ? '?' + queryString : '');
//     }
// }));

app.listen(8080, () => console.log(`server started`));
