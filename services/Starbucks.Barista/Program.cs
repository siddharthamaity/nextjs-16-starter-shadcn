using System;
using System.Diagnostics;
using Magnum;
using Magnum.StateMachine;
using MassTransit.Log4NetIntegration.Logging;
using Ninject;
using Topshelf;

namespace Starbucks.Cashier
{
    class Program
    {
        static void Main(string[] args)
        {
                        Log4NetLogger.Use("cashier.log4net.xml");

            HostFactory.Run(c =>
                {
                    c.SetServiceName("StarbucksCashier");
                    c.SetDisplayName("Starbucks Cashier");
                    c.SetDescription("a Mass Transit sample service for handling orders of coffee.");

                    c.RunAsLocalSystem();
                    c.DependsOnMsmq();

                    var kernel = new StandardKernel();
                    var module = new CashierRegistry();
                    kernel.Load(module);

                    DisplayStateMachine();

                    c.Service<CashierService>(s =>
                        {
                            s.ConstructUsing(builder => kernel.Get<CashierService>());
                            s.WhenStarted(o => o.Start());
                            s.WhenStopped(o => o.Stop());
                        });
                });
        }

        static void DisplayStateMachine()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            StateMachineInspector.Trace(new CashierSaga(CombGuid.Generate()));
        }

    }
}
