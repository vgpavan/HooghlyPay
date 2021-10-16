using HooghlyPay.API.ControllerLogic.Interface;
using HooghlyPay.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HooghlyPay.API.ControllerLogic.Implementaion
{
    public class PayService : IPayService
    {
        private readonly IConfiguration _config;
        private readonly ICrypto _crypto;
        public PayService(IConfiguration config, ICrypto CryptoService)
        {
            _config = config;
            _crypto = CryptoService;
        }
       
        public async Task<Output<ValidatePaymentResponse>> ValidatePayment(ValidatePaymentRequest obj, string OriginalEncSalt)
        {

            Output<ValidatePaymentResponse> dto = new Output<ValidatePaymentResponse>();

            var uri = _config.GetValue<string>("HooghlyPay:HooghlyPayURL");
            var url = uri;
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            try
            {
               
                obj.hash = ComputeHash(obj, OriginalEncSalt);

                obj.hash = obj.hash.ToUpper();
                var jsonString = JsonConvert.SerializeObject(obj);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var ss = response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Output<ValidatePaymentResponse>>(ss.Result);
                    dto = result;
                }
                else
                {
                    var ss = response.Content.ReadAsStringAsync();
                    var resps = ss.Result;
                    var resultse = JsonConvert.SerializeObject(resps.ToString());

                }

            }
            catch (HttpRequestException ex)
            {
                dto = new Output<ValidatePaymentResponse>();

                Log.Error($"{(ex.Message != null ? ex.Message.ToString() : "") + "Trace = " + (ex.StackTrace != null ? ex.StackTrace.ToString() : "")}{Environment.NewLine}");
            }

            return dto;

        }
        private string ComputeHash(ValidatePaymentRequest _req, string OriginalEncSalt)
        {

            string datatoComputeHash = $"{_req.amount}{_req.authKey}{_req.currency}{_req.merchantCode}{_req.pc}{_req.referenceID}{_req.sourceCurrency}{_req.timeStamp}{_req.tunnel}{_req.userReference}";

            return _crypto.GetHashValue(datatoComputeHash, _crypto.DecryptedString(OriginalEncSalt,
                                                                                     _config.GetValue<string>("Secrekeys:masterKey"),
                                                                                     _config.GetValue<string>("Secrekeys:masterIV")));

        }
    }
}
