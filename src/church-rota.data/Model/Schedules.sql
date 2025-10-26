CREATE TABLE [dbo].[Schedules]
(
    [ScheduleId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PersonId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    [ServiceDate] DATE NOT NULL,
    [Notes] NVARCHAR(500) NULL,
    [CreatedDate] DATETIME2 NOT NULL CONSTRAINT DF_Schedules_CreatedDate DEFAULT (SYSUTCDATETIME()),
    [ModifiedDate] DATETIME2 NOT NULL CONSTRAINT DF_Schedules_ModifiedDate DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Schedules_People FOREIGN KEY (PersonId) REFERENCES dbo.People(PersonId),
    CONSTRAINT FK_Schedules_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);

-- CREATE INDEX IX_Schedules_ServiceDate ON dbo.Schedules(ServiceDate);
-- CREATE INDEX IX_People_LastName ON dbo.People(LastName);
