CREATE PROCEDURE spReadAllCurrencies
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


CREATE PROCEDURE spReadCurrencyById
    @CurrencyId INT
AS
BEGIN
    SELECT
        CurrencyId,
        CurrencyCode,
        [Name],
        Symbol
    FROM Currencies
    WHERE CurrencyId = @CurrencyId;
END
GO


CREATE PROCEDURE spReadCurrencyByCode
    @CurrencyCode NVARCHAR(3)
AS
BEGIN
    SELECT
        CurrencyId,
        CurrencyCode,
        [Name],
        Symbol
    FROM Currencies
    WHERE CurrencyCode = @CurrencyCode;
END
GO


CREATE PROCEDURE spInsertCurrency
    @CurrencyCode NVARCHAR(3),
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
        @CurrencyCode,
        @Name,
        @Symbol
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO