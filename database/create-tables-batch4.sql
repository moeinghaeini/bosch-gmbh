-- Batch 4: Workflow Engine & Industry 4.0
USE IndustrialAutomationDb;
GO

-- Workflow Definitions for comprehensive business processes
CREATE TABLE WorkflowDefinitions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Version NVARCHAR(50) NOT NULL DEFAULT '1.0',
    Definition NVARCHAR(MAX) NOT NULL, -- JSON workflow definition
    IsActive BIT NOT NULL DEFAULT 1,
    Category NVARCHAR(50) NULL,
    Tags NVARCHAR(MAX) NULL, -- JSON tags
    Variables NVARCHAR(MAX) NULL, -- JSON variables
    Settings NVARCHAR(MAX) NULL, -- JSON settings
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Workflow Instances for comprehensive execution tracking
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
    ExecutionLogs NVARCHAR(MAX) NULL, -- JSON execution logs
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (WorkflowId) REFERENCES WorkflowDefinitions(Id) ON DELETE CASCADE,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Workflow Tasks for comprehensive task management
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
    ExecutionLogs NVARCHAR(MAX) NULL, -- JSON execution logs
    RetryCount INT NOT NULL DEFAULT 0,
    MaxRetries INT NOT NULL DEFAULT 3,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (InstanceId) REFERENCES WorkflowInstances(Id) ON DELETE CASCADE
);

-- OPC UA Connections for comprehensive industrial communication
CREATE TABLE OPCUAConnections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Endpoint NVARCHAR(500) NOT NULL,
    IsConnected BIT NOT NULL DEFAULT 0,
    Settings NVARCHAR(MAX) NULL, -- JSON settings
    ConnectedAt DATETIME2 NULL,
    LastConnectedAt DATETIME2 NULL,
    ConnectionCount INT NOT NULL DEFAULT 0,
    ErrorCount INT NOT NULL DEFAULT 0,
    LastError NVARCHAR(MAX) NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- MQTT Connections for comprehensive IoT communication
CREATE TABLE MQTTConnections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Broker NVARCHAR(500) NOT NULL,
    IsConnected BIT NOT NULL DEFAULT 0,
    Settings NVARCHAR(MAX) NULL, -- JSON settings
    ConnectedAt DATETIME2 NULL,
    LastConnectedAt DATETIME2 NULL,
    MessageCount INT NOT NULL DEFAULT 0,
    ErrorCount INT NOT NULL DEFAULT 0,
    LastError NVARCHAR(MAX) NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- Modbus Connections for comprehensive PLC communication
CREATE TABLE ModbusConnections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Host NVARCHAR(100) NOT NULL,
    Port INT NOT NULL,
    IsConnected BIT NOT NULL DEFAULT 0,
    Settings NVARCHAR(MAX) NULL, -- JSON settings
    ConnectedAt DATETIME2 NULL,
    LastConnectedAt DATETIME2 NULL,
    RequestCount INT NOT NULL DEFAULT 0,
    ErrorCount INT NOT NULL DEFAULT 0,
    LastError NVARCHAR(MAX) NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

-- SCADA Systems for supervisory control
CREATE TABLE SCADASystems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SystemId NVARCHAR(100) NOT NULL,
    Endpoint NVARCHAR(500) NOT NULL,
    IsConnected BIT NOT NULL DEFAULT 0,
    Settings NVARCHAR(MAX) NULL, -- JSON settings
    ConnectedAt DATETIME2 NULL,
    LastConnectedAt DATETIME2 NULL,
    TagCount INT NOT NULL DEFAULT 0,
    ErrorCount INT NOT NULL DEFAULT 0,
    LastError NVARCHAR(MAX) NULL,
    TenantId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id)
);

PRINT 'Batch 4: Workflow Engine & Industry 4.0 created successfully!';
