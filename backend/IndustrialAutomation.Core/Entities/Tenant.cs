namespace IndustrialAutomation.Core.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public TenantSettings Settings { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime SubscriptionExpiresAt { get; set; }
    public string SubscriptionPlan { get; set; } = "Basic";
    public int MaxUsers { get; set; } = 10;
    public int MaxAutomationJobs { get; set; } = 100;
    public string? CustomBranding { get; set; }
    public string? CustomDomain { get; set; }
    public List<TenantUser> Users { get; set; } = new();
    public List<TenantResource> Resources { get; set; } = new();
}

public class TenantSettings
{
    public bool DataIsolation { get; set; } = true;
    public bool CustomBranding { get; set; } = false;
    public bool AdvancedAnalytics { get; set; } = false;
    public bool AIIntegration { get; set; } = false;
    public bool WorkflowEngine { get; set; } = false;
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class TenantUser : BaseEntity
{
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; }
    public DateTime? LastAccessAt { get; set; }
    public TenantUserPermissions Permissions { get; set; } = new();
}

public class TenantUserPermissions
{
    public bool CanCreateJobs { get; set; } = true;
    public bool CanEditJobs { get; set; } = true;
    public bool CanDeleteJobs { get; set; } = false;
    public bool CanViewAnalytics { get; set; } = true;
    public bool CanManageUsers { get; set; } = false;
    public bool CanAccessAI { get; set; } = false;
    public bool CanManageSettings { get; set; } = false;
    public string[] AllowedModules { get; set; } = Array.Empty<string>();
}

public class TenantResource : BaseEntity
{
    public int TenantId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime LastAccessedAt { get; set; }
}
