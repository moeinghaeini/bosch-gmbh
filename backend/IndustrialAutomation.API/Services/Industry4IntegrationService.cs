using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IndustrialAutomation.API.Services;

public interface IIndustry4IntegrationService
{
    Task<OPCUAConnection> ConnectToOPCUAAsync(string endpoint, OPCUASettings settings);
    Task<List<OPCUANode>> BrowseOPCUANodesAsync(string connectionId, string nodeId);
    Task<OPCUAVariable> ReadOPCUAVariableAsync(string connectionId, string nodeId);
    Task<bool> WriteOPCUAVariableAsync(string connectionId, string nodeId, object value);
    Task<MQTTConnection> ConnectToMQTTAsync(string broker, MQTTSettings settings);
    Task<bool> PublishMQTTMessageAsync(string connectionId, string topic, object message);
    Task<bool> SubscribeToMQTTTopicAsync(string connectionId, string topic, Action<MQTTMessage> handler);
    Task<ModbusConnection> ConnectToModbusAsync(string host, int port, ModbusSettings settings);
    Task<ModbusData> ReadModbusDataAsync(string connectionId, int address, int count);
    Task<bool> WriteModbusDataAsync(string connectionId, int address, object[] values);
    Task<PLCData> ReadPLCDataAsync(string connectionId, string[] addresses);
    Task<bool> WritePLCDataAsync(string connectionId, Dictionary<string, object> data);
    Task<SCADAData> GetSCADADataAsync(string systemId, string[] tags);
    Task<bool> SendSCADACommandAsync(string systemId, string command, Dictionary<string, object> parameters);
}

public class Industry4IntegrationService : IIndustry4IntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<Industry4IntegrationService> _logger;

    public Industry4IntegrationService(HttpClient httpClient, IMemoryCache cache, ILogger<Industry4IntegrationService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<OPCUAConnection> ConnectToOPCUAAsync(string endpoint, OPCUASettings settings)
    {
        try
        {
            var connectionRequest = new OPCUAConnectionRequest
            {
                Endpoint = endpoint,
                Settings = settings
            };

            var json = JsonSerializer.Serialize(connectionRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/industry4/opcua/connect", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OPCUAConnection>(result) ?? new OPCUAConnection();
            }
            
            throw new Exception($"OPC UA connection failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to OPC UA endpoint {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<List<OPCUANode>> BrowseOPCUANodesAsync(string connectionId, string nodeId)
    {
        try
        {
            var cacheKey = $"opcua_nodes_{connectionId}_{nodeId}";
            if (_cache.TryGetValue(cacheKey, out List<OPCUANode>? cachedNodes))
            {
                return cachedNodes ?? new List<OPCUANode>();
            }

            var response = await _httpClient.GetAsync($"/api/industry4/opcua/{connectionId}/browse/{nodeId}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var nodes = JsonSerializer.Deserialize<List<OPCUANode>>(result) ?? new List<OPCUANode>();
                
                _cache.Set(cacheKey, nodes, TimeSpan.FromMinutes(10));
                return nodes;
            }
            
            return new List<OPCUANode>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error browsing OPC UA nodes for connection {ConnectionId}", connectionId);
            return new List<OPCUANode>();
        }
    }

    public async Task<OPCUAVariable> ReadOPCUAVariableAsync(string connectionId, string nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/industry4/opcua/{connectionId}/read/{nodeId}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OPCUAVariable>(result) ?? new OPCUAVariable();
            }
            
            return new OPCUAVariable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading OPC UA variable {NodeId} from connection {ConnectionId}", nodeId, connectionId);
            return new OPCUAVariable();
        }
    }

    public async Task<bool> WriteOPCUAVariableAsync(string connectionId, string nodeId, object value)
    {
        try
        {
            var writeRequest = new OPCUAWriteRequest
            {
                NodeId = nodeId,
                Value = value
            };

            var json = JsonSerializer.Serialize(writeRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/industry4/opcua/{connectionId}/write", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing OPC UA variable {NodeId} to connection {ConnectionId}", nodeId, connectionId);
            return false;
        }
    }

    public async Task<MQTTConnection> ConnectToMQTTAsync(string broker, MQTTSettings settings)
    {
        try
        {
            var connectionRequest = new MQTTConnectionRequest
            {
                Broker = broker,
                Settings = settings
            };

            var json = JsonSerializer.Serialize(connectionRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/industry4/mqtt/connect", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MQTTConnection>(result) ?? new MQTTConnection();
            }
            
            throw new Exception($"MQTT connection failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to MQTT broker {Broker}", broker);
            throw;
        }
    }

    public async Task<bool> PublishMQTTMessageAsync(string connectionId, string topic, object message)
    {
        try
        {
            var publishRequest = new MQTTPublishRequest
            {
                Topic = topic,
                Message = message,
                Qos = 1,
                Retain = false
            };

            var json = JsonSerializer.Serialize(publishRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/industry4/mqtt/{connectionId}/publish", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing MQTT message to topic {Topic}", topic);
            return false;
        }
    }

    public async Task<bool> SubscribeToMQTTTopicAsync(string connectionId, string topic, Action<MQTTMessage> handler)
    {
        try
        {
            var subscribeRequest = new MQTTSubscribeRequest
            {
                Topic = topic,
                Qos = 1
            };

            var json = JsonSerializer.Serialize(subscribeRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/industry4/mqtt/{connectionId}/subscribe", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to MQTT topic {Topic}", topic);
            return false;
        }
    }

    public async Task<ModbusConnection> ConnectToModbusAsync(string host, int port, ModbusSettings settings)
    {
        try
        {
            var connectionRequest = new ModbusConnectionRequest
            {
                Host = host,
                Port = port,
                Settings = settings
            };

            var json = JsonSerializer.Serialize(connectionRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/industry4/modbus/connect", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ModbusConnection>(result) ?? new ModbusConnection();
            }
            
            throw new Exception($"Modbus connection failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to Modbus device {Host}:{Port}", host, port);
            throw;
        }
    }

    public async Task<ModbusData> ReadModbusDataAsync(string connectionId, int address, int count)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/industry4/modbus/{connectionId}/read/{address}/{count}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ModbusData>(result) ?? new ModbusData();
            }
            
            return new ModbusData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading Modbus data from connection {ConnectionId}", connectionId);
            return new ModbusData();
        }
    }

    public async Task<bool> WriteModbusDataAsync(string connectionId, int address, object[] values)
    {
        try
        {
            var writeRequest = new ModbusWriteRequest
            {
                Address = address,
                Values = values
            };

            var json = JsonSerializer.Serialize(writeRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/industry4/modbus/{connectionId}/write", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing Modbus data to connection {ConnectionId}", connectionId);
            return false;
        }
    }

    public async Task<PLCData> ReadPLCDataAsync(string connectionId, string[] addresses)
    {
        try
        {
            var readRequest = new PLCReadRequest
            {
                Addresses = addresses
            };

            var json = JsonSerializer.Serialize(readRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/industry4/plc/{connectionId}/read", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PLCData>(result) ?? new PLCData();
            }
            
            return new PLCData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading PLC data from connection {ConnectionId}", connectionId);
            return new PLCData();
        }
    }

    public async Task<bool> WritePLCDataAsync(string connectionId, Dictionary<string, object> data)
    {
        try
        {
            var writeRequest = new PLCWriteRequest
            {
                Data = data
            };

            var json = JsonSerializer.Serialize(writeRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/industry4/plc/{connectionId}/write", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing PLC data to connection {ConnectionId}", connectionId);
            return false;
        }
    }

    public async Task<SCADAData> GetSCADADataAsync(string systemId, string[] tags)
    {
        try
        {
            var dataRequest = new SCADADataRequest
            {
                SystemId = systemId,
                Tags = tags
            };

            var json = JsonSerializer.Serialize(dataRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/industry4/scada/data", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SCADAData>(result) ?? new SCADAData();
            }
            
            return new SCADAData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting SCADA data from system {SystemId}", systemId);
            return new SCADAData();
        }
    }

    public async Task<bool> SendSCADACommandAsync(string systemId, string command, Dictionary<string, object> parameters)
    {
        try
        {
            var commandRequest = new SCADACommandRequest
            {
                SystemId = systemId,
                Command = command,
                Parameters = parameters
            };

            var json = JsonSerializer.Serialize(commandRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/industry4/scada/command", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SCADA command to system {SystemId}", systemId);
            return false;
        }
    }
}

// Data Models
public class OPCUAConnection
{
    public string Id { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime ConnectedAt { get; set; }
    public OPCUASettings Settings { get; set; } = new();
}

public class OPCUASettings
{
    public string SecurityPolicy { get; set; } = "None";
    public string SecurityMode { get; set; } = "None";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int SessionTimeout { get; set; } = 60000;
    public int RequestTimeout { get; set; } = 5000;
}

public class OPCUANode
{
    public string NodeId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string NodeClass { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsReadable { get; set; }
    public bool IsWritable { get; set; }
    public List<OPCUANode> Children { get; set; } = new();
}

public class OPCUAVariable
{
    public string NodeId { get; set; } = string.Empty;
    public object Value { get; set; } = new();
    public string DataType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Quality { get; set; } = string.Empty;
}

public class MQTTConnection
{
    public string Id { get; set; } = string.Empty;
    public string Broker { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime ConnectedAt { get; set; }
    public MQTTSettings Settings { get; set; } = new();
}

public class MQTTSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool CleanSession { get; set; } = true;
    public int KeepAlive { get; set; } = 60;
    public int ConnectionTimeout { get; set; } = 30;
}

public class MQTTMessage
{
    public string Topic { get; set; } = string.Empty;
    public object Payload { get; set; } = new();
    public int Qos { get; set; }
    public bool Retain { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ModbusConnection
{
    public string Id { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool IsConnected { get; set; }
    public DateTime ConnectedAt { get; set; }
    public ModbusSettings Settings { get; set; } = new();
}

public class ModbusSettings
{
    public int SlaveId { get; set; } = 1;
    public int Timeout { get; set; } = 5000;
    public int Retries { get; set; } = 3;
    public string Protocol { get; set; } = "TCP";
}

public class ModbusData
{
    public int Address { get; set; }
    public object[] Values { get; set; } = Array.Empty<object>();
    public DateTime Timestamp { get; set; }
    public string Quality { get; set; } = string.Empty;
}

public class PLCData
{
    public Dictionary<string, object> Values { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string Quality { get; set; } = string.Empty;
}

public class SCADAData
{
    public string SystemId { get; set; } = string.Empty;
    public Dictionary<string, SCADATag> Tags { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

public class SCADATag
{
    public string Name { get; set; } = string.Empty;
    public object Value { get; set; } = new();
    public string DataType { get; set; } = string.Empty;
    public string Quality { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

// Request Models
public class OPCUAConnectionRequest
{
    public string Endpoint { get; set; } = string.Empty;
    public OPCUASettings Settings { get; set; } = new();
}

public class OPCUAWriteRequest
{
    public string NodeId { get; set; } = string.Empty;
    public object Value { get; set; } = new();
}

public class MQTTConnectionRequest
{
    public string Broker { get; set; } = string.Empty;
    public MQTTSettings Settings { get; set; } = new();
}

public class MQTTPublishRequest
{
    public string Topic { get; set; } = string.Empty;
    public object Message { get; set; } = new();
    public int Qos { get; set; }
    public bool Retain { get; set; }
}

public class MQTTSubscribeRequest
{
    public string Topic { get; set; } = string.Empty;
    public int Qos { get; set; }
}

public class ModbusConnectionRequest
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public ModbusSettings Settings { get; set; } = new();
}

public class ModbusWriteRequest
{
    public int Address { get; set; }
    public object[] Values { get; set; } = Array.Empty<object>();
}

public class PLCReadRequest
{
    public string[] Addresses { get; set; } = Array.Empty<string>();
}

public class PLCWriteRequest
{
    public Dictionary<string, object> Data { get; set; } = new();
}

public class SCADADataRequest
{
    public string SystemId { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
}

public class SCADACommandRequest
{
    public string SystemId { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}
