using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.Models.DTO
{
    public class CheckOutReturnDTO
    {
        public string TrackID { get; set; }
        public string ReferenceID { get; set; }
        public string ErrorMessage { get; set; }
        public string Result { get; set; }
        public string PaidAmount { get; set; }
        public string Currency { get; set; }
        public string MerchantType { get; set; }
    }
}
