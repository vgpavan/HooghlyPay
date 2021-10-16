using HooghlyPay.API.ControllerLogic.Interface;
using HooghlyPay.API.Models.DTO;
using HooghlyPay.API.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HooghlyPay.API.Controllers
{
    public class PayController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ICrypto _crypto;
        private readonly IPayService _payService;
        public PayController(IConfiguration config, ICrypto CryptoService, IPayService payService)
        {

            _config = config;
            _crypto = CryptoService;
            _payService = payService;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetPayDetails()
        {
            LoginViewModel model = new LoginViewModel();
            model.ValidatePaymentRequest = new ValidatePaymentRequest();
            model.ValidatePaymentRequest.billingDetails = new billing();
            return View("Pay", model);
        }
        [HttpPost("ValidatePayment")]
        public async Task<string> ValidatePayment(LoginViewModel obj)
        {
            LoginViewModel model = new LoginViewModel();
            Output<ValidatePaymentResponse> dto = new Output<ValidatePaymentResponse>();
            string OriginalEncSalt = "";

            try
            {

                obj.ValidatePaymentRequest.authKey = _config.GetValue<string>("HooghlyPay:AuthKey");
                obj.ValidatePaymentRequest.merchantCode = _config.GetValue<string>("HooghlyPay:MerchantCode");
                obj.ValidatePaymentRequest.referenceID = Convert.ToInt64(RandomNumber(15));
                obj.ValidatePaymentRequest.userReference = Convert.ToInt64(RandomNumber(15));
                obj.ValidatePaymentRequest.timeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss tt");
                obj.ValidatePaymentRequest.callbackURL = _config.GetValue<string>("HooghlyPay:CallBackURL");
                obj.ValidatePaymentRequest.language = "en";
                obj.ValidatePaymentRequest.pc = "All";
                obj.ValidatePaymentRequest.currency = "INR";
                obj.ValidatePaymentRequest.tunnel = _config.GetValue<string>("HooghlyPay:defaulttunnel");
                TempData["OriginalEncSalt"] = _config.GetValue<string>("HooghlyPay:OriginalSalt");
                if (obj.VendorDetails.payoutIFSCcode != null)
                {
                    obj.ValidatePaymentRequest.vendorDetails = new List<Vendor>();
                    Vendor ven = new Vendor();
                    ven.payoutReferenceID = Convert.ToInt64(RandomNumber(15));
                    obj.VendorDetails.payoutReferenceID = ven.payoutReferenceID;
                    if (obj.VendorDetails.payoutAmount == null)
                    {
                        obj.VendorDetails.payoutAmount = _config.GetValue<double>("HooghlyPay:defaultamt");
                        obj.ValidatePaymentRequest.amount = _config.GetValue<double>("HooghlyPay:defaultamt");
                    }
                    ven.payoutAmount = obj.VendorDetails.payoutAmount;
                    obj.ValidatePaymentRequest.amount = obj.VendorDetails.payoutAmount;
                    ven.payoutBankAccount = obj.VendorDetails.payoutBankAccount;
                    ven.payoutEmail = obj.VendorDetails.payoutEmail;
                    ven.payoutIFSCcode = obj.VendorDetails.payoutIFSCcode;
                    ven.payoutName = obj.VendorDetails.payoutName;
                    ven.payoutPhone = obj.VendorDetails.payoutPhone;
                    ven.payoutPurpose = obj.VendorDetails.payoutPurpose;
                    obj.ValidatePaymentRequest.vendorDetails.Add(ven);
                }
                OriginalEncSalt = _config.GetValue<string>("HooghlyPay:OriginalSalt");

                if (obj.ValidatePaymentRequest.billingDetails == null)
                {
                    obj.ValidatePaymentRequest.billingDetails = new billing();
                }
                dto = await _payService.ValidatePayment(obj.ValidatePaymentRequest, OriginalEncSalt);
                if (dto != null)
                {
                    if (dto.Result != null)
                    {
                        var amount = dto.Result.PaidAmount;
                        TempData["PdAmount"] = amount.ToString();                        
                        var currency = obj.ValidatePaymentRequest.currency;
                        TempData["currency"] = currency;
                        
                    }
                }

                if (dto.ErrorCode.ToString() != "0")
                {
                    return dto.ErrorMessgae;
                }
                else
                {

                    string dots = dto.Result.redirectURL != "" ? dto.Result.redirectURL + "✌" + dto.ErrorCode : "";
                    return dto.Result.redirectURL != "" ? dto.Result.redirectURL + "✌" + dto.ErrorCode : "";
                }

            }
            catch (Exception ex)
            {
                Log.Error($"{Environment.NewLine}{Environment.NewLine}{HttpContext.Request.Scheme} {HttpContext.Request.Host}{HttpContext.Request.Path} {HttpContext.Request.QueryString}{Environment.NewLine}{(ex.Message != null ? ex.Message.ToString() : "") + "Trace = " + (ex.StackTrace != null ? ex.StackTrace.ToString() : "")}{Environment.NewLine}");
            }

            return dto.ErrorMessgae;
        }
        public async Task<IActionResult> CheckOutReturn(String trackid, String result, String refid, String errormessage, String Hash, string amount, string Currency)
        {
            LoginViewModel lvm = new LoginViewModel();
            CheckOutReturnDTO co = new CheckOutReturnDTO();
            try
            {
                Log.Information("CheckOutReturn Start:" + "Started");
                string Original = _config.GetValue<string>("HooghlyPay:OriginalSalt");
                #region Genral

                String outParams = $"trackid={trackid}&result={result}&refid={refid}";
                Log.Information("outParams:" + outParams);
                Log.Information("masterKey :" + _config.GetValue<string>("Secrekeys:masterKey"));
                Log.Information("masterIV :" + _config.GetValue<string>("Secrekeys:masterIV"));
                String outhashValue = _crypto.GetHashValue(outParams, _crypto.DecryptedString(Original,
                                                                                         _config.GetValue<string>("Secrekeys:masterKey"),
                                                                                         _config.GetValue<string>("Secrekeys:masterIV")));
                #endregion
                if (Hash != outhashValue)
                    TempData["CheckOutError"] = "Tampered";
                else
                {
                    
                    Log.Information("PaidAmount :" + amount);
                    Log.Information("Currency :" + Currency);
                    co.TrackID = trackid;
                    co.ReferenceID = refid;
                    co.Result = result;
                    co.ErrorMessage = errormessage;
                    co.PaidAmount = amount;
                    co.Currency = Currency;

                    lvm.CheckOutReturnDTO = co;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"{Environment.NewLine}{Environment.NewLine}{HttpContext.Request.Scheme} {HttpContext.Request.Host}{HttpContext.Request.Path} {HttpContext.Request.QueryString}{Environment.NewLine}{(ex.Message != null ? ex.Message.ToString() : "") + "Trace = " + (ex.StackTrace != null ? ex.StackTrace.ToString() : "")}{Environment.NewLine}");
            }
            return View("CheckOutReturn", lvm);
        }
        public string RandomNumber(int KeyLength)
        {
            String a = "123456789";
            Char[] chars = new Char[(a.Length)];
            chars = a.ToCharArray();
            byte[] data = new byte[(KeyLength)];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(KeyLength);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
