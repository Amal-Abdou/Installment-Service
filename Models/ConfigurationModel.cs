using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Installment.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.ValuFrameId")]
        public string ValuFrameId { get; set; }
        public bool ValuFrameId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.ValuIntegrationId")]
        public string ValuIntegrationId { get; set; }
        public bool ValuIntegrationId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.SoohoulaFrameId")]
        public string SoohoulaFrameId { get; set; }
        public bool SoohoulaFrameId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.SoohoulaIntegrationId")]
        public string SoohoulaIntegrationId { get; set; }
        public bool SoohoulaIntegrationId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.AmanFrameId")]
        public string AmanFrameId { get; set; }
        public bool AmanFrameId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.AmanIntegrationId")]
        public string AmanIntegrationId { get; set; }
        public bool AmanIntegrationId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.SymplFrameId")]
        public string SymplFrameId { get; set; }
        public bool SymplFrameId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.SymplIntegrationId")]
        public string SymplIntegrationId { get; set; }
        public bool SymplIntegrationId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.BankFrameId")]
        public string BankFrameId { get; set; }
        public bool BankFrameId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayMob.Fields.BankIntegrationId")]
        public string BankIntegrationId { get; set; }
        public bool BankIntegrationId_OverrideForStore { get; set; }


    }
}