using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    public class Zlib
    {
        [DllImport("zlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int uncompress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);

        [DllImport("zlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);

        [DllImport("zlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compressBound(uint sourceLen);
    }
}
