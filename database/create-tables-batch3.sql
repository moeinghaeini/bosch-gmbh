-- Batch 3: Automation & AI/ML
USE IndustrialAutomationDb;
GO

-- Automation Jobs with comprehensive features
CREATE TABLE AutomationJobs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    StatusId INT NOT NULL DEFAULT 1,
    JobTypeId INT NOT NULL DEFAULT 1,
    Configuration NVARCHAR(MAX) NULL, -- JSON configuration
    ErrorMessage NVARCHAR(MAX) NULL,
    CreatedBy INT NULL,
    AssignedTo INT NULL,
    ExecutionTimeMs BIGINT NULL,
    ScheduledAt DATETIME2 NULL,
    StartedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    Priority INT NOT NULL DEFAULT 1,
    RetryCount INT NOT NULL DEFAULT 0,
    MaxRetries INT NOT NULL DEFAULT 3,
    TimeoutMinutes INT NOT NULL DEFAULT 60,
    Tags NVARCHAR(MAX) NULL, -- JSON tags
    Metadata NVARCHAR(MAX) NULL, -- JSON metadata
    TenantId INT NULL, -- Multi-tenant support
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    FOREIGN KEY (AssignedTo) REFERENCES Users(Id),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Job Schedules for advanced automation scheduling
CREATE TABLE JobSchedules (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobId INT NOT NULL,
    CronExpression NVARCHAR(100) NOT NULL,
    TimeZone NVARCHAR(50) NOT NULL DEFAULT 'UTC',
    IsActive BIT NOT NULL DEFAULT 1,
    NextRunAt DATETIME2 NULL,
    LastRunAt DATETIME2 NULL,
    LastRunStatus NVARCHAR(20) NULL,
    LastRunError NVARCHAR(MAX) NULL,
    RunCount INT NOT NULL DEFAULT 0,
    SuccessCount INT NOT NULL DEFAULT 0,
    FailureCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (JobId) REFERENCES AutomationJobs(Id) ON DELETE CASCADE
);

-- Test Executions for comprehensive quality assurance
CREATE TABLE TestExecutions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobId INT NOT NULL,
    TestName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Result NVARCHAR(MAX) NULL, -- JSON result
    ExecutionTimeMs BIGINT NULL,
    StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    TestData NVARCHAR(MAX) NULL, -- JSON test data
    ExpectedResult NVARCHAR(MAX) NULL, -- JSON expected result
    ActualResult NVARCHAR(MAX) NULL, -- JSON actual result
    ErrorMessage NVARCHAR(MAX) NULL,
    Screenshots NVARCHAR(MAX) NULL, -- JSON screenshot paths
    Logs NVARCHAR(MAX) NULL, -- JSON execution logs
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (JobId) REFERENCES AutomationJobs(Id) ON DELETE CASCADE,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Web Automations for advanced web-based automation
CREATE TABLE WebAutomations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    Selector NVARCHAR(500) NULL,
    Action NVARCHAR(50) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    Configuration NVARCHAR(MAX) NULL, -- JSON configuration
    Screenshots NVARCHAR(MAX) NULL, -- JSON screenshot paths
    Logs NVARCHAR(MAX) NULL, -- JSON execution logs
    LastExecutedAt DATETIME2 NULL,
    ExecutionCount INT NOT NULL DEFAULT 0,
    SuccessCount INT NOT NULL DEFAULT 0,
    FailureCount INT NOT NULL DEFAULT 0,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- ML Models for comprehensive AI/ML management
CREATE TABLE MLModels (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Version NVARCHAR(50) NOT NULL,
    Framework NVARCHAR(50) NOT NULL,
    Algorithm NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Training',
    ModelPath NVARCHAR(500) NULL,
    Parameters NVARCHAR(MAX) NULL, -- JSON parameters
    TrainingData NVARCHAR(500) NULL,
    PerformanceMetrics NVARCHAR(MAX) NULL, -- JSON metrics
    DeployedAt DATETIME2 NULL,
    TrainedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    TrainingDuration BIGINT NULL,
    ModelSize BIGINT NULL,
    Accuracy FLOAT NULL,
    Precision FLOAT NULL,
    Recall FLOAT NULL,
    F1Score FLOAT NULL,
    Auc FLOAT NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Model Versions for comprehensive version control
CREATE TABLE ModelVersions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ModelId INT NOT NULL,
    Version NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    Changelog NVARCHAR(MAX) NULL,
    PerformanceScore FLOAT NULL,
    IsProduction BIT NOT NULL DEFAULT 0,
    DeployedAt DATETIME2 NULL,
    RollbackToVersion NVARCHAR(50) NULL,
    DeploymentStrategy NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (ModelId) REFERENCES MLModels(Id) ON DELETE CASCADE
);

-- Model Deployments for comprehensive deployment tracking
CREATE TABLE ModelDeployments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ModelId INT NOT NULL,
    VersionId INT NOT NULL,
    Environment NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Deploying',
    DeploymentStrategy NVARCHAR(50) NOT NULL DEFAULT 'BlueGreen',
    Endpoint NVARCHAR(500) NULL,
    Replicas INT NOT NULL DEFAULT 1,
    CpuLimit NVARCHAR(20) NULL,
    MemoryLimit NVARCHAR(20) NULL,
    HealthCheckUrl NVARCHAR(500) NULL,
    DeployedAt DATETIME2 NULL,
    RolledBackAt DATETIME2 NULL,
    DeploymentLogs NVARCHAR(MAX) NULL, -- JSON deployment logs
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (ModelId) REFERENCES MLModels(Id) ON DELETE CASCADE,
    FOREIGN KEY (VersionId) REFERENCES ModelVersions(Id)
);

PRINT 'Batch 3: Automation & AI/ML created successfully!';
