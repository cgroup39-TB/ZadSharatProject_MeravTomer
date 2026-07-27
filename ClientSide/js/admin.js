/*
 * admin.js
 * Powers pages/admin-users.html (lock/unlock users) and pages/admin-stats.html
 * (basic usage numbers). Both pages are admin-only.
 */
$(function () {
    if ($("#adminUsersTable").length) initAdminUsers();
    if ($("#adminStats").length) initAdminStats();
});

/**
 * Boots the admin-users page: enforces the admin-only client-side guard,
 * loads the table, and wires the lock/unlock buttons via event delegation
 * (so they work for rows re-rendered later by loadUsers()).
 */
function initAdminUsers() {
    Auth.requireAdmin();
    loadUsers();

    $("#adminUsersTable").on("click", ".btn-lock", function () {
        toggleLock($(this).data("id"), "lock");
    });
    $("#adminUsersTable").on("click", ".btn-unlock", function () {
        toggleLock($(this).data("id"), "unlock");
    });
    $("#adminUsersTable").on("click", ".btn-share-disable", function () {
        toggleCanShare($(this).data("id"), false);
    });
    $("#adminUsersTable").on("click", ".btn-share-enable", function () {
        toggleCanShare($(this).data("id"), true);
    });
}

/**
 * Fetches the user list from the admin-only API and re-renders the table;
 * called both on page load and after every lock/unlock action.
 */
function loadUsers() {
    Api.Admin.getUsers()
        .done(renderUsers)
        .fail(Common.showError);
}

/**
 * Renders the users table, replacing the lock/unlock action button with a
 * "(you)" label for the currently logged-in admin's own row so an admin
 * can't lock themselves out.
 */
function renderUsers(users) {
    const $tbody = $("#adminUsersTable tbody").empty();
    const currentUser = Auth.getCurrentUser();

    users.forEach(function (user) {
        const statusLabel = user.active
            ? '<span class="badge badge-success">Active</span>'
            : '<span class="badge badge-danger">Locked</span>';

        const actionBtn = user.id === currentUser.id
            ? '<span class="muted">(you)</span>'
            : (user.active
                ? '<button class="btn btn-small btn-danger btn-lock" data-id="' + user.id + '">Lock</button>'
                : '<button class="btn btn-small btn-outline btn-unlock" data-id="' + user.id + '">Unlock</button>');

        const shareLabel = user.canShare
            ? '<span class="badge badge-success">Allowed</span>'
            : '<span class="badge badge-danger">Blocked</span>';

        const shareActionBtn = user.id === currentUser.id
            ? '<span class="muted">(you)</span>'
            : (user.canShare
                ? '<button class="btn btn-small btn-danger btn-share-disable" data-id="' + user.id + '">Revoke sharing</button>'
                : '<button class="btn btn-small btn-outline btn-share-enable" data-id="' + user.id + '">Allow sharing</button>');

        $tbody.append(
            '<tr>' +
            '<td>' + user.name + '</td>' +
            '<td>' + user.email + '</td>' +
            '<td>' + (user.isAdmin ? "Admin" : "User") + '</td>' +
            '<td>' + statusLabel + '</td>' +
            '<td>' + shareLabel + '</td>' +
            '<td>' + actionBtn + '</td>' +
            '<td>' + shareActionBtn + '</td>' +
            '</tr>'
        );
    });
}

/**
 * Calls the admin-only lock/unlock endpoint and refreshes the table on success.
 * The server independently enforces the admin check (User BL throws
 * UnauthorizedAccessException for non-admins), so this is UX only, not the real gate.
 */
function toggleLock(userId, action) {
    const request = action === "lock" ? Api.Admin.lockUser(userId) : Api.Admin.unlockUser(userId);
    request
        .done(function () {
            Common.showAlert(action === "lock" ? "User locked." : "User unlocked.", "success");
            loadUsers();
        })
        .fail(Common.showError);
}

/**
 * Calls the admin-only enable/disable-sharing endpoint and refreshes the table on success.
 * The server independently enforces both the admin check and the CanShare flag itself
 * (BL/UserVisitedCountry.cs rejects isShared=true posts/updates for blocked users), so this
 * is UX only, not the real gate.
 */
function toggleCanShare(userId, canShare) {
    const request = canShare ? Api.Admin.enableSharing(userId) : Api.Admin.disableSharing(userId);
    request
        .done(function () {
            Common.showAlert(canShare ? "Sharing allowed for user." : "Sharing revoked for user.", "success");
            loadUsers();
        })
        .fail(Common.showError);
}

/**
 * Boots the admin-stats page: enforces the admin-only client-side guard,
 * then fetches and fills in the summary stat tiles plus the daily-logins list.
 */
function initAdminStats() {
    Auth.requireAdmin();

    Api.Admin.getStats()
        .done(function (stats) {
            $("#statTodayLogins").text(stats.todayLogins);
            $("#statTotalLogins").text(stats.totalLogins);
            $("#statCountriesImported").text(stats.countriesImported);
            $("#statCountriesSaved").text(stats.countriesSaved);
            $("#statSharesCreated").text(stats.sharesCreated);
            renderDailyLogins(stats.dailyLogins);
        })
        .fail(Common.showError);
}

/**
 * Renders the per-day login counts list, showing a "no data yet" message
 * instead of an empty list when there's no history.
 */
function renderDailyLogins(dailyLogins) {
    const $list = $("#dailyLoginsList").empty();
    if (!dailyLogins.length) {
        $list.html('<p class="muted">No login data yet.</p>');
        return;
    }
    dailyLogins.forEach(function (row) {
        $list.append('<div class="list-row"><span>' + row.date + '</span><span>' + row.count + ' login(s)</span></div>');
    });
}
