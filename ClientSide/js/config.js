/*
 * config.js
 * Global configuration for the client app.
 *
 * ===> WHEN THE SERVER IS READY <===
 * 1. Set API_BASE_URL to the real Web API address (see launchSettings.json on the server project).
 * 2. Set USE_MOCK to false.
 * That's it - api.js already calls the real endpoints listed in the comments in api.js,
 * it only used the Mock.* functions in mockData.js while USE_MOCK was true.
 */
const CONFIG = {
    // TODO: replace with the real server URL (e.g. "https://localhost:7202/api")
    API_BASE_URL: "https://localhost:7202/api",

    // Master switch: true = use mockData.js, false = use real AJAX calls to API_BASE_URL
    USE_MOCK: true,

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
