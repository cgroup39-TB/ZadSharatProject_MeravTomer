/*
 * shares.js
 * Powers pages/shares.html: list all shares, filter by country, create a new
 * share, and edit/delete shares that belong to the logged-in user.
 */
$(function () {
    let countriesCache = [];
    let sharesCache = [];

    init();

    /**
     * Bootstraps the page: shows the new-share box only when logged in,
     * loads countries into both selects, loads the shares list, and wires
     * up filter/create/edit/delete/cancel handlers.
     */
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

    /**
     * Loads the full country catalog, caches it, fills the country filter
     * dropdown, and then narrows the new-share country select to only
     * visited countries.
     */
    function loadCountriesIntoSelects() {
        Api.Countries.getAll().done(function (countries) {
            countriesCache = countries;

            const $filter = $("#countryFilter");
            countries.forEach(function (c) {
                $filter.append($("<option>").val(c.id).text(c.commonName));
<<<<<<< HEAD
            });

            loadVisitedCountriesIntoNewShareSelect(countries);
        }).fail(Common.showError);
    }

    // A share is a review on a visited country, so only countries already
    // in the logged-in user's visited list can be picked here.
    function loadVisitedCountriesIntoNewShareSelect(countries) {
        const $newShareCountry = $("#newShareCountry");
        $newShareCountry.empty();

        const user = Auth.getCurrentUser();
        if (!user) return;

        Api.UserCountries.getByUser(user.id).done(function (entries) {
            const visitedIds = (entries || [])
                .filter(function (e) { return e.listType === "visited"; })
                .map(function (e) { return Number(e.countryId); });

            const visitedCountries = countries.filter(function (c) { return visitedIds.indexOf(c.id) !== -1; });

            if (!visitedCountries.length) {
                $newShareCountry.append($("<option>").val("").text("Mark a country as visited first"));
                $("#newShareContent, #newShareSubmitBtn").prop("disabled", true);
                return;
            }

            visitedCountries.forEach(function (c) {
                $newShareCountry.append($("<option>").val(c.id).text(c.commonName));
            });
        });
=======
            });

            loadVisitedCountriesIntoNewShareSelect(countries);
        }).fail(Common.showError);
>>>>>>> 1ae1bae4720eec596a5e22d21e582b0a22cff50d
    }

    // A share is a review on a visited country, so only countries already
    // in the logged-in user's visited list can be picked here.
    /**
     * Rebuilds the new-share country select from the user's visited
     * countries only, disabling the content field/submit button until at
     * least one visited country is available to pick.
     */
    function loadVisitedCountriesIntoNewShareSelect(countries) {
        const $newShareCountry = $("#newShareCountry");
        $newShareCountry.empty();
        // Fail closed until the visited check actually confirms a country -
        // an empty select falls back safely on its own (createShare() finds
        // no matching country), but keep the button disabled too so it
        // doesn't just look broken while it's blocked.
        $("#newShareContent, #newShareSubmitBtn").prop("disabled", true);

        const user = Auth.getCurrentUser();
        if (!user) return;

        Api.UserCountries.getByUser(user.id).done(function (entries) {
            const visitedIds = (entries || [])
                .filter(function (e) { return e.listType === "visited"; })
                .map(function (e) { return Number(e.countryId); });

            const visitedCountries = countries.filter(function (c) { return visitedIds.indexOf(c.id) !== -1; });

            if (!visitedCountries.length) {
                $newShareCountry.append($("<option>").val("").text("Mark a country as visited first"));
                $("#newShareContent, #newShareSubmitBtn").prop("disabled", true);
                return;
            }

            visitedCountries.forEach(function (c) {
                $newShareCountry.append($("<option>").val(c.id).text(c.commonName));
            });
            $("#newShareContent, #newShareSubmitBtn").prop("disabled", false);
        });
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
     * Creates a new share from the form, resolving the selected country id
     * against the cached country list to get its display name.
     */
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
