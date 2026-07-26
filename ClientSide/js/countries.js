/*
 * countries.js
 * Logic for pages/countries-list.html: render the country grid, and wire up
 * search / filter / sort controls. Uses Api.Countries.search() so swapping
 * mock -> real server later needs no changes here.
 */
$(function () {

    let allCountries = [];
    let requestSeq = 0;

    init();

    /**
     * Bootstraps the page: toggles admin/logged-in-only controls, fills the
     * region filter, wires up event handlers, and triggers the first load.
     */
    function init() {
        $("#addCountryBtn").toggle(Auth.isAdmin());
        $("#myStatusFilterRow").toggle(Auth.isLoggedIn());
        populateRegionFilter();
        bindEvents();
        loadCountries();
    }

    /**
     * Wires the search form, reset button, and sort/status controls so any
     * of them re-runs loadCountries().
     */
    function bindEvents() {
        $("#searchForm").on("submit", function (e) {
            e.preventDefault();
            loadCountries();
        });
        $("#resetFiltersBtn").on("click", function () {
            $("#searchForm")[0].reset();
            loadCountries();
        });
        $("#sortBy, #sortDir, #myStatusFilter").on("change", loadCountries);
    }

    /**
     * Fills the region filter dropdown from a hardcoded region list rather
     * than deriving it from the loaded countries.
     */
    function populateRegionFilter() {
        // Static list is enough for a student project; could also be derived from Api.Countries.getAll()
        const regions = ["Africa", "Americas", "Asia", "Europe", "Oceania"];
        const $select = $("#regionFilter");
        regions.forEach(function (region) {
            $select.append($("<option>").val(region).text(region));
        });
    }

    /**
     * Reads and trims every search/filter/sort control into a single params
     * object shaped for Api.Countries.search().
     */
    function readFiltersFromForm() {
        return {
            name: $("#nameSearch").val().trim(),
            region: $("#regionFilter").val(),
            language: $("#languageFilter").val().trim(),
            currency: $("#currencyFilter").val().trim(),
            minPopulation: $("#minPopulation").val(),
            maxPopulation: $("#maxPopulation").val(),
            minArea: $("#minArea").val(),
            maxArea: $("#maxArea").val(),
            sortBy: $("#sortBy").val(),
            sortDir: $("#sortDir").val()
        };
    }

    /**
     * Issues a new search request tagged with an incrementing sequence
     * number, so a slower/older response that arrives after a newer request
     * was issued can be detected and ignored instead of clobbering it.
     */
    function loadCountries() {
        const params = readFiltersFromForm();
        const seq = ++requestSeq;
        $("#countriesGrid").html('<p class="muted">Loading countries...</p>');

        Api.Countries.search(params)
            .done(function (countries) {
                // A newer request (e.g. the user changed the sort right after
                // page load) may have already been issued - ignore this
                // response so it can't clobber a more recent one.
                if (seq !== requestSeq) return;
                applyMyStatusFilter(countries, seq);
            })
            .fail(function (err) {
                if (seq !== requestSeq) return;
                Common.showError(err);
            });
    }

    /**
     * When a "my status" filter is set, fetches the user's visited/wishlist
     * entries and narrows the already-loaded countries by them; re-checks
     * seq against requestSeq so a stale call can't overwrite a newer one.
     */
    function applyMyStatusFilter(countries, seq) {
        const statusFilter = $("#myStatusFilter").val();
        if (!statusFilter || !Auth.isLoggedIn()) {
            allCountries = countries;
            renderCountries(countries);
            return;
        }

        const user = Auth.getCurrentUser();
        Api.UserCountries.getByUser(user.id).done(function (entries) {
            if (seq !== requestSeq) return;

            const statusByCountry = {};
            (entries || []).forEach(function (e) { statusByCountry[e.countryId] = e.listType; });

            const filtered = countries.filter(function (c) {
                const status = statusByCountry[c.id];
                return statusFilter === "none" ? !status : status === statusFilter;
            });

            allCountries = filtered;
            renderCountries(filtered);
        });
    }

    /**
     * Renders the country grid and (re)binds the delete-button click handler
     * on every call, since the buttons themselves are recreated each render.
     */
    function renderCountries(countries) {
        const $grid = $("#countriesGrid");
        $grid.empty();

        if (!countries.length) {
            $grid.html('<p class="muted">No countries match your filters.</p>');
            return;
        }

        countries.forEach(function (country) {
            const $card = $('<div class="country-card"></div>');
            $card.append('<img src="' + country.flagPng + '" alt="' + country.commonName + ' flag" class="flag-thumb">');
            $card.append('<h3>' + country.commonName + '</h3>');
            $card.append('<p>' + country.region + ' &middot; ' + country.capital + '</p>');
            $card.append('<p>Population: ' + Common.formatNumber(country.population) + '</p>');
            $card.append('<p>Area: ' + Common.formatNumber(country.area) + ' km&sup2;</p>');

            const $actions = $('<div class="card-actions"></div>');
            $actions.append('<a class="btn btn-small" href="country-details.html?id=' + country.id + '">View</a>');

            if (Auth.isAdmin()) {
                $actions.append('<a class="btn btn-small btn-outline" href="country-form.html?id=' + country.id + '">Edit</a>');
                $actions.append('<button class="btn btn-small btn-danger" data-id="' + country.id + '">Delete</button>');
            }

            $card.append($actions);
            $grid.append($card);
        });

        $grid.find(".btn-danger").on("click", function () {
            const id = $(this).data("id");
            if (!confirm("Delete this country?")) return;
            Api.Countries.delete(id)
                .done(function () {
                    Common.showAlert("Country deleted.", "success");
                    loadCountries();
                })
                .fail(Common.showError);
        });
    }
});
