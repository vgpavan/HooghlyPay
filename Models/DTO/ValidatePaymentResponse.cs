using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.Models.DTO
{
    public class ValidatePaymentResponse
    {
        public string redirectURL { get; set; }
        public string tokenID { get; set; }
        public string PaymentType { get; set; }
        public double OrderAmount { get; set; }
        public double PaidAmount { get; set; }
        public double ServiceAmount { get; set; }
        public double CommissionAmount { get; set; }
        public int? MerchantID { get; set; }
        public string TempKey { get; set; }
    }
}
