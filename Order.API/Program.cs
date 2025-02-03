using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Database;
using Order.API.Dtos;
using Order.API.Entity;
using Shared;
using Shared.Message;
using Shared.OrderEvents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(conf => conf.UseNpgsql("Host=localhost;Port=5432;Database=OrchestrationOrderAPI;Username=myuser;Password=mypassword;"));

builder.Services.AddMassTransit<IBus>(configurator =>
{
    configurator.AddConsumer<OrderFailedEventConsumer>();
    configurator.AddConsumer<OrderPaymentCompletedEventConsumer>();

    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration.GetSection("AmqpConf")["Host"], conf =>
        {
            conf.Username(builder.Configuration.GetSection("AmqpConf")["Username"]);
            conf.Password(builder.Configuration.GetSection("AmqpConf")["Password"]);
        });
        _configure.ReceiveEndpoint(RabbitMqSettings.Order_PaymentCompletedEventQueue, conf => conf.ConfigureConsumer<OrderPaymentCompletedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMqSettings.Order_OrderFailedEventQueue, conf => conf.ConfigureConsumer<OrderFailedEventConsumer>(context));
    });

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("Order/ReceivingOrder", async (AppDbContext dbContext, ReceivingOrderDto dto, IBus bus) =>
    {
        var order = new Order.API.Entity.Order()
        {
            BuyerId = dto.BuyerId,
            CreatedDate = DateTime.UtcNow,
            OrderItems = dto.OrderItems,
            OrderStatus = OrderStatus.Pending,
            TotalPrice = dto.OrderItems.Sum(item => item.Price * item.Count)
        };
        dbContext.Order.Add(order);
        await dbContext.SaveChangesAsync();

        OrderStartedEvent orderStartedEvent = new OrderStartedEvent()
        {
            OrderId = order.Id,
            BuyerId = order.BuyerId,
            OrderItemMessages = order.OrderItems.Select(y => new OrderItemMessage()
            {
                ProductId = y.ProductId,
                Count = y.Count,
                Price = y.Price
            }).ToList(),
            TotalPrice = order.TotalPrice,
        };

        var sendEndpoint = await bus.GetSendEndpoint(new Uri($"queue:{RabbitMqSettings.StateMachine}"));

        await sendEndpoint.Send(orderStartedEvent);

    })
    .WithOpenApi();

app.Run();
