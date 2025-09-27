namespace IndustrialAutomation.Core.Interfaces;

public interface IBackupService
{
    Task<BackupResult> CreateBackupAsync(BackupOptions options);
    Task<BackupResult> RestoreBackupAsync(string backupId, RestoreOptions options);
    Task<List<BackupInfo>> GetBackupsAsync();
    Task<BackupInfo> GetBackupInfoAsync(string backupId);
    Task<bool> DeleteBackupAsync(string backupId);
    Task<bool> VerifyBackupAsync(string backupId);
    Task<ArchiveResult> ArchiveDataAsync(ArchiveOptions options);
    Task<List<ArchiveInfo>> GetArchivesAsync();
    Task<bool> DeleteArchiveAsync(string archiveId);
    Task<BackupResult> ScheduleBackupAsync(BackupSchedule schedule);
    Task<bool> CancelScheduledBackupAsync(string scheduleId);
    Task<List<BackupSchedule>> GetScheduledBackupsAsync();
}

public class BackupOptions
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BackupType Type { get; set; } = BackupType.Full;
    public List<string> Tables { get; set; } = new();
    public bool Compress { get; set; } = true;
    public bool Encrypt { get; set; } = true;
    public string? EncryptionKey { get; set; }
    public string? StoragePath { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class RestoreOptions
{
    public string BackupId { get; set; } = string.Empty;
    public bool OverwriteExisting { get; set; } = false;
    public List<string>? Tables { get; set; }
    public string? DecryptionKey { get; set; }
    public bool ValidateData { get; set; } = true;
}

public class ArchiveOptions
{
    public string Name { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<string> Tables { get; set; } = new();
    public ArchiveRetention Retention { get; set; } = ArchiveRetention.OneYear;
    public bool Compress { get; set; } = true;
    public bool Encrypt { get; set; } = true;
    public string? EncryptionKey { get; set; }
    public string? StoragePath { get; set; }
}

public class BackupResult
{
    public bool Success { get; set; }
    public string BackupId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ArchiveResult
{
    public bool Success { get; set; }
    public string ArchiveId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int RecordsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ErrorMessage { get; set; }
}

public class BackupInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BackupType Type { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsEncrypted { get; set; }
    public bool IsCompressed { get; set; }
    public string? Checksum { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class ArchiveInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int RecordsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public ArchiveRetention Retention { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsEncrypted { get; set; }
    public bool IsCompressed { get; set; }
}

public class BackupSchedule
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public BackupOptions Options { get; set; } = new();
    public bool IsEnabled { get; set; } = true;
    public DateTime? LastRun { get; set; }
    public DateTime? NextRun { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum BackupType
{
    Full,
    Incremental,
    Differential
}

public enum ArchiveRetention
{
    OneMonth,
    ThreeMonths,
    SixMonths,
    OneYear,
    TwoYears,
    FiveYears,
    Permanent
}
