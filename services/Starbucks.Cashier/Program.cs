using System;
using System.Diagnostics;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Magnum;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Log4NetIntegration.Logging;
using MassTransit.Saga;
using Topshelf;

namespace Starbucks.Barista
{
    class Program
    {
        static void Main(string[] args)
        {
            Log4NetLogger.Use("barista.log4net.xml");

            var container = new WindsorContainer();
            container.Register(
                Component.For(typeof(ISagaRepository<>))
                    .ImplementedBy(typeof(InMemorySagaRepository<>))
                    .LifeStyle.Singleton,
                Component.For<DrinkPreparationSaga>(),
                Component.For<BaristaService>()
                    .LifeStyle.Singleton,
                Component.For<IServiceBus>().UsingFactoryMethod(() =>
                {
                    return ServiceBusFactory.New(sbc =>
                    {
                        sbc.ReceiveFrom("rabbitmq://localhost/starbucks_barista");
                        sbc.UseRabbitMq();
                        sbc.UseControlBus();

                        sbc.Subscribe(subs => { subs.LoadFrom(container); });
                    });
                }).LifeStyle.Singleton);

            HostFactory.Run(c =>
            {
                c.SetServiceName("StarbucksBarista");
                c.SetDisplayName("Starbucks Barista");
                c.SetDescription("a Mass Transit sample service for making orders of coffee.");

                c.DependsOnMsmq();
                c.RunAsLocalService();

                DisplayStateMachine();

                c.Service<BaristaService>(s =>
                {
                    s.ConstructUsing(builder => container.Resolve<BaristaService>());
                    s.WhenStarted(o => o.Start());
                    s.WhenStopped(o =>
                    {
                        o.Stop();
                        container.Dispose();
                    });
                });
            });
        }

        static void DisplayStateMachine()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            StateMachineInspector.Trace(new DrinkPreparationSaga(CombGuid.Generate()));
        }
    }
}
