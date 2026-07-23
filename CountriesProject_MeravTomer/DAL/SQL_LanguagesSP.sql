CREATE PROCEDURE spReadAllLanguages
AS
BEGIN
    SELECT
        LanguageId,
        LanguageName
    FROM Languages;
END
GO


CREATE PROCEDURE spReadLanguageById
    @LanguageId INT
AS
BEGIN
    SELECT
        LanguageId,
        LanguageName
    FROM Languages
    WHERE LanguageId = @LanguageId;
END
GO


CREATE PROCEDURE spReadLanguageByName
    @LanguageName NVARCHAR(50)
AS
BEGIN
    SELECT
        LanguageId,
        LanguageName
    FROM Languages
    WHERE LanguageName = @LanguageName;
END
GO


CREATE PROCEDURE spInsertLanguage
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