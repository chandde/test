import { Redirect } from 'react-router-dom';
import { useContext, useState } from 'react';
import { Input, Button } from 'react-bootstrap';
import Promise from 'bluebird';
import { useHistory } from "react-router-dom";

import _ from 'underscore';

import { SessionContext } from './context';
import { listFolder, UploadFiles } from './service';

export function HomePage() {
    const [files, setFiles] = useState(null);
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

    const uploadFile = () => (<div>
        <input type="file" id="input" multiple onChange={onFileChange} />
        </div>
    );

    const buildFileList = () => _.map(files, file => (<div className={file.fileType}>
        {`${file.fileType === "Folder" ? "+ " : ""}${file.fileName}`}
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
            <br/>
            { (files && files.length > 0) && 
                buildFileList()
            }
        </div>
    );
};
