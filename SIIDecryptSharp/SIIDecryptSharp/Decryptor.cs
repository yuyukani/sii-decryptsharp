using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SIIDecryptSharp
{
    public class SII_Header
    {
        public UInt32 Signature;
        public UInt32 DataSize;

        public SII_Header() {
            Signature = 999;
            DataSize = 0;
        }
    }
    public class SII_Data
    {
        public SII_Header Header;
        public byte[] Data;

        public SII_Data()
        {
            Header = new SII_Header();
            Data = new byte[0];
        }
    }
    public class Decryptor
    {
        public static byte[] SII_Key = new byte[]
        {
            0x2a, 0x5f, 0xcb, 0x17, 0x91, 0xd2, 0x2f, 0xb6, 0x02, 0x45, 0xb3, 0xd8, 0x36, 0x9e, 0xd0, 0xb2,
            0xc2, 0x73, 0x71, 0x56, 0x3f, 0xbf, 0x1f, 0x3c, 0x9e, 0xdf, 0x6b, 0x11, 0x82, 0x5a, 0x5d, 0x0a
        };

        public static SII_Data Decrypt(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);

            int streamPos = 0;

            if (bytes.Length >= streamPos + sizeof(UInt32))
            {
                var fileType = BitConverter.ToUInt32(bytes, streamPos);
                streamPos += sizeof(UInt32);

                switch (fileType)
                {
                    case ((UInt32)SignatureType.Encrypted):
                        return Decrypt(ref bytes, streamPos);
                    case ((uint)SignatureType.PlainText):
                        return DecodePlaintext(ref bytes, streamPos);
                    case ((uint)SignatureType.Binary):
                        return DecodeBinary(ref bytes, streamPos);
                    case ((uint)SignatureType._3nK):
                        break;
                }
            }
            else
            {
                throw new Exception("Not enough data");
            }
            return new SII_Data();
        }

        private static SII_Data Decrypt(ref byte[] encrypted, int offset)
        {
            StringBuilder stringBuilder = new StringBuilder();

            List<byte> decrypted = new List<byte>();

            SII_Header header = new SII_Header();

            byte[] HMAC = new byte[0];
            byte[] IV = new byte[0];

            var streamPos = 0;

            if (encrypted.Length - streamPos >= sizeof(UInt32))
            {
                header.Signature = BitConverter.ToUInt32(encrypted, streamPos);
                streamPos += sizeof(UInt32);
            }
            if (encrypted.Length - streamPos >= 32)
            {
                HMAC = encrypted.Skip(streamPos).Take(32).ToArray();
                streamPos += 32;
            }
            if (encrypted.Length - streamPos >= 16)
            {
                IV = encrypted.Skip(streamPos).Take(16).ToArray();
                streamPos += 16;
            }
            if (encrypted.Length - streamPos >= sizeof(UInt32))
            {
                header.DataSize = BitConverter.ToUInt32(encrypted, streamPos);
                streamPos += sizeof(UInt32);
            }


            var finalEncrypted = encrypted.Skip(streamPos);

            using (Aes aes = Aes.Create())
            {
                aes.Key = SII_Key;
                aes.IV = IV;

                ICryptoTransform decipher = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(finalEncrypted.ToArray()))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decipher, CryptoStreamMode.Read))
                    {
                        byte[] decryptBuff = new byte[finalEncrypted.Count()];
                        cs.Read(decryptBuff, 0, decryptBuff.Length);
                        decrypted.AddRange(decryptBuff);
                    }
                }
            }

            return new SII_Data() { Data = decrypted.ToArray(), Header = header };
        }

        private static SII_Data DecodePlaintext(ref byte[] data, int offset)
        {
            var result = new SII_Data();

            result.Header = new SII_Header();
            result.Header.DataSize = (uint)data.Length;
            result.Header.Signature = (uint)SignatureType.PlainText;
            result.Data = data;
            return result;
        }

        private static SII_Data DecodeBinary(ref byte[] data, int offset)
        {
            var result = new SII_Data();
            result.Header = new SII_Header();
            result.Header.DataSize = (uint)data.Length;
            result.Header.Signature = (uint)SignatureType.Binary;



            return result;
        }
    }
}