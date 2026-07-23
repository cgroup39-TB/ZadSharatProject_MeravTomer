CREATE PROCEDURE spReadAllCountries_MD_TB2
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries;
END
GO

CREATE PROCEDURE spInsertCountry_MD_TB2
    @CCA3 NVARCHAR(3),
    @Name NVARCHAR(50),
    @Capital NVARCHAR(50),
    @RegionName NVARCHAR(50),
    @SubRegion NVARCHAR(50),
    @Population BIGINT,
    @Area DECIMAL(18,2),
    @FlagUrl NVARCHAR(500),
    @Borders NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RegionId INT;

    SELECT @RegionId = RegionId
    FROM Regions
    WHERE [Name] = @RegionName;

    IF @RegionId IS NULL
    BEGIN
        RAISERROR('Region does not exist', 16, 1);
        RETURN;
    END;

    INSERT INTO Countries
    (
        CCA3,
        [Name],
        Capital,
        RegionId,
        SubRegion,
        Population,
        Area,
        FlagUrl,
        Borders
    )
    VALUES
    (
        @CCA3,
        @Name,
        @Capital,
        @RegionId,
        @SubRegion,
        @Population,
        @Area,
        @FlagUrl,
        @Borders
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewCountryId;
END
GO


CREATE PROCEDURE spReadCountryById
    @Id INT
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries
    WHERE CountryId = @Id;
END
GO


CREATE PROCEDURE spReadCountryByName
    @Name NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries
    WHERE [Name]= @Name; 
END
GO



CREATE PROCEDURE spReadCountriesByRegion  ---ID how can i search by id its by name 
    @Region NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM CountriesTable_MD_TB2
    WHERE Region =@Region;
END
GO



CREATE PROCEDURE spUpdateCountry  --- same how can i update id by name?

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

    UPDATE Countries
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
        
    WHERE CountryId = @Id;
END
GO
      

CREATE PROCEDURE spDeleteCountry
    @Id INT
AS
BEGIN
    --SET NOCOUNT ON;

    DELETE FROM Countries
    WHERE CountryId = @Id;
END
GO

