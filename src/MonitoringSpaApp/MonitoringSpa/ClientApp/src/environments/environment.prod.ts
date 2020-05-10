export const environment = {
    production: false,
    baseUrl: null, // Change this to the address of your backend API if different from frontend address
    tokenUrl: null, // For IdentityServer/Authorization Server API. You can set to null if same as baseUrl
    loginUrl: '/login',
    registerUrl: '/api/account/register',
    controllerRegistryUrl: '/api/controllerregistry',
    telemetryDataUrl: '/api/telemetrydata',
    dashboardDatUrl: '/api/dashboarddata'
};
