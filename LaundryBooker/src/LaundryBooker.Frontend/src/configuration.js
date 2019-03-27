var configuration = {}
if(process.env.NODE_ENV === 'production'){
  configuration = {
    client_id: 'laundrybooker-spa',
    redirect_uri: 'http://192.168.99.100:30505/authentication/callback',
    response_type: 'id_token token',
    post_logout_redirect_uri: 'http://192.168.99.100:30505:3000/',
    scope: 'openid profile bookings.write bookings.read bookings.delete',
    authority: 'http://192.168.99.100:30502',
    silent_redirect_uri: 'http://192.168.99.100:30505/authentication/silent_callback',
    automaticSilentRenew: true,
    loadUserInfo: true,
    triggerAuthFlow: true
  };
  
} else {
  configuration = {
    client_id: 'laundrybooker-spa',
    redirect_uri: 'http://localhost:3000/authentication/callback',
    response_type: 'id_token token',
    post_logout_redirect_uri: 'http://localhost:3000/',
    scope: 'openid profile bookings.write bookings.read bookings.delete',
    authority: 'http://localhost:5002',
    silent_redirect_uri: 'http://localhost:3000/authentication/silent_callback',
    automaticSilentRenew: true,
    loadUserInfo: true,
    triggerAuthFlow: true
  };
}

export default configuration;

