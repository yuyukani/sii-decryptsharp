using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    public class StreamUtils
    {
        public static UInt32 ReadUInt32(ref byte[] bytes, ref int offset)
        {
            var result = BitConverter.ToUInt32(bytes, offset);
            offset += sizeof(UInt32);
            return result;
        }
        public static bool TryReadUInt32(ref byte[] bytes, ref int offset, out UInt32 result)
        {
            try
            {
                result = BitConverter.ToUInt32(bytes, offset);
                offset += sizeof(UInt32);
                return true;
            }
            catch
            {
                result = 0;
                
            }
            return false;
        }
        public static bool ReadBool(ref byte[] bytes, ref int offset)
        {
            var result = BitConverter.ToBoolean(bytes, offset);
            offset += sizeof(bool);
            return result;
        }
        public static bool TryReadBool(ref byte[] bytes, ref int offset, out bool result)
        {
            try
            {
                result = BitConverter.ToBoolean(bytes, offset);
                offset += sizeof(bool);
                return true;
            }
            catch
            {
                result = false;
            }
            return false;
        }
    }
}
