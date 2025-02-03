using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Shared;
using StateMachine.Service.DbContext;
using StateMachine.Service.StateInstances;
using StateMachine.Service.StateMachines;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderStateDbContext>(conf=>conf.UseNpgsql("Host=localhost;Port=5432;Database=OrchestrationStateMachineService;Username=myuser;Password=mypassword;"));
builder.Services.AddMassTransit(conf =>
{
    conf.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
        .EntityFrameworkRepository(opt =>
        {
            opt.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            opt.AddDbContext<DbContext, OrderStateDbContext>();
            opt.UsePostgres();
        });
    conf.UsingRabbitMq((context, configure) =>
    {
        configure.Host(builder.Configuration.GetSection("AmqpConf")["Host"], config =>
        {
            config.Username(builder.Configuration.GetSection("AmqpConf")["Username"]);
            config.Password(builder.Configuration.GetSection("AmqpConf")["Password"]);

        });
        configure.ReceiveEndpoint(RabbitMqSettings.StateMachine, e =>
        {
            e.ConfigureSaga<OrderStateInstance>(context);
        });
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



app.MapGet("/exam/ff",()=>
{

    return "aa";
});
app.Run();
