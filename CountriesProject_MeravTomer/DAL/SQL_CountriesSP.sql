-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read All Countries>
-- =============================================
CREATE PROCEDURE spReadAllCountries_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Country By Id>
-- =============================================
CREATE PROCEDURE spReadCountryById_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Country By Name>
-- =============================================
CREATE PROCEDURE spReadCountryByName_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Countries By Region>
-- =============================================
CREATE PROCEDURE spReadCountriesByRegion_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Country>
-- =============================================
CREATE PROCEDURE spInsertCountry_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Update Country>
-- =============================================
CREATE PROCEDURE spUpdateCountry_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete Country>
-- =============================================
CREATE PROCEDURE spDeleteCountry_3MD_TB
    @CountryId INT
AS
BEGIN

    DELETE FROM Countries
    WHERE CountryId = @CountryId;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Sorted Countries By Name Or Population>
-- =============================================
CREATE PROCEDURE spReadSortedCountries_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Languages By Country Id>
-- =============================================
CREATE PROCEDURE sp_CountryLanguages_GetByCountryId_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Country Language Relation>
-- =============================================
CREATE PROCEDURE sp_CountryLanguages_Insert_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete Country Language Relations>
-- =============================================
CREATE PROCEDURE spDeleteLanguageByCountryId_3MD_TB
    @CountryId INT
AS
BEGIN

    DELETE FROM CountryLanguages
    WHERE CountryId = @CountryId;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Currencies By Country Id>
-- =============================================
CREATE PROCEDURE sp_CountryCurrencies_GetByCountryId_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Country Currency Relation>
-- =============================================
CREATE PROCEDURE sp_CountryCurrencies_Insert_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete Country Currency Relations>
-- =============================================
CREATE PROCEDURE spDeleteCurrencyByCountryId_3MD_TB
    @CountryId INT
AS
BEGIN

    DELETE FROM CountryCurrencies
    WHERE CountryId = @CountryId;

END
GO