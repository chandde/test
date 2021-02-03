
import { Redirect } from 'react-router-dom';
import { useContext } from 'react';
import { SessionContext } from './context';

export function HomePage() {
    const context = useContext(SessionContext);
    if (!context.globalContext || !context.globalContext.userId) {
        return <Redirect to="/login" />;
    }
};
