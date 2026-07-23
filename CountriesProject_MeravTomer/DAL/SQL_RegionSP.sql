-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read All Regions>
-- =============================================
CREATE PROCEDURE spReadAllRegions_3MD_TB
AS
BEGIN

    SELECT
        RegionId,
        RegionName
    FROM Regions;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Region By Id>
-- =============================================
CREATE PROCEDURE spReadRegionById_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Read Region By Name>
-- =============================================
CREATE PROCEDURE spReadRegionByName_3MD_TB
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


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <23.7.26>
-- Description:	<Insert Region>
-- =============================================
CREATE PROCEDURE spInsertRegion_3MD_TB
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