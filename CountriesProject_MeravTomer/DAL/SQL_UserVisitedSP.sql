-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Visits By User>
-- =============================================
CREATE PROCEDURE spReadVisitsByUser_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Visits By Country>
-- =============================================
CREATE PROCEDURE spReadVisitsByCountry_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Shared Reviews/Visits of a Country>
-- =============================================
CREATE PROCEDURE spReadSharedVisitsByCountry_3MD_TB
    @CountryId INT
AS
BEGIN

    SELECT
        v.UserId,
        v.CountryId,
        v.Rating,
        v.ReviewText,
        v.IsShared,
        v.CreatedAt,
        u.[Name] AS UserName
    FROM UserVisitedCountries v
    INNER JOIN Users u
        ON v.UserId = u.UserId
    WHERE v.CountryId = @CountryId
      AND v.IsShared = 1;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Shared Reviews/Visits of a User>
-- =============================================
CREATE PROCEDURE spReadSharedVisitsByUser_3MD_TB
    @UserId INT
AS
BEGIN

    SELECT
        v.UserId,
        v.CountryId,
        v.Rating,
        v.ReviewText,
        v.IsShared,
        v.CreatedAt,
        u.[Name] AS UserName
    FROM UserVisitedCountries v
    INNER JOIN Users u
        ON v.UserId = u.UserId
    WHERE v.UserId = @UserId
      AND v.IsShared = 1;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <24.7.26>
-- Description:	<Read All Shared Reviews/Visits (every country/user)>
-- =============================================
CREATE PROCEDURE spReadAllSharedVisits_3MD_TB
AS
BEGIN

    SELECT
        v.UserId,
        v.CountryId,
        v.Rating,
        v.ReviewText,
        v.IsShared,
        v.CreatedAt,
        u.[Name] AS UserName
    FROM UserVisitedCountries v
    INNER JOIN Users u
        ON v.UserId = u.UserId
    WHERE v.IsShared = 1
    ORDER BY v.CreatedAt DESC;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert User Visit>
-- =============================================
CREATE PROCEDURE spInsertVisit_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Update User Visit>
-- =============================================
CREATE PROCEDURE spUpdateVisit_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Delete User Visit>
-- =============================================
CREATE PROCEDURE spDeleteVisit_3MD_TB
    @UserId INT,
    @CountryId INT
AS
BEGIN

    DELETE FROM UserVisitedCountries
    WHERE UserId = @UserId
      AND CountryId = @CountryId;

END
GO