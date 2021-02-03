import './App.css';
import React, { useContext, useState } from 'react';
import { Switch, Route, BrowserRouter as Router } from 'react-router-dom';
import _ from 'underscore';

import { SessionContext } from './context';
import { LoginPage } from './loginpage';
import { HomePage } from './homepage';
import { RegisterPage } from './registerpage';

function App() {
  const [context, setContext] = useState(null);

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
