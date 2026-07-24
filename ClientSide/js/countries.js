/*
 * countries.js
 * Logic for pages/countries-list.html: render the country grid, and wire up
 * search / filter / sort controls. Uses Api.Countries.search() so swapping
 * mock -> real server later needs no changes here.
 */
$(function () {

    let allCountries = [];

    init();

    function init() {
        $("#addCountryBtn").toggle(Auth.isAdmin());
        populateRegionFilter();
        bindEvents();
        loadCountries();
    }

    function bindEvents() {
        $("#searchForm").on("submit", function (e) {
            e.preventDefault();
            loadCountries();
        });
        $("#resetFiltersBtn").on("click", function () {
            $("#searchForm")[0].reset();
            loadCountries();
        });
        $("#sortBy, #sortDir").on("change", loadCountries);
    }

    function populateRegionFilter() {
        // Static list is enough for a student project; could also be derived from Api.Countries.getAll()
        const regions = ["Africa", "Americas", "Asia", "Europe", "Oceania"];
        const $select = $("#regionFilter");
        regions.forEach(function (region) {
            $select.append($("<option>").val(region).text(region));
        });
    }

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

    function loadCountries() {
        const params = readFiltersFromForm();
        $("#countriesGrid").html('<p class="muted">Loading countries...</p>');

        Api.Countries.search(params)
            .done(function (countries) {
                allCountries = countries;
                renderCountries(countries);
            })
            .fail(Common.showError);
    }

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
