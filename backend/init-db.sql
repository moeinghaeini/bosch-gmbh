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

-- Create StatusTypes table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StatusTypes' AND xtype='U')
BEGIN
    CREATE TABLE StatusTypes (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(50) NOT NULL UNIQUE,
        Description nvarchar(200) NULL,
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create JobTypes table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobTypes' AND xtype='U')
BEGIN
    CREATE TABLE JobTypes (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(100) NOT NULL UNIQUE,
        Description nvarchar(200) NULL,
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create AutomationJobs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AutomationJobs' AND xtype='U')
BEGIN
    CREATE TABLE AutomationJobs (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(1000) NULL,
        StatusId int NOT NULL,
        JobTypeId int NOT NULL,
        Configuration nvarchar(max) NULL,
        ErrorMessage nvarchar(2000) NULL,
        ExecutionTimeMs int NULL,
        RetryCount int NOT NULL DEFAULT 0,
        MaxRetries int NOT NULL DEFAULT 3,
        Priority int NOT NULL DEFAULT 0,
        CreatedBy int NULL,
        AssignedTo int NULL,
        ScheduledAt datetime2 NULL,
        StartedAt datetime2 NULL,
        CompletedAt datetime2 NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL,
        FOREIGN KEY (StatusId) REFERENCES StatusTypes(Id),
        FOREIGN KEY (JobTypeId) REFERENCES JobTypes(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
        FOREIGN KEY (AssignedTo) REFERENCES Users(Id)
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

-- Create TestTypes table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestTypes' AND xtype='U')
BEGIN
    CREATE TABLE TestTypes (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(50) NOT NULL UNIQUE,
        Description nvarchar(200) NULL,
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL
    );
END

-- Create TestExecutions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestExecutions' AND xtype='U')
BEGIN
    CREATE TABLE TestExecutions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        TestName nvarchar(200) NOT NULL,
        TestTypeId int NOT NULL,
        StatusId int NOT NULL,
        TestSuite nvarchar(200) NOT NULL,
        TestData nvarchar(max) NULL,
        ExpectedResult nvarchar(max) NULL,
        ActualResult nvarchar(max) NULL,
        ErrorMessage nvarchar(2000) NULL,
        AIAnalysis nvarchar(max) NULL,
        ConfidenceScore nvarchar(10) NULL,
        ExecutionTimeMs int NULL,
        CreatedBy int NULL,
        ExecutedBy int NULL,
        TestEnvironment nvarchar(100) NULL,
        Browser nvarchar(50) NULL,
        Device nvarchar(50) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL,
        FOREIGN KEY (TestTypeId) REFERENCES TestTypes(Id),
        FOREIGN KEY (StatusId) REFERENCES StatusTypes(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
        FOREIGN KEY (ExecutedBy) REFERENCES Users(Id)
    );
END

-- Create WebAutomations table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebAutomations' AND xtype='U')
BEGIN
    CREATE TABLE WebAutomations (
        Id int IDENTITY(1,1) PRIMARY KEY,
        AutomationName nvarchar(200) NOT NULL,
        WebsiteUrl nvarchar(500) NOT NULL,
        StatusId int NOT NULL,
        JobTypeId int NOT NULL,
        TargetElement nvarchar(500) NULL,
        Action nvarchar(100) NULL,
        InputData nvarchar(max) NULL,
        OutputData nvarchar(max) NULL,
        AIPrompt nvarchar(max) NULL,
        AIResponse nvarchar(max) NULL,
        ElementSelector nvarchar(1000) NULL,
        ErrorMessage nvarchar(2000) NULL,
        ExecutionTimeMs int NULL,
        CreatedBy int NULL,
        Browser nvarchar(50) NULL,
        Device nvarchar(50) NULL,
        UserAgent nvarchar(500) NULL,
        ViewportSize nvarchar(20) NULL,
        ConfidenceScore nvarchar(10) NULL,
        CreatedAt datetime2 NOT NULL,
        UpdatedAt datetime2 NULL,
        FOREIGN KEY (StatusId) REFERENCES StatusTypes(Id),
        FOREIGN KEY (JobTypeId) REFERENCES JobTypes(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );
END

-- Create JobSchedules table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSchedules' AND xtype='U')
BEGIN
    CREATE TABLE JobSchedules (
        Id int IDENTITY(1,1) PRIMARY KEY,
        JobName nvarchar(200) NOT NULL,
        JobTypeId int NOT NULL,
        StatusId int NOT NULL,
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
        UpdatedAt datetime2 NULL,
        FOREIGN KEY (JobTypeId) REFERENCES JobTypes(Id),
        FOREIGN KEY (StatusId) REFERENCES StatusTypes(Id)
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

-- Insert lookup data
-- StatusTypes
IF NOT EXISTS (SELECT * FROM StatusTypes WHERE Name = 'Pending')
BEGIN
    INSERT INTO StatusTypes (Name, Description, IsActive, CreatedAt)
    VALUES 
    ('Pending', 'Job is waiting to be executed', 1, GETUTCDATE()),
    ('Running', 'Job is currently executing', 1, GETUTCDATE()),
    ('Completed', 'Job has finished successfully', 1, GETUTCDATE()),
    ('Failed', 'Job has failed with errors', 1, GETUTCDATE()),
    ('Cancelled', 'Job was cancelled', 1, GETUTCDATE());
END

-- JobTypes
IF NOT EXISTS (SELECT * FROM JobTypes WHERE Name = 'WebAutomation')
BEGIN
    INSERT INTO JobTypes (Name, Description, IsActive, CreatedAt)
    VALUES 
    ('WebAutomation', 'Web browser automation tasks', 1, GETUTCDATE()),
    ('APITest', 'API testing and validation', 1, GETUTCDATE()),
    ('DataProcessing', 'Data processing and analysis', 1, GETUTCDATE()),
    ('ReportGeneration', 'Automated report generation', 1, GETUTCDATE()),
    ('SystemMaintenance', 'System maintenance tasks', 1, GETUTCDATE());
END

-- TestTypes
IF NOT EXISTS (SELECT * FROM TestTypes WHERE Name = 'Functional')
BEGIN
    INSERT INTO TestTypes (Name, Description, IsActive, CreatedAt)
    VALUES 
    ('Functional', 'Functional testing', 1, GETUTCDATE()),
    ('Integration', 'Integration testing', 1, GETUTCDATE()),
    ('Performance', 'Performance testing', 1, GETUTCDATE()),
    ('Security', 'Security testing', 1, GETUTCDATE()),
    ('Regression', 'Regression testing', 1, GETUTCDATE());
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
    INSERT INTO AutomationJobs (Name, Description, StatusId, JobTypeId, Configuration, CreatedAt)
    VALUES 
    ('Sample Automation Job', 'A sample automation job for testing', 1, 1, '{"browser": "Chrome", "timeout": 30}', GETUTCDATE()),
    ('Data Processing Job', 'Process customer data files', 2, 3, '{"inputPath": "/data/input", "outputPath": "/data/output"}', GETUTCDATE()),
    ('Report Generation', 'Generate monthly reports', 3, 4, '{"template": "monthly", "format": "PDF"}', GETUTCDATE());
END

-- Insert sample test executions
IF NOT EXISTS (SELECT * FROM TestExecutions WHERE TestName = 'Login Test')
BEGIN
    INSERT INTO TestExecutions (TestName, TestTypeId, StatusId, TestSuite, TestData, ExpectedResult, TestEnvironment, Browser, Device, CreatedAt)
    VALUES 
    ('Login Test', 1, 3, 'Authentication Suite', '{"username": "testuser", "password": "testpass"}', 'User successfully logged in', 'Test', 'Chrome', 'Desktop', GETUTCDATE()),
    ('API Test', 2, 4, 'API Suite', '{"endpoint": "/api/users", "method": "GET"}', 'Status 200', 'Test', 'Chrome', 'Desktop', GETUTCDATE()),
    ('UI Test', 1, 3, 'UI Suite', '{"page": "dashboard", "element": "header"}', 'Header element visible', 'Test', 'Chrome', 'Desktop', GETUTCDATE());
END

-- Insert sample web automations
IF NOT EXISTS (SELECT * FROM WebAutomations WHERE AutomationName = 'Login Automation')
BEGIN
    INSERT INTO WebAutomations (AutomationName, WebsiteUrl, StatusId, JobTypeId, TargetElement, Action, InputData, Browser, Device, CreatedAt)
    VALUES 
    ('Login Automation', 'https://example.com', 3, 1, 'input[type="email"]', 'Fill', '{"email": "test@example.com"}', 'Chrome', 'Desktop', GETUTCDATE()),
    ('Data Extraction', 'https://data.example.com', 2, 1, '.data-table', 'Extract', '{"format": "JSON"}', 'Chrome', 'Desktop', GETUTCDATE()),
    ('Form Submission', 'https://forms.example.com', 1, 1, 'form', 'Submit', '{"name": "John Doe", "email": "john@example.com"}', 'Chrome', 'Desktop', GETUTCDATE());
END

-- Insert sample job schedules
IF NOT EXISTS (SELECT * FROM JobSchedules WHERE JobName = 'Daily Report')
BEGIN
    INSERT INTO JobSchedules (JobName, JobTypeId, StatusId, CronExpression, Priority, IsEnabled, NextRunTime, CreatedAt)
    VALUES 
    ('Daily Report', 4, 1, '0 9 * * *', 'High', 1, DATEADD(day, 1, GETUTCDATE()), GETUTCDATE()),
    ('Weekly Cleanup', 5, 1, '0 2 * * 0', 'Medium', 1, DATEADD(week, 1, GETUTCDATE()), GETUTCDATE()),
    ('Monthly Backup', 4, 1, '0 1 1 * *', 'High', 1, DATEADD(month, 1, GETUTCDATE()), GETUTCDATE());
END

PRINT 'Database initialization completed successfully!';
