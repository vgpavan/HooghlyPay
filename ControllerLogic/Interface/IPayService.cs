using HooghlyPay.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.ControllerLogic.Interface
{
    public interface IPayService
    {
        Task<Output<ValidatePaymentResponse>> ValidatePayment(ValidatePaymentRequest obj, string OriginalEncSalt);
    }
}
