CREATE TABLE CountriesTable_MD_TB2 (
    dbCountryId INT IDENTITY(1,1) PRIMARY KEY,
    CCA3 NVARCHAR(4) NOT NULL UNIQUE,
    [Name] NVARCHAR(50) NOT NULL,
    OfficialName NVARCHAR(500),
   -- Capital NVARCHAR(500),--איך שומרים רשימה??
    Region NVARCHAR(50),
    SubRegion NVARCHAR(20),
    [Population] BIGINT,
    Area FLOAT,
    Latitude FLOAT,
    Longitude FLOAT,
    FlagUrl NVARCHAR(500)
);

CREATE TABLE LanguagesTable_MD_TB2 (

    dbLanguageId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(20),
    Code NVARCHAR(50) NOT NULL,
   
);

CREATE TABLE CountryLanguagesTable_MD_TB2 (

    CountryId INT NOT NULL,
    LanguageId INT NOT NULL,
   
    PRIMARY KEY (CountryId, LanguageId),
    FOREIGN KEY (CountryId) REFERENCES CountriesTable_MD_TB2(dbCountryId) ON DELETE CASCADE,
    FOREIGN KEY (LanguageId) REFERENCES LanguagesTable_MD_TB2(dbLanguageId) ON DELETE CASCADE
);


CREATE TABLE CurrenciesTable_MD_TB2 (

    dbCurrencyId INT IDENTITY(1,1) PRIMARY KEY,
    Code INT NOT NULL,
    [Name] NVARCHAR(20),
    Symbol NVARCHAR(20),
   
);

CREATE TABLE CountryCurrenciesTable_MD_TB2 (
    
    CountryId INT NOT NULL,
    CurrencyId INT NOT NULL,

    PRIMARY KEY (CountryId, CurrencyId ),
    FOREIGN KEY (CountryId) REFERENCES  CountriesTable_MD_TB2(dbCountryId) ON DELETE CASCADE,
    FOREIGN KEY (CurrencyId) REFERENCES CurrenciesTable_MD_TB2(dbCurrencyId) ON DELETE CASCADE
   
);


CREATE TABLE BordersTable_MD_TB2 (


    CountryId INT NOT NULL,
    NeighborCountryId INT NOT NULL,

    PRIMARY KEY (CountryId, NeighborCountryId ),
    FOREIGN KEY (CountryId) REFERENCES  CountriesTable_MD_TB2(dbCountryId) ON DELETE CASCADE,
    FOREIGN KEY (NeighborCountryId) REFERENCES CurrenciesTable_MD_TB2(dbCurrencyId) ON DELETE CASCADE
   
);



CREATE TABLE UsersTable_MD_TB2(
    dbUserId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    IsAdmin BIT NOT NULL DEFAULT 0
);

--Users visit Countries--
CREATE TABLE VisitsTable_MD_TB2 (

    VisitId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CountryId INT NOT NULL,
    VisitDate DATE NOT NULL,
    ReturnDate DATE,
    Rating TINYINT CHECK (Rating BETWEEN 1 AND 5),
    Notes NVARCHAR(1000),
    IsFavorite BIT DEFAULT 0,

    CONSTRAINT FK_Visits_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Visits_Countries FOREIGN KEY (CountryId) REFERENCES Countries(CountryId)
);

CREATE TABLE TagGameTable_MD_TB2 (
    GameId INT NOT NULL,
    TagName VARCHAR(100) NOT NULL,
    PRIMARY KEY (GameId, TagName),
    FOREIGN KEY (GameId) REFERENCES GamesTable_MD_TB2(dbGameId) ON DELETE CASCADE
);