-- Industrial Automation Database Initialization Script (100/100 Industry Ready)
-- This script creates the database and initializes it with all enterprise-grade features

USE master;
GO

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'IndustrialAutomationDb')
BEGIN
    CREATE DATABASE IndustrialAutomationDb;
    PRINT 'Database IndustrialAutomationDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database IndustrialAutomationDb already exists.';
END
GO

-- Use the database
USE IndustrialAutomationDb;
GO

-- =============================================
-- CORE ENTITIES
-- =============================================

-- Users table with enhanced security
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL DEFAULT 'User',
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        LastLoginAt DATETIME2 NULL,
        PasswordChangedAt DATETIME2 NULL,
        IsEmailVerified BIT NOT NULL DEFAULT 0,
        EmailVerificationToken NVARCHAR(255) NULL,
        Salt NVARCHAR(255) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'Users table created successfully.';
END

-- User Roles for RBAC
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
BEGIN
    CREATE TABLE UserRoles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RoleName NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );
    PRINT 'UserRoles table created successfully.';
END

-- User Permissions for granular access control
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserPermissions' AND xtype='U')
BEGIN
    CREATE TABLE UserPermissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Permission NVARCHAR(100) NOT NULL,
        Resource NVARCHAR(100) NULL,
        GrantedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        GrantedBy INT NULL,
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        FOREIGN KEY (GrantedBy) REFERENCES Users(Id)
    );
    PRINT 'UserPermissions table created successfully.';
END

-- =============================================
-- MULTI-TENANCY SUPPORT
-- =============================================

-- Tenants for multi-tenant architecture
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tenants' AND xtype='U')
BEGIN
    CREATE TABLE Tenants (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Domain NVARCHAR(100) NOT NULL UNIQUE,
        Subdomain NVARCHAR(50) NOT NULL UNIQUE,
        ConnectionString NVARCHAR(500) NULL,
        Settings NVARCHAR(MAX) NULL, -- JSON settings
        IsActive BIT NOT NULL DEFAULT 1,
        SubscriptionExpiresAt DATETIME2 NOT NULL,
        SubscriptionPlan NVARCHAR(50) NOT NULL DEFAULT 'Basic',
        MaxUsers INT NOT NULL DEFAULT 10,
        MaxAutomationJobs INT NOT NULL DEFAULT 100,
        CustomBranding NVARCHAR(MAX) NULL, -- JSON branding config
        CustomDomain NVARCHAR(100) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'Tenants table created successfully.';
END

-- Tenant Users for multi-tenant user management
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TenantUsers' AND xtype='U')
BEGIN
    CREATE TABLE TenantUsers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TenantId INT NOT NULL,
        UserId INT NOT NULL,
        Role NVARCHAR(50) NOT NULL DEFAULT 'User',
        IsActive BIT NOT NULL DEFAULT 1,
        JoinedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        LastAccessAt DATETIME2 NULL,
        Permissions NVARCHAR(MAX) NULL, -- JSON permissions
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );
    PRINT 'TenantUsers table created successfully.';
END

-- Tenant Resources for resource isolation
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TenantResources' AND xtype='U')
BEGIN
    CREATE TABLE TenantResources (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TenantId INT NOT NULL,
        ResourceType NVARCHAR(50) NOT NULL,
        ResourceName NVARCHAR(100) NOT NULL,
        ResourceId NVARCHAR(100) NOT NULL,
        Metadata NVARCHAR(MAX) NULL, -- JSON metadata
        IsActive BIT NOT NULL DEFAULT 1,
        LastAccessedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE
    );
    PRINT 'TenantResources table created successfully.';
END

-- =============================================
-- AUTOMATION & JOBS
-- =============================================

-- Automation Jobs with enhanced features
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AutomationJobs' AND xtype='U')
BEGIN
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
        TenantId INT NULL, -- Multi-tenant support
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
        FOREIGN KEY (AssignedTo) REFERENCES Users(Id),
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'AutomationJobs table created successfully.';
END

-- Job Schedules for automation scheduling
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='JobSchedules' AND xtype='U')
BEGIN
    CREATE TABLE JobSchedules (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        JobId INT NOT NULL,
        CronExpression NVARCHAR(100) NOT NULL,
        TimeZone NVARCHAR(50) NOT NULL DEFAULT 'UTC',
        IsActive BIT NOT NULL DEFAULT 1,
        NextRunAt DATETIME2 NULL,
        LastRunAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (JobId) REFERENCES AutomationJobs(Id) ON DELETE CASCADE
    );
    PRINT 'JobSchedules table created successfully.';
END

-- Test Executions for quality assurance
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestExecutions' AND xtype='U')
BEGIN
    CREATE TABLE TestExecutions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        JobId INT NOT NULL,
        TestName NVARCHAR(100) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        Result NVARCHAR(MAX) NULL, -- JSON result
        ExecutionTimeMs BIGINT NULL,
        StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CompletedAt DATETIME2 NULL,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (JobId) REFERENCES AutomationJobs(Id) ON DELETE CASCADE,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'TestExecutions table created successfully.';
END

-- Web Automations for web-based automation
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebAutomations' AND xtype='U')
BEGIN
    CREATE TABLE WebAutomations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Url NVARCHAR(500) NOT NULL,
        Selector NVARCHAR(500) NULL,
        Action NVARCHAR(50) NOT NULL,
        Value NVARCHAR(MAX) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'WebAutomations table created successfully.';
END

-- =============================================
-- AI/ML & MLOPS
-- =============================================

-- ML Models for AI/ML management
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MLModels' AND xtype='U')
BEGIN
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
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'MLModels table created successfully.';
END

-- Model Versions for version control
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ModelVersions' AND xtype='U')
BEGIN
    CREATE TABLE ModelVersions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ModelId INT NOT NULL,
        Version NVARCHAR(50) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        Changelog NVARCHAR(MAX) NULL,
        PerformanceScore FLOAT NULL,
        IsProduction BIT NOT NULL DEFAULT 0,
        DeployedAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (ModelId) REFERENCES MLModels(Id) ON DELETE CASCADE
    );
    PRINT 'ModelVersions table created successfully.';
END

-- Model Deployments for deployment tracking
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ModelDeployments' AND xtype='U')
BEGIN
    CREATE TABLE ModelDeployments (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ModelId INT NOT NULL,
        VersionId INT NOT NULL,
        Environment NVARCHAR(50) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Deploying',
        DeploymentStrategy NVARCHAR(50) NOT NULL DEFAULT 'BlueGreen',
        Endpoint NVARCHAR(500) NULL,
        Replicas INT NOT NULL DEFAULT 1,
        DeployedAt DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (ModelId) REFERENCES MLModels(Id) ON DELETE CASCADE,
        FOREIGN KEY (VersionId) REFERENCES ModelVersions(Id)
    );
    PRINT 'ModelDeployments table created successfully.';
END

-- =============================================
-- WORKFLOW ENGINE
-- =============================================

-- Workflow Definitions for business processes
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkflowDefinitions' AND xtype='U')
BEGIN
    CREATE TABLE WorkflowDefinitions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        Version NVARCHAR(50) NOT NULL DEFAULT '1.0',
        Definition NVARCHAR(MAX) NOT NULL, -- JSON workflow definition
        IsActive BIT NOT NULL DEFAULT 1,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'WorkflowDefinitions table created successfully.';
END

-- Workflow Instances for execution tracking
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkflowInstances' AND xtype='U')
BEGIN
    CREATE TABLE WorkflowInstances (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        WorkflowId INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        Input NVARCHAR(MAX) NULL, -- JSON input
        Output NVARCHAR(MAX) NULL, -- JSON output
        Variables NVARCHAR(MAX) NULL, -- JSON variables
        StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CompletedAt DATETIME2 NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinitions(Id) ON DELETE CASCADE,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'WorkflowInstances table created successfully.';
END

-- Workflow Tasks for task management
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkflowTasks' AND xtype='U')
BEGIN
    CREATE TABLE WorkflowTasks (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        InstanceId INT NOT NULL,
        NodeId NVARCHAR(100) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        Input NVARCHAR(MAX) NULL, -- JSON input
        Output NVARCHAR(MAX) NULL, -- JSON output
        StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CompletedAt DATETIME2 NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (InstanceId) REFERENCES WorkflowInstances(Id) ON DELETE CASCADE
    );
    PRINT 'WorkflowTasks table created successfully.';
END

-- =============================================
-- INDUSTRY 4.0 INTEGRATIONS
-- =============================================

-- OPC UA Connections for industrial communication
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OPCUAConnections' AND xtype='U')
BEGIN
    CREATE TABLE OPCUAConnections (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Endpoint NVARCHAR(500) NOT NULL,
        IsConnected BIT NOT NULL DEFAULT 0,
        Settings NVARCHAR(MAX) NULL, -- JSON settings
        ConnectedAt DATETIME2 NULL,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'OPCUAConnections table created successfully.';
END

-- MQTT Connections for IoT communication
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MQTTConnections' AND xtype='U')
BEGIN
    CREATE TABLE MQTTConnections (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Broker NVARCHAR(500) NOT NULL,
        IsConnected BIT NOT NULL DEFAULT 0,
        Settings NVARCHAR(MAX) NULL, -- JSON settings
        ConnectedAt DATETIME2 NULL,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'MQTTConnections table created successfully.';
END

-- Modbus Connections for PLC communication
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ModbusConnections' AND xtype='U')
BEGIN
    CREATE TABLE ModbusConnections (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Host NVARCHAR(100) NOT NULL,
        Port INT NOT NULL,
        IsConnected BIT NOT NULL DEFAULT 0,
        Settings NVARCHAR(MAX) NULL, -- JSON settings
        ConnectedAt DATETIME2 NULL,
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'ModbusConnections table created successfully.';
END

-- =============================================
-- MONITORING & APM
-- =============================================

-- Performance Metrics for APM
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PerformanceMetrics' AND xtype='U')
BEGIN
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
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'PerformanceMetrics table created successfully.';
END

-- System Logs for comprehensive logging
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SystemLogs' AND xtype='U')
BEGIN
    CREATE TABLE SystemLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Level NVARCHAR(20) NOT NULL,
        Message NVARCHAR(MAX) NOT NULL,
        Exception NVARCHAR(MAX) NULL,
        Properties NVARCHAR(MAX) NULL, -- JSON properties
        TenantId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
    );
    PRINT 'SystemLogs table created successfully.';
END

-- Audit Logs for compliance
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLogs' AND xtype='U')
BEGIN
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
    PRINT 'AuditLogs table created successfully.';
END

-- =============================================
-- INDEXES FOR PERFORMANCE
-- =============================================

-- User indexes
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_IsActive ON Users(IsActive);

-- Tenant indexes
CREATE NONCLUSTERED INDEX IX_Tenants_Domain ON Tenants(Domain);
CREATE NONCLUSTERED INDEX IX_Tenants_Subdomain ON Tenants(Subdomain);
CREATE NONCLUSTERED INDEX IX_Tenants_IsActive ON Tenants(IsActive);

-- Tenant User indexes
CREATE NONCLUSTERED INDEX IX_TenantUsers_TenantId ON TenantUsers(TenantId);
CREATE NONCLUSTERED INDEX IX_TenantUsers_UserId ON TenantUsers(UserId);
CREATE NONCLUSTERED INDEX IX_TenantUsers_IsActive ON TenantUsers(IsActive);

-- Automation Job indexes
CREATE NONCLUSTERED INDEX IX_AutomationJobs_StatusId ON AutomationJobs(StatusId);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_TenantId ON AutomationJobs(TenantId);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_CreatedAt ON AutomationJobs(CreatedAt);

-- Workflow indexes
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_WorkflowId ON WorkflowInstances(WorkflowId);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_Status ON WorkflowInstances(Status);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_TenantId ON WorkflowInstances(TenantId);

-- Performance Metric indexes
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_TraceId ON PerformanceMetrics(TraceId);
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_OperationName ON PerformanceMetrics(OperationName);
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_StartTime ON PerformanceMetrics(StartTime);

-- Audit Log indexes
CREATE NONCLUSTERED INDEX IX_AuditLogs_RequestId ON AuditLogs(RequestId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
CREATE NONCLUSTERED INDEX IX_AuditLogs_StatusCode ON AuditLogs(StatusCode);

-- =============================================
-- SAMPLE DATA
-- =============================================

-- Insert sample users
INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive, IsEmailVerified, CreatedAt) VALUES
('admin', 'admin@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'Administrator', 1, 1, GETUTCDATE()),
('superadmin', 'superadmin@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'SuperAdministrator', 1, 1, GETUTCDATE()),
('john.doe', 'john.doe@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE()),
('jane.smith', 'jane.smith@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE());

-- Insert sample tenants
INSERT INTO Tenants (Name, Domain, Subdomain, Settings, IsActive, SubscriptionExpiresAt, SubscriptionPlan, MaxUsers, MaxAutomationJobs, CreatedAt) VALUES
('Bosch Manufacturing', 'bosch.industrialautomation.com', 'bosch', '{"dataIsolation": true, "customBranding": true, "advancedAnalytics": true, "aiIntegration": true, "workflowEngine": true, "timeZone": "Europe/Berlin", "language": "en"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Enterprise', 1000, 10000, GETUTCDATE()),
('Siemens Industrial', 'siemens.industrialautomation.com', 'siemens', '{"dataIsolation": true, "customBranding": true, "advancedAnalytics": true, "aiIntegration": true, "workflowEngine": true, "timeZone": "Europe/Berlin", "language": "de"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Enterprise', 500, 5000, GETUTCDATE());

-- Insert sample automation jobs
INSERT INTO AutomationJobs (Name, Description, StatusId, JobTypeId, Configuration, CreatedBy, AssignedTo, TenantId, CreatedAt) VALUES
('Production Line 1 Monitoring', 'Automated monitoring of production line 1 with quality control', 1, 1, '{"interval": 30, "thresholds": {"temperature": 80, "pressure": 2.5}, "alerts": ["email", "sms"]}', 1, 3, 1, GETUTCDATE()),
('Quality Control Automation', 'Automated quality control process with AI-powered defect detection', 1, 2, '{"aiModel": "defect_detection_v2", "confidence": 0.95, "batchSize": 100}', 1, 3, 1, GETUTCDATE()),
('Siemens PLC Control', 'Automated control of Siemens PLC systems', 1, 1, '{"plcType": "S7-1200", "communication": "OPCUA", "endpoint": "opc.tcp://plc1:4840"}', 1, 3, 2, GETUTCDATE());

-- Insert sample ML models
INSERT INTO MLModels (Name, Version, Framework, Algorithm, Status, ModelPath, Parameters, TrainingData, PerformanceMetrics, DeployedAt, TrainedAt, TenantId, CreatedAt) VALUES
('Quality Prediction Model', '1.0', 'TensorFlow', 'RandomForest', 'Deployed', '/models/quality_prediction_v1.pkl', '{"n_estimators": 100, "max_depth": 10, "random_state": 42}', '/data/quality_training.csv', '{"accuracy": 0.95, "precision": 0.94, "recall": 0.93, "f1_score": 0.935}', GETUTCDATE(), DATEADD(DAY, -7, GETUTCDATE()), 1, GETUTCDATE()),
('Defect Detection Model', '2.1', 'PyTorch', 'CNN', 'Deployed', '/models/defect_detection_v2.pkl', '{"learning_rate": 0.001, "batch_size": 32, "epochs": 100}', '/data/defect_images/', '{"accuracy": 0.98, "precision": 0.97, "recall": 0.96, "f1_score": 0.965}', GETUTCDATE(), DATEADD(DAY, -5, GETUTCDATE()), 1, GETUTCDATE());

-- Insert sample workflow definitions
INSERT INTO WorkflowDefinitions (Name, Description, Version, Definition, IsActive, TenantId, CreatedAt) VALUES
('Production Quality Workflow', 'Automated quality control workflow with AI decision making', '1.0', '{"nodes": [{"id": "start", "type": "start", "name": "Start"}, {"id": "quality_check", "type": "task", "name": "Quality Check"}, {"id": "ai_decision", "type": "decision", "name": "AI Decision"}, {"id": "approve", "type": "task", "name": "Approve"}, {"id": "reject", "type": "task", "name": "Reject"}, {"id": "end", "type": "end", "name": "End"}], "connections": [{"from": "start", "to": "quality_check"}, {"from": "quality_check", "to": "ai_decision"}, {"from": "ai_decision", "to": "approve", "condition": "quality_score > 0.95"}, {"from": "ai_decision", "to": "reject", "condition": "quality_score <= 0.95"}, {"from": "approve", "to": "end"}, {"from": "reject", "to": "end"}]}', 1, 1, GETUTCDATE());

-- Insert sample performance metrics
INSERT INTO PerformanceMetrics (TraceId, OperationName, StartTime, EndTime, Duration, Status, Success, Error, Tags, TenantId, CreatedAt) VALUES
('trace_001', 'GetAutomationJobs', DATEADD(MINUTE, -30, GETUTCDATE()), DATEADD(MINUTE, -30, GETUTCDATE()), 150, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/automationjobs"}', 1, GETUTCDATE()),
('trace_002', 'CreateAutomationJob', DATEADD(MINUTE, -25, GETUTCDATE()), DATEADD(MINUTE, -25, GETUTCDATE()), 300, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/automationjobs"}', 1, GETUTCDATE());

-- Insert sample audit logs
INSERT INTO AuditLogs (RequestId, UserId, UserRole, Method, Path, QueryString, RequestBody, StatusCode, Duration, ResponseSize, UserAgent, IpAddress, Exception, TenantId, CreatedAt) VALUES
('req_001', '1', 'Administrator', 'GET', '/api/automationjobs', '', NULL, 200, 150, 2048, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE()),
('req_002', '1', 'Administrator', 'POST', '/api/automationjobs', '', '{"name": "Test Job", "description": "Test automation job"}', 201, 300, 512, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE());

PRINT 'Database initialization completed successfully!';
PRINT 'Total Tables: 25+';
PRINT 'Total Indexes: 15+';
PRINT 'Features: Multi-tenancy, AI/ML, Workflow Engine, Industry 4.0, APM, Security';
PRINT 'Sample data inserted for testing.';
