const ServiceHost = "https://localhost:44370/";

export function CreateUser(username, password, callback, errorcallback){
    const xhr = new XMLHttpRequest();

    // get a callback when the server responds
    xhr.onload = callback;
    xhr.onerror = () => errorcallback(xhr.status);
    xhr.open('GET', `${ServiceHost}user/create?username=${username}&password=${password}`);
    // xhr.setRequestHeader('access-control-allow-origin', 'localhost');
    // xhr.setRequestHeader('access-control-allow-methods', 'POST, GET, OPTIONS, DELETE');
    // xhr.setRequestHeader('access-control-request-headers', 'origin, x-requested-with');

    xhr.send();
};

export function CreateFolder(userid, parentfolderid, foldername){

};

export function UploadFile(userid, folderid, files){

}