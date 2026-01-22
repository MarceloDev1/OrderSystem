using OrderWorkerService.Consumers;

namespace OrderWorkerService;

public sealed class Worker : BackgroundService
{
    private readonly OrderCreatedConsumer _consumer;

    public Worker(OrderCreatedConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _consumer.RunAsync(stoppingToken);
}