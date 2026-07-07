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

function addToList(countryId, listType) {
    const user = Auth.getCurrentUser();
    Api.UserCountries.create({ userId: user.id, countryId: Number(countryId), listType: listType })
        .done(function () {
            Common.showAlert("Added to your " + (listType === "visited" ? "visited" : "wishlist") + " list.", "success");
        })
        .fail(Common.showError);
}

function loadSharesForCountry(countryId) {
    Api.Shares.getByCountry(countryId)
        .done(renderShareList)
        .fail(Common.showError);
}

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
