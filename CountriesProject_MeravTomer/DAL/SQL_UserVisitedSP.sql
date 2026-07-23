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