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
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Role NVARCHAR(20) NOT NULL DEFAULT 'User',
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        CreatedBy NVARCHAR(50) NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'Users table created successfully.';
END

-- Create AutomationJobs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AutomationJobs' AND xtype='U')
BEGIN
    CREATE TABLE AutomationJobs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        JobType NVARCHAR(50) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        Priority INT NOT NULL DEFAULT 5,
        ScheduledAt DATETIME2 NULL,
        StartedAt DATETIME2 NULL,
        CompletedAt DATETIME2 NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        AssignedTo NVARCHAR(50) NULL,
        Configuration NVARCHAR(MAX) NULL,
        Result NVARCHAR(MAX) NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'AutomationJobs table created successfully.';
END

-- Create TestExecutions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestExecutions' AND xtype='U')
BEGIN
    CREATE TABLE TestExecutions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TestName NVARCHAR(100) NOT NULL,
        TestType NVARCHAR(50) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        TestSuite NVARCHAR(100) NULL,
        TestData NVARCHAR(MAX) NULL,
        ExpectedResult NVARCHAR(MAX) NULL,
        ActualResult NVARCHAR(MAX) NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        ExecutionTime INT NULL,
        AiAnalysis NVARCHAR(MAX) NULL,
        ConfidenceScore DECIMAL(5,2) NULL,
        TestEnvironment NVARCHAR(50) NULL,
        Browser NVARCHAR(50) NULL,
        Device NVARCHAR(50) NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'TestExecutions table created successfully.';
END

-- Create WebAutomations table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebAutomations' AND xtype='U')
BEGIN
    CREATE TABLE WebAutomations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        AutomationName NVARCHAR(100) NOT NULL,
        WebsiteUrl NVARCHAR(500) NOT NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        AutomationType NVARCHAR(50) NOT NULL,
        TargetElement NVARCHAR(200) NULL,
        Action NVARCHAR(50) NULL,
        InputData NVARCHAR(MAX) NULL,
        OutputData NVARCHAR(MAX) NULL,
        AiPrompt NVARCHAR(MAX) NULL,
        AiResponse NVARCHAR(MAX) NULL,
        ElementSelector NVARCHAR(200) NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        ExecutionTime INT NULL,
        Browser NVARCHAR(50) NULL,
        Device NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        ViewportSize NVARCHAR(20) NULL,
        ConfidenceScore DECIMAL(5,2) NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'WebAutomations table created successfully.';
END

-- Create JobSchedules table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSchedules' AND xtype='U')
BEGIN
    CREATE TABLE JobSchedules (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        JobType NVARCHAR(50) NOT NULL,
        IsEnabled BIT NOT NULL DEFAULT 1,
        CronExpression NVARCHAR(100) NULL,
        NextRunAt DATETIME2 NULL,
        LastRunAt DATETIME2 NULL,
        LastRunStatus NVARCHAR(20) NULL,
        Configuration NVARCHAR(MAX) NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'JobSchedules table created successfully.';
END

-- Create ComputerVisionResults table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ComputerVisionResults' AND xtype='U')
BEGIN
    CREATE TABLE ComputerVisionResults (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ImagePath NVARCHAR(500) NOT NULL,
        WebsiteUrl NVARCHAR(500) NULL,
        ElementType NVARCHAR(50) NOT NULL,
        ElementDescription NVARCHAR(500) NULL,
        BoundingBox NVARCHAR(100) NULL,
        ConfidenceScore DECIMAL(5,2) NULL,
        Selector NVARCHAR(200) NULL,
        Attributes NVARCHAR(MAX) NULL,
        Text NVARCHAR(500) NULL,
        Color NVARCHAR(20) NULL,
        Size NVARCHAR(20) NULL,
        Position NVARCHAR(20) NULL,
        Visibility NVARCHAR(20) NULL,
        Accessibility NVARCHAR(MAX) NULL,
        Action NVARCHAR(50) NULL,
        InputData NVARCHAR(MAX) NULL,
        ValidationRules NVARCHAR(MAX) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Detected',
        ErrorMessage NVARCHAR(MAX) NULL,
        DetectedAt DATETIME2 NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        Tags NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'ComputerVisionResults table created successfully.';
END

-- Create ExperimentalAnalysis table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExperimentalAnalysis' AND xtype='U')
BEGIN
    CREATE TABLE ExperimentalAnalysis (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        AnalysisType NVARCHAR(50) NOT NULL,
        Configuration NVARCHAR(MAX) NULL,
        Results NVARCHAR(MAX) NOT NULL,
        PerformanceMetrics NVARCHAR(MAX) NULL,
        StatisticalData NVARCHAR(MAX) NULL,
        Recommendations NVARCHAR(MAX) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Completed',
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'ExperimentalAnalysis table created successfully.';
END

-- Create PerformanceBenchmarks table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PerformanceBenchmarks' AND xtype='U')
BEGIN
    CREATE TABLE PerformanceBenchmarks (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        BenchmarkName NVARCHAR(100) NOT NULL,
        BenchmarkType NVARCHAR(50) NOT NULL,
        Configuration NVARCHAR(MAX) NULL,
        Results NVARCHAR(MAX) NOT NULL,
        PerformanceMetrics NVARCHAR(MAX) NULL,
        StatisticalAnalysis NVARCHAR(MAX) NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Completed',
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedBy NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(50) NULL
    );
    PRINT 'PerformanceBenchmarks table created successfully.';
END

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_Status ON AutomationJobs(Status);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_CreatedBy ON AutomationJobs(CreatedBy);
CREATE NONCLUSTERED INDEX IX_TestExecutions_Status ON TestExecutions(Status);
CREATE NONCLUSTERED INDEX IX_TestExecutions_CreatedBy ON TestExecutions(CreatedBy);
CREATE NONCLUSTERED INDEX IX_WebAutomations_Status ON WebAutomations(Status);
CREATE NONCLUSTERED INDEX IX_WebAutomations_CreatedBy ON WebAutomations(CreatedBy);
CREATE NONCLUSTERED INDEX IX_JobSchedules_IsEnabled ON JobSchedules(IsEnabled);
CREATE NONCLUSTERED INDEX IX_JobSchedules_NextRunAt ON JobSchedules(NextRunAt);

-- Insert sample data
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedBy)
VALUES 
    ('admin', 'admin@bosch.com', '$2a$11$example_hash_here', 'System', 'Administrator', 'Admin', 1, 'system'),
    ('testuser', 'test@bosch.com', '$2a$11$example_hash_here', 'Test', 'User', 'User', 1, 'admin'),
    ('operator', 'operator@bosch.com', '$2a$11$example_hash_here', 'Production', 'Operator', 'Operator', 1, 'admin');

INSERT INTO AutomationJobs (Name, Description, JobType, Status, Priority, CreatedBy)
VALUES 
    ('Production Line 1', 'Automated production line monitoring', 'Production', 'Running', 1, 'admin'),
    ('Quality Check', 'Automated quality control process', 'Quality', 'Pending', 2, 'admin'),
    ('Data Processing', 'Batch data processing job', 'Data', 'Completed', 3, 'admin');

INSERT INTO TestExecutions (TestName, TestType, Status, TestSuite, CreatedBy)
VALUES 
    ('Login Test', 'Functional', 'Passed', 'Authentication Suite', 'admin'),
    ('API Test', 'Integration', 'Failed', 'API Suite', 'admin'),
    ('UI Test', 'UI', 'Passed', 'Frontend Suite', 'admin');

INSERT INTO WebAutomations (AutomationName, WebsiteUrl, Status, AutomationType, CreatedBy)
VALUES 
    ('Login Automation', 'https://example.com', 'Completed', 'Login', 'admin'),
    ('Data Extraction', 'https://data.example.com', 'Running', 'Extraction', 'admin'),
    ('Form Submission', 'https://forms.example.com', 'Pending', 'Form', 'admin');

INSERT INTO JobSchedules (Name, Description, JobType, IsEnabled, CronExpression, CreatedBy)
VALUES 
    ('Daily Report', 'Generate daily production report', 'Report', 1, '0 0 6 * * *', 'admin'),
    ('Weekly Backup', 'Weekly database backup', 'Backup', 1, '0 0 2 * * 0', 'admin'),
    ('Monthly Cleanup', 'Monthly data cleanup', 'Maintenance', 1, '0 0 1 1 * *', 'admin');

PRINT 'Sample data inserted successfully.';
PRINT 'Database initialization completed successfully.';
