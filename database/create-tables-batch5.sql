-- Batch 5: Monitoring & APM
USE IndustrialAutomationDb;
GO

-- Performance Metrics for comprehensive APM
CREATE TABLE PerformanceMetrics (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TraceId NVARCHAR(100) NOT NULL,
    OperationName NVARCHAR(100) NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Duration BIGINT NULL,
    Status NVARCHAR(50) NOT NULL,
    Success BIT NOT NULL DEFAULT 1,
    Error NVARCHAR(MAX) NULL,
    Tags NVARCHAR(MAX) NULL, -- JSON tags
    Metadata NVARCHAR(MAX) NULL, -- JSON metadata
    CpuUsage FLOAT NULL,
    MemoryUsage FLOAT NULL,
    DiskUsage FLOAT NULL,
    NetworkLatency BIGINT NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- System Logs for comprehensive logging
CREATE TABLE SystemLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Level NVARCHAR(20) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Exception NVARCHAR(MAX) NULL,
    Properties NVARCHAR(MAX) NULL, -- JSON properties
    Source NVARCHAR(100) NULL,
    Category NVARCHAR(100) NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Audit Logs for comprehensive compliance
CREATE TABLE AuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequestId NVARCHAR(100) NOT NULL,
    UserId NVARCHAR(100) NULL,
    UserRole NVARCHAR(50) NULL,
    Method NVARCHAR(10) NOT NULL,
    Path NVARCHAR(500) NULL,
    QueryString NVARCHAR(1000) NULL,
    RequestBody NVARCHAR(MAX) NULL,
    StatusCode INT NOT NULL,
    Duration BIGINT NOT NULL,
    ResponseSize BIGINT NOT NULL,
    UserAgent NVARCHAR(500) NULL,
    IpAddress NVARCHAR(45) NULL,
    Exception NVARCHAR(MAX) NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Alerts for comprehensive monitoring
CREATE TABLE Alerts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Severity NVARCHAR(20) NOT NULL, -- Critical, High, Medium, Low
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Acknowledged, Resolved
    Source NVARCHAR(100) NULL,
    Metric NVARCHAR(100) NULL,
    Threshold FLOAT NULL,
    CurrentValue FLOAT NULL,
    TriggeredAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    AcknowledgedAt DATETIME2 NULL,
    AcknowledgedBy INT NULL,
    ResolvedAt DATETIME2 NULL,
    ResolvedBy INT NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    FOREIGN KEY (AcknowledgedBy) REFERENCES Users(Id),
    FOREIGN KEY (ResolvedBy) REFERENCES Users(Id),
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Model Training Jobs for ML pipeline management
CREATE TABLE ModelTrainingJobs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ModelId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Queued',
    TrainingData NVARCHAR(500) NOT NULL,
    Parameters NVARCHAR(MAX) NULL, -- JSON parameters
    StartedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    Duration BIGINT NULL,
    Progress FLOAT NULL,
    Logs NVARCHAR(MAX) NULL, -- JSON training logs
    ErrorMessage NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    FOREIGN KEY (ModelId) REFERENCES MLModels(Id) ON DELETE CASCADE
);

PRINT 'Batch 5: Monitoring & APM created successfully!';
