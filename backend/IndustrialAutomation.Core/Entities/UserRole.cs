namespace IndustrialAutomation.Core.Entities;

public class UserRole : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RoleId { get; set; } = 0; // Role ID for compatibility
    public string UserRoleName { get; set; } = string.Empty; // User role name for compatibility
    public string Permissions { get; set; } = string.Empty; // JSON permissions
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0; // Role hierarchy
    public string AllowedModules { get; set; } = string.Empty; // JSON allowed modules
    public string AllowedActions { get; set; } = string.Empty; // JSON allowed actions
    public string Restrictions { get; set; } = string.Empty; // JSON restrictions
    public DateTime LastModified { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}

public class UserPermission : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string PermissionType { get; set; } = string.Empty; // Read, Write, Execute, Admin
    public string Resource { get; set; } = string.Empty; // Module or feature
    public bool IsGranted { get; set; } = true;
    public DateTime GrantedAt { get; set; }
    public string GrantedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}
