-- Visited countries --

CREATE PROCEDURE sp_VisitedCountries_GetByUserId
    @UserId INT
AS
BEGIN
    SELECT UV.UserId, UV.CountryId, UV.Rating, UV.ReviewText, UV.IsShared,
           C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM UserVisitedCountries AS UV
    INNER JOIN Countries AS C ON UV.CountryId = C.CountryId
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    WHERE UV.UserId = @UserId;
END
GO

CREATE PROCEDURE sp_VisitedCountries_Exists
    @UserId INT,
    @CountryId INT
AS
BEGIN
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM UserVisitedCountries WHERE UserId = @UserId AND CountryId = @CountryId
    ) THEN 1 ELSE 0 END AS DoesExist;
END
GO

CREATE PROCEDURE sp_VisitedCountries_Insert
    @UserId INT,
    @CountryId INT
AS
BEGIN
    INSERT INTO UserVisitedCountries (UserId, CountryId)
    VALUES (@UserId, @CountryId);
END
GO

CREATE PROCEDURE sp_VisitedCountries_Delete
    @UserId INT,
    @CountryId INT
AS
BEGIN
    DELETE FROM UserVisitedCountries
    WHERE UserId = @UserId AND CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_VisitedCountries_UpdateRating
    @UserId INT,
    @CountryId INT,
    @Rating INT
AS
BEGIN
    UPDATE UserVisitedCountries
    SET Rating = @Rating
    WHERE UserId = @UserId AND CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_VisitedCountries_UpdateReview
    @UserId INT,
    @CountryId INT,
    @ReviewText NVARCHAR(1000)
AS
BEGIN
    UPDATE UserVisitedCountries
    SET ReviewText = @ReviewText
    WHERE UserId = @UserId AND CountryId = @CountryId;
END
GO

CREATE PROCEDURE sp_VisitedCountries_UpdateIsShared
    @UserId INT,
    @CountryId INT,
    @IsShared BIT
AS
BEGIN
    UPDATE UserVisitedCountries
    SET IsShared = @IsShared
    WHERE UserId = @UserId AND CountryId = @CountryId;
END
GO

-- Wanted countries --

CREATE PROCEDURE sp_WantedCountries_GetByUserId
    @UserId INT
AS
BEGIN
    SELECT C.CountryId, C.CCA3, C.[Name], C.Capital, C.RegionId, R.RegionName, C.SubRegion,
           C.[Population], C.Area, C.FlagUrl, C.Borders
    FROM UserWantedCountries AS UW
    INNER JOIN Countries AS C ON UW.CountryId = C.CountryId
    LEFT JOIN Regions AS R ON C.RegionId = R.RegionId
    WHERE UW.UserId = @UserId;
END
GO

CREATE PROCEDURE sp_WantedCountries_Exists
    @UserId INT,
    @CountryId INT
AS
BEGIN
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM UserWantedCountries WHERE UserId = @UserId AND CountryId = @CountryId
    ) THEN 1 ELSE 0 END AS DoesExist;
END
GO

CREATE PROCEDURE sp_WantedCountries_Insert
    @UserId INT,
    @CountryId INT
AS
BEGIN
    INSERT INTO UserWantedCountries (UserId, CountryId)
    VALUES (@UserId, @CountryId);
END
GO

CREATE PROCEDURE sp_WantedCountries_Delete
    @UserId INT,
    @CountryId INT
AS
BEGIN
    DELETE FROM UserWantedCountries
    WHERE UserId = @UserId AND CountryId = @CountryId;
END
GO
