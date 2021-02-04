import _ from 'underscore';

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
    xhr.onerror = () => {
        errorcallback(xhr.status);
    }
    xhr.open('POST', `${ServiceHost}listfolder`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(JSON.stringify({
        UserId: userid,
        FolderId: folderid,
    }));
}

export function CreateFolder(userid, parentfolderid, foldername) {

};

export function UploadFiles(userid, folderid, files, token, callback, errorcallback) {    
    const formData = new FormData();

    // Update the formData object
    _.each(files, file => formData.append(
      "file",
      file,
      file.name
    ));
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}uploadfile?userid=${userid}&folderid=${folderid}`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(formData);
};
