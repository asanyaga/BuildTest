using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Payments
{
    public class PaymentsModule : Registry
    {
        public PaymentsModule()
        {
            For<BankRepository>().Use<BankRepository>();
            For<PaymentProcessor>().Use<PaymentProcessor>();
        }
    }
}
