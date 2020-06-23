using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PipelineR
{
    public static class RequestExtension
    {

        public static string GenerateHash<TRequest>(this TRequest request)
        {

            var requestString = JsonConvert.SerializeObject(request);
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] key = encoding.GetBytes("072e77e426f92738a72fe23c4d1953b4");
            HMACSHA1 hmac = new HMACSHA1(key);
            Byte[] bytes = hmac.ComputeHash(encoding.GetBytes(requestString));
            Console.WriteLine(ByteArrayToString(bytes));
            var result = System.Convert.ToBase64String(bytes);

            return result;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba);
        }
    }
}
