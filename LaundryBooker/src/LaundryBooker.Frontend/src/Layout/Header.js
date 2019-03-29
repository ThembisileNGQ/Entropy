import React from 'react';
import { AuthenticationConsumer } from '@axa-fr/react-oidc-context';
import { Link } from 'react-router-dom';

const headerStyle = {
  display: 'flex',
  backgroundColor: '#222222',
  justifyContent: 'space-between',
  padding: 10,
};

const linkStyle = {
  color: '#D7DADB',
  textDecoration: 'underline',
};

export default () => (
  <header>
    <AuthenticationConsumer>
      {props => {
        return (
          <div style={headerStyle}>
            <h3>
              <Link style={linkStyle} to="/">
                HOME
              </Link>
            </h3>

            {props.oidcUser || !props.isEnabled ? (
              <ul>
                <li>
                  <Link style={linkStyle} to="/bookings">
                    Bookings
                  </Link>
                </li>
                <button onClick={props.logout}>logout</button>
              </ul>
            ) : (
              <button onClick={props.login}>login</button>
            )}
          </div>
        );
      }}
    </AuthenticationConsumer>
  </header>
);
