-- Industrial Automation Platform - Advanced Seed Data (100/100 Industry Ready)
-- This seed data includes comprehensive test data for all enterprise features

USE IndustrialAutomationDb;
GO

-- =============================================
-- CLEAR EXISTING DATA
-- =============================================

-- Clear existing data in reverse dependency order
DELETE FROM WorkflowTasks;
DELETE FROM WorkflowInstances;
DELETE FROM WorkflowDefinitions;
DELETE FROM ModelDeployments;
DELETE FROM ModelVersions;
DELETE FROM MLModels;
DELETE FROM TestExecutions;
DELETE FROM JobSchedules;
DELETE FROM AutomationJobs;
DELETE FROM WebAutomations;
DELETE FROM OPCUAConnections;
DELETE FROM MQTTConnections;
DELETE FROM ModbusConnections;
DELETE FROM PerformanceMetrics;
DELETE FROM SystemLogs;
DELETE FROM AuditLogs;
DELETE FROM TenantResources;
DELETE FROM TenantUsers;
DELETE FROM Tenants;
DELETE FROM UserPermissions;
DELETE FROM UserRoles;
DELETE FROM Users;

-- =============================================
-- USERS & ROLES
-- =============================================

-- Admin users
INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive, IsEmailVerified, CreatedAt) VALUES
('admin', 'admin@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'Administrator', 1, 1, GETUTCDATE()),
('superadmin', 'superadmin@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'SuperAdministrator', 1, 1, GETUTCDATE()),
('system', 'system@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'System', 1, 1, GETUTCDATE());

-- Regular users
INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive, IsEmailVerified, CreatedAt) VALUES
('john.doe', 'john.doe@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE()),
('jane.smith', 'jane.smith@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE()),
('mike.wilson', 'mike.wilson@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE()),
('sarah.jones', 'sarah.jones@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE()),
('david.brown', 'david.brown@industrialautomation.com', '$2a$11$K8vZ8fT8fT8fT8fT8fT8fO', 'User', 1, 1, GETUTCDATE());

-- User Roles
INSERT INTO UserRoles (UserId, RoleName, CreatedAt) VALUES
(1, 'Administrator', GETUTCDATE()),
(1, 'UserManager', GETUTCDATE()),
(2, 'SuperAdministrator', GETUTCDATE()),
(2, 'SystemAdministrator', GETUTCDATE()),
(3, 'System', GETUTCDATE()),
(4, 'User', GETUTCDATE()),
(5, 'User', GETUTCDATE()),
(6, 'User', GETUTCDATE()),
(7, 'User', GETUTCDATE()),
(8, 'User', GETUTCDATE());

-- User Permissions
INSERT INTO UserPermissions (UserId, Permission, Resource, GrantedAt, GrantedBy) VALUES
(1, 'Read', 'Users', GETUTCDATE(), 2),
(1, 'Write', 'Users', GETUTCDATE(), 2),
(1, 'Delete', 'Users', GETUTCDATE(), 2),
(1, 'Read', 'AutomationJobs', GETUTCDATE(), 2),
(1, 'Write', 'AutomationJobs', GETUTCDATE(), 2),
(1, 'Delete', 'AutomationJobs', GETUTCDATE(), 2),
(2, 'Read', '*', GETUTCDATE(), 2),
(2, 'Write', '*', GETUTCDATE(), 2),
(2, 'Delete', '*', GETUTCDATE(), 2),
(4, 'Read', 'AutomationJobs', GETUTCDATE(), 1),
(4, 'Write', 'AutomationJobs', GETUTCDATE(), 1);

-- =============================================
-- TENANTS (MULTI-TENANCY)
-- =============================================

-- Enterprise tenants
INSERT INTO Tenants (Name, Domain, Subdomain, Settings, IsActive, SubscriptionExpiresAt, SubscriptionPlan, MaxUsers, MaxAutomationJobs, CreatedAt) VALUES
('Bosch Manufacturing', 'bosch.industrialautomation.com', 'bosch', '{"dataIsolation": true, "customBranding": true, "advancedAnalytics": true, "aiIntegration": true, "workflowEngine": true, "timeZone": "Europe/Berlin", "language": "en"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Enterprise', 1000, 10000, GETUTCDATE()),
('Siemens Industrial', 'siemens.industrialautomation.com', 'siemens', '{"dataIsolation": true, "customBranding": true, "advancedAnalytics": true, "aiIntegration": true, "workflowEngine": true, "timeZone": "Europe/Berlin", "language": "de"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Enterprise', 500, 5000, GETUTCDATE()),
('ABB Automation', 'abb.industrialautomation.com', 'abb', '{"dataIsolation": true, "customBranding": false, "advancedAnalytics": true, "aiIntegration": true, "workflowEngine": false, "timeZone": "Europe/Zurich", "language": "en"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Professional', 200, 2000, GETUTCDATE()),
('Schneider Electric', 'schneider.industrialautomation.com', 'schneider', '{"dataIsolation": true, "customBranding": false, "advancedAnalytics": false, "aiIntegration": false, "workflowEngine": false, "timeZone": "Europe/Paris", "language": "fr"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Standard', 100, 1000, GETUTCDATE()),
('Rockwell Automation', 'rockwell.industrialautomation.com', 'rockwell', '{"dataIsolation": true, "customBranding": true, "advancedAnalytics": true, "aiIntegration": true, "workflowEngine": true, "timeZone": "America/New_York", "language": "en"}', 1, DATEADD(YEAR, 1, GETUTCDATE()), 'Enterprise', 800, 8000, GETUTCDATE());

-- Tenant Users
INSERT INTO TenantUsers (TenantId, UserId, Role, IsActive, JoinedAt, Permissions, CreatedAt) VALUES
(1, 1, 'Administrator', 1, GETUTCDATE(), '{"canCreateJobs": true, "canEditJobs": true, "canDeleteJobs": true, "canViewAnalytics": true, "canManageUsers": true, "canAccessAI": true, "canManageSettings": true, "allowedModules": ["automation", "ai", "analytics", "workflow"]}', GETUTCDATE()),
(1, 4, 'User', 1, GETUTCDATE(), '{"canCreateJobs": true, "canEditJobs": true, "canDeleteJobs": false, "canViewAnalytics": true, "canManageUsers": false, "canAccessAI": true, "canManageSettings": false, "allowedModules": ["automation", "ai"]}', GETUTCDATE()),
(1, 5, 'User', 1, GETUTCDATE(), '{"canCreateJobs": true, "canEditJobs": true, "canDeleteJobs": false, "canViewAnalytics": true, "canManageUsers": false, "canAccessAI": false, "canManageSettings": false, "allowedModules": ["automation"]}', GETUTCDATE()),
(2, 1, 'Administrator', 1, GETUTCDATE(), '{"canCreateJobs": true, "canEditJobs": true, "canDeleteJobs": true, "canViewAnalytics": true, "canManageUsers": true, "canAccessAI": true, "canManageSettings": true, "allowedModules": ["automation", "ai", "analytics", "workflow"]}', GETUTCDATE()),
(3, 6, 'User', 1, GETUTCDATE(), '{"canCreateJobs": true, "canEditJobs": true, "canDeleteJobs": false, "canViewAnalytics": true, "canManageUsers": false, "canAccessAI": true, "canManageSettings": false, "allowedModules": ["automation", "ai"]}', GETUTCDATE());

-- Tenant Resources
INSERT INTO TenantResources (TenantId, ResourceType, ResourceName, ResourceId, Metadata, IsActive, LastAccessedAt, CreatedAt) VALUES
(1, 'Database', 'Bosch Production DB', 'bosch_prod_db', '{"connectionString": "encrypted_connection", "backupEnabled": true, "encryptionEnabled": true}', 1, GETUTCDATE(), GETUTCDATE()),
(1, 'API', 'Bosch Manufacturing API', 'bosch_manufacturing_api', '{"baseUrl": "https://api.bosch.com", "version": "v2", "authentication": "OAuth2"}', 1, GETUTCDATE(), GETUTCDATE()),
(1, 'MLModel', 'Quality Prediction Model', 'quality_prediction_v1', '{"modelType": "RandomForest", "accuracy": 0.95, "lastTrained": "2024-01-15"}', 1, GETUTCDATE(), GETUTCDATE()),
(2, 'Database', 'Siemens Industrial DB', 'siemens_industrial_db', '{"connectionString": "encrypted_connection", "backupEnabled": true, "encryptionEnabled": true}', 1, GETUTCDATE(), GETUTCDATE()),
(2, 'API', 'Siemens Industrial API', 'siemens_industrial_api', '{"baseUrl": "https://api.siemens.com", "version": "v1", "authentication": "APIKey"}', 1, GETUTCDATE(), GETUTCDATE());

-- =============================================
-- AUTOMATION JOBS
-- =============================================

-- Production automation jobs
INSERT INTO AutomationJobs (Name, Description, StatusId, JobTypeId, Configuration, CreatedBy, AssignedTo, TenantId, CreatedAt) VALUES
('Production Line 1 Monitoring', 'Automated monitoring of production line 1 with quality control', 1, 1, '{"interval": 30, "thresholds": {"temperature": 80, "pressure": 2.5}, "alerts": ["email", "sms"]}', 1, 4, 1, GETUTCDATE()),
('Quality Control Automation', 'Automated quality control process with AI-powered defect detection', 1, 2, '{"aiModel": "defect_detection_v2", "confidence": 0.95, "batchSize": 100}', 1, 4, 1, GETUTCDATE()),
('Inventory Management', 'Automated inventory tracking and reorder process', 1, 3, '{"reorderLevel": 50, "suppliers": ["supplier1", "supplier2"], "notification": true}', 1, 5, 1, GETUTCDATE()),
('Energy Consumption Monitoring', 'Real-time energy consumption monitoring and optimization', 2, 1, '{"sensors": ["sensor1", "sensor2", "sensor3"], "optimization": true}', 1, 4, 1, GETUTCDATE()),
('Predictive Maintenance', 'AI-powered predictive maintenance for critical equipment', 1, 4, '{"model": "maintenance_prediction_v1", "sensors": ["vibration", "temperature", "pressure"]}', 1, 4, 1, GETUTCDATE()),
('Siemens PLC Control', 'Automated control of Siemens PLC systems', 1, 1, '{"plcType": "S7-1200", "communication": "OPCUA", "endpoint": "opc.tcp://plc1:4840"}', 1, 4, 2, GETUTCDATE()),
('ABB Robot Control', 'Automated control of ABB robotic systems', 1, 2, '{"robotType": "IRB6700", "program": "welding_program_v3", "safety": true}', 1, 6, 3, GETUTCDATE());

-- Job Schedules
INSERT INTO JobSchedules (JobId, CronExpression, TimeZone, IsActive, NextRunAt, CreatedAt) VALUES
(1, '0 */30 * * * *', 'Europe/Berlin', 1, DATEADD(MINUTE, 30, GETUTCDATE()), GETUTCDATE()),
(2, '0 0 */6 * * *', 'Europe/Berlin', 1, DATEADD(HOUR, 6, GETUTCDATE()), GETUTCDATE()),
(3, '0 0 2 * * *', 'Europe/Berlin', 1, DATEADD(DAY, 1, DATEADD(HOUR, 2, GETUTCDATE())), GETUTCDATE()),
(4, '0 */15 * * * *', 'Europe/Berlin', 1, DATEADD(MINUTE, 15, GETUTCDATE()), GETUTCDATE()),
(5, '0 0 1 * * *', 'Europe/Berlin', 1, DATEADD(DAY, 1, DATEADD(HOUR, 1, GETUTCDATE())), GETUTCDATE()),
(6, '0 */10 * * * *', 'Europe/Berlin', 1, DATEADD(MINUTE, 10, GETUTCDATE()), GETUTCDATE()),
(7, '0 0 8 * * 1-5', 'Europe/Zurich', 1, DATEADD(DAY, 1, DATEADD(HOUR, 8, GETUTCDATE())), GETUTCDATE());

-- Test Executions
INSERT INTO TestExecutions (JobId, TestName, Status, Result, ExecutionTimeMs, StartedAt, CompletedAt, TenantId, CreatedAt) VALUES
(1, 'Production Line Health Check', 'Passed', '{"temperature": 75, "pressure": 2.3, "status": "healthy"}', 1500, DATEADD(MINUTE, -30, GETUTCDATE()), DATEADD(MINUTE, -29, GETUTCDATE()), 1, GETUTCDATE()),
(2, 'Quality Control Test', 'Passed', '{"defects": 0, "quality_score": 98.5, "batch_size": 100}', 2500, DATEADD(MINUTE, -25, GETUTCDATE()), DATEADD(MINUTE, -23, GETUTCDATE()), 1, GETUTCDATE()),
(3, 'Inventory Check', 'Passed', '{"items": 150, "reorder_needed": false, "stock_level": "good"}', 800, DATEADD(MINUTE, -20, GETUTCDATE()), DATEADD(MINUTE, -19, GETUTCDATE()), 1, GETUTCDATE()),
(4, 'Energy Monitoring Test', 'Failed', '{"error": "Sensor communication timeout", "retry_count": 3}', 5000, DATEADD(MINUTE, -15, GETUTCDATE()), DATEADD(MINUTE, -10, GETUTCDATE()), 1, GETUTCDATE()),
(5, 'Predictive Maintenance Check', 'Passed', '{"maintenance_needed": false, "next_check": "2024-02-15", "confidence": 0.92}', 3000, DATEADD(MINUTE, -10, GETUTCDATE()), DATEADD(MINUTE, -7, GETUTCDATE()), 1, GETUTCDATE());

-- Web Automations
INSERT INTO WebAutomations (Name, Url, Selector, Action, Value, IsActive, TenantId, CreatedAt) VALUES
('Bosch Portal Login', 'https://portal.bosch.com/login', '#username', 'input', 'automated_user', 1, 1, GETUTCDATE()),
('Siemens Dashboard Access', 'https://dashboard.siemens.com', '#login-button', 'click', NULL, 1, 2, GETUTCDATE()),
('ABB System Status', 'https://status.abb.com', '#status-indicator', 'get_text', NULL, 1, 3, GETUTCDATE()),
('Schneider Reports', 'https://reports.schneider.com', '#generate-report', 'click', NULL, 1, 4, GETUTCDATE()),
('Rockwell Analytics', 'https://analytics.rockwell.com', '#refresh-data', 'click', NULL, 1, 5, GETUTCDATE());

-- =============================================
-- AI/ML MODELS
-- =============================================

-- ML Models
INSERT INTO MLModels (Name, Version, Framework, Algorithm, Status, ModelPath, Parameters, TrainingData, PerformanceMetrics, DeployedAt, TrainedAt, TenantId, CreatedAt) VALUES
('Quality Prediction Model', '1.0', 'TensorFlow', 'RandomForest', 'Deployed', '/models/quality_prediction_v1.pkl', '{"n_estimators": 100, "max_depth": 10, "random_state": 42}', '/data/quality_training.csv', '{"accuracy": 0.95, "precision": 0.94, "recall": 0.93, "f1_score": 0.935}', GETUTCDATE(), DATEADD(DAY, -7, GETUTCDATE()), 1, GETUTCDATE()),
('Defect Detection Model', '2.1', 'PyTorch', 'CNN', 'Deployed', '/models/defect_detection_v2.pkl', '{"learning_rate": 0.001, "batch_size": 32, "epochs": 100}', '/data/defect_images/', '{"accuracy": 0.98, "precision": 0.97, "recall": 0.96, "f1_score": 0.965}', GETUTCDATE(), DATEADD(DAY, -5, GETUTCDATE()), 1, GETUTCDATE()),
('Maintenance Prediction Model', '1.2', 'Scikit-learn', 'GradientBoosting', 'Training', '/models/maintenance_prediction_v1.pkl', '{"n_estimators": 200, "learning_rate": 0.1, "max_depth": 8}', '/data/maintenance_history.csv', '{"accuracy": 0.92, "precision": 0.91, "recall": 0.90, "f1_score": 0.905}', NULL, DATEADD(DAY, -3, GETUTCDATE()), 1, GETUTCDATE()),
('Energy Optimization Model', '1.0', 'TensorFlow', 'LSTM', 'Deployed', '/models/energy_optimization_v1.pkl', '{"sequence_length": 24, "hidden_units": 64, "dropout": 0.2}', '/data/energy_consumption.csv', '{"mae": 0.05, "rmse": 0.08, "r2_score": 0.89}', GETUTCDATE(), DATEADD(DAY, -10, GETUTCDATE()), 1, GETUTCDATE()),
('Siemens PLC Control Model', '1.0', 'PyTorch', 'ReinforcementLearning', 'Deployed', '/models/plc_control_v1.pkl', '{"learning_rate": 0.01, "gamma": 0.95, "epsilon": 0.1}', '/data/plc_control_logs.csv', '{"reward": 0.85, "success_rate": 0.92}', GETUTCDATE(), DATEADD(DAY, -14, GETUTCDATE()), 2, GETUTCDATE());

-- Model Versions
INSERT INTO ModelVersions (ModelId, Version, Status, Changelog, PerformanceScore, IsProduction, DeployedAt, CreatedAt) VALUES
(1, '1.0', 'Production', 'Initial release with basic quality prediction', 0.95, 1, GETUTCDATE(), GETUTCDATE()),
(1, '1.1', 'Staging', 'Improved feature engineering and hyperparameter tuning', 0.96, 0, NULL, GETUTCDATE()),
(2, '2.0', 'Production', 'Major update with improved CNN architecture', 0.98, 1, GETUTCDATE(), GETUTCDATE()),
(2, '2.1', 'Production', 'Bug fixes and performance improvements', 0.98, 1, GETUTCDATE(), GETUTCDATE()),
(3, '1.0', 'Draft', 'Initial maintenance prediction model', 0.92, 0, NULL, GETUTCDATE()),
(3, '1.1', 'Testing', 'Added more features and improved accuracy', 0.94, 0, NULL, GETUTCDATE()),
(3, '1.2', 'Training', 'Latest version with enhanced algorithms', 0.92, 0, NULL, GETUTCDATE());

-- Model Deployments
INSERT INTO ModelDeployments (ModelId, VersionId, Environment, Status, DeploymentStrategy, Endpoint, Replicas, DeployedAt, CreatedAt) VALUES
(1, 1, 'Production', 'Active', 'BlueGreen', 'https://api.industrialautomation.com/models/quality-prediction', 3, GETUTCDATE(), GETUTCDATE()),
(2, 3, 'Production', 'Active', 'BlueGreen', 'https://api.industrialautomation.com/models/defect-detection', 2, GETUTCDATE(), GETUTCDATE()),
(4, 4, 'Production', 'Active', 'Rolling', 'https://api.industrialautomation.com/models/energy-optimization', 1, GETUTCDATE(), GETUTCDATE()),
(5, 5, 'Production', 'Active', 'BlueGreen', 'https://api.industrialautomation.com/models/plc-control', 1, GETUTCDATE(), GETUTCDATE());

-- =============================================
-- WORKFLOW ENGINE
-- =============================================

-- Workflow Definitions
INSERT INTO WorkflowDefinitions (Name, Description, Version, Definition, IsActive, TenantId, CreatedAt) VALUES
('Production Quality Workflow', 'Automated quality control workflow with AI decision making', '1.0', '{"nodes": [{"id": "start", "type": "start", "name": "Start"}, {"id": "quality_check", "type": "task", "name": "Quality Check"}, {"id": "ai_decision", "type": "decision", "name": "AI Decision"}, {"id": "approve", "type": "task", "name": "Approve"}, {"id": "reject", "type": "task", "name": "Reject"}, {"id": "end", "type": "end", "name": "End"}], "connections": [{"from": "start", "to": "quality_check"}, {"from": "quality_check", "to": "ai_decision"}, {"from": "ai_decision", "to": "approve", "condition": "quality_score > 0.95"}, {"from": "ai_decision", "to": "reject", "condition": "quality_score <= 0.95"}, {"from": "approve", "to": "end"}, {"from": "reject", "to": "end"}]}', 1, 1, GETUTCDATE()),
('Maintenance Workflow', 'Predictive maintenance workflow with automated scheduling', '1.0', '{"nodes": [{"id": "start", "type": "start", "name": "Start"}, {"id": "sensor_data", "type": "task", "name": "Collect Sensor Data"}, {"id": "ai_analysis", "type": "task", "name": "AI Analysis"}, {"id": "maintenance_decision", "type": "decision", "name": "Maintenance Decision"}, {"id": "schedule_maintenance", "type": "task", "name": "Schedule Maintenance"}, {"id": "continue_monitoring", "type": "task", "name": "Continue Monitoring"}, {"id": "end", "type": "end", "name": "End"}], "connections": [{"from": "start", "to": "sensor_data"}, {"from": "sensor_data", "to": "ai_analysis"}, {"from": "ai_analysis", "to": "maintenance_decision"}, {"from": "maintenance_decision", "to": "schedule_maintenance", "condition": "maintenance_needed = true"}, {"from": "maintenance_decision", "to": "continue_monitoring", "condition": "maintenance_needed = false"}, {"from": "schedule_maintenance", "to": "end"}, {"from": "continue_monitoring", "to": "end"}]}', 1, 1, GETUTCDATE()),
('Energy Optimization Workflow', 'Automated energy optimization workflow', '1.0', '{"nodes": [{"id": "start", "type": "start", "name": "Start"}, {"id": "energy_data", "type": "task", "name": "Collect Energy Data"}, {"id": "optimization_analysis", "type": "task", "name": "Optimization Analysis"}, {"id": "apply_optimization", "type": "task", "name": "Apply Optimization"}, {"id": "monitor_results", "type": "task", "name": "Monitor Results"}, {"id": "end", "type": "end", "name": "End"}], "connections": [{"from": "start", "to": "energy_data"}, {"from": "energy_data", "to": "optimization_analysis"}, {"from": "optimization_analysis", "to": "apply_optimization"}, {"from": "apply_optimization", "to": "monitor_results"}, {"from": "monitor_results", "to": "end"}]}', 1, 1, GETUTCDATE());

-- Workflow Instances
INSERT INTO WorkflowInstances (WorkflowId, Status, Input, Output, Variables, StartedAt, CompletedAt, TenantId, CreatedAt) VALUES
(1, 'Completed', '{"product_id": "P001", "batch_size": 100, "quality_threshold": 0.95}', '{"quality_score": 0.98, "decision": "approved", "processed_count": 100}', '{"current_step": "end", "quality_score": 0.98}', DATEADD(HOUR, -2, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE()), 1, GETUTCDATE()),
(1, 'Running', '{"product_id": "P002", "batch_size": 50, "quality_threshold": 0.95}', NULL, '{"current_step": "ai_decision", "quality_score": 0.92}', DATEADD(MINUTE, -30, GETUTCDATE()), NULL, 1, GETUTCDATE()),
(2, 'Completed', '{"equipment_id": "EQ001", "sensor_data": {"vibration": 0.8, "temperature": 75, "pressure": 2.3}}', '{"maintenance_needed": false, "next_check": "2024-02-15", "confidence": 0.92}', '{"current_step": "end", "maintenance_needed": false}', DATEADD(HOUR, -4, GETUTCDATE()), DATEADD(HOUR, -3, GETUTCDATE()), 1, GETUTCDATE()),
(3, 'Running', '{"energy_sources": ["solar", "wind", "grid"], "consumption_data": {"current": 150, "peak": 200}}', NULL, '{"current_step": "optimization_analysis", "current_consumption": 150}', DATEADD(MINUTE, -15, GETUTCDATE()), NULL, 1, GETUTCDATE());

-- Workflow Tasks
INSERT INTO WorkflowTasks (InstanceId, NodeId, Name, Status, Input, Output, StartedAt, CompletedAt, CreatedAt) VALUES
(1, 'start', 'Start', 'Completed', '{"product_id": "P001", "batch_size": 100}', '{"status": "started"}', DATEADD(HOUR, -2, GETUTCDATE()), DATEADD(HOUR, -2, GETUTCDATE()), GETUTCDATE()),
(1, 'quality_check', 'Quality Check', 'Completed', '{"product_id": "P001", "batch_size": 100}', '{"quality_score": 0.98, "defects": 0}', DATEADD(HOUR, -2, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE()), GETUTCDATE()),
(1, 'ai_decision', 'AI Decision', 'Completed', '{"quality_score": 0.98, "threshold": 0.95}', '{"decision": "approved", "confidence": 0.95}', DATEADD(HOUR, -1, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE()), GETUTCDATE()),
(1, 'approve', 'Approve', 'Completed', '{"decision": "approved"}', '{"status": "approved", "processed_count": 100}', DATEADD(HOUR, -1, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE()), GETUTCDATE()),
(2, 'start', 'Start', 'Completed', '{"product_id": "P002", "batch_size": 50}', '{"status": "started"}', DATEADD(MINUTE, -30, GETUTCDATE()), DATEADD(MINUTE, -30, GETUTCDATE()), GETUTCDATE()),
(2, 'quality_check', 'Quality Check', 'Completed', '{"product_id": "P002", "batch_size": 50}', '{"quality_score": 0.92, "defects": 2}', DATEADD(MINUTE, -30, GETUTCDATE()), DATEADD(MINUTE, -25, GETUTCDATE()), GETUTCDATE()),
(2, 'ai_decision', 'AI Decision', 'Running', '{"quality_score": 0.92, "threshold": 0.95}', NULL, DATEADD(MINUTE, -25, GETUTCDATE()), NULL, GETUTCDATE());

-- =============================================
-- INDUSTRY 4.0 INTEGRATIONS
-- =============================================

-- OPC UA Connections
INSERT INTO OPCUAConnections (Name, Endpoint, IsConnected, Settings, ConnectedAt, TenantId, CreatedAt) VALUES
('Bosch Production Line 1', 'opc.tcp://plc1.bosch.com:4840', 1, '{"securityPolicy": "None", "securityMode": "None", "sessionTimeout": 60000, "requestTimeout": 5000}', GETUTCDATE(), 1, GETUTCDATE()),
('Siemens S7-1200 PLC', 'opc.tcp://plc2.siemens.com:4840', 1, '{"securityPolicy": "Basic256", "securityMode": "SignAndEncrypt", "username": "opcuser", "password": "encrypted_password"}', GETUTCDATE(), 2, GETUTCDATE()),
('ABB Robot Controller', 'opc.tcp://robot1.abb.com:4840', 0, '{"securityPolicy": "None", "securityMode": "None", "sessionTimeout": 30000}', NULL, 3, GETUTCDATE());

-- MQTT Connections
INSERT INTO MQTTConnections (Name, Broker, IsConnected, Settings, ConnectedAt, TenantId, CreatedAt) VALUES
('Bosch IoT Hub', 'mqtt://iot.bosch.com:1883', 1, '{"clientId": "bosch_automation", "username": "mqtt_user", "password": "encrypted_password", "cleanSession": true, "keepAlive": 60}', GETUTCDATE(), 1, GETUTCDATE()),
('Siemens MindSphere', 'mqtt://mindsphere.siemens.com:8883', 1, '{"clientId": "siemens_automation", "username": "mindsphere_user", "password": "encrypted_password", "cleanSession": false, "keepAlive": 120}', GETUTCDATE(), 2, GETUTCDATE()),
('ABB Ability', 'mqtt://ability.abb.com:1883', 0, '{"clientId": "abb_automation", "username": "ability_user", "password": "encrypted_password", "cleanSession": true, "keepAlive": 60}', NULL, 3, GETUTCDATE());

-- Modbus Connections
INSERT INTO ModbusConnections (Name, Host, Port, IsConnected, Settings, ConnectedAt, TenantId, CreatedAt) VALUES
('Bosch Production PLC', 'plc1.bosch.com', 502, 1, '{"slaveId": 1, "timeout": 5000, "retries": 3, "protocol": "TCP"}', GETUTCDATE(), 1, GETUTCDATE()),
('Siemens Industrial PLC', 'plc2.siemens.com', 502, 1, '{"slaveId": 2, "timeout": 3000, "retries": 5, "protocol": "TCP"}', GETUTCDATE(), 2, GETUTCDATE()),
('ABB Control System', 'plc3.abb.com', 502, 0, '{"slaveId": 1, "timeout": 5000, "retries": 3, "protocol": "TCP"}', NULL, 3, GETUTCDATE());

-- =============================================
-- MONITORING & APM DATA
-- =============================================

-- Performance Metrics
INSERT INTO PerformanceMetrics (TraceId, OperationName, StartTime, EndTime, Duration, Status, Success, Error, Tags, TenantId, CreatedAt) VALUES
('trace_001', 'GetAutomationJobs', DATEADD(MINUTE, -30, GETUTCDATE()), DATEADD(MINUTE, -30, GETUTCDATE()), 150, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/automationjobs"}', 1, GETUTCDATE()),
('trace_002', 'CreateAutomationJob', DATEADD(MINUTE, -25, GETUTCDATE()), DATEADD(MINUTE, -25, GETUTCDATE()), 300, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/automationjobs"}', 1, GETUTCDATE()),
('trace_003', 'UpdateAutomationJob', DATEADD(MINUTE, -20, GETUTCDATE()), DATEADD(MINUTE, -20, GETUTCDATE()), 250, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/automationjobs/1"}', 1, GETUTCDATE()),
('trace_004', 'DeleteAutomationJob', DATEADD(MINUTE, -15, GETUTCDATE()), DATEADD(MINUTE, -15, GETUTCDATE()), 200, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/automationjobs/2"}', 1, GETUTCDATE()),
('trace_005', 'GetMLModels', DATEADD(MINUTE, -10, GETUTCDATE()), DATEADD(MINUTE, -10, GETUTCDATE()), 400, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/mlops/models"}', 1, GETUTCDATE()),
('trace_006', 'DeployModel', DATEADD(MINUTE, -5, GETUTCDATE()), DATEADD(MINUTE, -5, GETUTCDATE()), 2000, 'Completed', 1, NULL, '{"user_id": "1", "tenant_id": "1", "endpoint": "/api/mlops/deploy"}', 1, GETUTCDATE());

-- System Logs
INSERT INTO SystemLogs (Level, Message, Exception, Properties, TenantId, CreatedAt) VALUES
('Information', 'User logged in successfully', NULL, '{"user_id": "1", "username": "admin", "ip_address": "192.168.1.100"}', 1, GETUTCDATE()),
('Information', 'Automation job created', NULL, '{"job_id": "1", "job_name": "Production Line 1 Monitoring", "user_id": "1"}', 1, GETUTCDATE()),
('Warning', 'High CPU usage detected', NULL, '{"cpu_usage": 85, "threshold": 80, "server": "prod-server-1"}', 1, GETUTCDATE()),
('Error', 'Database connection timeout', 'TimeoutException: Connection timeout after 30 seconds', '{"connection_string": "encrypted", "retry_count": 3}', 1, GETUTCDATE()),
('Information', 'ML model deployed successfully', NULL, '{"model_id": "1", "model_name": "Quality Prediction Model", "version": "1.0"}', 1, GETUTCDATE());

-- Audit Logs
INSERT INTO AuditLogs (RequestId, UserId, UserRole, Method, Path, QueryString, RequestBody, StatusCode, Duration, ResponseSize, UserAgent, IpAddress, Exception, TenantId, CreatedAt) VALUES
('req_001', '1', 'Administrator', 'GET', '/api/automationjobs', '', NULL, 200, 150, 2048, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE()),
('req_002', '1', 'Administrator', 'POST', '/api/automationjobs', '', '{"name": "Test Job", "description": "Test automation job"}', 201, 300, 512, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE()),
('req_003', '1', 'Administrator', 'PUT', '/api/automationjobs/1', '', '{"name": "Updated Job", "description": "Updated automation job"}', 200, 250, 512, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE()),
('req_004', '1', 'Administrator', 'DELETE', '/api/automationjobs/2', '', NULL, 204, 200, 0, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE()),
('req_005', '1', 'Administrator', 'GET', '/api/mlops/models', '', NULL, 200, 400, 4096, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE()),
('req_006', '1', 'Administrator', 'POST', '/api/mlops/deploy', '', '{"modelId": "1", "version": "1.0", "environment": "production"}', 200, 2000, 1024, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', NULL, 1, GETUTCDATE());

PRINT 'Advanced Seed Data (100/100 Industry Ready) created successfully!';
PRINT 'Total Users: 8';
PRINT 'Total Tenants: 5';
PRINT 'Total Automation Jobs: 7';
PRINT 'Total ML Models: 5';
PRINT 'Total Workflows: 3';
PRINT 'Total Industry 4.0 Connections: 9';
PRINT 'Total Performance Metrics: 6';
PRINT 'Total Audit Logs: 6';
PRINT 'Features: Multi-tenancy, AI/ML, Workflow Engine, Industry 4.0, APM, Security';
