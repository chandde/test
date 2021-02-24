
import { Redirect, Link } from 'react-router-dom';
import { Button } from 'react-bootstrap';
import { useContext, useState } from 'react';
import { useHistory } from "react-router-dom";
import jwt_decode from "jwt-decode";
import Promise from 'bluebird';
import {sha256} from "js-sha256";

import { SessionContext } from './context';
import { Login } from './service';

export function LoginPage() {
    const [username, setusername] = useState(null);
    const [password, setpassword] = useState(null);
    const [authErr, setAuthErr] = useState(false);

    let history = useHistory();

    const context = useContext(SessionContext)

    const usernamechanged = (e) => {
        setusername(e.target.value);
    }

    const passwordchanged = (e) => {
        setpassword(e.target.value);
    }

    const loginCallback = (response) => {
        if (response) {
            var decoded = jwt_decode(response);
            context.updateContext({
                userId: decoded.userId,
                userName: decoded.username,
                folderId: decoded.rootfolderId,
                token: response,                
            });
            history.push('/')
            // parse JWT token and populate data into global context
        } else {
            setAuthErr(false);
        }
    };

    const loginErrorCallback = (status) => {
        setAuthErr(true);
    };

    const login = () => {
        // hash password
        const passwordStream = new TextEncoder().encode(password);
        const sha = sha256.create();
        sha.update(password);
        // sha.hex();
        Login(username, sha.hex(), loginCallback, loginErrorCallback);

        // Promise.resolve(crypto.subtle.digest('SHA-256', passwordStream)).then(hashBuffer => {
        //     const hashArray = Array.from(new Uint8Array(hashBuffer));                     // convert buffer to byte array
        //     const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join(''); // convert bytes to hex string
        //     Login(username, hashHex, loginCallback, loginErrorCallback);
        // });
    };

    return (
        <div>
            <h1>Welcome to MyDrive, or your drive! Please log in</h1>
            <h2>if you have not registered before, please register first</h2>
            <Link to="/register">Switch to register</Link>
            <br />
            <br />
            <h2>User name</h2>
            <input className="username" onChange={usernamechanged} />
            <h2>Password</h2>
            <input className="password" type="password" onChange={passwordchanged} />
            <Button onClick={login}>Login</Button>
            {authErr && <div>Authentication failed, please try again</div>}
        </div>
    )
};
