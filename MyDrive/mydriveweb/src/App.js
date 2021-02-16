import './App.css';
import React, { useContext, useState } from 'react';
import { Switch, Route, BrowserRouter as Router } from 'react-router-dom';
import _ from 'underscore';

import { SessionContext } from './context';
import { LoginPage } from './loginpage';
import { HomePage } from './homepage';
import { RegisterPage } from './registerpage';

function App() {
  const getCookie = (name) => {
    const row = document.cookie
      .split('; ')
      .find(row => row.startsWith(name));

    if (row) {
      return row.split('=')[1];
    }
  };

  const userId = getCookie("userid");
  const folderId = getCookie("folderid");
  const token = getCookie("jwttokencookie");
  const userName = getCookie("username");

  const [context, setContext] = useState({
    userId,
    userName,
    folderId,
    token,
  });

  const updateContext = (newContext) => {
    setContext(_.defaults(newContext, context));
  };

  return (
    <SessionContext.Provider value={{ globalContext: context, updateContext }}>
      <Router>
        <Route path="/">
          <HomePage />
        </Route>
        <Route path="/login">
          <LoginPage />
        </Route>
        <Route path="/register">
          <RegisterPage />
        </Route>
      </Router>
    </SessionContext.Provider>
  );
}

export default App;
