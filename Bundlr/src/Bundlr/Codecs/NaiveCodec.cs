using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Bundlr.Core;

//<number of entries>
//<keylength><key><valuelength><value><keylength><key><valuelength><value>…
namespace Bundlr.Codecs
{
    public class NaiveCodec : ICodec
    {
        private const ushort ChecksumSizeMax = 16;
        public byte[] Encode(Message message)
        {
            var payloadSize = (uint)message.Payload.Length;
            var headerSize = (byte)message.Headers.Count;
            var checksumSize = ChecksumSizeMax;
                
            var buffer = new byte[0];
            
            buffer = Combine(buffer, new [] {headerSize});
            buffer = Combine(buffer, BitConverter.GetBytes(payloadSize));
            buffer = Combine(buffer, BitConverter.GetBytes(checksumSize));

            var header = message.Headers;
            
            foreach (var key in header.Keys)
            {
                var keyInBytes = Encoding.UTF8.GetBytes(key);
                var keyLengthInBytes = BitConverter.GetBytes(keyInBytes.Length);
                
                if(keyLengthInBytes.Length > Constants.HEADER_ITEM_BYTE_LENGTH_MAX)
                    throw new EncodeException("header key exceeds specification");
                
                var valueInBytes = Encoding.UTF8.GetBytes(header[key]);
                var valueLengthInBytes = BitConverter.GetBytes(valueInBytes.Length);
                
                if(valueLengthInBytes.Length > Constants.HEADER_ITEM_BYTE_LENGTH_MAX)
                    throw new EncodeException("header value exceeds specification");

                buffer = Combine(buffer, keyLengthInBytes, keyInBytes, valueLengthInBytes, valueInBytes);
            }

            buffer = Combine(buffer, message.Payload);
            
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(buffer);
                Console.WriteLine(hashBytes.Length);
                buffer = Combine(buffer, hashBytes);
                return buffer;
            }
        }

        public Message Decode(byte[] data)
        {
            var span = new Span<byte>(data);

            if (!IsValidChecksum(span))
              throw new DecodeException("the error code checks do not match");  
            

            //this would be so much cleaner with c# 8 range/index types
            var headerSize = span[0];
            var payloadSize = BitConverter.ToUInt32(span.Slice(sizeof(byte), sizeof(uint)));
            var checkSumSize = BitConverter.ToUInt16(span.Slice(sizeof(byte) + sizeof(uint), sizeof(ushort)));
            var headerStart = sizeof(byte) + sizeof(uint) + sizeof(ushort);

            var headers = GetHeaders(data, headerSize, headerStart);
            var payload = GetPayload(data, headers.position);

            Console.WriteLine(new Guid(payload));
            return new Message(headers.headers, payload);
        }


        internal static byte[] GetPayload(Span<byte> data, int headerPosition)
        {
            return data.Slice(headerPosition, data.Length - headerPosition - ChecksumSizeMax).ToArray();
        }

        private static (Dictionary<string, string> headers, int position) GetHeaders(
            Span<byte> data, 
            byte headerSize, 
            int headerStart)
        {
            var headers = new Dictionary<string,string>();
            var headerPosition = headerStart;
            for (byte i = 0; i < headerSize; i++)
            {
                var keyLength = BitConverter.ToInt32(data.Slice(headerPosition, sizeof(int)));
                var key = Encoding.UTF8.GetString(data.Slice(headerPosition + sizeof(int), keyLength));

                headerPosition = headerPosition + sizeof(int) + keyLength;
                
                var valueLength = BitConverter.ToInt32(data.Slice(headerPosition, sizeof(int)));
                var value = Encoding.UTF8.GetString(data.Slice(headerPosition + sizeof(int), valueLength));
                
                headerPosition = headerPosition + sizeof(int) + valueLength;

                headers[key] = value;
            }


            return (headers, headerPosition);
        }

        internal static bool IsValidChecksum(Span<byte> data)
        {
            using (MD5 md5 = MD5.Create())
            {
                var fromCompute = md5.ComputeHash(data.Slice(0,data.Length - ChecksumSizeMax).ToArray());
                var fromTailer = data.Slice(data.Length - ChecksumSizeMax, ChecksumSizeMax);
                return fromCompute.SequenceEqual(fromTailer.ToArray());
            }
        }
        
        internal static byte[] Combine(
            byte[] first,
            byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        internal static byte[] Combine(
            byte[] first,
            byte[] second,
            byte[] third,
            byte[] fourth,
            byte[] fifth)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length + fifth.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,third.Length);
            Buffer.BlockCopy(fourth, 0, ret, first.Length + second.Length + third.Length,fourth.Length);
            Buffer.BlockCopy(fifth, 0, ret, first.Length + second.Length + third.Length + fourth.Length,fifth.Length);
            return ret;
        }

    }
}