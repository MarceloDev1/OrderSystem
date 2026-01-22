using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using OrderWorkerService.Contracts;
using OrderWorkerService.Repositories;

namespace OrderWorkerService.Consumers;

public sealed class OrderCreatedConsumer
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger,
        IConfiguration config,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _config = config;
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        var bootstrap = _config["Kafka:BootstrapServers"] ?? "localhost:9092";
        var groupId = _config["Kafka:GroupId"] ?? "order-worker-group";
        var topic = _config["Kafka:OrdersTopic"] ?? "orders.created";

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrap,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        _logger.LogInformation(
            "✅ Kafka consumer online | Bootstrap={Bootstrap} | Group={Group} | Topic={Topic}",
            bootstrap, groupId, topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;

                try
                {
                    result = consumer.Consume(stoppingToken);
                    if (result?.Message?.Value is null) continue;

                    var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(
                        result.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (evt is null)
                    {
                        _logger.LogWarning(
                            "⚠️ Não foi possível desserializar a mensagem: {Value}",
                            result.Message.Value);

                        // MVP: commita pra não travar
                        consumer.Commit(result);
                        continue;
                    }

                    _logger.LogInformation(
                        "📩 OrderCreated recebido | OrderId={OrderId} | ProductId={ProductId} | Qty={Qty}",
                        evt.OrderId, evt.ProductId, evt.Quantity);

                    // Cria scope por mensagem (resolve Scoped services corretamente)
                    using var scope = _scopeFactory.CreateScope();
                    var orders = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    // IMPORTANTE: await antes do commit
                    await orders.UpdateStatusAsync(evt.OrderId, "Processed", stoppingToken);

                    consumer.Commit(result);

                    _logger.LogInformation("✅ Pedido processado e commitado | OrderId={OrderId}", evt.OrderId);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError("❌ Kafka ConsumeException: {Reason}", ex.Error.Reason);
                }
                catch (OperationCanceledException)
                {
                    break; // shutdown normal
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao processar mensagem. (Não commitado para tentar novamente)");
                    // Em produção: retry + dead-letter topic
                }
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("🛑 Kafka consumer encerrado.");
        }
    }
}