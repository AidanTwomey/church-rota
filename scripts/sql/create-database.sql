-- Create the ChurchRota database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ChurchRota')
BEGIN
    CREATE DATABASE ChurchRota;
END
GO

USE ChurchRota;
GO

-- Add your schema creation here
-- Example tables (you can modify these based on your needs):

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'People')
BEGIN
    CREATE TABLE People (
        PersonId INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255),
        Phone NVARCHAR(20),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        RoleId INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Schedules')
BEGIN
    CREATE TABLE Schedules (
        ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
        PersonId INT NOT NULL,
        RoleId INT NOT NULL,
        ServiceDate DATE NOT NULL,
        Notes NVARCHAR(500),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (PersonId) REFERENCES People(PersonId),
        FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
    );
END

-- Create indexes
CREATE INDEX IX_Schedules_ServiceDate ON Schedules(ServiceDate);
CREATE INDEX IX_People_LastName ON People(LastName);
GO