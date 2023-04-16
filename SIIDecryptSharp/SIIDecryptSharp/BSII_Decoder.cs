using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    public class BSII_Data
    {
        public BSII_Header Header { get; set; }
        public List<BSII_StructureBlock> Blocks { get; set; }

        public BSII_Data()
        {
            Header = new BSII_Header();
            Blocks = new List<BSII_StructureBlock>();
        }
    }
    public class BSII_Header
    {
        public UInt32 Signature { get; set; }
        public UInt32 Version { get; set; }
    }

    public class BSII_StructureBlock
    {
        public UInt32 Type { get; set; }
        public UInt32 StructureId { get; set; }

        public bool Validity { get; set; }
        public string Name { get; set; }

        public List<BSII_DataSegment> Segments { get; set; }

        public BSII_StructureBlock()
        {
            Segments = new List<BSII_DataSegment>();
        }

        public dynamic ID { get; set; }
    }

    public class BSII_DataSegment
    {
        public string Name { get; set; }
        public UInt32 Type { get; set; }

        public dynamic Value { get; set; }
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
            int streamPos = 0;

            if(!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 headerSignature))
            {
                return;
            }

            if (!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 headerVersion))
            {
                return;
            }

            var fileData = new BSII_Data();
            fileData.Header.Signature = headerSignature;
            fileData.Header.Version = headerVersion;

            if (fileData.Header.Version != (uint)BSII_Supported_Versions.Version1 && fileData.Header.Version != (uint)BSII_Supported_Versions.Version2 && fileData.Header.Version != (uint)BSII_Supported_Versions.Version3)
            {
                throw new Exception("BSII version not supported");
            }

            BSII_StructureBlock currentBlock = new BSII_StructureBlock();
            bool keepReading = true;
            UInt32 blockType = 0;
            do
            {
                blockType = StreamUtils.ReadUInt32(ref bytes, ref streamPos);

                if (blockType != 0)
                {
                    //load data block local
                    var blockData = fileData.Blocks.Where(x => x.StructureId == blockType).First();
                    LoadDataBlockLocal(ref bytes, ref streamPos, ref blockData);
                    fileData.Blocks.RemoveAll(x=>x.StructureId == blockType);
                    fileData.Blocks.Add(blockData);
                    break;
                }
                else
                {
                    keepReading = LoadStructureBlockLocal(ref bytes, ref streamPos, out BSII_StructureBlock localBlock);
                    if(keepReading)
                    {
                        fileData.Blocks.Add(localBlock);
                    }
                }

                /*if (blockType == 0)
                {
                    //enter or exit struct block
                    inStruct = !inStruct;
                    if (inStruct)
                    {
                        currentBlock = new BSII_StructureBlock();
                        if (!StreamUtils.TryReadBool(ref bytes, ref streamPos, out bool valid))
                        {
                            Debug.WriteLine("ERROR INSIDE STRUCT");
                            return;
                        }
                        if (!valid)
                        {
                            Debug.WriteLine("End of file");
                            break;
                        }

                        if (!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 structId))
                        {
                            Debug.WriteLine("Struct Id Error");
                            return;
                        }

                        if (!StreamUtils.TryReadUInt32(ref bytes, ref streamPos, out UInt32 structNameLength))
                        {
                            Debug.WriteLine("Struct name length Error");
                            return;
                        }

                        if (!StreamUtils.TryReadChars(ref bytes, ref streamPos, (int)structNameLength, out string structName))
                        {
                            Debug.WriteLine("Struct name error");
                            return;
                        }
                        currentBlock.StructureId = structId;
                        currentBlock.Name = structName;
                        currentBlock.Validity = valid;
                        currentBlock.Type = blockType;

                        BSII_DataSegment segment = new BSII_DataSegment();
                        segment.Type = 999;

                        while (segment.Type != 0)
                        {
                            segment = ReadDataBlock(ref bytes, ref streamPos);
                            currentBlock.Segments.Add(segment);
                        }
                        inStruct = false;
                        fileData.Blocks.Add(currentBlock);
                        Debug.WriteLine("Done reading segments in struct id: " + structId.ToString());
                    }
                }*/
            } while (keepReading);

        }

        private static bool LoadStructureBlockLocal(ref byte[] bytes, ref int streamPos, out BSII_StructureBlock block)
        {
            block = new BSII_StructureBlock();
            block.Type = 0;
            block.Validity = StreamUtils.ReadBool(ref bytes, ref streamPos);
            if (!block.Validity) return false;

            block.StructureId = StreamUtils.ReadUInt32(ref bytes, ref streamPos);

            if (block.StructureId == 0) throw new Exception("Invalid block id");

            block.Name = StreamUtils.ReadChars(ref bytes, ref streamPos);

            UInt32 ValueType = 0;
            do
            {
                ValueType = StreamUtils.ReadUInt32(ref bytes, ref streamPos);
                if (ValueType == 0) break;

                BSII_DataSegment segment = new BSII_DataSegment();
                segment.Type = ValueType;
                var length2 = StreamUtils.ReadUInt32(ref bytes, ref streamPos);
                var trueLength2 = (int)length2;
                segment.Name = StreamUtils.ReadChars(ref bytes, ref streamPos, trueLength2);
                if(segment.Type == 0x37)
                {
                    segment.Value = StreamUtils.ReadUInt32(ref bytes, ref streamPos);
                }
                block.Segments.Add(segment);

            } while (ValueType != 0);
            return true;
        }

        private static bool LoadDataBlockLocal(ref byte[] bytes, ref int streamPos, ref BSII_StructureBlock segment)
        {
            segment.ID = LoadDataBlockId(ref  bytes, ref streamPos);

            for(int i = 0; i < segment.Segments.Count; i ++)
            {
                var dataType = segment.Segments[i].Type;
                switch(dataType)
                {

                }
            }

            return true;
        }

        private static dynamic LoadDataBlockId(ref byte[] bytes, ref int streamPos)
        {
            var IDLength = StreamUtils.ReadInt8(ref bytes, ref streamPos);
            if(IDLength == 255)//0xFF
            {
                var id = StreamUtils.ReadUInt64(ref bytes, ref streamPos);
                return id;
            }
            else
            {
                List<string> parts = new List<string>();
                for(int i = 0; i < IDLength; i++)
                {
                    UInt64 part = StreamUtils.ReadUInt64(ref bytes, ref streamPos);
                    parts.Add(DecoderUtils.DecodeUInt64String(part));
                }
                return parts.ToArray();
            }
            
        }
        
    }
}
