/*
 * api.js
 * Single place that talks to the backend. Every other JS file calls Api.* and
 * never touches $.ajax or Mock.* directly.
 *
 * Each function has two branches:
 *   - CONFIG.USE_MOCK === true  -> resolves/rejects using mockData.js (fake latency included)
 *   - CONFIG.USE_MOCK === false -> real $.ajax call to CONFIG.API_BASE_URL
 *
 * The real server (see CountriesProject_MeravTomer/Controllers) uses different
 * route names/shapes than the original mock contract in a few places (no
 * generic search/UserCountries/Shares/Admin resources - those are composed
 * here out of the real endpoints: Countries, Users, UserVisitedCountries,
 * Users/{id}/wantedCountries, Users/{id}/regions, Users/{id}/languages,
 * Users/statistics, Quizzes). This file is the only place that knows about
 * that mapping - every other page keeps using the original mock-era shapes
 * (country.commonName, share.userName, userCountry.listType, ...).
 *
 * All functions return a jQuery Promise, so callers use them the same way regardless
 * of mock/real: Api.Countries.getAll().done(function(data){...}).fail(function(err){...});
 */
const Api = (function () {

    // ---------- generic helpers ----------

    /**
     * Wraps a synchronous work function as an async jQuery promise with a fake network delay (CONFIG.MOCK_DELAY_MS).
     * A thrown error inside workFn is turned into a rejected promise shaped like a real jqXHR failure ({responseJSON:{message}}).
     */
    function mockResponse(workFn) {
        const dfd = $.Deferred();
        setTimeout(function () {
            try {
                const result = workFn();
                dfd.resolve(result);
            } catch (err) {
                dfd.reject({ responseJSON: { message: err.message || "Mock error" } });
            }
        }, CONFIG.MOCK_DELAY_MS);
        return dfd.promise();
    }

    /**
     * Performs the real (non-mock) $.ajax call to CONFIG.API_BASE_URL, JSON-encoding the body and attaching the auth header.
     * Returns the raw $.ajax() promise - $.when() wraps this in a [data, status, jqXHR] array when combined with other promises.
     */
    function realRequest(method, path, data) {
        return $.ajax({
            url: CONFIG.API_BASE_URL + path,
            method: method,
            contentType: "application/json",
            dataType: "json",
            data: data !== undefined ? JSON.stringify(data) : undefined,
            headers: Auth.getAuthHeader ? Auth.getAuthHeader() : {}
        });
    }

    /** Returns a shallow copy of a user object with the password field removed. */
    function sanitizeUser(user) {
        if (!user) return user;
        const copy = $.extend({}, user);
        delete copy.password;
        return copy;
    }

    // ---------- server DTO -> client view-model mappers ----------

    // Server Country (Controllers/CountriesController.cs, BL/Country.cs):
    //   { countryId, cca3, name, capital, region:{regionId,regionName}, subRegion,
    //     population, area, flagUrl, borders[], languages:[{languageId,languageName}],
    //     currencies:[{currencyId,currencyCode,name,symbol}] }
    /** Maps a server Country DTO to the client's flat view-model shape; reuses the single server flag URL for both flagPng and flagSvg and only keeps the first currency. */
    function mapCountryFromServer(c) {
        if (!c) return c;
        const firstCurrency = (c.currencies && c.currencies[0]) || {};
        return {
            id: c.countryId,
            apiCountryCode: c.cca3,
            commonName: c.name,
            officialName: c.name, // server has no separate "official name"
            capital: c.capital,
            region: c.region ? c.region.regionName : "",
            subregion: c.subRegion,
            population: c.population,
            area: c.area,
            flagPng: c.flagUrl,
            flagSvg: c.flagUrl, // server only stores a single flag url
            currencyName: firstCurrency.name || "",
            currencySymbol: firstCurrency.symbol || "",
            languages: (c.languages || []).map(function (l) { return l.languageName; })
        };
    }

    // Server User (BL/User.cs): { userId, name, email, password, isActive, isAdmin, canShare }
    /** Maps a server User DTO to the client's view-model shape and strips the password via sanitizeUser. */
    function mapUserFromServer(u) {
        if (!u) return u;
        return sanitizeUser({
            id: u.userId,
            name: u.name,
            email: u.email,
            active: u.isActive,
            isAdmin: u.isAdmin,
            canShare: u.canShare
        });
    }

    // Finds the region object (with its real RegionId) of an existing country
    // with the given region name - there's no dedicated GET /Regions endpoint,
    // so this is derived from the countries that already carry one.
    /**
     * Returns a `.then()`-derived promise: the underlying $.ajax() result is transformed into a plain region object (or null) via `.then()`,
     * so it resolves with that single value directly (not the [data,status,jqXHR] triple) - `$.when()` callers must NOT index the result with [0].
     */
    function findRegionRaw(regionName) {
        return realRequest("GET", "/Countries").then(function (countries) {
            const match = (countries || []).find(function (c) {
                return c.region && c.region.regionName === regionName;
            });
            return match ? match.region : null;
        });
    }

    // ---------- Users ----------
    // Real endpoints:
    //   POST /api/Users/register
    //   POST /api/Users/login
    //   PUT  /api/Users/{id}/profile?actingUserId=...
    //   GET  /api/Users/{id}
    const Users = {
        /** Registers a new user. Mock branch pre-checks for a duplicate email and throws; real branch just posts to /Users/register (raw $.ajax() promise) and lets the server validate. */
        register: function (userData) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const emailTaken = users.some(function (u) { return u.email.toLowerCase() === userData.email.toLowerCase(); });
                    if (emailTaken) throw new Error("This email is already registered.");
                    const newUser = {
                        id: Mock.nextUserId(),
                        name: userData.name,
                        email: userData.email,
                        password: userData.password,
                        active: true,
                        isAdmin: false,
                        preferences: { continents: [], countries: [], languages: [] }
                    };
                    users.push(newUser);
                    Mock.saveUsers(users);
                    return sanitizeUser(newUser);
                });
            }
            return realRequest("POST", "/Users/register", userData);
        },

        /**
         * Logs in a user. The real server issues no JWT, so a lightweight local session token ("session-{id}-{timestamp}") is synthesized client-side from the returned user row.
         * Real branch is a `.then()`-derived promise - resolves directly with {token, user}, not the [data,status,jqXHR] triple, so `$.when()` callers must NOT index the result with [0].
         */
        login: function (credentials) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const user = users.find(function (u) { return u.email.toLowerCase() === credentials.email.toLowerCase(); });
                    if (!user || user.password !== credentials.password) throw new Error("Invalid email or password.");
                    if (!user.active) throw new Error("This account is locked. Please contact an administrator.");

                    const log = Mock.getLoginLog();
                    log.push({ userId: user.id, date: new Date().toISOString().slice(0, 10) });
                    Mock.saveLoginLog(log);

                    return { token: "mock-token-" + user.id + "-" + Date.now(), user: sanitizeUser(user) };
                });
            }
            // The real server has no JWT issuance yet - it just validates the
            // credentials and returns the user row, so a lightweight local
            // session token is synthesized here (same role as the mock token).
            return realRequest("POST", "/Users/login", credentials).then(function (user) {
                return {
                    token: "session-" + user.userId + "-" + Date.now(),
                    user: mapUserFromServer(user)
                };
            });
        },

        /**
         * Updates a user's profile. Real branch is a `.then()`-derived promise that ultimately resolves with a single mapped user object
         * (not the [data,status,jqXHR] triple), so `$.when()` callers must NOT index the result with [0].
         */
        update: function (id, data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const idx = users.findIndex(function (u) { return u.id === Number(id); });
                    if (idx === -1) throw new Error("User not found.");
                    users[idx] = $.extend({}, users[idx], data, { id: users[idx].id });
                    Mock.saveUsers(users);
                    return sanitizeUser(users[idx]);
                });
            }

            // "preferences" updates (continents/countries/languages) aren't
            // backed by a matching field on the real User - the server keeps
            // them as separate resources (Users/{id}/regions, .../languages)
            // and there is no way yet to read them back onto the logged-in
            // user object, so that part of the Preferences page is UI-only
            // for now and is accepted without a network call.
            if (data && data.preferences && data.name === undefined && data.email === undefined) {
                return $.Deferred().resolve(Auth.getCurrentUser()).promise();
            }

            const actingUser = Auth.getCurrentUser();

            // PUT /Users/{id}/profile overwrites every column, so the full
            // current row is fetched first and merged with the requested change.
            return realRequest("GET", "/Users/" + id).then(function (current) {
                const merged = {
                    userId: current.userId,
                    name: data.name !== undefined ? data.name : current.name,
                    email: data.email !== undefined ? data.email : current.email,
                    password: current.password,
                    isActive: current.isActive,
                    isAdmin: current.isAdmin,
                    canShare: current.canShare
                };

                return realRequest(
                    "PUT",
                    "/Users/" + id + "/profile?actingUserId=" + (actingUser ? actingUser.id : id),
                    merged
                );
            }).then(function () {
                return realRequest("GET", "/Users/" + id).then(mapUserFromServer);
            });
        },

        /** Fetches a user by id. Real branch is `.then()`-derived (maps through mapUserFromServer), resolving with a single object - not the [data,status,jqXHR] triple. */
        getById: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const user = Mock.getUsers().find(function (u) { return u.id === Number(id); });
                    if (!user) throw new Error("User not found.");
                    return sanitizeUser(user);
                });
            }
            return realRequest("GET", "/Users/" + id).then(mapUserFromServer);
        }
    };

    // ---------- Countries ----------
    // Real endpoints:
    //   GET    /api/Countries
    //   GET    /api/Countries/{id}
    //   POST   /api/Countries
    //   PUT    /api/Countries/{id}
    //   DELETE /api/Countries/{id}
    // (no generic /Countries/search - filtering/sorting is done client-side
    // over the full list, same as the mock branch already did)
    const Countries = {
        /**
         * Fetches and maps every country. Real branch is a `.then()`-derived promise - resolves directly with the mapped array
         * (not the [data,status,jqXHR] triple), so `$.when()` callers must NOT index the result with [0].
         */
        getAll: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () { return Mock.getCountries().slice(); });
            }
            return realRequest("GET", "/Countries").then(function (list) {
                return (list || []).map(mapCountryFromServer);
            });
        },

        /** Fetches a single country. Real branch is `.then()`-derived (maps through mapCountryFromServer), resolving with a single object. */
        getById: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const country = Mock.getCountries().find(function (c) { return c.id === Number(id); });
                    if (!country) throw new Error("Country not found.");
                    return country;
                });
            }
            return realRequest("GET", "/Countries/" + id).then(mapCountryFromServer);
        },

        /**
         * Creates a country: real branch resolves the region via findRegionRaw, then POSTs.
         * NOTE ON PROMISE SHAPE: although this chains `.then()`, the callback's `return realRequest(...)` returns the raw ajax
         * promise itself (not a plain value), so the outer promise adopts its multi-arg resolution - it behaves like a raw
         * $.ajax() promise for `$.when()` purposes (wrapped as [data,status,jqXHR]), unlike Countries.getAll/getById/update.
         */
        create: function (data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const countries = Mock.getCountries();
                    const newCountry = $.extend({}, data, { id: Mock.nextCountryId() });
                    countries.push(newCountry);
                    Mock.saveCountries(countries);
                    return newCountry;
                });
            }
            // NOTE: the server's Country.Insert() BL method is currently a
            // stub that never calls the DAL insert - this call is wired to
            // the documented real endpoint but won't persist until that's
            // implemented server-side.
            return findRegionRaw(data.region).then(function (region) {
                return realRequest("POST", "/Countries", {
                    cca3: data.apiCountryCode,
                    name: data.commonName,
                    capital: data.capital,
                    region: region || { regionId: 0, regionName: data.region },
                    subRegion: data.subregion,
                    population: data.population,
                    area: data.area,
                    flagUrl: data.flagPng,
                    borders: []
                });
            });
        },

        /**
         * Updates a country: resolves the region via findRegionRaw, PUTs the change, then re-fetches via Countries.getById.
         * Real branch is a `.then()`-derived promise that ultimately resolves with the single mapped country object (not the [data,status,jqXHR] triple).
         */
        update: function (id, data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const countries = Mock.getCountries();
                    const idx = countries.findIndex(function (c) { return c.id === Number(id); });
                    if (idx === -1) throw new Error("Country not found.");
                    countries[idx] = $.extend({}, countries[idx], data, { id: countries[idx].id });
                    Mock.saveCountries(countries);
                    return countries[idx];
                });
            }
            return findRegionRaw(data.region).then(function (region) {
                return realRequest("PUT", "/Countries/" + id, {
                    cca3: data.apiCountryCode,
                    name: data.commonName,
                    capital: data.capital,
                    region: region || { regionId: 0, regionName: data.region },
                    subRegion: data.subregion,
                    population: data.population,
                    area: data.area,
                    flagUrl: data.flagPng,
                    borders: []
                });
            }).then(function () {
                return Countries.getById(id);
            });
        },

        /** Deletes a country by id; real branch is a raw $.ajax() promise (unlike most other Countries methods), so $.when() wraps a single result as [data,status,jqXHR]. */
        delete: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const countries = Mock.getCountries().filter(function (c) { return c.id !== Number(id); });
                    Mock.saveCountries(countries);
                    return { success: true };
                });
            }
            return realRequest("DELETE", "/Countries/" + id);
        },

        // params: { name, region, language, currency, minPopulation, maxPopulation, minArea, maxArea, sortBy, sortDir }
        /**
         * Searches/filters countries entirely client-side (no server-side search endpoint): real branch fetches the full list
         * via Countries.getAll() and then filters/sorts it in JS. `.then()`-derived promise - resolves with a single array.
         */
        search: function (params) {
            params = params || {};
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    return filterAndSortCountries(Mock.getCountries().slice(), params);
                });
            }
            return Countries.getAll().then(function (list) {
                return filterAndSortCountries(list, params);
            });
        }
    };

    /** Applies client-side filtering (name/region/language/currency/min-max population/area) and optional sorting to a country list. */
    function filterAndSortCountries(list, params) {
        if (params.name) {
            const term = params.name.toLowerCase();
            list = list.filter(function (c) { return c.commonName.toLowerCase().indexOf(term) !== -1; });
        }
        if (params.region) {
            list = list.filter(function (c) { return c.region === params.region; });
        }
        if (params.language) {
            const lang = params.language.toLowerCase();
            list = list.filter(function (c) { return c.languages.some(function (l) { return l.toLowerCase() === lang; }); });
        }
        if (params.currency) {
            const cur = params.currency.toLowerCase();
            list = list.filter(function (c) { return c.currencyName.toLowerCase().indexOf(cur) !== -1; });
        }
        if (params.minPopulation) list = list.filter(function (c) { return c.population >= Number(params.minPopulation); });
        if (params.maxPopulation) list = list.filter(function (c) { return c.population <= Number(params.maxPopulation); });
        if (params.minArea) list = list.filter(function (c) { return c.area >= Number(params.minArea); });
        if (params.maxArea) list = list.filter(function (c) { return c.area <= Number(params.maxArea); });

        if (params.sortBy) {
            const dir = params.sortDir === "desc" ? -1 : 1;
            list.sort(function (a, b) {
                const av = a[params.sortBy], bv = b[params.sortBy];
                if (typeof av === "string") return av.localeCompare(bv) * dir;
                return (av - bv) * dir;
            });
        }
        return list;
    }

    // ---------- UserCountries (personal "visited" / "wishlist" lists) ----------
    // Composed from the real endpoints:
    //   GET/POST/DELETE api/UserVisitedCountries/...  (listType "visited")
    //   GET/POST/DELETE api/Users/{id}/wantedCountries (listType "wishlist")
    // Entries don't have a real numeric id on the server (the natural key is
    // userId+countryId), so a synthetic "listType:countryId" id is used - the
    // rest of the app (userLists.js, countryDetails.js) treats it as opaque.
    const UserCountries = {
        /**
         * Combines the visited list (/UserVisitedCountries/user/{id}) and wishlist (/Users/{id}/wantedCountries) into one array
         * with synthetic "visited:{countryId}" / "wishlist:{countryId}" ids. Internally uses $.when() on two raw $.ajax() promises,
         * so indexing visitedResp[0]/wantedResp[0] there is correct - but the function AS A WHOLE returns a `.then()`-derived
         * promise that resolves directly with the entries array (not the [data,status,jqXHR] triple), so callers who wrap
         * THIS function's result in their own `$.when()` must NOT index the result with [0] (see userLists.js loadLists()).
         */
        getByUser: function (userId) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    return Mock.getUserCountries().filter(function (uc) { return uc.userId === Number(userId); });
                });
            }

            const visitedReq = realRequest("GET", "/UserVisitedCountries/user/" + userId);
            const wantedReq = realRequest("GET", "/Users/" + userId + "/wantedCountries");

            return $.when(visitedReq, wantedReq).then(function (visitedResp, wantedResp) {
                const visited = visitedResp[0] || [];
                const wanted = wantedResp[0] || [];
                const entries = [];

                visited.forEach(function (v) {
                    entries.push({
                        id: "visited:" + v.country.countryId,
                        userId: v.userId,
                        countryId: v.country.countryId,
                        listType: "visited"
                    });
                });

                wanted.forEach(function (c) {
                    entries.push({
                        id: "wishlist:" + c.countryId,
                        userId: Number(userId),
                        countryId: c.countryId,
                        listType: "wishlist"
                    });
                });

                return entries;
            });
        },

        // data: { userId, countryId, listType }
        /**
         * Adds a country to a user's visited or wishlist list depending on data.listType (different real endpoint for each), raw $.ajax() promise.
         * A duplicate (userId+countryId+listType) insert is a composite-key conflict server-side (HTTP 409); this function does no special
         * handling for it - the 409 just reaches the caller's .fail() like any other error (mock branch instead throws a JS Error up front).
         */
        create: function (data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const list = Mock.getUserCountries();
                    const exists = list.some(function (uc) {
                        return uc.userId === data.userId && uc.countryId === data.countryId && uc.listType === data.listType;
                    });
                    if (exists) throw new Error("This country is already in that list.");
                    const entry = $.extend({}, data, { id: Mock.nextUserCountryId() });
                    list.push(entry);
                    Mock.saveUserCountries(list);
                    return entry;
                });
            }

            if (data.listType === "wishlist") {
                return realRequest("POST", "/Users/" + data.userId + "/wantedCountries/" + data.countryId);
            }
            return realRequest("POST", "/UserVisitedCountries", {
                userId: data.userId,
                country: { countryId: data.countryId },
                rating: 0,
                reviewText: null,
                isShared: false
            });
        },

        // used to "move" a country between lists by changing listType
        /**
         * Moves an entry between lists by deleting from the source list then creating on the target list (two chained real requests).
         * Promise shape is inconsistent by branch: a same-type no-op resolves with a single {success:true} value, while an actual
         * move's `.then()` callback returns the raw ajax promise of the final POST, so it resolves multi-arg like a raw $.ajax() call.
         */
        update: function (id, data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const list = Mock.getUserCountries();
                    const idx = list.findIndex(function (uc) { return uc.id === Number(id); });
                    if (idx === -1) throw new Error("Entry not found.");
                    list[idx] = $.extend({}, list[idx], data, { id: list[idx].id });
                    Mock.saveUserCountries(list);
                    return list[idx];
                });
            }

            const parts = String(id).split(":");
            const fromType = parts[0];
            const countryId = Number(parts[1]);
            const targetType = data.listType;
            const user = Auth.getCurrentUser();

            if (fromType === targetType) {
                return $.Deferred().resolve({ success: true }).promise();
            }

            if (fromType === "wishlist" && targetType === "visited") {
                return realRequest("DELETE", "/Users/" + user.id + "/wantedCountries/" + countryId).then(function () {
                    return realRequest("POST", "/UserVisitedCountries", {
                        userId: user.id,
                        country: { countryId: countryId },
                        rating: 0,
                        reviewText: null,
                        isShared: false
                    });
                });
            }

            // visited -> wishlist
            return realRequest("DELETE", "/UserVisitedCountries/" + user.id + "/" + countryId).then(function () {
                return realRequest("POST", "/Users/" + user.id + "/wantedCountries/" + countryId);
            });
        },

        /** Removes an entry from whichever list it belongs to (parsed from the synthetic id's "visited:"/"wishlist:" prefix); real branch is a raw $.ajax() promise. */
        delete: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const list = Mock.getUserCountries().filter(function (uc) { return uc.id !== Number(id); });
                    Mock.saveUserCountries(list);
                    return { success: true };
                });
            }

            const parts = String(id).split(":");
            const listType = parts[0];
            const countryId = Number(parts[1]);
            const user = Auth.getCurrentUser();

            if (listType === "wishlist") {
                return realRequest("DELETE", "/Users/" + user.id + "/wantedCountries/" + countryId);
            }
            return realRequest("DELETE", "/UserVisitedCountries/" + user.id + "/" + countryId);
        }
    };

    // ---------- Shares ----------
    // Composed from api/UserVisitedCountries - a "share" is simply a visited
    // country with IsShared = true. There's no separate share id on the
    // server, so a synthetic "userId_countryId" id is used (opaque to shares.js).
    /** Maps a visited-country server record (with isShared=true) into the client's "share" shape, using a synthetic "userId_countryId" id since shares have no id of their own on the server. */
    function mapShareFromVisit(v) {
        return {
            id: v.userId + "_" + v.country.countryId,
            userId: v.userId,
            countryId: v.country.countryId,
            userName: v.userName,
            countryName: v.country.name,
            content: v.reviewText,
            createdAt: v.createdAt ? v.createdAt.substring(0, 10) : ""
        };
    }

    /**
     * Finds a user's existing UserVisitedCountries entry for a given country by fetching the user's full visited list and
     * searching client-side (no direct GET-by-pair endpoint). `.then()`-derived promise - resolves directly with the entry
     * or null (not the [data,status,jqXHR] triple), so `$.when()` callers must NOT index the result with [0].
     */
    function findVisitEntry(userId, countryId) {
        return realRequest("GET", "/UserVisitedCountries/user/" + userId).then(function (visits) {
            return (visits || []).find(function (v) {
                return v.country && v.country.countryId === Number(countryId);
            }) || null;
        });
    }

    const Shares = {
        /** Fetches every shared review across all users. Real branch is `.then()`-derived (maps through mapShareFromVisit), resolving with a single array. */
        getAll: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () { return Mock.getShares().slice().reverse(); });
            }
            return realRequest("GET", "/UserVisitedCountries/shared").then(function (list) {
                return (list || []).map(mapShareFromVisit);
            });
        },

        /** Fetches shared reviews for one country. Real branch is `.then()`-derived (maps through mapShareFromVisit), resolving with a single array. */
        getByCountry: function (countryId) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    return Mock.getShares().filter(function (s) { return s.countryId === Number(countryId); }).reverse();
                });
            }
            return realRequest("GET", "/UserVisitedCountries/shared/country/" + countryId).then(function (list) {
                return (list || []).map(mapShareFromVisit);
            });
        },

        // data: { userId, countryId, userName, countryName, content }
        /**
         * Posts/shares a review: looks up any existing visited entry for the pair first - if found it's PUT (preserving its
         * rating) with isShared=true, otherwise a new UserVisitedCountries row is POSTed. Because the `.then()` callback
         * returns that raw PUT/POST ajax promise directly, the outer promise adopts its multi-arg resolution and behaves
         * like a raw $.ajax() promise for `$.when()` (wrapped as [data,status,jqXHR]), unlike most other `.then()`-derived Api functions.
         */
        create: function (data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const author = Mock.getUsers().find(function (u) { return u.id === data.userId; });
                    if (!author || !author.active) throw new Error("Locked accounts cannot post shares.");
                    if (!author.canShare) throw new Error("You don't have permission to share reviews. Contact an admin to enable sharing.");

                    const shares = Mock.getShares();
                    const newShare = $.extend({}, data, {
                        id: Mock.nextShareId(),
                        createdAt: new Date().toISOString().slice(0, 10)
                    });
                    shares.push(newShare);
                    Mock.saveShares(shares);
                    return newShare;
                });
            }

            return findVisitEntry(data.userId, data.countryId).then(function (existing) {
                const body = {
                    userId: data.userId,
                    country: { countryId: data.countryId },
                    rating: existing ? existing.rating : 0,
                    reviewText: data.content,
                    isShared: true
                };
                return existing
                    ? realRequest("PUT", "/UserVisitedCountries", body)
                    : realRequest("POST", "/UserVisitedCountries", body);
            });
        },

        /**
         * Edits a share's text by re-fetching the existing visited entry (to preserve its rating) and PUTting the new reviewText;
         * rejects if no matching entry is found. Same promise-shape caveat as Shares.create: on success it adopts the raw PUT ajax
         * promise's multi-arg resolution, so it behaves like raw $.ajax() for `$.when()` purposes.
         */
        update: function (id, data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const shares = Mock.getShares();
                    const idx = shares.findIndex(function (s) { return s.id === Number(id); });
                    if (idx === -1) throw new Error("Share not found.");
                    shares[idx] = $.extend({}, shares[idx], data, { id: shares[idx].id });
                    Mock.saveShares(shares);
                    return shares[idx];
                });
            }

            const parts = String(id).split("_");
            const userId = Number(parts[0]);
            const countryId = Number(parts[1]);

            return findVisitEntry(userId, countryId).then(function (existing) {
                if (!existing) {
                    return $.Deferred().reject({ responseJSON: { message: "Share not found." } }).promise();
                }
                return realRequest("PUT", "/UserVisitedCountries", {
                    userId: userId,
                    country: { countryId: countryId },
                    rating: existing.rating,
                    reviewText: data.content,
                    isShared: true
                });
            });
        },

        /**
         * "Deletes" a share by PUTting isShared=false on the underlying visited entry (see note below); rejects if no matching entry is found.
         * Same promise-shape caveat as Shares.create/update: on success it adopts the raw PUT ajax promise's multi-arg resolution.
         */
        delete: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const shares = Mock.getShares().filter(function (s) { return s.id !== Number(id); });
                    Mock.saveShares(shares);
                    return { success: true };
                });
            }

            const parts = String(id).split("_");
            const userId = Number(parts[0]);
            const countryId = Number(parts[1]);

            // "Delete" only un-shares the review - the visited entry (and its
            // rating) is kept in the user's own list.
            return findVisitEntry(userId, countryId).then(function (existing) {
                if (!existing) {
                    return $.Deferred().reject({ responseJSON: { message: "Share not found." } }).promise();
                }
                return realRequest("PUT", "/UserVisitedCountries", {
                    userId: userId,
                    country: { countryId: countryId },
                    rating: existing.rating,
                    reviewText: existing.reviewText,
                    isShared: false
                });
            });
        }
    };

    // ---------- Quizzes ----------
    // Real endpoints:
    //   GET  /api/Quizzes                    (catalog)
    //   GET  /api/Quizzes/{quizId}/questions
    //   POST /api/Quizzes/submit
    const Quizzes = {
        /** Returns the quiz catalog (id/title/description); real branch is a raw $.ajax() promise. */
        getCatalog: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    return Mock.getQuizzes().map(function (q) {
                        return { id: q.id, title: q.title, description: q.description || "" };
                    });
                });
            }
            return realRequest("GET", "/Quizzes");
        },

        /** Returns a quiz's questions; mock branch strips correctIndex to mimic what a real server response should omit. Real branch is a raw $.ajax() promise. */
        getQuestions: function (quizId) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const quiz = Mock.getQuizById(quizId);
                    if (!quiz) throw new Error("Quiz not found.");
                    // correctIndex is stripped out here on purpose, same as a real server would do
                    const safeQuestions = quiz.questions.map(function (q) {
                        return { id: q.id, text: q.text, options: q.options };
                    });
                    return { id: quiz.id, title: quiz.title, durationSeconds: CONFIG.QUIZ_DURATION_SECONDS, questions: safeQuestions };
                });
            }
            return realRequest("GET", "/Quizzes/" + quizId + "/questions");
        },

        // data: { quizId, userId, answers: [{ questionId, selectedIndex }] }
        /** Submits quiz answers; mock branch grades client-side and returns per-question correctness. Real branch is a raw $.ajax() promise POSTing the raw answers for server-side grading. */
        submit: function (data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const quiz = Mock.getQuizById(data.quizId);
                    if (!quiz) throw new Error("Quiz not found.");
                    let correct = 0;
                    const details = quiz.questions.map(function (q) {
                        const answer = data.answers.find(function (a) { return a.questionId === q.id; });
                        const isCorrect = !!answer && answer.selectedIndex === q.correctIndex;
                        if (isCorrect) correct++;
                        return { questionId: q.id, correctIndex: q.correctIndex, isCorrect: isCorrect };
                    });
                    return { quizId: quiz.id, total: quiz.questions.length, correct: correct, details: details };
                });
            }
            return realRequest("POST", "/Quizzes/submit", data);
        }
    };

    // ---------- Admin ----------
    // Real endpoints:
    //   GET /api/Users                              (catalog, reused as the admin user list)
    //   PUT /api/Users/{id}/active?actingUserId=...  (lock/unlock)
    //   GET /api/Users/statistics?actingUserId=...
    /**
     * Locks/unlocks a user: fetches the current row (PUT .../active overwrites every column, same reasoning as Users.update)
     * and PUTs it back with isActive toggled. `.then()`-derived promise - resolves directly with the mapped user (not the
     * [data,status,jqXHR] triple), so `$.when()` callers must NOT index the result with [0].
     */
    function setUserActive(id, active) {
        const admin = Auth.getCurrentUser();

        // PUT .../active also overwrites every column, so the full current
        // row is fetched first (same reasoning as Users.update above).
        return realRequest("GET", "/Users/" + id).then(function (user) {
            return realRequest("PUT", "/Users/" + id + "/active?actingUserId=" + admin.id, {
                userId: user.userId,
                name: user.name,
                email: user.email,
                password: user.password,
                isActive: active,
                isAdmin: user.isAdmin,
                canShare: user.canShare
            });
        }).then(function () {
            return realRequest("GET", "/Users/" + id).then(mapUserFromServer);
        });
    }

    // Grants/revokes a user's permission to publicly share reviews via PUT
    // .../canShare (same "fetch full row first" reasoning as setUserActive).
    function setCanShare(id, canShare) {
        const admin = Auth.getCurrentUser();

        return realRequest("GET", "/Users/" + id).then(function (user) {
            return realRequest("PUT", "/Users/" + id + "/canShare?actingUserId=" + admin.id, {
                userId: user.userId,
                name: user.name,
                email: user.email,
                password: user.password,
                isActive: user.isActive,
                isAdmin: user.isAdmin,
                canShare: canShare
            });
        }).then(function () {
            return realRequest("GET", "/Users/" + id).then(mapUserFromServer);
        });
    }

    const Admin = {
        /** Fetches every user (reused as the admin user list). Real branch is `.then()`-derived (maps through mapUserFromServer), resolving with a single array. */
        getUsers: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () { return Mock.getUsers().map(sanitizeUser); });
            }
            return realRequest("GET", "/Users").then(function (list) {
                return (list || []).map(mapUserFromServer);
            });
        },

        /** Locks a user; real branch delegates directly to setUserActive(id, false) (same `.then()`-derived, single-value promise shape). */
        lockUser: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const idx = users.findIndex(function (u) { return u.id === Number(id); });
                    if (idx === -1) throw new Error("User not found.");
                    users[idx].active = false;
                    Mock.saveUsers(users);
                    return sanitizeUser(users[idx]);
                });
            }
            return setUserActive(id, false);
        },

        /** Unlocks a user; real branch delegates directly to setUserActive(id, true) (same `.then()`-derived, single-value promise shape). */
        unlockUser: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const idx = users.findIndex(function (u) { return u.id === Number(id); });
                    if (idx === -1) throw new Error("User not found.");
                    users[idx].active = true;
                    Mock.saveUsers(users);
                    return sanitizeUser(users[idx]);
                });
            }
            return setUserActive(id, true);
        },

        /** Revokes a user's sharing permission; real branch delegates to setCanShare(id, false) (same `.then()`-derived, single-value promise shape). */
        disableSharing: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const idx = users.findIndex(function (u) { return u.id === Number(id); });
                    if (idx === -1) throw new Error("User not found.");
                    users[idx].canShare = false;
                    Mock.saveUsers(users);
                    return sanitizeUser(users[idx]);
                });
            }
            return setCanShare(id, false);
        },

        /** Grants a user's sharing permission; real branch delegates to setCanShare(id, true) (same `.then()`-derived, single-value promise shape). */
        enableSharing: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const users = Mock.getUsers();
                    const idx = users.findIndex(function (u) { return u.id === Number(id); });
                    if (idx === -1) throw new Error("User not found.");
                    users[idx].canShare = true;
                    Mock.saveUsers(users);
                    return sanitizeUser(users[idx]);
                });
            }
            return setCanShare(id, true);
        },

        /**
         * Fetches admin dashboard stats. Real branch is `.then()`-derived and reshapes the server's aggregate-only response:
         * todayLogins is set equal to totalLogins (server tracks no per-day breakdown) and dailyLogins is always returned
         * empty, so the daily-logins chart has nothing to render against the real API yet.
         */
        getStats: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const log = Mock.getLoginLog();
                    const today = new Date().toISOString().slice(0, 10);

                    const byDate = {};
                    log.forEach(function (entry) {
                        byDate[entry.date] = (byDate[entry.date] || 0) + 1;
                    });
                    const dailyLogins = Object.keys(byDate).sort().map(function (date) {
                        return { date: date, count: byDate[date] };
                    });

                    return {
                        todayLogins: byDate[today] || 0,
                        totalLogins: log.length,
                        countriesImported: Mock.getCountries().length,
                        countriesSaved: Mock.getUserCountries().length,
                        sharesCreated: Mock.getShares().length,
                        dailyLogins: dailyLogins
                    };
                });
            }

            const admin = Auth.getCurrentUser();
            return realRequest("GET", "/Users/statistics?actingUserId=" + admin.id).then(function (stats) {
                // The server only tracks aggregate counts, not a per-day
                // breakdown, so the daily-logins chart stays empty for now.
                return {
                    todayLogins: stats.dailyLogins,
                    totalLogins: stats.dailyLogins,
                    countriesImported: stats.importedCountries,
                    countriesSaved: stats.savedCountries,
                    sharesCreated: stats.sharedReviews,
                    dailyLogins: []
                };
            });
        }
    };

    return {
        Users: Users,
        Countries: Countries,
        UserCountries: UserCountries,
        Shares: Shares,
        Quizzes: Quizzes,
        Admin: Admin
    };
})();
