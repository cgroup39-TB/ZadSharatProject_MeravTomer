/*
 * userLists.js
 * Powers pages/my-lists.html: shows the logged-in user's "visited" and
 * "wishlist" countries, lets them move a country between lists or remove it.
 */
$(function () {
    Auth.requireAuth();
    loadLists();

    $("#visitedList, #wishlistList").on("click", ".btn-move", function () {
        const entryId = $(this).data("entry-id");
        const newListType = $(this).data("target-list");
        moveEntry(entryId, newListType);
    });

    $("#visitedList, #wishlistList").on("click", ".btn-remove", function () {
        const entryId = $(this).data("entry-id");
        removeEntry(entryId);
    });
});

function loadLists() {
    const user = Auth.getCurrentUser();

    $.when(Api.UserCountries.getByUser(user.id), Api.Countries.getAll())
        .done(function (entriesResult, countriesResult) {
            const entries = entriesResult[0];
            const countries = countriesResult[0];
            renderList(entries, countries, "visited", "#visitedList");
            renderList(entries, countries, "wishlist", "#wishlistList");
        })
        .fail(Common.showError);
}

function renderList(entries, countries, listType, containerSelector) {
    const $container = $(containerSelector);
    $container.empty();

    const rows = entries.filter(function (e) { return e.listType === listType; });
    if (!rows.length) {
        $container.html('<p class="muted">Nothing here yet.</p>');
        return;
    }

    const otherList = listType === "visited" ? "wishlist" : "visited";
    const moveLabel = listType === "visited" ? "Move to Wishlist" : "Mark as Visited";

    rows.forEach(function (entry) {
        const country = countries.find(function (c) { return c.id === entry.countryId; });
        if (!country) return;

        $container.append(
            '<div class="list-row">' +
            '<img src="' + country.flagPng + '" class="flag-thumb-small" alt="">' +
            '<a href="country-details.html?id=' + country.id + '">' + country.commonName + '</a>' +
            '<div class="list-row-actions">' +
            '<button class="btn btn-small btn-outline btn-move" data-entry-id="' + entry.id + '" data-target-list="' + otherList + '">' + moveLabel + '</button>' +
            '<button class="btn btn-small btn-danger btn-remove" data-entry-id="' + entry.id + '">Remove</button>' +
            '</div></div>'
        );
    });
}

function moveEntry(entryId, targetList) {
    Api.UserCountries.update(entryId, { listType: targetList })
        .done(function () {
            Common.showAlert("List updated.", "success");
            loadLists();
        })
        .fail(Common.showError);
}

function removeEntry(entryId) {
    if (!confirm("Remove this country from your list?")) return;
    Api.UserCountries.delete(entryId)
        .done(function () {
            Common.showAlert("Removed.", "success");
            loadLists();
        })
        .fail(Common.showError);
}
