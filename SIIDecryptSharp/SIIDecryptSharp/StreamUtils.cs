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
        public static byte ReadUInt8(ref byte[] bytes, ref int offset) {
            var result = bytes.Skip(offset).Take(1).First();
            offset += 1;
            return result;
        }
        public static bool TryReadUInt8(ref byte[] bytes, ref int offset, out byte result)
        {
            try
            {
                result = (byte)bytes.Skip(offset).Take(1).First();
                offset += 1;
                return true;
            }
            catch
            {
                result = 0;
            }
            return false;
        }
        public static sbyte ReadInt8(ref byte[] bytes, ref int offset)
        {
            var result = Convert.ToSByte(bytes.Skip(offset).Take(1).First());
            offset += 1;
            return result;
        }
        public static bool TryReadInt8(ref byte[] bytes, ref int offset, out sbyte result)
        {
            try
            {
                result = Convert.ToSByte(bytes.Skip(offset).Take(1).First());
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
            var result = System.Text.Encoding.UTF8.GetString(bytes.Skip(offset).Take(length).ToArray());
            offset += length;
            return result;
        }
        public static bool TryReadChars(ref byte[] bytes, ref int offset, out string result)
        {
            try
            {
                var length = (int)ReadUInt32(ref bytes, ref offset);
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

        public static UInt64 ReadUInt64(ref byte[] bytes, ref int offset)
        {
            var result = BitConverter.ToUInt64(bytes, offset);
            offset += sizeof(UInt64);
            return result;
        }
        public static bool TryReadUInt64(ref byte[] bytes, ref int offset, out UInt64 result)
        {
            try
            {
                result = BitConverter.ToUInt64(bytes, offset);
                offset += sizeof(UInt64);
                return true;
            }
            catch
            {
                result = 0;
            }
            return false;
        }

        public static Dictionary<uint, string> ReadOrdinalStrings(ref byte[] bytes, ref int offset)
        {
            var length = ReadUInt32(ref bytes, ref offset);
            Dictionary<uint,string> values = new Dictionary<uint,string>();
            for(int i = 0; i < length; i++)
            {
                var ordinal = ReadUInt32(ref bytes, ref offset);
                values.Add(ordinal,ReadChars(ref bytes, ref offset));
            }
            return values;
        }
        public static Single ReadSingle(ref byte[] bytes, ref int offset)
        {
            var result = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(Single);
            return result;
        }
        public static bool TryReadSingle(ref byte[] bytes, ref int offset, out Single result)
        {
            try
            {
                result = BitConverter.ToSingle(bytes, offset);
                offset += sizeof(Single);
                return true;
            }
            catch
            {
                result = 0.00f;
            }
            return false;
        }
    }
}
