using System;
using FluentValidation;
using Nop.Plugin.Payments.Installment.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.Installment.Validators
{
    public partial class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        public PaymentInfoValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.InstallmentPaymentType).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Payment.InstallmentPaymentType.Required"));
        }
    }
}