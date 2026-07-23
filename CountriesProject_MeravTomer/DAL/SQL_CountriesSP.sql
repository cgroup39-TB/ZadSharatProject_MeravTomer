-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Read ALL Existing Countries>
-- =============================================
CREATE PROCEDURE spReadAllCountries_3MD_TB
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries;
END
GO

-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Insert a new Country>
-- =============================================
CREATE PROCEDURE spInsertCountry_3MD_TB
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

-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Read a Country by its CountryId>
-- =============================================
CREATE PROCEDURE spReadCountryById_3MD_TB
    @Id INT
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries
    WHERE CountryId = @Id;
END
GO


CREATE PROCEDURE spReadCountryByName_3MD_TB
    @Name NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries
    WHERE [Name]= @Name; 
END
GO


-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Read all countries by a specifiic Region>
-- =============================================
CREATE PROCEDURE spReadCountriesByRegion_3MD_TB  ---ID how can i search by id its by name 
    @Region NVARCHAR(50)
AS
BEGIN
    --SET NOCOUNT ON;

    SELECT *
    FROM Countries
    WHERE Region =@Region;
END
GO



-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Update a specific country details>
-- =============================================
CREATE PROCEDURE spUpdateCountry_3MD_TB

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
      
-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Delete a specific Country>
-- =============================================
CREATE PROCEDURE spDeleteCountry_3MD_TB
    @Id INT
AS
BEGINS
    --SET NOCOUNT ON;

    DELETE FROM Countries
    WHERE CountryId = @Id;
END
GO

