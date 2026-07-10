CREATE PROCEDURE spInsertCountry_MD_TB2
    @CCA3  NVARCHAR(4),
    @Name NVARCHAR(50),
    @OfficialName NVARCHAR(50),
    --@Capital NVARCHAR(500),
    @Region NVARCHAR(50),
    @SubRegion NVARCHAR(20),
    @Population BIGINT,
    @Area FLOAT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @FlagUrl NVARCHAR(500)
AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO CountriesTable_MD_TB2
    (
        CCA3,
        [Name],
        OfficialName,
      --  Capital,
        Region,
        SubRegion,
        [Population],
        Area,
        Latitude,
        Longitude,
        FlagUrl
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
