using HooghlyPay.API.ControllerLogic.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HooghlyPay.API.ControllerLogic.Implementaion
{
    public class Crypto: ICrypto
    {
        public Crypto()
        {

        }
        public string GetHashValue(String dataToComputeHash, String HashKey)
        {

            HMACSHA256 hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(HashKey));

            string computedHash = convertToHex(hmac.ComputeHash(System.Text.UTF8Encoding.Default.GetBytes(dataToComputeHash)));
            return computedHash;
        }
        private string convertToHex(byte[] data)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(data.Length);
            foreach (byte b in data)
                sb.AppendFormat("{0:X2}", (int)b);

            return sb.ToString();
        }

        public String EncryptedValue(String plainText, String masterKey, String masterIV)
        {


            using (Aes myAes = Aes.Create())
            {
                myAes.Key = System.Text.Encoding.UTF8.GetBytes(masterKey);
                myAes.IV = System.Text.Encoding.UTF8.GetBytes(masterIV);
                return
                   Convert.ToBase64String(EncryptStringToBytes_Aes(plainText, myAes.Key, myAes.IV));
            }
        }
        public String DecryptedString(String encryptedString, String masterKey, String masterIV)
        {


            using (Aes myAes = Aes.Create())
            {
                myAes.Key = System.Text.Encoding.UTF8.GetBytes(masterKey);
                myAes.IV = System.Text.Encoding.UTF8.GetBytes(masterIV);
                return DecryptStringFromBytes_Aes(Convert.FromBase64String(encryptedString), myAes.Key, myAes.IV);
            }
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an Aes object 
            // with the specified key and IV. 
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an Aes object 
            // with the specified key and IV. 
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }
        private String bytesToHexString(byte[] bytes)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bytes)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
    }
}
