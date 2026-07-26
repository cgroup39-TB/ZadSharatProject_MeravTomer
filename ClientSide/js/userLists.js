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

    $("#visitedList").on("click", ".btn-share", function () {
        const $inline = $(this).closest(".list-row").find(".share-inline");
        if ($inline.is(":visible")) {
            $inline.hide().empty();
            return;
        }
        $inline.html(
            '<textarea class="share-inline-textarea" rows="2" placeholder="Write your review..."></textarea>' +
            '<button class="btn btn-small btn-post-inline-share">Post</button>'
        ).show();
    });

    $("#visitedList").on("click", ".btn-post-inline-share", function () {
        const $inline = $(this).closest(".share-inline");
        const content = $inline.find(".share-inline-textarea").val().trim();
        if (!content) return;

        const user = Auth.getCurrentUser();
        Api.Shares.create({
            userId: user.id,
            countryId: Number($inline.data("country-id")),
            userName: user.name,
            countryName: $inline.data("country-name"),
            content: content
        }).done(function () {
            Common.showAlert("Review posted.", "success");
            $inline.hide().empty();
        }).fail(Common.showError);
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

        const shareButton = listType === "visited"
            ? '<button class="btn btn-small btn-share">Write a Review</button>'
            : '';

        $container.append(
            '<div class="list-row">' +
            '<img src="' + country.flagPng + '" class="flag-thumb-small" alt="">' +
            '<a href="country-details.html?id=' + country.id + '">' + country.commonName + '</a>' +
            '<div class="list-row-actions">' +
            shareButton +
            '<button class="btn btn-small btn-outline btn-move" data-entry-id="' + entry.id + '" data-target-list="' + otherList + '">' + moveLabel + '</button>' +
            '<button class="btn btn-small btn-danger btn-remove" data-entry-id="' + entry.id + '">Remove</button>' +
            '</div>' +
            (listType === "visited"
                ? '<div class="share-inline" data-country-id="' + country.id + '" data-country-name="' + country.commonName + '" style="display:none; width:100%; margin-top:8px;"></div>'
                : '') +
            '</div>'
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
