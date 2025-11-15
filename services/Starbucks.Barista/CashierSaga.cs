using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Starbucks.Messages;

namespace Starbucks.Cashier
{
    public class CashierSaga :
      SagaStateMachine<CashierSaga>,
      ISaga
    {
        decimal _amount;
        string _item;
        string _name;
        string _size;

        static CashierSaga()
        {
            Define(() =>
            {
                Initially(
                    When(NewOrder)
                        .Then((saga, message) => saga.ProcessNewOrder(message))
                        .TransitionTo(WaitingForPayment)
                    );

                During(WaitingForPayment,
                       When(PaymentSubmitted)
                           .Then((saga, message) => saga.ProcessPayment(message))
                           .Complete()
                    );
            });
        }

        public CashierSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public static State Initial { get; set; }
        public static State Completed { get; set; }
        public static State WaitingForPayment { get; set; }

        public static Event<NewOrderMessage> NewOrder { get; set; }
        public static Event<SubmitPaymentMessage> PaymentSubmitted { get; set; }

        public Guid CorrelationId { get; set; }

        public IServiceBus Bus { get; set; }

        void ProcessNewOrder(NewOrderMessage message)
        {
            _name = message.Name;
            _item = message.Item;
            _size = message.Size;
            _amount = GetPriceForSize(_size);

            Console.WriteLine(string.Format("I've received an order for a {0} {1} for {2}.", _size, _item, _name));

            var paymentDueMessage = new PaymentDueMessage
            {
                CorrelationId = message.CorrelationId,
                Amount = _amount,
            };

            Bus.Context().Respond(paymentDueMessage);
        }

        void ProcessPayment(SubmitPaymentMessage message)
        {
            if (message.Amount > _amount)
            {
                Console.WriteLine("Thanks for the tip!");
            }
            else if (message.Amount < _amount)
            {
                Console.WriteLine("What are you, some kind of charity case?");
            }

            var paymentType = message.PaymentType;

            Console.WriteLine("Received a payment of {0} for {1} ({2})", paymentType, _item, _size);

            if (paymentType == PaymentType.CreditCard)
            {
                Console.Write("Authorizing Card...");
                Thread.Sleep(4000);
                Console.WriteLine("done!");
            }

            var completeMessage = new PaymentCompleteMessage
            {
                CorrelationId = message.CorrelationId,
            };

            Bus.Publish(completeMessage);
        }


        static decimal GetPriceForSize(string size)
        {
            switch (size.ToLower())
            {
                case "tall":
                    return 3.25m;
                case "grande":
                    return 4.00m;
                case "venti":
                    return 4.75m;
                default:
                    throw new Exception(string.Format("We don't have that size ({0})", size));
            }
        }
    }
}
