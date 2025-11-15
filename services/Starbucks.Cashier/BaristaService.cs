using MassTransit;

namespace Starbucks.Barista
{
    public class BaristaService
    {
        private readonly IServiceBus _bus;

        public BaristaService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}
