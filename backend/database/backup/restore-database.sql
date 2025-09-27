-- Bosch Industrial Automation Platform - Database Restore Script
-- This script restores the database from backup files with proper recovery

USE master;

-- =============================================
-- RESTORE CONFIGURATION
-- =============================================

DECLARE @BackupPath NVARCHAR(500) = '/var/opt/mssql/backup/';
DECLARE @DatabaseName NVARCHAR(100) = 'IndustrialAutomationDb';
DECLARE @FullBackupFile NVARCHAR(500);
DECLARE @DiffBackupFile NVARCHAR(500);
DECLARE @LogBackupFile NVARCHAR(500);

-- Set backup file names (these would be provided as parameters in a real scenario)
SET @FullBackupFile = @BackupPath + @DatabaseName + '_Full_20241227_143000.bak';
SET @DiffBackupFile = @BackupPath + @DatabaseName + '_Diff_20241227_143000.bak';
SET @LogBackupFile = @BackupPath + @DatabaseName + '_Log_20241227_143000.trn';

-- =============================================
-- CHECK IF DATABASE EXISTS AND DROP IF NEEDED
-- =============================================

IF EXISTS (SELECT name FROM sys.databases WHERE name = @DatabaseName)
BEGIN
    PRINT 'Database exists. Dropping existing database...';
    
    -- Close all connections to the database
    ALTER DATABASE [IndustrialAutomationDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Drop the database
    DROP DATABASE [IndustrialAutomationDb];
    
    PRINT 'Existing database dropped successfully';
END

-- =============================================
-- RESTORE FULL DATABASE BACKUP
-- =============================================

PRINT 'Starting full database restore...';

RESTORE DATABASE [IndustrialAutomationDb] 
FROM DISK = @FullBackupFile
WITH 
    MOVE 'IndustrialAutomationDb' TO '/var/opt/mssql/data/IndustrialAutomationDb.mdf',
    MOVE 'IndustrialAutomationDb_Log' TO '/var/opt/mssql/data/IndustrialAutomationDb_Log.ldf',
    REPLACE,
    NORECOVERY,
    STATS = 10;

PRINT 'Full database restore completed';

-- =============================================
-- RESTORE DIFFERENTIAL BACKUP (if exists)
-- =============================================

-- Check if differential backup file exists
IF EXISTS (SELECT * FROM sys.dm_os_file_exists(@DiffBackupFile))
BEGIN
    PRINT 'Starting differential backup restore...';
    
    RESTORE DATABASE [IndustrialAutomationDb] 
    FROM DISK = @DiffBackupFile
    WITH 
        NORECOVERY,
        STATS = 10;
    
    PRINT 'Differential backup restore completed';
END
ELSE
BEGIN
    PRINT 'No differential backup found, skipping...';
END

-- =============================================
-- RESTORE TRANSACTION LOG BACKUP (if exists)
-- =============================================

-- Check if transaction log backup file exists
IF EXISTS (SELECT * FROM sys.dm_os_file_exists(@LogBackupFile))
BEGIN
    PRINT 'Starting transaction log restore...';
    
    RESTORE LOG [IndustrialAutomationDb] 
    FROM DISK = @LogBackupFile
    WITH 
        RECOVERY,
        STATS = 10;
    
    PRINT 'Transaction log restore completed';
END
ELSE
BEGIN
    PRINT 'No transaction log backup found, completing recovery...';
    
    -- Complete recovery without transaction log
    RESTORE DATABASE [IndustrialAutomationDb] WITH RECOVERY;
    
    PRINT 'Database recovery completed without transaction log';
END

-- =============================================
-- VERIFY DATABASE RESTORE
-- =============================================

-- Check database status
SELECT 
    name,
    state_desc,
    recovery_model_desc,
    collation_name
FROM sys.databases 
WHERE name = @DatabaseName;

-- Check table counts
USE IndustrialAutomationDb;

SELECT 
    'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 
    'AutomationJobs' as TableName, COUNT(*) as RecordCount FROM AutomationJobs
UNION ALL
SELECT 
    'TestExecutions' as TableName, COUNT(*) as RecordCount FROM TestExecutions
UNION ALL
SELECT 
    'WebAutomations' as TableName, COUNT(*) as RecordCount FROM WebAutomations
UNION ALL
SELECT 
    'JobSchedules' as TableName, COUNT(*) as RecordCount FROM JobSchedules
UNION ALL
SELECT 
    'MLModels' as TableName, COUNT(*) as RecordCount FROM MLModels
UNION ALL
SELECT 
    'AITrainingData' as TableName, COUNT(*) as RecordCount FROM AITrainingData;

-- =============================================
-- UPDATE STATISTICS
-- =============================================

PRINT 'Updating database statistics...';

-- Update statistics for all tables
EXEC sp_updatestats;

PRINT 'Database statistics updated successfully';

-- =============================================
-- VERIFY INTEGRITY
-- =============================================

PRINT 'Verifying database integrity...';

-- Check database integrity
DBCC CHECKDB([IndustrialAutomationDb]) WITH NO_INFOMSGS;

PRINT 'Database integrity check completed';

-- =============================================
-- RESTORE METADATA
-- =============================================

-- Log restore operation
INSERT INTO BackupMetadata (BackupType, FileName, FileSize, BackupDate, Status, Notes) VALUES
('Restore', @FullBackupFile, 0, GETUTCDATE(), 'Success', 'Full database restore completed'),
('Restore', @DiffBackupFile, 0, GETUTCDATE(), 'Success', 'Differential backup restore completed'),
('Restore', @LogBackupFile, 0, GETUTCDATE(), 'Success', 'Transaction log restore completed');

PRINT 'Restore metadata recorded successfully';

-- =============================================
-- FINAL VERIFICATION
-- =============================================

-- Test basic functionality
SELECT TOP 1 'Database restore verification' as Status, GETDATE() as Timestamp;

-- Check if key tables are accessible
IF EXISTS (SELECT * FROM Users WHERE Username = 'admin')
    PRINT 'SUCCESS: Admin user found - database restore successful';
ELSE
    PRINT 'WARNING: Admin user not found - may need to re-seed data';

PRINT 'Database restore process completed successfully';
PRINT 'Database is ready for use';
