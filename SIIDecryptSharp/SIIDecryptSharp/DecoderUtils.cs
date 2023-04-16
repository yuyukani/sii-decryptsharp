using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    public class DecoderUtils
    {
        private static char[] CharTable = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '_' };

        public static string DecodeUInt64String(UInt64 value)
        {
            string result = "";
            var bytes = BitConverter.GetBytes(value);
            bytes = bytes.Take(bytes.Length-1).ToArray();
            var realValue = BitConverter.ToUInt64(bytes,0);
            while(realValue != 0)
            {
                var modulus = (decimal)(realValue % 38);
                var charIdx = (int)Math.Abs(modulus);
                realValue = (UInt64)(realValue / 38);
                if(charIdx > -1 && charIdx < 38)
                {
                    result += CharTable[charIdx];
                }
            }
            return result;
        }
    }
}
