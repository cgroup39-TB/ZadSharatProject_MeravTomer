/*
 * config.js
 * Global configuration for the client app.
 *
 * API_BASE_URL points at the ASP.NET Core Web API, deployed on the
 * institution's server (Swagger UI: https://proj.ruppin.ac.il/cgroup39/test2/tar1/swagger/index.html).
 * For local development, swap this back to "https://localhost:7182/api" (see
 * CountriesProject_MeravTomer/Properties/launchSettings.json for the port).
 */
const CONFIG = {
    API_BASE_URL: "https://proj.ruppin.ac.il/cgroup39/test2/tar1/api",

    // localStorage keys used across the app (kept in one place to avoid typos)
    STORAGE_KEYS: {
        TOKEN: "zad_token",
        CURRENT_USER: "zad_current_user"
    }
};
