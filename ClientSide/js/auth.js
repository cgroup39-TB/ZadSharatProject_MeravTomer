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

    function getCurrentUser() {
        const raw = localStorage.getItem(CONFIG.STORAGE_KEYS.CURRENT_USER);
        return raw ? JSON.parse(raw) : null;
    }

    function isLoggedIn() {
        return !!getToken() && !!getCurrentUser();
    }

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
    function getAuthHeader() {
        const token = getToken();
        return token ? { "Authorization": "Bearer " + token } : {};
    }

    // credentials: { email, password }
    function login(credentials) {
        return Api.Users.login(credentials).done(function (result) {
            setSession(result.token, result.user);
        });
    }

    // userData: { name, email, password }
    function register(userData) {
        return Api.Users.register(userData);
    }

    function logout() {
        clearSession();
        window.location.href = resolvePath("pages/login.html");
    }

    // data: { name, email } (password change intentionally kept simple for the student project)
    function updateProfile(data) {
        const user = getCurrentUser();
        if (!user) return $.Deferred().reject({ responseJSON: { message: "Not logged in." } }).promise();

        return Api.Users.update(user.id, data).done(function (updatedUser) {
            const token = getToken();
            setSession(token, updatedUser);
        });
    }

    function getPreferences() {
        const user = getCurrentUser();
        return (user && user.preferences) || { continents: [], countries: [], languages: [] };
    }

    // preferences: { continents: string[], countries: string[], languages: [{ name, level }] }
    function savePreferences(preferences) {
        return updateProfile({ preferences: preferences });
    }

    // Small helper so guards work the same whether the current page is at the site root or under /pages
    function resolvePath(pathFromRoot) {
        const inPagesFolder = window.location.pathname.indexOf("/pages/") !== -1;
        return inPagesFolder ? pathFromRoot.replace(/^pages\//, "") : pathFromRoot;
    }

    // Call at the top of any page that requires a logged-in user
    function requireAuth() {
        if (!isLoggedIn()) {
            window.location.href = resolvePath("pages/login.html");
        }
    }

    // Call at the top of any admin-only page
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
