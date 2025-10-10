CREATE DATABASE StacktimDb;
GO
USE StacktimDb;
GO

CREATE TABLE Players (
    Id INT IDENTITY PRIMARY KEY,
    Pseudo NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Rank NVARCHAR(20) NOT NULL,
    TotalScore INT NOT NULL DEFAULT 0,
    CONSTRAINT CK_Players_TotalScore CHECK (TotalScore >= 0),
    CONSTRAINT CK_Players_Rank CHECK (Rank IN ('Fer', 'Bronze', 'Argent', 'Or', 'Platine', 'Diamant')),
    CONSTRAINT UQ_Players_Pseudo UNIQUE (Pseudo),
    CONSTRAINT UQ_Players_Email UNIQUE (Email)
);
GO

CREATE TABLE Teams (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Tag CHAR(3) NOT NULL,
    CONSTRAINT CK_Teams_Tag CHECK (Tag LIKE '[A-Z][A-Z][A-Z]'),
    CONSTRAINT UQ_Teams_Tag UNIQUE (Tag)
);
GO

CREATE TABLE TeamPlayers (
    TeamId INT NOT NULL,
    PlayerId INT NOT NULL,
    Role INT NOT NULL,
    CONSTRAINT PK_TeamPlayers PRIMARY KEY (TeamId, PlayerId),
    CONSTRAINT FK_TeamPlayers_Teams FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TeamPlayers_Players FOREIGN KEY (PlayerId) REFERENCES Players(Id) ON DELETE CASCADE,
    CONSTRAINT CK_TeamPlayers_Role CHECK (Role IN (0, 1, 2))
);
GO
