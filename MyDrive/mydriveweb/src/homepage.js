import { Redirect } from 'react-router-dom';
import { useContext, useState } from 'react';
import Promise from 'bluebird';

import { SessionContext } from './context';
import { listFolder } from './service';

export function HomePage() {
    const [files, setFiles] = useState(null);

    const callback = (response) => {
        if (response) {
            var list = JSON.parse(response);
            setFiles(list);
        }
    };

    const errorcallback = () => {

    };

    const context = useContext(SessionContext);

    if(!files) {
        if (!context.globalContext || !context.globalContext.userId) {
            return <Redirect to="/login" />;
        } else {
            listFolder(
                context.globalContext.userId,
                context.globalContext.folderId,
                context.globalContext.token,
                callback,
                errorcallback
            );
        }
    }

    if(!files || files.length === 0) {
        return (<div>Seems you don't have anything here..</div>);
    }
    
    return (<div>{`You have ${files.length} files here!`}</div>)
};
