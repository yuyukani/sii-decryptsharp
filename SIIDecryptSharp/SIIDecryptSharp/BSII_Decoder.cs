using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIIDecryptSharp
{
    public enum DataTypeIdFormat
    {
        //string, char is 8 bits, not null terminated
        UTF8String=0x01,
        //array of type 0x01
        ArrayOfUTF8String=0x02,
        //string stored as a 64bit number, only lower 63 bits matter
        EncodedString=0x03,
        //array of type 0x03
        ArrayOfEncodedString=0x04,
        //4 byte float
        Single=0x05,
        //array of 4 byte float
        ArrayOfSingle=0x06,
        //2 4 byte floats
        VectorOf2Single=0x07,
        //3 4 byte floats
        VectorOf3Single=0x09,
        //Array of vectors of 3 4 byte floats (array of type 0x9)
        ArrayOfVectorOf3Single=0x0A,
        //3 4 byte signed integers
        VectorOf3Int32=0x11,
        //Array of vectors of 3 4 byte signed integers(array of type 0x11)
        ArrayOfVectorOf3Int32=0x12,
        //vector of 4 4 byte floats
        VectorOf4Single=0x17,
        //Array of vectors of 4 4 byte floats (array of type 0x17)
        ArrayOfVectorOf4Single=0x18,
        //Vector of 8 4 byte floats (format 1 is 7)
        VectorOf8Single=0x19,
        //Array of Vectors of 8 4 byte floats (array of 0x19 - format 1 is 7)
        ArrayOfVectorOf8Single=0x1A,
        //Signed 32-bit integer
        Int32=0x25,
        //Array of Int32
        ArrayOfInt32=0x26,
        //Unsigned 32-bit integer
        UInt32=0x27,
        //Array of UInt32
        ArrayOfUInt32=0x28,
        //Unsigned 16 bit integer
        UInt16=0x2B,
        //Array of UInt16
        ArrayOfUInt16=0x2C,
        //UInt32 (same as 0x27)
        UInt32Type2=0x2F,
        //64 bit signed integer
        Int64=0x31,
        //64 bit unsigned integer
        UInt64=0x33,
        //Array of 64 bit unsigned integers (array of type 0x33)
        ArrayOfUInt64=0x34,
        //8 bit bool - 0 = false, any other = true
        ByteBool=0x35,
        //Array of 8 bit bools (array of type 0x35)
        ArrayOfByteBool=0x36,
        //Orginal String
        OrdinalString=0x37,
        //Id complex type
        Id=0x39,
        //Array of Id
        ArrayOfIdA=0x3A,
        //Array of Id
        ArrayOfIdC=0x3C,
        //Id Complex type
        IdType2=0x3B,
        //Id complex type
        IdType3=0x3D
    }
    
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

        public IDComplexType ID { get; set; }
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

            var fileData = new BSII_Data();
            fileData.Header.Signature = BSII_Type_Decoder.DecodeUInt32(ref bytes, ref streamPos);
            fileData.Header.Version = BSII_Type_Decoder.DecodeUInt32(ref bytes, ref streamPos);

            if (fileData.Header.Version != (uint)BSII_Supported_Versions.Version1 && fileData.Header.Version != (uint)BSII_Supported_Versions.Version2 && fileData.Header.Version != (uint)BSII_Supported_Versions.Version3)
            {
                throw new Exception("BSII version not supported");
            }
            
            BSII_StructureBlock currentBlock = new BSII_StructureBlock();
            UInt32 blockType = 0;
            List<Tuple<uint, string>> blocks = new List<Tuple<uint, string>>();
            do
            {
                blockType = BSII_Type_Decoder.DecodeUInt32(ref bytes, ref streamPos);

                if (blockType == 0)
                {
                    currentBlock = new BSII_StructureBlock();
                    currentBlock.Type = blockType;
                    currentBlock.Validity = BSII_Type_Decoder.DecodeBool(ref bytes, ref streamPos);
                    if (!currentBlock.Validity)
                    {
                        fileData.Blocks.Add(currentBlock);
                        continue;
                    }

                    currentBlock.StructureId = BSII_Type_Decoder.DecodeUInt32(ref bytes, ref streamPos);
                    currentBlock.Name = BSII_Type_Decoder.DecodeUTF8String(ref bytes, ref streamPos);
                    
                    BSII_DataSegment segment = new BSII_DataSegment();
                    segment.Type = 999;

                    while (segment.Type != 0)
                    {
                        segment = ReadDataBlock(ref bytes, ref streamPos);
                        currentBlock.Segments.Add(segment);
                    }
                    fileData.Blocks.Add(currentBlock);
                    blocks.Add(new Tuple<uint, string>(currentBlock.StructureId, currentBlock.Name));
                    Debug.WriteLine("Done reading segments in struct id: " + currentBlock.StructureId.ToString());
                }
                else
                {
                    var blockData = fileData.Blocks.Where(x => x.StructureId == blockType).First();
                    LoadDataBlockLocal(ref bytes, ref streamPos, ref blockData, fileData.Header.Version);
                    fileData.Blocks.RemoveAll(x => x.StructureId == blockType);
                    fileData.Blocks.Add(blockData);
                }
            } while (streamPos < bytes.Length);

            Debug.WriteLine("DECODED!");

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
                segment.Name = StreamUtils.ReadChars(ref bytes, ref streamPos);
                if(segment.Type == 0x37)
                {
                    segment.Value = StreamUtils.ReadUInt32(ref bytes, ref streamPos);
                }
                block.Segments.Add(segment);

            } while (ValueType != 0);
            return true;
        }

        private static bool LoadDataBlockLocal(ref byte[] bytes, ref int streamPos, ref BSII_StructureBlock segment, uint formatVersion)
        {
            segment.ID = BSII_Type_Decoder.DecodeID(ref bytes, ref streamPos);

            for(int i = 0; i < segment.Segments.Count; i ++)
            {
                var dataType = (int)segment.Segments[i].Type;
                switch(dataType)
                {
                     case (int)DataTypeIdFormat.ArrayOfByteBool:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeBoolArray(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfEncodedString:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt64StringArray(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfIdA:
                    case (int)DataTypeIdFormat.ArrayOfIdC:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeIDArray(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfInt32:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeInt32Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfSingle:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleArray(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfUInt16:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt16Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfUInt32:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt32Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfUInt64:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt64Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfUTF8String:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUTF8StringArray(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfVectorOf3Int32:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeInt32Vector3Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfVectorOf3Single:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector3Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfVectorOf4Single:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector4Array(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.ArrayOfVectorOf8Single:
                        if(formatVersion == 1)
                        {
                            segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector7Array(ref bytes, ref streamPos);
                        }
                        else
                        {
                            segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector8Array(ref bytes, ref streamPos);
                        }
                        break;
                    case (int)DataTypeIdFormat.ByteBool:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeBool(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.EncodedString:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt64String(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.IdType3:
                    case (int)DataTypeIdFormat.IdType2:
                    case (int)DataTypeIdFormat.Id:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeID(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.Int32:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeInt32(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.Int64:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeInt64(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.UInt32Type2:
                    case (int)DataTypeIdFormat.UInt32:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt32(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.UInt64:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt64(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.UInt16:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUInt16(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.OrdinalString:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeOrdinalString(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.Single:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingle(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.UTF8String:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeUTF8String(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.VectorOf2Single:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector2(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.VectorOf3Int32:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeInt32Vector3(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.VectorOf3Single:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector3(ref  bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.VectorOf4Single:
                        segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector4(ref bytes, ref streamPos);
                        break;
                    case (int)DataTypeIdFormat.VectorOf8Single:
                        if (formatVersion == 1)
                        {
                            segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector7(ref bytes, ref streamPos);
                        }
                        else
                        {
                            segment.Segments[i].Value = BSII_Type_Decoder.DecodeSingleVector8(ref bytes, ref streamPos);
                        }
                        break;
                    case 0:
                        continue;
                    default:
                        Debug.WriteLine("UNKNOWN TYPE: " + dataType);
                        break;

                }
            }

            return true;
        }

        private static dynamic LoadDataBlockId(ref byte[] bytes, ref int streamPos)
        {
            var IDLength = StreamUtils.ReadUInt8(ref bytes, ref streamPos);
            if(IDLength == 0xFF)//0xFF
            {
                var id = BSII_Type_Decoder.DecodeUInt64(ref bytes, ref streamPos);
                return id;
            }
            else
            {
                List<string> parts = new List<string>();
                for(int i = 0; i < IDLength; i++)
                {
                    parts.Add(BSII_Type_Decoder.DecodeUInt64String(ref bytes, ref streamPos));
                }
                return parts.ToArray();
            }
            
        }
        
        private static BSII_DataSegment ReadDataBlock(ref byte[] bytes, ref int streamPos)
        {
            var result = new BSII_DataSegment();
            result.Type = BSII_Type_Decoder.DecodeUInt32(ref bytes, ref streamPos);
            if (result.Type != 0)
            {
                result.Name = BSII_Type_Decoder.DecodeUTF8String(ref bytes, ref streamPos);
            }
            //IF THE TYPE IS 55
            if(result.Type == (uint)DataTypeIdFormat.OrdinalString)
            {
                //READ THE ORDINAL STRING NOW
                result.Value = BSII_Type_Decoder.DecodeOrdinalString(ref bytes, ref streamPos);
            }
            return result;
        }
    }
}
