const ServiceHost = "https://localhost:44370/";

export function CreateUser(username, password, callback, errorcallback){
    const xhr = new XMLHttpRequest();

    // get a callback when the server responds
    xhr.onload = callback;
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('GET', `${ServiceHost}user/create?username=${username}&password=${password}`);
    xhr.send();
};

export function Login(username, password, callback, errorcallback){
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('GET', `${ServiceHost}user/authenticate?username=${username}&password=${password}`);
    xhr.send();
}

export function listFolder(folderid, userid, token, callback, errorcallback){
    const xhr = new XMLHttpRequest();
    xhr.onload = () => callback(xhr.response);
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('GET', `${ServiceHost}user/listfolder?username=${userid}&token=${token}&folderid=${folderid}`);
    xhr.send();    
}

export function CreateFolder(userid, parentfolderid, foldername){

};

export function UploadFile(userid, folderid, files){

}