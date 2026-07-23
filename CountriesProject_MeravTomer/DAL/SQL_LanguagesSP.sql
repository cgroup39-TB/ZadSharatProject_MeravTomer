-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read All Languages>
-- =============================================
CREATE PROCEDURE spReadAllLanguages_3MD_TB
AS
BEGIN

    SELECT
        LanguageId,
        LanguageName
    FROM Languages;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Language By Id>
-- =============================================
CREATE PROCEDURE spReadLanguageById_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Language By Name>
-- =============================================
CREATE PROCEDURE spReadLanguageByName_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Language>
-- =============================================
CREATE PROCEDURE spInsertLanguage_3MD_TB
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