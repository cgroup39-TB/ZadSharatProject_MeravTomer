/*
 * shares.js
 * Powers pages/shares.html: list all shares, filter by country, and
 * edit/delete shares that belong to the logged-in user. Writing a new review
 * is only available from my-lists.html, not from this page.
 */
$(function () {
    let sharesCache = [];

    init();

    /**
     * Bootstraps the page: fills the country filter, loads the shares list,
     * and wires up filter/edit/delete/cancel handlers.
     */
    function init() {
        loadCountriesIntoFilter();
        loadShares();

        $("#countryFilter").on("change", loadShares);

        $("#sharesContainer").on("click", ".btn-delete-share", function () {
            deleteShare($(this).data("id"));
        });

        $("#sharesContainer").on("click", ".btn-edit-share", function () {
            startEdit($(this).data("id"));
        });

        $("#sharesContainer").on("click", ".btn-save-share", function () {
            saveEdit($(this).data("id"));
        });

        $("#sharesContainer").on("click", ".btn-cancel-edit", function () {
            renderShares(sharesCache);
        });
    }

    /**
     * Fills the country filter dropdown from the full country catalog.
     */
    function loadCountriesIntoFilter() {
        Api.Countries.getAll().done(function (countries) {
            const $filter = $("#countryFilter");
            countries.forEach(function (c) {
                $filter.append($("<option>").val(c.id).text(c.commonName));
            });
        }).fail(Common.showError);
    }

    /**
     * Loads either all shares or shares for the selected country filter,
     * caches the result for later edit/lookup, and renders it.
     */
    function loadShares() {
        const countryId = $("#countryFilter").val();
        const request = countryId ? Api.Shares.getByCountry(countryId) : Api.Shares.getAll();

        $("#sharesContainer").html('<p class="muted">Loading shares...</p>');
        request.done(function (shares) {
            sharesCache = shares;
            renderShares(shares);
        }).fail(Common.showError);
    }

    /**
     * Renders the shares list, showing edit/delete buttons only on cards
     * owned by the currently logged-in user.
     */
    function renderShares(shares) {
        const $container = $("#sharesContainer");
        $container.empty();

        if (!shares.length) {
            $container.html('<p class="muted">No shares yet.</p>');
            return;
        }

        const currentUser = Auth.getCurrentUser();

        shares.forEach(function (share) {
            const isOwner = currentUser && currentUser.id === share.userId;
            const $card = $('<div class="share-card" data-share-id="' + share.id + '"></div>');

            $card.append(
                '<p class="share-meta"><strong>' + share.userName + '</strong> on ' +
                '<a href="country-details.html?id=' + share.countryId + '">' + share.countryName + '</a>' +
                ' &middot; ' + share.createdAt + '</p>'
            );
            $card.append('<p class="share-content">' + share.content + '</p>');

            if (isOwner) {
                $card.append(
                    '<div class="card-actions">' +
                    '<button class="btn btn-small btn-outline btn-edit-share" data-id="' + share.id + '">Edit</button>' +
                    '<button class="btn btn-small btn-danger btn-delete-share" data-id="' + share.id + '">Delete</button>' +
                    '</div>'
                );
            }

            $container.append($card);
        });
    }

    /**
     * Switches a share card into inline-edit mode by swapping its content
     * for a textarea and its action buttons for save/cancel.
     */
    function startEdit(shareId) {
        const share = sharesCache.find(function (s) { return s.id === shareId; });
        if (!share) return;

        const $card = $('.share-card[data-share-id="' + shareId + '"]');
        $card.find(".share-content").replaceWith(
            '<textarea class="edit-share-textarea">' + share.content + '</textarea>'
        );
        $card.find(".card-actions").html(
            '<button class="btn btn-small btn-save-share" data-id="' + shareId + '">Save</button>' +
            '<button class="btn btn-small btn-outline btn-cancel-edit">Cancel</button>'
        );
    }

    /**
     * Reads the inline edit textarea's value and persists it as the share's
     * updated content.
     */
    function saveEdit(shareId) {
        const $card = $('.share-card[data-share-id="' + shareId + '"]');
        const newContent = $card.find(".edit-share-textarea").val().trim();
        if (!newContent) return;

        Api.Shares.update(shareId, { content: newContent })
            .done(function () {
                Common.showAlert("Share updated.", "success");
                loadShares();
            })
            .fail(Common.showError);
    }

    /**
     * Deletes a share after a confirm prompt.
     */
    function deleteShare(shareId) {
        if (!confirm("Delete this share?")) return;
        Api.Shares.delete(shareId)
            .done(function () {
                Common.showAlert("Share deleted.", "success");
                loadShares();
            })
            .fail(Common.showError);
    }
});
