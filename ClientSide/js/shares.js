/*
 * shares.js
 * Powers pages/shares.html: list all shares, filter by country, create a new
 * share, and edit/delete shares that belong to the logged-in user.
 */
$(function () {
    let countriesCache = [];
    let sharesCache = [];

    init();

    function init() {
        $("#newShareBox").toggle(Auth.isLoggedIn());
        loadCountriesIntoSelects();
        loadShares();

        $("#countryFilter").on("change", loadShares);

        $("#newShareForm").on("submit", function (e) {
            e.preventDefault();
            createShare();
        });

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

    function loadCountriesIntoSelects() {
        Api.Countries.getAll().done(function (countries) {
            countriesCache = countries;

            const $filter = $("#countryFilter");
            const $newShareCountry = $("#newShareCountry");
            countries.forEach(function (c) {
                $filter.append($("<option>").val(c.id).text(c.commonName));
                $newShareCountry.append($("<option>").val(c.id).text(c.commonName));
            });
        }).fail(Common.showError);
    }

    function loadShares() {
        const countryId = $("#countryFilter").val();
        const request = countryId ? Api.Shares.getByCountry(countryId) : Api.Shares.getAll();

        $("#sharesContainer").html('<p class="muted">Loading shares...</p>');
        request.done(function (shares) {
            sharesCache = shares;
            renderShares(shares);
        }).fail(Common.showError);
    }

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

    function createShare() {
        const user = Auth.getCurrentUser();
        const countryId = Number($("#newShareCountry").val());
        const country = countriesCache.find(function (c) { return c.id === countryId; });
        const content = $("#newShareContent").val().trim();
        if (!content || !country) return;

        Api.Shares.create({
            userId: user.id,
            countryId: countryId,
            userName: user.name,
            countryName: country.commonName,
            content: content
        }).done(function () {
            $("#newShareContent").val("");
            Common.showAlert("Share posted.", "success");
            loadShares();
        }).fail(Common.showError);
    }

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
