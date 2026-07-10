CREATE PROCEDURE spInsertCountry_MD_TB2
    @SteamAppId INT,
    @Name NVARCHAR(200),
    @SteamUrl NVARCHAR(500),
    @CapsuleImage NVARCHAR(500),
    @ReleaseDate NVARCHAR(50),
    @ReviewSummary NVARCHAR(20),
    @Price INT,
    @Windows BIT,
    @Mac BIT,
    @Linux BIT
AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO GamesTable_MD_TB2
    (
        SteamAppId,
        [Name],
        SteamUrl,
        CapsuleImage,
        ReleaseDate,
        ReviewSummary,
        Price,
        Windows,
        Mac,
        Linux
    )
    VALUES
    (
        @SteamAppId,
        @Name,
        @SteamUrl,
        @CapsuleImage,
        @ReleaseDate,
        @ReviewSummary,
        @Price,
        @Windows,
        @Mac,
        @Linux
    );

    SELECT SCOPE_IDENTITY() AS NewGameId; --returns the last id that identity insereted in the table (last game that was inserted) for now it is not neccessary but maybe later
END
GO
