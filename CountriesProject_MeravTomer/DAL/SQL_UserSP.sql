-- ============================================================
-- USERS
-- ============================================================


-- ------------------------------------------------------------
-- INSERT USER
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertUser
    @Name NVARCHAR(100),
    @Email NVARCHAR(150),
    @Password NVARCHAR(255),
    @IsActive BIT,
    @IsAdmin BIT,
    @CanShare BIT
AS
BEGIN

    INSERT INTO Users
    (
        [Name],
        Email,
        [Password],
        IsActive,
        IsAdmin,
        CanShare
    )
    VALUES
    (
        @Name,
        @Email,
        @Password,
        @IsActive,
        @IsAdmin,
        @CanShare
    );

END
GO


-- ------------------------------------------------------------
-- READ ALL USERS
-- ------------------------------------------------------------
CREATE PROCEDURE spReadAllUsers
AS
BEGIN

    SELECT
        UserId,
        [Name],
        Email,
        [Password],
        IsActive,
        IsAdmin,
        CanShare
    FROM Users;

END
GO


-- ------------------------------------------------------------
-- READ USER BY ID
-- ------------------------------------------------------------
CREATE PROCEDURE spReadUserById
    @UserId INT
AS
BEGIN

    SELECT
        UserId,
        [Name],
        Email,
        [Password],
        IsActive,
        IsAdmin,
        CanShare
    FROM Users
    WHERE UserId = @UserId;

END
GO


-- ------------------------------------------------------------
-- READ USER BY EMAIL
-- ------------------------------------------------------------
CREATE PROCEDURE spReadUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN

    SELECT
        UserId,
        [Name],
        Email,
        [Password],
        IsActive,
        IsAdmin,
        CanShare
    FROM Users
    WHERE Email = @Email;

END
GO


-- ------------------------------------------------------------
-- READ USER BY NAME
-- ------------------------------------------------------------
CREATE PROCEDURE spReadUserByName
    @Name NVARCHAR(100)
AS
BEGIN

    SELECT
        UserId,
        [Name],
        Email,
        [Password],
        IsActive,
        IsAdmin,
        CanShare
    FROM Users
    WHERE [Name] = @Name;

END
GO


-- ------------------------------------------------------------
-- UPDATE USER
-- One UPDATE used by several BL methods
-- ------------------------------------------------------------
CREATE PROCEDURE spUpdateUser
    @UserId INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(150),
    @Password NVARCHAR(255),
    @IsActive BIT,
    @IsAdmin BIT,
    @CanShare BIT
AS
BEGIN

    UPDATE Users
    SET
        [Name] = @Name,
        Email = @Email,
        [Password] = @Password,
        IsActive = @IsActive,
        IsAdmin = @IsAdmin,
        CanShare = @CanShare
    WHERE UserId = @UserId;

END
GO



-- ============================================================
-- USER REGIONS
-- ============================================================


-- ------------------------------------------------------------
-- READ USER REGIONS
-- ------------------------------------------------------------
CREATE PROCEDURE spReadUserRegions
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
GO


-- ------------------------------------------------------------
-- INSERT USER REGION
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertUserRegion
    @UserId INT,
    @RegionId INT
AS
BEGIN

    INSERT INTO UserRegions
    (
        UserId,
        RegionId
    )
    VALUES
    (
        @UserId,
        @RegionId
    );

END
GO


-- ------------------------------------------------------------
-- DELETE ALL USER REGIONS
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteUserRegions
    @UserId INT
AS
BEGIN

    DELETE FROM UserRegions
    WHERE UserId = @UserId;

END
GO



-- ============================================================
-- USER LANGUAGES
-- ============================================================


-- ------------------------------------------------------------
-- READ USER LANGUAGES
-- ------------------------------------------------------------
CREATE PROCEDURE spReadUserLanguages
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
GO


-- ------------------------------------------------------------
-- INSERT USER LANGUAGE
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertUserLanguage
    @UserId INT,
    @LanguageId INT,
    @LevelLanguage INT = NULL
AS
BEGIN

    INSERT INTO UserLanguages
    (
        UserId,
        LanguageId,
        LevelLanguage
    )
    VALUES
    (
        @UserId,
        @LanguageId,
        @LevelLanguage
    );

END
GO


-- ------------------------------------------------------------
-- DELETE ALL USER LANGUAGES
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteUserLanguages
    @UserId INT
AS
BEGIN

    DELETE FROM UserLanguages
    WHERE UserId = @UserId;

END
GO



-- ============================================================
-- USER WANTED COUNTRIES
-- ============================================================


-- ------------------------------------------------------------
-- READ WANTED COUNTRIES
-- ------------------------------------------------------------
CREATE PROCEDURE spReadUserWantedCountries
    @UserId INT
AS
BEGIN

    SELECT
        c.CountryId,
        c.CCA3,
        c.[Name],
        c.Capital,
        c.RegionId,
        r.RegionName,
        c.SubRegion,
        c.Population,
        c.Area,
        c.FlagUrl,
        c.Borders
    FROM UserWantedCountries uwc

    INNER JOIN Countries c
        ON uwc.CountryId = c.CountryId

    LEFT JOIN Regions r
        ON c.RegionId = r.RegionId

    WHERE uwc.UserId = @UserId;

END
GO


-- ------------------------------------------------------------
-- ADD WANTED COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertUserWantedCountry
    @UserId INT,
    @CountryId INT
AS
BEGIN

    IF NOT EXISTS
    (
        SELECT 1
        FROM UserWantedCountries
        WHERE UserId = @UserId
          AND CountryId = @CountryId
    )
    BEGIN

        INSERT INTO UserWantedCountries
        (
            UserId,
            CountryId
        )
        VALUES
        (
            @UserId,
            @CountryId
        );

    END

END
GO


-- ------------------------------------------------------------
-- REMOVE WANTED COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteUserWantedCountry
    @UserId INT,
    @CountryId INT
AS
BEGIN

    DELETE FROM UserWantedCountries
    WHERE UserId = @UserId
      AND CountryId = @CountryId;

END
GO

CREATE PROCEDURE spInsertUserLogin
    @UserId INT
AS
BEGIN

    INSERT INTO UserLogins
    (
        UserId,
        LoginDate
    )
    VALUES
    (
        @UserId,
        GETDATE()
    );

END
GO


CREATE PROCEDURE spReadAdminStatistics
AS
BEGIN

    SELECT

        (
            SELECT COUNT(*)
            FROM UserLogins
            WHERE CAST(LoginDate AS DATE)
                = CAST(GETDATE() AS DATE)
        ) AS DailyLogins,

        (
            SELECT COUNT(*)
            FROM Countries
        ) AS ImportedCountries,

        (
            (SELECT COUNT(*)
             FROM UserVisitedCountries)

            +

            (SELECT COUNT(*)
             FROM UserWantedCountries)
        ) AS SavedCountries,

        (
            SELECT COUNT(*)
            FROM UserVisitedCountries
            WHERE IsShared = 1
        ) AS SharedReviews;

END
GO

