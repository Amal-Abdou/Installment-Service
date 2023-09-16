using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Installment;
using Nop.Plugin.Payments.Installment.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.PayMob.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class PaymentInstallmentController : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentInstallmentController(IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var installmentPaymentSettings = await _settingService.LoadSettingAsync<InstallmentPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = installmentPaymentSettings.UseSandbox,
                ApiKey = installmentPaymentSettings.ApiKey,
                AmanFrameId = installmentPaymentSettings.AmanFrameId,
                AmanIntegrationId = installmentPaymentSettings.AmanIntegrationId,
                BankFrameId = installmentPaymentSettings.BankFrameId,
                BankIntegrationId = installmentPaymentSettings.BankIntegrationId,
                SoohoulaFrameId = installmentPaymentSettings.SoohoulaFrameId,
                SoohoulaIntegrationId = installmentPaymentSettings.SoohoulaIntegrationId,
                SymplFrameId = installmentPaymentSettings.SymplFrameId,
                SymplIntegrationId = installmentPaymentSettings.SymplIntegrationId,
                ValuFrameId = installmentPaymentSettings.ValuFrameId,
                ValuIntegrationId = installmentPaymentSettings.ValuIntegrationId,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope <= 0)
                return View("~/Plugins/Payments.InstallmentService/Views/Configure.cshtml", model);

            model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.UseSandbox, storeScope);
            model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.ApiKey, storeScope);
            model.AmanFrameId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.AmanFrameId, storeScope);
            model.AmanIntegrationId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.AmanIntegrationId, storeScope);
            model.BankFrameId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.BankFrameId, storeScope);
            model.BankIntegrationId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.BankIntegrationId, storeScope);
            model.SoohoulaFrameId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.SoohoulaFrameId, storeScope);
            model.SoohoulaIntegrationId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.SoohoulaIntegrationId, storeScope);
            model.SymplFrameId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.SymplFrameId, storeScope);
            model.SymplIntegrationId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.SymplIntegrationId, storeScope);
            model.ValuFrameId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.ValuFrameId, storeScope);
            model.ValuIntegrationId_OverrideForStore = await _settingService.SettingExistsAsync(installmentPaymentSettings, x => x.ValuIntegrationId, storeScope);



            return View("~/Plugins/Payments.InstallmentService/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]        
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var installmentPaymentSettings = await _settingService.LoadSettingAsync<InstallmentPaymentSettings>(storeScope);

            installmentPaymentSettings.UseSandbox = model.UseSandbox;
            installmentPaymentSettings.ApiKey = model.ApiKey;
            installmentPaymentSettings.AmanFrameId = model.AmanFrameId;
            installmentPaymentSettings.AmanIntegrationId = model.AmanIntegrationId;
            installmentPaymentSettings.BankFrameId = model.BankFrameId;
            installmentPaymentSettings.BankIntegrationId = model.BankIntegrationId;
            installmentPaymentSettings.ValuFrameId = model.ValuFrameId;
            installmentPaymentSettings.ValuIntegrationId = model.ValuIntegrationId;
            installmentPaymentSettings.SoohoulaFrameId = model.SoohoulaFrameId;
            installmentPaymentSettings.SoohoulaIntegrationId = model.SoohoulaIntegrationId;
            installmentPaymentSettings.SymplFrameId = model.SymplFrameId;
            installmentPaymentSettings.SymplIntegrationId = model.SymplIntegrationId;


            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.AmanFrameId, model.AmanFrameId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.AmanIntegrationId, model.AmanIntegrationId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.BankFrameId, model.BankFrameId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.BankIntegrationId, model.BankIntegrationId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.ValuFrameId, model.ValuFrameId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.ValuIntegrationId, model.ValuIntegrationId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.SoohoulaFrameId, model.SoohoulaFrameId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.SoohoulaIntegrationId, model.SoohoulaIntegrationId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.SymplFrameId, model.SymplFrameId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(installmentPaymentSettings, x => x.SymplIntegrationId, model.SymplIntegrationId_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate PayPal rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = await _localizationService.GetResourceAsync("Plugins.Payments.Installment.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        public async Task<IActionResult> PDTHandler(
    string currency,
    //string source_data.type,
    string integration_id,
    string created_at,
    string is_refund,
    //  string source_data.pan,
    string is_auth,
    string hmac,
    string is_void,
    string is_voided,
    string merchant_order_id,
    string id,
    string pending,
    string is_standalone_payment,
    string owner,
    string captured_amount,
    string amount_cents,
    string has_parent_transaction,
    string is_capture,
    //   string data.message,
    string is_3d_secure,
    string order,
    //   string source_data.sub_type,
    string refunded_amount_cents,
    string error_occured,
    string profile_id,
    bool success,
    string txn_response_code,
    string is_refunded,
    string acq_response_code
    )
        {
            var status = success;
            //  var orderid = Convert.ToInt64(JObject.Parse(response).GetValue("merchant_order_id"));
            var order1 = await _orderService.GetOrderByIdAsync(int.Parse(merchant_order_id));
            if (order1 != null && status == true)
            {
                await _orderProcessingService.MarkOrderAsPaidAsync(order1);
                return RedirectToRoute("CheckoutCompleted", new { orderId = order1.Id });
            }
            else if (order1 != null && status == false)
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order1.Id,
                    Note = "The order cancelled because payment failed.",
                    DisplayToCustomer = true,
                    CreatedOnUtc = DateTime.UtcNow
                });
                order1.OrderStatusId = (int)OrderStatus.Cancelled;
                await _orderService.UpdateOrderAsync(order1);
            }

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        public async Task<IActionResult> CancelOrder(string PaymentID, string Result, string PostDate, string TranID, string Ref, string TrackID, string Auth, string OrderID, string cust_ref, string trnUdf)
        {
            var order = await _orderService.GetOrderByIdAsync(int.Parse(OrderID));

            if (order != null)
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "The order cancelled because payment failed.",
                    DisplayToCustomer = true,
                    CreatedOnUtc = DateTime.UtcNow
                });
                order.OrderStatusId = (int)OrderStatus.Cancelled;
                await _orderService.UpdateOrderAsync(order);
            }

            return RedirectToRoute("Homepage");
        }

        #endregion
    }
}