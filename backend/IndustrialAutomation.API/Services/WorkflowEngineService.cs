using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IndustrialAutomation.API.Services;

public interface IWorkflowEngineService
{
    Task<WorkflowDefinition> CreateWorkflowAsync(WorkflowDefinition definition);
    Task<WorkflowInstance> StartWorkflowAsync(string workflowId, Dictionary<string, object> input);
    Task<WorkflowInstance> GetWorkflowInstanceAsync(string instanceId);
    Task<bool> CompleteTaskAsync(string instanceId, string taskId, Dictionary<string, object> result);
    Task<List<WorkflowInstance>> GetWorkflowInstancesAsync(string workflowId);
    Task<WorkflowExecution> ExecuteWorkflowAsync(string workflowId, Dictionary<string, object> input);
    Task<bool> PauseWorkflowAsync(string instanceId);
    Task<bool> ResumeWorkflowAsync(string instanceId);
    Task<bool> CancelWorkflowAsync(string instanceId);
}

public class WorkflowEngineService : IWorkflowEngineService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WorkflowEngineService> _logger;

    public WorkflowEngineService(HttpClient httpClient, IMemoryCache cache, ILogger<WorkflowEngineService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<WorkflowDefinition> CreateWorkflowAsync(WorkflowDefinition definition)
    {
        try
        {
            var json = JsonSerializer.Serialize(definition);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/workflows", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<WorkflowDefinition>(result) ?? new WorkflowDefinition();
            }
            
            throw new Exception($"Workflow creation failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow {WorkflowName}", definition.Name);
            throw;
        }
    }

    public async Task<WorkflowInstance> StartWorkflowAsync(string workflowId, Dictionary<string, object> input)
    {
        try
        {
            var startRequest = new WorkflowStartRequest
            {
                WorkflowId = workflowId,
                Input = input,
                Priority = WorkflowPriority.Normal
            };

            var json = JsonSerializer.Serialize(startRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/workflows/start", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<WorkflowInstance>(result) ?? new WorkflowInstance();
            }
            
            throw new Exception($"Workflow start failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting workflow {WorkflowId}", workflowId);
            throw;
        }
    }

    public async Task<WorkflowInstance> GetWorkflowInstanceAsync(string instanceId)
    {
        try
        {
            var cacheKey = $"workflow_instance_{instanceId}";
            if (_cache.TryGetValue(cacheKey, out WorkflowInstance? cachedInstance))
            {
                return cachedInstance ?? new WorkflowInstance();
            }

            var response = await _httpClient.GetAsync($"/api/workflows/instances/{instanceId}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var instance = JsonSerializer.Deserialize<WorkflowInstance>(result) ?? new WorkflowInstance();
                
                _cache.Set(cacheKey, instance, TimeSpan.FromMinutes(5));
                return instance;
            }
            
            return new WorkflowInstance();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instance {InstanceId}", instanceId);
            return new WorkflowInstance();
        }
    }

    public async Task<bool> CompleteTaskAsync(string instanceId, string taskId, Dictionary<string, object> result)
    {
        try
        {
            var taskCompletion = new TaskCompletion
            {
                InstanceId = instanceId,
                TaskId = taskId,
                Result = result,
                CompletedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(taskCompletion);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/workflows/tasks/complete", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task {TaskId} for instance {InstanceId}", taskId, instanceId);
            return false;
        }
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstancesAsync(string workflowId)
    {
        try
        {
            var cacheKey = $"workflow_instances_{workflowId}";
            if (_cache.TryGetValue(cacheKey, out List<WorkflowInstance>? cachedInstances))
            {
                return cachedInstances ?? new List<WorkflowInstance>();
            }

            var response = await _httpClient.GetAsync($"/api/workflows/{workflowId}/instances");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var instances = JsonSerializer.Deserialize<List<WorkflowInstance>>(result) ?? new List<WorkflowInstance>();
                
                _cache.Set(cacheKey, instances, TimeSpan.FromMinutes(5));
                return instances;
            }
            
            return new List<WorkflowInstance>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instances for {WorkflowId}", workflowId);
            return new List<WorkflowInstance>();
        }
    }

    public async Task<WorkflowExecution> ExecuteWorkflowAsync(string workflowId, Dictionary<string, object> input)
    {
        try
        {
            var executionRequest = new WorkflowExecutionRequest
            {
                WorkflowId = workflowId,
                Input = input,
                ExecutionMode = ExecutionMode.Synchronous
            };

            var json = JsonSerializer.Serialize(executionRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/workflows/execute", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<WorkflowExecution>(result) ?? new WorkflowExecution();
            }
            
            return new WorkflowExecution { Status = WorkflowStatus.Failed };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing workflow {WorkflowId}", workflowId);
            return new WorkflowExecution { Status = WorkflowStatus.Failed };
        }
    }

    public async Task<bool> PauseWorkflowAsync(string instanceId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/workflows/instances/{instanceId}/pause", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing workflow instance {InstanceId}", instanceId);
            return false;
        }
    }

    public async Task<bool> ResumeWorkflowAsync(string instanceId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/workflows/instances/{instanceId}/resume", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming workflow instance {InstanceId}", instanceId);
            return false;
        }
    }

    public async Task<bool> CancelWorkflowAsync(string instanceId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/workflows/instances/{instanceId}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling workflow instance {InstanceId}", instanceId);
            return false;
        }
    }
}

// Data Models
public class WorkflowDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public WorkflowNode[] Nodes { get; set; } = Array.Empty<WorkflowNode>();
    public WorkflowConnection[] Connections { get; set; } = Array.Empty<WorkflowConnection>();
    public Dictionary<string, object> Variables { get; set; } = new();
    public WorkflowSettings Settings { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class WorkflowNode
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public WorkflowPosition Position { get; set; } = new();
}

public class WorkflowConnection
{
    public string Id { get; set; } = string.Empty;
    public string SourceNodeId { get; set; } = string.Empty;
    public string TargetNodeId { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
}

public class WorkflowPosition
{
    public double X { get; set; }
    public double Y { get; set; }
}

public class WorkflowSettings
{
    public int TimeoutMinutes { get; set; } = 60;
    public int MaxRetries { get; set; } = 3;
    public bool ParallelExecution { get; set; } = false;
    public string ErrorHandling { get; set; } = "Stop";
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class WorkflowInstance
{
    public string Id { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; }
    public Dictionary<string, object> Input { get; set; } = new();
    public Dictionary<string, object> Output { get; set; } = new();
    public Dictionary<string, object> Variables { get; set; } = new();
    public List<WorkflowTask> Tasks { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class WorkflowTask
{
    public string Id { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public Dictionary<string, object> Input { get; set; } = new();
    public Dictionary<string, object> Output { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class WorkflowExecution
{
    public string Id { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; }
    public Dictionary<string, object> Input { get; set; } = new();
    public Dictionary<string, object> Output { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long Duration { get; set; }
    public string? ErrorMessage { get; set; }
}

// Request Models
public class WorkflowStartRequest
{
    public string WorkflowId { get; set; } = string.Empty;
    public Dictionary<string, object> Input { get; set; } = new();
    public WorkflowPriority Priority { get; set; }
}

public class TaskCompletion
{
    public string InstanceId { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public Dictionary<string, object> Result { get; set; } = new();
    public DateTime CompletedAt { get; set; }
}

public class WorkflowExecutionRequest
{
    public string WorkflowId { get; set; } = string.Empty;
    public Dictionary<string, object> Input { get; set; } = new();
    public ExecutionMode ExecutionMode { get; set; }
}

// Enums
public enum WorkflowStatus
{
    Pending,
    Running,
    Paused,
    Completed,
    Failed,
    Cancelled
}

public enum TaskStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Skipped
}

public enum WorkflowPriority
{
    Low,
    Normal,
    High,
    Critical
}

public enum ExecutionMode
{
    Synchronous,
    Asynchronous
}
