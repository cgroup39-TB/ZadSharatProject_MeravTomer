/*
 * countries.js
 * Logic for pages/countries-list.html: render the country grid, and wire up
 * search / filter / sort controls. Uses Api.Countries.search() so swapping
 * mock -> real server later needs no changes here.
 */
$(function () {

    let allCountries = [];
    let requestSeq = 0;
    let recommendationsCache = null;

    init();

    /**
     * Bootstraps the page: toggles admin/logged-in-only controls, fills the
     * region filter, wires up event handlers, and triggers the first load.
     */
    function init() {
        $("#myStatusFilterRow").toggle(Auth.isLoggedIn());
        $("#showRecommendedBtn").toggle(Auth.isLoggedIn());
        populateRegionFilter();
        bindEvents();
        loadCountries();
    }

    /**
     * Wires the search form, reset button, sort/status controls, and the
     * recommended-countries toggle button.
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
        $("#showRecommendedBtn").on("click", toggleRecommendations);
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

    /**
     * Toggles the "Recommended for You" section open/closed on button click;
     * fetches the recommendations on first open only, then reuses the cached
     * result on subsequent toggles.
     */
    function toggleRecommendations() {
        const $section = $("#recommendedSection");
        if ($section.is(":visible")) {
            $section.hide();
            return;
        }

        $section.show();
        if (recommendationsCache) {
            renderRecommendations(recommendationsCache);
            return;
        }
        loadRecommendations();
    }

    /**
     * Loads countries matching the logged-in user's preferred continents/
     * languages (excluding anything already on their visited/wishlist).
     */
    function loadRecommendations() {
        const user = Auth.getCurrentUser();
        $("#recommendedGrid").html('<p class="muted">Loading...</p>');

        Api.Recommendations.getForUser(user.id)
            .done(function (recommendations) {
                recommendationsCache = recommendations.slice(0, 6);
                renderRecommendations(recommendationsCache);
            })
            .fail(function () {
                recommendationsCache = [];
                renderRecommendations(recommendationsCache);
            });
    }

    /**
     * Renders up to a handful of recommended country cards, each labeled
     * with which of the user's preferences it matched (region and/or
     * spoken languages).
     */
    function renderRecommendations(recommendations) {
        const $grid = $("#recommendedGrid").empty();

        if (!recommendations.length) {
            $grid.html('<p class="muted">No recommendations yet - set your preferred continents/languages in My Preferences.</p>');
            return;
        }

        recommendations.forEach(function (rec) {
            const country = rec.country;
            const reasons = [];
            if (rec.matchedRegion) reasons.push(country.region);
            reasons.push.apply(reasons, rec.matchedLanguages);

            const $card = $('<div class="country-card"></div>');
            $card.append('<img src="' + country.flagPng + '" alt="' + country.commonName + ' flag" class="flag-thumb">');
            $card.append('<h3>' + country.commonName + '</h3>');
            $card.append('<p class="muted">Matches: ' + reasons.join(", ") + '</p>');
            $card.append('<div class="card-actions"><a class="btn btn-small" href="country-details.html?id=' + country.id + '">View</a></div>');
            $grid.append($card);
        });
    }
});
