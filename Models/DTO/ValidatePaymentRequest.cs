using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.Models.DTO
{
    public class ValidatePaymentRequest
    {
        public string merchantCode { get; set; }
        public string authKey { get; set; }
        public string currency { get; set; }
        public string pc { get; set; }
        public string tunnel { get; set; }
        public double? amount { get; set; }
        public string doConvert { get; set; } // Y/N
        public string sourceCurrency { get; set; }  // Requires only when doConvert ="Y"
        public string description { get; set; }
        public long referenceID { get; set; }  // 15 digits number only
        public string timeStamp { get; set; }  // ddMMyyyyHHmmssfff

        public string language { get; set; }    //Standard Language codes
        public string callbackURL { get; set; }
        public long userReference { get; set; }
        public string hash { get; set; }
        public string cid { get; set; }
        public billing billingDetails { get; set; }
        public List<Vendor> vendorDetails { get; set; }

        //   public shipping shippingDetails { get; set; } // It will be included in Phase 2
        //  public order orderDetails { get; set; }       // It will be included in Phase 2

    }
    public class billing
    {
        public string fName { get; set; }
        public string lName { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string state { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
    }

    public class Vendor
    {
        public long? payoutReferenceID { get; set; }
        public double? payoutAmount { get; set; }
        public string payoutName { get; set; }
        public string payoutBankAccount { get; set; }
        public string payoutIFSCcode { get; set; }
        public string payoutPurpose { get; set; }
        public string payoutEmail { get; set; }
        public string payoutPhone { get; set; }
    }
}

