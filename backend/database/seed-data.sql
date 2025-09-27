-- Bosch Industrial Automation Platform - Comprehensive Seed Data
-- This script populates the database with realistic, production-ready data

USE IndustrialAutomationDb;

-- =============================================
-- CLEAR EXISTING DATA (for fresh start)
-- =============================================

-- Clear existing data in reverse dependency order
DELETE FROM JobExecutionHistory;
DELETE FROM PerformanceMetrics;
DELETE FROM SystemLogs;
DELETE FROM AuditLogs;
DELETE FROM AITrainingData;
DELETE FROM MLModels;
DELETE FROM JobSchedules;
DELETE FROM WebAutomations;
DELETE FROM TestExecutions;
DELETE FROM AutomationJobs;
DELETE FROM UserPermissions;
DELETE FROM UserRoles;
DELETE FROM Users;

-- Reset identity columns
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('UserRoles', RESEED, 0);
DBCC CHECKIDENT ('UserPermissions', RESEED, 0);
DBCC CHECKIDENT ('AutomationJobs', RESEED, 0);
DBCC CHECKIDENT ('TestExecutions', RESEED, 0);
DBCC CHECKIDENT ('WebAutomations', RESEED, 0);
DBCC CHECKIDENT ('JobSchedules', RESEED, 0);
DBCC CHECKIDENT ('MLModels', RESEED, 0);
DBCC CHECKIDENT ('AITrainingData', RESEED, 0);

-- =============================================
-- SEED USERS
-- =============================================

INSERT INTO Users (Username, Email, PasswordHash, Salt, Role, IsActive, IsEmailVerified, LastLoginAt, PasswordChangedAt, TwoFactorEnabled, CreatedAt) VALUES
('admin', 'admin@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'salt123', 'SuperAdmin', 1, 1, DATEADD(day, -1, GETUTCDATE()), GETUTCDATE(), 0, GETUTCDATE()),
('john.doe', 'john.doe@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'salt456', 'Admin', 1, 1, DATEADD(hour, -2, GETUTCDATE()), GETUTCDATE(), 1, GETUTCDATE()),
('jane.smith', 'jane.smith@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'salt789', 'Manager', 1, 1, DATEADD(hour, -4, GETUTCDATE()), GETUTCDATE(), 0, GETUTCDATE()),
('mike.wilson', 'mike.wilson@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'salt101', 'Developer', 1, 1, DATEADD(hour, -6, GETUTCDATE()), GETUTCDATE(), 0, GETUTCDATE()),
('sarah.jones', 'sarah.jones@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'salt202', 'Tester', 1, 1, DATEADD(hour, -8, GETUTCDATE()), GETUTCDATE(), 0, GETUTCDATE()),
('bob.brown', 'bob.brown@bosch.com', 'AQAAAAEAACcQAAAAEExampleHash', 'salt303', 'Viewer', 1, 0, NULL, GETUTCDATE(), 0, GETUTCDATE());

-- =============================================
-- SEED AUTOMATION JOBS
-- =============================================

INSERT INTO AutomationJobs (Name, Description, StatusId, JobTypeId, Configuration, ScheduledAt, StartedAt, CompletedAt, ExecutionTimeMs, RetryCount, MaxRetries, Priority, CreatedBy, CreatedAt) VALUES
('E-commerce Login Automation', 'Automated login testing for e-commerce platform', 1, 1, '{"browser": "Chrome", "timeout": 30, "headless": false, "viewport": "1920x1080"}', DATEADD(hour, 1, GETUTCDATE()), NULL, NULL, NULL, 0, 3, 5, 1, GETUTCDATE()),
('Data Processing Pipeline', 'Process customer data files from FTP server', 2, 2, '{"inputPath": "/data/input", "outputPath": "/data/output", "fileTypes": ["csv", "xlsx"], "batchSize": 1000}', GETUTCDATE(), GETUTCDATE(), NULL, 45000, 0, 3, 8, 1, GETUTCDATE()),
('Monthly Sales Report', 'Generate comprehensive monthly sales report', 3, 3, '{"template": "monthly_sales", "format": "PDF", "recipients": ["management@bosch.com"], "includeCharts": true}', DATEADD(day, -1, GETUTCDATE()), DATEADD(day, -1, GETUTCDATE()), DATEADD(day, -1, DATEADD(minute, 15, GETUTCDATE())), 900000, 0, 3, 7, 1, GETUTCDATE()),
('API Health Check', 'Monitor API endpoints for availability', 1, 6, '{"endpoints": ["/api/health", "/api/users", "/api/automationjobs"], "interval": 300, "timeout": 10}', DATEADD(minute, 30, GETUTCDATE()), NULL, NULL, NULL, 0, 5, 6, 1, GETUTCDATE()),
('Database Backup', 'Automated database backup and archival', 1, 2, '{"backupType": "full", "retentionDays": 30, "compression": true, "encryption": true}', DATEADD(hour, 2, GETUTCDATE()), NULL, NULL, NULL, 0, 2, 9, 1, GETUTCDATE()),
('Performance Test Suite', 'Execute comprehensive performance tests', 1, 4, '{"testType": "load", "users": 100, "duration": 3600, "rampUp": 300}', DATEADD(hour, 3, GETUTCDATE()), NULL, NULL, NULL, 0, 2, 4, 1, GETUTCDATE());

-- =============================================
-- SEED TEST EXECUTIONS
-- =============================================

INSERT INTO TestExecutions (TestName, TestTypeId, StatusId, TestSuite, TestData, ExpectedResult, ActualResult, ErrorMessage, ExecutionTimeMs, AIAnalysis, ConfidenceScore, TestEnvironment, Browser, Device, ScreenshotPath, VideoPath, LogPath, CreatedBy, CreatedAt) VALUES
('User Login Test', 1, 3, 'Authentication Suite', '{"username": "testuser@bosch.com", "password": "TestPass123", "rememberMe": true}', 'User successfully logged in and redirected to dashboard', 'User successfully logged in and redirected to dashboard', '', 2500, 'Login test passed with high confidence. User authentication flow working correctly.', 95.5, 'Test', 'Chrome', 'Desktop', '/screenshots/login_test_001.png', '/videos/login_test_001.mp4', '/logs/login_test_001.log', 1, GETUTCDATE()),
('API Endpoint Test', 2, 4, 'API Integration Suite', '{"endpoint": "/api/users", "method": "GET", "headers": {"Authorization": "Bearer token123"}}', 'Status 200 with user data', 'Status 500 - Internal Server Error', 'Database connection timeout', 5000, 'API test failed due to database connectivity issues. Recommend checking database connection pool.', 88.2, 'Test', 'Chrome', 'Desktop', '/screenshots/api_test_001.png', '/videos/api_test_001.mp4', '/logs/api_test_001.log', 1, GETUTCDATE()),
('UI Component Test', 6, 3, 'UI Test Suite', '{"page": "dashboard", "component": "navigation-menu", "action": "click"}', 'Navigation menu opens and displays all items', 'Navigation menu opens and displays all items', '', 1800, 'UI test passed. Navigation component functioning correctly with proper accessibility.', 92.8, 'Test', 'Chrome', 'Desktop', '/screenshots/ui_test_001.png', '/videos/ui_test_001.mp4', '/logs/ui_test_001.log', 1, GETUTCDATE()),
('Performance Load Test', 4, 2, 'Performance Suite', '{"concurrentUsers": 100, "duration": 300, "rampUp": 60}', 'Response time < 2 seconds, 99% uptime', 'Response time 1.8 seconds, 99.2% uptime', '', 300000, 'Performance test shows excellent results. System handling load well within acceptable parameters.', 96.1, 'Test', 'Chrome', 'Desktop', '/screenshots/perf_test_001.png', '/videos/perf_test_001.mp4', '/logs/perf_test_001.log', 1, GETUTCDATE()),
('Security Vulnerability Test', 5, 3, 'Security Suite', '{"testType": "sql_injection", "payload": "admin'' OR ''1''=''1"}', 'Request blocked with security error', 'Request blocked with security error', '', 3200, 'Security test passed. SQL injection protection working correctly.', 98.5, 'Test', 'Chrome', 'Desktop', '/screenshots/security_test_001.png', '/videos/security_test_001.mp4', '/logs/security_test_001.log', 1, GETUTCDATE());

-- =============================================
-- SEED WEB AUTOMATIONS
-- =============================================

INSERT INTO WebAutomations (AutomationName, WebsiteUrl, StatusId, JobTypeId, TargetElement, Action, InputData, OutputData, AIPrompt, AIResponse, ElementSelector, ErrorMessage, Browser, Device, UserAgent, ViewportSize, ConfidenceScore, ExecutionTimeMs, CreatedBy, CreatedAt) VALUES
('E-commerce Product Search', 'https://shop.bosch.com', 3, 1, 'input[type="search"]', 'Fill', '{"searchTerm": "power tools", "category": "tools"}', '{"results": 25, "products": ["Drill", "Saw", "Sander"]}', 'Search for power tools and extract product information', 'Successfully found 25 power tool products. Top results include professional drill, circular saw, and orbital sander.', '#search-input', '', 'Chrome', 'Desktop', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '1920x1080', 94.2, 8500, 1, GETUTCDATE()),
('Form Data Entry', 'https://forms.bosch.com/contact', 2, 1, 'form', 'Fill', '{"name": "John Doe", "email": "john@example.com", "message": "Inquiry about products"}', '{"status": "submitted", "confirmationId": "CF-12345"}', 'Fill contact form with customer inquiry', 'Form submitted successfully. Customer inquiry recorded with confirmation ID CF-12345.', 'form.contact-form', '', 'Chrome', 'Desktop', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '1920x1080', 91.8, 4200, 1, GETUTCDATE()),
('Social Media Post', 'https://linkedin.com', 1, 1, 'div[data-test="post-composer"]', 'Post', '{"content": "Excited about our new automation platform!", "hashtags": ["#automation", "#innovation"]}', '{"postId": "123456789", "engagement": "likes: 15, comments: 3"}', 'Create professional LinkedIn post about automation platform', 'Post created successfully with good initial engagement. Content optimized for professional audience.', '.post-composer textarea', '', 'Chrome', 'Desktop', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '1920x1080', 89.5, 6800, 1, GETUTCDATE()),
('Data Extraction', 'https://reports.bosch.com/analytics', 3, 1, 'table.data-table', 'Extract', '{"columns": ["date", "revenue", "orders"], "format": "csv"}', '{"rows": 150, "filePath": "/exports/analytics_2024.csv"}', 'Extract analytics data from reports table', 'Successfully extracted 150 rows of analytics data. Data includes revenue and order information for Q1 2024.', 'table.analytics-table', '', 'Chrome', 'Desktop', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '1920x1080', 96.7, 12000, 1, GETUTCDATE());

-- =============================================
-- SEED JOB SCHEDULES
-- =============================================

INSERT INTO JobSchedules (JobName, JobTypeId, StatusId, CronExpression, Configuration, Priority, TimeZone, IsEnabled, NextRunTime, LastRunTime, LastRunStatus, ExecutionHistory, Notifications, Dependencies, MaxExecutionTime, RetryPolicy, CreatedBy, CreatedAt) VALUES
('Daily System Health Check', 6, 1, '0 9 * * *', '{"endpoints": ["/health", "/api/status"], "timeout": 30, "retries": 3}', 8, 'UTC', 1, DATEADD(day, 1, CAST(GETDATE() AS date)), DATEADD(day, -1, GETUTCDATE()), 'Completed', '{"runs": 30, "successRate": 96.7, "avgDuration": 45}', '{"email": ["admin@bosch.com"], "slack": ["#alerts"]}', '[]', 60, '{"maxRetries": 3, "backoffMultiplier": 2}', 1, GETUTCDATE()),
('Weekly Performance Report', 3, 1, '0 8 * * 1', '{"template": "weekly_performance", "recipients": ["management@bosch.com"], "includeCharts": true}', 7, 'UTC', 1, DATEADD(week, 1, CAST(GETDATE() AS date)), DATEADD(week, -1, GETUTCDATE()), 'Completed', '{"runs": 12, "successRate": 100, "avgDuration": 300}', '{"email": ["management@bosch.com"]}', '["daily_health_check"]', 180, '{"maxRetries": 2, "backoffMultiplier": 1.5}', 1, GETUTCDATE()),
('Monthly Data Archive', 2, 1, '0 2 1 * *', '{"sourcePath": "/data/current", "archivePath": "/data/archive", "retentionDays": 365}', 9, 'UTC', 1, DATEADD(month, 1, CAST(GETDATE() AS date)), DATEADD(month, -1, GETUTCDATE()), 'Completed', '{"runs": 6, "successRate": 100, "avgDuration": 1800}', '{"email": ["admin@bosch.com"]}', '[]', 240, '{"maxRetries": 1, "backoffMultiplier": 1}', 1, GETUTCDATE()),
('Hourly API Monitoring', 6, 1, '0 * * * *', '{"endpoints": ["/api/health", "/api/automationjobs", "/api/users"], "timeout": 10}', 6, 'UTC', 1, DATEADD(hour, 1, GETUTCDATE()), GETUTCDATE(), 'Running', '{"runs": 720, "successRate": 99.2, "avgDuration": 5}', '{"slack": ["#monitoring"]}', '[]', 15, '{"maxRetries": 2, "backoffMultiplier": 1.5}', 1, GETUTCDATE()),
('Quarterly Security Scan', 5, 1, '0 0 1 */3 *', '{"scanType": "vulnerability", "targets": ["web", "api", "database"], "severity": "medium"}', 10, 'UTC', 1, DATEADD(month, 3, CAST(GETDATE() AS date)), DATEADD(month, -3, GETUTCDATE()), 'Completed', '{"runs": 2, "successRate": 100, "avgDuration": 3600}', '{"email": ["security@bosch.com"], "slack": ["#security"]}', '[]', 480, '{"maxRetries": 1, "backoffMultiplier": 1}', 1, GETUTCDATE());

-- =============================================
-- SEED ML MODELS
-- =============================================

INSERT INTO MLModels (ModelName, ModelType, Version, StatusId, Accuracy, TrainingData, ModelPath, Configuration, PerformanceMetrics, IsActive, CreatedBy, CreatedAt) VALUES
('Test Result Predictor', 'Classification', '1.2.0', 1, 94.5, '{"samples": 10000, "features": 25, "distribution": "balanced"}', '/models/test_predictor_v1.2.pkl', '{"algorithm": "RandomForest", "maxDepth": 10, "nEstimators": 100}', '{"precision": 0.945, "recall": 0.938, "f1Score": 0.941, "auc": 0.967}', 1, 1, GETUTCDATE()),
('Performance Anomaly Detector', 'Anomaly Detection', '2.1.0', 1, 89.2, '{"samples": 50000, "features": 15, "anomalyRate": 0.05}', '/models/anomaly_detector_v2.1.pkl', '{"algorithm": "IsolationForest", "contamination": 0.05, "nEstimators": 200}', '{"precision": 0.892, "recall": 0.876, "f1Score": 0.884, "falsePositiveRate": 0.023}', 1, 1, GETUTCDATE()),
('User Behavior Classifier', 'Classification', '1.0.0', 1, 91.8, '{"samples": 25000, "features": 30, "classes": 5}', '/models/user_behavior_v1.0.pkl', '{"algorithm": "GradientBoosting", "learningRate": 0.1, "maxDepth": 6}', '{"precision": 0.918, "recall": 0.905, "f1Score": 0.911, "auc": 0.952}', 0, 1, GETUTCDATE()),
('Resource Usage Predictor', 'Regression', '1.5.0', 1, 87.3, '{"samples": 30000, "features": 20, "target": "continuous"}', '/models/resource_predictor_v1.5.pkl', '{"algorithm": "XGBoost", "learningRate": 0.05, "maxDepth": 8, "nEstimators": 500}', '{"mse": 0.127, "mae": 0.089, "r2Score": 0.873, "rmse": 0.356}', 1, 1, GETUTCDATE());

-- =============================================
-- SEED AI TRAINING DATA
-- =============================================

INSERT INTO AITrainingData (DataType, Content, Label, Category, QualityScore, Source, ProcessedAt, IsProcessed, CreatedBy, CreatedAt) VALUES
('Test Execution', '{"testName": "Login Test", "result": "Passed", "duration": 2500, "browser": "Chrome"}', 'Passed', 'Functional Testing', 0.95, 'Automated Test Runner', GETUTCDATE(), 1, 1, GETUTCDATE()),
('Performance Metric', '{"metric": "Response Time", "value": 1.8, "threshold": 2.0, "status": "Good"}', 'Good', 'Performance', 0.92, 'Performance Monitor', GETUTCDATE(), 1, 1, GETUTCDATE()),
('Error Log', '{"error": "Database connection timeout", "severity": "High", "component": "API"}', 'High Severity', 'Error Analysis', 0.88, 'Error Logger', GETUTCDATE(), 1, 1, GETUTCDATE()),
('User Interaction', '{"action": "Click", "element": "Submit Button", "success": true, "duration": 1200}', 'Successful', 'User Behavior', 0.91, 'User Analytics', GETUTCDATE(), 1, 1, GETUTCDATE()),
('System Event', '{"event": "Job Completed", "jobId": 12345, "status": "Success", "duration": 45000}', 'Success', 'System Events', 0.94, 'Job Scheduler', GETUTCDATE(), 1, 1, GETUTCDATE());

-- =============================================
-- SEED PERFORMANCE METRICS
-- =============================================

INSERT INTO PerformanceMetrics (MetricName, MetricValue, MetricUnit, Category, Timestamp, Tags) VALUES
('CPU Usage', 68.5, 'percent', 'System', GETUTCDATE(), '{"server": "web-01", "environment": "production"}'),
('Memory Usage', 72.3, 'percent', 'System', GETUTCDATE(), '{"server": "web-01", "environment": "production"}'),
('Disk Usage', 45.8, 'percent', 'System', GETUTCDATE(), '{"server": "web-01", "environment": "production"}'),
('Network Latency', 12.5, 'milliseconds', 'Network', GETUTCDATE(), '{"endpoint": "/api/health", "region": "us-east"}'),
('Response Time', 145.2, 'milliseconds', 'API', GETUTCDATE(), '{"endpoint": "/api/automationjobs", "method": "GET"}'),
('Error Rate', 2.1, 'percent', 'Quality', GETUTCDATE(), '{"component": "API", "severity": "error"}'),
('Throughput', 1250.5, 'requests per second', 'Performance', GETUTCDATE(), '{"endpoint": "/api/users", "method": "GET"}'),
('Active Users', 342, 'count', 'Usage', GETUTCDATE(), '{"timeframe": "current", "type": "concurrent"}');

-- =============================================
-- SEED JOB EXECUTION HISTORY
-- =============================================

INSERT INTO JobExecutionHistory (JobId, JobType, Status, StartedAt, CompletedAt, ExecutionTimeMs, ErrorMessage, ResourceUsage, Timestamp) VALUES
(1, 'AutomationJob', 'Completed', DATEADD(hour, -2, GETUTCDATE()), DATEADD(hour, -1, GETUTCDATE()), 45000, NULL, '{"cpu": 25.5, "memory": 128, "disk": 50}', GETUTCDATE()),
(2, 'AutomationJob', 'Running', DATEADD(hour, -1, GETUTCDATE()), NULL, NULL, NULL, '{"cpu": 45.2, "memory": 256, "disk": 100}', GETUTCDATE()),
(3, 'AutomationJob', 'Completed', DATEADD(day, -1, GETUTCDATE()), DATEADD(day, -1, DATEADD(minute, 15, GETUTCDATE())), 900000, NULL, '{"cpu": 60.8, "memory": 512, "disk": 200}', GETUTCDATE()),
(4, 'TestExecution', 'Completed', DATEADD(hour, -3, GETUTCDATE()), DATEADD(hour, -2, GETUTCDATE()), 2500, NULL, '{"cpu": 15.2, "memory": 64, "disk": 25}', GETUTCDATE()),
(5, 'TestExecution', 'Failed', DATEADD(hour, -4, GETUTCDATE()), DATEADD(hour, -3, GETUTCDATE()), 5000, 'Database connection timeout', '{"cpu": 30.1, "memory": 128, "disk": 50}', GETUTCDATE());

-- =============================================
-- SEED AUDIT LOGS
-- =============================================

INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, OldValues, NewValues, IpAddress, UserAgent, Timestamp) VALUES
(1, 'CREATE', 'AutomationJob', 1, NULL, '{"name": "E-commerce Login Automation", "status": "Pending"}', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', GETUTCDATE()),
(1, 'UPDATE', 'AutomationJob', 1, '{"status": "Pending"}', '{"status": "Running"}', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', GETUTCDATE()),
(2, 'CREATE', 'TestExecution', 1, NULL, '{"testName": "User Login Test", "status": "Pending"}', '192.168.1.101', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36', GETUTCDATE()),
(3, 'LOGIN', 'User', 3, NULL, '{"lastLoginAt": "' + CONVERT(varchar, GETUTCDATE(), 126) + '"}', '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', GETUTCDATE());

-- =============================================
-- SEED SYSTEM LOGS
-- =============================================

INSERT INTO SystemLogs (Level, Message, Exception, Source, UserId, IpAddress, Timestamp) VALUES
('INFO', 'User john.doe logged in successfully', NULL, 'AuthenticationService', 2, '192.168.1.101', GETUTCDATE()),
('WARN', 'High memory usage detected on server web-01', NULL, 'SystemMonitor', NULL, '192.168.1.1', GETUTCDATE()),
('ERROR', 'Database connection timeout in AutomationJob execution', 'System.Data.SqlClient.SqlException: Timeout expired', 'AutomationJobService', NULL, '192.168.1.1', GETUTCDATE()),
('INFO', 'Test execution completed successfully', NULL, 'TestExecutionService', 1, '192.168.1.100', GETUTCDATE()),
('DEBUG', 'Performance metrics collected', NULL, 'PerformanceMonitor', NULL, '192.168.1.1', GETUTCDATE());

PRINT 'Comprehensive seed data inserted successfully!';
PRINT 'Database is now ready for production use with realistic data.';
