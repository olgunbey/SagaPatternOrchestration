using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachine.Service.StateInstances;

namespace StateMachine.Service.StateMaps;

public class OrderStateMap:SagaClassMap<OrderStateInstance>
{
    protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
    {
        entity.Property(x => x.OrderId).IsRequired();
        entity.Property(x => x.BuyerId).IsRequired();
        entity.Property(x => x.TotalPrice).IsRequired();
    }
}