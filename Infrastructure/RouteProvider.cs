using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Installment.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //PDT
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Installment.PDTHandler", "Plugins/PaymentInstallment/PDTHandler",
                 new { controller = "PaymentInstallment", action = "PDTHandler" });

            //Cancel
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Installment.CancelOrder", "Plugins/PaymentInstallment/CancelOrder",
                 new { controller = "PaymentInstallment", action = "CancelOrder" });

        }

        public int Priority => -1;
    }
}