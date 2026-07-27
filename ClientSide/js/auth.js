/*
 * auth.js
 * Session management: login, register, logout, "who am I", and page guards.
 * Session is kept in localStorage (token + user object). Real JWT handling can replace
 * getAuthHeader() later without touching any other file.
 */
const Auth = (function () {

    function getToken() {
        return localStorage.getItem(CONFIG.STORAGE_KEYS.TOKEN);
    }

    /**
     * Returns the cached current-user object from localStorage, or null if
     * none is stored (i.e. no JSON parsing/decoding is attempted on absence).
     */
    function getCurrentUser() {
        const raw = localStorage.getItem(CONFIG.STORAGE_KEYS.CURRENT_USER);
        return raw ? JSON.parse(raw) : null;
    }

    /**
     * Fails closed: requires both a stored token AND a stored user object,
     * so a partially-corrupted session (only one of the two present) counts as logged out.
     */
    function isLoggedIn() {
        return !!getToken() && !!getCurrentUser();
    }

    /**
     * Gates admin-only UI/pages by checking the `isAdmin` flag on the cached
     * current user; returns false (not throws) if no user is logged in.
     */
    function isAdmin() {
        const user = getCurrentUser();
        return !!user && !!user.isAdmin;
    }

    function setSession(token, user) {
        localStorage.setItem(CONFIG.STORAGE_KEYS.TOKEN, token);
        localStorage.setItem(CONFIG.STORAGE_KEYS.CURRENT_USER, JSON.stringify(user));
    }

    function clearSession() {
        localStorage.removeItem(CONFIG.STORAGE_KEYS.TOKEN);
        localStorage.removeItem(CONFIG.STORAGE_KEYS.CURRENT_USER);
    }

    // Used by api.js so every real AJAX call sends the token once the server is ready.
    // TODO: once the real server issues real JWTs, this is already wired to send them.
    /**
     * Builds the Authorization header for AJAX calls; returns an empty object
     * (no header at all) rather than an empty/invalid header when no token is stored.
     */
    function getAuthHeader() {
        const token = getToken();
        return token ? { "Authorization": "Bearer " + token } : {};
    }

    // credentials: { email, password }
    /**
     * Logs in via the API, then stashes the returned token/user into localStorage
     * on success so the rest of the app immediately sees the new session.
     */
    function login(credentials) {
        return Api.Users.login(credentials).done(function (result) {
            setSession(result.token, result.user);
        });
    }

    // userData: { name, email, password }
    function register(userData) {
        return Api.Users.register(userData);
    }

    /**
     * Clears the local session and redirects to the login page, regardless
     * of whether the server is ever notified.
     */
    function logout() {
        clearSession();
        window.location.href = resolvePath("pages/login.html");
    }

    // data: { name, email } (password change intentionally kept simple for the student project)
    /**
     * Updates the profile via the API; short-circuits with a rejected promise
     * (mimicking a failed AJAX response shape) if no user is currently logged in,
     * instead of sending the request. On success, re-saves the session with the
     * existing token but the fresh user object returned by the server.
     */
    function updateProfile(data) {
        const user = getCurrentUser();
        if (!user) return $.Deferred().reject({ responseJSON: { message: "Not logged in." } }).promise();

        return Api.Users.update(user.id, data).done(function (updatedUser) {
            const token = getToken();
            setSession(token, updatedUser);
        });
    }

    /**
     * Returns the current user's saved preferences, falling back to an
     * empty-but-well-shaped object so callers never need a null check.
     */
    function getPreferences() {
        const user = getCurrentUser();
        return (user && user.preferences) || { continents: [], countries: [], languages: [] };
    }

    // preferences: { continents: string[], countries: string[], languages: [{ name, level }] }
    function savePreferences(preferences) {
        return updateProfile({ preferences: preferences });
    }

    // Small helper so guards work the same whether the current page is at the site root or under /pages
    /**
     * Rewrites a root-relative path (e.g. "pages/login.html") to work correctly
     * whether the current page already lives inside /pages or at the site root.
     */
    function resolvePath(pathFromRoot) {
        const inPagesFolder = window.location.pathname.indexOf("/pages/") !== -1;
        return inPagesFolder ? pathFromRoot.replace(/^pages\//, "") : pathFromRoot;
    }

    // Call at the top of any page that requires a logged-in user
    /**
     * Page guard: redirects to the login page immediately if there is no
     * valid session, so callers don't need their own logged-in check.
     */
    function requireAuth() {
        if (!isLoggedIn()) {
            window.location.href = resolvePath("pages/login.html");
        }
    }

    // Call at the top of any admin-only page
    /**
     * Page guard: enforces login first, then redirects non-admins away to the
     * countries list. Client-side only — the real gate is server-side (User BL
     * throws UnauthorizedAccessException for non-admins on admin endpoints).
     */
    function requireAdmin() {
        requireAuth();
        if (!isAdmin()) {
            window.location.href = resolvePath("pages/countries-list.html");
        }
    }

    return {
        login: login,
        register: register,
        logout: logout,
        updateProfile: updateProfile,
        getPreferences: getPreferences,
        savePreferences: savePreferences,
        getCurrentUser: getCurrentUser,
        isLoggedIn: isLoggedIn,
        isAdmin: isAdmin,
        getAuthHeader: getAuthHeader,
        requireAuth: requireAuth,
        requireAdmin: requireAdmin
    };
})();
