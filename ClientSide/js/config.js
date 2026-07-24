/*
 * config.js
 * Global configuration for the client app.
 *
 * The server is connected: USE_MOCK is false and API_BASE_URL points at the
 * ASP.NET Core Web API (see CountriesProject_MeravTomer/Properties/launchSettings.json
 * for the port - "applicationUrl": "https://localhost:7182;http://localhost:5262").
 * Switch USE_MOCK back to true to fall back to the in-browser mock data.
 */
const CONFIG = {
    API_BASE_URL: "https://localhost:7182/api",

    // Master switch: true = use mockData.js, false = use real AJAX calls to API_BASE_URL
    USE_MOCK: false,

    // Fake network latency (ms) so the mock behaves like a real AJAX call (loaders, spinners, etc.)
    MOCK_DELAY_MS: 300,

    // Quiz settings
    QUIZ_DURATION_SECONDS: 60,

    // localStorage keys used across the app (kept in one place to avoid typos)
    STORAGE_KEYS: {
        TOKEN: "zad_token",
        CURRENT_USER: "zad_current_user",
        USERS: "zad_mock_users",
        COUNTRIES: "zad_mock_countries",
        USER_COUNTRIES: "zad_mock_user_countries",
        SHARES: "zad_mock_shares",
        LOGIN_LOG: "zad_mock_login_log",
        SEEDED: "zad_mock_seeded_v1"
    }
};
