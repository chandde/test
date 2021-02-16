/* eslint-disable jsx-a11y/anchor-is-valid */
import { Redirect } from 'react-router-dom';
import { useContext, useState } from 'react';
import { Button, Modal } from 'react-bootstrap';
import Promise from 'bluebird';
import { useHistory } from "react-router-dom";

import _ from 'underscore';

import { SessionContext } from './context';
import { listFolder, UploadFiles, DownloadFile, CreateFolder, GetParentFolder } from './service';

export function HomePage() {
    const [files, setFiles] = useState(null);
    const [creatingFolder, setCreatingFolder] = useState(false);
    const [newFolderName, setNewFolderName] = useState("");
    let history = useHistory();

    const listCallback = (response) => {
        if (response) {
            var list = JSON.parse(response);
            setFiles(list);
        }
    };

    const listErrorcallback = () => {
        history.push('/login')
    };

    const uploadCallback = (response) => {
        // on successful upload, call listFolder to reload the file list
        listFolder(
            context.globalContext.userId,
            context.globalContext.folderId,
            context.globalContext.token,
            listCallback,
            listErrorcallback
        );
    };

    const uploadErrorCallback = () => {

    };

    const onCreateFolderClicks = () => {
        setCreatingFolder(true);
        setNewFolderName("");
    }

    const context = useContext(SessionContext);

    if (!files) {
        if (!context.globalContext || !context.globalContext.userId) {
            return <Redirect to="/login" />;
        } else {
            listFolder(
                context.globalContext.userId,
                context.globalContext.folderId,
                context.globalContext.token,
                listCallback,
                listErrorcallback
            );
        }
    }

    const onFileChange = (e) => {
        // setSelectedFiles(e.target.files);
        UploadFiles(
            context.globalContext.userId,
            context.globalContext.folderId,
            e.target.files,
            context.globalContext.token,
            uploadCallback,
            uploadErrorCallback
        );
    };

    const onDownload = (blob, fileName ) => {
        var a = document.createElement("a");
        a.href = URL.createObjectURL(blob);
        a.download = fileName;
        a.hidden = true;
        document.body.appendChild(a);
        a.click();
    }

    const uploadFile = () => (<div>
        <input type="file" id="input" multiple onChange={onFileChange} />
        </div>
    );

    const openFile = (file) => {
        if (file.fileType === "Folder") {
            setFiles(null);
            context.updateContext({
                folderId: file.fileId
            });
        } else {
            DownloadFile(context.globalContext.userId, file.fileId, file.fileName, context.globalContext.token, onDownload);
        }
    };

    const creatingCancelClicked = () =>{
        setCreatingFolder(false);
        setNewFolderName("");
    }

    const creatingOKClicked = () =>{
        setCreatingFolder(false);   
        CreateFolder(context.globalContext.userId, context.globalContext.folderId, newFolderName, context.globalContext.token, uploadCallback);
    }

    const getparentCallback = (parentFolderId) => {
        if (parentFolderId && parentFolderId.length > 0) {
            setFiles(null);
            context.updateContext({
                folderId: parentFolderId
            });
        }
    }

    const onGoUpClick = () => {
        GetParentFolder(context.globalContext.userId, context.globalContext.folderId, context.globalContext.token, getparentCallback)
    }

    const newFolderNameUpdated = (e) => {
        setNewFolderName(e.target.value);
    }

    const buildFileList = () => _.map(_.sortBy(files, f => f.fileName.toLowerCase()), file => (<div>
        <a
            className={file.fileType}
            href={`javascript:void(0)`}
            onClick={() => openFile(file)}
        >
            {`${file.fileType === "Folder" ? "<<<Folder>>> " : ""}${file.fileName}`}
        </a>
        <br />
    </div>));

    console.log(files);

    return (
        <div>
            {(!files || files.length === 0)  && 
                <div>Seems you don't have anything here...</div>
            }
            <br/>            
            {uploadFile()}
            <br/>
            <Button onClick={onCreateFolderClicks}>Create folder</Button>
            <Button onClick={onGoUpClick}>Go up</Button>
            <Modal show={creatingFolder}>
                <Modal.Header>
                    <Modal.Title>Please enter new folder name:</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <input className="newFolderName" onChange={newFolderNameUpdated} />
                </Modal.Body>
                <Modal.Footer>
                    <Button className="creatingOK" disabled={newFolderName.length === 0} onClick={creatingOKClicked}>OK</Button>
                    <Button className="creatingCancel" onClick={creatingCancelClicked}>Cancel</Button>
                </Modal.Footer>
            </Modal>
            <br/>
            { (files && files.length > 0) && 
                buildFileList()
            }
        </div>
    );
};
