
import { Redirect, Link } from 'react-router-dom';
import { useContext } from 'react';
import { SessionContext } from './context';

export function LoginPage() {
    return (
        <div>
            <h1>Welcome to MyDrive, or your drive! Please log in</h1>
            <h2>if you have not registered before, please register first</h2>
            <Link to="/register">Switch to register</Link>
            <br/>
            <br/>
            <h2>User name</h2>
            <input className="username"/>
            <h2>Password</h2>
            <input className="password" type="password"/>
        </div>
    )
};
