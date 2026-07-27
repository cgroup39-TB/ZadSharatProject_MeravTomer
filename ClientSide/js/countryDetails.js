/*
 * countryDetails.js
 * Powers two pages:
 *   - pages/country-details.html : read-only view + "add to my lists" + shares for that country
 *   - pages/country-form.html    : create/edit form (admin only), reused for both Add and Edit
 * Which block runs is decided by which container exists on the page.
 */
$(function () {
    if ($("#countryDetailsView").length) initDetailsPage();
    if ($("#countryForm").length) initFormPage();
});

// ===================== Details (view) page =====================

/**
 * Bootstraps the read-only details view: loads the country and its shares,
 * evaluates the "write a share" form's locked/unlocked state, and wires the
 * delete/add-to-list/share-form handlers.
 */
function initDetailsPage() {
    const id = Common.getQueryParams().id;
    if (!id) {
        Common.showAlert("No country id provided.", "error");
        return;
    }

    $("#editCountryBtn, #deleteCountryBtn").toggle(Auth.isAdmin());
    $("#userListActions").toggle(Auth.isLoggedIn());

    loadCountry(id);
    loadSharesForCountry(id);
    refreshShareFormState(id);

    $("#deleteCountryBtn").on("click", function () {
        if (!confirm("Delete this country? This cannot be undone.")) return;
        Api.Countries.delete(id)
            .done(function () {
                Common.showAlert("Country deleted.", "success");
                window.location.href = "countries-list.html";
            })
            .fail(Common.showError);
    });

    $("#addVisitedBtn").on("click", function () { addToList(id, "visited"); });
    $("#addWishlistBtn").on("click", function () { addToList(id, "wishlist"); });

    $("#shareForm").on("submit", function (e) {
        e.preventDefault();
        submitShare(id);
    });
}

/**
 * Fetches the country and renders it, or shows a "not found" message in the
 * details container if the request fails.
 */
function loadCountry(id) {
    Api.Countries.getById(id)
        .done(function (country) {
            renderCountry(country);
            $("#editCountryBtn").attr("href", "country-form.html?id=" + country.id);
        })
        .fail(function (err) {
            Common.showError(err);
            $("#countryDetailsView").html('<p class="muted">Country not found.</p>');
        });
}

/**
 * Renders the country header and detail list as a single HTML string.
 */
function renderCountry(country) {
    $("#countryDetailsView").html(
        '<div class="country-details-header">' +
        '<img src="' + country.flagPng + '" alt="' + country.commonName + ' flag" class="flag-large">' +
        '<div>' +
        '<h2>' + country.commonName + '</h2>' +
        '<p class="muted">' + country.officialName + '</p>' +
        '</div></div>' +
        '<dl class="details-list">' +
        '<dt>Capital</dt><dd>' + country.capital + '</dd>' +
        '<dt>Region</dt><dd>' + country.region + ' / ' + country.subregion + '</dd>' +
        '<dt>Population</dt><dd>' + Common.formatNumber(country.population) + '</dd>' +
        '<dt>Area</dt><dd>' + Common.formatNumber(country.area) + ' km&sup2;</dd>' +
        '<dt>Currency</dt><dd>' + country.currencyName + ' (' + country.currencySymbol + ')</dd>' +
        '<dt>Languages</dt><dd>' + country.languages.join(", ") + '</dd>' +
        '</dl>'
    );
}

/**
 * Adds the country to the user's visited or wishlist list; when it's added
 * as visited, also re-evaluates the share form's locked state since writing
 * a review now becomes possible.
 */
function addToList(countryId, listType) {
    const user = Auth.getCurrentUser();
    Api.UserCountries.create({ userId: user.id, countryId: Number(countryId), listType: listType })
        .done(function () {
            Common.showAlert("Added to your " + (listType === "visited" ? "visited" : "wishlist") + " list.", "success");
            if (listType === "visited") {
                refreshShareFormState(countryId);
            }
        })
        .fail(Common.showError);
}

// Reviews are stored as a field on the visited-country record itself, so
// writing one only makes sense once the country is actually in "My Lists".
// Shows the form (enabled) once visited, or a locked explanation until then.
<<<<<<< HEAD
=======
/**
 * Locks the share form by default, then unlocks it only if a fresh check of
 * the user's visited entries confirms this country is actually visited.
 */
>>>>>>> 1ae1bae4720eec596a5e22d21e582b0a22cff50d
function refreshShareFormState(countryId) {
    if (!Auth.isLoggedIn()) {
        $("#shareForm").hide();
        $("#shareLockedMsg").hide();
        return;
    }

<<<<<<< HEAD
=======
    // Fail closed: keep it locked until the visited check actually
    // confirms otherwise, instead of leaving the form's default (enabled)
    // HTML state in place if the status lookup itself fails.
    $("#shareForm").show();
    $("#shareContent, #shareSubmitBtn").prop("disabled", true);
    $("#shareLockedMsg").show();

>>>>>>> 1ae1bae4720eec596a5e22d21e582b0a22cff50d
    const user = Auth.getCurrentUser();
    Api.UserCountries.getByUser(user.id).done(function (entries) {
        const visited = (entries || []).some(function (e) {
            return e.listType === "visited" && Number(e.countryId) === Number(countryId);
        });

        $("#shareForm").show();
        $("#shareContent, #shareSubmitBtn").prop("disabled", !visited);
        $("#shareLockedMsg").toggle(!visited);
    });
}

<<<<<<< HEAD
=======
/**
 * Fetches the shares for this country and renders them.
 */
>>>>>>> 1ae1bae4720eec596a5e22d21e582b0a22cff50d
function loadSharesForCountry(countryId) {
    Api.Shares.getByCountry(countryId)
        .done(renderShareList)
        .fail(Common.showError);
}

/**
 * Renders the list of shares for the current country, or a placeholder
 * message when there are none yet.
 */
function renderShareList(shares) {
    const $list = $("#sharesList");
    $list.empty();

    if (!shares.length) {
        $list.html('<p class="muted">No shares yet for this country.</p>');
        return;
    }

    shares.forEach(function (share) {
        $list.append(
            '<div class="share-card">' +
            '<p class="share-meta"><strong>' + share.userName + '</strong> &middot; ' + share.createdAt + '</p>' +
            '<p>' + share.content + '</p>' +
            '</div>'
        );
    });
}

/**
 * Posts a new share using the current country's displayed name (read back
 * from the DOM rather than a stored variable) and reloads the share list.
 */
function submitShare(countryId) {
    const user = Auth.getCurrentUser();
    const country = $("#countryDetailsView h2").text();
    const content = $("#shareContent").val().trim();
    if (!content) return;

    Api.Shares.create({
        userId: user.id,
        countryId: Number(countryId),
        userName: user.name,
        countryName: country,
        content: content
    }).done(function () {
        $("#shareContent").val("");
        Common.showAlert("Share posted.", "success");
        loadSharesForCountry(countryId);
    }).fail(Common.showError);
}

// ===================== Add / Edit form page =====================

/**
 * Bootstraps the admin-only add/edit form: requires admin, loads existing
 * data to prefill when an id is present, and wires the submit handler.
 */
function initFormPage() {
    Auth.requireAdmin();

    const id = Common.getQueryParams().id;
    const isEdit = !!id;
    $("#formTitle").text(isEdit ? "Edit Country" : "Add Country");

    if (isEdit) {
        Api.Countries.getById(id)
            .done(fillForm)
            .fail(Common.showError);
    }

    $("#countryForm").on("submit", function (e) {
        e.preventDefault();
        saveCountry(isEdit ? id : null);
    });
}

function fillForm(country) {
    $("#apiCountryCode").val(country.apiCountryCode);
    $("#commonName").val(country.commonName);
    $("#officialName").val(country.officialName);
    $("#capital").val(country.capital);
    $("#region").val(country.region);
    $("#subregion").val(country.subregion);
    $("#population").val(country.population);
    $("#area").val(country.area);
    $("#flagPng").val(country.flagPng);
    $("#flagSvg").val(country.flagSvg);
    $("#currencyName").val(country.currencyName);
    $("#currencySymbol").val(country.currencySymbol);
    $("#languages").val(country.languages.join(", "));
}

/**
 * Reads the add/edit form into a data object; uppercases the country code
 * and splits the comma-separated languages field into a trimmed array.
 */
function readFormData() {
    return {
        apiCountryCode: $("#apiCountryCode").val().trim().toUpperCase(),
        commonName: $("#commonName").val().trim(),
        officialName: $("#officialName").val().trim(),
        capital: $("#capital").val().trim(),
        region: $("#region").val().trim(),
        subregion: $("#subregion").val().trim(),
        population: Number($("#population").val()),
        area: Number($("#area").val()),
        flagPng: $("#flagPng").val().trim(),
        flagSvg: $("#flagSvg").val().trim(),
        currencyName: $("#currencyName").val().trim(),
        currencySymbol: $("#currencySymbol").val().trim(),
        languages: $("#languages").val().split(",").map(function (l) { return l.trim(); }).filter(Boolean)
    };
}

/**
 * Creates or updates the country depending on whether id is set, then
 * redirects back to the list on success.
 */
function saveCountry(id) {
    const data = readFormData();
    const request = id ? Api.Countries.update(id, data) : Api.Countries.create(data);

    request
        .done(function () {
            Common.showAlert("Country saved.", "success");
            window.location.href = "countries-list.html";
        })
        .fail(Common.showError);
}
