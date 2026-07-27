/*
 * Idempotent catch-up script for databases created before the user-preferences
 * feature (regions/languages) and the seed demo users were added to
 * CreateTables.sql / SQL_UserSP.sql.
 *
 * Symptoms this fixes:
 *   - SqlException: "Could not find stored procedure 'spReadUserLanguages_3MD_TB'"
 *     (or spReadUserRegions_3MD_TB / spInsertUserRegion_3MD_TB / etc.)
 *   - Logging in with admin@test.com / admin123 (or test@test.com / 123456)
 *     fails because those rows don't exist in Users yet.
 *
 * Both of these mean the database is out of sync with the current
 * CreateTables.sql / SQL_UserSP.sql - this script only creates/inserts
 * whatever is actually missing, so it's safe to run repeatedly and safe to
 * run against a database that already has some or all of these.
 *
 * Update the USE statement below to your actual database name (the
 * "Initial Catalog" in appsettings.json's connection string - NOT the
 * "myProjDB" connection-string key name, which is just a label).
 */

USE [igroup139_test2];
GO

-- ---------- Seed demo users ----------

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'admin@test.com')
INSERT INTO Users ([Name], Email, [Password], IsAdmin, IsActive, CanShare)
VALUES (N'Admin', N'admin@test.com', N'admin123', 1, 1, 1);
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'test@test.com')
INSERT INTO Users ([Name], Email, [Password], IsAdmin, IsActive, CanShare)
VALUES (N'Test User', N'test@test.com', N'123456', 0, 1, 1);
GO

-- ---------- User preferred-regions procedures ----------

IF OBJECT_ID('dbo.spReadUserRegions_3MD_TB', 'P') IS NULL
EXEC('
CREATE PROCEDURE spReadUserRegions_3MD_TB
    @UserId INT
AS
BEGIN
    SELECT
        r.RegionId,
        r.RegionName
    FROM UserRegions ur
    INNER JOIN Regions r
        ON ur.RegionId = r.RegionId
    WHERE ur.UserId = @UserId;
END
');
GO

IF OBJECT_ID('dbo.spInsertUserRegion_3MD_TB', 'P') IS NULL
EXEC('
CREATE PROCEDURE spInsertUserRegion_3MD_TB
    @UserId INT,
    @RegionId INT
AS
BEGIN
    INSERT INTO UserRegions (UserId, RegionId)
    VALUES (@UserId, @RegionId);
END
');
GO

IF OBJECT_ID('dbo.spDeleteUserRegions_3MD_TB', 'P') IS NULL
EXEC('
CREATE PROCEDURE spDeleteUserRegions_3MD_TB
    @UserId INT
AS
BEGIN
    DELETE FROM UserRegions
    WHERE UserId = @UserId;
END
');
GO

-- ---------- User languages procedures ----------

IF OBJECT_ID('dbo.spReadUserLanguages_3MD_TB', 'P') IS NULL
EXEC('
CREATE PROCEDURE spReadUserLanguages_3MD_TB
    @UserId INT
AS
BEGIN
    SELECT
        ul.UserId,
        l.LanguageId,
        l.LanguageName,
        ul.LevelLanguage
    FROM UserLanguages ul
    INNER JOIN Languages l
        ON ul.LanguageId = l.LanguageId
    WHERE ul.UserId = @UserId;
END
');
GO

IF OBJECT_ID('dbo.spInsertUserLanguage_3MD_TB', 'P') IS NULL
EXEC('
CREATE PROCEDURE spInsertUserLanguage_3MD_TB
    @UserId INT,
    @LanguageId INT,
    @LevelLanguage INT = NULL
AS
BEGIN
    INSERT INTO UserLanguages (UserId, LanguageId, LevelLanguage)
    VALUES (@UserId, @LanguageId, @LevelLanguage);
END
');
GO

IF OBJECT_ID('dbo.spDeleteUserLanguages_3MD_TB', 'P') IS NULL
EXEC('
CREATE PROCEDURE spDeleteUserLanguages_3MD_TB
    @UserId INT
AS
BEGIN
    DELETE FROM UserLanguages
    WHERE UserId = @UserId;
END
');
GO

-- ---------- Sanity checks ----------

SELECT DB_NAME() AS ranAgainstDatabase;

SELECT UserId, Name, Email, IsAdmin, IsActive, CanShare
FROM Users
WHERE Email IN (N'admin@test.com', N'test@test.com');

SELECT name FROM sys.objects
WHERE type = 'P' AND name IN (
    'spReadUserRegions_3MD_TB', 'spInsertUserRegion_3MD_TB', 'spDeleteUserRegions_3MD_TB',
    'spReadUserLanguages_3MD_TB', 'spInsertUserLanguage_3MD_TB', 'spDeleteUserLanguages_3MD_TB'
);
