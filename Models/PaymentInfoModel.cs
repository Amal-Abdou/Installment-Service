using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Payments.Installment.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public PaymentInfoModel()
        {
            InstallmentPaymentTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Payment.SelectInstallmentPaymentType")]
        public string InstallmentPaymentType { get; set; }

        [NopResourceDisplayName("Payment.SelectInstallmentPaymentType")]
        public IList<SelectListItem> InstallmentPaymentTypes { get; set; }

        [NopResourceDisplayName("Payment.InstallmentPaymentNumber")]
        public string InstallmentPaymentNumber { get; set; }

    }
}