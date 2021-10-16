using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.Models.DTO
{
    public class Output<T>
    {
        public int ErrorCode { get; set; }
        public string ErrorMessgae { get; set; }
        public T Result { get; set; }
    }
}
