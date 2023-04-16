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
        public static int ReadInt8(ref byte[] bytes, ref int offset) {
            var result = (int)bytes.Skip(offset).Take(1).First();
            offset += 1;
            return result;
        }
        public static bool TryReadInt8(ref byte[] bytes, ref int offset, out int result)
        {
            try
            {
                result = (int)bytes.Skip(offset).Take(1).First();
                offset += 1;
                return true;
            }
            catch
            {
                result = 0;
            }
            return false;
        }

        public static string ReadChars(ref byte[] bytes, ref int offset, int length)
        {
            var result = System.Text.Encoding.UTF8.GetString(bytes.Skip(offset).Take(length).ToArray());
            offset += length;
            return result;
        }
        public static bool TryReadChars(ref byte[] bytes, ref int offset, int length, out string result)
        {
            try
            {
                result = System.Text.Encoding.UTF8.GetString(bytes.Skip(offset).Take(length).ToArray());
                offset += length;
                return true;
            }
            catch
            {
                result = string.Empty;
            }
            return false;
        }

        public static string ReadChars(ref byte[] bytes, ref int offset)
        {
            var length = (int)ReadUInt32(ref bytes, ref offset);
            return System.Text.Encoding.ASCII.GetString(bytes.Skip(offset).Take(length).ToArray());
        }
        public static bool TryReadChars(ref byte[] bytes, ref int offset, out string result)
        {
            try
            {
                var length = (int)ReadUInt32(ref bytes, ref offset);
                result = System.Text.Encoding.ASCII.GetString(bytes.Skip(offset).Take(length).ToArray());
                return true;
            }
            catch
            {
                result = string.Empty;
            }
            return false;
        }
    }
}
