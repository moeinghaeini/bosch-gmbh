-- Create Comprehensive Indexes for Performance
USE IndustrialAutomationDb;
GO

-- User indexes
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_IsActive ON Users(IsActive);
CREATE NONCLUSTERED INDEX IX_Users_LastLoginAt ON Users(LastLoginAt);
CREATE NONCLUSTERED INDEX IX_Users_CreatedAt ON Users(CreatedAt);

-- Tenant indexes
CREATE NONCLUSTERED INDEX IX_Tenants_Domain ON Tenants(Domain);
CREATE NONCLUSTERED INDEX IX_Tenants_Subdomain ON Tenants(Subdomain);
CREATE NONCLUSTERED INDEX IX_Tenants_IsActive ON Tenants(IsActive);
CREATE NONCLUSTERED INDEX IX_Tenants_SubscriptionPlan ON Tenants(SubscriptionPlan);

-- Tenant User indexes
CREATE NONCLUSTERED INDEX IX_TenantUsers_TenantId ON TenantUsers(TenantId);
CREATE NONCLUSTERED INDEX IX_TenantUsers_UserId ON TenantUsers(UserId);
CREATE NONCLUSTERED INDEX IX_TenantUsers_IsActive ON TenantUsers(IsActive);
CREATE NONCLUSTERED INDEX IX_TenantUsers_LastAccessAt ON TenantUsers(LastAccessAt);

-- Automation Job indexes
CREATE NONCLUSTERED INDEX IX_AutomationJobs_StatusId ON AutomationJobs(StatusId);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_TenantId ON AutomationJobs(TenantId);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_CreatedAt ON AutomationJobs(CreatedAt);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_Priority ON AutomationJobs(Priority);
CREATE NONCLUSTERED INDEX IX_AutomationJobs_ScheduledAt ON AutomationJobs(ScheduledAt);

-- Workflow indexes
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_WorkflowId ON WorkflowInstances(WorkflowId);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_Status ON WorkflowInstances(Status);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_TenantId ON WorkflowInstances(TenantId);
CREATE NONCLUSTERED INDEX IX_WorkflowInstances_StartedAt ON WorkflowInstances(StartedAt);

-- Performance Metric indexes
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_TraceId ON PerformanceMetrics(TraceId);
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_OperationName ON PerformanceMetrics(OperationName);
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_StartTime ON PerformanceMetrics(StartTime);
CREATE NONCLUSTERED INDEX IX_PerformanceMetrics_Success ON PerformanceMetrics(Success);

-- Audit Log indexes
CREATE NONCLUSTERED INDEX IX_AuditLogs_RequestId ON AuditLogs(RequestId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
CREATE NONCLUSTERED INDEX IX_AuditLogs_StatusCode ON AuditLogs(StatusCode);
CREATE NONCLUSTERED INDEX IX_AuditLogs_Method ON AuditLogs(Method);

-- Alert indexes
CREATE NONCLUSTERED INDEX IX_Alerts_Severity ON Alerts(Severity);
CREATE NONCLUSTERED INDEX IX_Alerts_Status ON Alerts(Status);
CREATE NONCLUSTERED INDEX IX_Alerts_TriggeredAt ON Alerts(TriggeredAt);
CREATE NONCLUSTERED INDEX IX_Alerts_TenantId ON Alerts(TenantId);

PRINT 'Comprehensive indexes created successfully!';
