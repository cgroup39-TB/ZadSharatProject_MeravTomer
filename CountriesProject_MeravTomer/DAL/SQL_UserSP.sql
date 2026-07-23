-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert User>
-- =============================================
CREATE PROCEDURE spInsertUser_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read All Users>
-- =============================================
CREATE PROCEDURE spReadAllUsers_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read User By Id>
-- =============================================
CREATE PROCEDURE spReadUserById_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read User By Email>
-- =============================================
CREATE PROCEDURE spReadUserByEmail_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read User By Name>
-- =============================================
CREATE PROCEDURE spReadUserByName_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Update User>
-- =============================================
CREATE PROCEDURE spUpdateUser_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read User Preferred Regions>
-- =============================================
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
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert User Preferred Region>
-- =============================================
CREATE PROCEDURE spInsertUserRegion_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete User Preferred Regions>
-- =============================================
CREATE PROCEDURE spDeleteUserRegions_3MD_TB
    @UserId INT
AS
BEGIN

    DELETE FROM UserRegions
    WHERE UserId = @UserId;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read User Languages>
-- =============================================
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
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert User Language>
-- =============================================
CREATE PROCEDURE spInsertUserLanguage_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete User Languages>
-- =============================================
CREATE PROCEDURE spDeleteUserLanguages_3MD_TB
    @UserId INT
AS
BEGIN

    DELETE FROM UserLanguages
    WHERE UserId = @UserId;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read User Wanted Countries>
-- =============================================
CREATE PROCEDURE spReadUserWantedCountries_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert User Wanted Country>
-- =============================================
CREATE PROCEDURE spInsertUserWantedCountry_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete User Wanted Country>
-- =============================================
CREATE PROCEDURE spDeleteUserWantedCountry_3MD_TB
    @UserId INT,
    @CountryId INT
AS
BEGIN

    DELETE FROM UserWantedCountries
    WHERE UserId = @UserId
      AND CountryId = @CountryId;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Successful User Login>
-- =============================================
CREATE PROCEDURE spInsertUserLogin_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Admin Statistics>
-- =============================================
CREATE PROCEDURE spReadAdminStatistics_3MD_TB
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