/*
 * config.js
 * Global configuration for the client app.
 *
 * API_BASE_URL points at the ASP.NET Core Web API (see
 * CountriesProject_MeravTomer/Properties/launchSettings.json for the port -
 * "applicationUrl": "https://localhost:7182;http://localhost:5262").
 */
const CONFIG = {
    API_BASE_URL: "https://localhost:7182/api",

    // localStorage keys used across the app (kept in one place to avoid typos)
    STORAGE_KEYS: {
        TOKEN: "zad_token",
        CURRENT_USER: "zad_current_user"
    }
};
