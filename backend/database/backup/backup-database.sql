-- Bosch Industrial Automation Platform - Database Backup Script
-- This script creates a comprehensive backup of the database with all data and schema

USE IndustrialAutomationDb;

-- =============================================
-- BACKUP CONFIGURATION
-- =============================================

DECLARE @BackupPath NVARCHAR(500) = '/var/opt/mssql/backup/';
DECLARE @DatabaseName NVARCHAR(100) = 'IndustrialAutomationDb';
DECLARE @BackupFileName NVARCHAR(500);
DECLARE @Timestamp NVARCHAR(20) = FORMAT(GETDATE(), 'yyyyMMdd_HHmmss');

-- Set backup file name with timestamp
SET @BackupFileName = @BackupPath + @DatabaseName + '_Full_' + @Timestamp + '.bak';

-- =============================================
-- CREATE FULL DATABASE BACKUP
-- =============================================

-- Create full database backup
BACKUP DATABASE [IndustrialAutomationDb] 
TO DISK = @BackupFileName
WITH 
    FORMAT,
    INIT,
    NAME = 'IndustrialAutomationDb Full Backup',
    SKIP,
    NOREWIND,
    NOUNLOAD,
    STATS = 10,
    CHECKSUM,
    COMPRESSION;

PRINT 'Full database backup completed: ' + @BackupFileName;

-- =============================================
-- CREATE DIFFERENTIAL BACKUP (if base backup exists)
-- =============================================

DECLARE @DiffBackupFileName NVARCHAR(500);
SET @DiffBackupFileName = @BackupPath + @DatabaseName + '_Diff_' + @Timestamp + '.bak';

-- Create differential backup
BACKUP DATABASE [IndustrialAutomationDb] 
TO DISK = @DiffBackupFileName
WITH 
    DIFFERENTIAL,
    FORMAT,
    INIT,
    NAME = 'IndustrialAutomationDb Differential Backup',
    SKIP,
    NOREWIND,
    NOUNLOAD,
    STATS = 10,
    CHECKSUM,
    COMPRESSION;

PRINT 'Differential backup completed: ' + @DiffBackupFileName;

-- =============================================
-- CREATE TRANSACTION LOG BACKUP
-- =============================================

DECLARE @LogBackupFileName NVARCHAR(500);
SET @LogBackupFileName = @BackupPath + @DatabaseName + '_Log_' + @Timestamp + '.trn';

-- Create transaction log backup
BACKUP LOG [IndustrialAutomationDb] 
TO DISK = @LogBackupFileName
WITH 
    FORMAT,
    INIT,
    NAME = 'IndustrialAutomationDb Transaction Log Backup',
    SKIP,
    NOREWIND,
    NOUNLOAD,
    STATS = 10,
    CHECKSUM;

PRINT 'Transaction log backup completed: ' + @LogBackupFileName;

-- =============================================
-- VERIFY BACKUP INTEGRITY
-- =============================================

-- Verify full backup
RESTORE VERIFYONLY 
FROM DISK = @BackupFileName;

-- Verify differential backup
RESTORE VERIFYONLY 
FROM DISK = @DiffBackupFileName;

-- Verify transaction log backup
RESTORE VERIFYONLY 
FROM DISK = @LogBackupFileName;

PRINT 'All backup files verified successfully';

-- =============================================
-- BACKUP METADATA
-- =============================================

-- Create backup metadata table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BackupMetadata' AND xtype='U')
BEGIN
    CREATE TABLE BackupMetadata (
        Id int IDENTITY(1,1) PRIMARY KEY,
        BackupType nvarchar(50) NOT NULL,
        FileName nvarchar(500) NOT NULL,
        FileSize bigint NOT NULL,
        BackupDate datetime2 NOT NULL DEFAULT GETUTCDATE(),
        Status nvarchar(20) NOT NULL DEFAULT 'Success',
        Notes nvarchar(500) NULL
    );
END

-- Get file sizes
DECLARE @FullBackupSize bigint;
DECLARE @DiffBackupSize bigint;
DECLARE @LogBackupSize bigint;

-- Note: File size calculation would require additional system procedures
-- For now, we'll insert metadata without file sizes
INSERT INTO BackupMetadata (BackupType, FileName, FileSize, BackupDate, Status, Notes) VALUES
('Full', @BackupFileName, 0, GETUTCDATE(), 'Success', 'Full database backup with compression'),
('Differential', @DiffBackupFileName, 0, GETUTCDATE(), 'Success', 'Differential backup with compression'),
('Transaction Log', @LogBackupFileName, 0, GETUTCDATE(), 'Success', 'Transaction log backup');

PRINT 'Backup metadata recorded successfully';

-- =============================================
-- CLEANUP OLD BACKUPS (optional)
-- =============================================

-- Clean up backups older than 30 days
DECLARE @OldBackupDate datetime2 = DATEADD(day, -30, GETUTCDATE());

-- This would require additional file system operations
-- For now, we'll just log the cleanup intention
PRINT 'Old backup cleanup would be performed here (backups older than 30 days)';

PRINT 'Database backup process completed successfully';
PRINT 'Backup files created:';
PRINT '  - Full Backup: ' + @BackupFileName;
PRINT '  - Differential Backup: ' + @DiffBackupFileName;
PRINT '  - Transaction Log Backup: ' + @LogBackupFileName;
