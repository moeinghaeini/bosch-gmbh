-- Initialize BoschThesis Database Schema
USE IndustrialAutomationDb;

-- Create Users table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Username nvarchar(100) NOT NULL UNIQUE,
        Email nvarchar(200) NOT NULL UNIQUE,
        PasswordHash nvarchar(500) NOT NULL,
        Role nvarchar(50) NOT NULL,
        IsActive bit NOT NULL DEFAULT 1,
        LastLoginAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END
ELSE
BEGIN
    -- Add LastLoginAt column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'LastLoginAt')
    BEGIN
        ALTER TABLE Users ADD LastLoginAt datetime2 NULL;
    END
END

-- Create AutomationJobs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AutomationJobs' AND xtype='U')
BEGIN
    CREATE TABLE AutomationJobs (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(1000) NULL,
        Status nvarchar(50) NOT NULL,
        JobType nvarchar(100) NOT NULL,
        Configuration nvarchar(max) NULL,
        ErrorMessage nvarchar(2000) NULL,
        ScheduledAt datetime2 NULL,
        StartedAt datetime2 NULL,
        CompletedAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END
ELSE
BEGIN
    -- Add missing columns if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AutomationJobs') AND name = 'ScheduledAt')
    BEGIN
        ALTER TABLE AutomationJobs ADD ScheduledAt datetime2 NULL;
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AutomationJobs') AND name = 'StartedAt')
    BEGIN
        ALTER TABLE AutomationJobs ADD StartedAt datetime2 NULL;
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AutomationJobs') AND name = 'CompletedAt')
    BEGIN
        ALTER TABLE AutomationJobs ADD CompletedAt datetime2 NULL;
    END
END

-- Create TestExecutions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestExecutions' AND xtype='U')
BEGIN
    CREATE TABLE TestExecutions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        TestName nvarchar(200) NOT NULL,
        TestType nvarchar(50) NOT NULL,
        Status nvarchar(50) NOT NULL,
        TestSuite nvarchar(200) NOT NULL,
        TestData nvarchar(max) NULL,
        ExpectedResult nvarchar(max) NULL,
        ActualResult nvarchar(max) NULL,
        ErrorMessage nvarchar(2000) NULL,
        AIAnalysis nvarchar(max) NULL,
        ConfidenceScore nvarchar(10) NULL,
        TestEnvironment nvarchar(100) NULL,
        Browser nvarchar(50) NULL,
        Device nvarchar(50) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create WebAutomations table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebAutomations' AND xtype='U')
BEGIN
    CREATE TABLE WebAutomations (
        Id int IDENTITY(1,1) PRIMARY KEY,
        AutomationName nvarchar(200) NOT NULL,
        WebsiteUrl nvarchar(500) NOT NULL,
        Status nvarchar(50) NOT NULL,
        AutomationType nvarchar(100) NOT NULL,
        TargetElement nvarchar(500) NULL,
        Action nvarchar(100) NULL,
        InputData nvarchar(max) NULL,
        OutputData nvarchar(max) NULL,
        AIPrompt nvarchar(max) NULL,
        AIResponse nvarchar(max) NULL,
        ElementSelector nvarchar(1000) NULL,
        ErrorMessage nvarchar(2000) NULL,
        Browser nvarchar(50) NULL,
        Device nvarchar(50) NULL,
        UserAgent nvarchar(500) NULL,
        ViewportSize nvarchar(20) NULL,
        ConfidenceScore nvarchar(10) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create JobSchedules table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSchedules' AND xtype='U')
BEGIN
    CREATE TABLE JobSchedules (
        Id int IDENTITY(1,1) PRIMARY KEY,
        JobName nvarchar(200) NOT NULL,
        JobType nvarchar(100) NOT NULL,
        Status nvarchar(50) NOT NULL,
        CronExpression nvarchar(100) NULL,
        Configuration nvarchar(max) NULL,
        Priority nvarchar(20) NOT NULL,
        TimeZone nvarchar(50) NULL,
        IsEnabled bit NOT NULL DEFAULT 1,
        NextRunTime datetime2 NULL,
        LastRunTime datetime2 NULL,
        ErrorMessage nvarchar(2000) NULL,
        ExecutionHistory nvarchar(max) NULL,
        Notifications nvarchar(max) NULL,
        Dependencies nvarchar(max) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create UserRoles table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
BEGIN
    CREATE TABLE UserRoles (
        Id int IDENTITY(1,1) PRIMARY KEY,
        RoleName nvarchar(100) NOT NULL,
        Description nvarchar(500) NULL,
        Permissions nvarchar(max) NULL,
        IsActive bit NOT NULL DEFAULT 1,
        Priority int NOT NULL DEFAULT 0,
        AllowedModules nvarchar(max) NULL,
        AllowedActions nvarchar(max) NULL,
        Restrictions nvarchar(max) NULL,
        LastModified datetime2 NOT NULL,
        ModifiedBy nvarchar(100) NOT NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create UserPermissions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserPermissions' AND xtype='U')
BEGIN
    CREATE TABLE UserPermissions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UserId int NOT NULL,
        RoleId int NOT NULL,
        PermissionType nvarchar(50) NOT NULL,
        Resource nvarchar(100) NOT NULL,
        IsGranted bit NOT NULL DEFAULT 1,
        GrantedAt datetime2 NOT NULL,
        GrantedBy nvarchar(100) NOT NULL,
        ExpiresAt datetime2 NULL,
        Notes nvarchar(500) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create MLModels table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MLModels' AND xtype='U')
BEGIN
    CREATE TABLE MLModels (
        Id int IDENTITY(1,1) PRIMARY KEY,
        ModelName nvarchar(200) NOT NULL,
        ModelType nvarchar(100) NOT NULL,
        Version nvarchar(50) NOT NULL,
        Status nvarchar(50) NOT NULL,
        Accuracy decimal(5,2) NULL,
        TrainingData nvarchar(max) NULL,
        ModelPath nvarchar(500) NULL,
        Configuration nvarchar(max) NULL,
        PerformanceMetrics nvarchar(max) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create AITrainingData table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AITrainingData' AND xtype='U')
BEGIN
    CREATE TABLE AITrainingData (
        Id int IDENTITY(1,1) PRIMARY KEY,
        DataType nvarchar(100) NOT NULL,
        Content nvarchar(max) NOT NULL,
        Label nvarchar(200) NULL,
        Category nvarchar(100) NULL,
        QualityScore decimal(3,2) NULL,
        Source nvarchar(200) NULL,
        ProcessedAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Insert sample data
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive, CreatedAt)
    VALUES ('admin', 'admin@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'Admin', 1, GETUTCDATE());
END

-- Insert sample automation jobs
IF NOT EXISTS (SELECT * FROM AutomationJobs WHERE Name = 'Sample Automation Job')
BEGIN
    INSERT INTO AutomationJobs (Name, Description, Status, JobType, Configuration, CreatedAt)
    VALUES 
    ('Sample Automation Job', 'A sample automation job for testing', 'Pending', 'WebAutomation', '{"browser": "Chrome", "timeout": 30}', GETUTCDATE()),
    ('Data Processing Job', 'Process customer data files', 'Running', 'DataProcessing', '{"inputPath": "/data/input", "outputPath": "/data/output"}', GETUTCDATE()),
    ('Report Generation', 'Generate monthly reports', 'Completed', 'ReportGeneration', '{"template": "monthly", "format": "PDF"}', GETUTCDATE());
END

-- Insert sample test executions
IF NOT EXISTS (SELECT * FROM TestExecutions WHERE TestName = 'Login Test')
BEGIN
    INSERT INTO TestExecutions (TestName, TestType, Status, TestSuite, TestData, ExpectedResult, TestEnvironment, Browser, Device, CreatedAt)
    VALUES 
    ('Login Test', 'Functional', 'Passed', 'Authentication Suite', '{"username": "testuser", "password": "testpass"}', 'User successfully logged in', 'Test', 'Chrome', 'Desktop', GETUTCDATE()),
    ('API Test', 'Integration', 'Failed', 'API Suite', '{"endpoint": "/api/users", "method": "GET"}', 'Status 200', 'Test', 'Chrome', 'Desktop', GETUTCDATE()),
    ('UI Test', 'UI', 'Passed', 'UI Suite', '{"page": "dashboard", "element": "header"}', 'Header element visible', 'Test', 'Chrome', 'Desktop', GETUTCDATE());
END

-- Insert sample web automations
IF NOT EXISTS (SELECT * FROM WebAutomations WHERE AutomationName = 'Login Automation')
BEGIN
    INSERT INTO WebAutomations (AutomationName, WebsiteUrl, Status, AutomationType, TargetElement, Action, InputData, Browser, Device, CreatedAt)
    VALUES 
    ('Login Automation', 'https://example.com', 'Completed', 'Login', 'input[type="email"]', 'Fill', '{"email": "test@example.com"}', 'Chrome', 'Desktop', GETUTCDATE()),
    ('Data Extraction', 'https://data.example.com', 'Running', 'DataExtraction', '.data-table', 'Extract', '{"format": "JSON"}', 'Chrome', 'Desktop', GETUTCDATE()),
    ('Form Submission', 'https://forms.example.com', 'Pending', 'FormSubmission', 'form', 'Submit', '{"name": "John Doe", "email": "john@example.com"}', 'Chrome', 'Desktop', GETUTCDATE());
END

-- Insert sample job schedules
IF NOT EXISTS (SELECT * FROM JobSchedules WHERE JobName = 'Daily Report')
BEGIN
    INSERT INTO JobSchedules (JobName, JobType, Status, CronExpression, Priority, IsEnabled, NextRunTime, CreatedAt)
    VALUES 
    ('Daily Report', 'ReportGeneration', 'Active', '0 9 * * *', 'High', 1, DATEADD(day, 1, GETUTCDATE()), GETUTCDATE()),
    ('Weekly Cleanup', 'Maintenance', 'Active', '0 2 * * 0', 'Medium', 1, DATEADD(week, 1, GETUTCDATE()), GETUTCDATE()),
    ('Monthly Backup', 'Backup', 'Active', '0 1 1 * *', 'High', 1, DATEADD(month, 1, GETUTCDATE()), GETUTCDATE());
END

PRINT 'Database initialization completed successfully!';
