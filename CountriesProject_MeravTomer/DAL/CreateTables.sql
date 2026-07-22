CREATE TABLE Currencies (

    CurrencyId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(20),
    Symbol NVARCHAR(20)
);

CREATE TABLE CountriesTable_MD_TB2 (
    CountryId INT IDENTITY(1,1) PRIMARY KEY,
    CCA3 NVARCHAR(4) NOT NULL UNIQUE,
    [Name] NVARCHAR(50) NOT NULL,
    Capital NVARCHAR(50),
    Region NVARCHAR(50),
    SubRegion NVARCHAR(50),
    [Population] BIGINT,
    Area FLOAT,
    FlagUrl NVARCHAR(500), 
    Borders NVARCHAR(500)
);

CREATE TABLE LanguagesTable_MD_TB2 (

    LanguageId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(20)

);

   
);

CREATE TABLE CountryCurrencies (
    
    CountryId INT NOT NULL,
    CurrencyId INT NOT NULL,

    PRIMARY KEY (CountryId, CurrencyId ),
    FOREIGN KEY (CountryId) REFERENCES  Countries(CountryId) ON DELETE CASCADE,
    FOREIGN KEY (CurrencyId) REFERENCES Currencies(CurrencyId) ON DELETE CASCADE
   
);

CREATE TABLE Users(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(30) NOT NULL,
    Email NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    IsAdmin BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CanShare BIT NOT NULL DEFAULT 1
    
);

CREATE TABLE Regions(
    RegionId INT IDENTITY(1,1) PRIMARY KEY,
    RegionName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Languages(
    LanguageId INT IDENTITY(1,1) PRIMARY KEY,
    LanguageName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE UserLanguages(
    UserLanguageId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    LanguageId INT NOT NULL,
    ProficiencyLevel NVARCHAR(50) NOT NULL,

    CONSTRAINT FK_UserLanguages_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT FK_UserLanguages_Languages
        FOREIGN KEY (LanguageId)
        REFERENCES Languages(LanguageId),

    CONSTRAINT UQ_UserLanguages
        UNIQUE (UserId, LanguageId)
);

CREATE TABLE UserRegions(
    UserRegionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    RegionId INT NOT NULL,

    CONSTRAINT FK_UserRegions_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT FK_UserRegions_Regions
        FOREIGN KEY (RegionId)
        REFERENCES Regions(RegionId),

    CONSTRAINT UQ_UserRegions
        UNIQUE (UserId, RegionId)
);

CREATE TABLE UserVisitedCountries(
    UserId INT NOT NULL,
    CountryId INT NOT NULL,
    Rating INT NULL,
    ReviewText NVARCHAR(1000) NULL,
    IsAdmin BIT NOT NULL DEFAULT 0

    PRIMARY KEY(UserId, CountryId),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId),

    FOREIGN KEY(CountryId)
        REFERENCES Countries(CountryId),

    CHECK (Rating IS NULL OR Rating BETWEEN 1 AND 5)
);

CREATE TABLE UserWantedCountries(
    UserId INT NOT NULL,
    CountryId INT NOT NULL,

    PRIMARY KEY(UserId, CountryId),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId),

    FOREIGN KEY(CountryId)
        REFERENCES Countries(CountryId)
);

