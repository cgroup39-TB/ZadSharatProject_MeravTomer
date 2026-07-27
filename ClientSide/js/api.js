/*
 * api.js
 * Single place that talks to the backend. Every other JS file calls Api.* and
 * never touches $.ajax directly.
 *
 * The real server (see CountriesProject_MeravTomer/Controllers) uses different
 * route names/shapes than the client's original view-model in a few places (no
 * generic search/UserCountries/Shares/Admin resources - those are composed
 * here out of the real endpoints: Countries, Users, UserVisitedCountries,
 * Users/{id}/wantedCountries, Users/{id}/regions, Users/{id}/languages,
 * Users/statistics, Quizzes). This file is the only place that knows about
 * that mapping - every other page keeps using the client's own flat shapes
 * (country.commonName, share.userName, userCountry.listType, ...).
 *
 * All functions return a jQuery Promise: Api.Countries.getAll().done(function(data){...}).fail(function(err){...});
 */
const Api = (function () {

    // ---------- generic helpers ----------

    /**
     * Performs the real $.ajax call to CONFIG.API_BASE_URL, JSON-encoding the body and attaching the auth header.
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

    // Same idea as findRegionRaw, but for languages: there's no dedicated
    // /Languages endpoint either, so a language's real {languageId,
    // languageName} is looked up from whichever country happens to speak it.
    const LANGUAGE_LEVEL_TO_NUMBER = { Beginner: 1, Intermediate: 2, Advanced: 3, Native: 4 };

    function findLanguagesRaw(languageNames) {
        if (!languageNames.length) {
            return $.Deferred().resolve([]).promise();
        }
        return realRequest("GET", "/Countries").then(function (countries) {
            const byName = {};
            (countries || []).forEach(function (c) {
                (c.languages || []).forEach(function (l) { byName[l.languageName] = l; });
            });
            // Names that don't match any language currently spoken by a
            // seeded country are silently dropped - there's no endpoint to
            // create a brand-new Language row from the client.
            return languageNames.map(function (name) { return byName[name]; }).filter(Boolean);
        });
    }

    // ---------- Users ----------
    // Real endpoints:
    //   POST /api/Users/register
    //   POST /api/Users/login
    //   PUT  /api/Users/{id}/profile?actingUserId=...
    //   GET  /api/Users/{id}
    const Users = {
        /** Registers a new user (raw $.ajax() promise); the server validates (e.g. duplicate email). */
        register: function (userData) {
            return realRequest("POST", "/Users/register", userData);
        },

        /**
         * Logs in a user. The real server issues no JWT, so a lightweight local session token ("session-{id}-{timestamp}") is synthesized client-side from the returned user row.
         * `.then()`-derived promise - resolves directly with {token, user}, not the [data,status,jqXHR] triple, so `$.when()` callers must NOT index the result with [0].
         */
        login: function (credentials) {
            return realRequest("POST", "/Users/login", credentials).then(function (user) {
                return {
                    token: "session-" + user.userId + "-" + Date.now(),
                    user: mapUserFromServer(user)
                };
            });
        },

        /**
         * Updates a user's profile. `.then()`-derived promise that ultimately resolves with a single mapped user object
         * (not the [data,status,jqXHR] triple), so `$.when()` callers must NOT index the result with [0].
         */
        update: function (id, data) {
            // "preferences" updates (continents/countries/languages) aren't
            // backed by a matching field on the real User - continents and
            // languages are persisted via the dedicated Users/{id}/regions
            // and Users/{id}/languages resources (see savePreferences below).
            // "countries" (the favorite-countries picklist) has no server
            // resource at all, so it's only ever kept in the locally cached
            // user object, same as before.
            if (data && data.preferences && data.name === undefined && data.email === undefined) {
                return savePreferences(id, data.preferences);
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

        /** Fetches a user by id. `.then()`-derived (maps through mapUserFromServer), resolving with a single object - not the [data,status,jqXHR] triple. */
        getById: function (id) {
            return realRequest("GET", "/Users/" + id).then(mapUserFromServer);
        },

        /** Returns the user's preferred-region rows ([{regionId, regionName}]); raw $.ajax() promise. */
        getPreferredRegions: function (id) {
            return realRequest("GET", "/Users/" + id + "/regions");
        },

        /** Returns the user's recorded languages ([{userId, language:{languageId,languageName}, proficiencyLevel}]); raw $.ajax() promise. */
        getUserLanguages: function (id) {
            return realRequest("GET", "/Users/" + id + "/languages");
        }
    };

    /**
     * Persists the "continents" and "languages" parts of a preferences save to their real
     * server resources (Users/{id}/regions, Users/{id}/languages) - "countries" has no server
     * resource, so it only ever lives in the locally cached user object. Resolves with a merged
     * user object (existing cached user + the given preferences) so Auth.updateProfile's
     * setSession() call actually keeps the local cache in sync, instead of the previous no-op
     * stub that silently dropped every preferences save.
     */
    function savePreferences(userId, preferences) {
        const continents = preferences.continents || [];
        const languages = preferences.languages || [];

        const regionsSaved = saveContinentsAsRegions(userId, continents);
        const languagesSaved = saveLanguagesByName(userId, languages);

        return $.when(regionsSaved, languagesSaved).then(function () {
            const current = Auth.getCurrentUser();
            return $.extend({}, current, { preferences: preferences });
        });
    }

    /** Resolves each continent name to its real regionId (via findRegionRaw) before PUTting the id list. */
    function saveContinentsAsRegions(userId, continentNames) {
        if (!continentNames.length) {
            return realRequest("PUT", "/Users/" + userId + "/regions", []);
        }
        return realRequest("GET", "/Countries").then(function (countries) {
            const idByName = {};
            (countries || []).forEach(function (c) {
                if (c.region) idByName[c.region.regionName] = c.region.regionId;
            });
            const regionIds = continentNames.map(function (name) { return idByName[name]; }).filter(Boolean);
            return realRequest("PUT", "/Users/" + userId + "/regions", regionIds);
        });
    }

    /** Resolves each {name, level} to a real language object (via findLanguagesRaw) before PUTting the UserLanguages list. */
    function saveLanguagesByName(userId, languages) {
        const names = languages.map(function (l) { return l.name; });
        return findLanguagesRaw(names).then(function (resolvedLanguages) {
            const byName = {};
            resolvedLanguages.forEach(function (lang) { byName[lang.languageName] = lang; });

            const payload = languages
                .map(function (l) {
                    const lang = byName[l.name];
                    if (!lang) return null;
                    return {
                        userId: userId,
                        language: lang,
                        // Must match the server's UserLanguages.ProficiencyLevel property name
                        // (camelCased in the actual JSON) - a mismatched key silently fails model
                        // binding, leaving ProficiencyLevel null and violating the DB's NOT NULL constraint.
                        proficiencyLevel: LANGUAGE_LEVEL_TO_NUMBER[l.level] || null
                    };
                })
                .filter(Boolean);

            return realRequest("PUT", "/Users/" + userId + "/languages", payload);
        });
    }

    // ---------- Countries ----------
    // Real endpoints:
    //   GET    /api/Countries
    //   GET    /api/Countries/{id}
    //   POST   /api/Countries
    //   PUT    /api/Countries/{id}
    //   DELETE /api/Countries/{id}
    // (no generic /Countries/search - filtering/sorting is done client-side over the full list)
    const Countries = {
        /**
         * Fetches and maps every country. `.then()`-derived promise - resolves directly with the mapped array
         * (not the [data,status,jqXHR] triple), so `$.when()` callers must NOT index the result with [0].
         */
        getAll: function () {
            return realRequest("GET", "/Countries").then(function (list) {
                return (list || []).map(mapCountryFromServer);
            });
        },

        /** Fetches a single country. `.then()`-derived (maps through mapCountryFromServer), resolving with a single object. */
        getById: function (id) {
            return realRequest("GET", "/Countries/" + id).then(mapCountryFromServer);
        },

        /**
         * Creates a country: resolves the region via findRegionRaw, then POSTs.
         * NOTE ON PROMISE SHAPE: although this chains `.then()`, the callback's `return realRequest(...)` returns the raw ajax
         * promise itself (not a plain value), so the outer promise adopts its multi-arg resolution - it behaves like a raw
         * $.ajax() promise for `$.when()` purposes (wrapped as [data,status,jqXHR]), unlike Countries.getAll/getById/update.
         */
        create: function (data) {
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
         * `.then()`-derived promise that ultimately resolves with the single mapped country object (not the [data,status,jqXHR] triple).
         */
        update: function (id, data) {
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

        /** Deletes a country by id; raw $.ajax() promise (unlike most other Countries methods), so $.when() wraps a single result as [data,status,jqXHR]. */
        delete: function (id) {
            return realRequest("DELETE", "/Countries/" + id);
        },

        // params: { name, region, language, currency, sortBy, sortDir }
        /**
         * Searches/filters countries entirely client-side (no server-side search endpoint): fetches the full list
         * via Countries.getAll() and then filters/sorts it in JS. `.then()`-derived promise - resolves with a single array.
         */
        search: function (params) {
            params = params || {};
            return Countries.getAll().then(function (list) {
                return filterAndSortCountries(list, params);
            });
        }
    };

    /** Applies client-side filtering (name/region/language/currency) and optional sorting to a country list. */
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
                        listType: "visited",
                        rating: v.rating,
                        reviewText: v.reviewText,
                        isShared: v.isShared
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
         * Adds a country to a user's visited or wishlist list depending on data.listType (different real endpoint for each).
         * A country can only ever be on one list at a time, so this first checks whether it's currently on the *other*
         * list and, if so, removes it from there before adding - otherwise a country could end up marked both visited
         * and wanted at once. A duplicate insert on the SAME list is still a composite-key conflict server-side (HTTP 409)
         * and just reaches the caller's .fail() like any other error.
         */
        create: function (data) {
            const countryId = Number(data.countryId);
            const oppositeType = data.listType === "wishlist" ? "visited" : "wishlist";

            function addRequest() {
                if (data.listType === "wishlist") {
                    return realRequest("POST", "/Users/" + data.userId + "/wantedCountries/" + countryId);
                }
                return realRequest("POST", "/UserVisitedCountries", {
                    userId: data.userId,
                    country: { countryId: countryId },
                    rating: 0,
                    reviewText: null,
                    isShared: false
                });
            }

            return UserCountries.getByUser(data.userId).then(function (entries) {
                const onOppositeList = (entries || []).some(function (e) {
                    return e.countryId === countryId && e.listType === oppositeType;
                });

                if (!onOppositeList) {
                    return addRequest();
                }

                const removeFromOpposite = oppositeType === "wishlist"
                    ? realRequest("DELETE", "/Users/" + data.userId + "/wantedCountries/" + countryId)
                    : realRequest("DELETE", "/UserVisitedCountries/" + data.userId + "/" + countryId);

                return removeFromOpposite.then(addRequest);
            });
        },

        // used to "move" a country between lists by changing listType
        /**
         * Moves an entry between lists by deleting from the source list then creating on the target list (two chained real requests).
         * Promise shape is inconsistent by branch: a same-type no-op resolves with a single {success:true} value, while an actual
         * move's `.then()` callback returns the raw ajax promise of the final POST, so it resolves multi-arg like a raw $.ajax() call.
         */
        update: function (id, data) {
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

        /** Removes an entry from whichever list it belongs to (parsed from the synthetic id's "visited:"/"wishlist:" prefix); raw $.ajax() promise. */
        delete: function (id) {
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
        /** Fetches every shared review across all users. `.then()`-derived (maps through mapShareFromVisit), resolving with a single array. */
        getAll: function () {
            return realRequest("GET", "/UserVisitedCountries/shared").then(function (list) {
                return (list || []).map(mapShareFromVisit);
            });
        },

        /** Fetches shared reviews for one country. `.then()`-derived (maps through mapShareFromVisit), resolving with a single array. */
        getByCountry: function (countryId) {
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
        /** Returns the quiz catalog (id/title/description); raw $.ajax() promise. */
        getCatalog: function () {
            return realRequest("GET", "/Quizzes");
        },

        /** Returns a quiz's questions; raw $.ajax() promise. */
        getQuestions: function (quizId) {
            return realRequest("GET", "/Quizzes/" + quizId + "/questions");
        },

        // data: { quizId, userId, answers: [{ questionId, selectedIndex }] }
        /** Submits quiz answers; raw $.ajax() promise POSTing the raw answers for server-side grading. */
        submit: function (data) {
            return realRequest("POST", "/Quizzes/submit", data);
        }
    };

    // ---------- Admin ----------
    // Real endpoints:
    //   GET    /api/Users                              (catalog, reused as the admin user list)
    //   PUT    /api/Users/{id}/active?actingUserId=...  (lock/unlock)
    //   PUT    /api/Users/{id}/canShare?actingUserId=... (allow/revoke sharing)
    //   DELETE /api/Users/{id}?actingUserId=...          (permanently delete a user)
    //   GET    /api/Users/statistics?actingUserId=...
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
        /** Fetches every user (reused as the admin user list). `.then()`-derived (maps through mapUserFromServer), resolving with a single array. */
        getUsers: function () {
            return realRequest("GET", "/Users").then(function (list) {
                return (list || []).map(mapUserFromServer);
            });
        },

        /** Locks a user; delegates to setUserActive(id, false) (`.then()`-derived, single-value promise shape). */
        lockUser: function (id) {
            return setUserActive(id, false);
        },

        /** Unlocks a user; delegates to setUserActive(id, true) (`.then()`-derived, single-value promise shape). */
        unlockUser: function (id) {
            return setUserActive(id, true);
        },

        /** Revokes a user's sharing permission; delegates to setCanShare(id, false) (`.then()`-derived, single-value promise shape). */
        disableSharing: function (id) {
            return setCanShare(id, false);
        },

        /** Grants a user's sharing permission; delegates to setCanShare(id, true) (`.then()`-derived, single-value promise shape). */
        enableSharing: function (id) {
            return setCanShare(id, true);
        },

        /** Permanently deletes a user; raw $.ajax() promise (unlike most other Admin methods), so $.when() wraps a single result as [data,status,jqXHR]. */
        deleteUser: function (id) {
            const admin = Auth.getCurrentUser();
            return realRequest("DELETE", "/Users/" + id + "?actingUserId=" + admin.id);
        },

        /**
         * Fetches admin dashboard stats. `.then()`-derived and reshapes the server's aggregate-only response:
         * todayLogins is set equal to totalLogins (server tracks no per-day breakdown) and dailyLogins is always returned
         * empty, so the daily-logins chart has nothing to render against the real API yet.
         */
        getStats: function () {
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

    // ---------- Recommendations ----------
    // Not a real server resource - composed client-side from the user's
    // preferred regions/languages (Users/{id}/regions, .../languages) plus
    // the full country catalog and their existing visited/wishlist entries.
    const Recommendations = {
        /**
         * Suggests countries the user hasn't already listed (visited or
         * wishlisted) whose region or spoken languages match the user's
         * saved preferences, best matches first. Resolves with
         * [{ country, score, matchedRegion, matchedLanguages }] - score is
         * just 1 point per matched region + 1 point per matched language.
         */
        getForUser: function (userId) {
            return $.when(
                Users.getPreferredRegions(userId),
                Users.getUserLanguages(userId),
                Countries.getAll(),
                UserCountries.getByUser(userId)
            ).then(function (regionsResp, languagesResp, countries, entries) {
                // getPreferredRegions/getUserLanguages are raw $.ajax() promises, so $.when
                // wraps each as [data,status,jqXHR] - but Countries.getAll()/UserCountries.getByUser()
                // are already `.then()`-derived single-value promises, so those two are NOT indexed.
                const preferredRegionNames = (regionsResp[0] || []).map(function (r) { return r.regionName; });
                const preferredLanguageNames = (languagesResp[0] || []).map(function (ul) { return ul.language.languageName; });

                const alreadyListedIds = {};
                (entries || []).forEach(function (e) { alreadyListedIds[e.countryId] = true; });

                return countries
                    .filter(function (c) { return !alreadyListedIds[c.id]; })
                    .map(function (c) {
                        const matchedLanguages = (c.languages || []).filter(function (l) {
                            return preferredLanguageNames.indexOf(l) !== -1;
                        });
                        const matchedRegion = preferredRegionNames.indexOf(c.region) !== -1;

                        return {
                            country: c,
                            score: (matchedRegion ? 1 : 0) + matchedLanguages.length,
                            matchedRegion: matchedRegion,
                            matchedLanguages: matchedLanguages
                        };
                    })
                    .filter(function (r) { return r.score > 0; })
                    .sort(function (a, b) {
                        return b.score - a.score || a.country.commonName.localeCompare(b.country.commonName);
                    });
            });
        }
    };

    return {
        Users: Users,
        Countries: Countries,
        UserCountries: UserCountries,
        Shares: Shares,
        Quizzes: Quizzes,
        Admin: Admin,
        Recommendations: Recommendations
    };
})();
