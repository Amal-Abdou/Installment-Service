using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Installment
{
    public class InstallmentPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }

        public string ApiKey { get; set; }

        public string ValuFrameId { get; set; }

        public string ValuIntegrationId { get; set; }

        public string SoohoulaFrameId { get; set; }

        public string SoohoulaIntegrationId { get; set; }

        public string AmanFrameId { get; set; }

        public string AmanIntegrationId { get; set; }

        public string SymplFrameId { get; set; }

        public string SymplIntegrationId { get; set; }

        public string BankFrameId { get; set; }

        public string BankIntegrationId { get; set; }

    }
}
