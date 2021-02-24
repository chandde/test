
import { Redirect, Link } from 'react-router-dom';
import { useContext, useState } from 'react';
import { Button } from 'react-bootstrap';
import { SessionContext } from './context';
import { CreateUser } from './service';
import { sha256 } from 'js-sha256';

import Promise from 'bluebird';

export function RegisterPage() {
    const createAccount = () => {
        // hash password
       
        const passwordStream = new TextEncoder().encode(password1);
        const sha = sha256.create();
        sha.update(password1);
        // sha.hex();
        CreateUser(userName, sha.hex(), createAccountCallback, createAccountErrorCallback);

        // const passwordStream = new TextEncoder().encode(password1);
        // Promise.resolve(crypto.subtle.digest('SHA-256', passwordStream)).then(hashBuffer => {
        //     const hashArray = Array.from(new Uint8Array(hashBuffer));                     // convert buffer to byte array
        //     const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join(''); // convert bytes to hex string
        //     CreateUser(userName, hashHex, createAccountCallback, createAccountErrorCallback);
        // });
    };

    const createAccountCallback = () => {
        setusercreated(true);
    };

    const createAccountErrorCallback = (errorcode) => {
        setusercreateerror(errorcode);
    }

    const usernamechanged = (e) => {
        setUserName(e.target.value);
    };

    const password1changed = (e) => {
        setPassword1(e.target.value);
    };

    const password2changed = (e) => {
        setPassword2(e.target.value);
    };

    const [userName, setUserName] = useState(null);
    const [password1, setPassword1] = useState(null);
    const [password2, setPassword2] = useState(null);
    const [usercreated, setusercreated] = useState(false);
    const [usercreateerror, setusercreateerror] = useState(null);

    return (
        <div>
            <h1>Register MyDrive account here</h1>
            <br />
            <h2>User name</h2>
            <input className="username" onChange={usernamechanged} />
            <h2>Password</h2>
            <input className="password" type="password" onChange={password1changed} />
            <h2>Repeat password</h2>
            <input className="password2" type="password" onChange={password2changed} />
            <br />
            {password1 !== password2 &&
                <div className="passwordunmatch" >passwords don't match</div>
            }
            <Button disabled={!password1 || !password2 || password1 !== password2} onClick={createAccount}>Create account</Button>

            {usercreated &&
                <div><h2>Your account has been created successfully, please log in<Link to="/login">Login</Link></h2>
                </div>
            }

            {usercreateerror !== null && usercreateerror !== undefined && 
                <div><h2>Something went wrong, please try again</h2>
                </div>}
        </div>);
};