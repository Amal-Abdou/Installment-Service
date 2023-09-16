using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Installment.Services
{

    public partial class InstallmentHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly InstallmentPaymentSettings _installmentPaymentSettings;

        #endregion

        #region Ctor

        public InstallmentHttpClient(HttpClient client,
            InstallmentPaymentSettings installmentPaymentSettings)
        {
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");
            
            _httpClient = client;
            _installmentPaymentSettings = installmentPaymentSettings;
        }

        #endregion

        #region Methods

        public async Task<string> GetPdtDetailsAsync(string tx)
        {
            var url = _installmentPaymentSettings.UseSandbox ?
                "https://migs-mtf.mastercard.com.au/vpcpay" :
                "https://migs-mtf.mastercard.com.au/vpcpay";
            var requestContent = new StringContent($"cmd=_notify-synch&at=&tx={tx}",
                Encoding.UTF8, MimeTypes.ApplicationXWwwFormUrlencoded);
            var response = await _httpClient.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> VerifyIpnAsync(string formString)
        {
            var url = _installmentPaymentSettings.UseSandbox ?
                "https://migs-mtf.mastercard.com.au/vpcpay" :
                "https://migs-mtf.mastercard.com.au/vpcpay";
            var requestContent = new StringContent($"cmd=_notify-validate&{formString}",
                Encoding.UTF8, MimeTypes.ApplicationXWwwFormUrlencoded);
            var response = await _httpClient.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


        public async Task<string> Auth_API()
        {
            var url = "https://accept.paymobsolutions.com/api/auth/tokens";
            var payload = new
            {
                api_key = _installmentPaymentSettings.ApiKey
            };

            // Serialize our concrete class into a JSON String
            var stringPayload = JsonConvert.SerializeObject(payload);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var token = "";
            if (!string.IsNullOrEmpty(responseString))
            {
                token = JObject.Parse(responseString).GetValue("token").ToString();
            }

            return token;
        }

        public async Task<string> OrderRegistration_API(string token , PostProcessPaymentRequest postProcessPaymentRequest)
        {
                var url = "https://accept.paymobsolutions.com/api/ecommerce/orders";
                var payload = new
                {
                    auth_token = token,
                    delivery_needed = "false",
                    amount_cents = postProcessPaymentRequest.Order.OrderTotal * 100,
                    currency = "EGP",
                    merchant_order_id = postProcessPaymentRequest.Order.Id.ToString(),
                    items = new object[] { }

                };

                // Serialize our concrete class into a JSON String
                var stringPayload = JsonConvert.SerializeObject(payload);

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var orderID = "";
                if (!string.IsNullOrEmpty(jsonString))
                {
                    // var responseObject = JsonConvert.DeserializeObject<object>(jsonString);
                    orderID = JObject.Parse(jsonString).GetValue("id").ToString();
                }
           
            return orderID;
        }

        public async Task<string> PaymentKey_API(PostProcessPaymentRequest postProcessPaymentRequest, string token, string orderID,Address address,string integrationId)
        {
                var paymentKey = "";
                var url = "https://accept.paymobsolutions.com/api/acceptance/payment_keys";
                var FirstName = address == null || string.IsNullOrEmpty(address.FirstName) ? "" : address.FirstName;
                var LastName = address == null || string.IsNullOrEmpty(address.LastName) ? "" : address.LastName;
                var Phone = address == null || string.IsNullOrEmpty(address.PhoneNumber) ? "" : address.PhoneNumber;
                var Email = address == null || string.IsNullOrEmpty(address.Email) ? "" : address.Email;

                var payload = new
                {
                    auth_token = token,
                    amount_cents = postProcessPaymentRequest.Order.OrderTotal * 100,
                    expiration = 3600,
                    order_id = orderID,
                    billing_data = new
                    {
                        apartment = "1",
                        email = Email,
                        floor = "1",
                        first_name = FirstName,
                        street = "1",
                        building = "1",
                        phone_number = Phone,
                        shipping_method = "PKG",
                        postal_code = "01898",
                        city = "Cairo",
                        country = "Egypt",
                        last_name = LastName,
                        state = "Cairo"
                    },
                    currency = "EGP",
                    integration_id = integrationId,
                    lock_order_when_paid = "false"
                };

                // Serialize our concrete class into a JSON String
                var stringPayload = JsonConvert.SerializeObject(payload);

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(jsonString))
                {
                    // var responseObject = JsonConvert.DeserializeObject<object>(jsonString);
                    paymentKey = JObject.Parse(jsonString).GetValue("token").ToString();
                }
            return paymentKey;
        }


        #endregion
    }
}