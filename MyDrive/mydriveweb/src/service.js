import _ from 'underscore';

const ServiceHost = process.env.REACT_APP_SERVICE_ENDPOINT;

export function CreateUser(username, password, callback, errorcallback) {
    const xhr = new XMLHttpRequest();

    // get a callback when the server responds
    xhr.onload = callback;
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}createuser`);
    xhr.send(JSON.stringify({
        UserName: username,
        Password: password,
    }));
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

export function listFolder(userId, folderId, token, callback, errorcallback) {
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => {
        errorcallback(xhr.status);
    }
    xhr.open('POST', `${ServiceHost}listfolder`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(JSON.stringify({
        UserId: userId,
        FolderId: folderId,
    }));
}

export function UploadFiles(userId, folderId, files, token, callback, errorcallback) {
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
    xhr.open('POST', `${ServiceHost}uploadfile?userId=${userId}&folderId=${folderId}`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(formData);
};

export function DownloadFile(userId, fileId, filename, token, callback, errorcallback) {
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response, filename);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}downloadfile?fileId=${fileId}`);
    xhr.responseType = "blob";
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(JSON.stringify({
        UserId: userId,
    }));
}

export function CreateFolder(userId, folderId, newFolderName, token, callback, errorcallback) {
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}createfolder?folderName=${newFolderName}`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(JSON.stringify({
        UserId: userId,
        FolderId: folderId,
        FileName: newFolderName,
    }));
}

export function GetParentFolder(userId, folderId, token, callback, errorcallback) {
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('POST', `${ServiceHost}getparent`);
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(JSON.stringify({
        UserId: userId,
        FolderId: folderId,
    }));    
}