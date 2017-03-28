using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Core.Utils
{
    public class HashAlgorithmHelper<T>
        where T : HashAlgorithm, new()
    {
        public static string ComputeHash(string originalText, bool upper = true)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(originalText), upper);
        }

        public static string ComputeHash(byte[] data, bool upper = true)
        {
            using (T algorithm = new T())
            {
                var ret = algorithm.ComputeHash(data);
                StringBuilder tmp = new StringBuilder();
                for (int i = 0; i < ret.Length; i++)
                {
                    tmp.Append(ret[i].ToString(upper ? "X2" : "x2"));
                }
                return tmp.ToString();
            }
        }
    }
}
