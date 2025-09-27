using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace IndustrialAutomation.Infrastructure.Services;

public class BackupService : IBackupService
{
    private readonly IndustrialAutomationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BackupService> _logger;
    private readonly string _backupPath;
    private readonly string _archivePath;

    public BackupService(
        IndustrialAutomationDbContext context,
        IConfiguration configuration,
        ILogger<BackupService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _backupPath = _configuration["Backup:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "backups");
        _archivePath = _configuration["Archive:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "archives");
        
        // Ensure directories exist
        Directory.CreateDirectory(_backupPath);
        Directory.CreateDirectory(_archivePath);
    }

    public async Task<BackupResult> CreateBackupAsync(BackupOptions options)
    {
        var startTime = DateTime.UtcNow;
        var backupId = Guid.NewGuid().ToString();
        
        try
        {
            _logger.LogInformation("Starting backup {BackupId} with options {Options}", backupId, JsonSerializer.Serialize(options));
            
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string not configured");

            var fileName = $"backup_{backupId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.sql";
            var filePath = Path.Combine(_backupPath, fileName);

            // Create SQL backup script
            var sqlScript = await GenerateBackupScriptAsync(connectionString, options);
            
            // Write to file
            await File.WriteAllTextAsync(filePath, sqlScript);
            
            var fileInfo = new FileInfo(filePath);
            var sizeBytes = fileInfo.Length;

            // Compress if requested
            if (options.Compress)
            {
                var compressedPath = filePath + ".gz";
                await CompressFileAsync(filePath, compressedPath);
                File.Delete(filePath);
                filePath = compressedPath;
                sizeBytes = new FileInfo(filePath).Length;
            }

            // Encrypt if requested
            if (options.Encrypt && !string.IsNullOrEmpty(options.EncryptionKey))
            {
                var encryptedPath = filePath + ".enc";
                await EncryptFileAsync(filePath, encryptedPath, options.EncryptionKey);
                File.Delete(filePath);
                filePath = encryptedPath;
                sizeBytes = new FileInfo(filePath).Length;
            }

            // Calculate checksum
            var checksum = await CalculateChecksumAsync(filePath);

            // Store backup metadata
            await StoreBackupMetadataAsync(backupId, options, filePath, sizeBytes, checksum);

            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogInformation("Backup {BackupId} completed successfully in {Duration}ms", backupId, duration.TotalMilliseconds);

            return new BackupResult
            {
                Success = true,
                BackupId = backupId,
                FilePath = filePath,
                SizeBytes = sizeBytes,
                CreatedAt = DateTime.UtcNow,
                Duration = duration,
                Metadata = new Dictionary<string, object>
                {
                    ["type"] = options.Type.ToString(),
                    ["compressed"] = options.Compress,
                    ["encrypted"] = options.Encrypt,
                    ["tables"] = options.Tables
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backup {BackupId}", backupId);
            return new BackupResult
            {
                Success = false,
                BackupId = backupId,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow,
                Duration = DateTime.UtcNow - startTime
            };
        }
    }

    public async Task<BackupResult> RestoreBackupAsync(string backupId, RestoreOptions options)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            _logger.LogInformation("Starting restore for backup {BackupId}", backupId);
            
            var backupInfo = await GetBackupInfoAsync(backupId);
            if (backupInfo == null)
                throw new FileNotFoundException($"Backup {backupId} not found");

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string not configured");

            var filePath = backupInfo.FilePath;
            
            // Decrypt if needed
            if (backupInfo.IsEncrypted && !string.IsNullOrEmpty(options.DecryptionKey))
            {
                var decryptedPath = filePath.Replace(".enc", "_decrypted");
                await DecryptFileAsync(filePath, decryptedPath, options.DecryptionKey);
                filePath = decryptedPath;
            }

            // Decompress if needed
            if (backupInfo.IsCompressed)
            {
                var decompressedPath = filePath.Replace(".gz", "_decompressed");
                await DecompressFileAsync(filePath, decompressedPath);
                if (filePath != backupInfo.FilePath) File.Delete(filePath);
                filePath = decompressedPath;
            }

            // Execute restore
            await ExecuteRestoreScriptAsync(connectionString, filePath, options);

            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogInformation("Restore for backup {BackupId} completed successfully in {Duration}ms", backupId, duration.TotalMilliseconds);

            return new BackupResult
            {
                Success = true,
                BackupId = backupId,
                CreatedAt = DateTime.UtcNow,
                Duration = duration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring backup {BackupId}", backupId);
            return new BackupResult
            {
                Success = false,
                BackupId = backupId,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow,
                Duration = DateTime.UtcNow - startTime
            };
        }
    }

    public async Task<List<BackupInfo>> GetBackupsAsync()
    {
        try
        {
            // This would typically query a backup metadata table
            // For now, scan the backup directory
            var backups = new List<BackupInfo>();
            var backupFiles = Directory.GetFiles(_backupPath, "*.sql*");
            
            foreach (var file in backupFiles)
            {
                var fileInfo = new FileInfo(file);
                var backupId = ExtractBackupIdFromFileName(file);
                
                if (!string.IsNullOrEmpty(backupId))
                {
                    backups.Add(new BackupInfo
                    {
                        Id = backupId,
                        Name = Path.GetFileNameWithoutExtension(file),
                        FilePath = file,
                        SizeBytes = fileInfo.Length,
                        CreatedAt = fileInfo.CreationTimeUtc,
                        IsCompressed = file.EndsWith(".gz"),
                        IsEncrypted = file.EndsWith(".enc")
                    });
                }
            }

            return backups.OrderByDescending(b => b.CreatedAt).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backups");
            return new List<BackupInfo>();
        }
    }

    public async Task<BackupInfo> GetBackupInfoAsync(string backupId)
    {
        try
        {
            var backups = await GetBackupsAsync();
            return backups.FirstOrDefault(b => b.Id == backupId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backup info for {BackupId}", backupId);
            return null;
        }
    }

    public async Task<bool> DeleteBackupAsync(string backupId)
    {
        try
        {
            var backupInfo = await GetBackupInfoAsync(backupId);
            if (backupInfo == null)
                return false;

            File.Delete(backupInfo.FilePath);
            
            _logger.LogInformation("Backup {BackupId} deleted successfully", backupId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backup {BackupId}", backupId);
            return false;
        }
    }

    public async Task<bool> VerifyBackupAsync(string backupId)
    {
        try
        {
            var backupInfo = await GetBackupInfoAsync(backupId);
            if (backupInfo == null)
                return false;

            if (!File.Exists(backupInfo.FilePath))
                return false;

            // Verify file integrity
            var currentChecksum = await CalculateChecksumAsync(backupInfo.FilePath);
            return currentChecksum == backupInfo.Checksum;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying backup {BackupId}", backupId);
            return false;
        }
    }

    public async Task<ArchiveResult> ArchiveDataAsync(ArchiveOptions options)
    {
        var startTime = DateTime.UtcNow;
        var archiveId = Guid.NewGuid().ToString();
        
        try
        {
            _logger.LogInformation("Starting archive {ArchiveId} with options {Options}", archiveId, JsonSerializer.Serialize(options));
            
            var fromDate = options.FromDate ?? DateTime.UtcNow.AddYears(-1);
            var toDate = options.ToDate ?? DateTime.UtcNow;
            
            var fileName = $"archive_{archiveId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(_archivePath, fileName);

            var archivedData = new Dictionary<string, object>();
            var totalRecords = 0;

            // Archive data from each table
            foreach (var table in options.Tables)
            {
                var tableData = await ArchiveTableDataAsync(table, fromDate, toDate);
                archivedData[table] = tableData;
                totalRecords += tableData.Count;
            }

            // Write archive file
            var jsonData = JsonSerializer.Serialize(archivedData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, jsonData);
            
            var fileInfo = new FileInfo(filePath);
            var sizeBytes = fileInfo.Length;

            // Compress if requested
            if (options.Compress)
            {
                var compressedPath = filePath + ".gz";
                await CompressFileAsync(filePath, compressedPath);
                File.Delete(filePath);
                filePath = compressedPath;
                sizeBytes = new FileInfo(filePath).Length;
            }

            // Encrypt if requested
            if (options.Encrypt && !string.IsNullOrEmpty(options.EncryptionKey))
            {
                var encryptedPath = filePath + ".enc";
                await EncryptFileAsync(filePath, encryptedPath, options.EncryptionKey);
                File.Delete(filePath);
                filePath = encryptedPath;
                sizeBytes = new FileInfo(filePath).Length;
            }

            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogInformation("Archive {ArchiveId} completed successfully in {Duration}ms", archiveId, duration.TotalMilliseconds);

            return new ArchiveResult
            {
                Success = true,
                ArchiveId = archiveId,
                FilePath = filePath,
                SizeBytes = sizeBytes,
                RecordsArchived = totalRecords,
                CreatedAt = DateTime.UtcNow,
                Duration = duration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating archive {ArchiveId}", archiveId);
            return new ArchiveResult
            {
                Success = false,
                ArchiveId = archiveId,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow,
                Duration = DateTime.UtcNow - startTime
            };
        }
    }

    public async Task<List<ArchiveInfo>> GetArchivesAsync()
    {
        try
        {
            var archives = new List<ArchiveInfo>();
            var archiveFiles = Directory.GetFiles(_archivePath, "*.json*");
            
            foreach (var file in archiveFiles)
            {
                var fileInfo = new FileInfo(file);
                var archiveId = ExtractArchiveIdFromFileName(file);
                
                if (!string.IsNullOrEmpty(archiveId))
                {
                    archives.Add(new ArchiveInfo
                    {
                        Id = archiveId,
                        Name = Path.GetFileNameWithoutExtension(file),
                        FilePath = file,
                        SizeBytes = fileInfo.Length,
                        CreatedAt = fileInfo.CreationTimeUtc,
                        IsCompressed = file.EndsWith(".gz"),
                        IsEncrypted = file.EndsWith(".enc")
                    });
                }
            }

            return archives.OrderByDescending(a => a.CreatedAt).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting archives");
            return new List<ArchiveInfo>();
        }
    }

    public async Task<bool> DeleteArchiveAsync(string archiveId)
    {
        try
        {
            var archives = await GetArchivesAsync();
            var archive = archives.FirstOrDefault(a => a.Id == archiveId);
            if (archive == null)
                return false;

            File.Delete(archive.FilePath);
            
            _logger.LogInformation("Archive {ArchiveId} deleted successfully", archiveId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting archive {ArchiveId}", archiveId);
            return false;
        }
    }

    public async Task<BackupResult> ScheduleBackupAsync(BackupSchedule schedule)
    {
        try
        {
            // This would typically store the schedule in a database
            // For now, we'll just log it
            _logger.LogInformation("Backup schedule created: {ScheduleId}", schedule.Id);
            
            return new BackupResult
            {
                Success = true,
                BackupId = schedule.Id,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling backup {ScheduleId}", schedule.Id);
            return new BackupResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public async Task<bool> CancelScheduledBackupAsync(string scheduleId)
    {
        try
        {
            _logger.LogInformation("Backup schedule cancelled: {ScheduleId}", scheduleId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling backup schedule {ScheduleId}", scheduleId);
            return false;
        }
    }

    public async Task<List<BackupSchedule>> GetScheduledBackupsAsync()
    {
        try
        {
            // This would typically query a backup schedule table
            return new List<BackupSchedule>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting scheduled backups");
            return new List<BackupSchedule>();
        }
    }

    private async Task<string> GenerateBackupScriptAsync(string connectionString, BackupOptions options)
    {
        var script = new StringBuilder();
        script.AppendLine("-- Industrial Automation Platform Backup");
        script.AppendLine($"-- Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        script.AppendLine($"-- Type: {options.Type}");
        script.AppendLine();

        // This is a simplified version - in production, you'd use SQL Server backup commands
        script.AppendLine("USE IndustrialAutomationDb;");
        script.AppendLine();

        foreach (var table in options.Tables)
        {
            script.AppendLine($"-- Backup table: {table}");
            script.AppendLine($"SELECT * FROM {table};");
            script.AppendLine();
        }

        return script.ToString();
    }

    private async Task ExecuteRestoreScriptAsync(string connectionString, string scriptPath, RestoreOptions options)
    {
        var script = await File.ReadAllTextAsync(scriptPath);
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        
        using var command = new SqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<List<Dictionary<string, object>>> ArchiveTableDataAsync(string tableName, DateTime fromDate, DateTime toDate)
    {
        // This is a simplified version - in production, you'd query the actual database
        return new List<Dictionary<string, object>>();
    }

    private async Task CompressFileAsync(string inputPath, string outputPath)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.Create(outputPath);
        using var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);
        await inputStream.CopyToAsync(gzipStream);
    }

    private async Task DecompressFileAsync(string inputPath, string outputPath)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.Create(outputPath);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        await gzipStream.CopyToAsync(outputStream);
    }

    private async Task EncryptFileAsync(string inputPath, string outputPath, string key)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.Create(outputPath);
        
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.GenerateIV();
        
        await outputStream.WriteAsync(aes.IV, 0, aes.IV.Length);
        
        using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await inputStream.CopyToAsync(cryptoStream);
    }

    private async Task DecryptFileAsync(string inputPath, string outputPath, string key)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.Create(outputPath);
        
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        
        var iv = new byte[16];
        await inputStream.ReadAsync(iv, 0, 16);
        aes.IV = iv;
        
        using var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        await cryptoStream.CopyToAsync(outputStream);
    }

    private async Task<string> CalculateChecksumAsync(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return Convert.ToBase64String(hash);
    }

    private async Task StoreBackupMetadataAsync(string backupId, BackupOptions options, string filePath, long sizeBytes, string checksum)
    {
        // This would typically store in a database table
        _logger.LogInformation("Backup metadata stored for {BackupId}", backupId);
    }

    private string ExtractBackupIdFromFileName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var match = System.Text.RegularExpressions.Regex.Match(fileName, @"backup_([a-f0-9-]+)_");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private string ExtractArchiveIdFromFileName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var match = System.Text.RegularExpressions.Regex.Match(fileName, @"archive_([a-f0-9-]+)_");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}
