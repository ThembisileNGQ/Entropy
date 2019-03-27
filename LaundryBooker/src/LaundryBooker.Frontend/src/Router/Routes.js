import React from 'react';
import { Switch, Route } from 'react-router-dom';
import { withOidcSecure } from '@axa-fr/react-oidc-context';
import Home from '../Pages/Home';
import Bookings from '../Pages/Bookings';

const Routes = () => (
  <Switch>
    <Route exact path="/" component={Home} />
    <Route path="/bookings" component={withOidcSecure(Bookings)} />
  </Switch>
);

export default Routes;
