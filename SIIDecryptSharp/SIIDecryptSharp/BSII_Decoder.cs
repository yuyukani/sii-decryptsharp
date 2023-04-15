using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    public class BSII_Header
    {
        public UInt32 Signature { get; set; }
        public UInt32 Version { get; set; }
    }
    public enum BSII_Supported_Versions
    {
        Version1 = 1,
        Version2 = 2,
        Version3 = 3,
    }

    public class BSII_Decoder
    {
        public static void Decode(ref byte[] bytes)
        {
            if(bytes.Length < sizeof(UInt32)*2)
            {
                throw new Exception("Not enough data");
            }

            int streamPos = 0;

            if(!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 headerSignature))
            {
                return;
            }

            if (!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 headerVersion))
            {
                return;
            }

            var header = new BSII_Header();
            header.Signature = headerSignature;
            header.Version = headerVersion;



        }
    }
}
