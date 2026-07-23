CREATE PROCEDURE spReadAllRegions
AS
BEGIN
    SELECT
        RegionId,
        RegionName
    FROM Regions;
END
GO


CREATE PROCEDURE spReadRegionById
    @RegionId INT
AS
BEGIN
    SELECT
        RegionId,
        RegionName
    FROM Regions
    WHERE RegionId = @RegionId;
END
GO


CREATE PROCEDURE spReadRegionByName
    @RegionName NVARCHAR(50)
AS
BEGIN
    SELECT
        RegionId,
        RegionName
    FROM Regions
    WHERE RegionName = @RegionName;
END
GO


CREATE PROCEDURE spInsertRegion
    @RegionName NVARCHAR(50)
AS
BEGIN
    INSERT INTO Regions
    (
        RegionName
    )
    VALUES
    (
        @RegionName
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO