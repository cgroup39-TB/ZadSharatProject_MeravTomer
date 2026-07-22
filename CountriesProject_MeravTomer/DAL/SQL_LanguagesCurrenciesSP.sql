CREATE PROCEDURE spInsertLanguage
    @LanguageName NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO Languages
    (
        LanguageName  
    )
    VALUES
    (
        @LanguageName
    );

    SELECT SCOPE_IDENTITY() AS NewLanguageId; --returns the last id that identity insereted in the table (last game that was inserted) for now it is not neccessary but maybe later
END
GO



CREATE PROCEDURE spReadAllLanguages
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM LanguagesTable;
END
GO 

CREATE PROCEDURE spInsertCurrency
    @CurrencyCode NVARCHAR(3),
    @Name NVARCHAR(50),
    @Symbol  NVARCHAR(20)

AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO Languages
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

    SELECT SCOPE_IDENTITY() AS NewCurrencyId; --returns the last id that identity insereted in the table (last game that was inserted) for now it is not neccessary but maybe later
END
GO
