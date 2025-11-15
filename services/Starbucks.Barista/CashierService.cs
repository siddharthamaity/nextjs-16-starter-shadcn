using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;

namespace Starbucks.Cashier
{
    public class CashierService
    {
        private IServiceBus _bus;
        private readonly ISagaRepository<CashierSaga> _sagaRepository;
        private UnsubscribeAction _unsubscribeAction;

        public CashierService(IServiceBus bus, ISagaRepository<CashierSaga> sagaRepository)
        {
            _bus = bus;
            _sagaRepository = sagaRepository;
        }

        public void Start()
        {
            // ninject doesn't have the brains for this one
            _unsubscribeAction = _bus.SubscribeSaga(_sagaRepository);
        }

        public void Stop()
        {
            _unsubscribeAction();
            _bus.Dispose();
        }
    }
}
