-- ============================================================
-- COUNTRIES
-- ============================================================


-- ------------------------------------------------------------
-- READ ALL COUNTRIES
-- ------------------------------------------------------------
CREATE PROCEDURE spReadAllCountries_MD_TB2
AS
BEGIN

    SELECT
        c.CountryId AS dbCountryId,
        c.CCA3,
        c.[Name],
        c.Capital,
        c.RegionId,
        r.RegionName,
        c.SubRegion,
        c.Population,
        c.Area,
        c.FlagUrl,
        c.Borders
    FROM Countries c
    LEFT JOIN Regions r
        ON c.RegionId = r.RegionId;

END
GO


-- ------------------------------------------------------------
-- READ COUNTRY BY ID
-- ------------------------------------------------------------
CREATE PROCEDURE spReadCountryById_MD_TB2
    @Id INT
AS
BEGIN

    SELECT
        c.CountryId AS dbCountryId,
        c.CCA3,
        c.[Name],
        c.Capital,
        c.RegionId,
        r.RegionName,
        c.SubRegion,
        c.Population,
        c.Area,
        c.FlagUrl,
        c.Borders
    FROM Countries c
    LEFT JOIN Regions r
        ON c.RegionId = r.RegionId
    WHERE c.CountryId = @Id;

END
GO


-- ------------------------------------------------------------
-- READ COUNTRY BY NAME
-- ------------------------------------------------------------
CREATE PROCEDURE spReadCountryByName
    @Name NVARCHAR(50)
AS
BEGIN

    SELECT
        c.CountryId AS dbCountryId,
        c.CCA3,
        c.[Name],
        c.Capital,
        c.RegionId,
        r.RegionName,
        c.SubRegion,
        c.Population,
        c.Area,
        c.FlagUrl,
        c.Borders
    FROM Countries c
    LEFT JOIN Regions r
        ON c.RegionId = r.RegionId
    WHERE c.[Name] = @Name;

END
GO


-- ------------------------------------------------------------
-- READ COUNTRIES BY REGION
-- ------------------------------------------------------------
CREATE PROCEDURE spReadCountriesByRegion
    @RegionId INT
AS
BEGIN

    SELECT
        c.CountryId AS dbCountryId,
        c.CCA3,
        c.[Name],
        c.Capital,
        c.RegionId,
        r.RegionName,
        c.SubRegion,
        c.Population,
        c.Area,
        c.FlagUrl,
        c.Borders
    FROM Countries c
    INNER JOIN Regions r
        ON c.RegionId = r.RegionId
    WHERE c.RegionId = @RegionId;

END
GO


-- ------------------------------------------------------------
-- INSERT COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertCountry_MD_TB2
    @CCA3 NVARCHAR(3),
    @Name NVARCHAR(50),
    @Capital NVARCHAR(50),
    @RegionId INT,
    @SubRegion NVARCHAR(50),
    @Population BIGINT,
    @Area FLOAT,
    @FlagUrl NVARCHAR(500),
    @Borders NVARCHAR(500)
AS
BEGIN

    INSERT INTO Countries
    (
        CCA3,
        [Name],
        Capital,
        RegionId,
        SubRegion,
        Population,
        Area,
        FlagUrl,
        Borders
    )
    VALUES
    (
        @CCA3,
        @Name,
        @Capital,
        @RegionId,
        @SubRegion,
        @Population,
        @Area,
        @FlagUrl,
        @Borders
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);

END
GO


-- ------------------------------------------------------------
-- UPDATE COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spUpdateCountry
    @Id INT,
    @CCA3 NVARCHAR(3),
    @Name NVARCHAR(50),
    @Capital NVARCHAR(50),
    @RegionId INT,
    @SubRegion NVARCHAR(50),
    @Population BIGINT,
    @Area FLOAT,
    @FlagUrl NVARCHAR(500),
    @Borders NVARCHAR(500)
AS
BEGIN

    UPDATE Countries
    SET
        CCA3 = @CCA3,
        [Name] = @Name,
        Capital = @Capital,
        RegionId = @RegionId,
        SubRegion = @SubRegion,
        Population = @Population,
        Area = @Area,
        FlagUrl = @FlagUrl,
        Borders = @Borders
    WHERE CountryId = @Id;

END
GO


-- ------------------------------------------------------------
-- DELETE COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteCountry
    @CountryId INT
AS
BEGIN

    DELETE FROM Countries
    WHERE CountryId = @CountryId;

END
GO



-- ============================================================
-- LANGUAGES
-- ============================================================


-- ------------------------------------------------------------
-- READ ALL LANGUAGES
-- ------------------------------------------------------------
CREATE PROCEDURE spReadAllLanguages
AS
BEGIN

    SELECT
        LanguageId,
        LanguageName
    FROM Languages;

END
GO


-- ------------------------------------------------------------
-- INSERT LANGUAGE
-- LanguageId is IDENTITY - therefore it is NOT inserted manually
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertLanguage_MD_TB2
    @LanguageName NVARCHAR(50)
AS
BEGIN

    INSERT INTO Languages
    (
        LanguageName
    )
    VALUES
    (
        @LanguageName
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);

END
GO



-- ============================================================
-- COUNTRY - LANGUAGES
-- ============================================================


-- ------------------------------------------------------------
-- READ LANGUAGES BY COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE sp_CountryLanguages_GetByCountryId
    @CountryId INT
AS
BEGIN

    SELECT
        l.LanguageId,
        l.LanguageName
    FROM CountryLanguages cl
    INNER JOIN Languages l
        ON cl.LanguageId = l.LanguageId
    WHERE cl.CountryId = @CountryId;

END
GO


-- ------------------------------------------------------------
-- INSERT COUNTRY-LANGUAGE RELATION
-- ------------------------------------------------------------
CREATE PROCEDURE sp_CountryLanguages_Insert
    @CountryId INT,
    @LanguageId INT
AS
BEGIN

    INSERT INTO CountryLanguages
    (
        CountryId,
        LanguageId
    )
    VALUES
    (
        @CountryId,
        @LanguageId
    );

END
GO


-- ------------------------------------------------------------
-- DELETE LANGUAGE RELATIONS OF COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteLanguageByCountryId_MD_TB2
    @CountryId INT
AS
BEGIN

    DELETE FROM CountryLanguages
    WHERE CountryId = @CountryId;

END
GO



-- ============================================================
-- CURRENCIES
-- ============================================================


-- ------------------------------------------------------------
-- READ ALL CURRENCIES
-- ------------------------------------------------------------
CREATE PROCEDURE spReadAllCurrencies_MD_TB2
AS
BEGIN

    SELECT
        CurrencyId,
        CurrencyCode,
        [Name],
        Symbol
    FROM Currencies;

END
GO


-- ------------------------------------------------------------
-- INSERT CURRENCY
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertCurrency_MD_TB2
    @Code NVARCHAR(3),
    @Name NVARCHAR(50),
    @Symbol NVARCHAR(20)
AS
BEGIN

    INSERT INTO Currencies
    (
        CurrencyCode,
        [Name],
        Symbol
    )
    VALUES
    (
        @Code,
        @Name,
        @Symbol
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);

END
GO



-- ============================================================
-- COUNTRY - CURRENCIES
-- ============================================================


-- ------------------------------------------------------------
-- READ CURRENCIES BY COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE sp_CountryCurrencies_GetByCountryId
    @CountryId INT
AS
BEGIN

    SELECT
        c.CurrencyId,
        c.CurrencyCode,
        c.[Name] AS CurrencyName,
        c.Symbol AS CurrencySymbol
    FROM CountryCurrencies cc
    INNER JOIN Currencies c
        ON cc.CurrencyId = c.CurrencyId
    WHERE cc.CountryId = @CountryId;

END
GO


-- ------------------------------------------------------------
-- INSERT COUNTRY-CURRENCY RELATION
-- ------------------------------------------------------------
CREATE PROCEDURE sp_CountryCurrencies_Insert
    @CountryId INT,
    @CurrencyId INT
AS
BEGIN

    INSERT INTO CountryCurrencies
    (
        CountryId,
        CurrencyId
    )
    VALUES
    (
        @CountryId,
        @CurrencyId
    );

END
GO


-- ------------------------------------------------------------
-- DELETE CURRENCY RELATIONS OF COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteCurrencyByCountryId_MD_TB2
    @CountryId INT
AS
BEGIN

    DELETE FROM CountryCurrencies
    WHERE CountryId = @CountryId;

END
GO


CREATE PROCEDURE spReadSortedCountries
    @SortBy NVARCHAR(20),
    @Ascending BIT
AS
BEGIN

    SELECT
        c.CountryId AS dbCountryId,
        c.CCA3,
        c.[Name],
        c.Capital,
        c.RegionId,
        r.RegionName,
        c.SubRegion,
        c.Population,
        c.Area,
        c.FlagUrl,
        c.Borders

    FROM Countries c

    LEFT JOIN Regions r
        ON c.RegionId = r.RegionId

    ORDER BY

        CASE
            WHEN @SortBy = 'name'
             AND @Ascending = 1
            THEN c.[Name]
        END ASC,

        CASE
            WHEN @SortBy = 'name'
             AND @Ascending = 0
            THEN c.[Name]
        END DESC,

        CASE
            WHEN @SortBy = 'population'
             AND @Ascending = 1
            THEN c.Population
        END ASC,

        CASE
            WHEN @SortBy = 'population'
             AND @Ascending = 0
            THEN c.Population
        END DESC;

END
GO