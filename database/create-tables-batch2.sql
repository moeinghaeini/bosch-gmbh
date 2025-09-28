-- Batch 2: Multi-Tenancy Support
USE IndustrialAutomationDb;
GO

-- Tenants for comprehensive multi-tenant architecture
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
    MaxStorageGB INT NOT NULL DEFAULT 10,
    CustomBranding NVARCHAR(MAX) NULL, -- JSON branding config
    CustomDomain NVARCHAR(100) NULL,
    BillingEmail NVARCHAR(100) NULL,
    BillingAddress NVARCHAR(MAX) NULL, -- JSON address
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedBy INT NULL,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);

-- Tenant Users for multi-tenant user management
CREATE TABLE TenantUsers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    UserId INT NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    IsActive BIT NOT NULL DEFAULT 1,
    JoinedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastAccessAt DATETIME2 NULL,
    Permissions NVARCHAR(MAX) NULL, -- JSON permissions
    InvitedBy INT NULL,
    InvitedAt DATETIME2 NULL,
    AcceptedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (InvitedBy) REFERENCES Users(Id)
);

-- Tenant Resources for resource isolation
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

-- Tenant Billing for subscription management
CREATE TABLE TenantBilling (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    SubscriptionId NVARCHAR(100) NOT NULL,
    PlanName NVARCHAR(50) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Currency NVARCHAR(3) NOT NULL DEFAULT 'USD',
    BillingCycle NVARCHAR(20) NOT NULL, -- Monthly, Yearly
    Status NVARCHAR(20) NOT NULL, -- Active, Cancelled, Suspended
    NextBillingDate DATETIME2 NOT NULL,
    LastPaymentDate DATETIME2 NULL,
    PaymentMethod NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE
);

PRINT 'Batch 2: Multi-Tenancy Support created successfully!';
