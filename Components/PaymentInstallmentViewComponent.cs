using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.Installment.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Installment.Components
{
    [ViewComponent(Name = "PaymentInstallment")]
    public class PaymentInstallmentViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel()
            {
                InstallmentPaymentTypes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Select Card Pay", Value = "" },
                    new SelectListItem { Text = "Valu", Value = "valu" },
                    new SelectListItem { Text = "Soohoula", Value = "soohoula" },
                    new SelectListItem { Text = "Aman", Value = "aman" },
                    new SelectListItem { Text = "Sympl", Value = "sympl" },
                    new SelectListItem { Text = "Bank", Value = "bank" },
                }
            };

            return View("~/Plugins/Payments.InstallmentService/Views/PaymentInfo.cshtml", model);
        }
    }
}

