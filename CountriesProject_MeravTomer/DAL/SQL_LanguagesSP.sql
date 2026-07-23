-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Insert a new Language>
-- =============================================
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


-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Read all languages existing>
-- =============================================
CREATE PROCEDURE spReadAllLanguages
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM LanguagesTable;
END
GO 


