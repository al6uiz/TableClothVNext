using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TableCloth3Mcp;

public sealed class LauncherMcpWorker : BackgroundService
{
    private readonly ILogger _logger;

    public LauncherMcpWorker(
        ILogger<LauncherMcpWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MCP stdio server started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync(stoppingToken);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var options = McpJsonContext.Default.McpRequest;

            try
            {
                var request = JsonSerializer.Deserialize(line, options);
                ArgumentNullException.ThrowIfNull(request);
                var response = HandleRequest(request);

                var json = JsonSerializer.Serialize(response, options);
                await Console.Out.WriteLineAsync(json.AsMemory(), stoppingToken);
                await Console.Out.FlushAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                var error = new McpResponse { Status = "error", Result = ex.Message };
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(error, options).AsMemory(), stoppingToken);
                await Console.Out.FlushAsync(stoppingToken);
            }
        }
    }

    private McpResponse HandleRequest(McpRequest request)
    {
        // 간단한 명령 분기
        return request.Command switch
        {
            "echo" => new McpResponse { Status = "ok", Result = request.Payload },
            _ => new McpResponse { Status = "unknown_command", Result = string.Empty, },
        };
    }
}

public class McpRequest
{
    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;
}

public class McpResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
}

[JsonSerializable(typeof(McpRequest))]
[JsonSerializable(typeof(McpResponse))]
public partial class McpJsonContext : JsonSerializerContext
{
    // EmitCompilerGeneratedFiles: True required
}
