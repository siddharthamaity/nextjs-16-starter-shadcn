using MassTransit;
using MassTransit.Saga;
using Ninject.Modules;

namespace Starbucks.Cashier
{
    public class CashierRegistry :
          NinjectModule
    {
        public override void Load()
        {
            Bind<ISagaRepository<CashierSaga>>()
                .To<InMemorySagaRepository<CashierSaga>>()
                .InSingletonScope();

            Bind<CashierService>()
                .To<CashierService>()
                .InSingletonScope();

            Bind<IServiceBus>().ToMethod(context =>
            {
                return ServiceBusFactory.New(sbc =>
                {
                    sbc.UseRabbitMq();
                    sbc.ReceiveFrom("rabbitmq://localhost/starbucks_cashier");
                    sbc.SetConcurrentConsumerLimit(1); //a cashier cannot multi-task

                    sbc.UseControlBus();
                    sbc.EnableRemoteIntrospection();
                });
            })
                .InSingletonScope();
        }
    }
}
