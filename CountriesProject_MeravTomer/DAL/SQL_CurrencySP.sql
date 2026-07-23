-- =============================================
-- Author:      <Tomer,Merav>
-- Create date: <23.7.2026>
-- Description: <Insert a new coin(Currency)>
-- =============================================
CREATE PROCEDURE spInsertCurrency_3MD_TB
    @CurrencyCode NVARCHAR(3),
    @Name NVARCHAR(50),
    @Symbol  NVARCHAR(20)

AS
BEGIN
    --SET NOCOUNT ON;

    INSERT INTO Languages
    (
        CurrencyCode,
        [Name],
        Symbol
      
    )
    VALUES
    (
        @Code,
        @Name,
        @Symbol
    );

    SELECT SCOPE_IDENTITY() AS NewCurrencyId; --returns the last id that identity insereted in the table (last game that was inserted) for now it is not neccessary but maybe later
END
GO
