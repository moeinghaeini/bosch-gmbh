-- Create Industrial Automation Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'IndustrialAutomationDb')
BEGIN
    CREATE DATABASE IndustrialAutomationDb;
    PRINT 'Database IndustrialAutomationDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database IndustrialAutomationDb already exists.';
END

-- Use the database
USE IndustrialAutomationDb;

-- Create Users table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Role NVARCHAR(50) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'Users table created successfully.';
END

-- Create AutomationJobs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AutomationJobs' AND xtype='U')
BEGIN
    CREATE TABLE AutomationJobs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        StatusId INT NOT NULL DEFAULT 1,
        JobTypeId INT NOT NULL DEFAULT 1,
        ScheduledAt DATETIME2 NULL,
        StartedAt DATETIME2 NULL,
        CompletedAt DATETIME2 NULL,
        ErrorMessage NVARCHAR(2000) NULL,
        Configuration NVARCHAR(MAX) NULL,
        ExecutionTimeMs INT NULL,
        RetryCount INT NOT NULL DEFAULT 0,
        MaxRetries INT NOT NULL DEFAULT 3,
        Priority INT NOT NULL DEFAULT 1,
        CreatedBy NVARCHAR(100) NULL,
        AssignedTo NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'AutomationJobs table created successfully.';
END

-- Create TestExecutions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestExecutions' AND xtype='U')
BEGIN
    CREATE TABLE TestExecutions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TestName NVARCHAR(200) NOT NULL,
        TestTypeId INT NOT NULL DEFAULT 1,
        StatusId INT NOT NULL DEFAULT 1,
        TestSuite NVARCHAR(200) NOT NULL,
        TestData NVARCHAR(MAX) NULL,
        ExpectedResult NVARCHAR(MAX) NULL,
        ActualResult NVARCHAR(MAX) NULL,
        ErrorMessage NVARCHAR(2000) NULL,
        AIAnalysis NVARCHAR(MAX) NULL,
        ConfidenceScore NVARCHAR(10) NULL,
        TestEnvironment NVARCHAR(100) NULL,
        Browser NVARCHAR(50) NULL,
        Device NVARCHAR(50) NULL,
        CreatedBy NVARCHAR(100) NULL,
        ExecutedBy NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'TestExecutions table created successfully.';
END

-- Create WebAutomations table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebAutomations' AND xtype='U')
BEGIN
    CREATE TABLE WebAutomations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        AutomationName NVARCHAR(200) NOT NULL,
        WebsiteUrl NVARCHAR(500) NOT NULL,
        StatusId INT NOT NULL DEFAULT 1,
        JobTypeId INT NOT NULL DEFAULT 1,
        TargetElement NVARCHAR(500) NULL,
        Action NVARCHAR(100) NULL,
        InputData NVARCHAR(MAX) NULL,
        OutputData NVARCHAR(MAX) NULL,
        AIPrompt NVARCHAR(MAX) NULL,
        AIResponse NVARCHAR(MAX) NULL,
        ElementSelector NVARCHAR(1000) NULL,
        ErrorMessage NVARCHAR(2000) NULL,
        Browser NVARCHAR(50) NULL,
        Device NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        ViewportSize NVARCHAR(20) NULL,
        ConfidenceScore NVARCHAR(10) NULL,
        CreatedBy NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'WebAutomations table created successfully.';
END

-- Create JobSchedules table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSchedules' AND xtype='U')
BEGIN
    CREATE TABLE JobSchedules (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        JobName NVARCHAR(200) NOT NULL,
        JobTypeId INT NOT NULL DEFAULT 1,
        StatusId INT NOT NULL DEFAULT 1,
        CronExpression NVARCHAR(100) NULL,
        Configuration NVARCHAR(MAX) NULL,
        Priority NVARCHAR(20) NOT NULL DEFAULT 'Normal',
        TimeZone NVARCHAR(50) NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        NextRunTime DATETIME2 NULL,
        LastRunTime DATETIME2 NULL,
        ErrorMessage NVARCHAR(2000) NULL,
        ExecutionHistory NVARCHAR(MAX) NULL,
        Notifications NVARCHAR(MAX) NULL,
        Dependencies NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'JobSchedules table created successfully.';
END

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_StatusId ON AutomationJobs(StatusId);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_JobTypeId ON AutomationJobs(JobTypeId);
CREATE NONCLUSTERED INDEX IX_TestExecutions_StatusId ON TestExecutions(StatusId);
CREATE NONCLUSTERED INDEX IX_TestExecutions_TestTypeId ON TestExecutions(TestTypeId);
CREATE NONCLUSTERED INDEX IX_WebAutomations_StatusId ON WebAutomations(StatusId);
CREATE NONCLUSTERED INDEX IX_WebAutomations_JobTypeId ON WebAutomations(JobTypeId);
CREATE NONCLUSTERED INDEX IX_JobSchedules_StatusId ON JobSchedules(StatusId);
CREATE NONCLUSTERED INDEX IX_JobSchedules_IsEnabled ON JobSchedules(IsEnabled);

-- Insert sample data
INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive)
VALUES 
    ('admin', 'admin@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'Admin', 1),
    ('testuser', 'test@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'User', 1),
    ('operator', 'operator@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'Operator', 1);

INSERT INTO AutomationJobs (Name, Description, StatusId, JobTypeId, Priority, CreatedBy)
VALUES 
    ('Production Line 1', 'Automated production line monitoring', 1, 1, 1, 'admin'),
    ('Quality Check', 'Automated quality control process', 1, 2, 2, 'admin'),
    ('Data Processing', 'Batch data processing job', 3, 3, 3, 'admin');

INSERT INTO TestExecutions (TestName, TestTypeId, StatusId, TestSuite, CreatedBy)
VALUES 
    ('Login Test', 1, 3, 'Authentication Suite', 'admin'),
    ('API Test', 2, 2, 'API Suite', 'admin'),
    ('UI Test', 3, 3, 'Frontend Suite', 'admin');

INSERT INTO WebAutomations (AutomationName, WebsiteUrl, StatusId, JobTypeId, CreatedBy)
VALUES 
    ('Login Automation', 'https://example.com', 3, 1, 'admin'),
    ('Data Extraction', 'https://data.example.com', 1, 2, 'admin'),
    ('Form Submission', 'https://forms.example.com', 1, 3, 'admin');

INSERT INTO JobSchedules (JobName, JobTypeId, StatusId, IsEnabled, CronExpression)
VALUES 
    ('Daily Report', 1, 1, 1, '0 0 6 * * *'),
    ('Weekly Backup', 2, 1, 1, '0 0 2 * * 0'),
    ('Monthly Cleanup', 3, 1, 1, '0 0 1 1 * *');

PRINT 'Sample data inserted successfully.';
PRINT 'Database initialization completed successfully.';
