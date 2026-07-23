-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read All Currencies>
-- =============================================
CREATE PROCEDURE spReadAllCurrencies_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Currency By Id>
-- =============================================
CREATE PROCEDURE spReadCurrencyById_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Currency By Code>
-- =============================================
CREATE PROCEDURE spReadCurrencyByCode_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Currency>
-- =============================================
CREATE PROCEDURE spInsertCurrency_3MD_TB
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