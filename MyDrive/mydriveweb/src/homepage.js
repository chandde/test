import { Redirect } from 'react-router-dom';
import { useContext } from 'react';
import Promise from 'bluebird';

import { SessionContext } from './context';
import { listFolder } from './service';

export function HomePage() {
    const callback = () => {

    };

    const errorcallback = () => {

    };

    const context = useContext(SessionContext);
    if (!context.globalContext || !context.globalContext.userId) {
        return <Redirect to="/login" />;
    } else {
        return Promise.resolve(listFolder(
            context.globalContext.userId,
            context.globalContext.folderId,
            context.globalContext.token,
            callback,
            errorcallback
        )).then((response) => {
            // serialze response into a list of files
            const files = JSON.parse(response);
            
        }).catch((error) => {

        });
    }
};
