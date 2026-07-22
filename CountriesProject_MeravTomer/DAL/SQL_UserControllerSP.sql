CREATE PROCEDURE sp_User_Insert
    @Name NVARCHAR(30),
    @Email NVARCHAR(50),
    @Password NVARCHAR(255),
    @IsAdmin BIT,
    @IsActive BIT,
    @CanShare BIT
AS
BEGIN
    INSERT INTO Users (Name, Email, Password, IsAdmin, IsActive, CanShare)
    VALUES (@Name, @Email, @Password, @IsAdmin, @IsActive, @CanShare);

    SELECT SCOPE_IDENTITY() AS NewUserId;
END
GO

CREATE PROCEDURE sp_User_GetById
    @UserId INT
AS
BEGIN
    SELECT UserId, Name, Email, Password, IsAdmin, IsActive, CanShare
    FROM Users
    WHERE UserId = @UserId;
END
GO

CREATE PROCEDURE sp_User_GetByEmail
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT UserId, Name, Email, Password, IsAdmin, IsActive, CanShare
    FROM Users
    WHERE Email = @Email;
END
GO

CREATE PROCEDURE sp_User_UpdateProfile
    @UserId INT,
    @Name NVARCHAR(30),
    @Email NVARCHAR(50)
AS
BEGIN
    UPDATE Users
    SET Name = @Name,
        Email = @Email
    WHERE UserId = @UserId;
END
GO

CREATE PROCEDURE sp_User_UpdatePassword
    @UserId INT,
    @Password NVARCHAR(255)
AS
BEGIN
    UPDATE Users
    SET Password = @Password
    WHERE UserId = @UserId;
END
GO

CREATE PROCEDURE sp_UserLanguages_GetByUserId
    @UserId INT
AS
BEGIN
    SELECT L.LanguageId, L.LanguageName, UL.ProficiencyLevel
    FROM UserLanguages AS UL
    INNER JOIN Languages AS L ON UL.LanguageId = L.LanguageId
    WHERE UL.UserId = @UserId;
END
GO

CREATE PROCEDURE sp_UserLanguages_DeleteByUserId
    @UserId INT
AS
BEGIN
    DELETE FROM UserLanguages
    WHERE UserId = @UserId;
END
GO

CREATE PROCEDURE sp_UserLanguages_Insert
    @UserId INT,
    @LanguageId INT,
    @ProficiencyLevel NVARCHAR(50)
AS
BEGIN
    INSERT INTO UserLanguages (UserId, LanguageId, ProficiencyLevel)
    VALUES (@UserId, @LanguageId, @ProficiencyLevel);
END
GO

CREATE PROCEDURE sp_UserRegions_GetByUserId
    @UserId INT
AS
BEGIN
    SELECT R.RegionId, R.RegionName
    FROM UserRegions AS UR
    INNER JOIN Regions AS R ON UR.RegionId = R.RegionId
    WHERE UR.UserId = @UserId;
END
GO

CREATE PROCEDURE sp_UserRegions_DeleteByUserId
    @UserId INT
AS
BEGIN
    DELETE FROM UserRegions
    WHERE UserId = @UserId;
END
GO

CREATE PROCEDURE sp_UserRegions_Insert
    @UserId INT,
    @RegionId INT
AS
BEGIN
    INSERT INTO UserRegions (UserId, RegionId)
    VALUES (@UserId, @RegionId);
END
GO
