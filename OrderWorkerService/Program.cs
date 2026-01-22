using OrderWorkerService;
using OrderWorkerService.Consumers;
using OrderWorkerService.Data;
using OrderWorkerService.Repositories;

var builder = Host.CreateApplicationBuilder(args);

// DI
builder.Services.AddSingleton<IConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddSingleton<OrderCreatedConsumer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();