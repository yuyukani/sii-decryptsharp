using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    internal class BSIIUtils
    {
        internal static char[] CharTable = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '_' };
    }
    public interface IDataFragment
    {
        string SerializeToString();
    }


    public class SingleVector2
    {
        public Single A { get; set; }
        public Single B { get; set; }
    }

    public class SingleVector3 : SingleVector2
    {
        public Single C { get; set; }
    }

    public class SingleVector4 : SingleVector3
    {
        public Single D { get; set; }
    }

    public class SingleVector7 : SingleVector4
    {
        public Single E { get; set; }
        public Single F { get; set; }
        public Single G { get; set; }
    }

    public class SingleVector8 : SingleVector7
    {
        public Single H { get; set; }
    }

    public class Int32Vector2
    {
        public Int32 A { get; set; }
        public Int32 B { get; set; }
    }
    public class Int32Vector3 : Int32Vector2
    {
        public Int32 C { get; set; }
    }

    public class Int32Vector4 : Int32Vector3
    {
        public Int32 D { get; set; }
    }

    public class Int32Vector7 : Int32Vector4
    {
        public Int32 E { get; set; }
        public Int32 F { get; set; }
        public Int32 G { get; set; }
    }
    public class Int32Vector8 : Int32Vector7
    {
        public Int32 H { get; set; }
    }
    public class IDComplexType
    {
        public byte PartCount { get; set; }
        public UInt64 Address { get; set; }
        public string Value { get; set; }
    }

    public class BSII_Type_Decoder
    {
        internal static char[] CharTable = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '_' };
        
        //0x01
        public static string DecodeUTF8String(ref byte[] bytes, ref int offset)
        {
            var length = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = System.Text.Encoding.UTF8.GetString(bytes.Skip(offset).Take(length).ToArray());
            offset += length;
            return result;
        }
        //0x02
        public static string[] DecodeUTF8StringArray(ref byte[] bytes, ref int offset)
        {
            var numberOfStrings = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new string[numberOfStrings];
            for(int i = 0; i < numberOfStrings; i++)
            {
                result[i] = DecodeUTF8String(ref bytes, ref offset);
            }
            return result;
        }
        //0x03
        public static string DecodeUInt64String(ref byte[] bytes, ref int offset)
        {
            string result = "";
            var value = DecodeUInt64(ref bytes, ref offset);
            //value &= ~(1U << 63);
            while (value != 0)
            {
                int charIdx = (int)(value % 38);
                if (charIdx < 0) charIdx = charIdx * -1;
                charIdx -= 1;
                value = value / 38;
                if (charIdx > -1 && charIdx < 38)
                {
                    result += BSIIUtils.CharTable[charIdx];
                }
            }
            return result;
        }
        //0x04
        public static string[] DecodeUInt64StringArray(ref byte[] bytes, ref int offset)
        {
            var numberOfStrings = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new string[numberOfStrings];
            for(int i = 0; i < numberOfStrings; i++)
            {
                result[i] = DecodeUInt64String(ref bytes, ref offset);
            }
            return result;
        }
        //0x05
        public static Single DecodeSingle(ref byte[] bytes, ref int offset)
        {
            var result = BitConverter.ToSingle(bytes, offset);
            offset += sizeof(Single);
            return result;
        }
        //0x06
        public static Single[] DecodeSingleArray(ref byte[] bytes, ref int offset)
        {
            var numberOfSingles = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new Single[numberOfSingles];
            for(int i = 0; i < numberOfSingles; i++)
            {
                result[i] = DecodeSingle(ref bytes, ref offset);
            }
            return result;
        }
        //0x07
        public static SingleVector2 DecodeSingleVector2(ref byte[] bytes, ref int offset)
        {
            var result = new SingleVector2();
            result.A = DecodeSingle(ref bytes, ref offset);
            result.B = DecodeSingle(ref bytes, ref offset);
            return result;
        }
        //0x09
        public static SingleVector3 DecodeSingleVector3(ref byte[] bytes, ref int offset)
        {
            var result = new SingleVector3();
            result.A = DecodeSingle(ref bytes, ref offset);
            result.B = DecodeSingle(ref bytes, ref offset);
            result.C = DecodeSingle(ref bytes, ref offset);
            return result;
        }
        //0x0A
        public static SingleVector3[] DecodeSingleVector3Array(ref byte[] bytes, ref int offset)
        {
            var numberOfVector3s = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new SingleVector3[numberOfVector3s];
            for(int i = 0; i < numberOfVector3s; i++)
            {
                result[i] = DecodeSingleVector3(ref bytes, ref offset);
            }
            return result;
        }
        //0x11
        public static Int32Vector3 DecodeInt32Vector3(ref byte[] bytes, ref int offset)
        {
            var result = new Int32Vector3();
            result.A = DecodeInt32(ref bytes, ref offset);
            result.B = DecodeInt32(ref bytes, ref offset);
            result.C = DecodeInt32(ref bytes, ref offset);
            return result;
        }
        //0x12
        public static Int32Vector3[] DecodeInt32Vector3Array(ref byte[] bytes, ref int offset)
        {
            var numberOfVector3s = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new Int32Vector3[numberOfVector3s];
            for(int i = 0; i < numberOfVector3s; i++)
            {
                result[i] = DecodeInt32Vector3(ref bytes, ref offset);
            }
            return result;
        }
        //0x17
        public static SingleVector4 DecodeSingleVector4(ref byte[] bytes, ref int offset)
        {
            var result = new SingleVector4();
            result.A = DecodeSingle(ref bytes, ref offset);
            result.B = DecodeSingle(ref bytes, ref offset);
            result.C = DecodeSingle(ref bytes, ref offset);
            result.D = DecodeSingle(ref bytes, ref offset);
            return result;
        }
        //0x18
        public static SingleVector4[] DecodeSingleVector4Array(ref byte[] bytes, ref int offset)
        {
            var numberOfVector4s = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new SingleVector4[numberOfVector4s];
            for (int i = 0; i < numberOfVector4s; i++)
            {
                result[i] = DecodeSingleVector4(ref bytes, ref offset);
            }
            return result;
        }
        //0x19
        public static SingleVector7 DecodeSingleVector7(ref byte[] bytes, ref int offset)
        {
            var result = new SingleVector7();
            result.A = DecodeSingle(ref bytes, ref offset);
            result.B = DecodeSingle(ref bytes, ref offset);
            result.C = DecodeSingle(ref bytes, ref offset);
            result.D = DecodeSingle(ref bytes, ref offset);
            result.E = DecodeSingle(ref bytes, ref offset);
            result.F = DecodeSingle(ref bytes, ref offset);
            return result;
        }
        public static SingleVector8 DecodeSingleVector8(ref byte[] bytes, ref int offset)
        {
            var result = new SingleVector8();
            result.A = DecodeSingle(ref bytes, ref offset);
            result.B = DecodeSingle(ref bytes, ref offset);
            result.C = DecodeSingle(ref bytes, ref offset);
            result.D = DecodeSingle(ref bytes, ref offset);
            result.E = DecodeSingle(ref bytes, ref offset);
            result.F = DecodeSingle(ref bytes, ref offset);
            result.G = DecodeSingle(ref bytes, ref offset);
            result.H = DecodeSingle(ref bytes, ref offset);
            Int64 bias = (int)result.D;

            Int64 bits = bias;
            bits &= 0xFFF;
            bits -= 2048;
            bits = bits << 9;
            result.A += bits;

            Int64 bits2 = bias;
            bits2 = bits2 >> 12;
            bits2 &= 0xFFF;
            bits2 -= 2048;
            bits2 = bits2 << 9;

            result.C += bits2;
            return result;
        }


        //0x1A
        public static SingleVector7[] DecodeSingleVector7Array(ref byte[] bytes, ref int offset)
        {
            var numberOfVector7s = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new SingleVector7[numberOfVector7s];
            for (int i = 0; i < numberOfVector7s; i++)
            {
                result[i] = DecodeSingleVector7(ref bytes, ref offset);
            }
            return result;
        }
        public static SingleVector8[] DecodeSingleVector8Array(ref byte[] bytes, ref int offset)
        {
            var numberOfVector8s = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new SingleVector8[numberOfVector8s];
            for (int i = 0; i < numberOfVector8s; i++)
            {
                result[i] = DecodeSingleVector8(ref bytes, ref offset);
            }

            return result;
        }

        //0x25
        public static Int32 DecodeInt32(ref byte[] bytes, ref int offset)
        {
            Int32 result = BitConverter.ToInt32(bytes, offset);
            offset += sizeof(Int32);
            return result;
        }
        //0x26
        public static Int32[] DecodeInt32Array(ref byte[] bytes, ref int offset)
        {
            var numberOfInts = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new Int32[numberOfInts];
            for(int i = 0; i < numberOfInts; i++)
            {
                result[i] = DecodeInt32(ref bytes, ref offset);
            }
            return result;
        }
        //0x27 and 0x2F
        public static UInt32 DecodeUInt32(ref byte[] bytes, ref int offset)
        {
            UInt32 result = BitConverter.ToUInt32(bytes, offset);
            offset += sizeof(UInt32);
            return result;
        }
        //0x28
        public static UInt32[] DecodeUInt32Array(ref byte[] bytes, ref int offset)
        {
            var numberOfInts = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new UInt32[numberOfInts];
            for (int i = 0; i < numberOfInts; i++)
            {
                result[i] = DecodeUInt32(ref bytes, ref offset);
            }
            return result;
        }

        //0x2B
        public static UInt16 DecodeUInt16(ref byte[] bytes, ref int offset)
        {
            UInt16 result = BitConverter.ToUInt16(bytes, offset);
            offset += sizeof(UInt16);
            return result;
        }
        //0x2C
        public static UInt16[] DecodeUInt16Array(ref byte[] bytes, ref int offset)
        {
            var numberOfInts = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new UInt16[numberOfInts];
            for (int i = 0; i < numberOfInts; i++)
            {
                result[i] = DecodeUInt16(ref bytes, ref offset);
            }
            return result;
        }
        //0x31
        public static Int64 DecodeInt64(ref byte[] bytes, ref int offset)
        {
            Int64 result = BitConverter.ToInt64(bytes, offset);
            offset += sizeof(Int64);
            return result;
        }
        //0x33
        public static UInt64 DecodeUInt64(ref byte[] bytes, ref int offset)
        {
            UInt64 result = BitConverter.ToUInt64(bytes, offset);
            offset += sizeof(UInt64);
            return result;
        }
        //0x34
        public static UInt64[] DecodeUInt64Array(ref byte[] bytes, ref int offset)
        {
            var numberOfInts = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new UInt64[numberOfInts];
            for (int i = 0; i < numberOfInts; i++)
            {
                result[i] = DecodeUInt64(ref bytes, ref offset);
            }
            return result;
        }
        //0x35
        public static bool DecodeBool(ref byte[] bytes, ref int offset)
        {
            bool result = BitConverter.ToBoolean(bytes, offset);
            offset += sizeof(bool);
            return result;
        }
        //0x36
        public static bool[] DecodeBoolArray(ref byte[] bytes, ref int offset)
        {
            var numberOfBools = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new bool[numberOfBools];
            for (int i = 0; i < numberOfBools; i++)
            {
                result[i] = DecodeBool(ref bytes, ref offset);
            }
            return result;
        }
        //0x37
        public static Dictionary<UInt32, string> DecodeOrdinalStringList(ref byte[] bytes, ref int offset)
        {
            var length = DecodeUInt32(ref bytes, ref offset);
            Dictionary<uint, string> values = new Dictionary<uint, string>();
            for (int i = 0; i < length; i++)
            {
                var ordinal = DecodeUInt32(ref bytes, ref offset);
                values.Add(ordinal, DecodeUTF8String(ref bytes, ref offset));
            }
            return values;
        }
        public static string GetOrdinalStringFromValues(Dictionary<UInt32, string> values, ref byte[] bytes, ref int offset)
        {
            var index = DecodeUInt32(ref bytes, ref offset);
            if(values.Keys.Contains(index))
            {
                return values[index];
            }
            return "";
        }
        //0x39, 0x3B, 0x3D
        public static IDComplexType DecodeID(ref byte[] bytes, ref int offset)
        {
            var result = new IDComplexType();
            result.Value = "";
            result.PartCount = bytes.Skip(offset).Take(1).First();
            offset += 1;
            if(result.PartCount == 0xFF)
            {
                
                result.Address = DecodeUInt64(ref bytes, ref offset);
                var data = BitConverter.GetBytes(result.Address);
                var parts = new string[data.Length/2];
                var currentPart = "";
                for(int i = 0; i < data.Length; i++)
                {
                    if (i % 2 == 0 && i > 0)
                    {

                        if (i >= data.Length - 2)
                        {
                            while (currentPart.StartsWith("0"))
                            {
                                currentPart = currentPart.Substring(1);
                            }
                        }

                        if (!String.IsNullOrEmpty(currentPart))
                        {
                            result.Value = currentPart + "." + result.Value;
                        }
                        
                        parts[(data.Length/2) - (i / 2)] = currentPart;
                        currentPart = "";
                    }
                    currentPart = data[i].ToString("x2") + currentPart;
                    if (i == data.Length - 1)
                    {
                        while(currentPart.StartsWith("0"))
                        {
                            currentPart = currentPart.Substring(1);
                        }
                        if (!String.IsNullOrEmpty(currentPart))
                        {
                            result.Value = currentPart + "." + result.Value;
                        }
                        parts[0] = currentPart;
                        currentPart = "";
                    }
                }
                result.Value = "_nameless." + result.Value.Substring(0,result.Value.Length-1);
            }
            else
            {
                for(int i = 0; i < result.PartCount; i++)
                {
                    //UInt64 variable = DecodeUInt64(ref bytes, ref offset);
                    var s = DecodeUInt64String(ref bytes, ref offset);
                    
                    if (i > 0)
                        result.Value += ".";
                    result.Value += s;
                }
                if(result.PartCount == 0) {
                    result.Value = "null";
                }
                
            }
            return result;
        }
        //0x3A, 0x3C
        public static IDComplexType[] DecodeIDArray(ref byte[] bytes, ref int offset)
        {
            var numberOfIds = Convert.ToInt32(DecodeUInt32(ref bytes, ref offset));
            var result = new IDComplexType[numberOfIds];
            for(int i = 0; i < numberOfIds; i++)
            {
                result[i] = DecodeID(ref bytes, ref offset);
            }
            return result;
        }
    }
    /*public class BSII_Decoder_Format1 : IBSII_Decoder
    {
        //0x01
        public string DecodeUTF8String(ref byte[] bytes, ref int offset)
        {
            string result = string.Empty;
            result = StreamUtils.ReadChars(ref bytes, ref offset);
            return result;
        }

        //0x02
        public string[] DecodeUTF8StringArray(ref byte[] bytes, ref int offset)
        {
            List<string> result = new List<string>();
            var numberOfStrings = StreamUtils.ReadUInt32(ref bytes, ref offset);
            for(uint i = 0; i < numberOfStrings; i++)
            {
                result.Add(StreamUtils.ReadChars(ref bytes, ref offset));
            }
            return result.ToArray();
        }

        //0x03
        public string DecodeUInt64String(ref byte[] bytes, ref int offset)
        {
            string result = "";
            var value = StreamUtils.ReadUInt32(ref bytes, ref offset);
            var valueBytes = BitConverter.GetBytes(value);
            valueBytes = valueBytes.Take(valueBytes.Length - 1).ToArray();
            var realValue = BitConverter.ToUInt64(valueBytes, 0);
            while (realValue != 0)
            {
                var modulus = (decimal)(realValue % 38);
                var charIdx = (int)Math.Abs(modulus);
                realValue = (UInt64)(realValue / 38);
                if (charIdx > -1 && charIdx < 38)
                {
                    result += BSIIUtils.CharTable[charIdx];
                }
            }
            return result;
        }
        //0x04
        public string[] DecodeUInt64StringArray(ref byte[] bytes, ref int offset)
        {
            var result = new List<string>();
            var numberOfStrings = StreamUtils.ReadUInt32(ref bytes, ref offset);
            for(uint i = 0; i < numberOfStrings; i++)
            {
                result.Add(DecodeUInt64String(ref bytes, ref offset));
            }
            return result.ToArray();
        }
        //0x05
        public float DecodeFloat(ref byte[] bytes, ref int offset)
        {
            return StreamUtils.ReadFloat(ref bytes, ref offset);
        }
        //0x06
        public float[] DecodeFloatArray(ref byte[] bytes, ref int offset)
        {
            var result = new List<float>();
            var numberOfFloats = StreamUtils.ReadUInt32(ref bytes, ref offset);
            for(uint i = 0; i < numberOfFloats; i++)
            {
                result.Add(StreamUtils.ReadFloat(ref bytes, ref offset));
            }
            return result.ToArray();
        }

    }

    public class BSII_Decoder_Format2: BSII_Decoder_Format1
    {

    }
    public class BSII_Decoder_Format3 : BSII_Decoder_Format2
    {

    }*/
}
