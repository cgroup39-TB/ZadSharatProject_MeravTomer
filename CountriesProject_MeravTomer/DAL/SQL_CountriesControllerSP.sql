-- Regions --

CREATE PROCEDURE sp_Region_GetByName
    @RegionName NVARCHAR(100)
AS
BEGIN
    SELECT RegionId, RegionName
    FROM Regions
    WHERE RegionName = @RegionName;
END
GO

CREATE PROCEDURE sp_Region_Insert
    @RegionName NVARCHAR(100)
AS
BEGIN
    INSERT INTO Regions (RegionName)
    VALUES (@RegionName);

    SELECT SCOPE_IDENTITY() AS NewRegionId;
END
GO

CREATE PROCEDURE sp_Region_GetAll
AS
BEGIN
    SELECT RegionId, RegionName
    FROM Regions;
END
GO

-- Countries --

CREATE PROCEDURE sp_Country_Insert
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
    INSERT INTO Countries (CCA3, [Name], Capital, RegionId, SubRegion, [Population], Area, FlagUrl, Borders)
    VALUES (@CCA3, @Name, @Capital, @RegionId, @SubRegion, @Population, @Area, @FlagUrl, @Borders);

    SELECT SCOPE_IDENTITY() AS NewCountryId;
END
GO

CREATE PROCEDURE sp_Country_GetAll
AS
BEGIN
    SELECT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId;
END
GO

CREATE PROCEDURE sp_Country_GetById
    @CountryId INT
AS
BEGIN
    SELECT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    WHERE C.CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_Country_GetByName
    @Name NVARCHAR(50)
AS
BEGIN
    SELECT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    WHERE C.[Name] = @Name;
END
GO

CREATE PROCEDURE sp_Country_SearchByName
    @NamePart NVARCHAR(50)
AS
BEGIN
    SELECT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    WHERE C.[Name] LIKE '%' + @NamePart + '%';
END
GO

CREATE PROCEDURE sp_Country_GetByRegion
    @RegionName NVARCHAR(100)
AS
BEGIN
    SELECT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    INNER JOIN Regions AS R ON C.RegionId = R.RegionId
    WHERE R.RegionName = @RegionName;
END
GO

CREATE PROCEDURE sp_Country_GetByLanguage
    @LanguageName NVARCHAR(100)
AS
BEGIN
    SELECT DISTINCT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    INNER JOIN CountryLanguages AS CL ON C.CountryId = CL.CountryId
    INNER JOIN Languages AS L ON CL.LanguageId = L.LanguageId
    WHERE L.LanguageName = @LanguageName;
END
GO

CREATE PROCEDURE sp_Country_GetByCurrency
    @CurrencyCode NVARCHAR(3)
AS
BEGIN
    SELECT DISTINCT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM Countries AS C
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    INNER JOIN CountryCurrencies AS CC ON C.CountryId = CC.CountryId
    INNER JOIN Currencies AS CUR ON CC.CurrencyId = CUR.CurrencyId
    WHERE CUR.CurrencyCode = @CurrencyCode;
END
GO

CREATE PROCEDURE sp_Country_Update
    @CountryId INT,
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
    SET CCA3 = @CCA3,
        [Name] = @Name,
        Capital = @Capital,
        RegionId = @RegionId,
        SubRegion = @SubRegion,
        [Population] = @Population,
        Area = @Area,
        FlagUrl = @FlagUrl,
        Borders = @Borders
    WHERE CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_Country_Delete
    @CountryId INT
AS
BEGIN
    -- CountryLanguages/CountryCurrencies rows cascade-delete via the FK constraints.
    DELETE FROM Countries
    WHERE CountryId = @CountryId;
END
GO
