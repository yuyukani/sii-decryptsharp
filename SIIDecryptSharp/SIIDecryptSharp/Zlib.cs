using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SIIDecryptSharp
{
    // RFC1950(zlib) to RFC1951(deflate)
    public class ZlibStream : DeflateStream
    {
        public ZlibStream(Stream stream, CompressionMode mode) : base(stream, mode)
        {
            if (mode == CompressionMode.Decompress)
            {
                byte[] buffer = new byte[2];
                stream.Read(buffer, 0, 2);

                var cm = (buffer[0] & 0x0f);
                if (cm != 8) throw new ArgumentOutOfRangeException();
                var cinfo = (buffer[0] & 0xf0) >> 4;

                var fdict = (buffer[1] & 0x20) >> 5;
                if (fdict != 0) throw new ArgumentOutOfRangeException();

                var flevel = (buffer[1] & 0xc0) >> 6;

                // checksum
                if ((buffer[0] * 256 + buffer[1]) % 31 != 0) throw new ArgumentOutOfRangeException();
            }
            else if (mode == CompressionMode.Compress)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class Zlib
    {
        /*
        [DllImport("zlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int uncompress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);

        [DllImport("zlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);

        [DllImport("zlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int compressBound(uint sourceLen);
        */
        public static int uncompress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(sourceBuffer, 0, (int)sourceLen);
            ms.Position = 0;
            ZlibStream ds = new ZlibStream(ms, CompressionMode.Decompress);
            MemoryStream mso = new MemoryStream();
            ds.CopyTo(mso);
            if (destLen > mso.Length)
            {
                destLen = (uint)mso.Length;
            }
            Array.Copy(mso.ToArray(), destBuffer, destLen);
            return 0;
        }

    }
}
