CREATE TABLE Countries (
    Id INT PRIMARY KEY,
    Name VARCHAR(50)
);

CREATE TABLE Cities (
    Id INT PRIMARY KEY,
    Name VARCHAR(50),
    Latitude DECIMAL(9,4),
    Longitude DECIMAL(9,4),
    CountryId INT,
    FOREIGN KEY (CountryId) REFERENCES Countries(Id)
);

INSERT INTO Countries (Id, Name)
VALUES
(1, 'Latvia'),
(2, 'Germany'),
(3, 'Spain');

INSERT INTO Cities (Id, Name, Latitude, Longitude, CountryId)
VALUES
(101, 'Riga', 56.9496, 24.1052, 1),
(102, 'Ventspils', 57.3894, 21.5606, 1),
(201, 'Berlin', 52.5200, 13.4050, 2),
(202, 'Munich', 48.1351, 11.5820, 2),
(301, 'Madrid', 40.4168, -3.7038, 3),
(302, 'Malaga', 36.7196, -4.4200, 3);

CREATE TABLE Temperatures (
    Id              INT        IDENTITY (1, 1) NOT NULL,
    CountryId       INT        NOT NULL,
    CityId          INT        NOT NULL,
    Temperature     FLOAT (53) NULL,
    UpdateTimeStamp DATETIME   NULL,
    PRIMARY KEY CLUSTERED (Id ASC), 
    CONSTRAINT FK_Temperatures_ToCountries FOREIGN KEY (CountryId) REFERENCES Countries(Id), 
    CONSTRAINT FK_Temperatures_ToCities FOREIGN KEY (CityId) REFERENCES Cities(Id) 
);

