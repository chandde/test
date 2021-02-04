const ServiceHost = "https://localhost:44370/";

export function CreateUser(username, password, callback, errorcallback) {
    const xhr = new XMLHttpRequest();

    // get a callback when the server responds
    xhr.onload = callback;
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}createuser`);
    xhr.send({
        UserName: username,
        Password: password,
    });
};

export function Login(username, password, callback, errorcallback) {
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}authenticate`);
    xhr.send(JSON.stringify({
        UserName: username,
        Password: password,
    }));
}

export function listFolder(userid, folderid, token, callback, errorcallback) {
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}listfolder`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(JSON.stringify({
        UserId: userid,
        FolderId: folderid,
    }));
}

export function CreateFolder(userid, parentfolderid, foldername) {

};

export function UploadFile(userid, folderid, files, token) {

}