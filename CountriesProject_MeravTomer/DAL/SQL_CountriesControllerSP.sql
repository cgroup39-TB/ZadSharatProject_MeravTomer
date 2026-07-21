CREATE PROCEDURE spInsertCountry_MD_TB2
    @CCA3 NVARCHAR(4),
    @Name NVARCHAR(50),
    @Capital NVARCHAR(50),
    @Region NVARCHAR(50),
    @SubRegion NVARCHAR(50),
    @Population BIGINT,
    @Area FLOAT,
    @FlagUrl NVARCHAR(500),
    @Borders NVARCHAR(500)
AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO CountriesTable_MD_TB2
    (
        CCA3,
        [Name] ,
        Capital,
        Region,
        SubRegion ,
        [Population],
        Area,
        FlagUrl,
        Borders 
    )
    VALUES
    (
        @CCA3,
        @Name,
        @Capital,
        @Region,
        @SubRegion,
        @Population,
        @Area,
        @FlagUrl,
        @Borders
    );

    SELECT SCOPE_IDENTITY() AS NewCountryId; --returns the last id that identity insereted in the table (last game that was inserted) for now it is not neccessary but maybe later
END
GO


CREATE PROCEDURE spReadAllCountries_MD_TB2
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM CountriesTable_MD_TB2;
END
GO


CREATE PROCEDURE spReadCountryById_MD_TB2
    @Id INT
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM CountriesTable_MD_TB2
    WHERE dbCountryId = @Id;
END
GO


CREATE PROCEDURE spReadCountryByName_MD_TB2
    @Name NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM CountriesTable_MD_TB2
    WHERE [Name]= @Name; 
END
GO



CREATE PROCEDURE spReadCountriesByRegion_MD_TB2
    @Region NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM CountriesTable_MD_TB2
    WHERE Region =@Region;
END
GO



CREATE PROCEDURE spUpdateCountry_MD_TB2

    @Id INT,
    @CCA3 NVARCHAR(4),
    @Name NVARCHAR(50),
    @Capital NVARCHAR(50),
    @Region NVARCHAR(50),
    @SubRegion NVARCHAR(50),
    @Population BIGINT,
    @Area FLOAT,
    @FlagUrl NVARCHAR(500),
    @Borders NVARCHAR(500)
AS
BEGIN
    --SET NOCOUNT ON;

    UPDATE CountriesTable_MD_TB2
    SET
        CCA3 =  @CCA3,
        [Name] = @Name,
        Capital = @Capital,
        Region = @Region,
        SubRegion = @SubRegion,
        [Population] = @Population,
        Area = @Area,
        FlagUrl = @FlagUrl,
        Borders = @Borders
        
    WHERE dbCountryId = @Id;
END
GO
      

CREATE PROCEDURE spDeleteCountry_MD_TB2
    @Id INT
AS
BEGIN
    --SET NOCOUNT ON;

    DELETE FROM CountriesTable_MD_TB2
    WHERE dbCountryId = @Id;
END
GO



--CREATE PROCEDURE spGetGamesByTags_MD_TB2
--    @Tags NVARCHAR(MAX)
--AS
--BEGIN
--    SELECT DISTINCT G.*
--    FROM GamesTable_MD_TB2 AS G
--    INNER JOIN TagGameTable_MD_TB2 AS T
--        ON G.dbGameId = T.GameId
--    INNER JOIN STRING_SPLIT(@Tags, ',') AS S
--        ON LTRIM(RTRIM(T.TagName)) = LTRIM(RTRIM(S.value))
--    WHERE LTRIM(RTRIM(S.value)) <> ''
--END
--GO


--CREATE PROCEDURE spGetTagsByGameId_MD_TB2
--    @GameId INT
--AS
--BEGIN
--    SELECT TagName
--    FROM TagGameTable_MD_TB2
--    WHERE GameId = @GameId;
--END
--GO
