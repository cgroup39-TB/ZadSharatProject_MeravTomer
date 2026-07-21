CREATE PROCEDURE spInsertLanguage_MD_TB2
    @Code NVARCHAR(100),
    @Name NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO LanguagesTable_MD_TB2
    (
        Code,
        [Name] 
      
    )
    VALUES
    (
        @Code,
        @Name
    );

    SELECT SCOPE_IDENTITY() AS NewLanguageId; --returns the last id that identity insereted in the table (last game that was inserted) for now it is not neccessary but maybe later
END
GO



CREATE PROCEDURE spReadAllLanguages_MD_TB2
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM LanguagesTable_MD_TB2;
END
GO 



CREATE PROCEDURE spInsertCurrency_MD_TB2
    @Code NVARCHAR(100),
    @Name NVARCHAR(50),
    @Symbol  NVARCHAR(50)

AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO LanguagesTable_MD_TB2
    (
        Code,
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


CREATE PROCEDURE sp_CountryLanguages_GetByCountryId
    @CountryId INT
AS
BEGIN
    SELECT TagName
    FROM TagGameTable_MD_TB2
    WHERE CountryId = @CountryId;
END
GO



CREATE PROCEDURE spAddGameToUserCollection_MD_TB2
    @UserId INT,
    @GameId INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM UsersGamesTable_MD_TB2
        WHERE UserId = @UserId
        AND GameId = @GameId
    )
    BEGIN
        INSERT INTO UsersGamesTable_MD_TB2
        (
            UserId,
            GameId
        )
        VALUES
        (
            @UserId,
            @GameId
        );
    END
END
GO