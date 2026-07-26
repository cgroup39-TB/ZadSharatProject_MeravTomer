# Project Overview

Quick-reference map of the codebase for presenting/discussing this project. No marketing
language, just where things are and how they connect.

## 1. Architecture

- **Server**: `CountriesProject_MeravTomer/` — ASP.NET Core 6 Web API (`net6.0`), REST endpoints
  under `/api/...`, Swagger enabled in Development.
- **Client**: `ClientSide/` — plain static HTML/CSS/JS. jQuery-based, no build step, no
  framework (no React/Angular/npm). Just open the `.html` files (e.g. via a local web server /
  VS Code Live Server) and it talks to the API over HTTP(S).
- **Transport**: the client calls the server's REST API with jQuery `$.ajax`. CORS is enabled
  server-side (`Program.cs`, `AllowAnyOrigin/AllowAnyMethod/AllowAnyHeader`) specifically so the
  static client — served from a different origin/port than the API — is allowed to call it.
- **Mock mode**: `ClientSide/js/config.js` has a `CONFIG.USE_MOCK` flag. When `true`, the client
  never calls the real API — `ClientSide/js/mockData.js` fakes all data in `localStorage`
  instead, with an artificial delay (`MOCK_DELAY_MS`) to simulate network latency. This lets the
  client be demoed/developed with the server turned off. `CONFIG.API_BASE_URL` points at the
  real API's base URL when mock mode is off.

## 2. Server layers

```
Controllers (HTTP in/out, routing, status codes)
      |
      v
BL - Business Logic (validation, authorization, orchestration)
      |
      v
DAL - Data Access (ADO.NET + stored procedures)
      |
      v
SQL Server
```

- **Controllers** (`Controllers/*.cs`): translate HTTP requests into calls on BL objects, and BL
  results/exceptions into HTTP status codes (200/400/401/404/409). They contain no business
  rules of their own.
- **BL** (`BL/*.cs`): the actual domain classes (`Country`, `User`, `Quiz`, `UserVisitedCountry`,
  etc.). Validation and authorization rules live here — e.g. "only admins can change user
  status", "a rating must be 0 or 1-5". BL methods construct and call into the matching DAL class.
- **DAL** (`DAL/*.cs`): one `DB*Services` class per domain area (`DBCountryServices`,
  `DBUserServices`, `DBUserVisitCountryServices`, `DBCurrencyServices`, `DBLanguageServices`,
  `DBRegionServices`, `DBQuizServices`). Each opens its own ADO.NET `SqlConnection` (via a
  `connect(...)` helper reading the `myProjDB` connection string from `appsettings.json`) and
  calls stored procedures exclusively — there is no inline SQL and no Entity Framework anywhere
  in this project.
- **Database schema and stored procedures are plain `.sql` scripts, not EF migrations**:
  - `DAL/CreateTables.sql` — all `CREATE TABLE` statements (Regions, Countries, Currencies,
    Languages, Users, plus join/preference tables for user visits, wanted countries, user
    languages/regions, quizzes, etc.).
  - `DAL/SQL_CountriesSP.sql`, `SQL_CurrencySP.sql`, `SQL_LanguagesSP.sql`, `SQL_RegionSP.sql`,
    `SQL_UserSP.sql`, `SQL_UserVisitedSP.sql`, `SQL_QuizzesSP.sql` — one file per domain area,
    each containing that area's `CREATE PROCEDURE` statements.
  - **These must be run manually against the target SQL Server database** (in order:
    `CreateTables.sql` first, then the `SQL_*SP.sql` files) before the API will work. There is no
    automatic migration step — nothing runs these on startup.
  - All stored procedure names follow a `..._3MD_TB` suffix naming convention (e.g.
    `spReadAllCountries_3MD_TB`). The C# code and the SQL scripts must agree on this exact suffix
    (see Known Gotchas below).

## 3. Feature-to-files map

| Feature | Controller | BL | DAL | Client JS / pages |
|---|---|---|---|---|
| Auth (login/register) | `Controllers/UsersController.cs` (`Register`, `Login`) | `BL/User.cs` (`Register`, `Login`), `BL/LoginRequest.cs` | `DAL/DBUserServices.cs` | `ClientSide/js/auth.js`, `ClientSide/js/api.js` (`Api.Users`); pages `pages/login.html`, `pages/register.html` |
| Countries browse/search/CRUD | `Controllers/CountriesController.cs` | `BL/Country.cs`, `BL/Region.cs`, `BL/Language.cs`, `BL/Currency.cs` | `DAL/DBCountryServices.cs`, `DBRegionServices.cs`, `DBLanguageServices.cs`, `DBCurrencyServices.cs` | `ClientSide/js/countries.js`, `countryDetails.js`, `api.js` (`Api.Countries`); pages `pages/countries-list.html`, `country-details.html`, `country-form.html` |
| My Lists (visited / wishlist) | `Controllers/UserVisitedCountriesController.cs` (visited), `Controllers/UsersController.cs` (`*WantedCountry*` endpoints, wishlist) | `BL/UserVisitedCountry.cs` (visited), `BL/User.cs` (`*WantedCountry` methods, wishlist) | `DAL/DBUserVisitCountryServices.cs` (visited), `DAL/DBUserServices.cs` (wishlist) | `ClientSide/js/userLists.js`, `api.js` (`Api.UserCountries`); page `pages/my-lists.html` |
| Shares / Reviews | `Controllers/UserVisitedCountriesController.cs` (`shared` endpoints) | `BL/UserVisitedCountry.cs` (`ReadSharedReviews*`, `ReadAllSharedReviews`) | `DAL/DBUserVisitCountryServices.cs` | `ClientSide/js/shares.js`, `countryDetails.js` (per-country reviews), `api.js` (`Api.Shares`); page `pages/shares.html` |
| Quizzes | `Controllers/QuizzesController.cs` | `BL/Quiz.cs`, `QuizQuestion.cs`, `QuizAnswer.cs`, `QuizSubmission.cs`, `QuizResult.cs` | `DAL/DBQuizServices.cs` | `ClientSide/js/quizzes.js`, `api.js` (`Api.Quizzes`); pages `pages/quiz-list.html`, `quiz-play.html` |
| Admin (users / stats) | `Controllers/UsersController.cs` (`SetUserActive`, `SetCanShare`, `SetAdmin`, `GetStatistics`) | `BL/User.cs` (permission-gated setters, `ReadStatistics`), `BL/AdminStatistics.cs` | `DAL/DBUserServices.cs` | `ClientSide/js/admin.js`, `api.js` (`Api.Admin`); pages `pages/admin-users.html`, `admin-stats.html` |
| User preferences (languages, preferred regions) | `Controllers/UsersController.cs` (`*languages`, `*regions` endpoints) | `BL/User.cs` (`ReadUserLanguages`/`UpdateUserLanguages`, `ReadPreferredRegions`/`UpdatePreferredRegions`), `BL/UserLanguages.cs` | `DAL/DBUserServices.cs` | `ClientSide/js/auth.js` / page `pages/preferences.html`, `profile.html` |

Cross-cutting client files: `api.js` (all AJAX calls, one `Api.<Area>` namespace per feature),
`common.js` (shared UI helpers: navbar, alerts, query params), `config.js` (`USE_MOCK`/API base
URL/localStorage key names), `mockData.js` (in-browser fake data store used when `USE_MOCK` is true).

## 4. Known gotchas

Real issues hit (and fixed) during development — useful talking points for "what did you debug
and why does the code look like this":

- **`[ApiController]` + `<Nullable>enable</Nullable>` silently required every property.**
  With nullable reference types on project-wide, ASP.NET Core's `[ApiController]` infers an
  implicit `[Required]` on every non-nullable reference-type property of any `[FromBody]` model.
  Several endpoints intentionally accept partial objects (e.g. marking a country visited only
  needs `userId` + `country.countryId`, not every field of `UserVisitedCountry`), so that
  implicit rule rejected valid partial POSTs with an unhelpful 400. Fixed in `Program.cs` with
  `options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true`, which restores
  each property's actual nullability as the only source of truth for "is this required".

- **`$.when(...)` array-indexing trap on the client.** jQuery's `$.when(...)` resolves each
  input promise differently depending on how it was built. A promise built directly from
  `$.ajax()` keeps jQuery's classic 3-argument resolve signature, so `$.when` wraps its result as
  `[data, status, jqXHR]` — callers need `result[0]` for the actual data. A promise built via
  `.then()` that returns one value resolves with that value directly — `$.when` does **not**
  wrap it in an array, so indexing it with `[0]` silently grabs the wrong thing. In
  `ClientSide/js/api.js`, both `Api.UserCountries.getByUser` and `Api.Countries.getAll` are
  `.then()`-derived, so any `$.when(...)` call site that consumes them must not index their
  result with `[0]` — while a genuine `$.ajax()`-derived result in the same `$.when(...)` call
  still needs the `[0]`. This is easy to get backwards; check both promises' origins before
  indexing.

- **Composite-key duplicate inserts.** `UserVisitedCountries` and `UserWantedCountries` both
  have a composite primary key of `(UserId, CountryId)`. Marking the same country
  visited/wanted twice throws a `SqlException` with error number 2627 or 2601 (constraint
  violation). The relevant POST controllers (`UserVisitedCountriesController.Post`,
  `UsersController.AddWantedCountry`) catch that specific exception and return **409 Conflict**
  with a friendly message instead of letting a raw 500 through.

- **Stored procedure naming mismatches (`_3MD_TB` suffix).** Every stored procedure in this
  project is named with a `_3MD_TB` suffix (e.g. `spReadAllCountries_3MD_TB`). Earlier in
  development, the C# strings calling these procedures and the actual `CREATE PROCEDURE` names in
  the `.sql` scripts drifted out of sync in a few DAL files, causing "could not find stored
  procedure" errors at runtime. These were fixed across all `DAL/DB*Services.cs` files by making
  the C#-side procedure name strings match the SQL scripts exactly. Worth double-checking this
  suffix any time a new stored procedure is added.

- Other things worth knowing (not bugs fixed, but relevant context if asked): passwords are
  stored/compared in plaintext (no hashing) — a known simplification for this student project,
  not something in scope to fix here. Every DAL method uses `catch (Exception ex) { throw ex; }`
  instead of a bare `throw;`, which resets the stack trace on rethrow — cosmetic/debugging
  concern, not a functional bug.
