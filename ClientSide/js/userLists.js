/*
 * userLists.js
 * Powers pages/my-lists.html: shows the logged-in user's "visited" and
 * "wishlist" countries, lets them move a country between lists or remove it.
 */
$(function () {
    let requestSeq = 0;
    let myReviewsCache = [];

    Auth.requireAuth();
    loadLists();
    loadMyReviews();

    $("#visitedList, #wishlistList").on("click", ".btn-move", function () {
        const entryId = $(this).data("entry-id");
        const newListType = $(this).data("target-list");
        moveEntry(entryId, newListType);
    });

    $("#visitedList, #wishlistList").on("click", ".btn-remove", function () {
        const entryId = $(this).data("entry-id");
        removeEntry(entryId);
    });

    // Opens the inline review box; pre-fills it with the country's existing
    // review (stashed on the element by renderList) so writing a second
    // review edits the first one instead of looking like a blank new form.
    $("#visitedList").on("click", ".btn-share", function () {
        const $inline = $(this).closest(".list-row").find(".share-inline");
        if ($inline.is(":visible")) {
            $inline.hide().empty();
            return;
        }
        const existingReview = $inline.data("existing-review") || "";
        $inline.html(
            '<textarea class="share-inline-textarea" rows="2" placeholder="Write your review...">' + existingReview + '</textarea>' +
            '<button class="btn btn-small btn-post-inline-share">' + (existingReview ? "Save" : "Post") + '</button>'
        ).show();
    });

    $("#visitedList").on("click", ".btn-post-inline-share", function () {
        const $inline = $(this).closest(".share-inline");
        const isEdit = !!$inline.data("existing-review");
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
            Common.showAlert(isEdit ? "Review updated." : "Review posted.", "success");
            $inline.hide().empty();
            loadLists();
            loadMyReviews();
        }).fail(Common.showError);
    });

    $("#myReviewsList").on("click", ".btn-edit-my-review", function () {
        startEditMyReview($(this).data("id"));
    });

    $("#myReviewsList").on("click", ".btn-save-my-review", function () {
        saveEditMyReview($(this).data("id"));
    });

    $("#myReviewsList").on("click", ".btn-cancel-my-review", function () {
        renderMyReviews(myReviewsCache);
    });

    $("#myReviewsList").on("click", ".btn-delete-my-review", function () {
        deleteMyReview($(this).data("id"));
    });

    /**
     * Loads both the visited and wishlist entries plus the full country catalog
     * in parallel via $.when, then renders each list. See the comment below on
     * why neither result is [0]-indexed. Tagged with an incrementing sequence
     * number (same pattern as countries.js's loadCountries()) so that a
     * slower/older response - e.g. from the initial page load, or from a
     * previous move/remove action - can't land after a newer one and clobber
     * the render with stale data.
     */
    function loadLists() {
        const user = Auth.getCurrentUser();
        const seq = ++requestSeq;

        $("#visitedList").html('<p class="muted">Loading...</p>');
        $("#wishlistList").html('<p class="muted">Loading...</p>');

        // Both of these are already single-value-resolving promises (each ends
        // in its own .then() that returns one array), not raw $.ajax() promises -
        // so $.when hands them back as-is, not wrapped in a [data, status, jqXHR]
        // array. Indexing with [0] here would silently grab the first *entry*/
        // *country* instead of the array, and .filter() on that throws, which
        // jQuery swallows instead of routing to .fail() - the lists just stay
        // empty with no visible error.
        $.when(Api.UserCountries.getByUser(user.id), Api.Countries.getAll())
            .done(function (entries, countries) {
                if (seq !== requestSeq) return;
                renderList(entries, countries, "visited", "#visitedList");
                renderList(entries, countries, "wishlist", "#wishlistList");
            })
            .fail(function (err) {
                if (seq !== requestSeq) return;
                Common.showError(err);
            });
    }

    /**
     * Moves an entry to the other list (visited <-> wishlist) by updating its
     * listType, then reloads both lists to reflect the change.
     */
    function moveEntry(entryId, targetList) {
        Api.UserCountries.update(entryId, { listType: targetList })
            .done(function () {
                Common.showAlert("List updated.", "success");
                loadLists();
            })
            .fail(Common.showError);
    }

    /**
     * Removes an entry from whichever list it belongs to, after a confirm
     * prompt, then reloads both lists.
     */
    function removeEntry(entryId) {
        if (!confirm("Remove this country from your list?")) return;
        Api.UserCountries.delete(entryId)
            .done(function () {
                Common.showAlert("Removed.", "success");
                loadLists();
            })
            .fail(Common.showError);
    }

    /**
     * Loads every review (share) the logged-in user has posted, across all
     * countries, and renders them as their own list on this page.
     */
    function loadMyReviews() {
        const user = Auth.getCurrentUser();
        $("#myReviewsList").html('<p class="muted">Loading...</p>');

        Api.Shares.getAll().done(function (shares) {
            myReviewsCache = (shares || []).filter(function (s) { return s.userId === user.id; });
            renderMyReviews(myReviewsCache);
        }).fail(Common.showError);
    }

    /**
     * Switches one of my-review's cards into inline-edit mode, same pattern
     * as shares.js's startEdit/saveEdit.
     */
    function startEditMyReview(shareId) {
        const review = myReviewsCache.find(function (r) { return r.id === shareId; });
        if (!review) return;

        const $card = $('.share-card[data-share-id="' + shareId + '"]');
        $card.find(".share-content").replaceWith(
            '<textarea class="edit-share-textarea">' + review.content + '</textarea>'
        );
        $card.find(".card-actions").html(
            '<button class="btn btn-small btn-save-my-review" data-id="' + shareId + '">Save</button>' +
            '<button class="btn btn-small btn-outline btn-cancel-my-review">Cancel</button>'
        );
    }

    function saveEditMyReview(shareId) {
        const $card = $('.share-card[data-share-id="' + shareId + '"]');
        const newContent = $card.find(".edit-share-textarea").val().trim();
        if (!newContent) return;

        Api.Shares.update(shareId, { content: newContent })
            .done(function () {
                Common.showAlert("Review updated.", "success");
                loadMyReviews();
                loadLists();
            })
            .fail(Common.showError);
    }

    function deleteMyReview(shareId) {
        if (!confirm("Delete this review?")) return;
        Api.Shares.delete(shareId)
            .done(function () {
                Common.showAlert("Review deleted.", "success");
                loadMyReviews();
                loadLists();
            })
            .fail(Common.showError);
    }
});

/**
 * Renders the logged-in user's own reviews (one card per country reviewed).
 */
function renderMyReviews(reviews) {
    const $container = $("#myReviewsList");
    $container.empty();

    if (!reviews.length) {
        $container.html('<p class="muted">You haven\'t posted any reviews yet.</p>');
        return;
    }

    reviews.forEach(function (review) {
        const $card = $('<div class="share-card" data-share-id="' + review.id + '"></div>');
        $card.append(
            '<p class="share-meta"><a href="country-details.html?id=' + review.countryId + '">' + review.countryName + '</a>' +
            ' &middot; ' + review.createdAt + '</p>'
        );
        $card.append('<p class="share-content">' + review.content + '</p>');
        $card.append(
            '<div class="card-actions">' +
            '<button class="btn btn-small btn-outline btn-edit-my-review" data-id="' + review.id + '">Edit</button>' +
            '<button class="btn btn-small btn-danger btn-delete-my-review" data-id="' + review.id + '">Delete</button>' +
            '</div>'
        );
        $container.append($card);
    });
}

/**
 * Renders one list (visited or wishlist) by filtering entries to listType
 * and joining each to its country record; entries whose country can't be
 * found (e.g. stale data) are silently skipped.
 */
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

        const hasReview = !!entry.reviewText;
        const shareButton = listType === "visited"
            ? '<button class="btn btn-small btn-share">' + (hasReview ? "Edit Review" : "Write a Review") + '</button>'
            : '';

        const $row = $(
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

        // Stashed via .data() rather than baked into the HTML string above,
        // since review text can contain quotes/special characters that would
        // break an inline HTML attribute.
        if (listType === "visited") {
            $row.find(".share-inline").data("existing-review", entry.reviewText || "");
        }

        $container.append($row);
    });
}
