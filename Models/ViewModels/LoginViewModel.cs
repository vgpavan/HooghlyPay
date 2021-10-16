using HooghlyPay.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.Models.ViewModels
{
    public class LoginViewModel
    {
        public ValidatePaymentRequest ValidatePaymentRequest { get; set; }
        public Vendor VendorDetails { get; set; }
        public billing billingDetails { get; set; }
        public CheckOutReturnDTO CheckOutReturnDTO { get; set; }
    }
}
