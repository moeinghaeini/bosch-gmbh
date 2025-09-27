-- Bosch Industrial Automation Platform - Comprehensive Database Schema
-- This schema is designed for production use with proper relationships, indexing, and constraints

USE IndustrialAutomationDb;

-- =============================================
-- ENUMS AND LOOKUP TABLES
-- =============================================

-- Job Status Lookup
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobStatuses' AND xtype='U')
BEGIN
    CREATE TABLE JobStatuses (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(50) NOT NULL UNIQUE,
        Description nvarchar(200) NULL,
        IsActive bit NOT NULL DEFAULT 1,
        SortOrder int NOT NULL DEFAULT 0,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL
    );
    
    INSERT INTO JobStatuses (Name, Description, SortOrder) VALUES
    ('Pending', 'Job is waiting to be executed', 1),
    ('Running', 'Job is currently executing', 2),
    ('Completed', 'Job has finished successfully', 3),
    ('Failed', 'Job has failed with errors', 4),
    ('Cancelled', 'Job was cancelled by user', 5),
    ('Scheduled', 'Job is scheduled for future execution', 6);
END

-- Job Types Lookup
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobTypes' AND xtype='U')
BEGIN
    CREATE TABLE JobTypes (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(100) NOT NULL UNIQUE,
        Description nvarchar(200) NULL,
        Category nvarchar(50) NOT NULL, -- WebAutomation, DataProcessing, ReportGeneration, etc.
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL
    );
    
    INSERT INTO JobTypes (Name, Description, Category) VALUES
    ('WebAutomation', 'Automated web browser interactions', 'Automation'),
    ('DataProcessing', 'Data transformation and processing', 'Data'),
    ('ReportGeneration', 'Automated report creation', 'Reporting'),
    ('TestExecution', 'Automated test execution', 'Testing'),
    ('FileProcessing', 'File manipulation and processing', 'Data'),
    ('APIIntegration', 'Third-party API integrations', 'Integration');
END

-- Test Types Lookup
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestTypes' AND xtype='U')
BEGIN
    CREATE TABLE TestTypes (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(50) NOT NULL UNIQUE,
        Description nvarchar(200) NULL,
        Category nvarchar(50) NOT NULL,
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL
    );
    
    INSERT INTO TestTypes (Name, Description, Category) VALUES
    ('Unit', 'Individual component testing', 'Functional'),
    ('Integration', 'Component interaction testing', 'Functional'),
    ('E2E', 'End-to-end user journey testing', 'Functional'),
    ('Performance', 'Load and stress testing', 'Non-Functional'),
    ('Security', 'Security vulnerability testing', 'Non-Functional'),
    ('UI', 'User interface testing', 'Functional');
END

-- =============================================
-- CORE ENTITIES
-- =============================================

-- Users with enhanced security and audit fields
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Username nvarchar(100) NOT NULL UNIQUE,
        Email nvarchar(200) NOT NULL UNIQUE,
        PasswordHash nvarchar(500) NOT NULL,
        Salt nvarchar(100) NOT NULL,
        Role nvarchar(50) NOT NULL,
        IsActive bit NOT NULL DEFAULT 1,
        IsEmailVerified bit NOT NULL DEFAULT 0,
        LastLoginAt datetime2 NULL,
        PasswordChangedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        FailedLoginAttempts int NOT NULL DEFAULT 0,
        LockedUntil datetime2 NULL,
        TwoFactorEnabled bit NOT NULL DEFAULT 0,
        TwoFactorSecret nvarchar(100) NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        DeletedAt datetime2 NULL,
        DeletedBy int NULL
    );
    
    -- Indexes for performance
    CREATE INDEX IX_Users_Email ON Users(Email);
    CREATE INDEX IX_Users_Role ON Users(Role);
    CREATE INDEX IX_Users_IsActive ON Users(IsActive);
    CREATE INDEX IX_Users_LastLoginAt ON Users(LastLoginAt);
    CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);
END

-- User Roles with hierarchical permissions
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
BEGIN
    CREATE TABLE UserRoles (
        Id int IDENTITY(1,1) PRIMARY KEY,
        RoleName nvarchar(100) NOT NULL UNIQUE,
        Description nvarchar(500) NULL,
        Permissions nvarchar(max) NULL, -- JSON permissions structure
        IsActive bit NOT NULL DEFAULT 1,
        Priority int NOT NULL DEFAULT 0, -- Role hierarchy
        AllowedModules nvarchar(max) NULL, -- JSON allowed modules
        AllowedActions nvarchar(max) NULL, -- JSON allowed actions
        Restrictions nvarchar(max) NULL, -- JSON restrictions
        LastModified datetime2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedBy nvarchar(100) NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0
    );
    
    -- Insert default roles
    INSERT INTO UserRoles (RoleName, Description, Priority, Permissions, AllowedModules, AllowedActions) VALUES
    ('SuperAdmin', 'Full system access', 100, '["*"]', '["*"]', '["*"]'),
    ('Admin', 'Administrative access', 90, '["read", "write", "delete", "manage_users"]', '["*"]', '["*"]'),
    ('Manager', 'Management access', 80, '["read", "write", "approve"]', '["automation", "reports", "users"]', '["create", "update", "view"]'),
    ('Developer', 'Development access', 70, '["read", "write", "execute"]', '["automation", "testing"]', '["create", "update", "execute"]'),
    ('Tester', 'Testing access', 60, '["read", "write"]', '["testing", "automation"]', '["create", "update", "execute"]'),
    ('Viewer', 'Read-only access', 50, '["read"]', '["*"]', '["view"]');
END

-- User Permissions with fine-grained control
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserPermissions' AND xtype='U')
BEGIN
    CREATE TABLE UserPermissions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UserId int NOT NULL,
        RoleId int NOT NULL,
        PermissionType nvarchar(50) NOT NULL, -- Read, Write, Execute, Admin
        Resource nvarchar(100) NOT NULL, -- Module or feature
        IsGranted bit NOT NULL DEFAULT 1,
        GrantedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        GrantedBy nvarchar(100) NOT NULL,
        ExpiresAt datetime2 NULL,
        Notes nvarchar(500) NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (UserId) REFERENCES Users(Id),
        FOREIGN KEY (RoleId) REFERENCES UserRoles(Id)
    );
    
    CREATE INDEX IX_UserPermissions_UserId ON UserPermissions(UserId);
    CREATE INDEX IX_UserPermissions_RoleId ON UserPermissions(RoleId);
    CREATE INDEX IX_UserPermissions_Resource ON UserPermissions(Resource);
END

-- Automation Jobs with enhanced tracking
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AutomationJobs' AND xtype='U')
BEGIN
    CREATE TABLE AutomationJobs (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(1000) NULL,
        StatusId int NOT NULL,
        JobTypeId int NOT NULL,
        Configuration nvarchar(max) NULL, -- JSON configuration
        ErrorMessage nvarchar(2000) NULL,
        ScheduledAt datetime2 NULL,
        StartedAt datetime2 NULL,
        CompletedAt datetime2 NULL,
        ExecutionTimeMs bigint NULL,
        RetryCount int NOT NULL DEFAULT 0,
        MaxRetries int NOT NULL DEFAULT 3,
        Priority int NOT NULL DEFAULT 0,
        CreatedBy int NOT NULL,
        AssignedTo int NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (StatusId) REFERENCES JobStatuses(Id),
        FOREIGN KEY (JobTypeId) REFERENCES JobTypes(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
        FOREIGN KEY (AssignedTo) REFERENCES Users(Id)
    );
    
    -- Performance indexes
    CREATE INDEX IX_AutomationJobs_StatusId ON AutomationJobs(StatusId);
    CREATE INDEX IX_AutomationJobs_JobTypeId ON AutomationJobs(JobTypeId);
    CREATE INDEX IX_AutomationJobs_CreatedBy ON AutomationJobs(CreatedBy);
    CREATE INDEX IX_AutomationJobs_ScheduledAt ON AutomationJobs(ScheduledAt);
    CREATE INDEX IX_AutomationJobs_CreatedAt ON AutomationJobs(CreatedAt);
    CREATE INDEX IX_AutomationJobs_StatusId_CreatedAt ON AutomationJobs(StatusId, CreatedAt);
END

-- Test Executions with comprehensive tracking
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestExecutions' AND xtype='U')
BEGIN
    CREATE TABLE TestExecutions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        TestName nvarchar(200) NOT NULL,
        TestTypeId int NOT NULL,
        StatusId int NOT NULL,
        TestSuite nvarchar(200) NOT NULL,
        TestData nvarchar(max) NULL, -- JSON test data
        ExpectedResult nvarchar(max) NULL,
        ActualResult nvarchar(max) NULL,
        ErrorMessage nvarchar(2000) NULL,
        ExecutionTimeMs bigint NULL,
        AIAnalysis nvarchar(max) NULL,
        ConfidenceScore decimal(5,2) NULL,
        TestEnvironment nvarchar(100) NULL,
        Browser nvarchar(50) NULL,
        Device nvarchar(50) NULL,
        ScreenshotPath nvarchar(500) NULL,
        VideoPath nvarchar(500) NULL,
        LogPath nvarchar(500) NULL,
        CreatedBy int NOT NULL,
        ExecutedBy int NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (TestTypeId) REFERENCES TestTypes(Id),
        FOREIGN KEY (StatusId) REFERENCES JobStatuses(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
        FOREIGN KEY (ExecutedBy) REFERENCES Users(Id)
    );
    
    -- Performance indexes
    CREATE INDEX IX_TestExecutions_TestTypeId ON TestExecutions(TestTypeId);
    CREATE INDEX IX_TestExecutions_StatusId ON TestExecutions(StatusId);
    CREATE INDEX IX_TestExecutions_CreatedBy ON TestExecutions(CreatedBy);
    CREATE INDEX IX_TestExecutions_CreatedAt ON TestExecutions(CreatedAt);
    CREATE INDEX IX_TestExecutions_StatusId_CreatedAt ON TestExecutions(StatusId, CreatedAt);
END

-- Web Automations with enhanced tracking
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
        Browser nvarchar(50) NULL,
        Device nvarchar(50) NULL,
        UserAgent nvarchar(500) NULL,
        ViewportSize nvarchar(20) NULL,
        ConfidenceScore decimal(5,2) NULL,
        ExecutionTimeMs bigint NULL,
        CreatedBy int NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (StatusId) REFERENCES JobStatuses(Id),
        FOREIGN KEY (JobTypeId) REFERENCES JobTypes(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );
    
    -- Performance indexes
    CREATE INDEX IX_WebAutomations_StatusId ON WebAutomations(StatusId);
    CREATE INDEX IX_WebAutomations_JobTypeId ON WebAutomations(JobTypeId);
    CREATE INDEX IX_WebAutomations_CreatedBy ON WebAutomations(CreatedBy);
    CREATE INDEX IX_WebAutomations_CreatedAt ON WebAutomations(CreatedAt);
    CREATE INDEX IX_WebAutomations_WebsiteUrl ON WebAutomations(WebsiteUrl);
END

-- Job Schedules with advanced scheduling
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSchedules' AND xtype='U')
BEGIN
    CREATE TABLE JobSchedules (
        Id int IDENTITY(1,1) PRIMARY KEY,
        JobName nvarchar(200) NOT NULL,
        JobTypeId int NOT NULL,
        StatusId int NOT NULL,
        CronExpression nvarchar(100) NULL,
        Configuration nvarchar(max) NULL, -- JSON configuration
        Priority int NOT NULL DEFAULT 0,
        TimeZone nvarchar(50) NULL,
        IsEnabled bit NOT NULL DEFAULT 1,
        NextRunTime datetime2 NULL,
        LastRunTime datetime2 NULL,
        LastRunStatus nvarchar(50) NULL,
        ErrorMessage nvarchar(2000) NULL,
        ExecutionHistory nvarchar(max) NULL, -- JSON execution history
        Notifications nvarchar(max) NULL, -- JSON notification settings
        Dependencies nvarchar(max) NULL, -- JSON job dependencies
        MaxExecutionTime int NULL, -- Maximum execution time in minutes
        RetryPolicy nvarchar(max) NULL, -- JSON retry policy
        CreatedBy int NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (JobTypeId) REFERENCES JobTypes(Id),
        FOREIGN KEY (StatusId) REFERENCES JobStatuses(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );
    
    -- Performance indexes
    CREATE INDEX IX_JobSchedules_JobTypeId ON JobSchedules(JobTypeId);
    CREATE INDEX IX_JobSchedules_StatusId ON JobSchedules(StatusId);
    CREATE INDEX IX_JobSchedules_IsEnabled ON JobSchedules(IsEnabled);
    CREATE INDEX IX_JobSchedules_NextRunTime ON JobSchedules(NextRunTime);
    CREATE INDEX IX_JobSchedules_CreatedBy ON JobSchedules(CreatedBy);
    CREATE INDEX IX_JobSchedules_StatusId_IsEnabled ON JobSchedules(StatusId, IsEnabled);
END

-- ML Models with versioning
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MLModels' AND xtype='U')
BEGIN
    CREATE TABLE MLModels (
        Id int IDENTITY(1,1) PRIMARY KEY,
        ModelName nvarchar(200) NOT NULL,
        ModelType nvarchar(100) NOT NULL,
        Version nvarchar(50) NOT NULL,
        StatusId int NOT NULL,
        Accuracy decimal(5,2) NULL,
        TrainingData nvarchar(max) NULL, -- JSON training data metadata
        ModelPath nvarchar(500) NULL,
        Configuration nvarchar(max) NULL, -- JSON model configuration
        PerformanceMetrics nvarchar(max) NULL, -- JSON performance metrics
        IsActive bit NOT NULL DEFAULT 0,
        CreatedBy int NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (StatusId) REFERENCES JobStatuses(Id),
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );
    
    CREATE INDEX IX_MLModels_ModelName ON MLModels(ModelName);
    CREATE INDEX IX_MLModels_ModelType ON MLModels(ModelType);
    CREATE INDEX IX_MLModels_IsActive ON MLModels(IsActive);
    CREATE INDEX IX_MLModels_CreatedBy ON MLModels(CreatedBy);
END

-- AI Training Data with categorization
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
        IsProcessed bit NOT NULL DEFAULT 0,
        CreatedBy int NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
    );
    
    CREATE INDEX IX_AITrainingData_DataType ON AITrainingData(DataType);
    CREATE INDEX IX_AITrainingData_Category ON AITrainingData(Category);
    CREATE INDEX IX_AITrainingData_IsProcessed ON AITrainingData(IsProcessed);
    CREATE INDEX IX_AITrainingData_CreatedBy ON AITrainingData(CreatedBy);
END

-- =============================================
-- AUDIT AND LOGGING TABLES
-- =============================================

-- System Audit Log
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLogs' AND xtype='U')
BEGIN
    CREATE TABLE AuditLogs (
        Id bigint IDENTITY(1,1) PRIMARY KEY,
        UserId int NULL,
        Action nvarchar(100) NOT NULL,
        EntityType nvarchar(100) NOT NULL,
        EntityId int NULL,
        OldValues nvarchar(max) NULL, -- JSON old values
        NewValues nvarchar(max) NULL, -- JSON new values
        IpAddress nvarchar(45) NULL,
        UserAgent nvarchar(500) NULL,
        Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE(),
        
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    
    CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
    CREATE INDEX IX_AuditLogs_EntityType ON AuditLogs(EntityType);
    CREATE INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp);
    CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
END

-- System Logs
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SystemLogs' AND xtype='U')
BEGIN
    CREATE TABLE SystemLogs (
        Id bigint IDENTITY(1,1) PRIMARY KEY,
        Level nvarchar(20) NOT NULL, -- INFO, WARN, ERROR, DEBUG
        Message nvarchar(max) NOT NULL,
        Exception nvarchar(max) NULL,
        Source nvarchar(200) NULL,
        UserId int NULL,
        IpAddress nvarchar(45) NULL,
        Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE(),
        
        FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    
    CREATE INDEX IX_SystemLogs_Level ON SystemLogs(Level);
    CREATE INDEX IX_SystemLogs_Timestamp ON SystemLogs(Timestamp);
    CREATE INDEX IX_SystemLogs_Source ON SystemLogs(Source);
    CREATE INDEX IX_SystemLogs_UserId ON SystemLogs(UserId);
END

-- =============================================
-- PERFORMANCE AND MONITORING TABLES
-- =============================================

-- System Performance Metrics
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PerformanceMetrics' AND xtype='U')
BEGIN
    CREATE TABLE PerformanceMetrics (
        Id bigint IDENTITY(1,1) PRIMARY KEY,
        MetricName nvarchar(100) NOT NULL,
        MetricValue decimal(18,4) NOT NULL,
        MetricUnit nvarchar(20) NULL,
        Category nvarchar(50) NULL,
        Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE(),
        Tags nvarchar(max) NULL -- JSON tags
    );
    
    CREATE INDEX IX_PerformanceMetrics_MetricName ON PerformanceMetrics(MetricName);
    CREATE INDEX IX_PerformanceMetrics_Timestamp ON PerformanceMetrics(Timestamp);
    CREATE INDEX IX_PerformanceMetrics_Category ON PerformanceMetrics(Category);
END

-- Job Execution History
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobExecutionHistory' AND xtype='U')
BEGIN
    CREATE TABLE JobExecutionHistory (
        Id bigint IDENTITY(1,1) PRIMARY KEY,
        JobId int NOT NULL,
        JobType nvarchar(100) NOT NULL,
        Status nvarchar(50) NOT NULL,
        StartedAt datetime2 NOT NULL,
        CompletedAt datetime2 NULL,
        ExecutionTimeMs bigint NULL,
        ErrorMessage nvarchar(2000) NULL,
        ResourceUsage nvarchar(max) NULL, -- JSON resource usage
        Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_JobExecutionHistory_JobId ON JobExecutionHistory(JobId);
    CREATE INDEX IX_JobExecutionHistory_JobType ON JobExecutionHistory(JobType);
    CREATE INDEX IX_JobExecutionHistory_Status ON JobExecutionHistory(Status);
    CREATE INDEX IX_JobExecutionHistory_StartedAt ON JobExecutionHistory(StartedAt);
END

PRINT 'Comprehensive database schema created successfully!';
