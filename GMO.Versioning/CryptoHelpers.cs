using System;
using System.IO;
using System.Security.Cryptography;

namespace GMO.Versioning
{
    /// <summary> 
    /// Perform crypto calculations 
    /// </summary> 
    static class CryptoHelpers
    {
        /// <summary> 
        /// SHA 256 sum of input stream 
        /// </summary> 
        public static string GetSHASum(Stream input)
        {
            var hasher = new SHA256Managed();
            byte[] result = hasher.ComputeHash(input);

            string checkhash = BitConverter.ToString(result).Replace("-", "");

            return checkhash;
        }
    }
}
