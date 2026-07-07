/*
 * admin.js
 * Powers pages/admin-users.html (lock/unlock users) and pages/admin-stats.html
 * (basic usage numbers). Both pages are admin-only.
 */
$(function () {
    if ($("#adminUsersTable").length) initAdminUsers();
    if ($("#adminStats").length) initAdminStats();
});

function initAdminUsers() {
    Auth.requireAdmin();
    loadUsers();

    $("#adminUsersTable").on("click", ".btn-lock", function () {
        toggleLock($(this).data("id"), "lock");
    });
    $("#adminUsersTable").on("click", ".btn-unlock", function () {
        toggleLock($(this).data("id"), "unlock");
    });
}

function loadUsers() {
    Api.Admin.getUsers()
        .done(renderUsers)
        .fail(Common.showError);
}

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

        $tbody.append(
            '<tr>' +
            '<td>' + user.name + '</td>' +
            '<td>' + user.email + '</td>' +
            '<td>' + (user.isAdmin ? "Admin" : "User") + '</td>' +
            '<td>' + statusLabel + '</td>' +
            '<td>' + actionBtn + '</td>' +
            '</tr>'
        );
    });
}

function toggleLock(userId, action) {
    const request = action === "lock" ? Api.Admin.lockUser(userId) : Api.Admin.unlockUser(userId);
    request
        .done(function () {
            Common.showAlert(action === "lock" ? "User locked." : "User unlocked.", "success");
            loadUsers();
        })
        .fail(Common.showError);
}

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
