/*
 * mockData.js
 * In-memory / localStorage-backed replacement for the real database.
 *
 * ===> WHEN THE SERVER IS READY <===
 * This whole file stops being used once CONFIG.USE_MOCK is set to false in config.js.
 * api.js is the only file that talks to Mock.* - nothing else in the app should import this directly.
 *
 * Data shapes match the JSON contracts agreed with the server side (BL/Country.cs, BL/User.cs, ...):
 *   Country:      { id, apiCountryCode, commonName, officialName, capital, region, subregion,
 *                    population, area, flagPng, flagSvg, currencyName, currencySymbol, languages[] }
 *   User:         { id, name, email, password, active, isAdmin }
 *   Share:        { id, userId, countryId, userName, countryName, content, createdAt }
 *   UserCountry:  { id, userId, countryId, listType: "visited" | "wishlist" }
 */
const Mock = (function () {

    // ---------- seed data (used only the very first time the app runs) ----------

    const seedCountries = [
        { id: 1, apiCountryCode: "ISR", commonName: "Israel", officialName: "State of Israel", capital: "Jerusalem", region: "Asia", subregion: "Western Asia", population: 9850000, area: 22072, flagPng: "https://flagcdn.com/w320/il.png", flagSvg: "https://flagcdn.com/il.svg", currencyName: "Israeli new shekel", currencySymbol: "₪", languages: ["Hebrew", "Arabic"] },
        { id: 2, apiCountryCode: "USA", commonName: "United States", officialName: "United States of America", capital: "Washington, D.C.", region: "Americas", subregion: "North America", population: 331000000, area: 9833517, flagPng: "https://flagcdn.com/w320/us.png", flagSvg: "https://flagcdn.com/us.svg", currencyName: "United States dollar", currencySymbol: "$", languages: ["English"] },
        { id: 3, apiCountryCode: "FRA", commonName: "France", officialName: "French Republic", capital: "Paris", region: "Europe", subregion: "Western Europe", population: 67390000, area: 551695, flagPng: "https://flagcdn.com/w320/fr.png", flagSvg: "https://flagcdn.com/fr.svg", currencyName: "Euro", currencySymbol: "€", languages: ["French"] },
        { id: 4, apiCountryCode: "JPN", commonName: "Japan", officialName: "Japan", capital: "Tokyo", region: "Asia", subregion: "Eastern Asia", population: 125800000, area: 377975, flagPng: "https://flagcdn.com/w320/jp.png", flagSvg: "https://flagcdn.com/jp.svg", currencyName: "Japanese yen", currencySymbol: "¥", languages: ["Japanese"] },
        { id: 5, apiCountryCode: "EGY", commonName: "Egypt", officialName: "Arab Republic of Egypt", capital: "Cairo", region: "Africa", subregion: "Northern Africa", population: 102300000, area: 1002450, flagPng: "https://flagcdn.com/w320/eg.png", flagSvg: "https://flagcdn.com/eg.svg", currencyName: "Egyptian pound", currencySymbol: "£", languages: ["Arabic"] },
        { id: 6, apiCountryCode: "BRA", commonName: "Brazil", officialName: "Federative Republic of Brazil", capital: "Brasília", region: "Americas", subregion: "South America", population: 212600000, area: 8515767, flagPng: "https://flagcdn.com/w320/br.png", flagSvg: "https://flagcdn.com/br.svg", currencyName: "Brazilian real", currencySymbol: "R$", languages: ["Portuguese"] },
        { id: 7, apiCountryCode: "DEU", commonName: "Germany", officialName: "Federal Republic of Germany", capital: "Berlin", region: "Europe", subregion: "Western Europe", population: 83240000, area: 357114, flagPng: "https://flagcdn.com/w320/de.png", flagSvg: "https://flagcdn.com/de.svg", currencyName: "Euro", currencySymbol: "€", languages: ["German"] },
        { id: 8, apiCountryCode: "AUS", commonName: "Australia", officialName: "Commonwealth of Australia", capital: "Canberra", region: "Oceania", subregion: "Australia and New Zealand", population: 25690000, area: 7692024, flagPng: "https://flagcdn.com/w320/au.png", flagSvg: "https://flagcdn.com/au.svg", currencyName: "Australian dollar", currencySymbol: "$", languages: ["English"] },
        { id: 9, apiCountryCode: "IND", commonName: "India", officialName: "Republic of India", capital: "New Delhi", region: "Asia", subregion: "Southern Asia", population: 1380000000, area: 3287263, flagPng: "https://flagcdn.com/w320/in.png", flagSvg: "https://flagcdn.com/in.svg", currencyName: "Indian rupee", currencySymbol: "₹", languages: ["Hindi", "English"] },
        { id: 10, apiCountryCode: "ITA", commonName: "Italy", officialName: "Italian Republic", capital: "Rome", region: "Europe", subregion: "Southern Europe", population: 60360000, area: 301336, flagPng: "https://flagcdn.com/w320/it.png", flagSvg: "https://flagcdn.com/it.svg", currencyName: "Euro", currencySymbol: "€", languages: ["Italian"] },
        { id: 11, apiCountryCode: "ZAF", commonName: "South Africa", officialName: "Republic of South Africa", capital: "Pretoria", region: "Africa", subregion: "Southern Africa", population: 59310000, area: 1221037, flagPng: "https://flagcdn.com/w320/za.png", flagSvg: "https://flagcdn.com/za.svg", currencyName: "South African rand", currencySymbol: "R", languages: ["Zulu", "English", "Afrikaans"] },
        { id: 12, apiCountryCode: "MEX", commonName: "Mexico", officialName: "United Mexican States", capital: "Mexico City", region: "Americas", subregion: "Central America", population: 128900000, area: 1964375, flagPng: "https://flagcdn.com/w320/mx.png", flagSvg: "https://flagcdn.com/mx.svg", currencyName: "Mexican peso", currencySymbol: "$", languages: ["Spanish"] }
    ];

    const seedUsers = [
        { id: 1, name: "Admin", email: "admin@test.com", password: "admin123", active: true, isAdmin: true },
        { id: 2, name: "May", email: "test@test.com", password: "123456", active: true, isAdmin: false },
        { id: 3, name: "Locked User", email: "locked@test.com", password: "123456", active: false, isAdmin: false }
    ];

    const seedUserCountries = [
        { id: 1, userId: 2, countryId: 1, listType: "visited" },
        { id: 2, userId: 2, countryId: 3, listType: "wishlist" },
        { id: 3, userId: 2, countryId: 4, listType: "wishlist" }
    ];

    const seedShares = [
        { id: 1, userId: 2, countryId: 1, userName: "May", countryName: "Israel", content: "Beautiful country to visit, amazing food!", createdAt: "2026-06-01" },
        { id: 2, userId: 2, countryId: 3, userName: "May", countryName: "France", content: "The Eiffel Tower at night is unforgettable.", createdAt: "2026-06-10" }
    ];

    // Two quizzes with mock questions - replaced later by GET /api/Quizzes/{quizId}/questions
    const quizzes = [
        {
            id: 1,
            title: "Flags & Capitals Quiz",
            questions: [
                { id: 1, text: "What is the capital of Japan?", options: ["Osaka", "Tokyo", "Kyoto", "Nagoya"], correctIndex: 1 },
                { id: 2, text: "What is the capital of Australia?", options: ["Sydney", "Melbourne", "Canberra", "Perth"], correctIndex: 2 },
                { id: 3, text: "Which country's flag features a red maple leaf?", options: ["USA", "Canada", "Austria", "Peru"], correctIndex: 1 },
                { id: 4, text: "What is the capital of Egypt?", options: ["Cairo", "Alexandria", "Giza", "Luxor"], correctIndex: 0 },
                { id: 5, text: "What is the capital of Brazil?", options: ["Rio de Janeiro", "São Paulo", "Brasília", "Salvador"], correctIndex: 2 }
            ]
        },
        {
            id: 2,
            title: "World Geography Quiz",
            questions: [
                { id: 1, text: "Which is the largest country in the world by area?", options: ["China", "USA", "Russia", "Canada"], correctIndex: 2 },
                { id: 2, text: "Which continent is Egypt located in?", options: ["Asia", "Africa", "Europe", "South America"], correctIndex: 1 },
                { id: 3, text: "Which currency is used in Germany?", options: ["Franc", "Euro", "Mark", "Pound"], correctIndex: 1 },
                { id: 4, text: "Which country has the largest population?", options: ["India", "USA", "Indonesia", "China"], correctIndex: 3 },
                { id: 5, text: "What language is mainly spoken in Brazil?", options: ["Spanish", "Portuguese", "French", "Italian"], correctIndex: 1 }
            ]
        }
    ];

    // ---------- low level localStorage helpers ----------

    function readStore(key, fallback) {
        const raw = localStorage.getItem(key);
        if (!raw) return fallback;
        try {
            return JSON.parse(raw);
        } catch (e) {
            return fallback;
        }
    }

    function writeStore(key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    }

    function nextId(list) {
        return list.length ? Math.max.apply(null, list.map(function (item) { return item.id; })) + 1 : 1;
    }

    // Seed localStorage once so CRUD changes survive page reloads during the demo
    function ensureSeeded() {
        if (localStorage.getItem(CONFIG.STORAGE_KEYS.SEEDED)) return;
        writeStore(CONFIG.STORAGE_KEYS.COUNTRIES, seedCountries);
        writeStore(CONFIG.STORAGE_KEYS.USERS, seedUsers);
        writeStore(CONFIG.STORAGE_KEYS.USER_COUNTRIES, seedUserCountries);
        writeStore(CONFIG.STORAGE_KEYS.SHARES, seedShares);
        writeStore(CONFIG.STORAGE_KEYS.LOGIN_LOG, []);
        localStorage.setItem(CONFIG.STORAGE_KEYS.SEEDED, "true");
    }

    ensureSeeded();

    // ---------- public data accessors (kept dumb on purpose - filtering/paging lives in api.js) ----------

    return {
        // Countries
        getCountries: function () { return readStore(CONFIG.STORAGE_KEYS.COUNTRIES, []); },
        saveCountries: function (list) { writeStore(CONFIG.STORAGE_KEYS.COUNTRIES, list); },
        nextCountryId: function () { return nextId(Mock.getCountries()); },

        // Users
        getUsers: function () { return readStore(CONFIG.STORAGE_KEYS.USERS, []); },
        saveUsers: function (list) { writeStore(CONFIG.STORAGE_KEYS.USERS, list); },
        nextUserId: function () { return nextId(Mock.getUsers()); },

        // Personal country lists (visited / wishlist)
        getUserCountries: function () { return readStore(CONFIG.STORAGE_KEYS.USER_COUNTRIES, []); },
        saveUserCountries: function (list) { writeStore(CONFIG.STORAGE_KEYS.USER_COUNTRIES, list); },
        nextUserCountryId: function () { return nextId(Mock.getUserCountries()); },

        // Shares
        getShares: function () { return readStore(CONFIG.STORAGE_KEYS.SHARES, []); },
        saveShares: function (list) { writeStore(CONFIG.STORAGE_KEYS.SHARES, list); },
        nextShareId: function () { return nextId(Mock.getShares()); },

        // Login log, used only to build the admin "daily logins" stat
        getLoginLog: function () { return readStore(CONFIG.STORAGE_KEYS.LOGIN_LOG, []); },
        saveLoginLog: function (list) { writeStore(CONFIG.STORAGE_KEYS.LOGIN_LOG, list); },

        // Quizzes are read-only mock content, no localStorage needed
        getQuizzes: function () { return quizzes; },
        getQuizById: function (quizId) {
            return quizzes.find(function (q) { return q.id === Number(quizId); }) || null;
        }
    };
})();
