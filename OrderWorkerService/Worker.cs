using Confluent.Kafka;
using System.Text.Json;

namespace OrderWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        var groupId = _configuration["Kafka:GroupId"] ?? "order-worker-group";
        var topic = _configuration["Kafka:OrdersTopic"] ?? "orders.created";

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        _logger.LogInformation("✅ Worker conectado no Kafka");
        _logger.LogInformation("📌 BootstrapServers: {Bootstrap}", bootstrapServers);
        _logger.LogInformation("📌 GroupId: {GroupId}", groupId);
        _logger.LogInformation("📌 Topic: {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (result?.Message?.Value is null)
                        continue;

                    var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(
                        result.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (evt is null)
                    {
                        _logger.LogWarning("⚠️ Mensagem recebida, mas não deu pra desserializar: {Value}", result.Message.Value);
                        continue;
                    }

                    _logger.LogInformation(
                        "📩 OrderCreated | OrderId={OrderId} | ProductId={ProductId} | Qty={Qty} | At={At}",
                        evt.OrderId, evt.ProductId, evt.Quantity, evt.CreatedAt
                    );

                }
                catch (ConsumeException ex)
                {
                    _logger.LogError("❌ Erro ao consumir Kafka: {Reason}", ex.Error.Reason);
                }
                catch (JsonException)
                {
                    _logger.LogWarning("⚠️ JSON inválido recebido (ignorando mensagem).");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("🛑 Encerrando worker (shutdown solicitado).");
        }
        finally
        {
            try
            {
                consumer.Close();
            }
            catch
            {
            }
        }

        return Task.CompletedTask;
    }

    private sealed class OrderCreatedEvent
    {
        public string? OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}