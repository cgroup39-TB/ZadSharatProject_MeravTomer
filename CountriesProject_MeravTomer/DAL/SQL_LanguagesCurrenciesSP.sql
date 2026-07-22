-- Languages --

CREATE PROCEDURE sp_Language_GetAll
AS
BEGIN
    SELECT LanguageId, LanguageName
    FROM Languages;
END
GO

CREATE PROCEDURE sp_Language_GetByName
    @LanguageName NVARCHAR(100)
AS
BEGIN
    SELECT LanguageId, LanguageName
    FROM Languages
    WHERE LanguageName = @LanguageName;
END
GO

CREATE PROCEDURE sp_Language_Insert
    @LanguageName NVARCHAR(100)
AS
BEGIN
    INSERT INTO Languages (LanguageName)
    VALUES (@LanguageName);

    SELECT SCOPE_IDENTITY() AS NewLanguageId;
END
GO

-- Currencies --

CREATE PROCEDURE sp_Currency_GetAll
AS
BEGIN
    SELECT CurrencyId, CurrencyCode, [Name], Symbol
    FROM Currencies;
END
GO

CREATE PROCEDURE sp_Currency_GetByCode
    @CurrencyCode NVARCHAR(3)
AS
BEGIN
    SELECT CurrencyId, CurrencyCode, [Name], Symbol
    FROM Currencies
    WHERE CurrencyCode = @CurrencyCode;
END
GO

CREATE PROCEDURE sp_Currency_Insert
    @CurrencyCode NVARCHAR(3),
    @Name NVARCHAR(50),
    @Symbol NVARCHAR(20)
AS
BEGIN
    INSERT INTO Currencies (CurrencyCode, [Name], Symbol)
    VALUES (@CurrencyCode, @Name, @Symbol);

    SELECT SCOPE_IDENTITY() AS NewCurrencyId;
END
GO

-- Country <-> Language links --

CREATE PROCEDURE sp_CountryLanguages_GetByCountryId
    @CountryId INT
AS
BEGIN
    SELECT L.LanguageId, L.LanguageName
    FROM CountryLanguages AS CL
    INNER JOIN Languages AS L ON CL.LanguageId = L.LanguageId
    WHERE CL.CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_CountryLanguages_Insert
    @CountryId INT,
    @LanguageId INT
AS
BEGIN
    INSERT INTO CountryLanguages (CountryId, LanguageId)
    VALUES (@CountryId, @LanguageId);
END
GO

CREATE PROCEDURE sp_CountryLanguages_DeleteByCountryId
    @CountryId INT
AS
BEGIN
    DELETE FROM CountryLanguages
    WHERE CountryId = @CountryId;
END
GO

-- Country <-> Currency links --

CREATE PROCEDURE sp_CountryCurrencies_GetByCountryId
    @CountryId INT
AS
BEGIN
    SELECT CUR.CurrencyId, CUR.CurrencyCode, CUR.[Name], CUR.Symbol
    FROM CountryCurrencies AS CC
    INNER JOIN Currencies AS CUR ON CC.CurrencyId = CUR.CurrencyId
    WHERE CC.CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_CountryCurrencies_Insert
    @CountryId INT,
    @CurrencyId INT
AS
BEGIN
    INSERT INTO CountryCurrencies (CountryId, CurrencyId)
    VALUES (@CountryId, @CurrencyId);
END
GO

CREATE PROCEDURE sp_CountryCurrencies_DeleteByCountryId
    @CountryId INT
AS
BEGIN
    DELETE FROM CountryCurrencies
    WHERE CountryId = @CountryId;
END
GO
