/*
 * api.js
 * Single place that talks to the backend. Every other JS file calls Api.* and
 * never touches $.ajax or Mock.* directly.
 *
 * Each function has two branches:
 *   - CONFIG.USE_MOCK === true  -> resolves/rejects using mockData.js (fake latency included)
 *   - CONFIG.USE_MOCK === false -> real $.ajax call to CONFIG.API_BASE_URL
 *
 * ===> WHEN THE SERVER IS READY <===
 * Set CONFIG.USE_MOCK = false in config.js. Every function below already targets the
 * real endpoint (see the "Real:" comment above each one) - nothing else needs to change.
 * If the real routes end up with different names, only this file needs to be edited.
 *
 * All functions return a jQuery Promise, so callers use them the same way regardless
 * of mock/real: Api.Countries.getAll().done(function(data){...}).fail(function(err){...});
 */
const Api = (function () {

    // ---------- generic helpers ----------

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

    function sanitizeUser(user) {
        if (!user) return user;
        const copy = $.extend({}, user);
        delete copy.password;
        return copy;
    }

    // ---------- Users ----------
    // Real endpoints:
    //   POST /api/Users/register
    //   POST /api/Users/login
    //   PUT  /api/Users/{id}
    //   GET  /api/Users/{id}
    const Users = {
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
            return realRequest("POST", "/Users/login", credentials);
        },

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
            return realRequest("PUT", "/Users/" + id, data);
        },

        getById: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const user = Mock.getUsers().find(function (u) { return u.id === Number(id); });
                    if (!user) throw new Error("User not found.");
                    return sanitizeUser(user);
                });
            }
            return realRequest("GET", "/Users/" + id);
        }
    };

    // ---------- Countries ----------
    // Real endpoints:
    //   GET    /api/Countries
    //   GET    /api/Countries/{id}
    //   POST   /api/Countries
    //   PUT    /api/Countries/{id}
    //   DELETE /api/Countries/{id}
    //   GET    /api/Countries/search?name=&region=&language=&currency=&minPopulation=&maxPopulation=&minArea=&maxArea=
    const Countries = {
        getAll: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () { return Mock.getCountries().slice(); });
            }
            return realRequest("GET", "/Countries");
        },

        getById: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const country = Mock.getCountries().find(function (c) { return c.id === Number(id); });
                    if (!country) throw new Error("Country not found.");
                    return country;
                });
            }
            return realRequest("GET", "/Countries/" + id);
        },

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
            return realRequest("POST", "/Countries", data);
        },

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
            return realRequest("PUT", "/Countries/" + id, data);
        },

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
        search: function (params) {
            params = params || {};
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    let list = Mock.getCountries().slice();

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
                });
            }
            return realRequest("GET", "/Countries/search?" + $.param(params));
        }
    };

    // ---------- UserCountries (personal "visited" / "wishlist" lists) ----------
    // Real endpoints:
    //   GET    /api/UserCountries/{userId}
    //   POST   /api/UserCountries
    //   PUT    /api/UserCountries/{id}
    //   DELETE /api/UserCountries/{id}
    const UserCountries = {
        getByUser: function (userId) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    return Mock.getUserCountries().filter(function (uc) { return uc.userId === Number(userId); });
                });
            }
            return realRequest("GET", "/UserCountries/" + userId);
        },

        // data: { userId, countryId, listType }
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
            return realRequest("POST", "/UserCountries", data);
        },

        // used to "move" a country between lists by changing listType
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
            return realRequest("PUT", "/UserCountries/" + id, data);
        },

        delete: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const list = Mock.getUserCountries().filter(function (uc) { return uc.id !== Number(id); });
                    Mock.saveUserCountries(list);
                    return { success: true };
                });
            }
            return realRequest("DELETE", "/UserCountries/" + id);
        }
    };

    // ---------- Shares ----------
    // Real endpoints:
    //   GET    /api/Shares
    //   GET    /api/Shares/country/{countryId}
    //   POST   /api/Shares
    //   PUT    /api/Shares/{id}
    //   DELETE /api/Shares/{id}
    const Shares = {
        getAll: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () { return Mock.getShares().slice().reverse(); });
            }
            return realRequest("GET", "/Shares");
        },

        getByCountry: function (countryId) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    return Mock.getShares().filter(function (s) { return s.countryId === Number(countryId); }).reverse();
                });
            }
            return realRequest("GET", "/Shares/country/" + countryId);
        },

        // data: { userId, countryId, userName, countryName, content }
        create: function (data) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const author = Mock.getUsers().find(function (u) { return u.id === data.userId; });
                    if (!author || !author.active) throw new Error("Locked accounts cannot post shares.");

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
            return realRequest("POST", "/Shares", data);
        },

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
            return realRequest("PUT", "/Shares/" + id, data);
        },

        delete: function (id) {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () {
                    const shares = Mock.getShares().filter(function (s) { return s.id !== Number(id); });
                    Mock.saveShares(shares);
                    return { success: true };
                });
            }
            return realRequest("DELETE", "/Shares/" + id);
        }
    };

    // ---------- Quizzes ----------
    // Real endpoints:
    //   GET  /api/Quizzes/{quizId}/questions
    //   POST /api/Quizzes/submit
    const Quizzes = {
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
    //   GET /api/Admin/users
    //   PUT /api/Admin/users/{id}/lock
    //   PUT /api/Admin/users/{id}/unlock
    //   GET /api/Admin/stats
    const Admin = {
        getUsers: function () {
            if (CONFIG.USE_MOCK) {
                return mockResponse(function () { return Mock.getUsers().map(sanitizeUser); });
            }
            return realRequest("GET", "/Admin/users");
        },

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
            return realRequest("PUT", "/Admin/users/" + id + "/lock");
        },

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
            return realRequest("PUT", "/Admin/users/" + id + "/unlock");
        },

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
            return realRequest("GET", "/Admin/stats");
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
