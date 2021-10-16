using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HooghlyPay.API.ControllerLogic.Interface
{
    public interface ICrypto
    {
        string GetHashValue(string dataToComputeHash, string HashKey);
        string EncryptedValue(string plainText, string masterKey, string masterIV);
        string DecryptedString(string encryptedString, string masterKey, string masterIV);
    }
}
