-- Advanced Database Schema for Bosch Industrial Automation Platform
-- Enterprise-grade database design with optimization and advanced features

USE IndustrialAutomationDb;
GO

-- =============================================
-- CREATE ADVANCED INDEXES FOR PERFORMANCE
-- =============================================

-- Composite indexes for common query patterns
CREATE NONCLUSTERED INDEX IX_AutomationJobs_StatusId_JobTypeId_CreatedAt 
ON AutomationJobs (StatusId, JobTypeId, CreatedAt DESC)
INCLUDE (Name, Description, Priority, ExecutionTimeMs);

CREATE NONCLUSTERED INDEX IX_TestExecutions_StatusId_TestTypeId_ExecutedAt 
ON TestExecutions (StatusId, TestTypeId, ExecutedAt DESC)
INCLUDE (TestName, TestSuite, ExecutionTimeMs, ConfidenceScore);

CREATE NONCLUSTERED INDEX IX_WebAutomations_StatusId_JobTypeId_CreatedAt 
ON WebAutomations (StatusId, JobTypeId, CreatedAt DESC)
INCLUDE (AutomationName, WebsiteUrl, ExecutionTimeMs);

CREATE NONCLUSTERED INDEX IX_JobSchedules_IsEnabled_NextRunTime_StatusId 
ON JobSchedules (IsEnabled, NextRunTime, StatusId)
INCLUDE (JobName, JobTypeId, Priority);

-- Covering indexes for frequently accessed columns
CREATE NONCLUSTERED INDEX IX_Users_Email_IsActive 
ON Users (Email, IsActive)
INCLUDE (Username, Role, LastLoginAt);

CREATE NONCLUSTERED INDEX IX_Users_Username_IsActive 
ON Users (Username, IsActive)
INCLUDE (Email, Role, CreatedAt);

-- Performance indexes for audit and logging
CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId_Action_Timestamp 
ON AuditLogs (UserId, Action, Timestamp DESC)
INCLUDE (EntityType, EntityId);

CREATE NONCLUSTERED INDEX IX_SystemLogs_Level_Timestamp 
ON SystemLogs (Level, Timestamp DESC)
INCLUDE (Message, Source, UserId);

-- =============================================
-- CREATE PARTITIONED TABLES FOR LARGE DATA
-- =============================================

-- Partition function for time-based partitioning
CREATE PARTITION FUNCTION PF_LogsByMonth (datetime2)
AS RANGE RIGHT FOR VALUES (
    '2024-01-01', '2024-02-01', '2024-03-01', '2024-04-01',
    '2024-05-01', '2024-06-01', '2024-07-01', '2024-08-01',
    '2024-09-01', '2024-10-01', '2024-11-01', '2024-12-01',
    '2025-01-01', '2025-02-01', '2025-03-01', '2025-04-01',
    '2025-05-01', '2025-06-01', '2025-07-01', '2025-08-01',
    '2025-09-01', '2025-10-01', '2025-11-01', '2025-12-01'
);

-- Partition scheme
CREATE PARTITION SCHEME PS_LogsByMonth
AS PARTITION PF_LogsByMonth
ALL TO ([PRIMARY]);

-- Create partitioned system logs table
CREATE TABLE SystemLogsPartitioned (
    Id bigint IDENTITY(1,1) NOT NULL,
    Level nvarchar(20) NOT NULL,
    Message nvarchar(max) NOT NULL,
    Exception nvarchar(max) NULL,
    Source nvarchar(200) NULL,
    UserId int NULL,
    IpAddress nvarchar(45) NULL,
    Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_SystemLogsPartitioned PRIMARY KEY (Id, Timestamp)
) ON PS_LogsByMonth(Timestamp);

-- =============================================
-- CREATE ADVANCED STORED PROCEDURES
-- =============================================

-- Stored procedure for bulk operations
CREATE PROCEDURE sp_BulkUpdateAutomationJobs
    @JobUpdates NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UpdatedCount INT = 0;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Parse JSON and update jobs
        UPDATE aj
        SET 
            StatusId = j.StatusId,
            UpdatedAt = GETUTCDATE()
        FROM AutomationJobs aj
        INNER JOIN OPENJSON(@JobUpdates) WITH (
            Id INT '$.Id',
            StatusId INT '$.StatusId'
        ) j ON aj.Id = j.Id;
        
        SET @UpdatedCount = @@ROWCOUNT;
        
        COMMIT TRANSACTION;
        
        SELECT @UpdatedCount AS UpdatedCount;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Stored procedure for performance analytics
CREATE PROCEDURE sp_GetPerformanceAnalytics
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @JobTypeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        jt.JobTypeName,
        st.StatusName,
        COUNT(*) AS JobCount,
        AVG(aj.ExecutionTimeMs) AS AvgExecutionTime,
        MAX(aj.ExecutionTimeMs) AS MaxExecutionTime,
        MIN(aj.ExecutionTimeMs) AS MinExecutionTime,
        SUM(CASE WHEN aj.StatusId = 3 THEN 1 ELSE 0 END) AS SuccessCount,
        SUM(CASE WHEN aj.StatusId = 4 THEN 1 ELSE 0 END) AS FailureCount,
        CAST(SUM(CASE WHEN aj.StatusId = 3 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS SuccessRate
    FROM AutomationJobs aj
    INNER JOIN JobTypes jt ON aj.JobTypeId = jt.Id
    INNER JOIN StatusTypes st ON aj.StatusId = st.Id
    WHERE aj.CreatedAt BETWEEN @StartDate AND @EndDate
        AND (@JobTypeId IS NULL OR aj.JobTypeId = @JobTypeId)
        AND aj.IsDeleted = 0
    GROUP BY jt.JobTypeName, st.StatusName
    ORDER BY JobCount DESC;
END;
GO

-- Stored procedure for cleanup operations
CREATE PROCEDURE sp_CleanupOldData
    @RetentionDays INT = 90,
    @BatchSize INT = 1000
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DeletedCount INT = 0;
    DECLARE @TotalDeleted INT = 0;
    DECLARE @CutoffDate DATETIME2 = DATEADD(DAY, -@RetentionDays, GETUTCDATE());
    
    -- Clean up old system logs
    WHILE 1 = 1
    BEGIN
        DELETE TOP (@BatchSize) 
        FROM SystemLogs 
        WHERE Timestamp < @CutoffDate;
        
        SET @DeletedCount = @@ROWCOUNT;
        SET @TotalDeleted = @TotalDeleted + @DeletedCount;
        
        IF @DeletedCount < @BatchSize
            BREAK;
            
        WAITFOR DELAY '00:00:01'; -- Prevent blocking
    END
    
    -- Clean up old audit logs
    WHILE 1 = 1
    BEGIN
        DELETE TOP (@BatchSize) 
        FROM AuditLogs 
        WHERE Timestamp < @CutoffDate;
        
        SET @DeletedCount = @@ROWCOUNT;
        SET @TotalDeleted = @TotalDeleted + @DeletedCount;
        
        IF @DeletedCount < @BatchSize
            BREAK;
            
        WAITFOR DELAY '00:00:01';
    END
    
    SELECT @TotalDeleted AS TotalRecordsDeleted;
END;
GO

-- =============================================
-- CREATE ADVANCED FUNCTIONS
-- =============================================

-- Function to calculate job success rate
CREATE FUNCTION fn_CalculateJobSuccessRate(@JobTypeId INT, @Days INT = 30)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @SuccessRate DECIMAL(5,2);
    
    SELECT @SuccessRate = CAST(
        SUM(CASE WHEN StatusId = 3 THEN 1 ELSE 0 END) * 100.0 / COUNT(*)
        AS DECIMAL(5,2)
    )
    FROM AutomationJobs
    WHERE JobTypeId = @JobTypeId
        AND CreatedAt >= DATEADD(DAY, -@Days, GETUTCDATE())
        AND IsDeleted = 0;
    
    RETURN ISNULL(@SuccessRate, 0);
END;
GO

-- Function to get job performance metrics
CREATE FUNCTION fn_GetJobPerformanceMetrics(@JobId INT)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        aj.Id,
        aj.Name,
        aj.ExecutionTimeMs,
        aj.StatusId,
        st.StatusName,
        CASE 
            WHEN aj.ExecutionTimeMs < 1000 THEN 'Fast'
            WHEN aj.ExecutionTimeMs < 5000 THEN 'Medium'
            WHEN aj.ExecutionTimeMs < 10000 THEN 'Slow'
            ELSE 'Very Slow'
        END AS PerformanceCategory,
        CASE 
            WHEN aj.StatusId = 3 THEN 'Success'
            WHEN aj.StatusId = 4 THEN 'Failed'
            ELSE 'In Progress'
        END AS Result
    FROM AutomationJobs aj
    INNER JOIN StatusTypes st ON aj.StatusId = st.Id
    WHERE aj.Id = @JobId
);
GO

-- =============================================
-- CREATE ADVANCED TRIGGERS
-- =============================================

-- Trigger for audit trail on AutomationJobs
CREATE TRIGGER tr_AutomationJobs_Audit
ON AutomationJobs
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Action NVARCHAR(10);
    DECLARE @UserId INT = ISNULL(CAST(SESSION_CONTEXT(N'UserId') AS INT), 0);
    
    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @Action = 'UPDATE';
    ELSE IF EXISTS(SELECT * FROM inserted)
        SET @Action = 'INSERT';
    ELSE
        SET @Action = 'DELETE';
    
    -- Log audit information
    INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, OldValues, NewValues, Timestamp)
    SELECT 
        @UserId,
        @Action,
        'AutomationJob',
        ISNULL(i.Id, d.Id),
        CASE WHEN @Action IN ('UPDATE', 'DELETE') THEN 
            (SELECT d.* FOR JSON AUTO) 
        END,
        CASE WHEN @Action IN ('INSERT', 'UPDATE') THEN 
            (SELECT i.* FOR JSON AUTO) 
        END,
        GETUTCDATE()
    FROM inserted i
    FULL OUTER JOIN deleted d ON i.Id = d.Id;
END;
GO

-- Trigger for updating job statistics
CREATE TRIGGER tr_AutomationJobs_UpdateStats
ON AutomationJobs
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update performance metrics when job completes
    IF UPDATE(StatusId)
    BEGIN
        UPDATE aj
        SET ExecutionTimeMs = DATEDIFF(MILLISECOND, aj.StartedAt, aj.CompletedAt)
        FROM AutomationJobs aj
        INNER JOIN inserted i ON aj.Id = i.Id
        WHERE i.StatusId = 3 -- Completed
            AND aj.StartedAt IS NOT NULL
            AND aj.CompletedAt IS NOT NULL;
    END
END;
GO

-- =============================================
-- CREATE ADVANCED VIEWS
-- =============================================

-- View for dashboard KPIs
CREATE VIEW vw_DashboardKPIs
AS
SELECT 
    (SELECT COUNT(*) FROM AutomationJobs WHERE IsDeleted = 0) AS TotalJobs,
    (SELECT COUNT(*) FROM AutomationJobs WHERE StatusId = 2 AND IsDeleted = 0) AS RunningJobs,
    (SELECT COUNT(*) FROM AutomationJobs WHERE StatusId = 3 AND IsDeleted = 0) AS CompletedJobs,
    (SELECT COUNT(*) FROM AutomationJobs WHERE StatusId = 4 AND IsDeleted = 0) AS FailedJobs,
    (SELECT AVG(ExecutionTimeMs) FROM AutomationJobs WHERE StatusId = 3 AND IsDeleted = 0) AS AvgExecutionTime,
    (SELECT COUNT(*) FROM Users WHERE IsActive = 1) AS ActiveUsers,
    (SELECT COUNT(*) FROM JobSchedules WHERE IsEnabled = 1) AS ActiveSchedules;
GO

-- View for job performance analysis
CREATE VIEW vw_JobPerformanceAnalysis
AS
SELECT 
    aj.Id,
    aj.Name,
    jt.JobTypeName,
    st.StatusName,
    aj.ExecutionTimeMs,
    aj.CreatedAt,
    aj.StartedAt,
    aj.CompletedAt,
    DATEDIFF(MILLISECOND, aj.StartedAt, aj.CompletedAt) AS ActualExecutionTime,
    aj.RetryCount,
    aj.MaxRetries,
    CASE 
        WHEN aj.StatusId = 3 THEN 'Success'
        WHEN aj.StatusId = 4 THEN 'Failed'
        WHEN aj.StatusId = 2 THEN 'Running'
        ELSE 'Pending'
    END AS Result,
    CASE 
        WHEN aj.ExecutionTimeMs < 1000 THEN 'Fast'
        WHEN aj.ExecutionTimeMs < 5000 THEN 'Medium'
        WHEN aj.ExecutionTimeMs < 10000 THEN 'Slow'
        ELSE 'Very Slow'
    END AS PerformanceCategory
FROM AutomationJobs aj
INNER JOIN JobTypes jt ON aj.JobTypeId = jt.Id
INNER JOIN StatusTypes st ON aj.StatusId = st.Id
WHERE aj.IsDeleted = 0;
GO

-- View for system health monitoring
CREATE VIEW vw_SystemHealth
AS
SELECT 
    'Database' AS Component,
    'Online' AS Status,
    GETUTCDATE() AS LastCheck,
    (SELECT COUNT(*) FROM AutomationJobs WHERE IsDeleted = 0) AS MetricValue
UNION ALL
SELECT 
    'Active Jobs',
    CASE WHEN COUNT(*) > 0 THEN 'Active' ELSE 'Idle' END,
    MAX(UpdatedAt),
    COUNT(*)
FROM AutomationJobs 
WHERE StatusId = 2 AND IsDeleted = 0
UNION ALL
SELECT 
    'Failed Jobs (24h)',
    CASE WHEN COUNT(*) > 10 THEN 'Warning' ELSE 'Normal' END,
    MAX(CreatedAt),
    COUNT(*)
FROM AutomationJobs 
WHERE StatusId = 4 
    AND CreatedAt >= DATEADD(HOUR, -24, GETUTCDATE())
    AND IsDeleted = 0;
GO

-- =============================================
-- CREATE ADVANCED SECURITY FEATURES
-- =============================================

-- Create database roles for different access levels
CREATE ROLE db_automation_admin;
CREATE ROLE db_automation_operator;
CREATE ROLE db_automation_viewer;

-- Grant permissions to roles
GRANT SELECT, INSERT, UPDATE, DELETE ON AutomationJobs TO db_automation_admin;
GRANT SELECT, INSERT, UPDATE ON AutomationJobs TO db_automation_operator;
GRANT SELECT ON AutomationJobs TO db_automation_viewer;

GRANT SELECT, INSERT, UPDATE, DELETE ON TestExecutions TO db_automation_admin;
GRANT SELECT, INSERT, UPDATE ON TestExecutions TO db_automation_operator;
GRANT SELECT ON TestExecutions TO db_automation_viewer;

GRANT SELECT, INSERT, UPDATE, DELETE ON WebAutomations TO db_automation_admin;
GRANT SELECT, INSERT, UPDATE ON WebAutomations TO db_automation_operator;
GRANT SELECT ON WebAutomations TO db_automation_viewer;

-- Grant execute permissions on stored procedures
GRANT EXECUTE ON sp_BulkUpdateAutomationJobs TO db_automation_admin;
GRANT EXECUTE ON sp_GetPerformanceAnalytics TO db_automation_operator;
GRANT EXECUTE ON sp_GetPerformanceAnalytics TO db_automation_viewer;

-- =============================================
-- CREATE DATA COMPRESSION
-- =============================================

-- Enable data compression on large tables
ALTER TABLE SystemLogs REBUILD WITH (DATA_COMPRESSION = PAGE);
ALTER TABLE AuditLogs REBUILD WITH (DATA_COMPRESSION = PAGE);
ALTER TABLE PerformanceMetrics REBUILD WITH (DATA_COMPRESSION = PAGE);

-- =============================================
-- CREATE MAINTENANCE PROCEDURES
-- =============================================

-- Procedure for index maintenance
CREATE PROCEDURE sp_MaintainIndexes
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Rebuild fragmented indexes
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @TableName NVARCHAR(128);
    DECLARE @IndexName NVARCHAR(128);
    DECLARE @Fragmentation FLOAT;
    
    DECLARE index_cursor CURSOR FOR
    SELECT 
        OBJECT_NAME(ips.object_id) AS TableName,
        i.name AS IndexName,
        ips.avg_fragmentation_in_percent
    FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
    INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
    WHERE ips.avg_fragmentation_in_percent > 30
        AND i.name IS NOT NULL;
    
    OPEN index_cursor;
    FETCH NEXT FROM index_cursor INTO @TableName, @IndexName, @Fragmentation;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER INDEX ' + @IndexName + ' ON ' + @TableName + ' REBUILD';
        EXEC sp_executesql @sql;
        
        FETCH NEXT FROM index_cursor INTO @TableName, @IndexName, @Fragmentation;
    END
    
    CLOSE index_cursor;
    DEALLOCATE index_cursor;
END;
GO

-- =============================================
-- CREATE MONITORING AND ALERTING
-- =============================================

-- Procedure for health check
CREATE PROCEDURE sp_HealthCheck
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @HealthStatus NVARCHAR(20) = 'Healthy';
    DECLARE @Issues NVARCHAR(MAX) = '';
    
    -- Check for failed jobs in last hour
    IF EXISTS (SELECT 1 FROM AutomationJobs WHERE StatusId = 4 AND CreatedAt >= DATEADD(HOUR, -1, GETUTCDATE()))
    BEGIN
        SET @HealthStatus = 'Warning';
        SET @Issues = @Issues + 'Failed jobs detected in last hour. ';
    END
    
    -- Check for long-running jobs
    IF EXISTS (SELECT 1 FROM AutomationJobs WHERE StatusId = 2 AND StartedAt < DATEADD(HOUR, -2, GETUTCDATE()))
    BEGIN
        SET @HealthStatus = 'Warning';
        SET @Issues = @Issues + 'Long-running jobs detected. ';
    END
    
    -- Check database size
    DECLARE @DbSizeMB FLOAT;
    SELECT @DbSizeMB = SUM(CAST(FILEPROPERTY(name, 'SpaceUsed') AS FLOAT) * 8 / 1024)
    FROM sys.database_files;
    
    IF @DbSizeMB > 10000 -- 10GB
    BEGIN
        SET @HealthStatus = 'Warning';
        SET @Issues = @Issues + 'Database size exceeds 10GB. ';
    END
    
    SELECT 
        @HealthStatus AS HealthStatus,
        @Issues AS Issues,
        @DbSizeMB AS DatabaseSizeMB,
        GETUTCDATE() AS CheckTime;
END;
GO

-- =============================================
-- CREATE BACKUP AND RECOVERY PROCEDURES
-- =============================================

-- Procedure for automated backup
CREATE PROCEDURE sp_CreateBackup
    @BackupPath NVARCHAR(500) = 'C:\Backups\'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @BackupFile NVARCHAR(500);
    DECLARE @BackupName NVARCHAR(100);
    
    SET @BackupName = 'IndustrialAutomationDb_' + FORMAT(GETUTCDATE(), 'yyyyMMdd_HHmmss');
    SET @BackupFile = @BackupPath + @BackupName + '.bak';
    
    -- Create full backup
    BACKUP DATABASE IndustrialAutomationDb
    TO DISK = @BackupFile
    WITH FORMAT, INIT, COMPRESSION;
    
    -- Create transaction log backup
    SET @BackupFile = @BackupPath + @BackupName + '_Log.trn';
    BACKUP LOG IndustrialAutomationDb
    TO DISK = @BackupFile
    WITH FORMAT, INIT, COMPRESSION;
    
    SELECT @BackupName AS BackupName, @BackupFile AS BackupFile;
END;
GO

-- =============================================
-- CREATE PERFORMANCE MONITORING
-- =============================================

-- Procedure to get performance statistics
CREATE PROCEDURE sp_GetPerformanceStats
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get table sizes
    SELECT 
        t.name AS TableName,
        s.name AS SchemaName,
        p.rows AS RowCounts,
        CAST(ROUND(((SUM(a.total_pages) * 8) / 1024.00), 2) AS NUMERIC(36, 2)) AS TotalSpaceMB,
        CAST(ROUND(((SUM(a.used_pages) * 8) / 1024.00), 2) AS NUMERIC(36, 2)) AS UsedSpaceMB,
        CAST(ROUND(((SUM(a.total_pages) - SUM(a.used_pages)) * 8) / 1024.00, 2) AS NUMERIC(36, 2)) AS UnusedSpaceMB
    FROM sys.tables t
    INNER JOIN sys.indexes i ON t.object_id = i.object_id
    INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
    INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
    LEFT OUTER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE t.name NOT LIKE 'dt%' 
        AND t.is_ms_shipped = 0
        AND i.object_id > 255
    GROUP BY t.name, s.name, p.rows
    ORDER BY TotalSpaceMB DESC;
    
    -- Get index usage statistics
    SELECT 
        OBJECT_NAME(ius.object_id) AS TableName,
        i.name AS IndexName,
        ius.user_seeks,
        ius.user_scans,
        ius.user_lookups,
        ius.user_updates,
        ius.last_user_seek,
        ius.last_user_scan,
        ius.last_user_lookup
    FROM sys.dm_db_index_usage_stats ius
    INNER JOIN sys.indexes i ON ius.object_id = i.object_id AND ius.index_id = i.index_id
    WHERE ius.database_id = DB_ID()
    ORDER BY ius.user_seeks + ius.user_scans + ius.user_lookups DESC;
END;
GO

-- =============================================
-- CREATE DATA VALIDATION PROCEDURES
-- =============================================

-- Procedure to validate data integrity
CREATE PROCEDURE sp_ValidateDataIntegrity
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Issues TABLE (Issue NVARCHAR(MAX));
    
    -- Check for orphaned records
    INSERT INTO @Issues
    SELECT 'Orphaned AutomationJob records: ' + CAST(COUNT(*) AS NVARCHAR(10))
    FROM AutomationJobs aj
    LEFT JOIN JobTypes jt ON aj.JobTypeId = jt.Id
    WHERE jt.Id IS NULL;
    
    INSERT INTO @Issues
    SELECT 'Orphaned TestExecution records: ' + CAST(COUNT(*) AS NVARCHAR(10))
    FROM TestExecutions te
    LEFT JOIN TestTypes tt ON te.TestTypeId = tt.Id
    WHERE tt.Id IS NULL;
    
    -- Check for invalid status transitions
    INSERT INTO @Issues
    SELECT 'Invalid status transitions detected: ' + CAST(COUNT(*) AS NVARCHAR(10))
    FROM AutomationJobs aj
    WHERE aj.StatusId NOT IN (1, 2, 3, 4, 5);
    
    -- Return validation results
    SELECT 
        CASE WHEN COUNT(*) = 0 THEN 'All data integrity checks passed' 
             ELSE 'Data integrity issues found: ' + CAST(COUNT(*) AS NVARCHAR(10))
        END AS ValidationResult
    FROM @Issues;
    
    SELECT * FROM @Issues;
END;
GO

-- =============================================
-- CREATE SCHEDULED MAINTENANCE JOBS
-- =============================================

-- Create SQL Server Agent job for index maintenance
EXEC msdb.dbo.sp_add_job
    @job_name = 'IndustrialAutomation_IndexMaintenance',
    @description = 'Maintain indexes for Industrial Automation Platform';

EXEC msdb.dbo.sp_add_jobstep
    @job_name = 'IndustrialAutomation_IndexMaintenance',
    @step_name = 'Rebuild Fragmented Indexes',
    @command = 'EXEC IndustrialAutomationDb.dbo.sp_MaintainIndexes',
    @database_name = 'IndustrialAutomationDb';

EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'Weekly_IndexMaintenance',
    @freq_type = 8, -- Weekly
    @freq_interval = 1, -- Sunday
    @freq_recurrence_factor = 1,
    @active_start_time = 020000; -- 2:00 AM

EXEC msdb.dbo.sp_attach_schedule
    @job_name = 'IndustrialAutomation_IndexMaintenance',
    @schedule_name = 'Weekly_IndexMaintenance';

-- Create job for data cleanup
EXEC msdb.dbo.sp_add_job
    @job_name = 'IndustrialAutomation_DataCleanup',
    @description = 'Clean up old data for Industrial Automation Platform';

EXEC msdb.dbo.sp_add_jobstep
    @job_name = 'IndustrialAutomation_DataCleanup',
    @step_name = 'Cleanup Old Data',
    @command = 'EXEC IndustrialAutomationDb.dbo.sp_CleanupOldData @RetentionDays = 90',
    @database_name = 'IndustrialAutomationDb';

EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'Daily_DataCleanup',
    @freq_type = 4, -- Daily
    @freq_interval = 1,
    @active_start_time = 030000; -- 3:00 AM

EXEC msdb.dbo.sp_attach_schedule
    @job_name = 'IndustrialAutomation_DataCleanup',
    @schedule_name = 'Daily_DataCleanup';

-- Create job for health monitoring
EXEC msdb.dbo.sp_add_job
    @job_name = 'IndustrialAutomation_HealthMonitoring',
    @description = 'Monitor system health for Industrial Automation Platform';

EXEC msdb.dbo.sp_add_jobstep
    @job_name = 'IndustrialAutomation_HealthMonitoring',
    @step_name = 'Health Check',
    @command = 'EXEC IndustrialAutomationDb.dbo.sp_HealthCheck',
    @database_name = 'IndustrialAutomationDb';

EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'Hourly_HealthCheck',
    @freq_type = 4, -- Daily
    @freq_interval = 1,
    @freq_subday_type = 8, -- Hourly
    @freq_subday_interval = 1,
    @active_start_time = 000000; -- Start at midnight

EXEC msdb.dbo.sp_attach_schedule
    @job_name = 'IndustrialAutomation_HealthMonitoring',
    @schedule_name = 'Hourly_HealthCheck';

PRINT 'Advanced database schema and optimization completed successfully!';
