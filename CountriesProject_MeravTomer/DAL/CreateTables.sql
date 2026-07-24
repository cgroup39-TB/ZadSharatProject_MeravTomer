
CREATE TABLE Regions(
    RegionId INT IDENTITY(1,1) PRIMARY KEY,
    RegionName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Countries (
    CountryId INT IDENTITY(1,1) PRIMARY KEY,
    CCA3 NVARCHAR(3) NOT NULL UNIQUE,
    [Name] NVARCHAR(50) NOT NULL,
    Capital NVARCHAR(50),
    RegionId INT,
    SubRegion NVARCHAR(50),
    [Population] BIGINT,
    Area FLOAT,
    FlagUrl NVARCHAR(500), 
    Borders NVARCHAR(500),

    CONSTRAINT FK_Countries_Regions
        FOREIGN KEY (RegionId)
        REFERENCES Regions(RegionId)
);

CREATE TABLE Currencies (

    CurrencyId INT IDENTITY(1,1) PRIMARY KEY,
    CurrencyCode NVARCHAR(3) NOT NULL UNIQUE, --- example: ILS , EUR ,USD
    [Name] NVARCHAR(50), --- Full name of the coin
    Symbol NVARCHAR(20)
);

CREATE TABLE Languages(
    LanguageId INT IDENTITY(1,1) PRIMARY KEY,
    LanguageName NVARCHAR(50) NOT NULL UNIQUE
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

CREATE TABLE CountryCurrencies (
    
    CountryId INT NOT NULL,
    CurrencyId INT NOT NULL,

    PRIMARY KEY (CountryId, CurrencyId ),
    FOREIGN KEY (CountryId) REFERENCES  Countries(CountryId) ON DELETE CASCADE,
    FOREIGN KEY (CurrencyId) REFERENCES Currencies(CurrencyId) ON DELETE CASCADE
   
);

CREATE TABLE CountryLanguages(
    CountryId INT NOT NULL,
    LanguageId INT NOT NULL,

    PRIMARY KEY (CountryId, LanguageId),

    FOREIGN KEY (CountryId)
        REFERENCES Countries(CountryId)
        ON DELETE CASCADE,

    FOREIGN KEY (LanguageId)
        REFERENCES Languages(LanguageId)
);

CREATE TABLE UserLanguages(
    UserLanguageId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    LanguageId INT NOT NULL,
    ProficiencyLevel NVARCHAR(50) NOT NULL,

    CONSTRAINT FK_UserLanguages_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,

    CONSTRAINT FK_UserLanguages_Languages
        FOREIGN KEY (LanguageId)
        REFERENCES Languages(LanguageId) ON DELETE CASCADE,

    CONSTRAINT UQ_UserLanguages
        UNIQUE (UserId, LanguageId)
);

CREATE TABLE UserRegions( ---User Preffered Regions
    UserRegionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    RegionId INT NOT NULL,

    CONSTRAINT FK_UserRegions_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,

    CONSTRAINT FK_UserRegions_Regions
        FOREIGN KEY (RegionId)
        REFERENCES Regions(RegionId) ON DELETE CASCADE,

    CONSTRAINT UQ_UserRegions
        UNIQUE (UserId, RegionId)
);

CREATE TABLE UserVisitedCountries(
    UserId INT NOT NULL,
    CountryId INT NOT NULL,
    Rating INT NULL,
    ReviewText NVARCHAR(1000) NULL,
    IsShared BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    PRIMARY KEY(UserId, CountryId),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,

    FOREIGN KEY(CountryId)
        REFERENCES Countries(CountryId) ON DELETE CASCADE,

    -- 0 = not rated yet (e.g. just marked as visited)
    CHECK (Rating IS NULL OR Rating BETWEEN 0 AND 5)
);

CREATE TABLE UserWantedCountries(
    UserId INT NOT NULL,
    CountryId INT NOT NULL,

    PRIMARY KEY(UserId, CountryId),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId) ON DELETE CASCADE,

    FOREIGN KEY(CountryId)
        REFERENCES Countries(CountryId) ON DELETE CASCADE
);

CREATE TABLE UserLogins
(
    LoginId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    LoginDate DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (UserId)
        REFERENCES Users(UserId)
        ON DELETE CASCADE
);

CREATE TABLE Quizzes(
    QuizId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    DurationSeconds INT NOT NULL DEFAULT 60
);

CREATE TABLE QuizQuestions(
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    QuizId INT NOT NULL,
    QuestionText NVARCHAR(300) NOT NULL,
    OptionA NVARCHAR(200) NOT NULL,
    OptionB NVARCHAR(200) NOT NULL,
    OptionC NVARCHAR(200) NOT NULL,
    OptionD NVARCHAR(200) NOT NULL,
    CorrectIndex TINYINT NOT NULL, -- 0-based index into OptionA..OptionD

    CONSTRAINT FK_QuizQuestions_Quizzes
        FOREIGN KEY (QuizId)
        REFERENCES Quizzes(QuizId) ON DELETE CASCADE,

    CONSTRAINT CK_QuizQuestions_CorrectIndex
        CHECK (CorrectIndex BETWEEN 0 AND 3)
);

-- =============================================
-- Seed data: two country quizzes, five questions each
-- (mirrors the client's former mock quiz catalog)
-- =============================================

INSERT INTO Quizzes (Title, Description, DurationSeconds)
VALUES
    (N'Flags & Capitals Quiz', N'Test your knowledge of capital cities and flags.', 60),
    (N'World Geography Quiz', N'Areas, populations, currencies and languages.', 60);

INSERT INTO QuizQuestions (QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectIndex)
VALUES
    (1, N'What is the capital of Japan?', N'Osaka', N'Tokyo', N'Kyoto', N'Nagoya', 1),
    (1, N'What is the capital of Australia?', N'Sydney', N'Melbourne', N'Canberra', N'Perth', 2),
    (1, N'Which country''s flag features a red maple leaf?', N'USA', N'Canada', N'Austria', N'Peru', 1),
    (1, N'What is the capital of Egypt?', N'Cairo', N'Alexandria', N'Giza', N'Luxor', 0),
    (1, N'What is the capital of Brazil?', N'Rio de Janeiro', N'São Paulo', N'Brasília', N'Salvador', 2),
    (2, N'Which is the largest country in the world by area?', N'China', N'USA', N'Russia', N'Canada', 2),
    (2, N'Which continent is Egypt located in?', N'Asia', N'Africa', N'Europe', N'South America', 1),
    (2, N'Which currency is used in Germany?', N'Franc', N'Euro', N'Mark', N'Pound', 1),
    (2, N'Which country has the largest population?', N'India', N'USA', N'Indonesia', N'China', 3),
    (2, N'What language is mainly spoken in Brazil?', N'Spanish', N'Portuguese', N'French', N'Italian', 1);