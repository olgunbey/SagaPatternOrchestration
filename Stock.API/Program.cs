using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MongoService>();
builder.Services.AddSingleton(y=> new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.AddConsumer<StockPaymentCompletedEventConsumer>();

    configure.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration.GetSection("AmqpConf")["Host"], config =>
        {
            config.Username(builder.Configuration.GetSection("AmqpConf")["Username"]!);
            config.Password(builder.Configuration.GetSection("AmqpConf")["Password"]!);
        });
        _configurator.ReceiveEndpoint(RabbitMqSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMqSettings.Stock_PaymentCompletedEventQueue, e => e.ConfigureConsumer<StockPaymentCompletedEventConsumer>(context));

    });
});
var app = builder.Build();
// using (var scope=app.Services.CreateScope())
// {
//    var mongoService= scope.ServiceProvider.GetRequiredService<MongoService>();
//    await mongoService.AddStock();
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

