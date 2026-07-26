/*
 * common.js
 * Small helpers shared by every page: navbar rendering, alert boxes, query-string reading.
 * Include this after auth.js and before the page-specific script.
 */
const Common = (function () {

    // Reads the current page's query string into a plain object, e.g. ?id=3&mode=edit
    /**
     * Parses `window.location.search` into a plain key/value object, URL-decoding
     * both sides and defaulting a valueless param (e.g. "?flag") to an empty string.
     */
    function getQueryParams() {
        const params = {};
        const search = window.location.search.substring(1);
        search.split("&").forEach(function (pair) {
            if (!pair) return;
            const parts = pair.split("=");
            params[decodeURIComponent(parts[0])] = decodeURIComponent(parts[1] || "");
        });
        return params;
    }

    // Shows a dismissible message inside the #alertBox element every page includes.
    // type: "success" | "error" | "info"
    /**
     * Shows a message in #alertBox, auto-hiding after 4s; falls back to a
     * native `alert()` on pages that don't include the #alertBox element.
     */
    function showAlert(message, type) {
        const $box = $("#alertBox");
        if (!$box.length) {
            alert(message);
            return;
        }
        $box.removeClass("alert-success alert-error alert-info")
            .addClass("alert-" + (type || "info"))
            .text(message)
            .show();
        setTimeout(function () { $box.fadeOut(); }, 4000);
    }

    /**
     * Normalizes a jQuery AJAX failure into a user-facing error message: accepts
     * a plain string, or pulls a message/title/error out of responseJSON (string
     * or object) or falls back to raw responseText, before showing it via showAlert.
     */
    function showError(jqXHRorMessage) {
        let message;

        if (typeof jqXHRorMessage === "string") {
            message = jqXHRorMessage;
        } else if (jqXHRorMessage && jqXHRorMessage.responseJSON) {
            const body = jqXHRorMessage.responseJSON;
            message = typeof body === "string"
                ? body
                : (body.message || body.title || body.error);
        } else if (jqXHRorMessage && jqXHRorMessage.responseText) {
            message = jqXHRorMessage.responseText;
        }

        showAlert(message || "Something went wrong.", "error");
    }

    function formatNumber(num) {
        return Number(num).toLocaleString();
    }

    // Renders the shared top navbar into #navbar, adjusting links to the logged-in state.
    /**
     * Rebuilds the navbar's HTML based on login/admin state (extra links for
     * logged-in users, admin-only links for admins) and rewires the logout click
     * handler each time, since the markup is fully replaced.
     */
    function renderNavbar() {
        const $nav = $("#navbar");
        if (!$nav.length) return;

        const inPages = window.location.pathname.indexOf("/pages/") !== -1;
        const root = inPages ? "" : "pages/";
        const home = inPages ? "../index.html" : "index.html";
        const loggedIn = Auth.isLoggedIn();
        const admin = Auth.isAdmin();
        const user = Auth.getCurrentUser();

        let html = '<div class="navbar-inner">';
        html += '<a class="brand" href="' + home + '">🌍 Countries App</a>';
        html += '<div class="nav-links">';
        html += '<a href="' + root + 'countries-list.html">Countries</a>';

        if (loggedIn) {
            html += '<a href="' + root + 'my-lists.html">My Lists</a>';
            html += '<a href="' + root + 'shares.html">Shares</a>';
            html += '<a href="' + root + 'quiz-list.html">Quizzes</a>';
            if (admin) {
                html += '<a href="' + root + 'admin-users.html">Admin Users</a>';
                html += '<a href="' + root + 'admin-stats.html">Admin Stats</a>';
            }
            html += '<a href="' + root + 'preferences.html">Preferences</a>';
            html += '<a href="' + root + 'profile.html">' + (user ? user.name : "Profile") + '</a>';
            html += '<a href="#" id="logoutLink">Logout</a>';
        } else {
            html += '<a href="' + root + 'quiz-list.html">Quizzes</a>';
            html += '<a href="' + root + 'login.html">Login</a>';
            html += '<a href="' + root + 'register.html">Register</a>';
        }
        html += '</div></div>';

        $nav.html(html);
        $("#logoutLink").on("click", function (e) {
            e.preventDefault();
            Auth.logout();
        });
    }

    return {
        getQueryParams: getQueryParams,
        showAlert: showAlert,
        showError: showError,
        formatNumber: formatNumber,
        renderNavbar: renderNavbar
    };
})();

$(function () {
    Common.renderNavbar();
});
