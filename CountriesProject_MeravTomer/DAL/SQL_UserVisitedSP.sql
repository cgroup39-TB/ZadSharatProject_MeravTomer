-- ============================================================
-- USER VISITED COUNTRIES
-- ============================================================


-- ------------------------------------------------------------
-- READ VISITS BY USER
-- ------------------------------------------------------------
CREATE PROCEDURE spReadVisitsByUser
    @UserId INT
AS
BEGIN

    SELECT
        UserId,
        CountryId,
        Rating,
        ReviewText,
        IsShared
    FROM UserVisitedCountries
    WHERE UserId = @UserId;

END
GO


-- ------------------------------------------------------------
-- READ VISITS BY COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spReadVisitsByCountry
    @CountryId INT
AS
BEGIN

    SELECT
        UserId,
        CountryId,
        Rating,
        ReviewText,
        IsShared
    FROM UserVisitedCountries
    WHERE CountryId = @CountryId;

END
GO


-- ------------------------------------------------------------
-- READ SHARED VISITS BY COUNTRY
-- ------------------------------------------------------------
CREATE PROCEDURE spReadSharedVisitsByCountry
    @CountryId INT
AS
BEGIN

    SELECT
        UserId,
        CountryId,
        Rating,
        ReviewText,
        IsShared
    FROM UserVisitedCountries
    WHERE CountryId = @CountryId
      AND IsShared = 1;

END
GO


-- ------------------------------------------------------------
-- READ SHARED VISITS BY USER
-- ------------------------------------------------------------
CREATE PROCEDURE spReadSharedVisitsByUser
    @UserId INT
AS
BEGIN

    SELECT
        UserId,
        CountryId,
        Rating,
        ReviewText,
        IsShared
    FROM UserVisitedCountries
    WHERE UserId = @UserId
      AND IsShared = 1;

END
GO


-- ------------------------------------------------------------
-- INSERT VISIT
-- ------------------------------------------------------------
CREATE PROCEDURE spInsertVisit
    @UserId INT,
    @CountryId INT,
    @Rating INT,
    @ReviewText NVARCHAR(1000),
    @IsShared BIT
AS
BEGIN

    INSERT INTO UserVisitedCountries
    (
        UserId,
        CountryId,
        Rating,
        ReviewText,
        IsShared
    )
    VALUES
    (
        @UserId,
        @CountryId,
        @Rating,
        @ReviewText,
        @IsShared
    );

END
GO


-- ------------------------------------------------------------
-- UPDATE VISIT
-- ------------------------------------------------------------
CREATE PROCEDURE spUpdateVisit
    @UserId INT,
    @CountryId INT,
    @Rating INT,
    @ReviewText NVARCHAR(1000),
    @IsShared BIT
AS
BEGIN

    UPDATE UserVisitedCountries
    SET
        Rating = @Rating,
        ReviewText = @ReviewText,
        IsShared = @IsShared
    WHERE UserId = @UserId
      AND CountryId = @CountryId;

END
GO


-- ------------------------------------------------------------
-- DELETE VISIT
-- ------------------------------------------------------------
CREATE PROCEDURE spDeleteVisit
    @UserId INT,
    @CountryId INT
AS
BEGIN

    DELETE FROM UserVisitedCountries
    WHERE UserId = @UserId
      AND CountryId = @CountryId;

END
GO