using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class BSII_DataBlock
    {
        public UInt32 Type { get; set; }
        public UInt32 StructureId { get; set; }

        public bool Validity { get; set; }
        public string Name { get; set; }
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

            if(header.Version != (uint)BSII_Supported_Versions.Version1 && header.Version != (uint)BSII_Supported_Versions.Version2 && header.Version != (uint)BSII_Supported_Versions.Version3)
            {
                throw new Exception("BSII version not supported");
            }

            StringBuilder output = new StringBuilder();
            output.AppendLine("SiiNunit");
            output.AppendLine("{");
            bool inStruct = false;
            while(streamPos < bytes.Length)
            {
                if(!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 blockType))
                {
                    Debug.WriteLine("ERROR READING BLOCK TYPE");
                    return;
                }

                if(blockType == 0)
                {
                    //enter or exit struct block
                    inStruct = !inStruct;
                    if(inStruct)
                    {
                        if(!StreamUtils.TryReadBool(ref bytes, ref streamPos, out bool valid))
                        {
                            Debug.WriteLine("ERROR INSIDE STRUCT");
                            return;
                        }
                        if(!valid)
                        {
                            Debug.WriteLine("End of file");
                            break;
                        }


                    }
                }
            }
            output.AppendLine("}");

        }

        private static void ReadBlock(ref byte[] bytes, ref int streamPos)
        {
            UInt32 blockType = 0;
            bool valid = false;
            UInt32 structType = 0;
            UInt32 nameLength = 0;
            string name = "";
        }

        private static void ReadDataBlock(ref byte[] bytes, ref int streamPos)
        {
            UInt32 blockType = 0;
            UInt32 nameLength = 0;
            string name = "";
        }
    }
}
